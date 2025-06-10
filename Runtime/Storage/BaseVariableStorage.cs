using System.Reflection;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
    public abstract class BaseVariableStorage<T, K> : MonoBehaviour
		where K : SaveVariable<T>
    {
		#region Fields

		[SerializeField]
		private Object m_object;

		[SerializeField]
		private Component m_component;

		[SerializeField]
		private string m_memberName;

		[SerializeField]
		private bool m_isProperty;

		[SerializeField]
		private K m_variable;

		[SerializeField]
		private bool m_loadOnStart;

		[SerializeField]
		private bool m_saveOnDestroy;

		private MemberInfo m_memberInfo = null;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_component != null && !string.IsNullOrWhiteSpace(m_memberName))
			{
				m_memberInfo = !m_isProperty
					? m_component.GetType().GetField(m_memberName, BindingFlags.Public | BindingFlags.Instance)
					: m_component.GetType().GetProperty(m_memberName, BindingFlags.Public | BindingFlags.Instance);
			}
		}

		private void Start()
		{
			if (m_loadOnStart)
			{
				Load();
			}
		}

		private void OnDestroy()
		{
			if (m_saveOnDestroy)
			{
				Save();
			}
		}

		[ContextMenu("Load")]
		public void Load()
		{
			if (m_variable.isDefined)
			{
				m_memberInfo.SetMemberValue(m_component, m_variable.value);
			}
		}

		[ContextMenu("Save")]
		public void Save()
		{
			if (m_variable.isDefined)
			{
				m_variable.value = (T)m_memberInfo.GetMemberValue(m_component);
			}
		}

		#endregion
	}
}