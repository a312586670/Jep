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
using System.Text.RegularExpressions;
using Jep.Net.Email.MimeWord;

namespace Jep.Net.Email 
{

	

	/// <summary>
	/// The class handle an RFC2822 email address
	/// </summary>
	public class MailAddress
	{
		// TODO : build a correct charset for everybody

		/// <summary>
		/// A string containing the source of the adress
		/// </summary>
		/// <remarks>
		/// src contain the original string of the address. It may contain a
		/// full adress (name + mailbox) or a partial address (mailbox)
		/// </remarks>
		protected 	string 	m_src = "";
		
		/// <summary>
		/// A string containing the name part of the address
		/// </summary>
		protected 	string 	m_name = "";

		/// <summary>
		/// A string containing the mailbox part of the address
		/// </summary>
		protected 	string 	m_mailbox ="";

		/// <summary>
		/// A string containing the domain part of the address
		/// </summary>
		protected	string 	m_domain ="";

		/// <summary>
		/// True if the address is valid. False otherwise
		/// </summary>
		protected 	bool 	m_valid = false;

		/// <summary>
		/// Default constructor
		/// </summary>
		public MailAddress ()
		{

		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="src">A string containing an smtp address</param>
		/// <remarks>
		/// Valid address are:<br/>
		/// <code escaped="true">
		/// Toto &lt;toto@toto.com&gt;
		/// &lt;youpi@bad.trip.com&gt;
		/// </code>
		/// </remarks>
		/// <exception cref="MailAddressException">Invalid address syntax</exception>
		public MailAddress(string src)
		{
			Parse (src);		
		}

		/// <summary>
		/// Read an email address from string
		/// </summary>
		/// <param name="src">An email address string</param>
		/// <example>
		/// <code>
		/// MailAddress m = new MailAddress ();
		/// m.Parse ("Toto &lt;toto@tyty.com&gt;");
		/// </code>
		/// </example>
		public void Parse (string src)
		{
			if (src.IndexOf ("@") == -1)
				throw (new MailAddressException ("Invalid address"));
			if (src.IndexOf ("\r\n") != -1)
			{
				src = src.Replace ("\r", "");
				src = src.Replace ("\n","");
			}

			if (src.IndexOf ("<") == -1)
				m_src = "<" + src + ">";
			else
				m_src = src;

			// build domain
			Regex r = new Regex ("<.*@(.*)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Match m = r.Match (m_src);
			if (m.Success)
				m_domain =  m.Result ("$1");
			else
				throw new MailAddressException ("Invalid mailbox domain :" + src);

			// build mailbox
			r = new Regex ("<(.*)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			m = r.Match (m_src);
			if (m.Success)
				m_mailbox =  "<" + m.Result ("$1") +">";
			else
				throw new MailAddressException ("Invalid mailbox :" + m_src);

			//build name
			string name = "";
			r = new Regex ("(.*)<.*>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			m = r.Match (m_src);
			
			if (m.Success)
			{
				char [] space = {' '};
				name = m.Result ("$1");
				name = name.TrimEnd (space);
			}
			if (MimeWordDecoder.IsMimeWord (name))
				m_name = MimeWordDecoder.Decode (name);
			else
				m_name = name;

			m_valid = true;

			Save (); // rebuild src address

		}
		/// <summary>Constructor with name and email address</summary>
		/// <example>
		/// <code>MailAddress a = new MailAddress("Johny", "johny@toto.com");</code>
		/// </example>
		/// <param name="name">The name part of the address</param>
		/// <param name="mailbox">The mailbox part of the address</param>
		public MailAddress(string name, string mailbox)
		{
			m_name = name;
			if (mailbox[0] == '<')
				m_mailbox = mailbox;
			else
				m_mailbox = "<" + mailbox + ">";
			
			// build domain part
			Regex r = new Regex ("<.*@(.*)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Match m = r.Match (m_mailbox);
			if (m.Success)
				m_domain =  m.Result ("$1");
			else
				throw new MailAddressException ("Invalid mailbox domain :" + m_mailbox);

			// TODO: validate address syntax

			m_valid = true;
			Save (); // rebuild src address
		}

		
		/// <summary>
		/// A string containing the address in literal format
		/// </summary>
		public string Src
		{
			get { 
				return (m_src); 
			}
			set { 
				Parse (value);
			}
		}

		/// <summary>
		/// A string containing the name part of the address
		/// </summary>
		/// <example>
		/// <code>
		/// MailAddress m = new MailAddress ("Toto &lt;tyty@tata.com&gt;");
		/// Console.Out.WriteLine (m.Name);
		/// </code>
		/// Output : Toto
		/// </example>
		public string Name
		{
			get { 
				return(m_name); 
			}
			set { 
				m_name = value; 
				Save (); // rebuild src
			}
		}
		
		/// <summary>
		/// A string containing the domain part of the mailbox
		/// </summary>
		public string Domain
		{
			get { return m_domain; }
		}

		/// <summary>
		/// A string containing the mailbox part of the address
		/// </summary>
		public string Mailbox
		{
			get { return m_mailbox; }
		}

		/// <summary>
		/// Validity flag of the address
		/// </summary>
		public bool IsValid
		{
			get { return m_valid; }
		}

		/// <summary>
		/// Returns a string containing the MailAdress details
		/// </summary>
		/// <returns>A string containing the MailAddress details</returns>
		public override string ToString()
		{
			String st;
			if (m_name.Length > 0)
				st = m_name + " " + m_mailbox;
			else
				st = m_mailbox;
			return (st);	
		}

		/// <summary>
		/// The method builds the mime src (the valid RFC2822  address)
		/// </summary>
		protected void Save ()
		{
			if (m_name.Length > 0)
			{
				if (QPEncoder.IsAscii (m_name))
					m_src = m_name + " " + m_mailbox;
				else
					m_src = MimeWordEncoder.Encode (m_name, Config.defaultEncoding, EncodingIdentifier.Base64) + " " + m_mailbox;
			}
			else
				m_src = m_mailbox;

			m_valid = true;
		}
		
		/// <summary>
		/// Read adresses from header line and add each address in "List"
		/// </summary>
		/// <param name="List">The address list to initialize</param>
		/// <param name="Src">A string containing header line with address list</param>
		public void ReadAddresses (ref MailAddressList List, string Src)
		{
			string[] s = Src.Split (';');
			if (s == null)
			{
				List.Add (new MailAddress (Src));
			}
			else
			{
				foreach (string a in s)
				{
					int p;
					string tmp = a;
					while ((p = tmp.LastIndexOf ("\r\n")) != -1)
						tmp = tmp.Remove (p, 2);
					List.Add (new MailAddress(tmp));
				}
			}
		}
		
	}

}