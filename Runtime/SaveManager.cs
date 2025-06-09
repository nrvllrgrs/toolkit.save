using OdinSerializer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.SaveManagement
{
    public class SaveManager : ConfigurableSubsystem<SaveManager, SaveManagerConfig>
    {
		#region Fields

		private Dictionary<string, object> m_map = new();

		#endregion

		#region Methods

		protected override void Initialize()
		{
			foreach (var collection in Config.collections)
			{
				foreach (var entry in collection)
				{
					m_map.Add(entry.id, entry.value);
				}
			}
		}

		public void Save()
		{
			Save(Path.Combine(Application.persistentDataPath, "save001.sav"));
		}

		public void Save(string path)
		{
			byte[] bytes = SerializationUtility.SerializeValue(m_map, DataFormat.Binary);
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
			m_map = SerializationUtility.DeserializeValue<Dictionary<string, object>>(bytes, DataFormat.Binary);
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

			if (m_map.TryGetValue(id, out var v))
			{
				value = (T)v;
				return true;
			}

			value = default;
			return false;
		}

		public bool TrySetValue<T>(string id, T value)
		{
			if (!m_map.ContainsKey(id))
				return false;

			m_map[id] = value;
			return true;
		}

		public bool Add(string id, object value)
		{
			if (m_map.ContainsKey(id))
				return false;

			m_map.Add(id, value);
			return true;
		}

		public bool Remove(string id)
		{
			if (!m_map.ContainsKey(id))
				return false;

			m_map.Remove(id);
			return true;
		}

		#endregion
	}
}