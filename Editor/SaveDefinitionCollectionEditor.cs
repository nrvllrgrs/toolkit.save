using UnityEngine;
using UnityEditor;
using ToolkitEngine.SaveManagement;

namespace ToolkitEditor.SaveManagement
{
    [CustomEditor(typeof(SaveDefinitionCollection))]
    public class SaveDefinitionCollectionEditor : Editor
    {
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Save Variables"))
			{
				var collection = target as SaveDefinitionCollection;
				EditorWindow.GetWindow<SaveVariableWindow>().SetSelectedCollection(collection);
			}
		}
	}
}