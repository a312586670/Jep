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

namespace Jep.Net.Email
{
    /// <summary>
    /// Provide encoder for unicode string
    /// </summary>
    internal class MimeEncoder
    {
        /// <summary>
        /// Constructs an encoder instance
        /// </summary>
        public MimeEncoder()
        {
        }





        /// <summary>
        /// Converts a string into base 64 using the specified charset
        /// </summary>
        /// <param name="Src">The string to encode</param>
        /// <param name="Encoding">The charset of the string to encode</param>
        /// <param name="MaxLineLength">Maximum line lengh in base64 code</param>
        /// <returns>A string containing the base64 code for "src"</returns>
        /// <example>
        /// <code>
        /// StringBase64 (Src, System.Text.Encoding.Default, 78);
        /// </code>
        ///</example>
        public static string StringBase64(string Src, System.Text.Encoding Encoding, int MaxLineLength)
        {
            Byte[] code = Encoding.GetBytes(Src);
            return (ByteBase64(code, MaxLineLength));
        }

#if false
		/// <summary>
		/// Converts a string into base 64 using the specified charset
		/// </summary>
		/// <param name="Src">The string to encode</param>
		/// <param name="Charset">The charset of the string to encode</param>
		/// <param name="MaxLineLength">Maximum line lengh in base64 code</param>
		/// <returns>A string containing the base64 code for "src"</returns>
		/// <example>
		/// <code>
		/// StringBase64 (Src, "UTF-8", 78);
		/// StringBase64 (Src, "Windows-1252", 78);
		/// StringBase64 (Src, "iso-8859-15", 78);
		/// </code>
		///</example>
		public static string StringBase64 (string Src, string Charset, int MaxLineLength)
		{
			Byte[] code = System.Text.Encoding.GetEncoding (Charset).GetBytes (Src);
			return (ByteBase64 (code, MaxLineLength));
		}
#endif

        /// <summary>
        /// Converts an array of byte in Base64. 
        /// </summary>
        /// <param name="buf">Array of byte to encode</param>
        /// <param name="MaxLineLength">Maximum length of a line in Base64</param>
        /// <returns>A string containing the base64 code for "buf"</returns>
        public static string ByteBase64(Byte[] buf, int MaxLineLength)
        {
            StringBuilder build = new StringBuilder();
            int bloclength = ((MaxLineLength * 3) / 4) - 2;
            int wr;

            bloclength -= bloclength % 3;	// ensure bloclength divideable by 3

            for (wr = 0; wr + bloclength < buf.Length; wr += bloclength)
            {
                String tmp = Convert.ToBase64String(buf, wr, bloclength);
                //if (tmp.IndexOf ("==") != -1)
                //	tmp.Replace ("==","");

                build.Append(tmp);
                build.Append("\r\n");
            }
            if (wr < buf.Length)
            {
                build.Append(Convert.ToBase64String(buf, wr, buf.Length - wr));

            }


            return (build.ToString());
        }


    }
}
