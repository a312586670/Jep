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
using System.Collections;
using Jep.Net.Email.MimeWord;

namespace Jep.Net.Email
{
    /// <summary>
    /// An RFC822 MimeMessage
    /// </summary>
    /// <example>
    /// This short sample send a mail in 7bit text
    /// <code>
    /// public void SendSimpleMail()
    /// {
    ///		string body = "This is a simple message for test";
    ///		string subject = "A simple message for test";
    ///
    ///			
    ///		MimeMessage msg = new MimeMessage ();
    ///			
    ///		// build the address source and destination
    ///		MailAddressList from = new MailAddressList ();
    ///		from.Add (new MailAddress ("toto" ,"toto@toto.com"));
    ///		msg.AddressFrom = from;
    ///			
    ///		MailAddressList to = new MailAddressList ();
    ///		to.Add (new MailAddress (TestConstant.toadr));
    ///		msg.AddressTo = to;
    ///			
    ///		msg.SaveAdr ();
    ///			
    ///		// build message
    ///		msg.SetSubject (subject, MimeTransferEncoding.Ascii7Bit);
    ///		msg.SetBody (body, MimeTransferEncoding.Ascii7Bit, MimeTextContentType.TextPlain);
    ///
    ///		// send message
    ///		SMTPClient smtp = new SMTPClient (TestConstant.host, TestConstant.portsmtp);
    ///		smtp.Open ();
    ///		smtp.SendMail (msg);
    ///		smtp.Close ();
    ///			
    ///}
    ///</code>
    /// </example>
    public class MimeMessage
    {
        /// <summary>
        /// The decoder states
        /// </summary>
        public enum decoder_state
        {
            /// <summary>
            /// decoding header
            /// </summary>
            header,
            /// <summary>
            /// decoding body
            /// </summary>
            body,
            /// <summary>
            /// decoding attachment
            /// </summary>
            attachment
        }

        /// <summary>
        /// The message attachments
        /// </summary>
        protected MimeAttachmentList m_attachments = new MimeAttachmentList();

        /// <summary>
        /// Current attachment
        /// </summary>
        protected MimeAttachment m_last_attachment;

        /// <summary>
        /// The number of lines decoded
        /// </summary>
        protected int m_lines_treated = 0;

        /// <summary>
        /// last header field decoded
        /// </summary>
        protected string m_last_field;				// 

        /// <summary>
        /// Multipart flag
        /// </summary>
        protected bool m_multipart = false;

        /// <summary>
        /// The "boundary" string (see multipart/mixed)
        /// </summary>
        protected string m_boundary = "";

        /// <summary>
        /// Current decoder state
        /// </summary>
        protected decoder_state m_state = decoder_state.header;

        /// <summary>
        /// The header source
        /// </summary>
        public string m_header = "";

        /// <summary>
        /// A temporary string builder to build header string
        /// </summary>
        public StringBuilder m_headerBuilder;		// Message header

        /// <summary>
        /// body source
        /// </summary>
        protected string m_body = "";

        /// <summary>
        /// A temporary string builder to build body string
        /// </summary>
        protected StringBuilder m_bodyBuilder;

        /// <summary>
        /// The message headers
        /// </summary>
        //protected Hashtable	m_headers = new Hashtable ();
        protected MimeFieldList m_headers = new MimeFieldList();


        ///
        ///
        /// <summary>
        /// The from address list
        /// </summary>
        protected MailAddressList m_address_from = new MailAddressList();

        /// <summary>
        /// The sender address
        /// </summary>
        protected MailAddress m_address_sender = new MailAddress();

        /// <summary>
        /// The to address list
        /// </summary>
        protected MailAddressList m_address_to = new MailAddressList();

        /// <summary>
        /// The CC address list
        /// </summary>
        protected MailAddressList m_address_cc = new MailAddressList();

        /// <summary>
        /// The BCC adress list
        /// </summary>
        protected MailAddressList m_address_bcc = new MailAddressList();

        /// <summary>
        /// Message number
        /// </summary>
        protected int number;

        /// <summary>
        /// message size
        /// </summary>
        protected int size;


        /// <summary>
        /// Contains last not critical errors
        /// </summary>
        protected string m_errors;



