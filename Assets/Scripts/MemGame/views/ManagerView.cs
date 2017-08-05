using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using MemGame.models;
using MemGame.controllers;

namespace MemGame.views
{

	public class ManagerView : MonoBehaviour
	{

		[SerializeField]GameObject m_player_prefab = null;
		[SerializeField]GameObject m_card_prefab = null;
		[SerializeField]GameObject m_score_prefab = null;

		[SerializeField]Transform m_scores_holder = null;
	

		[SerializeField]Sprite m_card_back = null;
		[SerializeField]Sprite[] m_cards_images = null;
		[SerializeField]Sprite[] m_avatars_images = null;



		[SerializeField]int m_rows = 0;
		[SerializeField]int m_columns = 0;
		[SerializeField]int m_cards_to_match = 2;
		[SerializeField]int m_num_players = 2;
		[SerializeField]bool m_winner_plays_on = true;
		[SerializeField]int m_num_human_players = 1;

		[SerializeField]bool m_show_bot_debug = false;

		int m_num_of_matches_needed;


		[SerializeField]GameObject m_selection_gui = null;
		[SerializeField]GameObject m_restart_button = null;

		[SerializeField]TableView m_table_view=null;

		//List<PlayerView> m_players_views = new List<PlayerView> ();
		//CardView[] m_card_views_arr;



		List<string> m_score_event_list = new List<string> ();
		bool m_score_processing = false;



		static ManagerView m_instance;

	

		public static ManagerView 		Instance {
			get {

				if (m_instance == null) {
					ManagerView manager = GameObject.FindObjectOfType<ManagerView> ();
					m_instance = manager.GetComponent<ManagerView> ();
				}
				return m_instance;
			}
		}

		public void SetTableParams (int rows, int cols, int players, int match, int speed)
		{
			m_rows = rows;
			m_columns = cols;
			m_num_players = players;
			m_cards_to_match = match;
		}

		public Sprite[]Cards_images {
			get {
				return m_cards_images;
			}
		}

		public GameObject Card_prefab {
			get {
				return m_card_prefab;
			}
		}

		public GameObject Player_prefab {
			get {
				return m_player_prefab;
			}
		}


		public void NumRowsChanged (string newText)
		{
			m_rows = Convert.ToInt32 (newText);
		}

		public void NumColsChanged (string newText)
		{
			m_columns = Convert.ToInt32 (newText);
		}

		public void NumMatchChanged (string newText)
		{
			m_cards_to_match = Convert.ToInt32 (newText);
		}

		public void NumPlayersChanged (string newText)
		{
			m_num_players = Convert.ToInt32 (newText);
		}

		public void NumHumanPlayersChanged (string newText)
		{
			m_num_human_players = Convert.ToInt32 (newText);
		}

		public void NumWinnerChanged (string newText)
		{
			if (Convert.ToInt32 (newText) == 0)
				m_winner_plays_on = false;
			else
				m_winner_plays_on = true;
		}


		void Update ()
		{
			if (Input.GetKeyUp (KeyCode.G))
				StartCoroutine (GameController.Instance.ShowAllCards (null, -1));
			if (Input.GetKeyUp (KeyCode.S))
				CreateScoreItem ("1000 pts");
		
		}





		private void Init ()
		{




		}

		void Start ()
		{
			Init ();
			ShowSelectionGui ();
		}

		private void ShowSelectionGui ()
		{
			m_selection_gui.SetActive (true);
			m_selection_gui.transform.FindChild ("Text").GetComponent<TMP_Text> ().text = DateTime.Now.ToString ();
		}

		public void ShowRestart ()
		{
			//before we really show it - need to see if there are any cards left - if so they are bonus cards - they need to be opened and scored

			for (int i = 0; i < m_table_view.Card_views.Length; i++) {

				if (m_table_view.Card_views [i].Pos_index != GameController.MATCHED) {

					m_table_view.Card_views [i].ShowCard (true);
					GameController.Instance.ScoreSpecialCurrentMove (m_table_view.Card_views [i].Image_index);
				}
			}

			m_restart_button.SetActive (true);
		}

		public void RestartClicked ()
		{
			m_restart_button.SetActive (false);
			m_table_view.Init ();
			ShowSelectionGui ();
		}

		public void StartButtonClicked ()
		{
			StopAllCoroutines ();
			BuildTable ();
		}

		private void BuildTable ()
		{
			m_num_of_matches_needed = m_rows * m_columns / m_cards_to_match;

			if (m_cards_to_match == 2 || m_cards_to_match == 3)
				m_num_of_matches_needed--;


			ModelManager.Instance.Init ();

			Table table = new Table (m_rows, m_columns, m_cards_to_match, m_num_players, m_winner_plays_on);

			ModelManager.Instance.Table = table;

			m_selection_gui.SetActive (false);

			m_table_view.Init ();


		}







		IEnumerator ProcessScoreEvents ()
		{
			m_score_processing = true;

			while (m_score_event_list.Count > 0) {
				CreateScoreItem (m_score_event_list [0]);
				yield return new WaitForSecondsRealtime (0.25f);
				m_score_event_list.RemoveAt (0);
			}

			m_score_processing = false;
		}

		public void AddScoreEvent (string item)
		{
			m_score_event_list.Add (item);

			if (!m_score_processing)
				StartCoroutine (ProcessScoreEvents ());
		}

		private void CreateScoreItem (string score_text)
		{
			GameObject score = (GameObject)Instantiate (ManagerView.Instance.Score_prefab);
			RectTransform rect = score.GetComponent<RectTransform> ();
			score.transform.SetParent (m_scores_holder);
			rect.localPosition = new Vector3 (0, 0, 0);
			rect.localScale = new Vector3 (1f, 1f, 1f);

			score.GetComponent<TMP_Text> ().text = score_text;

			iTween.MoveAdd (score, iTween.Hash ("y", 5, "islocal", true, "easeType", iTween.EaseType.easeInCirc, "time", 2, "onComplete", "ScoreAnimDone", "onCompleteTarget", this.gameObject, "oncompleteparams", score));

		}

		private void ScoreAnimDone (GameObject param)
		{
			Destroy (param);
		}

		public Sprite[] Avatars_images {
			get {
				return m_avatars_images;
			}
		}

		public Sprite Card_back {
			get {
				return m_card_back;
			}
		}



		public int Cards_to_match {
			get {
				return m_cards_to_match;
			}
		}

		public int Num_players {
			get {
				return m_num_players;
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



		public bool Show_bot_debug {
			get {
				return m_show_bot_debug;
			}
		}

		public GameObject Score_prefab {
			get {
				return m_score_prefab;
			}
		}

		public TableView Table_view {
			get {
				return m_table_view;
			}
		}
	}
}