using System;


namespace MemGame.models
{

	public class Player
	{
		bool	m_is_human;
		Bot		m_bot=null;
		int		m_position;
		int		m_server_position=0;

		public Player (int pos_index,bool human, Bot bot=null)
		{
			m_position = pos_index;
			m_is_human = human;
			if (bot != null) {
				m_bot = new Bot ();
				m_bot = bot;
			}

		}

		public Bot Bot {
			get {
				return m_bot;
			}
		}

		public bool Is_human {
			get {
				return m_is_human;
			}
		}
	}
}

