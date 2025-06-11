using UnityEditor;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public class StringDefinition : SaveDefinition<string>
	{
		public StringDefinition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.TextField(castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out string value))
				{
					EditorGUILayout.TextField(castValue);
				}
			};
#endif
		}
	}
}