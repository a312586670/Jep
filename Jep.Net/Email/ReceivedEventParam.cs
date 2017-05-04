//
// SmtPop	SMTP and POP library
//
// Copyright (C) 2004-2005 sillycoder	sillycoder@users.sourceforge.net 
//
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// 

namespace Jep.Net.Email
{
	/// <summary>
	/// The received event parameters
	/// </summary>
	public class ReceivedEventParam : System.EventArgs
	{
		/// <summary>
		/// The amount of byte already received
		/// </summary>
		private int m_received;

		/// <summary>
		/// The amount of byte of the message
		/// </summary>
		private int m_expected;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ByteReceived">The number of Bytes received</param>
		/// <param name="Bytexpected">The number of Bytes of the messages</param>
		public ReceivedEventParam(int ByteReceived, int Bytexpected)
		{
			m_received = ByteReceived;
			m_expected = Bytexpected;
		}

		/// <summary>
		/// // the total amount of byte received
		/// </summary>
		public int ByteReceived
		{
			get
			{
				return m_received;
			}
			set
			{
				m_received = value;
			}
		}

		/// <summary>
		/// The total amount of bytes expected (The message size in bytes)
		/// </summary>
		public int ByteExpected
		{
			get
			{
				return m_expected;
			}
			set
			{
				m_expected = value;
			}
		}


	}
}
