using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ControlRoom
{
    public class TableDataLoader : MonoBehaviour
    {
        public const string baseUrl = "https://docs.google.com/spreadsheet/pub?key={0}&single=true&output=csv&gid={1}";
        public static bool OnlineMode = true;

        private TableData data;
        private static TableDataLoader loader;
        public static TableDataLoader Loader
        {
            get
            {
                if(loader == null)
                {
                    GameObject obj = TableManager.Instance.gameObject;
                    loader = obj.GetComponent<TableDataLoader>();

                    if (loader == null)
                        loader = obj.AddComponent<TableDataLoader>();
                  
                }
                return loader;
            }
        }
            
    
        public delegate void CallBack(TableData tData);
        public delegate void GoogleSheetDownloadCallback(string rawText);
        public delegate void BinaryLoadCallback(System.IO.BinaryReader reader);

        
        private static void LoadOnlineTableDataFromGoogleSheet(int docsId, CallBack callBack)
        {
            Loader.LoadOnlineCSVData(docsId, callBack);
        }

        private static void LoadTableDataFromCSVFile(int docsId, CallBack callBack)
        {
            Loader.LoadCSVFile(docsId, callBack);
        }

        private static void LoadTableDataFromBinaryFile(int docsId, BinaryLoadCallback callback)
        {
            Loader.LoadBinaryFile(docsId, callback);
        }

        public static void DownloadGoogleDocs(int docsId, GoogleSheetDownloadCallback callback)
        {
            Loader.DownloadOnlineCSV(docsId, callback);
        }

        public static void LoadData(int docsId,CallBack callback)
        {
            if (OnlineMode)
                LoadOnlineTableDataFromGoogleSheet(docsId, callback);
            else
                LoadTableDataFromCSVFile(docsId, callback);
        }

        public static void LoadData(int docsId, BinaryLoadCallback callback)
        {
            LoadTableDataFromBinaryFile(docsId, callback);
        }

        void DownloadOnlineCSV(int docsId, GoogleSheetDownloadCallback callback)
        {
            StartCoroutine(RequestToDownloadGoogleDocs(docsId, callback));
        }

        void LoadOnlineCSVData(int docsId, CallBack callback)
        {
            StartCoroutine(RequestToDownloadGoogleDocs(docsId, (string downloadedText) =>
            {
                data = new TableData();
                data.LoadRawCSVText(downloadedText);
                callback(data);

            }));
        }

        void LoadCSVFile(int docsId,CallBack callback)
        {
            var text = Resources.Load<TextAsset>(string.Format("Table/{0}", ((TableManager.GoogleDocsID)docsId).ToString().ToLower()));
            if (text != null)
            {
                data = new TableData();

                data.LoadRawCSVText(text.text);
                callback(data);
            }
            else
            {
                Debug.LogError(string.Format("{0} - local docs data could not loaded", docsId));
            }
        }

        void LoadBinaryFile(int docsId, BinaryLoadCallback callback)
        {
            var binPath = $"{Application.dataPath}/Resources/Table/{((TableManager.GoogleDocsID)docsId).ToString().ToLower()}.bin";
            try
            {
                var bytes = System.IO.File.ReadAllBytes(binPath);
                using (var memoryStream = new System.IO.MemoryStream(bytes))
                {
                    using (var reader = new System.IO.BinaryReader(memoryStream))
                    {
                        while (memoryStream.Position < memoryStream.Length)
                        {
                            callback(reader);
                        }
                    }
                }
            }
            catch
            {
                Debug.LogError(string.Format("{0} - local Binary data could not loaded", docsId));
            }
           
        }


        IEnumerator RequestToDownloadGoogleDocs(int docsId, GoogleSheetDownloadCallback callBack)
        {
            string docsKey=TableManager.docsKey;

            string url = string.Format(baseUrl, docsKey, docsId);
            UnityWebRequest request = UnityWebRequest.Get(url);
          

            yield return request.SendWebRequest();

            if(request.result==UnityWebRequest.Result.ConnectionError||request.result==UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network Error");
            }
            else
            {
                callBack(request.downloadHandler.text);
                
            }

        }


    }
    


}
