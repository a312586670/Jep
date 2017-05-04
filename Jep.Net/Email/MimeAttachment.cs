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
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Jep.Net.Email
{
	/// <summary>
	/// An attachment in a MimeMessage
	/// </summary>
	public class MimeAttachment
	{
		/// <summary>
		/// A constant defining end of line sequence
		/// </summary>
		protected static readonly string endl = "\r\n";

		/// <summary>
		/// Multipart flag
		/// </summary>
		protected bool m_multipart = false;
		
		/// <summary>
		/// The attachment headers
		/// </summary>
		//public Hashtable m_headers = new Hashtable ();
		protected MimeFieldList	m_headers = new MimeFieldList ();
		/// <summary>
		/// The attachment header source
		/// </summary>
		protected string m_header = ""; 

		/// <summary>
		/// A temporary builder to build message header
		/// </summary>
		protected StringBuilder	m_headerBuilder = new StringBuilder ();

		/// <summary>
		/// The attachment body source
		/// </summary>
		protected string m_body = "";
		
		/// <summary>
		/// A temporary builder to build message body
		/// </summary>
		protected StringBuilder m_bodyBuilder;		// :)

		/// <summary>
		/// The boundary key (see multipart in Mime format)
		/// </summary>
		protected string m_boundary ="";
		
		/// <summary>
		/// Current decoder state
		/// </summary>
		protected decoder_state m_state;
		
		/// <summary>
		/// Last header field decoded
		/// </summary>
		protected string m_last_field;
		
		/// <summary>
		/// The attachments list
		/// </summary>
		protected MimeAttachmentList m_attachments = new MimeAttachmentList ();
		
		/// <summary>
		/// Current attachment decoded
		/// </summary>
		protected MimeAttachment  m_last_attachment;
			
		/// <summary>
		///  The decoder state
		/// </summary>
		public enum decoder_state
		{
			/// <summary>
			/// Decoding header
			/// </summary>
			header,	
			/// <summary>
			/// Decoding body
			/// </summary>
			body,
			/// <summary>
			/// Decoding attachment
			/// </summary>
			attachment
		}

		/// <summary>
		/// Content-disposition enumeration
		/// </summary>
		public enum MimeDisposition
		{
			/// <summary>
			/// The attachment is a part of the mail message
			/// </summary>
			inline,

			/// <summary>
			/// The attachment is a separate file
			/// </summary>
			attachment
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public MimeAttachment ()
		{
			m_state = decoder_state.header;
			m_multipart = false;
		}
		
		/// <summary>
		/// Constructs an attachment from a body string
		/// </summary>
		/// <param name="body">The body text</param>
		/// <param name="type">The content-type text</param>
		/// <param name="coding">The text Content-transfer-encoding</param>
		/// <param name="charset">The text charset</param>
		/// <example>
		/// <code>
		/// MimeAttachment m = new MimeAttachment ("hello", MimeTextContentType.TextPlain, MimeCharset.UTF8);
		/// </code>
		/// </example>
		public MimeAttachment (string body, MimeTextContentType type, MimeTransferEncoding coding, System.Text.Encoding charset)
		{
			
			m_headers["Content-Type"] = MimeConstant.GetContentTypeId (type)  + "; charset=\"" + 
				charset.BodyName + "\" ";

			m_headers["Content-Transfer-Encoding"] = MimeConstant.GetTransferEncodingId (coding);
			m_headers["Content-Disposition"] = "inline;\r\n";

			m_bodyBuilder = new StringBuilder ();
			m_bodyBuilder.Append (endl);

			if (charset == System.Text.Encoding.ASCII)
			{
				m_bodyBuilder.Append (body);
			}
			else
			{
				if (coding == MimeTransferEncoding.QuotedPrintable)
					m_bodyBuilder.Append (QPEncoder.Encode (body, charset));
				else if (coding == MimeTransferEncoding.Base64)
					m_bodyBuilder.Append (MimeEncoder.StringBase64 (body, charset, 78));	
				else
					m_bodyBuilder.Append (body);
			}
			m_bodyBuilder.Append (endl);
			m_body = m_bodyBuilder.ToString ();
			m_bodyBuilder = null;
		}

		/// <summary>
		/// Constructs an attahcment from string
		/// </summary>
		/// <param name="body">A string containing the attachment body text</param>
		/// <param name="type">The Content-Type of the attachement</param>
		/// <remarks>
		/// This method initializes a body text with utf8 charset, base64 transfer encoding and inline disposition.
		/// </remarks>
		public MimeAttachment (string body, MimeTextContentType type)
		{
			System.Text.Encoding encoding;
			
			if (QPEncoder.IsAscii (body))
				encoding = System.Text.Encoding.ASCII;
			else
				encoding = Config.defaultEncoding;
			
			m_headers["Content-Type"] = MimeConstant.GetContentTypeId (type)  + "; charset=\"" + 
				encoding.BodyName + "\" ";

			if (encoding == System.Text.Encoding.ASCII)
				m_headers["Content-Transfer-Encoding"] = MimeConstant.GetTransferEncodingId (MimeTransferEncoding.Ascii7Bit);
			else
				m_headers["Content-Transfer-Encoding"] = MimeConstant.GetTransferEncodingId (MimeTransferEncoding.Base64);
			
			m_headers["Content-Disposition"] = "inline;\r\n";

			m_bodyBuilder = new StringBuilder ();
			//m_bodyBuilder.Append (endl);
			
			if (encoding == System.Text.Encoding.ASCII)
				m_bodyBuilder.Append (body);
			else
			{
				m_bodyBuilder.Append(MimeEncoder.StringBase64 (body, Config.defaultEncoding,  78) );
			}
			
			m_bodyBuilder.Append (endl);
			m_body = m_bodyBuilder.ToString ();
			m_bodyBuilder = null;
		}

		/// <summary>
		/// Constructs an attachment from a local file
		/// </summary>
		/// <param name="path">Path of local file to send</param>
		/// <param name="filename_send">The filename of the atachment</param>
		/// <param name="content_type">The Mime Content-Type </param>
		/// <param name="disposition">The Mime disposition</param>
		public MimeAttachment (string path, string filename_send, string content_type, MimeDisposition disposition)
		{
			m_headers["Content-Type"] = content_type + ";\r\n        name=\"" +filename_send+"\"" + endl;
			m_headers["Content-Transfer-Encoding"] = "Base64";
			if (disposition == MimeDisposition.attachment)
				m_headers["Content-Disposition"] = "attachment;\r\n        filename=\"" + filename_send+"\"" + endl;
			else
				m_headers["Content-Disposition"] = "inline;\r\n        filename=\"" + filename_send+"\"" + endl;

			m_bodyBuilder = new StringBuilder ();
			m_bodyBuilder.Append (endl);
		
			System.IO.FileStream file = new System.IO.FileStream (path, System.IO.FileMode.Open,
				System.IO.FileAccess.Read);
			System.IO.BinaryReader read = new System.IO.BinaryReader (file);
		
			// Attachment limited to 2Go !
			Byte [] buf = read.ReadBytes ((int) file.Length);
			read.Close ();

			m_bodyBuilder.Append (MimeEncoder.ByteBase64 (buf, 78));
			m_bodyBuilder.Append (endl);
			m_body = m_bodyBuilder.ToString ();
			m_bodyBuilder = null;
		}

		
		
		/// <summary>
		/// Constructs an attachment from a local file
		/// </summary>
		/// <param name="path">The local path of the file to send</param>
		/// <param name="filename_send">The filename send in attachment</param>
		/// <param name="content_type">Mime type of the file</param>
		public MimeAttachment (string path, string filename_send, string content_type)
		{
			m_headers["Content-Type"] = content_type + ";\r\n        name=\"" +filename_send+"\"" + endl;
			m_headers["Content-Transfer-Encoding"] = "Base64";
			m_headers["Content-Disposition"] = "attachment;\r\n        filename=\"" + filename_send+"\"" + endl;

			m_bodyBuilder = new StringBuilder ();
			m_bodyBuilder.Append (endl);
			
			System.IO.FileStream file = new System.IO.FileStream (path, System.IO.FileMode.Open,
				System.IO.FileAccess.Read);
			System.IO.BinaryReader read = new System.IO.BinaryReader (file);
			
			// Attachment limited to 2Go !
			Byte [] buf = read.ReadBytes ((int) file.Length);
			read.Close ();

			m_bodyBuilder.Append (MimeEncoder.ByteBase64 (buf, 78));
			m_bodyBuilder.Append (endl);
			m_body = m_bodyBuilder.ToString ();
			m_bodyBuilder = null;
		}

		
		
		/// <summary>
		/// Constructs an attachment from a memory buffer
		/// </summary>
		/// <param name="name">The attachment name</param>
		/// <param name="filename_send">The attachment filename if need</param>
		/// <param name="content_type">The content type</param>
		/// <param name="buf">The buffer to send</param>
		/// <param name="disposition">The file disposition</param>
		public MimeAttachment (string name, string filename_send, string content_type, Byte[] buf, MimeDisposition disposition)
		{
			m_headers["Content-Type"] = content_type + ";\r\n        name=\"" +name+"\"" + endl;
			m_headers["Content-Transfer-Encoding"] = "Base64";
			
			if (disposition == MimeDisposition.attachment)
				m_headers["Content-Disposition"] = "attachment;\r\n        filename=\"" + filename_send+"\"" + endl;
			else
				m_headers["Content-Disposition"] = "inline;\r\n        filename=\"" + filename_send+"\"" + endl;


			m_bodyBuilder = new StringBuilder ();
			m_bodyBuilder.Append (endl);
			m_bodyBuilder.Append (MimeEncoder.ByteBase64 (buf, 78));
			m_bodyBuilder.Append (endl);
			m_body = m_bodyBuilder.ToString ();
			m_bodyBuilder = null;
		}
		
		/// <summary>
		/// Attachment headers array
		/// </summary>
		public MimeFieldList Headers
		{
			get { return m_headers;}
			set { m_headers = value;}
		}

		/// <summary>
		/// Attachments list
		/// </summary>
		public MimeAttachmentList Attachments
		{
			get
			{
				return m_attachments;
			}
			set
			{
				m_attachments = value;
			}
		}

		/// <summary>
		/// Gets or sets the "Content-Transfer-Encoding" header field
		/// </summary>
		public string ContentTransferEncoding
		{
			get
			{
				if (m_headers.FieldExist ("Content-Transfer-Encoding"))
					return (string) m_headers["Content-Transfer-Encoding"];	
				else
					return ("");
			}
			set
			{
				m_headers["Content-Transfer-Encoding"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the "Content-Type" header field
		/// </summary>
		public string ContentType
		{
			get
			{
				if (m_headers.FieldExist ("Content-Type"))
					return (string) m_headers["Content-Type"];	
				else
					return ("");
			}
			set
			{
				m_headers["Content-Type"] = value;
			}
		}

		/// <summary>
		/// Initilizes attachment as text. The text is coded in utf8 Base64
		/// </summary>
		/// <param name="body">The body text</param>
		public void SetBodyText (string body)
		{
			m_headers["MIME-Version"] = "1.0";
			m_headers["Content-Type"] = "text/plain;\r\n charset=" + Config.defaultEncoding.BodyName;
			m_headers["Content-Transfer-Encoding"] = "base64";
			m_body = MimeEncoder.StringBase64 (body, Config.defaultEncoding, 78);
		}
		
		/// <summary>
		/// Initilizes attachment as html. The text is coded in utf8 Base64
		/// </summary>
		/// <param name="body">The body text</param>
		public void SetBodyHtml (string body)
		{
			// TODO: add test for SetBodyHtml method
			m_headers["MIME-Version"] = "1.0";
			m_headers["Content-Type"] = "text/html;\r\n charset=" + Config.defaultEncoding.BodyName;
			m_headers["Content-Transfer-Encoding"] = "base64";
			m_body = MimeEncoder.StringBase64 (body, Config.defaultEncoding, 78);
		}

		/// <summary>
		/// Initilizes attachment as text
		/// </summary>
		/// <param name="body">The body text of attachment</param>
		/// <param name="TransferEncoding">The encoding for transfer</param>
		/// <param name="ContentType">The Mime content type</param>
		public void SetBodyText (string body, MimeTransferEncoding TransferEncoding, MimeTextContentType ContentType)
		{
			string type = "";
			
			switch (ContentType)
			{
				case MimeTextContentType.TextPlain:
					type = "text/plain" ;
					break;
				case MimeTextContentType.TextHtml:
					type = "text/html";
					break;
				default:
					throw (new MimeException ("Unknown ContentType"));
			}
			m_headers["MIME-Version"] = "1.0";
			// TODO: clarifier le charset
			switch (TransferEncoding)
			{
				case MimeTransferEncoding.Ascii7Bit:
					m_headers["Content-Type"] = type +";\r\n charset=us-ascii"; 
					m_headers["Content-Transfer-Encoding"] = "7bit";
					m_body = body;
					break;
				case MimeTransferEncoding.QuotedPrintable:
					m_headers["Content-Type"] = type + ";\r\n charset=" + Config.defaultEncoding.BodyName; 
					m_headers["Content-Transfer-Encoding"] = "quoted-printable";
					m_body = QPEncoder.Encode (body, Config.defaultEncoding);
					break;
				case MimeTransferEncoding.Base64:
					m_headers["Content-Type"] = type + ";\r\n charset=" + Config.defaultEncoding.BodyName;
					m_headers["Content-Transfer-Encoding"] = "base64";
					m_body = MimeEncoder.StringBase64 (body, Config.defaultEncoding, 78);
					break;

			}
		}


		/// <summary>
		/// Check if an attachment is pending before stop decoding
		/// </summary>
		/// <returns>
		/// If the method add an attachment the method return 1. Otherwise it return 0. 
		///</returns>
		public int CheckAttachmentEnd ()
		{
			if (m_last_attachment != null)
			{
				m_last_attachment.CheckAttachmentEnd ();
				if (m_last_attachment.Headers.Count > 0)
				{
					m_attachments.Add (m_last_attachment);
					return (1);
				}

			}
			if (m_bodyBuilder != null)
				m_body = m_bodyBuilder.ToString ();
			if (m_headerBuilder != null)
				m_header = m_headerBuilder.ToString ();
			m_bodyBuilder = null;
			m_headerBuilder = null;
			return (0);
		}
	
		/// <summary>
		/// Add a line to the decoder
		/// </summary>
		/// <param name="line">A string containing a message line</param>
		/// <returns>The method return a positive value if the Attachment need more lines. Otherwise it return 0 to indicate the end of attachment body</returns>
		public int AddLine (string line)
		{
			if (m_state == decoder_state.header)
			{
				if (line.TrimStart (' ') != "\r\n")
				{
					// store the line	
					m_headerBuilder.Append (line);
					
					// check if there is field on the current line
					int r = line.IndexOf (":");
					if (r != -1 && line[0] != ' ' && line[0] != '\t')
					{
						// extract header field and value
						string field_name = line.Substring (0, r);
						string field_value = line.Substring (r + 1);
							
						// store header
						m_headers.Add (field_name, field_value);
						
						// store last field name
						m_last_field = field_name;
					}
					else if (m_last_field != null)
					{
						m_headers[m_last_field] += line;
					}

				}
				else
				{
					// initialize next state
					m_state = decoder_state.body;
					
					// iniitialize body decodig
					m_bodyBuilder = new StringBuilder ();
					
					// store headers
					m_header = m_headerBuilder.ToString ();
					m_headerBuilder.Length = 0;
					m_headerBuilder = null;
					
					
					// check multipart value
					if (IsMultipart ())
					{
						m_multipart = true;
						m_boundary = HeaderBoundary ();
					}

				}
					
			}
			else if (m_state == decoder_state.body)
			{
					if (line.IndexOf(m_boundary) != -1 && Multipart)
					{
						// start a new attachment
						m_state = decoder_state.attachment;
						m_last_attachment = new MimeAttachment ();
						m_last_attachment.AddLine (line);
						m_body = m_bodyBuilder.ToString ();
						m_bodyBuilder = null;
					}
					else
					{
						m_bodyBuilder.Append (line);
					}

			}
			else if (m_state == decoder_state.attachment)
			{
				if (m_last_attachment != null)
				{
					if (line.IndexOf (m_boundary) != -1)
					{
						// attachement end
						m_last_attachment.CheckAttachmentEnd ();
						if (m_last_attachment.Headers.Count > 0)
							m_attachments.Add (m_last_attachment);
						m_last_attachment = new MimeAttachment ();
						m_last_attachment.AddLine (line);
					}
					else  
					{
						m_last_attachment.AddLine (line);
						
					}
				}
				else
				{
					if (line.IndexOf (m_boundary) != -1)
					{
						m_last_attachment = new MimeAttachment ();
						m_last_attachment.AddLine (line);
					}
				}
			}
			return(1);
		}
		/// <summary>
		/// The method returns a string containing MimeAttachment details
		/// </summary>
		/// <returns>
		/// A string containing MimeAttachment details
		/// </returns>
		public override string ToString ()
		{
			StringBuilder st = new StringBuilder ();
				
			//st += "headers\n";
			foreach (MimeField s in m_headers)
			{
				st.AppendFormat ("{0}:{1}\n", s.Name, s.Value);
			}
			//st.Append ("body\n");
			st.Append (m_body);

			return (st.ToString ());
		}

		/// <summary>
		/// The attachment body
		/// </summary>
		public string Body
		{
			get 
			{
				return m_body;
			}
			set 
			{
				m_body = value;
			}
		}
		/// <summary>
		/// The attachment header
		/// </summary>
		public string Header
		{
			get
			{
				return m_header;
			}
		}

		/// <summary>
		/// Return the filename property
		/// </summary>
		public string Filename
		{
			get
			{
				return ExtractFilename ();
					
			}
				
		}

		/// <summary>
		/// Return the multipart flag
		/// </summary>
		public bool Multipart
		{
			get
			{
				return (m_multipart);
			}
		}

		/// <summary>
		/// The method get the multipart boudary. (see multipart/mixed, multipart/alternative ... in Mime format)
		/// </summary>
		/// <returns>
		/// The boundary string
		/// </returns>
		protected string HeaderBoundary ()
		{
			Regex r = new Regex ("Content-type\\s*:\\s*.*\\s*boundary=\\\"(.*)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Match m = r.Match (m_header.ToString ());

			if (m.Success)
			{
				return m.Result ("$1");
			}
			else
			{
				r = new Regex ("Content-type\\s*:\\s*.*\\s*boundary=(.*)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				m = r.Match (m_header.ToString ());
				if (m.Success)
					return m.Result ("$1");
				else
					return ("");

			}
				
		}

		/// <summary>
		/// The method check if the attachment is multipart (see multipart/mixed, multipart/alternative ... in Mime format)
		/// </summary>
		/// <returns></returns>
		protected bool IsMultipart ()
		{
			Regex r = new Regex (@"multipart/", RegexOptions.IgnoreCase);			
			return r.IsMatch (m_header.ToString ());
		}

		/// <summary>
		/// Writes attachment to a stream
		/// </summary>
		/// <param name="stream">The stream to write the attachmment on</param>
		public void Write (System.IO.TextWriter stream)
		{
			string st ="";
			
			// writes headers
#if false			
			foreach (System.Collections.DictionaryEntry s in m_headers)
			{
				string v = (string) s.Value;
					
				st = s.Key +":"+ v;
				if (! v.EndsWith ("\r\n"))
					st+= endl;

				stream.Write (st);
			}
#else
			foreach (MimeField m in m_headers)
			{
				st = m.Name +":" + m.Value;
				if (! st.EndsWith ("\r\n"))
					st += endl;

				stream.Write (st);
			}
#endif
			
			// \r\n to end header
			stream.Write (endl);
			
			// write body
			System.IO.StringReader r = new System.IO.StringReader (m_body.ToString ());
			string tmp;
			while ((tmp = r.ReadLine ()) != null)
				stream.Write (tmp + "\r\n");
			r.Close ();
			stream.Write (endl);
			
			// write attachments
			if (m_attachments.Count > 0)
			{
				stream.Write (m_boundary);
				stream.Write (endl);

				foreach (MimeAttachment m in m_attachments)
				{
					m.Write (stream);
				}
				stream.Write (m_boundary);
				stream.Write (endl);
			}
		}
		/// <summary>
		/// The methods extract the filename parameters from attachment
		/// </summary>
		/// <returns>The filename value from "Content-Disposition" header</returns>
		public string ExtractFilename()
		{
			string fname = "";
			string[] reg = {
							   "Content-disposition\\s*:\\s*.*\\s*filename=\\\"(.*)\\\"",
							   "Content-Type\\s*:\\s*.*\\s*name=\\\"(.*)\\\""
						   };

			for (int i =0; i < reg.Length && fname.Length == 0; i++)
			{
				Regex r = new Regex (reg[i], RegexOptions.IgnoreCase | RegexOptions.Multiline);
				Match m = r.Match (m_header.ToString ());

				if (m.Success)
				{
					fname = m.Result ("$1");
				}
			}


			return fname;
		}

		/// <summary>
		/// Returns an array of Byte from a base64 attachement
		/// </summary>
		/// <returns></returns>
		public Byte [] GetBytes ()
		{
			if (! m_headers.FieldExist ("Content-Transfer-Encoding"))
				throw new MimeException ("Invalid attachment format");
			string ct = (string) m_headers["Content-Transfer-Encoding"];
			string [] type = {"base64", "BASE64"};
			int i;

			for (i = 0; i < type.Length; i++)
			{
				if (ct.IndexOf (type[i]) != -1)
					break;
				if (ct.ToUpper ().IndexOf (type[i]) != -1)
					break;
			}

			if (i == type.Length)
				throw new MimeException ("Invalid attachment format");

			return (Convert.FromBase64String (m_body));
		}

		/// <summary>
		/// Returns Stream from a base64 attachement
		/// </summary>
		/// <returns></returns>
		public MemoryStream GetStream ()
		{
			return (new MemoryStream (GetBytes ()));
		}
	}
		
		
}