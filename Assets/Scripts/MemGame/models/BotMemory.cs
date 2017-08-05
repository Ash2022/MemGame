using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemGame.models
{

public class BotMemory  {


	int 		m_initial_memory;
	int 		m_decline_rate;
	int 		m_forget_value;


	public BotMemory(int initial_memory, int decline_rate,int forget_value)
	{
		m_initial_memory = initial_memory;
		m_decline_rate = decline_rate;
		m_forget_value = forget_value;
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

	public int Forget_value {
		get {
			return m_forget_value;
		}
		set {
			m_forget_value = value;
		}
	}
}
}