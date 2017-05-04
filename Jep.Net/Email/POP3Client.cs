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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Text;



namespace Jep.Net.Email
{

    /// <summary>
    /// Pop3 Client
    /// </summary>
    public class POP3Client
    {
        /// <summary>
        /// last error string
        /// </summary>
        protected string m_error = "";

        /// <summary>
        /// The server host name
        /// </summary>
        protected string m_host;

        /// <summary>
        /// The server port
        /// </summary>
        protected int m_port;

        /// <summary>
        /// The user name
        /// </summary>
        protected string m_user;


        ///
        ///
        /// <summary>
        /// connection to server
        /// </summary>
        protected TcpClient m_tcpClient;

        /// <summary>
        /// stream to server
        /// </summary>
        protected NetworkStream m_netStream;

        /// <summary>
        /// NetworkStream decoder
        /// </summary>
        protected StreamReader m_streamReader;

        /// <summary>
        /// NetworkStream encoder
        /// </summary>
        protected StreamWriter m_streamWriter;

        /// <summary>
        /// End of line constant
        /// </summary>
        protected const string m_endl = "\r\n";

        /// <summary>
        /// Timeout for write on socket
        /// </summary>
        protected int m_sendTimeout = 50000;

        /// <summary>
        /// Timeout for read on socket
        /// </summary>
        protected int m_receiveTimeout = 50000;

        /// <summary>
        /// Last error string
        /// </summary>
        public string Error
        {
            get
            {
                return m_error;
            }
        }

        /// <summary>
        /// Timeout for write on socket
        /// </summary>
        public int SendTimeout
        {
            get
            {
                return m_sendTimeout;
            }
            set
            {
                m_sendTimeout = value;
                if (m_tcpClient != null)
                    m_tcpClient.SendTimeout = m_sendTimeout;
            }
        }

        /// <summary>
        /// Timeout for read on socket
        /// </summary>
        public int ReceiveTimeout
        {
            get
            {
                return m_receiveTimeout;
            }
            set
            {
                m_receiveTimeout = value;
                if (m_tcpClient != null)
                    m_tcpClient.ReceiveTimeout = m_receiveTimeout;
            }
        }

        /// <summary>
        /// Event fire when connection is established with the server
        /// </summary>
        public event ConnectEventHandler Connected;

        /// <summary>
        /// Event fire when authentification is done
        /// </summary>
        public event AuthentifiedEventHandler Authentified;

        /// <summary>
        /// Event fire when some data has been received from the server
        /// </summary>
        public event ReceivedEventHandler Received;

        /// <summary>
        /// Event fire when a command is send to the server
        /// </summary>
        public event ClientCommandEventHandler SendedCommand;

        /// <summary>
        /// Event fire when an aswer is receive from the server
        /// </summary>
        public event ServerAnswerEventHandler ServerAnswer;

        /// <summary>
        /// Sends the 'SendedCommand' event
        /// </summary>
        /// <param name="command"></param>
        private void FireSendedCommand(string command)
        {
            if (SendedCommand != null)
                SendedCommand(this, new ClientCommandEventParam(command));
        }

        /// <summary>
        /// Sends the 'Answer received' event
        /// </summary>
        /// <param name="answer">The answer received from the server</param>
        private void FireAnswerReceive(string answer)
        {
            if (ServerAnswer != null)
                ServerAnswer(this, new ServerAnswerEventParam(answer));
        }

        /// <summary>
        /// Sends the 'connected' event
        /// </summary>
        private void FireConnected()
        {
            if (Connected != null)
                Connected(this, new ConnectEventParam(this.m_host, this.m_port));
        }

        /// <summary>
        /// Fire the 'authentified' event
        /// </summary>
        private void FireAuthentified()
        {
            if (Authentified != null)
                Authentified(this, new AuthentifiedEventParam(this.m_user));
        }



        /// <summary>
        /// Connect the client to a POP3 server
        /// </summary>
        /// 
        /// <param name="pop3host">
        /// Pop3 Mailserver
        /// </param>
        /// <param name="port">
        /// POP3 server connection port (110)
        /// </param>
        /// <param name="user">
        /// user login
        /// </param>
        /// <param name="pwd">
        /// user password
        /// </param>
        /// <returns>1 if connected, -1 if error. Error property describe the last error</returns>
        /// <remarks>
        /// The command open the connection with the server
        /// </remarks>
        /// <example>
        /// <code>
        /// Open ("pop.mydomain", 110, "toto", "mypassword");
        /// </code>
        /// </example>
        public int Open(string pop3host, int port, string user, string pwd)
        {
            try
            {
                m_error = "";

                m_host = pop3host;
                m_port = port;
                m_user = user;

                // create POP3 connection
                m_tcpClient = new TcpClient(pop3host, port);
                FireConnected();
                m_tcpClient.ReceiveTimeout = m_receiveTimeout;
                m_tcpClient.SendTimeout = m_sendTimeout;

                // initialization
                m_netStream = m_tcpClient.GetStream();
                m_streamReader = new StreamReader(m_tcpClient.GetStream(), System.Text.Encoding.ASCII);
                m_streamWriter = new StreamWriter(m_tcpClient.GetStream(), System.Text.Encoding.ASCII);
                m_streamWriter.AutoFlush = true;

                string answer = m_streamReader.ReadLine();
                FireAnswerReceive(answer);

                if (!answer.StartsWith("+OK"))
                {
                    m_error = answer;
                    return -1;
                }

                SendLogin(user, pwd);
                FireAuthentified();

            }
            catch (Exception err)
            {
                throw (err);

            }

            return (1); // connection ready
        }

