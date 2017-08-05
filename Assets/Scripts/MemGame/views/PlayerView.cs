using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MemGame.models;

namespace MemGame.views
{

	public class PlayerView : MonoBehaviour
	{

		[SerializeField]Image 	m_bg_image = null;
		int 					m_player_avatar_image_index;
		[SerializeField]Image 	m_timer = null;

		int m_score;
		int m_multi;
		[SerializeField]Text m_score_text = null;
		[SerializeField]Text m_bot_level_text = null;
		Player m_player;

		public void SetPlayer (Player player,int index, int score = 0)
		{
			m_player = player;
			m_player_avatar_image_index = index;
			m_score = score;
			SetDataDisplay ();
		}

		private void SetDataDisplay ()
		{
			m_bg_image.sprite = ManagerView.Instance.Avatars_images [m_player_avatar_image_index];
			m_score_text.text = m_score.ToString ();

			if (m_player.Bot != null)
				m_bot_level_text.text = Bot.Bot_level.ToString ();
			else
				m_bot_level_text.text = "Human";
		}

		public void ShowHideTimer (bool show)
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
				return m_player.Bot;
			}
		}


		public int Score {
			get {
				return m_score;
			}
			set {
				m_score = value;
				m_score_text.text = m_score.ToString ();
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

		public void PlayerClicked ()
		{

		}
	}
}