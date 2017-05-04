using System;
using System.Text;

namespace Jep.Net.Email
{
	/// <summary>
	/// Provide decoder for unicode string
	/// </summary>
	internal class MimeDecoder
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MimeDecoder()
		{
			
		}

		/// <summary>
		/// Converts iso-8859-1 buffer into a string
		/// </summary>
		/// <param name="src">A buffer containing iso-8859-1 char</param>
		/// <returns>A string </returns>
		public static  string IsoString (Byte[] src)
		{
			StringBuilder str = new StringBuilder (src.Length);
			
			for (int i = 0; i < src.Length; i++)
			{
				str.Append ((char) src[i]);;
			}

			return str.ToString ();
		}
	
		/// <summary>
		/// Converts us-ascii buffer into a string
		/// </summary>
		/// <param name="src">A buffer containing us-ascii char</param>
		/// <returns>A string</returns>
		public static  string UsString (Byte[] src)
		{
			StringBuilder str = new StringBuilder (src.Length);
			
			for (int i = 0; i < src.Length; i++)
			{
				str.Append ((char) src[i]);;
			}

			return str.ToString ();
		}
		
		/// <summary>
		/// Converts utf-8 buffer into a string
		/// </summary>
		/// <param name="src">A buffer containing utf8 char</param>
		/// <returns>A string</returns>
		public static string Utf8String (Byte[] src)
		{
			return Encoding.UTF8.GetString (src);
		}
	}
}
