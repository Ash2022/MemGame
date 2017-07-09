using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMemory  {


	int 		m_initial_memory;
	int 		m_decline_rate;


	public BotMemory(int initial_memory, int decline_rate)
	{
		m_initial_memory = initial_memory;
		m_decline_rate = decline_rate;
	}

	public int Decline_rate {
		get {
			return m_decline_rate;
		}
		set {
			m_decline_rate = value;
		}
	}

	public int Initial_memory {
		get {
			return m_initial_memory;
		}
		set {
			m_initial_memory = value;
		}
	}
}
