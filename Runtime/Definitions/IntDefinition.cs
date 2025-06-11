using UnityEditor;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public class IntDefinition : SaveDefinition<int>
	{
		public IntDefinition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.IntField(castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out int value))
				{
					EditorGUILayout.IntField(castValue);
				}
			};
#endif
		}
	}
}