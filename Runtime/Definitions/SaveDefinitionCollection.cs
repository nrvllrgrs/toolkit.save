using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	public class SaveDefinitionCollection : ScriptableObject, IList<SaveDefinition>, IList
    {
		#region Fields

		[SerializeReference]
        protected List<SaveDefinition> m_variables = new();

		#endregion

		#region Properties

		public SaveDefinition this[int index]
		{
			get => m_variables[index];
			set => m_variables[index] = value;
		}

		object IList.this[int index]
		{
			get => ((IList)m_variables)[index];
			set => ((IList)m_variables)[index] = value;
		}

		public int Count => m_variables.Count;

		public bool IsReadOnly => false;

		public bool IsFixedSize => ((IList)m_variables).IsFixedSize;

		public bool IsSynchronized => ((ICollection)m_variables).IsSynchronized;

		public object SyncRoot => ((ICollection)m_variables).SyncRoot;

		#endregion

		#region Methods

		public void Add(SaveDefinition item)
		{
			m_variables.Add(item);
		}

		public int Add(object value)
		{
			return ((IList)m_variables).Add(value);
		}

		public void Clear()
		{
			m_variables.Clear();
		}

		public bool Contains(SaveDefinition item)
		{
			return m_variables.Contains(item);
		}

		public bool Contains(object value)
		{
			return ((IList)m_variables).Contains(value);
		}

		public void CopyTo(SaveDefinition[] array, int arrayIndex)
		{
			m_variables.CopyTo(array, arrayIndex);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)m_variables).CopyTo(array, index);
		}

		public IEnumerator<SaveDefinition> GetEnumerator()
		{
			return m_variables.GetEnumerator();
		}

		public int IndexOf(SaveDefinition item)
		{
			return m_variables.IndexOf(item);
		}

		public int IndexOf(object value)
		{
			return ((IList)m_variables).IndexOf(value);
		}

		public void Insert(int index, SaveDefinition item)
		{
			m_variables.Insert(index, item);
		}

		public void Insert(int index, object value)
		{
			((IList)m_variables).Insert(index, value);
		}

		public bool Remove(SaveDefinition item)
		{
			return m_variables.Remove(item);
		}

		public void Remove(object value)
		{
			((IList)m_variables).Remove(value);
		}

		public void RemoveAt(int index)
		{
			m_variables.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_variables.GetEnumerator();
		}

		#endregion
	}
}