using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MemGame.models;


namespace MemGame.views
{

public class LobbyItemView : MonoBehaviour {

	[SerializeField]TMP_Text	m_title_text=null;
	[SerializeField]TMP_Text	m_table_size_text=null;
	[SerializeField]TMP_Text	m_players_text=null;
	[SerializeField]TMP_Text	m_match_text=null;
	[SerializeField]TMP_Text	m_speed_text=null;

	[SerializeField]int			m_table_index=0;
	[SerializeField]int			m_table_rows=0;
	[SerializeField]int			m_table_cols=0;
	[SerializeField]int			m_players=0;
	[SerializeField]int			m_match=0;
	[SerializeField]int			m_speed=0;

	// Use this for initialization
	void Start () 
	{
		m_title_text.text = "Table #" + m_table_index;
		m_table_size_text.text = "Table Size  <sprite=0> " + m_table_rows + "*" +m_table_cols;

		if (m_players == 2)
			m_players_text.text = "Players <sprite=4> <sprite=2>";
		else if(m_players==3)
			m_players_text.text = "Players <sprite=4> <sprite=2> <sprite=4>";
		else if(m_players==4)
			m_players_text.text = "Players <sprite=4> <sprite=2> <sprite=4> <sprite=2>";

		if (m_match == 2)
			m_match_text.text = "Match <sprite=3>+<sprite=3>";
		else if(m_match==3)
			m_match_text.text = "Match <sprite=3>+<sprite=3>+<sprite=3>";
		else if (m_match==4)
			m_match_text.text = "Match <sprite=3>+<sprite=3>+<sprite=3>+<sprite=3>";

		if (m_speed == 1)
			m_speed_text.text = "Speed <sprite=1>";
		else if(m_speed==2)
			m_speed_text.text = "Speed <sprite=1> <sprite=1>";
		else if(m_speed==3)
			m_speed_text.text = "Speed <sprite=1> <sprite=1> <sprite=1>";
	}
	
	public void TableClicked()
	{
		Debug.Log ("Table Clicked");

		ManagerView.Instance.SetTableParams (m_table_rows, m_table_cols, m_players, m_match, m_speed);
		ManagerView.Instance.StartButtonClicked ();
	}
}
}