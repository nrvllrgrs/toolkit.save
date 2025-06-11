using UnityEditor;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public class Vector2IntDefinition : SaveDefinition<Vector2Int>
	{
		public Vector2IntDefinition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.Vector2IntField(string.Empty, castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out Vector2Int value))
				{
					EditorGUILayout.Vector2IntField(string.Empty, castValue);
				}
			};
#endif
		}
	}
}