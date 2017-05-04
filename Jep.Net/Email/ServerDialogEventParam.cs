using System;

namespace Jep.Net.Email
{
	/// <summary>
	/// Description résumée de ServerDialogEventParam.
	/// </summary>
	public class ServerDialogEventParam
	{
		/// <summary>
		/// The command sended to the server
		/// </summary>
		private string m_command;

		/// <summary>
		/// The answer received from the server
		/// </summary>
		private string m_answer;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Command">The command sended to the server</param>
		/// <param name="Answer">The answer received from the server</param>
		public ServerDialogEventParam(string Command, string Answer)
		{
			m_command = Command;
			m_answer = Answer;
		}
		/// <summary>
		/// The command sended to the server
		/// </summary>
		public string Command
		{
			get {return m_command;}
			set {m_command = value;}
		}

		/// <summary>
		/// The answer received from the server
		/// </summary>
		public string Answer
		{
			get {return m_answer;}
			set {m_answer = value;}
		}


	}
}
