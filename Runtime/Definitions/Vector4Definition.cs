using UnityEditor;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public class Vector4Definition : SaveDefinition<Vector4>
	{
		public Vector4Definition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.Vector4Field(string.Empty, castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out Vector4 value))
				{
					EditorGUILayout.Vector4Field(string.Empty, castValue);
				}
			};
#endif
		}
	}
}