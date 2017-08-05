using System;
using UnityEngine;
using MemGame.models;
using System.Collections.Generic;
using MemGame.controllers;

namespace MemGame.views
{
	public class TableView : MonoBehaviour
	{
		public const	int PLAYER_X_OFFSET = 250;
		public const	int PLAYER_Y_OFFSET = 800;

		public const	int CARD_X_SPACE = 10;
		public const	int CARD_Y_SPACE = 10;

		public const 	int CARD_WIDTH = 160;

		float m_table_width = 0;
		float m_table_height = 0;

		[SerializeField]Transform m_players_holder = null;
		[SerializeField]Transform m_cards_holder = null;
		[SerializeField]RectTransform	m_table_holder = null;

		List<PlayerView> 	m_players_views = new List<PlayerView> ();
		CardView[]		 	m_card_views;

		Table 				m_table;

		Vector3[] m_player_pos = new Vector3[4];

		public enum Player_Postions
		{
			South = 0,
			West = 1,
			North = 2,
			East = 3

		}

		public TableView ()
		{
			
		}

		public void Awake()
		{
			m_player_pos [(int)Player_Postions.South] = new Vector3 (-PLAYER_X_OFFSET, -PLAYER_Y_OFFSET + 120f, 0);
			m_player_pos [(int)Player_Postions.West] = new Vector3 (-PLAYER_X_OFFSET, PLAYER_Y_OFFSET, 0);
			m_player_pos [(int)Player_Postions.North] = new Vector3 (PLAYER_X_OFFSET, PLAYER_Y_OFFSET, 0);
			m_player_pos [(int)Player_Postions.East] = new Vector3 (PLAYER_X_OFFSET, -PLAYER_Y_OFFSET + 120f, 0);
		}

		public void Init()
		{
			m_table = ModelManager.Instance.Table;
			ClearOldAssets ();
			GeneratePlayerViews ();
			GenerateCardViews ();

		}

		public void BuildCards()
		{

		}

		private void ClearOldAssets()
		{
			foreach (Transform child in m_players_holder) {

				Destroy (child.gameObject);

			}

			foreach (Transform child in m_cards_holder) {

				Destroy (child.gameObject);

			}

			m_table_width = m_table_holder.rect.width;
			m_table_height = m_table_holder.rect.height;


			m_card_views = new CardView[m_table.Cards.Length];
			m_players_views.Clear ();
		}

		private void GeneratePlayerViews ()
		{
			for (int i = 0; i < m_table.Players.Count; i++) 
			{
				GameObject new_player = (GameObject)Instantiate (ManagerView.Instance.Player_prefab);

				new_player.transform.SetParent (m_players_holder);
				new_player.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
				new_player.GetComponent<RectTransform> ().localPosition =  m_player_pos [i];
				PlayerView pv = new_player.GetComponent<PlayerView> ();

				pv.SetPlayer (ModelManager.Instance.Table.Players [i],i,0);

				m_players_views.Add (pv);
			}

		}

		private void GenerateCardViews ()
		{

			int total_cards_on_table = ModelManager.Instance.Table.Cards.Length;

			if (total_cards_on_table % ModelManager.Instance.Table.Cards_to_match != 0) {
				Debug.Log ("Mis Match between number of cards to match and table size");
				return;
			}
			int i = 0;
			int j = 0;

			for (i = 0; i < m_table.Columns; i++) {
				for (j = 0; j < m_table.Rows; j++) {

					GameObject G = (GameObject)Instantiate (ManagerView.Instance.Card_prefab);
					G.transform.SetParent (m_cards_holder);
					G.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
					G.GetComponent<RectTransform> ().localPosition = new Vector3 (i * CARD_WIDTH + i * CARD_X_SPACE, j * CARD_WIDTH + j * CARD_Y_SPACE, 0);

					CardView cv = G.GetComponent<CardView> ();


					m_card_views [m_table.Rows * i + j] = cv;
				}
			}

			int total_x_cards = (m_table.Columns - 1) * CARD_WIDTH + (m_table.Columns - 1) * CARD_X_SPACE;
			int total_y_cards = (m_table.Rows - 1) * CARD_WIDTH + (m_table.Rows - 1) * CARD_X_SPACE;
			m_cards_holder.GetComponent<RectTransform> ().localPosition = new Vector3 (-total_x_cards / 2, -total_y_cards / 2, 0);


		
			for (i = 0; i < m_card_views.Length; i++) {

				m_card_views [i].SetCard (m_table.Cards[i]);

			}

			GameController.Instance.init ();

		}


		public CardView[] Card_views {
			get {
				return m_card_views;
			}
			set {
				m_card_views= value;
			}
		}

		public List<PlayerView> Players_views {
			get {
				return m_players_views;
			}
			set {
				m_players_views = value;
			}
		}
	}
}

