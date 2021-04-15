using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ControlRoom
{
    static class Menu 
    {
        [MenuItem("ControlRoom/SaveTableDataAtLocal")]
        static void SaveTableDataAtLocal()
        {

            LocalizationDataManager.Instance.DownLoadAndSaveData();
            ItemDataManager.Instance.DownLoadAndSaveData();
        }

        [MenuItem("ControlRoom/ClearSaveData")]
        static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();

        }

    }
}

