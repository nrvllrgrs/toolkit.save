using UnityEditor;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public class Vector3IntDefinition : SaveDefinition<Vector3Int>
	{
		public Vector3IntDefinition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.Vector3IntField(string.Empty, castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out Vector3Int value))
				{
					EditorGUILayout.Vector3IntField(string.Empty, castValue);
				}
			};
#endif
		}
	}
}