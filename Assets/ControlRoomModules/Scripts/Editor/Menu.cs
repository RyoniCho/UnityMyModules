using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ControlRoom
{
    static class Menu 
    {
#if TABLE_DATA_BUILDER
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
#endif

        [MenuItem("ControlRoom/Build/BuildClient(AOS)")]
        static void BuildAndroid()
        {
            BatchBuilder.Build_AOS();

        }
        [MenuItem("ControlRoom/Build/BuildClient(IOS)")]
        static void BuildIOS()
        {
            BatchBuilder.Build_IOS();

        }

        [MenuItem("ControlRoom/ClearSaveData")]
        static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();

        }

    }
}

