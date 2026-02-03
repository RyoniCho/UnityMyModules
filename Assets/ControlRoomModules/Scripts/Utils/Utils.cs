using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public interface IStayableData
    {
        DataSetting GetDataSetting();

        void SetDataSetting(string dataTag, DataSetting.DataPermission permissionType);

        Data SaveData();

        void LoadData(Data data);
    }

    public interface IGetHit
    {
        bool GetHit(int hitValue, Vector2 attackerPosition);
        Vector2[] GetHitPoints();
    }

    public interface ITalkObject
    {
        Transform GetTalkUIPoint();
        int GetTalkObjectIndex();
    }

    public interface IActionEvent
    {
        void ExecuteAction(int actionEventType); // Changed ActionEventType enum to int to avoid dependency if enum is missing
    }

    [Serializable]
    public class Data
    {
    }

    [Serializable]
    public class Data<T> : Data
    {
        public T value;

        public Data(T value)
        {
            this.value = value;
        }
    }

    [Serializable]
    public class Data<T0, T1> : Data
    {
        public T0 value0;
        public T1 value1;

        public Data(T0 value0, T1 value1)
        {
            this.value0 = value0;
            this.value1 = value1;
        }
    }

    [Serializable]
    public class DataSetting
    {
        public enum DataPermission
        {
            ReadOnly,
            WriteOnly,
            ReadWrite,
        }
        public DataPermission dataPermission = DataPermission.ReadWrite;
        public string dataTag = System.Guid.NewGuid().ToString();
    }

    [Serializable]
    public class SaveMetaInfo
    {
        public string key;
        public string classTypeName;

        public SaveMetaInfo(string _key, string _classTypeName)
        {
            this.key = _key;
            this.classTypeName = _classTypeName;
        }
    }

    public static class Utils
    {
        public static bool CheckLayerMask(LayerMask layerMask, int layer)
        {
            if (layerMask == (layerMask | (1 << layer)))
                return true;

            return false;
        }
    }
}