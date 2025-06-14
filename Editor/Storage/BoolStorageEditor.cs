using ToolkitEngine.SaveManagement;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor.SaveManagement
{
    [CustomEditor(typeof(BoolStorage))]
    public class BoolStorageEditor : BaseVariableStorageEditor
    {
		#region Fields

		protected SerializedProperty m_onInvertLoaded;

		#endregion

		#region Methods

		protected override void OnEnable()
		{
			base.OnEnable();
			m_onInvertLoaded = serializedObject.FindProperty(nameof(m_onInvertLoaded));
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onLoaded, "Events"))
			{
				EditorGUILayout.PropertyField(m_onLoaded);
				EditorGUILayout.PropertyField(m_onInvertLoaded);
				EditorGUILayout.PropertyField(m_onSaved);
			}
		}

		#endregion
	}
}