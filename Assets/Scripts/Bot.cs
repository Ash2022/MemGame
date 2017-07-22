using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot
{



	int[] 		m_cards;
	int[] 		m_cards_remember;
	int[] 		m_discovery_time;
	int 		m_discovery_index;

	int 		m_continuing_pick=0;
	List<int>	m_turns_match=new List<int>();
	bool		m_found_match=false;

	ManagerView.BotLevel	m_bot_level;

	BotMemory	m_memory;


	public void SetBot (int num_slots, ManagerView.BotLevel bot_level )
	{
		m_cards = new int[num_slots];
		m_cards_remember = new int[num_slots];
		m_discovery_time = new int[num_slots];

		m_discovery_index = 0;

		for (int i = 0; i < num_slots; i++) {
			m_cards [i] = GameController.UNKNOWN;
			m_cards_remember [i] = 0;
		}

		m_bot_level = bot_level;

		m_memory = ManagerView.Instance.Bots[(int)bot_level];

	}

	private int	GetNIndexOfMatch (int num_picks,int N=1)
	{
		int match_found_index_to_return = -1;
		List<int>	matched_cards = new List<int> ();

		for (int i = 0; i < m_cards.Length; i++) {

			matched_cards.Clear ();
			match_found_index_to_return = -1;

			if (m_cards [i] != GameController.MATCHED && m_cards [i] != GameController.UNKNOWN) {

				matched_cards.Add(i);

				for (int j = i + 1; j < m_cards.Length; j++) {

					if (m_cards [i] == m_cards [j]) {
						
						matched_cards.Add (j);
					}
				}

			} 

			if (matched_cards.Count >= num_picks) {

				match_found_index_to_return = matched_cards[N-1];
				break;

			}

		}

		return match_found_index_to_return;
	}

	private List<int> GetMatchIndexs(int num_to_match)
	{
		
		List<int>	matched_cards = new List<int> ();

		for (int i = 0; i < m_cards.Length; i++) 
		{

			matched_cards.Clear ();


			if (m_cards [i] != GameController.MATCHED && m_cards [i] != GameController.UNKNOWN) 
			{
				matched_cards.Add(i);
				for (int j=0; j < m_cards.Length; j++) 
				{
					if (m_cards [i] == m_cards [j]) 
					{
						if (i != j)
							matched_cards.Add (j);
					}
				}

			} 

			if (matched_cards.Count >= num_to_match) {
				return matched_cards;
			} else {
				matched_cards.Clear ();//if i find nothing - i will return an empty list 
			}
		}

		return matched_cards;
	}

	private int GetRandomPickIndex()
	{
		List<int>	m_potential_picks = new List<int> ();

		for (int i = 0; i < m_cards.Length; i++) 
		{
			if (m_cards [i] == GameController.UNKNOWN) {
				m_potential_picks.Add (i);
			}
		}

		//pick one of potential at random
		int temp = Random.Range (0, m_potential_picks.Count);

		temp = 0;

		if (m_potential_picks.Count == 0) {

			//all cards are known but with picking order a match wasnt detected - so we return some random number from the existing cards that were not matched
			for (int i = 0; i < m_cards.Length; i++) 
			{
				if (m_cards [i] != GameController.MATCHED) {
					m_potential_picks.Add (i);
				}
			}

			temp = Random.Range (0, m_potential_picks.Count);
		}

		return m_potential_picks[temp];

	}

	public int PickCards(int total_to_pick,int curr_pick_index)
	{



		if (curr_pick_index == 0) 
		{
			//picking first time in this turn - reset all memory of turn pick
			m_continuing_pick = 0;
			m_turns_match.Clear ();
			m_found_match = false;
			int value_to_return = 0;

			m_turns_match = GetMatchIndexs (total_to_pick); // getting back a list of matchs if exists

			if (m_turns_match.Count > 0)
			{
				m_found_match = true;
				value_to_return = m_turns_match [0];//returning the first of the match
			} 
			else 
			{
				//i didnt have a match - i need to pick a card at random - add it to my memory and take it from there
				value_to_return = GetRandomPickIndex ();
				m_turns_match.Add (value_to_return);

			}
			ManagerView.Instance.AddDataToAllBots (value_to_return, ManagerView.Instance.Card_views_arr [value_to_return].Image_index);
			return value_to_return;
			
		} 
		else 
		{
			//in the middle of pick process
			int value_to_return = 0;

			if (m_found_match) //if i already had a match - i am returning the other values of it based on their index
				return m_turns_match [curr_pick_index];

			//i already opened a or some cards - need to see if now i have a match with the new data in mind
			List<int>	temp_pick = new List<int> ();
			temp_pick = GetMatchIndexs (total_to_pick);

			if (temp_pick.Count > 0) 
			{
				//with the new data in mind from previous picks in this turn - i now have a full match in temp pick - i already return one or more values and they are in the m_turn_match and need to be removed from the 
				//current matches list - so they dont return twice.

				m_found_match = true;

				for (int i = temp_pick.Count-1; i >0 ; i--) 
				{
					for (int j = 0; j < m_turns_match.Count; j++) 
					{
						if (temp_pick [i] == m_turns_match [j]) 
						{
							temp_pick.RemoveAt (i);
							break;//this is for when i match 3 cards and i find 2 new ones at random and i know the third from past selections
						}
					}
				}

				//now temp_pick holds the parts of the match not yet returned - need to copy to mturns_match and return based on index of picks

				for (int i = 0; i < temp_pick.Count; i++)
					m_turns_match.Add (temp_pick [i]);

				value_to_return = m_turns_match [curr_pick_index];

			}
			else 
			{
				//i didnt have a match - i need to pick a card at random - add it to my memory and take it from there

				bool done = false;

				while (!done) 
				{
					done = true;

					value_to_return = GetRandomPickIndex ();

					for (int i = 0; i < m_turns_match.Count; i++)
						if (value_to_return == m_turns_match [i])
							done = false;
				}
				//need to make sure that this random picked value isnt already in the returned cards

				m_turns_match.Add (value_to_return);

			}

			if (curr_pick_index == 2) {

				for (int i = 0; i < m_turns_match.Count; i++)
					Debug.Log ("Turn Match: " + m_turns_match [i]);
			}

			ManagerView.Instance.AddDataToAllBots (value_to_return, ManagerView.Instance.Card_views_arr [value_to_return].Image_index);
			return value_to_return;

		}

	}

	public List<int> GetGeneratedPicks (int num_picks)
	{
		

		List<int> picks = new List<int> ();
		List<int> possible_to_pick = new List<int> ();


		int match_found_first_index = GetNIndexOfMatch (num_picks);

		//i already have a match with the data i know about
		if (match_found_first_index >= 0) {
			int picked_value = m_cards [match_found_first_index];

			for (int i = 0; i < m_cards.Length; i++)
				if (m_cards [i] == picked_value)
					possible_to_pick.Add (i);

			if (possible_to_pick.Count == num_picks) {
				Debug.Log ("Bot "+ Bot_level+" remembered pick");
			}

		} else {


			//no match with the data i know - try to pick an unknown card
			for (int i = 0; i < m_cards.Length; i++)
				if (m_cards [i] == GameController.UNKNOWN)
					possible_to_pick.Add (i);


			if (possible_to_pick.Count == 0) { //can happen in some cases
				for (int i = 0; i < m_cards.Length; i++)
						possible_to_pick.Add (i);
			}
		}

		int first_picked_card_index = Random.Range (0, possible_to_pick.Count);

		int first_picked_card = possible_to_pick [first_picked_card_index];

		match_found_first_index = GetNIndexOfMatch (num_picks);

		if (match_found_first_index == -1) {
			//we didnt have a match in hand - we are picking unknown cards - so need to pick 1 and then check to see if maybe now there is a match
			ManagerView.Instance.AddDataToAllBots (first_picked_card, ManagerView.Instance.Card_views_arr [first_picked_card].Image_index);
		}

		//check if maybe now there is a match - and if so give it back as answer or look for another card
		match_found_first_index = GetNIndexOfMatch (num_picks);

		if (match_found_first_index >= 0) {
			int picked_value = m_cards [match_found_first_index];

			possible_to_pick.Clear ();

			for (int i = 0; i < m_cards.Length; i++)
				if (m_cards [i] == picked_value)
					possible_to_pick.Add (i);
		}


		for (int i = 0; i < num_picks; i++)
			picks.Add (first_picked_card);

		bool done = false;

		while (!done) {

			done = true;
			for (int i = 1; i < picks.Count; i++) {
				int temp = Random.Range (0, possible_to_pick.Count);
				picks [i] = possible_to_pick [temp];
			}

			for (int i = 0; i < picks.Count; i++) {

				for (int j = i; j < picks.Count; j++)
					if (i != j) {

						if (picks [i] == picks [j])
							done = false;
					}
			}

		}


		//need to arrange picks by discovery time - so looks more like human player
		bool sort_by_discovery_time_done = false;
		while (!sort_by_discovery_time_done) {

			sort_by_discovery_time_done = true;

			for (int i = 0; i < picks.Count - 1; i++) {

				if (m_discovery_time [picks [i]] < m_discovery_time [picks [i + 1]]) {
					sort_by_discovery_time_done = false;
					int temp = picks [i];
					picks [i] = picks [i + 1];
					picks [i + 1] = temp;
				}

			}
		}

		return picks;
	}

	public void AddDataFromOtherPlayerMoves (int pick_index, int value)
	{
		m_cards [pick_index] = value;
		m_cards_remember [pick_index] = 1;

		m_discovery_time [pick_index] = m_discovery_index;
		m_discovery_index++;

		UpdateRememberCards ();
	}

	private void UpdateRememberCards()
	{
		int num_cards = m_cards_remember.Length;

		for (int i = 0; i < num_cards; i++) {

			if (m_cards_remember [i] > 0)
				m_cards_remember [i]++;
		}

		//increase number of turns since card was shown

		//calculate if player forgot a card
		for (int i = 0; i < num_cards; i++) {

			int entry_value = m_cards_remember [i];

			int curr_mem_value = m_memory.Initial_memory - entry_value * m_memory.Decline_rate;

			if (curr_mem_value < Random.Range (0, 70)) {//bot forgot value

				m_cards_remember [i] = 0;

			}
		}


		//go over all the memory - and if there are 0 - need to delete the entry in the m_cards;

		for (int i = 0; i < num_cards; i++) {

			if (m_cards_remember [i] == 0)

			if(m_cards[i]!=GameController.MATCHED)
				m_cards [i] = GameController.UNKNOWN;
		}

	}

	public void HighlightAllKnownCards(bool show)
	{
		if (show) {
			for (int i = 0; i < m_cards.Length; i++)
				if (m_cards [i] != GameController.UNKNOWN && m_cards [i] != GameController.MATCHED)
					ManagerView.Instance.Card_views_arr [i].HighLight (true);
		} else {

			for (int i = 0; i < m_cards.Length; i++)
				ManagerView.Instance.Card_views_arr [i].HighLight (false);
		}
	}

	public ManagerView.BotLevel Bot_level {
		get {
			return m_bot_level;
		}
	}

	public int M_continuing_pick {
		get {
			return m_continuing_pick;
		}
		set{
			m_continuing_pick = value;
		}
	}
}
