using System;
using System.Collections;

namespace Jep.Net.Email
{
	/// <summary>
	/// Handle a list of MimeField
	/// </summary>
	public class MimeFieldList : IList 
	{
		/// <summary>
		/// The MimeField Collection
		/// </summary>
		private ArrayList m_data = new ArrayList ();

		/// <summary>
		/// Constructor
		/// </summary>
		public MimeFieldList()
		{
			
		}

		/// <summary>
		/// Add a field to the Mime header
		/// </summary>
		/// <param name="field">The field name and value</param>
		public void Add (MimeField field)
		{
			m_data.Add (field);
		}

		/// <summary>
		/// Add a field to the Mime header
		/// </summary>
		/// <param name="Name">The field name (ie 'subject:')</param>
		/// <param name="Value">The field value</param>
		public void Add (string Name, string Value)
		{
			m_data.Add (new MimeField (Name, Value));
		}

		/// <summary>
		/// Update a field value
		/// </summary>
		/// <param name="Name">The field name</param>
		/// <param name="Value">The new value of the field</param>
		public void Update (string Name, string Value)
		{
			int id = FindField (Name);
			if (id == -1)
				Add (Name, Value);
			else
				((MimeField) m_data[id]).Value = Value;
		}

		/// <summary>
		/// Searches for a field in the header
		/// </summary>
		/// <param name="Name">The searched field name</param>
		/// <returns>The index of the field</returns>
		public int FindField (string Name)
		{
			return (FindField (Name, 0));
		}
		
		/// <summary>
		/// Find the n'th occurency of the field named 'Name'
		/// </summary>
		/// <param name="Name">The field name</param>
		/// <param name="n">The occurency of the field</param>
		/// <returns>The index of the field</returns>
		/// <example>
		/// Search for 'received' fields
		/// <code>
		/// int n = FieldCount (\"received\");
		/// for (int i = 0; i &lt; n; i++)
		///		FindField ("received", n);
		/// </code>
		/// </example>
		public int FindField (string Name, int n)
		{
			int c = 0;
			for (int i = 0; i < m_data.Count; i++)
			{
				if (((MimeField) m_data[i]).Name == Name)
				{
					if (c == n)
						return i;
					else
						c++;
				}
			}
			return (-1);
		}
		/// <summary>
		/// Counts the number of occurence of a particular field
		/// </summary>
		/// <param name="Name">The field name</param>
		/// <returns>The number of field 'Name' in the header</returns>
		/// <remarks>This is useful for some fields like 'receive'</remarks>
		public int FieldCount (string Name)
		{
			int c = 0;
			for (int i = 0; i < m_data.Count; i++)
			{
				if (((MimeField) m_data[i]).Name == Name)
					c++;
			}
			return (c);
		}

		/// <summary>
		/// Searches if a field exists in the header
		/// </summary>
		/// <param name="Name">The field name</param>
		/// <returns>true if the field exist. Otherwise false</returns>
		public bool FieldExist (string Name)
		{
			return (FindField (Name) != -1);
		}

		/// <summary>
		/// 
		/// </summary>
		public string this[string Name]
		{
			get
			{
				int id = FindField (Name);
				if (id == -1)
					throw (new ArgumentOutOfRangeException ("Name", Name, "Unknown field name"));
				return ((MimeField) m_data[id]).Value;
			}
			set
			{
				int id = FindField (Name);
				if (id == -1)
					m_data.Add (new MimeField (Name, value));
				else
					((MimeField) m_data[id]).Value = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string this[string Name, int n]
		{
			get
			{
				int id = FindField (Name, n);
				if (id == -1)
					throw (new ArgumentOutOfRangeException ("Name", Name, "Unknown field name"));
				return ((MimeField) m_data[id]).Value;
			}
			set
			{
				int id = FindField (Name, n);
				if (id == -1)
					throw (new ArgumentOutOfRangeException ("Name", Name, "Unknown field name"));
				((MimeField) m_data[id]).Value = value;
			}
		}
		#region IList members

		/// <summary>
		/// See Ilist interface
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return m_data.IsReadOnly;
			}
		}

		/// <summary>
		/// See Ilist interface
		/// </summary>
		object System.Collections.IList.this[int index]
		{
			get
			{
				
				return (MimeField) m_data[index];
			}
			set
			{
				m_data[index] = value;
			}
		}

		/// <summary>
		/// See IList interface
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			m_data.RemoveAt (index);
		}

		/// <summary>
		/// See IList interface
		/// </summary>
		/// <param name="index">See IList interface</param>
		/// <param name="value">See IList interface</param>
		public void Insert(int index, object value)
		{
			m_data.Insert (index, value);
		}

		/// <summary>
		/// See IList interface
		/// </summary>
		/// <param name="value">See IList interface</param>
		public void Remove(object value)
		{
			m_data.Remove (value);
		}

		/// <summary>
		/// See IList interface
		/// </summary>
		/// <param name="value">See IList interface</param>
		/// <returns></returns>
		public bool Contains(object value)
		{
			return m_data.Contains (value);
		}

		/// <summary>
		/// See IList interface
		/// </summary>
		public void Clear()
		{
			m_data.Clear ();
		}

		/// <summary>
		/// See IList interface
		/// </summary>
		/// <param name="value">See IList interface</param>
		/// <returns>See IList interface</returns>
		public int IndexOf(object value)
		{
			return m_data.IndexOf (value);
		}

		/// <summary>
		/// See IList interface
		/// </summary>
		/// <param name="value">See IList interface</param>
		/// <returns>See IList interface</returns>
		int System.Collections.IList.Add(object value)
		{
			return m_data.Add (value);
		}

		/// <summary>
		/// See IList interface
		/// </summary>
		public bool IsFixedSize
		{
			get
			{
				return m_data.IsFixedSize;
			}
		}

		#endregion

		#region ICollection members

		/// <summary>
		/// See ICollection interface
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				
				return m_data.IsSynchronized;
			}
		}

		/// <summary>
		/// See ICollection interface
		/// </summary>
		public int Count
		{
			get
			{
				return m_data.Count;
			}
		}

		/// <summary>
		/// See ICollection interface
		/// </summary>
		/// <param name="array">See ICollection interface</param>
		/// <param name="index">See ICollection interface</param>
		public void CopyTo(Array array, int index)
		{
			m_data.CopyTo (array, index);
		}

		/// <summary>
		/// See ICollection interface
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return m_data.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable members

		/// <summary>
		/// See IEnumerable interface
		/// </summary>
		/// <returns>See IEnumerable interface</returns>
		public IEnumerator GetEnumerator()
		{
			return (m_data.GetEnumerator ());
		}

		#endregion
	}
}
