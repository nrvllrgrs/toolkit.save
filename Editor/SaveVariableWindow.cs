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

				switch (definition.GetType())
				{
					case Type boolDefinition when boolDefinition == typeof(BoolDefinition):
						element.Add(CreateDefinitionCell(definition, index, new Toggle(), BoolDefinition_ValueChanged));
						break;

					case Type intDefinition when intDefinition == typeof(IntDefinition):
						element.Add(CreateDefinitionCell(definition, index, new IntegerField(), IntDefinition_ValueChanged));
						break;

					case Type floatDefinition when floatDefinition == typeof(FloatDefinition):
						element.Add(CreateDefinitionCell(definition, index, new FloatField(), FloatDefinition_ValueChanged));
						break;

					case Type stringDefinition when stringDefinition == typeof(StringDefinition):
						element.Add(CreateDefinitionCell(definition, index, new TextField(), StringDefinition_ValueChanged));
						break;

					case Type vector2Definition when vector2Definition == typeof(Vector2Definition):
						element.Add(CreateDefinitionCell(definition, index, new Vector2Field(), Vector2Definition_ValueChanged));
						break;

					case Type vector2IntDefinition when vector2IntDefinition == typeof(Vector2IntDefinition):
						element.Add(CreateDefinitionCell(definition, index, new Vector2IntField(), Vector2IntDefinition_ValueChanged));
						break;

					case Type vector3Definition when vector3Definition == typeof(Vector3Definition):
						element.Add(CreateDefinitionCell(definition, index, new Vector3Field(), Vector3Definition_ValueChanged));
						break;

					case Type vector3IntDefinition when vector3IntDefinition == typeof(Vector3IntDefinition):
						element.Add(CreateDefinitionCell(definition, index, new Vector3IntField(), Vector3IntDefinition_ValueChanged));
						break;

					case Type vector4Definition when vector4Definition == typeof(Vector4Definition):
						element.Add(CreateDefinitionCell(definition, index, new Vector4Field(), Vector4Definition_ValueChanged));
						break;

					default:
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
						break;
				}
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

				switch (definition.GetType())
				{
					case Type boolDefinition when boolDefinition == typeof(BoolDefinition):
						element.Add(CreateVariableCell(definition, index, new Toggle()));
						break;

					case Type intDefinition when intDefinition == typeof(IntDefinition):
						element.Add(CreateVariableCell(definition, index, new IntegerField()));
						break;

					case Type floatDefinition when floatDefinition == typeof(FloatDefinition):
						element.Add(CreateVariableCell(definition, index, new FloatField()));
						break;

					case Type stringDefinition when stringDefinition == typeof(StringDefinition):
						element.Add(CreateVariableCell(definition, index, new TextField()));
						break;

					case Type vector2Definition when vector2Definition == typeof(Vector2Definition):
						element.Add(CreateVariableCell(definition, index, new Vector2Field()));
						break;

					case Type vector2IntDefinition when vector2IntDefinition == typeof(Vector2IntDefinition):
						element.Add(CreateVariableCell(definition, index, new Vector2IntField()));
						break;

					case Type vector3Definition when vector3Definition == typeof(Vector3Definition):
						element.Add(CreateVariableCell(definition, index, new Vector3Field()));
						break;

					case Type vector3IntDefinition when vector3IntDefinition == typeof(Vector3IntDefinition):
						element.Add(CreateVariableCell(definition, index, new Vector3IntField()));
						break;

					case Type vector4Definition when vector4Definition == typeof(Vector4Definition):
						element.Add(CreateVariableCell(definition, index, new Vector4Field()));
						break;

					default:
						element.Add(CreateVariableCell(definition, index));
						break;
				}
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

		#region Definition Cell Methods

		private VisualElement CreateDefinitionCell<T>(SaveDefinition definition, int index, BaseField<T> field, EventCallback<ChangeEvent<T>> callback)
		{
			field.value = (definition as SaveDefinition<T>).castValue;
			field.RegisterValueChangedCallback(callback);
			field.userData = index;

			if (Application.isPlaying)
			{
				field.SetEnabled(false);
			}

			return field;
		}

		private void BoolDefinition_ValueChanged(ChangeEvent<bool> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as Toggle).value;
			});
		}

		private void IntDefinition_ValueChanged(ChangeEvent<int> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as IntegerField).value;
			});
		}

		private void FloatDefinition_ValueChanged(ChangeEvent<float> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as FloatField).value;
			});
		}

		private void StringDefinition_ValueChanged(ChangeEvent<string> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as TextField).value;
			});
		}

		private void Vector2Definition_ValueChanged(ChangeEvent<Vector2> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as Vector2Field).value;
			});
		}

		private void Vector2IntDefinition_ValueChanged(ChangeEvent<Vector2Int> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as Vector2IntField).value;
			});
		}

		private void Vector3Definition_ValueChanged(ChangeEvent<Vector3> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as Vector3Field).value;
			});
		}

		private void Vector3IntDefinition_ValueChanged(ChangeEvent<Vector3Int> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as Vector3IntField).value;
			});
		}

		private void Vector4Definition_ValueChanged(ChangeEvent<Vector4> args)
		{
			SaveDefinition_ValueChanged(args.currentTarget, (element) =>
			{
				return (element as Vector4Field).value;
			});
		}

		private void SaveDefinition_ValueChanged<T>(IEventHandler target, Func<VisualElement, T> getValue)
		{
			var element = target as VisualElement;
			if (element == null)
				return;

			(m_selectedCollection[(int)element.userData] as SaveDefinition<T>).castValue = getValue(element);
			EditorUtility.SetDirty(m_selectedCollection);
		}

		#endregion

		#region Variable Cell Methods

		private VisualElement CreateVariableCell<T>(SaveDefinition definition, int index, BaseField<T> field)
		{
			SaveManager.CastInstance.TryGetValue(definition.id, out T value);
			field.value = value;
			field.userData = index;
			field.SetEnabled(false);

			return field;
		}

		private VisualElement CreateVariableCell(SaveDefinition definition, int index)
		{
			return new IMGUIContainer(definition.currentGUIHandler)
			{
				enabledSelf = false,
				userData = index,
			};
		}

		#endregion
	}
}