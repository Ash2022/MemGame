using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemGame.views;
using MemGame.controllers;

namespace MemGame.models
{

	public class Bot
	{



		int[] m_cards;
		int[] m_cards_remember;
		int[] m_discovery_time;
		int m_discovery_index;

		int m_continuing_pick = 0;
		List<int>	m_turns_match = new List<int> ();
		bool m_found_match = false;

		ModelManager.BotLevel	m_bot_level;

		BotMemory	m_memory;


		public void SetBot (int num_slots, ModelManager.BotLevel bot_level)
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

			m_memory = ModelManager.Instance.Bots [(int)bot_level];

		}

		private int	GetNIndexOfMatch (int num_picks, int N = 1)
		{
			int match_found_index_to_return = -1;
			List<int>	matched_cards = new List<int> ();

			for (int i = 0; i < m_cards.Length; i++) {

				matched_cards.Clear ();
				match_found_index_to_return = -1;

				if (m_cards [i] != GameController.MATCHED && m_cards [i] != GameController.UNKNOWN) {

					matched_cards.Add (i);

					for (int j = i + 1; j < m_cards.Length; j++) {

						if (m_cards [i] == m_cards [j]) {
						
							matched_cards.Add (j);
						}
					}

				} 

				if (matched_cards.Count >= num_picks) {

					match_found_index_to_return = matched_cards [N - 1];
					break;

				}

			}

			return match_found_index_to_return;
		}

