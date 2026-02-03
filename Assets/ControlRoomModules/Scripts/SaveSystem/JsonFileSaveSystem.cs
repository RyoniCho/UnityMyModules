using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace ControlRoom
{
    public class JsonFileSaveSystem : ISaveSystem
    {
        private string GetPath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) fileName = "default_save.json";
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        // Simple wrapper to store key-value pairs in a single file if needed,
        // but for this implementation, we'll treat 'fileName' as the distinct storage unit
        // and 'key' as a specific entry if we were doing a full DB. 
        // HOWEVER, to keep it compatible with the interface where keys might be distinct objects:
        // A simple approach is: File = Dictionary<string, string> (Serialized)
        
        [System.Serializable]
        private class SaveContainer
        {
            public string key;
            public string jsonData;
        }

        [System.Serializable]
        private class FileData
        {
            public List<SaveContainer> items = new List<SaveContainer>();
        }

        public void Save<T>(string key, T data, string fileName = null)
        {
            string path = GetPath(fileName);
            FileData fileData = LoadFile(path);

            string jsonPayload = JsonUtility.ToJson(data);
            
            // Update or Add
            var item = fileData.items.Find(x => x.key == key);
            if (item != null)
            {
                item.jsonData = jsonPayload;
            }
            else
            {
                fileData.items.Add(new SaveContainer { key = key, jsonData = jsonPayload });
            }

            File.WriteAllText(path, JsonUtility.ToJson(fileData, true));
        }

        public T Load<T>(string key, string fileName = null, T defaultValue = default)
        {
            string path = GetPath(fileName);
            if (!File.Exists(path)) return defaultValue;

            FileData fileData = LoadFile(path);
            var item = fileData.items.Find(x => x.key == key);

            if (item != null)
            {
                return JsonUtility.FromJson<T>(item.jsonData);
            }
            return defaultValue;
        }

        public bool Exists(string key, string fileName = null)
        {
            string path = GetPath(fileName);
            if (!File.Exists(path)) return false;

            FileData fileData = LoadFile(path);
            return fileData.items.Exists(x => x.key == key);
        }

        public void Delete(string key, string fileName = null)
        {
            string path = GetPath(fileName);
            if (!File.Exists(path)) return;

            FileData fileData = LoadFile(path);
            int removed = fileData.items.RemoveAll(x => x.key == key);

            if (removed > 0)
            {
                File.WriteAllText(path, JsonUtility.ToJson(fileData, true));
            }
        }

        private FileData LoadFile(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<FileData>(json) ?? new FileData();
            }
            return new FileData();
        }
    }
}
