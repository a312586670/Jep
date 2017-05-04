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
	/// Transfer encoding enumeration
	/// </summary>
	public enum MimeTransferEncoding
	{
		/// <summary>
		/// 7 Bits text
		/// </summary>
		Ascii7Bit,

		/// <summary>
		/// Text encoded in quoted printable
		/// </summary>
		QuotedPrintable,

		/// <summary>
		/// Text or binary encoded in base64
		/// </summary>
		Base64,
	}
	
	/// <summary>
	/// Text type enumeration
	/// </summary>
	public enum MimeTextContentType
	{
		/// <summary>
		/// Plain text
		/// </summary>
		TextPlain,

		/// <summary>
		/// Html encoded text
		/// </summary>
		TextHtml
	}

	
	/// <summary>
	/// A class to deal with various Mime constant
	/// </summary>
	/// <remarks>Private constructor. All methods are static</remarks>
	public class MimeConstant
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>Private constructor. All methods are static</remarks>
		private MimeConstant () // all methods are static
		{
		}

		
		/// <summary>
		/// Returns a string identifying a Mime text type
		/// </summary>
		/// <param name="type">A text type</param>
		/// <returns>A string identifying the Mime text type</returns>
		public static string GetContentTypeId (MimeTextContentType type)
		{
			switch (type)
			{
				case MimeTextContentType.TextHtml:
					return "text/html";
				case MimeTextContentType.TextPlain:
					return "text/plain";
				default:
					return "";	// default to text/plain in mime standard
			}
		}

		/// <summary>
		/// Returns a string identifying a Mime transfer encoding
		/// </summary>
		/// <param name="encoding">A Mime transfer encoding</param>
		/// <returns>A string identifying the Mime transfer encoding</returns>
		public static string GetTransferEncodingId (MimeTransferEncoding encoding)
		{
			switch (encoding)
			{
				case MimeTransferEncoding.Ascii7Bit:
					return "7Bit";
				case MimeTransferEncoding.QuotedPrintable:
					return "quoted-printable";
				case MimeTransferEncoding.Base64:
					return "Base64";
				default:
					return "";	// default to 7bit in mime standard
									   
			}
		}
	}
}