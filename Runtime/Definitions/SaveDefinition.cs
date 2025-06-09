using System;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[Serializable]
	public abstract class SaveDefinition
	{
		#region Fields

		[SerializeField]
		protected string m_id = Guid.NewGuid().ToString();

#if UNITY_EDITOR
		[SerializeField]
		protected string m_name;
#endif
		#endregion

		#region Properties

		public string id => m_id;

		public virtual object value => null;

#if UNITY_EDITOR
		public string name
		{
			get => m_name;
			internal set => m_name = value;
		}

		public abstract string typeName { get; }
#endif
		#endregion
	}

	[Serializable]
    public abstract class SaveDefinition<T> : SaveDefinition
	{
		#region Fields

		[SerializeField]
		protected T m_defaultValue;

		#endregion

		#region Properties

		public override object value => m_defaultValue;

		public T castValue
		{
			get => m_defaultValue;
			internal set => m_defaultValue = value;
		}

#if UNITY_EDITOR
		public override string typeName => typeof(T).Name;
#endif
		#endregion
	}
}