        /// <summary>
        /// Intializes the From: adresses field
        /// </summary>
        /// <param name="Addr">An array of address</param>
        public void SetAddressFrom(MailAddress[] Addr)
        {
            m_address_from.Clear();
            foreach (MailAddress address in Addr)
                m_address_from.Add(address);
        }

        /// <summary>
        /// Intializes the To: adresses field
        /// </summary>
        /// <param name="addr">An array of address</param>
        public void SetAddressTo(MailAddress[] addr)
        {
            m_address_to.Clear();
            foreach (MailAddress a in addr)
                m_address_to.Add(a);
        }

        /// <summary>
        /// Message header array
        /// </summary>
        public MimeFieldList Headers
        {
            get { return m_headers; }
            set { m_headers = value; }
        }


        /// <summary>
        /// The message body
        /// </summary>
        public string Body
        {
            get
            {
                return (m_body);
            }
            set
            {
                m_body = value;
            }
        }

        /// <summary>
        /// The message subject
        /// </summary>
        public string Subject
        {

            get
            {
                string st = this.HeaderSubject;
                if (MimeWordDecoder.IsMimeWord(st))
                    return (MimeWordDecoder.Decode(st));
                else
                    return (st);
            }
            set
            {
                m_headers["Subject"] = MimeWordEncoder.Encode(value,
                    Config.defaultEncoding,
                    EncodingIdentifier.Base64);
            }
        }

        /// <summary>
        /// The <b>From:</b> address list
        /// </summary>
        public MailAddressList AddressFrom
        {
            get
            {
                return (m_address_from);
            }
            set
            {
                m_address_from = value;
            }
        }

        /// <summary>
        /// The <b>To:</b> address list
        /// </summary>
        public MailAddressList AddressTo
        {
            get
            {
                return (m_address_to);
            }
            set
            {
                m_address_to = value;
            }
        }

        /// <summary>
        /// The <b>CC:</b> address list
        /// </summary>
        public MailAddressList AddressCC
        {
            get
            {
                return (m_address_cc);
            }
            set
            {
                m_address_cc = value;
            }
        }

        /// <summary>
        /// The <b>BCC:</b> adress list
        /// </summary>
        public MailAddressList AddressBCC
        {
            get
            {
                return (m_address_bcc);
            }
            set
            {
                m_address_bcc = value;
            }
        }

        /// <summary>
        /// The <b>Sender:</b> address
        /// </summary>
        public MailAddress AddressSender
        {
            get
            {
                return (m_address_sender);
            }
            set
            {
                m_address_sender = value;
            }
        }




        /// <summary>
        /// Return a string containing MimeMessage details 
        /// </summary>
        /// <returns>
        /// A string containing MimeMessage details
        /// </returns>
        public override string ToString()
        {
            StringBuilder st = new StringBuilder();
            string endl = "\r\n";
#if false
			foreach (System.Collections.DictionaryEntry s in m_headers)
			{
				string v = (string) s.Value;
					
				st.Append(s.Key +":"+ v);
				if (! v.EndsWith ("\r\n"))
					st.Append (endl);
			}
#else
            foreach (MimeField f in m_headers)
            {
                string v = (string)f.Value;

                st.Append(f.Name + ":" + v);
                if (!v.EndsWith("\r\n"))
                    st.Append(endl);
            }
#endif
            st.Append(endl + m_body + endl);

            foreach (MimeAttachment m in m_attachments)
            {
                st.Append("----- Attachment -----\r\n");
                st.Append(m.ToString());
            }
            return (st.ToString());
        }

        /// <summary>
        /// Return / set the multipart flag
        /// </summary>
        public bool Multipart
        {
            get
            {
                return (m_multipart);
            }
            set
            {
                m_multipart = value;

            }
        }

        /// <summary>
        /// Check if the the message is multipart
        /// </summary>
        /// <returns>
        /// True if the message is "multipart". Otherwise false.
        /// </returns>
        public bool IsMultipart()
        {
            Regex r = new Regex(@"multipart/", RegexOptions.IgnoreCase);
            if (r.IsMatch(m_header.ToString()))
                return (true);
            if (m_attachments.Count > 0)
                return true;
            if (m_multipart)
                return true;
            return false;
        }

