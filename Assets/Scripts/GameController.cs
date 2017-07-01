using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {


	int				m_curr_player;
	int				m_num_picked_cards;
	bool			m_first_match_done;
	int				m_curr_num_matches;
	bool			m_last_pick_won;
	bool			m_human_can_play;

	const float 	CARDS_PICK_DELAY_TIME = 1.5f;
	const int 		MATCH_SCORE = 100;
	const int 		FIRST_SCORE = 200;
	const int		TURN_TIME = 15;

	public const int 		UNKNOWN = -1;
	public const int 		MATCHED = -100;

	List<int> 		m_human_picked_cards_indexes = new List<int> ();

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

	public void init()
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

	public void PlayNextTurn()
	{
		if (m_last_pick_won && ManagerView.Instance.Winner_plays_on)
			Debug.Log ("Winner plays on");
		else
			m_curr_player++;

		Debug.Log("Player " + m_curr_player + " Turn");

		if (m_curr_player == ManagerView.Instance.Num_players)
			m_curr_player = 0;


		if (ManagerView.Instance.Num_of_matches_needed != m_curr_num_matches) {

			if (ManagerView.Instance.Players_views [m_curr_player].Bot!=null) {
				StartCoroutine (GenerateAndPlayBotMove (ManagerView.Instance.Cards_to_match));
			}
			else {
				m_human_picked_cards_indexes.Clear ();
				StartCoroutine (StartHumanTimer ());
			}
		}
		else
			Debug.Log ("Round Done");

	}


	private IEnumerator StartHumanTimer()
	{
		//unlock player picking cards and start timer and wait for the result to come
		Debug.Log("Starting Human Play");
		m_human_can_play = true;
		m_num_picked_cards = 0;
		PlayerView player_view = ManagerView.Instance.Players_views [m_curr_player];

		player_view.Timer.fillAmount = 1;
		player_view.ShowHideTimer (true);

	
		bool time_expired = false;


		float f_start_time = Time.realtimeSinceStartup;

		while ((Time.realtimeSinceStartup<f_start_time+TURN_TIME)&&(time_expired==false)&&(m_num_picked_cards != ManagerView.Instance.Cards_to_match))
		{
			float temp = (Time.realtimeSinceStartup - f_start_time) / TURN_TIME;

			player_view.Timer.fillAmount = 1 - temp;

			if (Time.realtimeSinceStartup - f_start_time > TURN_TIME)
				time_expired = true;

			yield return new WaitForSeconds(0.1f);
		}

		player_view.ShowHideTimer (false);

	}

	public void HumanPickedCard(int card_pos)
	{
		Debug.Log ("Human picked card index pos: " + card_pos);

		m_num_picked_cards++;
		m_human_picked_cards_indexes.Add (card_pos);

		if (m_num_picked_cards == ManagerView.Instance.Cards_to_match) {

			m_human_can_play = false;
			StartCoroutine (PostHumanPickCards ());
		}

	}

	private IEnumerator PostHumanPickCards()
	{
		yield return new WaitForSecondsRealtime(1f);

		ScoreCurrentMove(CheckCurrentSelectionMatch (m_human_picked_cards_indexes));
		Debug.Log ("AA");
		yield return new WaitForSecondsRealtime(1.5f);
		Debug.Log ("BB");
		PlayNextTurn ();
	}

	private IEnumerator GenerateAndPlayBotMove(int num_cards_to_pick)
	{

		PlayerView player_view = ManagerView.Instance.Players_views [m_curr_player];

		player_view.Timer.fillAmount = 1;
		player_view.ShowHideTimer (true);

		// Wait time:
		// For bid: Normal distribution with mean of 4 and the stddev of 1
		// For turn: Triangular distribution between 0 and 3 with most frequent value of 0.5
		float actual_wait_time = 0;
	
		actual_wait_time = Random.Range (0.5f, TURN_TIME / 2);


		bool time_expired = false;


		float f_start_time = Time.realtimeSinceStartup;

		while ((Time.realtimeSinceStartup<f_start_time+TURN_TIME)&&(time_expired==false))
		{
			float temp = (Time.realtimeSinceStartup - f_start_time) / TURN_TIME;

			player_view.Timer.fillAmount = 1 - temp;

			if (Time.realtimeSinceStartup - f_start_time > actual_wait_time)
				time_expired = true;

			yield return new WaitForSeconds(0.1f);
		}


		player_view.ShowHideTimer (false);



		List<int> 		picked_cards_indexes = new List<int> ();

		picked_cards_indexes = player_view.Bot.GetGeneratedPicks (num_cards_to_pick);

		/*
		List<int> possible_to_pick = new List<int> ();

		for (int i = 0; i < ManagerView.Instance.Card_views_arr.Length; i++)
			if (ManagerView.Instance.Card_views_arr [i].Pos_index != MATCHED)
				possible_to_pick.Add (ManagerView.Instance.Card_views_arr [i].Pos_index);
		

		int first_picked_card_index = Random.Range (0, possible_to_pick.Count);

		int first_picked_card = possible_to_pick [first_picked_card_index];

		for (int i = 0; i < num_cards_to_pick; i++)
			picked_cards_indexes.Add (first_picked_card);

		bool done = false;

		while (!done) {

			done = true;
			for (int i = 1; i < picked_cards_indexes.Count; i++) {
				int temp = Random.Range (0, possible_to_pick.Count);
				picked_cards_indexes[i] = possible_to_pick [temp];
			}

			for (int i = 0; i < picked_cards_indexes.Count; i++) {

				for (int j = i; j < picked_cards_indexes.Count; j++)
					if (i != j) {

						if (picked_cards_indexes [i] == picked_cards_indexes [j])
							done = false;
					}
				
			}

		}
*/
		for (int i = 0; i <picked_cards_indexes.Count; i++) {

			Debug.Log (picked_cards_indexes [i]);
			ManagerView.Instance.Card_views_arr [picked_cards_indexes[i]].ShowCard (true);

			yield return new WaitForSecondsRealtime (CARDS_PICK_DELAY_TIME);
		}


		ScoreCurrentMove(CheckCurrentSelectionMatch (picked_cards_indexes));

		yield return new WaitForSecondsRealtime(2.5f);


		PlayNextTurn ();
	}

	private void ScoreCurrentMove(bool won)
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

			ManagerView.Instance.Players_views [m_curr_player].Multi=0;
		}
	}


	private bool CheckCurrentSelectionMatch(List<int> picked_cards_indexes)
	{
		int first_card_value = ManagerView.Instance.Card_views_arr[picked_cards_indexes[0]].Image_index;
		bool match = true;


		for (int i = 1; i < picked_cards_indexes.Count; i++) {
			if (ManagerView.Instance.Card_views_arr [picked_cards_indexes[i]].Image_index != first_card_value)
				match = false;
		}

		if (match) {
			Debug.Log ("Match");
			//sort the indexes of the picked cards by size and remove like that

			bool done_sorting = false;

			while (!done_sorting) {

				done_sorting = true;

				for (int i = 0; i < picked_cards_indexes.Count - 1; i++) {

					if (picked_cards_indexes [i] < picked_cards_indexes [i + 1]) {

						int temp = picked_cards_indexes [i];
						picked_cards_indexes [i] = picked_cards_indexes [i + 1];
						picked_cards_indexes [i + 1] = temp;

						done_sorting = false;
					}
				}
			}


			for (int i = 0; i < picked_cards_indexes.Count; i++) {
				ManagerView.Instance.Card_views_arr[picked_cards_indexes [i]].Pos_index = MATCHED;

				for (int k=0;k<ManagerView.Instance.Players_views.Count;k++)
				{
					if (ManagerView.Instance.Players_views [k].Bot != null)
						ManagerView.Instance.Players_views [k].Bot.AddDataFromOtherPlayerMoves (picked_cards_indexes [i], MATCHED);
				}
			}



		} else 
		{
			Debug.Log ("No Match - Closing Cards");
			for (int i = 0; i < picked_cards_indexes.Count; i++) {
				ManagerView.Instance.Card_views_arr[picked_cards_indexes [i]].ShowCard(false);


				for (int k=0;k<ManagerView.Instance.Players_views.Count;k++)
				{
					if (ManagerView.Instance.Players_views [k].Bot != null)
						ManagerView.Instance.Players_views [k].Bot.AddDataFromOtherPlayerMoves (picked_cards_indexes [i], ManagerView.Instance.Card_views_arr[picked_cards_indexes [i]].Image_index);
				}
			}
		}

		return(match);
	}

	public void ButClick()
	{
		
	}

	public bool Human_can_play {
		get {
			return m_human_can_play;
		}
	}
}
