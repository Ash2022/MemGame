using System;
using UnityEngine;

namespace MemGame.controllers
{
	public class ScoreController : MonoBehaviour
	{

		static ScoreController m_instance;

		public static ScoreController 		Instance {
			get {

				if (m_instance == null) {
					ScoreController manager = GameObject.FindObjectOfType<ScoreController> ();
					m_instance = manager.GetComponent<ScoreController> ();
				}
				return m_instance;
			}
		}

		public ScoreController ()
		{
		}
	}
}

