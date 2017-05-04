using System;

namespace Jep.Net.Email
{
	/// <summary>
	/// Description résumée de ServerAnswerEventParam.
	/// </summary>
	public class ServerAnswerEventParam
	{
		private string m_answer;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Answer">The answer received from the server</param>
		public ServerAnswerEventParam(string Answer)
		{
			m_answer = Answer;
		}
		
		/// <summary>
		/// The aswer received from the server
		/// </summary>
		public string Answer 
		{
			get {return m_answer;}
			set {m_answer = value;}
		}
	}
}
