using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{


	int m_curr_player;
	int m_num_picked_cards;
	bool m_first_match_done;
	int m_curr_num_matches;
	bool m_last_pick_won;
	bool m_human_can_play;

	const float CARDS_PICK_DELAY_TIME = .75f;
	const float SEE_CARDS_TIME = .75f;
	const int MATCH_SCORE = 100;
	const int SHOW_ALL_SCORE = 200;
	const int FIRST_SCORE = 200;
	const int TURN_TIME = 3;

	public const int UNKNOWN = -1;
	public const int MATCHED = -100;

	public const int JOKER = 18;
	public const int SHOW_ALL = 19;
	public const int SHUFFLE = 20;
	public const int POINTS = 27;

	List<int> m_human_picked_cards_indexes = new List<int> ();

	static GameController m_instance;

	public static GameController 		Instance {
		get {

			if (m_instance == null) {
				GameController manager = GameObject.FindObjectOfType<GameController> ();
				m_instance = manager.GetComponent<GameController> ();
			}
			return m_instance;
		}
	}



	public void init ()
	{
		//pick who starts

		m_curr_player = 0;
		m_num_picked_cards = 0;
		m_curr_num_matches = 0;
		m_first_match_done = false;
		m_last_pick_won = false;
		m_human_can_play = false;

		PlayNextTurn ();
	}

	public void PlayNextTurn ()
	{
		if (m_last_pick_won && ManagerView.Instance.Winner_plays_on)
			Debug.Log ("Winner plays on");
		else
			m_curr_player++;
		
		if (m_curr_player == ManagerView.Instance.Num_players)
			m_curr_player = 0;


		if (ManagerView.Instance.Num_of_matches_needed != m_curr_num_matches) {

			if (ManagerView.Instance.Players_views [m_curr_player].Bot != null) {
				StartCoroutine (GenerateAndPlayBotMove (ManagerView.Instance.Cards_to_match));
			} else {
				m_human_picked_cards_indexes.Clear ();
				StartCoroutine (StartHumanTimer ());
			}
		} else {
			ManagerView.Instance.ShowSelectionGui ();
			Debug.Log ("Round Done");
		}
	}


	private IEnumerator StartHumanTimer ()
	{
		//unlock player picking cards and start timer and wait for the result to come
		Debug.Log ("Starting Human Play");
		m_human_can_play = true;
		m_num_picked_cards = 0;
		PlayerView player_view = ManagerView.Instance.Players_views [m_curr_player];

		player_view.Timer.fillAmount = 1;
		player_view.ShowHideTimer (true);

	
		bool time_expired = false;


		float f_start_time = Time.realtimeSinceStartup;

		while ((Time.realtimeSinceStartup < f_start_time + TURN_TIME) && (time_expired == false) && (m_num_picked_cards != ManagerView.Instance.Cards_to_match)) {
			float temp = (Time.realtimeSinceStartup - f_start_time) / TURN_TIME;

			player_view.Timer.fillAmount = 1 - temp;

			if (Time.realtimeSinceStartup - f_start_time > TURN_TIME)
				time_expired = true;

			yield return new WaitForSeconds (0.1f);
		}

		player_view.ShowHideTimer (false);

	}

	public void HumanPickedCard (int card_pos)
	{
		Debug.Log ("Human picked card index pos: " + card_pos);

		int special_picked = 0;

		if (ManagerView.Instance.Card_views_arr [card_pos].Image_index == SHOW_ALL) {
			StartCoroutine (ManagerView.Instance.ShowAllCards (m_human_picked_cards_indexes, card_pos));

			special_picked = SHOW_ALL;
			//give show all score
		}

		if (ManagerView.Instance.Card_views_arr [card_pos].Image_index == SHUFFLE) {

			//need to go over all the cards - close any open cards except for the shuffle 
			for (int i = 0; i < m_human_picked_cards_indexes.Count; i++)
				if (ManagerView.Instance.Card_views_arr [m_human_picked_cards_indexes[i]].Image_index != GameController.SHUFFLE) {
					ManagerView.Instance.Card_views_arr [m_human_picked_cards_indexes[i]].ShowCard (false);
				}

			m_human_picked_cards_indexes.Clear ();
			m_num_picked_cards=0;

			StartCoroutine (ManagerView.Instance.Shuffle (m_human_picked_cards_indexes, card_pos));
			special_picked = SHUFFLE;


			//give show all score
		}

		if (special_picked == 0) {
			m_num_picked_cards++;
			m_human_picked_cards_indexes.Add (card_pos);
		}

		if (m_num_picked_cards == ManagerView.Instance.Cards_to_match) {

			m_human_can_play = false;
			StartCoroutine (PostHumanPickCards ());
		}

	}

	private IEnumerator PostHumanPickCards ()
	{
		yield return new WaitForSecondsRealtime (1f);

		yield return new WaitForSecondsRealtime (SEE_CARDS_TIME);

		ScoreCurrentMove (CheckCurrentSelectionMatch (m_human_picked_cards_indexes));

		PlayNextTurn ();
	}

	private IEnumerator GenerateAndPlayBotMove (int num_cards_to_pick)
	{

		PlayerView player_view = ManagerView.Instance.Players_views [m_curr_player];

		player_view.Timer.fillAmount = 1;
		player_view.ShowHideTimer (true);
		if (ManagerView.Instance.Show_bot_debug)
			player_view.Bot.HighlightAllKnownCards (true);
		// Wait time:
		// For bid: Normal distribution with mean of 4 and the stddev of 1
		// For turn: Triangular distribution between 0 and 3 with most frequent value of 0.5
		float actual_wait_time = 0;
	
		actual_wait_time = Random.Range (0.5f, TURN_TIME / 2);


		bool time_expired = false;


		float f_start_time = Time.realtimeSinceStartup;

		while ((Time.realtimeSinceStartup < f_start_time + TURN_TIME) && (time_expired == false)) {
			float temp = (Time.realtimeSinceStartup - f_start_time) / TURN_TIME;

			player_view.Timer.fillAmount = 1 - temp;

			if (Time.realtimeSinceStartup - f_start_time > actual_wait_time)
				time_expired = true;

			yield return new WaitForSeconds (0.1f);
		}


		player_view.ShowHideTimer (false);

		List<int> picked_cards_indexes = new List<int> ();

		int already_picked_cards=0;

		while (already_picked_cards < num_cards_to_pick) 
		{
			int curr_pick = player_view.Bot.PickCards (num_cards_to_pick,already_picked_cards);

			int curr_pick_value = ManagerView.Instance.Card_views_arr [curr_pick].Image_index;

			ManagerView.Instance.Card_views_arr [curr_pick].ShowCard (true);
			yield return new WaitForSecondsRealtime (CARDS_PICK_DELAY_TIME);

			if (curr_pick_value == SHOW_ALL)
			{
				StartCoroutine (ManagerView.Instance.ShowAllCards (picked_cards_indexes, curr_pick));

				yield return new WaitForSecondsRealtime (2.25f);
			} 
			else if (curr_pick_value == POINTS)
			{
				ManagerView.Instance.Card_views_arr [curr_pick].Pos_index = GameController.MATCHED;
				ManagerView.Instance.Card_views_arr [curr_pick].CardMatchedAnim ();

				ManagerView.Instance.AddDataToAllBots (curr_pick,GameController.MATCHED);

				yield return new WaitForSecondsRealtime (0.25f);
				GameController.Instance.ScoreSpecialCurrentMove (GameController.SHOW_ALL);
			} 
			else if (curr_pick_value == SHUFFLE)
			{
				for (int i = 0; i < picked_cards_indexes.Count; i++)
					if (ManagerView.Instance.Card_views_arr [picked_cards_indexes[i]].Image_index != GameController.SHUFFLE) {
						ManagerView.Instance.Card_views_arr [picked_cards_indexes[i]].ShowCard (false);
					}

				picked_cards_indexes.Clear ();
				already_picked_cards=0;

				StartCoroutine (ManagerView.Instance.Shuffle (picked_cards_indexes, curr_pick));
				yield return new WaitForSecondsRealtime (1.25f);

			}
			else 
			{
				picked_cards_indexes.Add (curr_pick);
				already_picked_cards++;
			}

		}


		yield return new WaitForSecondsRealtime (SEE_CARDS_TIME);
		ScoreCurrentMove (CheckCurrentSelectionMatch (picked_cards_indexes));


		player_view.Bot.HighlightAllKnownCards (false);
		PlayNextTurn ();
	}

	private void ScoreCurrentMove (bool won)
	{
		m_last_pick_won = false;
		if (won) {
			m_last_pick_won = true;
			m_curr_num_matches++;
			Debug.Log ("Needed:" + ManagerView.Instance.Num_of_matches_needed + " Curr: " + m_curr_num_matches);
			int turn_score = 0;
			ManagerView.Instance.Players_views [m_curr_player].Multi++;

			if (!m_first_match_done) {
				m_first_match_done = true;
				turn_score += FIRST_SCORE;
			}
			turn_score += MATCH_SCORE;

			ManagerView.Instance.Players_views [m_curr_player].Score += turn_score * ManagerView.Instance.Players_views [m_curr_player].Multi;

		} else {

			ManagerView.Instance.Players_views [m_curr_player].Multi = 0;
		}
	}

	public void ScoreSpecialCurrentMove (int special)
	{
		m_last_pick_won = true;
		Debug.Log ("Show All Score");
		int turn_score = 0;
		ManagerView.Instance.Players_views [m_curr_player].Multi++;

		if (special == SHOW_ALL) {
			turn_score += SHOW_ALL_SCORE;
		}

		ManagerView.Instance.Players_views [m_curr_player].Score += turn_score * ManagerView.Instance.Players_views [m_curr_player].Multi;

	}


	private bool CheckCurrentSelectionMatch (List<int> picked_cards_indexes)
	{
		bool match = true;
		bool joker = false;


		int value_to_match = ManagerView.Instance.Card_views_arr [picked_cards_indexes [0]].Image_index;

		if (value_to_match == JOKER) {//if the first card is joker - need to start with second card
			joker = true;


			value_to_match = ManagerView.Instance.Card_views_arr [picked_cards_indexes [1]].Image_index;
		} 


		for (int i = 0; i < picked_cards_indexes.Count; i++) {

			int curr_card_value = ManagerView.Instance.Card_views_arr [picked_cards_indexes [i]].Image_index;

			if (curr_card_value != value_to_match && curr_card_value != JOKER) 
			{
				match = false;
			}

			if (curr_card_value == JOKER) {
				joker = true;

			}

		}

	
		if (match) {
			Debug.Log ("Match");
			//i have a match - either pure match or joker 
			//if joker match - need to find the other card and remove all 3 of them
			//if no joker - just remove the cards in the match

			if (joker) {
				//find the other card that is complimented by the Joker and add open it
				int other_card_to_match_value_index = 0;
				int other_card_to_match_value = 0;

				for (int i = 0; i < picked_cards_indexes.Count; i++)
				{
					if (ManagerView.Instance.Card_views_arr[picked_cards_indexes [i]].Image_index != JOKER) 
					{
						other_card_to_match_value_index = picked_cards_indexes [i];
						other_card_to_match_value = ManagerView.Instance.Card_views_arr [other_card_to_match_value_index].Image_index;
					}
				}
				for (int i = 0; i < ManagerView.Instance.Card_views_arr.Length; i++) {

					if (ManagerView.Instance.Card_views_arr [i].Image_index == other_card_to_match_value)
					if (i != other_card_to_match_value_index) {
						picked_cards_indexes.Add (i);

						if (ManagerView.Instance.Card_views_arr [i].Button.interactable)
							ManagerView.Instance.Card_views_arr [i].ShowCard (true);

					}
				}

			}


			for (int i = 0; i < picked_cards_indexes.Count; i++) {
				ManagerView.Instance.Card_views_arr [picked_cards_indexes [i]].Pos_index = MATCHED;

				ManagerView.Instance.Card_views_arr [picked_cards_indexes [i]].CardMatchedAnim ();

				ManagerView.Instance.AddDataToAllBots (picked_cards_indexes [i], MATCHED);

			}

		} 
		else 
		{
			// No Match - Closing Cards and consuming Joker
			for (int i = 0; i < picked_cards_indexes.Count; i++) {
				ManagerView.Instance.Card_views_arr [picked_cards_indexes [i]].ShowCard (false);

				ManagerView.Instance.AddDataToAllBots (picked_cards_indexes [i], ManagerView.Instance.Card_views_arr [picked_cards_indexes [i]].Image_index);

			}
		}

		return(match);
	}

	public void ButClick ()
	{
		
	}

	public bool Human_can_play {
		get {
			return m_human_can_play;
		}
	}


}
