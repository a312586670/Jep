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

namespace Jep.Net.Email.MimeWord
{
	
	
	/// <summary>
	/// Encoding available for "Mime word" encoding
	/// </summary>
	public enum EncodingIdentifier
	{
		/// <summary>
		/// Quoted printable 
		/// </summary>
		QuotedPrintable,

		/// <summary>
		/// Base64
		/// </summary>
		Base64
	};

	

	/// <summary>
	/// Encodes string in "Mime Word"
	/// </summary>
	public class MimeWordEncoder
	{
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MimeWordEncoder()
		{
			
		}

		
		/// <summary>
		/// Encodes a string in "Mime Word"
		/// </summary>
		/// <param name="Src">The string to encode</param>
		/// <param name="Encoding">Encoding charset</param>
		/// <param name="TransfertEncoding">The transfert encoding</param>
		/// <returns>A MimeWord encoded string</returns>
		public static string Encode (string Src, System.Text.Encoding Encoding, EncodingIdentifier TransfertEncoding)
		{
			return Encode (Src, Encoding, TransfertEncoding, 78);
		}

		/// <summary>
		/// Encodes a string in "Mime Word"
		/// </summary>
		/// <param name="Src">The string to encode</param>
		/// <param name="Encoding">The string charset Encoding</param>
		/// <param name="TransfertEncoding">The transfert encoding</param>
		/// <param name="Maxlength">Maximum number of char beetween 2 CRLF</param>
		/// 
		/// <returns>A MimeWord encoded string</returns>
		public static string Encode (string Src, System.Text.Encoding Encoding, EncodingIdentifier TransfertEncoding, int Maxlength)
		{
			StringBuilder st = new StringBuilder (Src.Length);
			
			switch (TransfertEncoding)
			{
				case EncodingIdentifier.QuotedPrintable:
				{
					string suffix = "?=";
					string prefix = "=?" + Encoding.BodyName +"?q?";
					int len = prefix.Length + suffix.Length;
					
					st.Append (prefix);
					for (int i = 0; i < Src.Length; i++)
					{
						string tmp = QPEncoder.Encode (Src[i], Encoding);
						if (len  +  tmp.Length >= Maxlength)
						{
							st.AppendFormat ("{0}\r\n", suffix);
							len = prefix.Length + suffix.Length;
							st.Append (prefix);
						}
						st.Append (tmp);
						len += tmp.Length;
					}
					st.Append ("?=");
				}
					break;
				case EncodingIdentifier.Base64:
					
					st.AppendFormat ("=?{0}?b?{1}?=",
						Encoding.BodyName, 
						MimeEncoder.StringBase64 (Src, Encoding, 78)); 
							
					break;
					
				default:
					throw (new ArgumentOutOfRangeException ("TransfertEncoding", TransfertEncoding, "Invalid transfert identifier"));
					
			}

			return (st.ToString ());
		}
	}
}
