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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Jep.Net.Email
{
	/// <summary>
	/// This class help building mail message
	/// </summary>
	public class MessageBuilder
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MessageBuilder()
		{
			
		}

		/// <summary>
		/// Build a MimeMessage with text Body
		/// </summary>
		/// <param name="From">The From address</param>
		/// <param name="To">The destination address</param>
		/// <param name="Subject">The subject</param>
		/// <param name="Body">The body text</param>
		/// <param name="TextType">The body Content-Type</param>
		/// <returns>A MimeMessage</returns>
		/// <remarks>All text is encoded with the default charset define in SmtPop.Config</remarks>
		/// <example>
		/// <code>
		/// MimeMessage m = MessageBuilder (&lt;test@toto.com&gt;,&lt;test2@tyty.com&gt;,"Test","This is a test", MimeTextContentType.TextPlain);
		/// 
		/// Smtp.Send (m);
		/// 
		/// </code>
		/// </example>
		public static MimeMessage Build (string From, string To, string Subject, string Body, MimeTextContentType TextType)
		{
			MimeMessage m = new MimeMessage ();
			m.AddressFrom.Add (new MailAddress (From));
			m.AddressTo.Add (new MailAddress (To));
			m.SaveAdr ();
			m.SetSubject (Subject);
			m.SetBody (Body, MimeTransferEncoding.Base64, TextType);

			return (m);
		}

		
		/// <summary>
		/// Build a MimeMessage with text body and an image as attachment
		/// </summary>
		/// <param name="From">The from address</param>
		/// <param name="To">The destination address</param>
		/// <param name="Subject">The mail subject</param>
		/// <param name="Body">The mail body text</param>
		/// <param name="TextType">The body Content-Type</param>
		/// <param name="image">The image to send as attachment</param>
		/// <param name="format">The image format</param>
		/// <param name="name">The image name</param>
		/// <returns>A MimeMessage</returns>
		/// <remarks>All text is encoded with the default charset define in SmtPop.Config</remarks>
		/// 
		public static MimeMessage Build (string From, string To, string Subject, string Body, MimeTextContentType TextType, 
			Image image, ImageFormat format, string name)
		{
			MimeMessage m = new MimeMessage ();
			m.AddressFrom.Add (new MailAddress (From));
			m.AddressTo.Add (new MailAddress (To));
			m.SaveAdr ();

			m.SetSubject (Subject, MimeTransferEncoding.Base64);
			
			// define text as first attachment
			m.AddAttachment (new MimeAttachment (Body, TextType));

			// build an attachment with the image
			MemoryStream stream = new MemoryStream (0);
			image.Save (stream,  format);
			stream.GetBuffer ();
			string mime = "image/" + format.ToString ().ToLower ();
			if (name.IndexOf ('.') == -1)
				name += "." + format.ToString ().ToLower ();
			m.AddAttachment (new MimeAttachment (name, name, mime, stream.GetBuffer (), MimeAttachment.MimeDisposition.attachment));
			stream.Close ();
			
			return (m);
		}

		
		
		/// <summary>
		/// Build a MimeMessage with text body and binary as attachment
		/// </summary>
		/// <param name="From">The source address</param>
		/// <param name="To">The destination address</param>
		/// <param name="Subject">The mail subject</param>
		/// <param name="Body">The mail body text</param>
		/// <param name="TextType">The body Content-Type</param>
		/// <param name="Data">The binary data to send as attachment</param>
		/// <param name="MimeType">The mime type for binary data (ie "application/binary")</param>
		/// <param name="Name">The name of the attachment</param>
		/// <returns>A MimeMessage</returns>
		/// <remarks>All text is encoded with the default charset define in SmtPop.Config</remarks>
		public static MimeMessage Build (string From, string To, string Subject, string Body, MimeTextContentType TextType, 
			Byte[] Data, string MimeType, string Name)
		{
			MimeMessage m = new MimeMessage ();
			m.AddressFrom.Add (new MailAddress (From));
			m.AddressTo.Add (new MailAddress (To));
			m.SaveAdr ();

			m.SetSubject (Subject, MimeTransferEncoding.Base64);
			
			// define text as first attachment
			m.AddAttachment (new MimeAttachment (Body, TextType));

			// build an attachment with data
			m.AddAttachment (new MimeAttachment (Name, Name, MimeType, Data, MimeAttachment.MimeDisposition.attachment));
			
			
			return (m);
		}
		
		
		
		
		/// <summary>
		/// Build a MimeMessage with text body and binary data as attachment
		/// </summary>
		/// <param name="From">The source address</param>
		/// <param name="To">The destination address</param>
		/// <param name="Subject">The mail subjec</param>
		/// <param name="Body">The text body</param>
		/// <param name="TextType">The body Content-Type</param>
		/// <param name="Stream">The stream that contain binary data</param>
		/// <param name="MimeType">The Mime type of binary data (ie "application/binary")</param>
		/// <param name="Name">The attachment's name</param>
		/// <returns>A MimeMessage</returns>
		/// <remarks>All text is encoded with the default charset define in SmtPop.Config</remarks>
		public static MimeMessage Build (string From, string To, string Subject, string Body, MimeTextContentType TextType, 
			BinaryReader Stream, string MimeType, string Name)
		{
			MimeMessage m = new MimeMessage ();
			m.AddressFrom.Add (new MailAddress (From));
			m.AddressTo.Add (new MailAddress (To));
			m.SaveAdr ();

			m.SetSubject (Subject, MimeTransferEncoding.Base64);
			
			// define text as first attachment
			m.AddAttachment (new MimeAttachment (Body, TextType));

			// build an attachment with data
			MemoryStream tstream = new MemoryStream (0);
			tstream.Write (Stream.ReadBytes ((int) Stream.BaseStream.Length),0, (int) Stream.BaseStream.Length);
			m.AddAttachment (new MimeAttachment (Name, Name, MimeType, tstream.GetBuffer (), MimeAttachment.MimeDisposition.attachment));
			tstream.Close ();
			return (m);
		}

		
	
		/// <summary>
		/// Build a MimeMessage with text body and binary data as attachment
		/// </summary>
		/// <param name="From">The source address</param>
		/// <param name="To">The destination address</param>
		/// <param name="Subject">The mail subjec</param>
		/// <param name="Body">The text body</param>
		/// <param name="TextType">The body Content-Type</param>
		/// <param name="data">The object to serialize in binary data</param>
		/// <param name="MimeType">The Mime type of binary data (ie "application/binary")</param>
		/// <param name="Name">The attachment's name</param>
		/// <returns>A MimeMessage</returns>
		/// <remarks>All text is encoded with the default charset define in SmtPop.Config<br/>The data Object must be binary serializable</remarks>
		public static MimeMessage Build (string From, string To, string Subject, string Body, MimeTextContentType TextType, Object data, string MimeType, string Name)
		{
			MimeMessage m = new MimeMessage ();
			m.AddressFrom.Add (new MailAddress (From));
			m.AddressTo.Add (new MailAddress (To));
			m.SaveAdr ();

			m.SetSubject (Subject, MimeTransferEncoding.Base64);
			
			// define text as first attachment
			m.AddAttachment (new MimeAttachment (Body, TextType));

			// build an attachment with data
			MemoryStream mem = new MemoryStream ();
			BinaryFormatter bin = new BinaryFormatter ();
			bin.Serialize (mem, data);
			
			m.AddAttachment (new MimeAttachment (Name, Name, MimeType, mem.GetBuffer (), MimeAttachment.MimeDisposition.attachment));
			
			mem.Close ();
			
			return (m);
		}
		
	}
}
