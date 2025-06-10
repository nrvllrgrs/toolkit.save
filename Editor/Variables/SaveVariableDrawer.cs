using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ToolkitEngine.SaveManagement;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveVariable<>), true)]
    public class SaveVariableDrawer : PropertyDrawer
    {
		#region Fields

		private bool m_initialized = false;

		private Dictionary<string, string> m_idToPathMap = new();
		private Dictionary<string, string> m_pathToIdMap = new();
		private List<string> m_paths = new();

		#endregion

		#region Constructors

		~SaveVariableDrawer()
		{
			SaveVariableWindow.CollectionChanged -= Refresh;
			SaveVariableWindow.EntryChanged -= Refresh;
		}

		#endregion

		#region Methods

		private void Refresh()
		{
			m_idToPathMap.Clear();
			m_pathToIdMap.Clear();
			m_paths.Clear();

			var collections = AssetUtil.GetAssetsOfType<SaveDefinitionCollection>();
			foreach (var collection in collections)
			{
				if (collection == null)
					continue;

				foreach (var definition in collection)
				{
					if (!IsValid(definition))
						continue;

					string path = $"{collection.name}/{definition.name}";
					if (m_pathToIdMap.ContainsKey(path))
						continue;

					m_idToPathMap.Add(definition.id, path);
					m_pathToIdMap.Add(path, definition.id);
					m_paths.Add(path);
				}

				m_paths.Sort();
			}

			m_paths.Insert(0, "[Empty]");

			SaveVariableWindow.CollectionChanged -= Refresh;
			SaveVariableWindow.EntryChanged -= Refresh;

			SaveVariableWindow.CollectionChanged += Refresh;
			SaveVariableWindow.EntryChanged += Refresh;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!m_initialized)
			{
				Refresh();
				m_initialized = true;
			}

			var id = property.FindPropertyRelative("m_id");

			int selectedIndex = 0;
			EditorGUI.BeginChangeCheck();
			{
				if (!string.IsNullOrWhiteSpace(id.stringValue)
					&& m_idToPathMap.TryGetValue(id.stringValue, out var path))
				{
					selectedIndex = Mathf.Max(m_paths.IndexOf(path, 0));
				}

				var tooltipRect = position;
				tooltipRect.x += EditorGUIUtility.labelWidth;
				tooltipRect.width -= EditorGUIUtility.labelWidth;

				selectedIndex = EditorGUIRectLayout.Popup(ref position, label.text, selectedIndex, m_paths.ToArray());

				if (selectedIndex > 0)
				{
					string tooltip = m_paths[selectedIndex];
					tooltip = tooltip.Substring(tooltip.LastIndexOf('/') + 1);

					EditorGUI.LabelField(tooltipRect, new GUIContent(string.Empty, tooltip));
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				id.stringValue = selectedIndex > 0
					? m_pathToIdMap[m_paths[selectedIndex]]
					: string.Empty;
			}

			if (!string.IsNullOrWhiteSpace(id.stringValue)
				&& SaveManager.CastInstance.TryGetValue(id.stringValue, out object value))
			{
				EditorGUIRectLayout.LabelField(ref position, null, value?.ToString() ?? string.Empty);
			}
		}

		protected virtual bool IsValid(SaveDefinition definition) => true;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIUtility.singleLineHeight;

			var id = property.FindPropertyRelative("m_id");
			if (!string.IsNullOrWhiteSpace(id.stringValue))
			{
				height += EditorGUIUtility.singleLineHeight
					+ EditorGUIUtility.standardVerticalSpacing;
			}

			return height;
		}

		#endregion
	}
}