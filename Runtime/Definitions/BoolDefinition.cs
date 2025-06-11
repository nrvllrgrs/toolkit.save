using UnityEditor;

namespace ToolkitEngine.SaveManagement
{
	[System.Serializable]
    public class BoolDefinition : SaveDefinition<bool>
    {
		public BoolDefinition()
			: base()
		{
#if UNITY_EDITOR
			defaultGUIHandler = () =>
			{
				castValue = EditorGUILayout.Toggle(castValue);
			};

			currentGUIHandler = () =>
			{
				if (SaveManager.CastInstance.TryGetValue(m_id, out bool value))
				{
					EditorGUILayout.Toggle(castValue);
				}
			};
#endif
		}
	}
}