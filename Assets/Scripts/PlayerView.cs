using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour {

	[SerializeField]Image	m_bg_image=null;
	int						m_player_image_index;
	[SerializeField]Image	m_timer=null;
	Bot 					m_bot;
	int						m_score;
	int						m_multi;
	[SerializeField]Text	m_score_text;


	public void SetPlayer(int index,Bot bot, int score=0)
	{
		m_player_image_index = index;
		m_bot = bot;
		m_score = score;
		SetDataDisplay ();
	}

	private void SetDataDisplay()
	{
		m_bg_image.sprite = ManagerView.Instance.Avatars_images [m_player_image_index];
		m_score_text.text = "Score: "+ m_score;
	}

	public void ShowHideTimer(bool show)
	{
		m_timer.gameObject.SetActive (show);
	}

	public Image Timer {
		get {
			return m_timer;
		}
	}


	public Bot Bot {
		get {
			return m_bot;
		}
	}


	public int Score {
		get {
			return m_score;
		}
		set {
			m_score = value;
			m_score_text.text = "Score: "+ m_score;
		}
	}

	public int Multi {
		get {
			return m_multi;
		}
		set {
			m_multi = value;
		}
	}
}
