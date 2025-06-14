using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.SaveManagement
{
	public interface IVariableStorage
	{
		System.Type variableType { get; }
	}

	public abstract class BaseVariableStorage<T, K> : MonoBehaviour, ISaveable, IVariableStorage
		where K : SaveVariable<T>
    {
		#region Fields

		[SerializeField]
		protected Object m_object;

		[SerializeField]
		protected Component m_component;

		[SerializeField]
		protected string m_memberName;

		[SerializeField]
		protected bool m_isProperty;

		[SerializeField]
		protected K m_variable;

		[SerializeField]
		protected bool m_loadOnStart;

		[SerializeField]
		protected bool m_saveOnDestroy;

		private MemberInfo m_memberInfo = null;

		#endregion

		#region Events

		[SerializeField]
		protected UnityEvent<T> m_onLoaded;

		[SerializeField]
		protected UnityEvent<T> m_onSaved;

		#endregion

		#region Properties

		public System.Type variableType => typeof(T);

		public UnityEvent<T> onLoaded => m_onLoaded;
		public UnityEvent<T> onSaved => m_onSaved;

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

		private void OnEnable()
		{
			SaveManager.CastInstance.Register(this);
		}

		private void OnDisable()
		{
			if (SaveManager.Exists)
			{
				SaveManager.CastInstance.Unregister(this);
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
		public virtual void Load()
		{
			if (m_variable.isDefined)
			{
				Set(m_variable.value);
				m_onLoaded?.Invoke(m_variable.value);
			}
		}

		[ContextMenu("Save")]
		public void Save()
		{
			if (m_variable.isDefined && m_memberInfo != null)
			{
				m_variable.value = (T)m_memberInfo.GetMemberValue(m_component);
				m_onSaved?.Invoke(m_variable.value);
			}
		}

		public T Get() => m_variable.value;

		public void Set(T value)
		{
			if (m_variable.isDefined)
			{
				m_variable.value = value;
				if (m_memberInfo != null)
				{
					m_memberInfo.SetMemberValue(m_component, m_variable.value);
				}
			}
		}

		#endregion
	}
}