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
using System.Text;

namespace Jep.Net.Email
{
	/// <summary>
	/// The class provide method to encode/decode Quoted Printable
	/// </summary>
	public class QPEncoder
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public QPEncoder()
		{
			
		}
		
		/// <summary>
		/// Tests if a string is US-ASCII encoded
		/// </summary>
		/// <param name="src">The string to test</param>
		/// <returns>true if the the string is US-ASCII. Otherwise false</returns>
		public static bool IsAscii (string src)
		{
			for (int i = 0; i < src.Length; i++)
			{
				if (src[i] < 32 || src[i] > 126)
				{
					return (false);
				}
			}
			return (true);
		}
		
		/// <summary>
		/// The method encode a string into a quoted printable string. The used charset is the default system charset
		/// </summary>
		/// <param name="Src">A string to encode</param>
		/// <param name="Charset">The charset name (ie "utf-8" or "iso-8859-1")</param>
		/// <returns>A quoted printable string</returns>
		public static string Encode (string Src, string Charset)
		{
			System.Text.Encoding coder = System.Text.Encoding.GetEncoding (Charset);
			Byte [] code = coder.GetBytes (Src);
			System.Text.StringBuilder r = new System.Text.StringBuilder ();
			for (int i = 0; i < code.Length; i++)
			{
				
				if (code[i] < 33 || code[i] == '=' || code[i] =='?' || code[i] == '_' || code[i] > 126)
					r.AppendFormat ("={0}",  code[i].ToString ("X"));
				else
					r.Append ((char) code[i]);
				
			}
			return (r.ToString ());
		}
		
		/// <summary>
		/// The method encode a string into a quoted printable string. 
		/// </summary>
		/// <param name="Src">A string to encode</param>
		/// <param name="Encoding">The charset encoder</param>
		/// <returns>A quoted printable string</returns>
		public static string Encode (string Src, System.Text.Encoding Encoding)
		{
			Byte [] code = Encoding.GetBytes (Src);
			System.Text.StringBuilder r = new System.Text.StringBuilder ();
			for (int i = 0; i < code.Length; i++)
			{
				
				if (code[i] < 33 || code[i] == '=' || code[i] =='?' || code[i] == '_' || code[i] > 126)
					r.AppendFormat ("={0}",  code[i].ToString ("X"));
				else
					r.Append ((char) code[i]);
				
			}
			return (r.ToString ());
		}
		/// <summary>
		/// The method encodes a char into a quoted printable string. 
		/// </summary>
		/// <param name="Src">A char to encode</param>
		/// <param name="Encoding">The charset encoder</param>
		/// <returns>A quoted printable string</returns>
		public static string Encode (char Src, System.Text.Encoding Encoding)
		{
			// TODO: write a test for Encode
			string		str = new string (Src, 1);
			Byte []		code = Encoding.GetBytes (str);
			System.Text.StringBuilder r = new System.Text.StringBuilder (16);
			
			for (int i = 0; i < code.Length; i++)
			{
				
				if (code[i] < 33 || code[i] == '=' || code[i] =='?' || code[i] == '_' || code[i] > 126)
					r.AppendFormat ("={0}",  code[i].ToString ("X"));
				else
					r.Append ((char) code[i]);
				
			}
			return (r.ToString ());
		}
		/// <summary>
		/// The method encodes a char into a quoted printable string. 
		/// </summary>
		/// <param name="Src">A char to encode</param>
		/// <param name="Charset">The charset name (ie "utf-8" or "iso-8859-1")</param>
		/// <returns>A quoted printable string</returns>
		public static string Encode (char Src, string Charset)
		{
			string str = new string (Src, 1);
			System.Text.Encoding coder = System.Text.Encoding.GetEncoding (Charset);
			Byte [] code = coder.GetBytes (str);
			System.Text.StringBuilder r = new System.Text.StringBuilder ();
			for (int i = 0; i < code.Length; i++)
			{
				
				if (code[i] < 33 || code[i] == '=' || code[i] =='?' || code[i] == '_' || code[i] > 126)
					r.AppendFormat ("={0}",  code[i].ToString ("X"));
				else
					r.Append ((char) code[i]);
				
			}
			return (r.ToString ());
		}

		/// <summary>
		/// The method encodes a string into a quoted printable string. The used charset is UTF8
		/// </summary>
		/// <param name="Src">A string to encode</param>
		/// <returns>A quoted printable string</returns>
		protected static string Encode (string Src)
		{
			System.Text.Encoding coder = System.Text.Encoding.UTF8; 
			Byte [] code = coder.GetBytes (Src);
			System.Text.StringBuilder r = new System.Text.StringBuilder ();
			for (int i = 0; i < code.Length; i++)
			{
				
				if (code[i] < 33 || code[i] == '=' || code[i] =='?' || code[i] == '_' || code[i] > 126)
					r.AppendFormat ("={0}",  code[i].ToString ("X"));
				else
					r.Append ((char) code[i]);
				
			}
			return (r.ToString ());
		}

		/// <summary>
		/// Decodes a quoted printable string
		/// </summary>
		/// <param name="Src">A string containing quoted printable text</param>
		/// <param name="Charset">The charset of the string</param>
		/// <returns>A string containing the decoded string</returns>
		public static string Decode (string Src, string Charset)
		{
			StringBuilder	res		= new StringBuilder (Src.Length);
			StringBuilder	tmp		= new StringBuilder (4);
			byte []			code	= new byte[Src.Length];
			int				l		= 0;
			int				pos		= 0;
			Encoding		dec;
			int				p;
			
			try
			{
				dec = Encoding.GetEncoding (Charset);
			}
			catch (NotSupportedException)
			{
				dec = Encoding.Default;
			}
			catch (ArgumentException)
			{
				dec = Encoding.Default;
			}

			while (pos < Src.Length)
			{
				switch (Src[pos])
				{
					case '=':
						tmp.Length = 0;
						for (p = pos + 1; p < Src.Length && p < pos + 3; p++)
						{
							if (Char.IsDigit (Src[p]) ||			// two hex digit follow
								(Src[p] >= 'a' && Src[p] <= 'f') ||
								(Src[p] >= 'A' && Src[p] <= 'F') )
							{
								tmp.Append (Src[p]);
							}
						
						}
						
						code[l++] = Convert.ToByte (tmp.ToString (), 16);
						

						//next char
						pos = p;
					
						break;
					case '_': 
						code[l++] = (byte) ' '; // translate _ in SPACE
						//next char
						pos++;
						break;
					default:
						code[l++] = (byte) Src[pos];
						
						//next char
						pos++;
					
						break;
				}
				
			}

			return (dec.GetString (code, 0, l));
			
		}
	}
}
