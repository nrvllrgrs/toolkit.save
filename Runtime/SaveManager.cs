using OdinSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
	public class VariableEventArgs : EventArgs
	{
		#region Properties

		public string variableName { get; private set; }
		public bool boolValue { get; private set; }
		public float floatValue { get; private set; }
		public string stringValue { get; private set; }

		#endregion

		#region Constructors

		private VariableEventArgs(string variableName)
		{
			this.variableName = variableName;
		}

		public VariableEventArgs(string variableName, object value)
			: this(variableName)
		{
			if (value is bool)
			{
				boolValue = (bool)value;
			}
			else if (value is float)
			{
				floatValue = (float)value;
			}
			else if (value is string)
			{
				stringValue = (string)value;
			}
		}

		public VariableEventArgs(string variableName, bool value)
			: this(variableName)
		{
			boolValue = value;
		}

		public VariableEventArgs(string variableName, float value)
			: this(variableName)
		{
			floatValue = value;
		}

		public VariableEventArgs(string variableName, string value)
			: this(variableName)
		{
			stringValue = value;
		}

		#endregion
	}

	public class SaveManager : ConfigurableSubsystem<SaveManager, SaveManagerConfig>
    {
		#region Fields

		private Dictionary<string, object> m_variableMap = new();
		private HashSet<ISaveable> m_saveables = new();

		#endregion

		#region Events

		public event EventHandler<VariableEventArgs> VariableChanged;

		#endregion

		#region Methods

		protected override void Initialize()
		{
			foreach (var collection in Config.collections)
			{
				if (collection == null)
					continue;

				foreach (var entry in collection)
				{
					m_variableMap.Add(entry.id, entry.value);
				}
			}
		}

		public void Save()
		{
			Save(Path.Combine(Application.persistentDataPath, "save001.sav"));
		}

		public void Save(string path)
		{
			foreach (var saveable in m_saveables)
			{
				saveable.Save();
			}

			byte[] bytes = SerializationUtility.SerializeValue(m_variableMap, DataFormat.Binary);
			File.WriteAllBytes(path, bytes);
		}

		public void Load()
		{

		}

		public void Load(string path)
		{
			if (!File.Exists(path))
				return;

			byte[] bytes = File.ReadAllBytes(path);
			m_variableMap = SerializationUtility.DeserializeValue<Dictionary<string, object>>(bytes, DataFormat.Binary);
		}

		public bool ContainsId(string id)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				var definition = Config.collections.SelectMany(x => x)
					.FirstOrDefault(x => Equals(x.id, id));

				return definition != null;
			}
#endif

			return m_variableMap.ContainsKey(id);
		}


		public bool TryGetValue<T>(string id, out T value)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				var definition = Config.collections.SelectMany(x => x)
					.FirstOrDefault(x => Equals(x.id, id));

				if (definition == null)
				{
					value = default;
					return false;
				}

				value = (T)definition.value;
				return true;
			}
#endif

			if (m_variableMap.TryGetValue(id, out var v))
			{
				value = (T)v;
				return true;
			}

			value = default;
			return false;
		}

		public bool TrySetValue<T>(string id, T value)
		{
			if (!m_variableMap.ContainsKey(id))
				return false;

			m_variableMap[id] = value;
			return true;
		}

		public bool Add(string id, object value)
		{
			if (m_variableMap.ContainsKey(id))
				return false;

			m_variableMap.Add(id, value);
			return true;
		}

		public bool Remove(string id)
		{
			if (!m_variableMap.ContainsKey(id))
				return false;

			m_variableMap.Remove(id);
			return true;
		}

		#endregion

		#region Saveable Methods

		public void Register(ISaveable saveable)
		{
			m_saveables.Add(saveable);
		}

		public void Unregister(ISaveable saveable)
		{
			m_saveables.Remove(saveable);
		}

		#endregion

		#region Variable Methods

		public void InvokeVariableChanged(VariableEventArgs e)
		{
			VariableChanged?.Invoke(this, e);
		}
		
		#endregion

	}
}