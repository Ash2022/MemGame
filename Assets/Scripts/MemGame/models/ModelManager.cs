using System;
using System.Collections.Generic;
using UnityEngine;

namespace MemGame.models
{
	public class ModelManager :MonoBehaviour
	{
		Table	m_table;
		List<BotMemory>	m_bots = new List<BotMemory>();


		public enum BotLevel
		{
			Geniuos,
			smart,
			medium,
			lame,
			retard
		}

		static ModelManager m_instance;

		public static ModelManager 		Instance {
			get {

				if (m_instance == null) {
					ModelManager manager = GameObject.FindObjectOfType<ModelManager> ();
					m_instance = manager.GetComponent<ModelManager> ();
				}
				return m_instance;
			}
		}

		public ModelManager ()
		{
		}

		public void Init()
		{
			GenerateBots ();
		}

		private void GenerateBots()
		{
			m_bots.Add (new BotMemory (100, 0,70)); // never forget
			m_bots.Add (new BotMemory (90, 1,70));
			m_bots.Add (new BotMemory (70, 2,70));
			m_bots.Add (new BotMemory (60, 2,70)); 
			m_bots.Add (new BotMemory (50, 3,70)); // never remember
		}

		public Table Table {
			get {
				return m_table;
			}
			set {
				m_table = value;
			}
		}

		public List<BotMemory> Bots {
			get {
				return m_bots;
			}
		}
	}
}