        /// <summary>
        /// Send a command to the pop3 server
        /// </summary>
        /// <param name="command">
        /// command string
        /// </param>
        /// <exception cref="POP3Exception">Raise an exception if the server return an error condition</exception>
        /// <returns>Return a string containing the server answer</returns>
        /// <remarks>
        /// All POP answer need to start with +OK. Otherwise this is an error status
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        string SendCommand(string command)
        {
            string line;
            command += m_endl;

            m_streamWriter.Write(command);
            FireSendedCommand(command);

            line = m_streamReader.ReadLine();
            FireAnswerReceive(line);

            // check server answer
            if (!line.StartsWith("+OK"))
            {
                throw (new POP3Exception("Waiting +OK. Received :" + line));
            }

            return (line);
        }

        /// <summary>
        /// Send a command to the server and retreive a block of lines
        /// </summary>
        /// <param name="command">Command string</param>
        /// <exception cref="System.Exception">Raise an exception in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise an exception if the server return an error condition</exception>
        /// <returns>A string containing the server answer (without +OK... line)</returns>
        /// <remarks>
        /// All POP answer need to start with +OK. Otherwise this is an error status
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string SendCommandBlock(string command)
        {
            StringBuilder AnswerBlock = new StringBuilder();
            try
            {
                string sTemp;

                SendCommand(command);
                FireSendedCommand(command);
                do
                {
                    sTemp = m_streamReader.ReadLine();
                    FireAnswerReceive(sTemp);
                    if (sTemp != ".")
                        AnswerBlock.Append(sTemp + m_endl);

                } while (sTemp != ".");
            }
            catch (Exception err)
            {
                throw (err);
            }
            return AnswerBlock.ToString();
        }

        /// <summary>
        /// Send POP STAT command
        /// </summary>
        /// <returns>
        /// A string containing the server answer
        /// </returns>
        /// <remarks>
        /// The <b>STAT</b> command return the current details of the connected mailbox. in
        /// the form :<br/>
        /// <b>+OK nnn BBBB</b> where nnn indicate the number of messages in the mailbox and
        /// BBBB the total number of bytes in mailbox
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>

        public string GetStat()
        {
            return SendCommand("STAT");
        }



        /// <summary>
        /// Send user login and password to the server
        /// </summary>
        /// <param name="user">User name</param>
        /// <param name="password">User password</param>
        /// <returns>1 if the login succeded</returns>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public int SendLogin(string user, string password)
        {
            try
            {
                // send user login
                string answer = SendCommand("USER " + user);

                // send password
                answer = SendCommand("PASS " + password);

            }
            catch (System.Exception e)
            {
                throw (e);
            }

            return (1);
        }

        /// <summary>
        /// Sends LIST command
        /// </summary>
        /// <remarks>
        /// The <b>LIST</b> command return the current contents of the connected mailbox. in
        /// the form of a block of lines. 
        /// for each messages in mailbox the line as the form :<BR/>
        /// <b>+OK nnn BBBB </b>where nnn indicate the message number in the mailbox and
        /// BBBB the message size in Bytes
        /// </remarks>
        /// <returns>A string containing the server answer</returns>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string GetList()
        {
            return SendCommandBlock("LIST");
        }

        /// <summary>
        /// Get the message list
        /// </summary>
        /// <param name="num">
        /// Message number
        /// </param>
        /// <returns>
        /// The server answer
        /// </returns>
        /// /// <remarks>
        /// The <b>LIST n</b> command return the current contents of the connected mailbox. in
        /// the form :<BR/>
        /// <b>+OK nnn BBBB </b>where nnn indicate the message number and
        /// BBBB the message size in bytes.
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string GetList(int num)
        {
            return SendCommand("LIST " + num);
        }

