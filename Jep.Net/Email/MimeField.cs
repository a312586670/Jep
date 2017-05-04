using System;


namespace Jep.Net.Email
{
	/// <summary>
	/// A mime header field
	/// </summary>
	public class MimeField
	{
		/// <summary>
		/// The field name (ie "subject" or "received")
		/// </summary>
		private string				m_name;
		
		/// <summary>
		/// The value of the field. 
		/// </summary>
		private string				m_value;

		/// <summary>
		/// Constructor
		/// </summary>
		public MimeField()
		{
			
	
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Name">The field name</param>
		/// <param name="Val">The field value</param>
		public MimeField(string Name, string Val)
		{
			m_name = Name;
			m_value = Val;
		}


		/// <summary>
		/// The value of field.
		/// </summary>
		public string Value
		{
			get
			{
				return (m_value);
			}
			set
			{
				m_value = value;
			}
		}

		/// <summary>
		/// The field name (ie "subject", "received"
		/// </summary>
		public string Name
		{
			get
			{
				return (m_name);
			}
			set
			{
				m_name = value;
			}
		}
	}
}
