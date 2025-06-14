using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.SaveManagement
{
	[AddComponentMenu("Save/Bool Storage")]
	public class BoolStorage : BaseVariableStorage<bool, SaveBool>
	{
		#region Fields

		[SerializeField]
		private UnityEvent<bool> m_onInvertLoaded;

		#endregion

		#region Properties

		public UnityEvent<bool> onInvertLoaded => m_onInvertLoaded;

		#endregion

		#region Methods

		[ContextMenu("Load")]
		public override void Load()
		{
			if (m_variable.isDefined)
			{
				Set(m_variable.value);
				m_onLoaded?.Invoke(m_variable.value);
				m_onInvertLoaded?.Invoke(!m_variable.value);
			}
		}

		#endregion

	}
}