		private List<int> GetMatchIndexs (int num_to_match)
		{
		
			List<int>	matched_cards = new List<int> ();

			for (int i = 0; i < m_cards.Length; i++) {

				matched_cards.Clear ();


				if (m_cards [i] != GameController.MATCHED && m_cards [i] != GameController.UNKNOWN) {
					matched_cards.Add (i);
					for (int j = 0; j < m_cards.Length; j++) {
						if (m_cards [i] == m_cards [j]) {
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

		private int GetRandomPickIndex ()
		{
			List<int>	m_potential_picks = new List<int> ();

			for (int i = 0; i < m_cards.Length; i++) {
				if (m_cards [i] == GameController.UNKNOWN) {
					m_potential_picks.Add (i);
				}
			}

			//pick one of potential at random
			int temp = Random.Range (0, m_potential_picks.Count);

			//temp = 0;

			if (m_potential_picks.Count == 0) {

				//all cards are known but with picking order a match wasnt detected - so we return some random number from the existing cards that were not matched
				for (int i = 0; i < m_cards.Length; i++) {
					if (m_cards [i] != GameController.MATCHED) {
						m_potential_picks.Add (i);
					}
				}

				temp = Random.Range (0, m_potential_picks.Count);
			}

			return m_potential_picks [temp];

		}

		public Card PickCards (int total_to_pick, int curr_pick_index)
		{
		
			if (curr_pick_index == 0) {
				//picking first time in this turn - reset all memory of turn pick
				m_continuing_pick = 0;
				m_turns_match.Clear ();
				m_found_match = false;
				int value_to_return = 0;

				m_turns_match = GetMatchIndexs (total_to_pick); // getting back a list of matchs if exists

				if (m_turns_match.Count > 1) {
					Debug.Log ("FOUND A MATCH");
					m_found_match = true;
					value_to_return = m_turns_match [0];//returning the first of the match
				} else {
					Debug.Log ("DIDNT FIND A MATCH");
					//i didnt have a match - i need to pick a card at random - add it to my memory and take it from there
					value_to_return = GetRandomPickIndex ();
					m_turns_match.Add (value_to_return);

				}
				GameController.Instance.AddDataToAllBots (value_to_return);
				return value_to_return;
			
			} else {
			
				//in the middle of pick process
				int value_to_return = 0;

				if (m_found_match) { //if i already had a match - i am returning the other values of it based on their index
					return m_turns_match [curr_pick_index];
					Debug.Log ("COMPLETING MATCH");
				}

				//i already opened a or some cards - need to see if now i have a match with the new data in mind
				List<int>	temp_pick = new List<int> ();
				temp_pick = GetMatchIndexs (total_to_pick);

				if (temp_pick.Count > 0) {
					Debug.Log ("GOT MATCH AFTER LOOKING AT NEW CARD");
					//with the new data in mind from previous picks in this turn - i now have a full match in temp pick - i already return one or more values and they are in the m_turn_match and need to be removed from the 
					//current matches list - so they dont return twice.

					m_found_match = true;

					for (int i = temp_pick.Count - 1; i >= 0; i--) {
						for (int j = 0; j < m_turns_match.Count; j++) {
							if (temp_pick [i] == m_turns_match [j]) {
								temp_pick.RemoveAt (i);
								break;//this is for when i match 3 cards and i find 2 new ones at random and i know the third from past selections
							}
						}
					}

					//now temp_pick holds the parts of the match not yet returned - need to copy to mturns_match and return based on index of picks

					for (int i = 0; i < temp_pick.Count; i++)
						m_turns_match.Add (temp_pick [i]);

					value_to_return = m_turns_match [curr_pick_index];

				} else {
					//i didnt have a match - i need to pick a card at random - add it to my memory and take it from there
					Debug.Log ("DIDNT HAVE A MATCH 2ND CARD");
					bool done = false;
					int counter = 0;

					while (!done) {
						counter++;
						done = true;

						value_to_return = GetValidCardIndex ();


						for (int i = 0; i < m_turns_match.Count; i++)
							if (value_to_return == m_turns_match [i])
								done = false;
					
						if (counter > 100) {
							value_to_return = GetValidCardIndex ();
							done = true;
							Debug.Log ("Cards Left: " + m_turns_match.Count);
							Debug.Log ("ERROR IN BOT LOGIC");
						}

					}
					//need to make sure that this random picked value isnt already in the returned cards

					m_turns_match.Add (value_to_return);

				}

				if (curr_pick_index == 2) {

					for (int i = 0; i < m_turns_match.Count; i++)
						Debug.Log ("Turn Match: " + m_turns_match [i]);
				}

				GameController.Instance.AddDataToAllBots (value_to_return, ManagerView.Instance.Table_view.Card_views [value_to_return].Image_index);
				return value_to_return;

			}

		}




		public void AddDataFromOtherPlayerMoves (Card card)
		{/*
			m_cards [pick_index] = value;
			m_cards_remember [pick_index] = 1;

			m_discovery_time [pick_index] = m_discovery_index;
			m_discovery_index++;

			UpdateRememberCards ();*/
		}

		private int FindAlreadySeen ()
		{
			List<int>	possible_picks = new List<int> ();

			for (int i = 0; i < m_cards.Length; i++) {

				if (m_cards [i] != GameController.MATCHED && m_cards [i] != GameController.UNKNOWN)
					possible_picks.Add (m_cards [i]);
			}

			if (possible_picks.Count > 0)
				return possible_picks [Random.Range (0, possible_picks.Count)];
			else {
				for (int i = 0; i < m_cards.Length; i++) {

					if (m_cards [i] != GameController.MATCHED)
						possible_picks.Add (m_cards [i]);
				}

				if (possible_picks.Count > 0)
					return possible_picks [Random.Range (0, possible_picks.Count)];
				else
					return 1;
			}
		}

		private void UpdateRememberCards ()
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

				if (curr_mem_value < Random.Range (0, m_memory.Forget_value)) {//bot forgot value
					m_cards_remember [i] = 0;

					/*
				float temp = Random.Range (0f, 1f);

				if (temp > 0.8f) 
				{
					int found_value = FindAlreadySeen ();
					AddDataFromOtherPlayerMoves (i, found_value);
				} 
				else 
				{
					m_cards_remember [i] = 0;
				}
*/
				}
			}


			//go over all the memory - and if there are 0 - need to delete the entry in the m_cards;

			for (int i = 0; i < num_cards; i++) {

				if (m_cards_remember [i] == 0)
				if (m_cards [i] != GameController.MATCHED) {
				
					m_cards [i] = GameController.UNKNOWN;
			
				}
			}

		}

		public void ForgetAll ()
		{
			int num_cards = m_cards_remember.Length;

			for (int i = 0; i < num_cards; i++) {

				m_cards_remember [i] = 0;
			}
			UpdateRememberCards ();
		}

		private int GetValidCardIndex ()
		{
			List<int>	potentials = new List<int> ();

			for (int i = 0; i < m_cards.Length; i++) {

				if (m_cards [i] != GameController.MATCHED)
					potentials.Add (i);
			}

			int value = Random.Range (0, potentials.Count);

			if (potentials.Count == 0)
				return 0;
			else
				return potentials [value];

		}

		public void HighlightAllKnownCards (bool show)
		{
			if (show) {
				for (int i = 0; i < m_cards.Length; i++)
					if (m_cards [i] != GameController.UNKNOWN && m_cards [i] != GameController.MATCHED) {
						
						if (m_cards [i] != ModelManager.Instance.Table.Cards [i].Image_index)
							ManagerView.Instance.Table_view.Card_views [i].HighLight (true, true);
						else
							ManagerView.Instance.Table_view.Card_views [i].HighLight (true, false);
					
					}
			} else {

				for (int i = 0; i < m_cards.Length; i++)
					ManagerView.Instance.Table_view.Card_views [i].HighLight (false, false);
			}
		}

		public ModelManager.BotLevel Bot_level {
			get {
				return m_bot_level;
			}
		}

		public int M_continuing_pick {
			get {
				return m_continuing_pick;
			}
			set {
				m_continuing_pick = value;
			}
		}
	}
}
