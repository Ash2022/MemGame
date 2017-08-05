using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using MemGame.controllers;
using MemGame.models;

namespace MemGame.views
{

	public class CardView : MonoBehaviour
	{

		[SerializeField]Image m_bg_card;


		[SerializeField]Button m_button;
		int m_row;
		int m_col;
		[SerializeField]Text m_row_ol_indicator = null;
		[SerializeField]GameObject	m_highlight = null;
		CanvasGroup m_canvas_group;

		Card m_card_model;

		public void SetCard (Card card)
		{
			m_card_model = card;

			m_row_ol_indicator.text = m_card_model.Table_pos_index.ToString ();
			m_canvas_group = GetComponent<CanvasGroup> ();
			//ShowCard (true);

		}

		public void HighLight (bool show, bool wrong)
		{
			if (wrong)
				m_highlight.GetComponent<Image> ().color = new Color (1f, 0, 0);
			else
				m_highlight.GetComponent<Image> ().color = new Color (1f, 1f, 1f);

			m_highlight.SetActive (show);
		}

		public void ShowCard (bool front)
		{
			if (front) {
				iTween.RotateTo (m_bg_card.gameObject, iTween.Hash ("y", 90, "easeType", "linear", "time", .1, "onComplete", "FlipAndRotate", "onCompleteTarget", this.gameObject));

			} else {
				iTween.RotateTo (m_bg_card.gameObject, iTween.Hash ("y", 90, "easeType", "linear", "time", .1, "onComplete", "FlipBackAndRotate", "onCompleteTarget", this.gameObject));
			}
		}

		private void FlipAndRotate ()
		{
			m_bg_card.sprite = ManagerView.Instance.Cards_images [m_card_model.Image_index];
			iTween.RotateTo (m_bg_card.gameObject, iTween.Hash ("y", 180, "easeType", "linear", "time", .1));
		}

		private void FlipBackAndRotate ()
		{
			m_bg_card.sprite = ManagerView.Instance.Card_back;
			iTween.RotateTo (m_bg_card.gameObject, iTween.Hash ("y", 0, "easeType", "linear", "time", .1));
			m_button.interactable = true;
		}

		// Use this for initialization
		public void CardClicked ()
		{

			if (GameController.Instance.Human_can_play) {
				Debug.Log ("Card Clicked Pos " + m_card_model.Table_pos_index);
				m_button.interactable = false;
				ShowCard (true);
				GameController.Instance.HumanPickedCard (m_card_model.Table_pos_index);
			}
		}

		public void CardMatchedAnim ()
		{
			transform.SetAsLastSibling ();

			iTween.RotateAdd (m_bg_card.gameObject, iTween.Hash ("z", 500, "easeType", iTween.EaseType.easeInCirc, "time", .35, "onComplete", "CardMatchAnimDone", "onCompleteTarget", this.gameObject));
			iTween.ScaleTo (m_bg_card.gameObject, iTween.Hash ("x", 3, "y", 3, "easeType", iTween.EaseType.easeInCirc, "time", .35, "onComplete", "CardMatchAnimDone", "onCompleteTarget", this.gameObject));
			iTween.ValueTo (m_bg_card.gameObject, iTween.Hash ("from", 1, "to", 0, "easeType", iTween.EaseType.easeInCirc, "time", .35, "onupdate", "UpdateAlpha", "onupdatetarget", this.gameObject));

		}

		private void UpdateAlpha (float value)
		{
			m_canvas_group.alpha = value;
		}

		private void CardMatchAnimDone ()
		{
			gameObject.SetActive (false);
		}

		public int Pos_index {
			get {
				return m_card_model.Table_pos_index;
			}
			set {
				m_card_model.Table_pos_index = value;
			}
		}

		public int Image_index {
			get {
				return m_card_model.Image_index;
			}
		}

		public Button Button {
			get {
				return m_button;
			}
		}
	}
}