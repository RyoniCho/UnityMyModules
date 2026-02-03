using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class SaveManager : SingletonBase<SaveManager>
    {
        private ISaveSystem saveSystem;
        private Dictionary<string, Data> dicData = new Dictionary<string, Data>();
        private HashSet<IStayableData> hsStayableData = new HashSet<IStayableData>();

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);

            // Default implementation (can be swapped)
            saveSystem = new PlayerPrefsSaveSystem();
        }

        public void SetSaveSystem(ISaveSystem system)
        {
            this.saveSystem = system;
        }

        public void Register(IStayableData data)
        {
            if (data != null && !string.IsNullOrEmpty(data.GetDataSetting().dataTag))
            {
                hsStayableData.Add(data);
            }
        }

        public void UnRegister(IStayableData data)
        {
            hsStayableData.Remove(data);
        }

        public void SaveAllRegisteredData()
        {
            foreach (var data in hsStayableData)
            {
                SaveRuntimeData(data);
            }
        }

        public void LoadAllRegisteredData()
        {
            foreach (var data in hsStayableData)
            {
                LoadRuntimeData(data);
            }
        }

        private void SaveRuntimeData(IStayableData data)
        {
            if (data.GetDataSetting().dataPermission == DataSetting.DataPermission.ReadOnly)
                return;

            dicData[data.GetDataSetting().dataTag] = data.SaveData();
        }

        private void LoadRuntimeData(IStayableData data)
        {
            if (data.GetDataSetting().dataPermission == DataSetting.DataPermission.WriteOnly)
                return;

            if (dicData.ContainsKey(data.GetDataSetting().dataTag))
            {
                data.LoadData(dicData[data.GetDataSetting().dataTag]);
            }
        }

        // --- File I/O Delegates ---

        public void SaveToStorage<T>(string key, T data, string fileName = null)
        {
            saveSystem?.Save(key, data, fileName);
        }

        public T LoadFromStorage<T>(string key, string fileName = null, T defaultValue = default)
        {
            if (saveSystem == null) return defaultValue;
            return saveSystem.Load(key, fileName, defaultValue);
        }

        public bool ExistsInStorage(string key, string fileName = null)
        {
            return saveSystem != null && saveSystem.Exists(key, fileName);
        }
    }
}
