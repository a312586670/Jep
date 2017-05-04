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

using System;

namespace Jep.Net.Email
{
	/// <summary>
	/// A stream to send MimeMessage to smtp server
	/// </summary>
	[Serializable]
	public class SMTPWriter : System.IO.TextWriter
	{
		/// <summary>
		/// The main writer
		/// </summary>
 		System.IO.StreamWriter m_writer = null;

		/// <summary>
		/// A writer for debug purpose. All text writtten to the SMTPWriter is 
		/// written on this Stream;
		/// </summary>
		System.IO.TextWriter m_debugStream = null;  //= new System.IO.StreamWriter ("debug.eml", false, System.Text.Encoding.ASCII);
		
		/// <summary>
		/// The writer encoding
		/// </summary>
		public override System.Text.Encoding Encoding
		{
			get
			{
				return m_writer.Encoding;
			}
		}
		/// <summary>
		/// A writer for debug purpose. All text writtten to the SMTPWriter is 
		/// written on this Stream.
		/// It is useful to debug SMTP formatting
		/// 
		/// 
		/// </summary>
		/// <example>
		/// <code>
		/// SMTPWriter wr = new SMTPWriter (smtp);
		/// wr.DebugStream = = new System.IO.StreamWriter ("debug.eml", false, System.Text.Encoding.ASCII);
		/// wr.Write (Message);
		/// </code>
		/// </example>
		public System.IO.TextWriter DebugStream
		{
			set
			{
				m_debugStream = value;
			}
			get
			{
				return (m_debugStream);
			}
		}

		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="wr">A streamwriter to the SMTP server</param>
		public SMTPWriter(System.IO.StreamWriter wr)
		{
			m_writer = wr;	
			m_writer.AutoFlush = true;
		}
		
		/// <summary>
		/// Close the current stream. Leaving the network stream , DebugStream and DialogStream open
		///  
		/// </summary>
		public override void Close()
		{
			
			m_writer = null;
		}

		/// <summary>
		/// Flush all data to the server
		/// </summary>
		public override void Flush ()
		{
			if (m_debugStream != null)
				m_debugStream.Flush ();
		
			m_writer.Flush ();
		}
		
		/// <summary>
		/// Writes data to the server.
		/// </summary>
		/// <param name="st"></param>
		public override void Write(string st)
		{
			// avoid <CR><LF>.<CR><LF> in message data
			if (st.StartsWith ("\r\n"))
			{
				
				if (m_debugStream != null)
					m_debugStream.Write (st);
		
				m_writer.Write (st);
			}
			else if (st.StartsWith ("."))
			{
				if (m_debugStream != null)
					m_debugStream.Write (".{0}", st);
				
				m_writer.Write (".{0}", st);
			}
			else
			{
				if (m_debugStream != null)
					m_debugStream.Write (st);
				
				m_writer.Write (st);
			}
		}

	}
}