        /// <summary>
        /// Updates multiparts header before sending the mail. Dot not call directly
        /// </summary>
        protected void CheckMultipart()
        {
            if (IsMultipart())
            {
                m_multipart = true;
                if (m_boundary.Length == 0)
                {
                    m_boundary = "---BoundaryPart---" + GetHashCode().ToString() + "-" + System.DateTime.Now.Ticks.ToString();
                    m_headers["Content-Type"] = "multipart/mixed;\r\n boundary=\"" + m_boundary + "\"\r\n";

                }
            }
        }

        /// <summary>
        /// The number of text lines decoded to create the message
        /// </summary>
        public int LinesTreated
        {
            get
            {
                return m_lines_treated;
            }
        }
        /// <summary>
        /// The method adds an header in the header list
        /// </summary>
        /// <param name="field">The header field name</param>
        /// <param name="fieldval">The header field value</param>
        private void AddHeader(string field, string fieldval)
        {
#if false
			if (m_headers.ContainsKey(field))
			{
				if (m_headers[field].GetType () == typeof (ArrayList))
					((ArrayList)m_headers[field]).Add (fieldval);
				else
				{
					if (field.ToLower () == "received")
					{
						ArrayList a = new ArrayList ();
						a.Add (m_headers[field]);
						a.Add (fieldval);
						m_headers[field] = a;
					}
					else
						m_headers[field] = fieldval;
				}
			}
			else
			{
				m_headers[field] = fieldval;
				m_headers_order.Add (field);
			}
#else
            m_headers.Add(field, fieldval);
#endif

        }
        /// <summary>
        /// Add a message line to the decoder
        /// </summary>
        /// <param name="line">A string containing a message line</param>
        public void AddLine(string line)
        {
            m_lines_treated++;	// one line more
            if (m_state == decoder_state.header)
            {
                if (line != "\r\n")
                {
                    if (m_headerBuilder == null)
                        m_headerBuilder = new StringBuilder();

                    m_headerBuilder.Append(line);
                    int r = line.IndexOf(":");
                    if (r != -1 && line[0] != ' ' && line[0] != '\t')
                    {
                        string field_name = line.Substring(0, r);
                        string field_value = line.Substring(r + 1);

                        AddHeader(field_name, field_value);

                        m_last_field = field_name;
                    }
                    else if (m_last_field != null)
                    {
                        m_headers[m_last_field] += line;
                    }

                }
                else
                {
                    // decode header
                    m_header = m_headerBuilder.ToString();
                    m_headerBuilder = null;
                    m_boundary = HeaderBoundary();
                    m_multipart = IsMultipart();
                    m_state = decoder_state.body;
                    m_bodyBuilder = new StringBuilder();

                }
            }
            else if (m_state == decoder_state.body)
            {
                if (line != "\r\n")
                {
                    if (line.IndexOf(m_boundary) != -1 && m_multipart)
                    {
                        // end last attachment
                        if (m_last_attachment != null)
                        {
                            m_last_attachment.CheckAttachmentEnd();
                            if (m_last_attachment.Body.Length > 0 ||
                                m_last_attachment.Attachments.Count > 0 ||
                                m_last_attachment.Header.Length > 0)
                            {
                                m_attachments.Add(m_last_attachment);
                                m_last_attachment = null;
                            }
                        }
                        // start a new attachment
                        m_state = decoder_state.attachment;
                        m_body = m_bodyBuilder.ToString();
                        m_bodyBuilder.Length = 0;
                        m_bodyBuilder = null;
                        m_last_attachment = new MimeAttachment();
                        m_last_attachment.AddLine(line);
                    }
                    else
                        m_bodyBuilder.Append(line);	// append the line to the message body
                }
            }
            else if (m_state == decoder_state.attachment)
            {

                if (m_last_attachment != null)
                {
                    if (line.IndexOf(m_boundary) != -1)
                    {
                        // attachement end
                        m_last_attachment.CheckAttachmentEnd();
                        if (m_last_attachment.Header.Length > 0 ||
                            m_last_attachment.Body.Length > 0 ||
                            m_last_attachment.Attachments.Count > 0)
                        {
                            m_attachments.Add(m_last_attachment);

                        }
                        //m_attachments.Add (m_last_attachment);
                        m_last_attachment = new MimeAttachment();
                        m_last_attachment.AddLine(line);
                    }
                    else
                    {
                        m_last_attachment.AddLine(line); // append the line to current attachment

                    }

                }
                else
                {
                    if (line.IndexOf(m_boundary) != -1)
                    {
                        m_last_attachment = new MimeAttachment();
                        m_last_attachment.AddLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Ensure decode is ended
        /// </summary>
        public void EndDecode()
        {
            if (m_headerBuilder != null)
            {
                m_header = m_headerBuilder.ToString();
                m_headerBuilder = null;
            }
            if (m_bodyBuilder != null)
            {
                m_body = m_bodyBuilder.ToString();
                m_bodyBuilder = null;

            }

        }

        /// <summary>
        /// Gets or sets the "From" header field
        /// </summary>
        public string HeaderFrom
        {
            get
            {
                if (m_headers.FieldExist("From"))
                    return (string)m_headers["From"];
                else
                    return "";
            }
            set
            {
                m_headers["From"] = value;
            }
        }
        /// <summary>
        /// Gets or sets the "To" header field.
        /// </summary>
        public string HeaderTo
        {
            get
            {
                if (m_headers.FieldExist("To"))
                    return (string)m_headers["To"];
                else
                    return "";
            }
            set
            {
                m_headers["To"] = value;
            }
        }
        /// <summary>
        /// Gets or sets the "Subject" header field
        /// </summary>
        public string HeaderSubject
        {
            get
            {
                if (m_headers.FieldExist("Subject"))
                    return (string)m_headers["Subject"];
                else
                    return "";
            }
            set
            {
                m_headers["Subject"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Message-ID" header field
        /// </summary>
        public string HeaderMessageId
        {
            get
            {
                if (m_headers.FieldExist("Message-ID"))
                    return (string)m_headers["Message-ID"];
                else
                    return ("");
            }
            set
            {
                m_headers["Message-ID"] = value;
            }
        }
        /// <summary>
        /// Extract the boundary field from message header
        /// </summary>
        /// <returns>A string containing the message boundary</returns>
        protected string HeaderBoundary()
        {
            Regex r = new Regex("Content-type\\s*:\\s*.*\\s*boundary=\\\"(.*)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match m = r.Match(m_header.ToString());

            if (m.Success)
            {
                return m.Result("$1");
            }
            else
            {
                r = new Regex("Content-type\\s*:\\s*.*\\s*boundary=(.*)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                m = r.Match(m_header.ToString());
                if (m.Success)
                    return m.Result("$1");
                else
                {
                    r = new Regex("boundary=\\\"(.*)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    m = r.Match(m_header.ToString());
                    if (m.Success)
                        return m.Result("$1");
                    else
                    {
                        r = new Regex("boundary=(.*)\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        m = r.Match(m_header.ToString());
                        if (m.Success)
                            return m.Result("$1");
                        else
                            return ("");
                    }
                }

            }

        }
        /// <summary>
        /// Gets or sets the "Content-Type" header field
        /// </summary>
        public string HeaderContentType
        {
            get
            {
                if (m_headers.FieldExist("Content-Type"))
                    return (string)m_headers["Content-Type"];
                else
                    return "";
            }
            set
            {
                m_headers["Content-Type"] = value;
            }
        }
        /// <summary>
        /// Gets or sets the "Content-Transfer-Encoding" header field
        /// </summary>
        public string ContentTransferEncoding
        {
            get
            {
                if (m_headers.FieldExist("Content-Transfer-Encoding"))
                    return (string)m_headers["Content-Transfer-Encoding"];
                else
                    return ("");
            }
            set
            {
                m_headers["Content-Transfer-Encoding"] = value;
            }
        }

        /// <summary>
        /// Build headers from To, CC, and BCC address from address list
        /// </summary>
        public void SaveAdr()
        {
            string to = "";
            string cc = "";
            string bcc = "";
            string from = "";

            foreach (MailAddress adr in m_address_to)
            {
                if (to.Length > 0)
                    to += ";";
                to += adr.Src;
            }

            foreach (MailAddress adr in m_address_cc)
            {
                if (cc.Length > 0)
                    cc += ";";
                cc += adr.Src;
            }
            foreach (MailAddress adr in m_address_bcc)
            {
                if (bcc.Length > 0)
                    bcc += ";";
                bcc += adr.Src;
            }
            foreach (MailAddress adr in m_address_from)
            {
                if (from.Length > 0)
                    from += ";";
                from += adr.Src;
            }
            if (m_address_from.Count > 1 && m_address_sender.Mailbox.Length == 0)
                m_address_sender = m_address_from[0];

            if (m_address_sender != null &&
                m_address_sender.Mailbox.Length > 0)
            {
                m_headers["Sender"] = m_address_sender.Src;
            }

            if (from.Length > 0)
                m_headers["From"] = from;

            if (to.Length > 0)
                m_headers["To"] = to;
            if (cc.Length > 0)
                m_headers["CC"] = cc;
            if (bcc.Length > 0)
                m_headers["BCC"] = bcc;
        }

        /// <summary>
        /// The method read an RFC 2822 Mime Message from the stream "stream"
        /// </summary>
        /// <param name="stream">The input stream</param>
        /// <returns>The method return an RFC 2822 MimeMessage</returns>
        /// <remarks></remarks>
        /// <example>
        /// <code escaped="true">
        /// StreamReader re = new StreamReader (openFileDialog.FileName, System.Text.Encoding.ASCII);
        /// MimeMessage m = new MimeMessage ();
        /// m.Read (re);
        /// Console.Write (m.ToString ());
        /// </code>
        /// </example>
        /// <exception cref="System.Exception">Read error from stream</exception>
        public void Read(System.IO.TextReader stream)
        {
            string line;

            m_header = "";
            m_body = "";
            m_bodyBuilder = null;
            m_attachments.Clear();
            m_headers.Clear();
            m_multipart = false;
            m_errors = "";

            m_headerBuilder = new StringBuilder();

            do
            {
                line = stream.ReadLine();
                if (line != null)
                {
                    line += "\r\n";
                    AddLine(line);
                }

            } while (line != null);
            EndDecode();
            ReadAddressesFromHeaders();
        }

        /// <summary>
        /// Writes messages to a stream
        /// </summary>
        /// <param name="stream">The stream to write the message on</param>
        public void Write(System.IO.TextWriter stream)
        {
            string st = "";
            string endl = "\r\n";

            // check multipart before writing
            CheckMultipart();

            // write headers
#if false
			foreach (System.Collections.DictionaryEntry s in m_headers)
			{
				if (s.Value.GetType () == typeof (ArrayList))
				{
					// write multiple header
					foreach (string val in (ArrayList) s.Value)
					{
						st = s.Key +":"+ val.ToString ();
						if (! st.EndsWith ("\r\n"))
							st += endl;

						stream.Write (st);
					}
				}
				else
				{
					// write single header
					string v = (string) s.Value;
					st = s.Key +":"+ v.ToString ();
					if (! st.EndsWith ("\r\n"))
						st += endl;

					stream.Write (st);
				}
			}
#else
            foreach (MimeField m in m_headers)
            {
                st = m.Name + ":" + m.Value;
                if (!st.EndsWith("\r\n"))
                    st += endl;

                stream.Write(st);
            }
#endif
            stream.Write(endl);

            // write mail body
            System.IO.StringReader r = new System.IO.StringReader(m_body);
            string tmp;
            // write line by line to ensure correct SMTP syntax
            while ((tmp = r.ReadLine()) != null)
                stream.Write(tmp + "\r\n");
            r.Close();
            stream.Write(endl);

            // write attachments
            if (m_attachments != null)
            {
                if (m_attachments.Count > 0)
                {
                    stream.Write("--" + m_boundary);
                    stream.Write(endl);

                    foreach (MimeAttachment m in m_attachments)
                    {
                        m.Write(stream);
                        stream.Write("--" + m_boundary);
                        stream.Write(endl);
                    }

                }
            }


        }

        /// <summary>
        /// Initializes the message subject. Subject is MimeWord encoded utf8 base64 
        /// </summary>
        /// <param name="subject">A string containing the subject text</param>
        public void SetSubject(string subject)
        {
            m_headers["Subject"] = MimeWordEncoder.Encode(subject,
                Config.defaultEncoding, // OK for all unicode char
                EncodingIdentifier.Base64);
        }

        /// <summary>
        /// Initializes the message subject
        /// </summary>
        /// <param name="subject">A string containing the subject text</param>
        /// <param name="TransferEncoding">The encoding method for subject (choose base64 for unicode)</param>
        public void SetSubject(string subject, MimeTransferEncoding TransferEncoding)
        {
            switch (TransferEncoding)
            {
                case MimeTransferEncoding.Ascii7Bit:
                    m_headers["Subject"] = subject + "\r\n";
                    break;
                case MimeTransferEncoding.QuotedPrintable:
                    m_headers["Subject"] = MimeWordEncoder.Encode(subject,
                            Config.defaultEncoding, // use System charset as default
                            EncodingIdentifier.QuotedPrintable);
                    break;
                case MimeTransferEncoding.Base64:
                    m_headers["Subject"] = MimeWordEncoder.Encode(subject,
                            Config.defaultEncoding,
                            EncodingIdentifier.Base64);
                    break;

            }
        }


        /// <summary>
        /// Initializes the body
        /// </summary>
        /// <param name="body">The body text</param>
        /// 
        /// 
        public void SetBodyText(string body)
        {
            m_headers["MIME-Version"] = "1.0";
            m_headers["Content-Type"] = "text/plain;\r\n charset=" + Config.defaultEncoding.BodyName;
            m_headers["Content-Transfer-Encoding"] = "base64";
            m_body = MimeEncoder.StringBase64(body, Config.defaultEncoding, 78);
        }

        /// <summary>
        /// Initializes the body
        /// </summary>
        /// <param name="body">The body text</param>
        /// 
        /// 
        public void SetBodyHtml(string body)
        {
            m_headers["MIME-Version"] = "1.0";
            m_headers["Content-Type"] = "text/html;\r\n charset=" + Config.defaultEncoding.BodyName;
            m_headers["Content-Transfer-Encoding"] = "base64";
            m_body = MimeEncoder.StringBase64(body, Config.defaultEncoding, 78);
        }
        /// <summary>
        /// Initializes the body
        /// </summary>
        /// <param name="body">The body text</param>
        /// <param name="TransferEncoding">The Transfer encoding format</param>
        /// <param name="ContentType">The Mime content type</param>
        /// 
        public void SetBody(string body, MimeTransferEncoding TransferEncoding, MimeTextContentType ContentType)
        {
            string type = "";

            switch (ContentType)
            {
                case MimeTextContentType.TextPlain:
                    type = "text/plain";
                    break;
                case MimeTextContentType.TextHtml:
                    type = "text/html";
                    break;
                default:
                    throw (new MimeException("Unknown ContentType"));

            }
            m_headers["MIME-Version"] = "1.0";

            switch (TransferEncoding)
            {
                case MimeTransferEncoding.Ascii7Bit:
                    m_headers["Content-Type"] = type + ";\r\n charset=us-ascii";
                    m_headers["Content-Transfer-Encoding"] = "7bit";
                    m_body = body;
                    break;
                case MimeTransferEncoding.QuotedPrintable:
                    m_headers["Content-Type"] = type + ";\r\n charset=" + Config.defaultEncoding.BodyName;
                    m_headers["Content-Transfer-Encoding"] = "quoted-printable";
                    m_body = QPEncoder.Encode(body, Config.defaultEncoding);
                    break;
                case MimeTransferEncoding.Base64:
                    m_headers["Content-Type"] = type + ";\r\n charset=" + Config.defaultEncoding.BodyName;
                    m_headers["Content-Transfer-Encoding"] = "base64";
                    m_body = MimeEncoder.StringBase64(body, Config.defaultEncoding, 78);
                    break;

            }
        }



        /// <summary>
        /// Attachment list
        /// </summary>
        public MimeAttachmentList Attachments
        {
            get
            {
                return (m_attachments);
            }
            set
            {
                m_boundary = "---BoundaryPart---" + GetHashCode().ToString() + "-" + System.DateTime.Now.Ticks.ToString();

                m_headers["Content-Type"] = "multipart/mixed;\r\n boundary=\"" + m_boundary + "\"\r\n";
                m_body = "This is a multipart/mixed message\r\n\r\n";
                m_attachments.Clear();
                foreach (MimeAttachment m in value)
                    m_attachments.Add(m);
            }
        }
        /// <summary>
        /// Attachment list
        /// </summary>
        public MimeAttachment[] AttachmentArray
        {
            set
            {
                m_boundary = "---BoundaryPart---" + GetHashCode().ToString() + "-" + System.DateTime.Now.Ticks.ToString();

                m_headers["Content-Type"] = "multipart/mixed;\r\n boundary=\"" + m_boundary + "\"\r\n";
                m_body = "This is a multipart/mixed message\r\n\r\n";
                m_attachments.Clear();
                foreach (MimeAttachment m in value)
                    m_attachments.Add(m);
            }
        }

        /// <summary>
        /// Adds an attachment
        /// </summary>
        /// <param name="attachment">Mime attachment</param>
        public void AddAttachment(MimeAttachment attachment)
        {
            if (m_boundary.Length == 0)
            {
                m_boundary = "---BoundaryPart---" + GetHashCode().ToString() + "-" + System.DateTime.Now.Ticks.ToString();
                m_headers["Content-Type"] = "multipart/mixed;\r\n boundary=\"" + m_boundary + "\"\r\n";

            }
            m_attachments.Add(attachment);
            m_multipart = true;
        }
        /// <summary>
        /// Initializes the attachement list
        /// </summary>
        /// <param name="attachments">An attachment array</param>
        public void SetAttachments(MimeAttachment[] attachments)
        {
            m_boundary = "---BoundaryPart---" + GetHashCode().ToString() + "-" + System.DateTime.Now.Ticks.ToString();

            m_headers["Content-Type"] = "multipart/mixed;\r\n boundary=\"" + m_boundary + "\"\r\n";
            m_body = "This is a multipart/mixed message\r\n\r\n";
            m_attachments.Clear();
            foreach (MimeAttachment m in attachments)
                m_attachments.Add(m);
            if (m_attachments.Count > 0)
                m_multipart = true;
        }

        /// <summary>
        /// Initializes from,to,cc and bcc address list from headers.
        /// </summary>
        protected void ReadAddressesFromHeaders()
        {
            m_address_from.Clear();
            m_address_cc.Clear();
            m_address_bcc.Clear();
            m_address_to.Clear();

            if (m_headers.FieldExist("From"))
                ReadAddresses(ref m_address_from, (string)m_headers["From"]);
            if (m_headers.FieldExist("Sender"))
                m_address_sender = new MailAddress((string)m_headers["Sender"]);
            if (m_headers.FieldExist("To"))
            {
                try
                {
                    ReadAddresses(ref m_address_to, (string)m_headers["To"]);
                }
                catch (MailAddressException e)
                {
                    m_errors += e.Message + "\r\n";
                }
            }
            if (m_headers.FieldExist("CC"))
            {
                try
                {
                    ReadAddresses(ref m_address_cc, (string)m_headers["CC"]);
                }
                catch (MailAddressException e)
                {
                    m_errors += e.Message + "\r\n";
                }
            }
            if (m_headers.FieldExist("BCC"))
            {
                try
                {
                    ReadAddresses(ref m_address_bcc, (string)m_headers["BCC"]);
                }
                catch (MailAddressException e)
                {
                    m_errors += e.Message + "\r\n";
                }
            }

        }

        /// <summary>
        /// Read adresses from header line
        /// </summary>
        /// <param name="list">The address list to initialize</param>
        /// <param name="src">A string containing header line with address list</param>
        protected void ReadAddresses(ref MailAddressList list, string src)
        {
            string[] s = src.Split(';');
            if (s == null)
            {
                list.Add(new MailAddress(src));
            }
            else
            {
                foreach (string a in s)
                {
                    int p;
                    string tmp = a;
                    while ((p = tmp.LastIndexOf("\r\n")) != -1)
                        tmp = tmp.Remove(p, 2);
                    list.Add(new MailAddress(tmp));
                }
            }
        }

    }

}