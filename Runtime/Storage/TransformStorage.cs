using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[AddComponentMenu("Save/Transform Storage")]
	public class TransformStorage : MonoBehaviour, ISaveable
    {
		#region Fields

		[SerializeField]
		private Transform m_transform;

		[SerializeField]
		private SaveVector3 m_position;

		[SerializeField]
		private SaveVector3 m_rotation;

		[SerializeField]
		private SaveVector3 m_scale;

		[SerializeField]
		private bool m_loadOnStart;

		[SerializeField]
		private bool m_saveOnDestroy;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_transform == null)
			{
				m_transform = transform;
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
		public void Load()
		{
			if (m_position.isDefined && m_rotation.isDefined)
			{
				m_transform.SetLocalPositionAndRotation(m_position.value, Quaternion.Euler(m_rotation.value));
			}
			else if (m_position.isDefined)
			{
				m_transform.localPosition = m_position.value;
			}
			else if (m_rotation.isDefined)
			{
				m_transform.localRotation = Quaternion.Euler(m_rotation.value);
			}

			if (m_scale.isDefined)
			{
				m_transform.localScale = m_scale.value;
			}
		}

		[ContextMenu("Save")]
		public void Save()
		{
			if (m_position.isDefined)
			{
				m_position.value = m_transform.localPosition;
			}

			if (m_rotation.isDefined)
			{
				m_rotation.value = m_transform.localEulerAngles;
			}

			if (m_scale.isDefined)
			{
				m_scale.value = m_transform.localScale;
			}
		}

		#endregion
	}
}