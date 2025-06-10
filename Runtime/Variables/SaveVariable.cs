using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public abstract class SaveVariable
	{
		#region Fields

		[SerializeField]
		protected string m_id;

		#endregion
	}

	[System.Serializable]
	public abstract class SaveVariable<T> : SaveVariable
    {
		#region Properties

		public T value
		{
			get
			{
				if (!SaveManager.CastInstance.TryGetValue<T>(m_id, out var v))
				{
					throw new KeyNotFoundException();
				}
				return v;
			}
			set
			{
				SaveManager.CastInstance.TrySetValue<T>(m_id, value);
			}
		}

		public bool isDefined => !string.IsNullOrWhiteSpace(m_id) && SaveManager.CastInstance.ContainsId(m_id);

		#endregion
	}
}