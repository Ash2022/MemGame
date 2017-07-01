using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot
{


	float m_chance_to_forget;
	int[] m_cards;
	int[] m_discovery_time;
	int m_discovery_index;

	public void SetBot (int num_slots, float chance)
	{
		m_cards = new int[num_slots];
		m_discovery_time = new int[num_slots];

		m_discovery_index = 0;

		for (int i = 0; i < num_slots; i++)
			m_cards [i] = GameController.UNKNOWN;
		

		m_chance_to_forget = chance;
	}

	private int	GetFirstIndexOfMatch (int num_picks)
	{
		int match_found_first_index = -1;

		for (int i = 0; i < m_cards.Length; i++) {

			int card_i_match_counter = 0;
			match_found_first_index = -1;

			if (m_cards [i] != GameController.MATCHED && m_cards [i] != GameController.UNKNOWN) {

				for (int j = i + 1; j < m_cards.Length; j++) {

					if (m_cards [i] == m_cards [j]) {
						card_i_match_counter++;
					}
				}

			}

			if (card_i_match_counter >= num_picks - 1) {

				//found a match already in what i know - for a specific i - can stop the rest
				match_found_first_index = i;
				break;
			}

		}

		return match_found_first_index;
	}

	public List<int> GetGeneratedPicks (int num_picks)
	{
		List<int> picks = new List<int> ();
		List<int> possible_to_pick = new List<int> ();


		int match_found_first_index = GetFirstIndexOfMatch (num_picks);

		//i already have a match with the data i know about
		if (match_found_first_index >= 0) {
			int picked_value = m_cards [match_found_first_index];

			for (int i = 0; i < m_cards.Length; i++)
				if (m_cards [i] == picked_value)
					possible_to_pick.Add (i);
		} else {


			//no match with the data i know - try to pick an unknown card
			for (int i = 0; i < m_cards.Length; i++)
				if (m_cards [i] == GameController.UNKNOWN)
					possible_to_pick.Add (i);


			if (possible_to_pick.Count == 0) {
				Debug.Log ("ERRRRRRRRRRROR in bot logic - doesnt find a match and doesnt have any unknown cards!!!!");
			}
		}



		//need to make better - first see if you have in the list num_picks cards that you know their value - if so pick them

		// if dont have - try to see if there are still Unknown cards - pick them - after you pick a first unknown - check to see if you know the others - and if so pick - if not try another Unknown.



		int first_picked_card_index = Random.Range (0, possible_to_pick.Count);

		int first_picked_card = possible_to_pick [first_picked_card_index];

		match_found_first_index = GetFirstIndexOfMatch (num_picks);

		if (match_found_first_index == -1) {
			//we didnt have a match in hand - we are picking unknown cards - so need to pick 1 and then check to see if maybe now there is a match

			AddDataFromOtherPlayerMoves (first_picked_card, ManagerView.Instance.Card_views_arr [first_picked_card].Image_index);
		}

		//check if maybe now there is a match - and if so give it back as answer or look for another card
		match_found_first_index = GetFirstIndexOfMatch (num_picks);

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

	public void AddDataFromOtherPlayerMoves (int	pick_index, int value)
	{
		m_cards [pick_index] = value;

		m_discovery_time [pick_index] = m_discovery_index;
		m_discovery_index++;
	}

}