        /// <summary>
        /// Retrieve the message list from the server
        /// </summary>
        /// <returns>The list of message on the server</returns>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public POPMessageId[] GetMailList()
        {
            POPMessageId[] Messages;
            string list = GetList();
            if (list.Length == 0)
                return null;
            string[] param = list.Split('\n');
            Messages = new POPMessageId[param.Length - 1];

            for (int i = 0; i < param.Length - 1; i++)
            {
                string[] info = param[i].Split(' ');
                if (info.Length > 1)
                {
                    Messages[i] = new POPMessageId(int.Parse(info[0]), int.Parse(info[1]));
                }
            }

            return (Messages);
        }

        /// <summary>
        /// Construct a new PopReader pointing to a message on the server
        /// </summary>
        /// <param name="Id">Message identifier</param>
        /// <returns>A PopReader pointing to the message "num"</returns>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public POPReader GetMailReader(POPMessageId Id)
        {
            try
            {
                SendCommand("RETR " + Id.Id);

                return (new POPReader(m_streamReader, Received, Id.Size));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Construct a new PopReader pointing to a message header on the server
        /// </summary>
        /// <param name="Id">Message identifier</param>
        /// <returns>A PopReader pointing to the message "num" header</returns>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public POPReader GetHeaderReader(POPMessageId Id)
        {
            try
            {
                SendCommand("TOP " + Id.Id + " 0");

                return (new POPReader(m_streamReader, Received, Id.Size));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Send a RETR command
        /// </summary>
        /// <param name="num">
        /// Message number
        /// </param> 
        /// <returns>
        /// A string containing the message.
        /// </returns>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string Retr(int num)
        {
            return SendCommandBlock("RETR " + num);
        }

        /// <summary>
        /// Send DELE command
        /// </summary>
        /// <param name="num">
        /// Message number to delete
        /// </param>
        /// <returns>
        /// Server answer
        /// </returns>
        /// <remarks>
        /// The command mark the message "num" as deleted.
        /// </remarks>
        /// 
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string Dele(int num)
        {
            return SendCommand("DELE " + num);
        }

        /// <summary>
        /// Send NOOP command
        /// </summary>
        /// <returns>
        /// The server answer
        /// </returns>
        /// <remarks>
        /// The command do nothing (No Operation)
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string Noop()
        {
            return SendCommand("NOOP");
        }

        /// <summary>
        /// Send RSET Command
        /// </summary>
        /// <returns>
        /// The server answer
        /// </returns>
        /// <remarks>
        /// The command reset the mailbox (all deleted message are undeleted).
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string Rset()
        {
            return SendCommand("RSET");
        }

        /// <summary>
        /// Send the QUIT command
        /// </summary>
        /// <returns>
        /// The server answer
        /// </returns>
        /// <remarks>
        /// The command instruct the server to commit changes (delete messages marked as deleted) and close the connection.
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string Quit()
        {
            if (m_netStream == null || m_streamReader == null)
            {
                throw (new POP3Exception("-ERR not connected"));
            }
            string tmp = SendCommand("QUIT");
            m_netStream.Close();
            m_streamReader.Close();
            return tmp;
        }

        /// <summary>
        /// Send a TOP command to the pop3 server
        /// </summary>
        /// <param name="num_mess">
        /// message number
        /// </param>
        /// <param name="nlines">
        ///  number of lines
        /// </param>
        /// <returns>
        /// The server answer
        /// </returns>
        /// <remarks>
        /// the server return the message header and the "nlines" first lines of the body.<BR/>
        /// if "nlines" == 0 the server return the message header.
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string GetTop(int num_mess, int nlines)
        {
            return SendCommandBlock("TOP " + num_mess + " " + nlines);
        }

        /// <summary>
        /// Send a TOP command to the pop3 server
        /// </summary>
        /// <param name="num">
        /// message number
        /// </param>
        /// <returns>
        /// The server answer
        /// </returns>
        /// <remarks>
        /// The answer depend on server implementation.
        /// </remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string GetTop(int num)
        {
            return SendCommandBlock("TOP " + num);
        }

        /// <summary>
        /// Send a UIDL command
        /// </summary>
        /// <param name="num">
        /// Message number
        /// </param>
        /// <returns>
        /// The server answer an unique identifier for the message 'num'.
        /// </returns>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string GetUidl(int num)
        {
            return SendCommand("UIDL " + num);
        }

        /// <summary>
        /// Send a UIDL command
        /// </summary>
        /// <returns>
        /// The server answer
        ///</returns>
        ///<remarks>
        ///The server return a list of unique identifier for all message in mailbox.<BR/>
        ///Each line as the form :<BR/>
        ///<B>nnn IIIIIII</B> where n is the message number and IIIII the unique message identifier.
        ///</remarks>
        /// <exception cref="System.Exception">Raise in case of communication error</exception>
        /// <exception cref="POP3Exception">Raise if the server return an error condition</exception>
        public string GetUidl()
        {
            return SendCommandBlock("UIDL");
        }


    }
}
