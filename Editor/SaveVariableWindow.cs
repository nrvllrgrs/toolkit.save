using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToolkitEngine.SaveManagement;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolkitEditor.SaveManagement
{
	public class SaveVariableWindow : EditorWindow
	{
		#region Fields

		private static Dictionary<string, Type> s_definitionTypes = new();

		private DropdownField m_selectedCollectionDropdown = null;
		private MultiColumnListView m_multiColumnListView = null;

		private IList<SaveDefinitionCollection> m_collections;
		private SaveDefinitionCollection m_selectedCollection = null;
		private SerializedObject m_serializedSelectedCollection = null;

		private const string SELECTED_COLLECTION_PREF = "SaveVariableWindow.SelectedCollection";

		#endregion

		#region Events

		public static Action CollectionChanged;
		public static Action EntryChanged;

		#endregion

		#region Properties

		private static SaveVariableWindow instance => GetWindow<SaveVariableWindow>();

		#endregion

		#region Methods

		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			foreach (var type in from a in AppDomain.CurrentDomain.GetAssemblies()
								 from t in a.GetTypes()
								 where t.IsClass && !t.IsAbstract && typeof(SaveDefinition).IsAssignableFrom(t)
								 orderby t.Name
								 select t)
			{
				s_definitionTypes.Add(type.Name, type);
			}
		}

		[MenuItem("Window/Toolkit/Save Variables")]
		public static void ShowWindow()
		{
			GetWindow<SaveVariableWindow>();
			instance.titleContent = new GUIContent("Save Variables");
		}

		private void OnEnable()
		{
			var asset = AssetUtil.LoadAssetByGUID<VisualTreeAsset>("b27d3bcbe949c5c46b9a6e4be8160efb");
			var root = asset.Instantiate();

			m_multiColumnListView = root.Q<MultiColumnListView>("list");
			BindMultiColumnListView();

			instance.rootVisualElement.Add(root);

			// Bind toolbar buttons
			root.Q<Button>("newCollection").clicked += NewCollection_Clicked;
			root.Q<Button>("newEntry").clicked += NewEntry_Clicked;
			m_selectedCollectionDropdown = root.Q<DropdownField>("selectedCollection");
			m_selectedCollectionDropdown.RegisterValueChangedCallback(SelectedCollection_ValueChanged);

			root.StretchToParentSize();
			PopulateDefinitionCollections();

			var selectedCollectionGuid = EditorPrefs.GetString(SELECTED_COLLECTION_PREF);
			if (!string.IsNullOrWhiteSpace(selectedCollectionGuid))
			{
				var collection = AssetUtil.LoadAssetByGUID<SaveDefinitionCollection>(selectedCollectionGuid);
				var index = m_collections.IndexOf(collection);
				if (index >= 0)
				{
					m_selectedCollectionDropdown.index = index;
				}
			}
		}

		private void PopulateDefinitionCollections()
		{
			m_collections = AssetUtil.GetAssetsOfType<SaveDefinitionCollection>().ToList();
			m_selectedCollectionDropdown.choices = new List<string>(m_collections.Select(x => x.name));
		}

		private void NewCollection_Clicked()
		{
			string path = EditorUtility.SaveFilePanelInProject(
				"New Save Variable Collection",
				"SaveVariableCollection",
				"asset",
				"Please enter a file name.");

			if (path.Length == 0)
				return;

			var collection = CreateInstance<SaveDefinitionCollection>();
			collection.name = Path.GetFileNameWithoutExtension(path);

			// Add collection to database and config
			AssetDatabase.CreateAsset(collection, path);
			SaveManager.CastInstance.Config.Add(collection);

			PopulateDefinitionCollections();
			m_selectedCollectionDropdown.value = collection.name;

			m_selectedCollection = collection;
			CollectionChanged?.Invoke();
		}

		private void NewEntry_Clicked()
		{
			var menu = new GenericMenu();
			foreach (var p in s_definitionTypes)
			{
				menu.AddItem(new GUIContent(p.Key), false, NewEntryItemClicked, p.Value);
			}

			menu.ShowAsContext();
		}

		private void NewEntryItemClicked(object userData)
		{
			if (m_selectedCollection == null)
				return;

			var defintion = Activator.CreateInstance(userData as Type) as SaveDefinition;
			defintion.name = $"New {(userData as Type).Name}";

			m_selectedCollection.Add(defintion);
			EditorUtility.SetDirty(m_selectedCollection);

			m_multiColumnListView.RefreshItems();
			EntryChanged?.Invoke();
		}

		private void SelectedCollection_ValueChanged(ChangeEvent<string> args)
		{
			m_selectedCollection = m_collections[m_selectedCollectionDropdown.index];
			m_multiColumnListView.itemsSource = m_selectedCollection;
			m_serializedSelectedCollection = new SerializedObject(m_selectedCollection);

			EditorPrefs.SetString(SELECTED_COLLECTION_PREF, AssetUtil.GetGUID(m_selectedCollection));
		}

		internal void SetSelectedCollection(SaveDefinitionCollection collection)
		{
			if (m_collections.Count == 0)
			{
				PopulateDefinitionCollections();
			}
			m_selectedCollectionDropdown.index = m_collections.IndexOf(collection);
		}

		#endregion

		#region Bind Methods

		private void BindMultiColumnListView()
		{
			var nameColumn = m_multiColumnListView.columns["name"];
			nameColumn.makeCell = () =>
			{
				var nameField = new TextField();
				nameField.isDelayed = true;
				nameField.RegisterValueChangedCallback(NameDefinition_ValueChanged);

				if (Application.isPlaying)
				{
					nameField.SetEnabled(false);
				}

				return nameField;
			};
			nameColumn.bindCell = (element, index) =>
			{
				var nameField = element as TextField;
				nameField.userData = index;
				nameField.value = m_selectedCollection[index].name;
			};

			var idColumn = m_multiColumnListView.columns["id"];
			idColumn.makeCell = () => new Label();
			idColumn.bindCell = (element, index) =>
			{
				(element as Label).text = m_selectedCollection[index].id.ToString();
			};

			var typeColumn = m_multiColumnListView.columns["type"];
			typeColumn.makeCell = () => new Label();
			typeColumn.bindCell = (element, index) =>
			{
				(element as Label).text = m_selectedCollection[index].typeName;
			};

			var defaultValueColumn = m_multiColumnListView.columns["defaultValue"];
			defaultValueColumn.makeCell = () => new VisualElement();
			defaultValueColumn.bindCell = (element, index) =>
			{
				SaveDefinition definition = m_selectedCollection[index];
				for (int i = element.childCount - 1; i >= 0; --i)
				{
					element.RemoveAt(i);
				}

				var imguiContainer = new IMGUIContainer(definition.defaultGUIHandler)
				{
					enabledSelf = !Application.isPlaying,
					userData = index,
				};
				imguiContainer.Bind(m_serializedSelectedCollection);
				imguiContainer.TrackSerializedObjectValue(m_serializedSelectedCollection, (serializedObject) =>
				{
					EditorUtility.SetDirty(m_selectedCollection);
				});
				element.Add(imguiContainer);
			};

			var curentValueColumn = m_multiColumnListView.columns["currentValue"];
			curentValueColumn.makeCell = () => new VisualElement();
			curentValueColumn.bindCell = (element, index) =>
			{
				SaveDefinition definition = m_selectedCollection[index];
				for (int i = element.childCount - 1; i >= 0; --i)
				{
					element.RemoveAt(i);
				}

				element.Add(new IMGUIContainer(definition.currentGUIHandler)
				{
					enabledSelf = false,
					userData = index,
				});
			};


			var resetIcon = EditorGUIUtility.IconContent("TreeEditor.Refresh");
			var resetColumn = m_multiColumnListView.columns["reset"];
			resetColumn.makeCell = () => CreateButton(resetIcon.image as Texture2D, ResetButton_Clicked, "Reset");
			resetColumn.bindCell = (element, index) =>
			{
				(element as Button).userData = index;
			};

			var removeIcon = EditorGUIUtility.IconContent("TreeEditor.Trash");
			var removeColumn = m_multiColumnListView.columns["remove"];
			removeColumn.makeCell = () => CreateButton(removeIcon.image as Texture2D, RemoveButton_Clicked, "Delete");
			removeColumn.bindCell = (element, index) =>
			{
				(element as Button).userData = index;
			};
		}

		private void NameDefinition_ValueChanged(ChangeEvent<string> args)
		{
			var textField = args.currentTarget as TextField;
			if (textField == null)
				return;

			m_selectedCollection[(int)textField.userData].name = textField.text;
			EditorUtility.SetDirty(m_selectedCollection);
			EntryChanged?.Invoke();
		}

		private static void ResetButton_Clicked(ClickEvent e)
		{

		}

		private void RemoveButton_Clicked(ClickEvent e)
		{
			if (m_selectedCollection == null)
				return;

			int index = (int)(e.currentTarget as VisualElement).userData;
			if (!EditorUtility.DisplayDialog(
				"Delete Save Variable?",
				$"Are you sure you want to delete {m_selectedCollection[index].name}?\n\nYou cannot undo the delete variable action.",
				"Delete",
				"Cancel"))
			{
				return;
			}

			m_selectedCollection.RemoveAt(index);
			m_multiColumnListView.RefreshItems();
			EditorUtility.SetDirty(m_selectedCollection);

			EntryChanged?.Invoke();
		}

		private Button CreateButton(Texture2D icon, EventCallback<ClickEvent> clickCallback, string tooltip = null)
		{
			var button = new Button()
			{
				iconImage = new Background()
				{
					texture = icon,
				},
				tooltip = tooltip
			};
			button.RegisterCallback<ClickEvent>(clickCallback);

			if (Application.isPlaying)
			{
				button.SetEnabled(false);
			}

			return button;
		}

		#endregion
	}
}