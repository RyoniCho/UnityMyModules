using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ControlRoom
{
    static class Menu 
    {
        [MenuItem("ControlRoom/Build/BuildTableCSV")]
        static void SaveCSVTableDataAtLocal()
        {

            TableDataBuilder.BuildTableDataFromCSV();
        }
        [MenuItem("ControlRoom/Build/BuildTableDataBinary")]
        static void BuildTableData_Binary()
        {

           
            TableDataBuilder.BuildTableDataFromBinary();
        }

        [MenuItem("ControlRoom/ClearSaveData")]
        static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();

        }

    }
}

