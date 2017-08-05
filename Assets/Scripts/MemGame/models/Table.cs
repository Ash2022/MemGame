using System;
using System.Collections.Generic;
using MemGame.views;

namespace  MemGame.models
{
	public class Table
	{
		Card[]			m_cards;

		List<Player>	m_players = new List<Player> ();

		int				m_rows=0;
		int				m_columns=0;
		int				m_cards_to_match=2;
		bool			m_winner_plays_on=true;

		int				m_num_of_matches_needed;



		public Table (int rows,int columns, int cards_to_match, int num_players, bool winner_plays_on)
		{
			m_rows = rows;
			m_columns = columns;
			m_cards_to_match = cards_to_match;
			m_winner_plays_on = winner_plays_on;

			m_num_of_matches_needed = m_rows * m_columns / m_cards_to_match;

			m_cards = new Card[columns * rows];

			m_players.Add (new Player (0,true));	//player is always in index 0;
				
			for (int i = 1; i < num_players; i++) 
			{
				Bot bot = new Bot ();
				bot.SetBot (m_rows * m_columns, (ModelManager.BotLevel)UnityEngine.Random.Range (0, 5));
				m_players.Add(new Player(i,false,bot));
			}

			BuildCardDecK ();
		}

		private void BuildCardDecK()
		{

			int total_cards_on_table = m_rows * m_columns;

			List<Card>	potential_cards = new List<Card> ();
			List<Card>	deck = new List<Card> ();
			List<Card>	aux_deck = new List<Card> ();

			for (int i = 0; i < ManagerView.Instance.Cards_images.Length-3; i++) {//specials are at the end and are also added in a "special" way
				potential_cards.Add (new Card(i));
			}

			for (int i = 0; i < total_cards_on_table/ m_cards_to_match; i++) {

				int picked_index = UnityEngine.Random.Range (0, potential_cards.Count);

				for (int j = 0; j < m_cards_to_match; j++) {
					deck.Add (potential_cards [picked_index]);
				}

				potential_cards.RemoveAt (picked_index);
			}

			for (int i = 0; i <total_cards_on_table ; i++) {

				int temp = UnityEngine.Random.Range (0, deck.Count);

				aux_deck.Add (deck [temp]);

				deck.RemoveAt (temp);

			}
			/*
			if (m_cards_to_match == 2) {

				int temp_index = UnityEngine.Random.Range (0, aux_deck.Count);
				Card temp_value = aux_deck [temp_index];

				List<Card> special = new List<Card> ();

				for (int k = 0; k < aux_deck.Count; k++) {
					if (aux_deck [k] == temp_value)
						special.Add (new Card());
				}

				List<int> for_random = new List<int> ();

				for_random.Add (GameController.SHUFFLE);
				for_random.Add (GameController.SHOW_ALL);
				for_random.Add (GameController.JOKER);

				int item1 = UnityEngine.Random.Range (0, for_random.Count);
				int item1_value = for_random [item1];
				for_random.RemoveAt (item1);
				int item2 = UnityEngine.Random.Range (0, for_random.Count);
				int item2_value = for_random [item2];

				aux_deck [special [0]] = item1_value;
				aux_deck [special [1]] = item2_value;
		
			} else if (m_cards_to_match == 3) {
				int temp_index = UnityEngine.Random.Range (0, aux_deck.Count);
				int temp_value = aux_deck [temp_index];

				List<int> special = new List<int> ();

				for (int k = 0; k < aux_deck.Count; k++) {
					if (aux_deck [k] == temp_value)
						special.Add (k);
				}

				aux_deck [special [0]] = GameController.JOKER;
				aux_deck [special [1]] = GameController.SHOW_ALL;
				aux_deck [special [2]] = GameController.SHUFFLE;
			} */

			for (int i = 0; i < aux_deck.Count; i++) {

				aux_deck [i].Table_pos_index = i;
				m_cards [i]= aux_deck [i];

			}


		}

		public Card[] Cards {
			get {
				return m_cards;
			}
			set {
				m_cards = value;
			}
		}

		public List<Player> Players {
			get {
				return m_players;
			}
			set {
				m_players = value;
			}
		}

		public bool Winner_plays_on {
			get {
				return m_winner_plays_on;
			}
		}

		public int Num_of_matches_needed {
			get {
				return m_num_of_matches_needed;
			}
		}

		public int Cards_to_match {
			get {
				return m_cards_to_match;
			}
		}

		public int Columns {
			get {
				return m_columns;
			}
		}

		public int Rows {
			get {
				return m_rows;
			}
		}
	}
}

