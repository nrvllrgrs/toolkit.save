using UnityEditor;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
	public class FloatDefinition : SaveDefinition<float>
	{
		public FloatDefinition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.FloatField(castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out float value))
				{
					EditorGUILayout.FloatField(castValue);
				}
			};
#endif
		}
	}
}