using System;

namespace Jep.Net.Email
{
	/// <summary>
	/// Description résumée de ClientCommandEventParam.
	/// </summary>
	public class ClientCommandEventParam
	{
		string m_command;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="command">The sended command</param>
		public ClientCommandEventParam(string command)
		{
			m_command = command;
		}

		/// <summary>
		/// The sended command
		/// </summary>
		public string Command
		{
			get {return m_command;}
			set {m_command = value;}
		}


	}
}
