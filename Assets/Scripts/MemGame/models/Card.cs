using System;

namespace  MemGame.models
{
	public class Card
	{
		int		m_image_index;
		int		m_table_pos_index;

		public Card (int image_index=0, int pos_index=-1)
		{
			m_image_index = image_index;
			m_table_pos_index = pos_index;
		}


		public int Image_index {
			get {
				return m_image_index;
			}
			set {
				m_image_index = value;
			}
		}

		public int Table_pos_index {
			get {
				return m_table_pos_index;
			}
			set {
				m_table_pos_index = value;
			}
		}
	}
}

