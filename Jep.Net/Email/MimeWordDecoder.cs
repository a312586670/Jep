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
using System.Text;
using System.Text.RegularExpressions;

namespace Jep.Net.Email.MimeWord
{
	/// <summary>
	/// Decodes "Mime Word" encoded text
	/// </summary>
	public class MimeWordDecoder
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MimeWordDecoder()
		{
			
		}

		/// <summary>
		/// Tests if a string is Mime Word encoded
		/// </summary>
		/// <param name="code">The string to test</param>
		/// <returns>True if the string is MimeWord encoded, otherwise false</returns>
		public static bool IsMimeWord (string code)
		{
			Regex r = new Regex (@"=\?(.*)\?([bqBQ])\?(.*)\?=");
			if (r.IsMatch (code))
				return (true);
			return (false);
		}
		
		/// <summary>
		/// Returns the charset of the code
		/// </summary>
		/// <param name="code">A quoted printable string</param>
		/// <returns>The first charset found in the string</returns>
		public static string GetCharset (string code)
		{
			Regex r = new Regex (@"=\?(.*)\?([bqBQ])\?(.*)\?=");
			Match m = r.Match (code);
			
			if (! m.Success)
				return (null); // this is not a "Mime Word" encoded text
			
			return (m.Result ("$1"));
		}

		/// <summary>
		/// Decodes a "Mime word" encoded string
		/// </summary>
		/// <param name="code">A mime word encoded string</param>
		/// <returns>The decoded string</returns>
		/// <example>
		/// <code>
		/// MimeWordDecoder dec = new MimeWordDecoder ();
		/// 
		/// String subject = dec.DecodeWord ("=?iso-8859-1?q?Youpi=20it=20work?=");
		/// Console.Out.Writeline (subject);
		/// </code>
		/// Output:
		/// Youpi it work
		/// </example>
		public static string DecodeWord (string code)
		{
			Regex r = new Regex (@"=\?(.*)\?([bqBQ])\?(.*)\?=");
			Match m = r.Match (code);
			
			if (! m.Success)
				return (code); // this is not a "Mime Word" encoded text
			
			string CharsetString =  m.Result ("$1");
			string EncodingString = m.Result("$2");
			string Src = m.Result ("$3");
			string Text = null;

			EncodingIdentifier		EncodingId;
		
			// search encoding
			if (EncodingString == "b" ||
				EncodingString == "B")
				EncodingId = EncodingIdentifier.Base64;
			else if (EncodingString == "q" ||
				EncodingString == "Q")
				EncodingId = EncodingIdentifier.QuotedPrintable;
			else
				return (code); // invalid encoding

			// decode the string
			switch (EncodingId)
			{
				case EncodingIdentifier.QuotedPrintable:
					Text = QPEncoder.Decode (Src, CharsetString);
					break;
				case EncodingIdentifier.Base64:
					Text = Base64Decode (Src, CharsetString);
					break;
			}

			return (Text);
		}


		/// <summary>
		/// Decodes the "Mime word" encoded string "code"
		/// </summary>
		/// <param name="code">A mime word encoded string</param>
		/// <returns>The decoded string</returns>
		/// <example>
		/// <code>
		/// MimeWordDecoder dec = new MimeWordDecoder ();
		/// 
		/// String subject = dec.Decode ("=?iso-8859-1?q?Youpi=20it=20work?= =?iso-8859-15?q?fine?=");
		/// Console.Out.Writeline (subject);
		/// </code>
		/// Output:
		/// Youpi it work fine
		/// </example>
		public static string Decode (string code)
		{
			if (code.Length == 0)
				return code;

			StringBuilder res = new StringBuilder (code.Length);
			int p = 0;
			
			while (p < code.Length)
			{
				int pos = code.IndexOf ("=?", p);
				if (pos == -1)
				{
					res.Append (code.Substring (p, code.Length - p));
					p = code.Length;
				}
				else 
				{
					if (pos > p)
					{
						res.Append (code.Substring (p, pos - p));
						p = pos;
					}
					
					int e;

					e = code.IndexOf ("?=", p);
					if (e != -1)
						e += "?=".Length;
					while (e != -1 && ! IsMimeWord (code.Substring (p, e -p)) && e < code.Length)
					{
						e = code.IndexOf ("?=", e + 1);
						if (e != -1)
							e += "?=".Length;
					}
					if (e != -1 && e > p)
					{
						
						res.Append (DecodeWord (code.Substring (p, e - p)));
						p = e;
						if ((e = code.IndexOf ("\r\n", p)) == p) // remove CRLF SPACE
						{
							p += "\r\n".Length;
						}
						else if ((e = code.IndexOf (" =?", p)) == p) // remove single SPACE
						{
							p += " ".Length;
						}
					}
					else
					{
						res.Append (code.Substring (p, code.Length - p));
						p = code.Length;
					}
				}
				
			}

			return (res.ToString ());
		}

		/// <summary>
		/// Decodes a string encoded in base64
		/// </summary>
		/// <param name="Src">A string containing Base64 code</param>
		/// <param name="Charset">The charset of the string to decode</param>
		/// <returns>The decoded string</returns>
		protected static string Base64Decode (string Src, string Charset)
		{
			Byte []					code	= Convert.FromBase64String (Src);
			string					res		= Src;
			System.Text.Encoding	dec;
			try
			{
				dec = System.Text.Encoding.GetEncoding (Charset);
			}
			catch (NotSupportedException)
			{
				// decode in default system charset
				dec = System.Text.Encoding.Default;
			}
			catch (ArgumentException)
			{
				// decode in default system charset
				dec = System.Text.Encoding.Default;
			}
			
			res = dec.GetString (code);
			return (res);
		}
	}
}
