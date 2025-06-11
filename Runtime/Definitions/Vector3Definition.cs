using UnityEditor;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public class Vector3Definition : SaveDefinition<Vector3>
	{
		public Vector3Definition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.Vector3Field(string.Empty, castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out Vector3 value))
				{
					EditorGUILayout.Vector3Field(string.Empty, castValue);
				}
			};
#endif
		}
	}
}