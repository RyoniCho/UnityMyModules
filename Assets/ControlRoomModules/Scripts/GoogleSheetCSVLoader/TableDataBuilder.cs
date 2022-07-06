using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ControlRoom
{
    public static class TableDataBuilder 
    { 
        public delegate void BinaryBuildCallback(TableData tData, System.IO.BinaryWriter writer);
        private static int buildCount = 0;
        private static Stopwatch stopwatch = new Stopwatch();
        private static async Task DownloadCSVAndCreateFile(int docsId)
        {
            await TableDataLoader.DownloadGoogleDocs(docsId,(string csvText) =>
            {

                //Table Directory Check
                if (!System.IO.Directory.Exists(Application.dataPath + "/Resources/Table"))
                {
                    System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/Table");
                }

                string path = string.Format($"{ Application.dataPath}/Resources/Table/{((TableManager.GoogleDocsID)docsId).ToString().ToLower()}.csv");

                System.IO.File.WriteAllText(path, csvText);

                UnityEngine.Debug.Log("Table Save:" + path);
                buildCount += 1;


            });
        }

        public static async Task DownloadCSVAndCreateBinaryFile(int docsId, BinaryBuildCallback callBack)
        {
            await TableDataLoader.DownloadGoogleDocs(docsId, (string csvText) =>
            {
                //Table Directory Check
                if (!System.IO.Directory.Exists(Application.dataPath + "/Resources/Table"))
                {
                    System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/Table");
                }

                string path = string.Format($"{ Application.dataPath}/Resources/Table/{((TableManager.GoogleDocsID)docsId).ToString().ToLower()}.bin");

                using (System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                {
                    using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(fileStream))
                    {
                        var data = new TableData();
                        data.LoadRawCSVText(csvText);

                        callBack(data, writer);
                        buildCount += 1;
                        UnityEngine.Debug.Log("Table Save:" + path);
                    }
                }
            });
        }

        public static async void BuildTableDataFromCSV()
        {
            UnityEngine.Debug.Log("CSV TABLE Build START");
            stopwatch.Start();
            EditorUtility.DisplayProgressBar("테이블 빌드 (To CSV)", "테이블 빌드중입니다.", 0f);
            var maxCount = System.Enum.GetValues(typeof(TableManager.GoogleDocsID)).Length- 1;
            buildCount = 0;


            foreach (TableManager.GoogleDocsID id in System.Enum.GetValues(typeof(TableManager.GoogleDocsID)))
            {
             
                if (id != TableManager.GoogleDocsID.NONE)
                {
                    await DownloadCSVAndCreateFile((int)id);
                    EditorUtility.DisplayProgressBar("테이블 빌드 (To CSV)", "테이블 빌드중입니다.", (float)buildCount/maxCount);
                }
                    
            }
            
            UnityEngine.Debug.Log("CSV TABLE Build END");
            stopwatch.Stop();

            EditorUtility.ClearProgressBar();

            UnityEngine.Debug.Log($"CSV Table Build ElapsTime:{stopwatch.ElapsedMilliseconds}");
        }

        public static async void BuildTableDataFromBinary()
        {
            UnityEngine.Debug.Log("Binary Table Build START");
            stopwatch.Start();

            EditorUtility.DisplayProgressBar("테이블 빌드 (To Binary)", "테이블 빌드중입니다.", 0f);
            var maxCount = System.Enum.GetValues(typeof(TableManager.GoogleDocsID)).Length - 1;
            buildCount = 0;
                     
            await TableManager.Instance.BuildBinaryDataAll(()=> 
            {
                EditorUtility.DisplayProgressBar("테이블 빌드 (To Binary)", "테이블 빌드중입니다.", (float)buildCount / maxCount);
            });
           
            UnityEngine.Debug.Log("Binary Table Build END");
            stopwatch.Stop();
            EditorUtility.ClearProgressBar();

            UnityEngine.Debug.Log($"Binary Table Build ElapsTime:{stopwatch.ElapsedMilliseconds}");
        }

      
    }

    [InitializeOnLoad]
    public class AddDefineSymbols : Editor
    {
        public static readonly string[] Symbols = new string[] { "TABLE_DATA_BUILDER" };
 
        static AddDefineSymbols()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(Symbols.Except(allDefines));

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
             string.Join(";", allDefines.ToArray()));

        }
    }
}

