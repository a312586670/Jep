//
// SmtPop	SMTP and POP library
//
// Copyright (C) 2004-2006 sillycoder	sillycoder@users.sourceforge.net 
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

using System;
using System.IO;
using System.Net;

namespace Jep.Net.Email
{
	/// <summary>
	/// A stream to read one message from a pop3 server
	/// </summary>
	[Serializable]
	public class POPReader : TextReader
	{
		/// <summary>
		/// The total amount of byte read
		/// </summary>
		private int m_byteread = 0;

		/// <summary>
		/// The message size
		/// </summary>
		private int m_bytetotal = 0;


		/// <summary>
		/// Event fire when some data has been received from the server
		/// </summary>
		public event ReceivedEventHandler Received;
		
		/// <summary>
		/// The connection to the pop3 server
		/// </summary>
		protected System.IO.StreamReader	TCPStream;
		
		/// <summary>
		/// End of file flag
		/// </summary>
		protected bool						EOF = false;
		
		private void FireReceived ()
		{
			if (Received != null)
				Received (this, new ReceivedEventParam (m_byteread - 2, m_bytetotal));
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="stream">TCP stream to a POP3 server</param>
		/// <param name="ReceiveEvent">The receive event handler</param>
		/// <param name="MessageSize">The message size in byte</param>
		public POPReader(System.IO.StreamReader stream, ReceivedEventHandler ReceiveEvent, int MessageSize)
		{
			TCPStream = stream;
			Received = ReceiveEvent;
			m_byteread = 0;
			m_bytetotal = MessageSize;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="stream">TCP stream to a POP3 server</param>
		public POPReader(System.IO.StreamReader stream)
		{
			TCPStream = stream;
			m_byteread = 0;
			m_bytetotal = 0;
			Received = null;
		}
		/// <summary>
		/// Peek a char from the stream
		/// </summary>
		/// <returns>Char at current position</returns>
		public override System.Int32 Peek ()
		{
			if (EOF)
				return (-1);
			return (TCPStream.Peek ());
		}

		/// <summary>
		/// Read a line from the stream
		/// </summary>
		/// <returns>Line at current position</returns>
		public override string ReadLine ()
		{
			if (EOF)
				return null;

			string st = TCPStream.ReadLine ();
			if (st != null)
			{
				if (st == ".")
				{
					EOF = true;
					st = "";
					
					return (null);
					
				}
				
				m_byteread += (st.Length + 2); // CRLF discarded by read
				FireReceived ();
			}
			return st;
		}
		/// <summary>
		/// Read the entire stream
		/// </summary>
		/// <returns>The entire stream from current posiion</returns>
		public override string ReadToEnd ()
		{
			if (EOF)
				return ("");
			string st = "";
			string tmp;
			do 
			{
				tmp = TCPStream.ReadLine ();
				if (tmp == ".")
					EOF = true;
				else
					st += tmp + "\r\n";
				
				m_byteread += tmp.Length  + 2; // CRLF discarded by read
				FireReceived ();
			} while (! EOF);

			return (st);
		}
		/// <summary>
		/// Read from stream
		/// </summary>
		/// <param name="buf">Buffer</param>
		/// <param name="index">Position in buffer</param>
		/// <param name="count">Number of char to read</param>
		/// <returns>Number of char read</returns>
		public override System.Int32 Read (char [] buf, System.Int32 index, System.Int32 count)
		{
			if (EOF)
				return 0;

			char [] tmpbuf = new char[count];
			System.Int32 ret = TCPStream.Read (tmpbuf, 0, count);
			for (int i = 0; i < count; i++)
			{
				buf[i] = tmpbuf[i];

				if (i > 2)
				{
					if (tmpbuf[i] == '\r' || tmpbuf[i] == '\n')
						if (tmpbuf[i - 1] == '.' && tmpbuf[i - 2] == '\n')
							EOF = true;
				}
			}

			return (ret);
		}

		/// <summary>
		/// Show if it is possible to read the stream (eof status)
		/// </summary>
		public virtual  bool EndOfStream
		{
			get
			{
				return (EOF == false);
			}
		}
	}
}
