using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[CreateAssetMenu(menuName = "Toolkit/Config/SaveManager Config")]
	public class SaveManagerConfig : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private List<SaveDefinitionCollection> m_collections = new();

		#endregion

		#region Properties

		public IEnumerable<SaveDefinitionCollection> collections => m_collections;

		#endregion

		#region Editor Only
#if UNITY_EDITOR

		internal void Add(SaveDefinitionCollection collection)
		{
			if (collection == null)
				return;

			m_collections.Add(collection);
		}

#endif
		#endregion
	}
}