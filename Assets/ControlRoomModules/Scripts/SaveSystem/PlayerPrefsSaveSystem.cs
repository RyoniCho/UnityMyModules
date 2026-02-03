using UnityEngine;

namespace ControlRoom
{
    public class PlayerPrefsSaveSystem : ISaveSystem
    {
        public void Save<T>(string key, T data, string fileName = null)
        {
            // fileName is ignored in PlayerPrefs
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public T Load<T>(string key, string fileName = null, T defaultValue = default)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                try
                {
                    return JsonUtility.FromJson<T>(json);
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public bool Exists(string key, string fileName = null)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void Delete(string key, string fileName = null)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}
