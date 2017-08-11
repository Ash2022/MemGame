using System;

namespace  MemGame.models
{
	public class Card
	{
		int		m_image_index;
		bool	m_open=false;
		bool	m_matched=false;

		public Card (int image_index=0)
		{
			m_image_index = image_index;

		}

		public Action<bool>	OnCardOpen;
		public Action		OnCardMatched;

		public int Image_index {
			get {
				return m_image_index;
			}
			set {
				m_image_index = value;
			}
		}

		public bool Open {
			get {
				return m_open;
			}
			set {
				m_open = value;
				if (OnCardOpen != null)
					OnCardOpen (m_open);
			}
		}

		public bool Matched {
			get {
				return m_matched;
			}
			set {
				m_matched = value;
				if (OnCardMatched != null)
					OnCardMatched ();
			}
		}
	}
}

