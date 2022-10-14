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
            if (EditorUtility.DisplayDialog("Table Build", "CSV로 테이블 빌드하시겠습니까?", "확인", "취소"))
            {
                TableDataBuilder.BuildTableDataFromCSV();
            }
           
        }
        [MenuItem("ControlRoom/Build/BuildTableDataBinary")]
        static void BuildTableData_Binary()
        {

            if (EditorUtility.DisplayDialog("Table Build", "Binary로 테이블 빌드하시겠습니까?", "확인", "취소"))
            {
                TableDataBuilder.BuildTableDataFromBinary();
            }
            
        }

        [MenuItem("ControlRoom/Build/BuidTableAndCopyToStreamingAssets")]
        static void BuildTableAndCopyToStreamingAssets()
        {
            BatchBuilder.BuildAndMoveTableData();

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

