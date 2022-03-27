using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public static class TableDataBuilder 
    {
        public delegate void BinaryBuildCallback(TableData tData, System.IO.BinaryWriter writer);

        private static void DownloadCSVAndCreateFile(int docsId)
        {
            TableDataLoader.DownloadGoogleDocs(docsId,(string csvText) =>
            {

                //Table Directory Check
                if (!System.IO.Directory.Exists(Application.dataPath + "/Resources/Table"))
                {
                    System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/Table");
                }

                string path = string.Format($"{ Application.dataPath}/Resources/Table/{((TableManager.GoogleDocsID)docsId).ToString().ToLower()}.csv");

                System.IO.File.WriteAllText(path, csvText);

                Debug.Log("Table Save:" + path);
            });
        }

        public static void DownloadCSVAndCreateBinaryFile(int docsId, BinaryBuildCallback callBack)
        {
            TableDataLoader.DownloadGoogleDocs(docsId, (string csvText) =>
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

                    }
                }
            });
        }

        public static void BuildTableDataFromCSV()
        {
            foreach (TableManager.GoogleDocsID id in System.Enum.GetValues(typeof(TableManager.GoogleDocsID)))
            {
                DownloadCSVAndCreateFile((int)id);
            }
        }

        public static void BuildTableDataFromBinary()
        {
            TableManager.Instance.BuildBinaryDataAll();
            
        }

      
    }
}

