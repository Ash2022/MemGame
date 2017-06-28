using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour {

	[SerializeField]Image		m_bg_card;
	int							m_image_index;
	int							m_pos_index;

	[SerializeField]Button		m_button;
	int 						m_row;
	int							m_col;
	[SerializeField]Text		m_row_ol_indicator=null;


	public void SetCard(int image_index,int pos_index)
	{
		m_image_index = image_index;
		m_pos_index = pos_index;
		m_row_ol_indicator.text = "p" + m_pos_index;


	}

	public void ShowCard(bool front)
	{
		if (front)
			m_bg_card.sprite = ManagerView.Instance.Cards [m_image_index];
		else {
			m_bg_card.sprite = ManagerView.Instance.Card_back;
			m_button.interactable = true;
		}
	}

	// Use this for initialization
	public void CardClicked () {

		if (GameController.Instance.Human_can_play) {
			Debug.Log ("Card Clicked Pos " + m_pos_index);
			m_button.interactable = false;
			ShowCard (true);
			GameController.Instance.HumanPickedCard (m_pos_index);

		}
		
	}

	public int Pos_index {
		get {
			return m_pos_index;
		}
	}

	public int Image_index {
		get {
			return m_image_index;
		}
	}
}
