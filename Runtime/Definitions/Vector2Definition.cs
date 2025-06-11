using UnityEditor;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public class Vector2Definition : SaveDefinition<Vector2>
	{
		public Vector2Definition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.Vector2Field(string.Empty, castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out Vector2 value))
				{
					EditorGUILayout.Vector2Field(string.Empty, castValue);
				}
			};
#endif
		}
	}
}