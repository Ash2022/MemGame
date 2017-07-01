using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerView : MonoBehaviour {

	[SerializeField]GameObject		m_player_prefab=null;
	[SerializeField]GameObject		m_card_prefab=null;

	[SerializeField]Transform		m_players_holder=null;
	[SerializeField]Transform		m_cards_holder=null;
	[SerializeField]RectTransform	m_table=null;

	[SerializeField]Sprite			m_card_back=null;
	[SerializeField]Sprite[]		m_cards_images=null;
	[SerializeField]Sprite[]		m_avatars_images=null;

	public const	int				PLAYER_X_OFFSET = 450;
	public const	int				PLAYER_Y_OFFSET = 700;

	public const	int				CARD_X_SPACE = 10;
	public const	int				CARD_Y_SPACE = 10;

	public const 	int 			CARD_WIDTH = 128;

	float							m_table_width=0;
	float							m_table_height=0;


	[SerializeField]int				m_rows=0;
	[SerializeField]int				m_columns=0;
	[SerializeField]int				m_cards_to_match=2;
	[SerializeField]int				m_num_players=2;
	[SerializeField]bool			m_winner_plays_on=true;
	int								m_num_of_matches_needed;

	List<PlayerView>				m_players_views = new List<PlayerView> ();
	CardView[]						m_card_views_arr;

	Vector3[]						m_player_pos = new Vector3[4];

	public enum Player_Postions
	{
		South=0,
		West=1,
		North=2,
		East=3

	}

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

	public Sprite[]Cards {
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

	public Transform Players_holder {
		get {
			return m_players_holder;
		}
	}

	public Transform Cards_holder {
		get {
			return m_cards_holder;
		}
	}

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.G))
			BuildTable();
	}

	private void Init()
	{
		m_player_pos [(int)Player_Postions.South] = new Vector3 (0, -PLAYER_Y_OFFSET, 0);
		m_player_pos [(int)Player_Postions.West] = new Vector3 (-PLAYER_X_OFFSET, 0, 0);
		m_player_pos [(int)Player_Postions.North] = new Vector3 (0, PLAYER_Y_OFFSET, 0);
		m_player_pos [(int)Player_Postions.East] = new Vector3 (PLAYER_X_OFFSET, 0, 0);

		m_table_width = m_table.rect.width;
		m_table_height = m_table.rect.height;

		m_num_of_matches_needed = m_rows * m_columns / 2;

	}
	void Start()
	{
		Init ();
	}

	private void BuildTable()
	{
		ClearExsistingObjects ();
		GeneratePlayers ();
		GenerateCards ();

	}

	private void ClearExsistingObjects()
	{
		foreach (Transform child in m_players_holder) {

			Destroy (child.gameObject);
			
		}

		foreach (Transform child in m_cards_holder) {

			Destroy (child.gameObject);

		}

		m_card_views_arr = new CardView[m_rows * m_columns];

		m_players_views.Clear ();
	}

	private void GenerateCards()
	{
		
		int total_cards_on_table = m_rows * m_columns;

		if (total_cards_on_table % m_cards_to_match != 0) {
			Debug.Log ("Mis Match between number of cards to match and table size");
			return;
		}
		int i = 0;
		int j = 0;

		for (i = 0; i < m_columns; i++) {
			for (j = 0; j < m_rows; j++) {

				GameObject G = (GameObject)Instantiate (m_card_prefab);
				G.transform.SetParent (m_cards_holder);
				G.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
				G.GetComponent<RectTransform> ().localPosition = new Vector3 (i * 128 + i * CARD_X_SPACE, j * 128 + j * CARD_Y_SPACE, 0);

				CardView cv = G.GetComponent<CardView> ();


				m_card_views_arr  [m_rows*i+ j] = cv;
			}
		}

		int total_x_cards = (m_columns-1) * CARD_WIDTH + (m_columns - 1) * CARD_X_SPACE;
		int total_y_cards = (m_rows-1) * CARD_WIDTH + (m_rows - 1) * CARD_X_SPACE;
		m_cards_holder.GetComponent<RectTransform> ().localPosition = new Vector3 (-total_x_cards/2, -total_y_cards/2, 0);
	
		List<int>	potential_cards = new List<int> ();
		List<int>	deck = new List<int> ();
		List<int>	aux_deck = new List<int> ();

		for (i = 0; i < m_cards_images.Length; i++) {
			potential_cards.Add (i);
		}

		for (i = 0; i < total_cards_on_table/m_cards_to_match; i++) {

			int picked_index = Random.Range (0, potential_cards.Count);

			for (j = 0; j < m_cards_to_match; j++) {
				deck.Add (potential_cards[picked_index]);
			}

			potential_cards.RemoveAt (picked_index);
		}

		for (i = 0; i < total_cards_on_table; i++) {

			int temp = Random.Range(0,deck.Count);

			aux_deck.Add(deck[temp]);

			deck.RemoveAt (temp);

		}

		for (i = 0; i < aux_deck.Count; i++) {
			

			m_card_views_arr[i].SetCard(aux_deck[i],i);

		}

		GameController.Instance.init ();

	}

	private void GeneratePlayers()
	{
		if (m_num_players == 2) {

			//in this case we make a north south
			GameObject new_player = (GameObject)Instantiate(m_player_prefab);

			new_player.transform.SetParent(m_players_holder);
			new_player.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.South];
			PlayerView pv = new_player.GetComponent<PlayerView>();
			pv.SetPlayer ((int)Player_Postions.South,null);
			m_players_views.Add (pv);

			GameObject new_player2 = (GameObject)Instantiate(m_player_prefab);

			new_player2.transform.SetParent(m_players_holder);
			new_player2.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player2.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.North];
			PlayerView pv2 = new_player2.GetComponent<PlayerView>();
			Bot bot = new Bot ();
			bot.SetBot (m_rows * m_columns, 0.75f);
			pv2.SetPlayer ((int)Player_Postions.North,bot);
			m_players_views.Add (pv2);


		} else if (m_num_players == 3) {
			//make south and east west
			//in this case we make a north south
			GameObject new_player = (GameObject)Instantiate(m_player_prefab);

			new_player.transform.SetParent(m_players_holder);
			new_player.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.South];
			PlayerView pv = new_player.GetComponent<PlayerView>();
			pv.SetPlayer ((int)Player_Postions.South,null);
			m_players_views.Add (pv);

			GameObject new_player2 = (GameObject)Instantiate(m_player_prefab);

			new_player2.transform.SetParent(m_players_holder);
			new_player2.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player2.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.West];
			PlayerView pv2 = new_player2.GetComponent<PlayerView>();
			Bot bot2 = new Bot ();
			bot2.SetBot (m_rows * m_columns, 0.75f);
			pv2.SetPlayer ((int)Player_Postions.West,bot2);
			m_players_views.Add (pv2);

			GameObject new_player3 = (GameObject)Instantiate(m_player_prefab);

			new_player3.transform.SetParent(m_players_holder);
			new_player3.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player3.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.East];
			PlayerView pv3 = new_player3.GetComponent<PlayerView>();
			Bot bot3 = new Bot ();
			bot3.SetBot (m_rows * m_columns, 0.75f);
			pv3.SetPlayer ((int)Player_Postions.East,bot3);
			m_players_views.Add (pv3);


		} else if (m_num_players == 4) {

			GameObject new_player = (GameObject)Instantiate(m_player_prefab);

			new_player.transform.SetParent(m_players_holder);
			new_player.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.South];
			PlayerView pv = new_player.GetComponent<PlayerView>();
			pv.SetPlayer ((int)Player_Postions.South,null);
			m_players_views.Add (pv);

			GameObject new_player2 = (GameObject)Instantiate(m_player_prefab);

			new_player2.transform.SetParent(m_players_holder);
			new_player2.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player2.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.West];
			PlayerView pv2 = new_player2.GetComponent<PlayerView>();
			Bot bot2 = new Bot ();
			bot2.SetBot (m_rows * m_columns, 0.75f);
			pv2.SetPlayer ((int)Player_Postions.West,bot2);
			m_players_views.Add (pv2);

			GameObject new_player3 = (GameObject)Instantiate(m_player_prefab);

			new_player3.transform.SetParent(m_players_holder);
			new_player3.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player3.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.North];
			PlayerView pv3 = new_player3.GetComponent<PlayerView>();
			Bot bot3 = new Bot ();
			bot3.SetBot (m_rows * m_columns, 0.75f);
			pv3.SetPlayer ((int)Player_Postions.North,bot3);
			m_players_views.Add (pv3);

			GameObject new_player4 = (GameObject)Instantiate(m_player_prefab);

			new_player4.transform.SetParent(m_players_holder);
			new_player4.GetComponent<RectTransform> ().localScale = new Vector3 (1f, 1f, 1f);
			new_player4.GetComponent<RectTransform> ().localPosition = m_player_pos [(int)Player_Postions.East];
			PlayerView pv4 = new_player4.GetComponent<PlayerView>();
			Bot bot4 = new Bot ();
			bot4.SetBot (m_rows * m_columns, 0.75f);
			pv4.SetPlayer ((int)Player_Postions.East,bot4);
			m_players_views.Add (pv4);

		}


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

	public List<PlayerView> Players_views {
		get {
			return m_players_views;
		}
	}



	public CardView[] Card_views_arr {
		get {
			return m_card_views_arr;
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
}
