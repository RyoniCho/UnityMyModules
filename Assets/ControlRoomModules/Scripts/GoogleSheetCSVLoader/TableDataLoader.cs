using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace ControlRoom
{
    public class TableDataLoader 
    {
        public const string baseUrl = "https://docs.google.com/spreadsheet/pub?key={0}&single=true&output=csv&gid={1}";
        public static bool OnlineMode = true;

        private static TableData data;
       
            
    
        public delegate void CallBack(TableData tData);
        public delegate void GoogleSheetDownloadCallback(string rawText);
        public delegate void BinaryLoadCallback(System.IO.BinaryReader reader);

        
        private static async Task LoadOnlineTableDataFromGoogleSheet(int docsId, CallBack callBack)
        {
            await LoadOnlineCSVData(docsId, callBack);
        }

        private static void LoadTableDataFromCSVFile(int docsId, CallBack callBack)
        {
            LoadCSVFile(docsId, callBack);
        }

        private static async Task LoadTableDataFromBinaryFile(int docsId, BinaryLoadCallback callback)
        {
            await LoadBinaryFile(docsId, callback);
        }

        public static async Task DownloadGoogleDocs(int docsId, GoogleSheetDownloadCallback callback)
        {
            await DownloadOnlineCSV(docsId, callback);
        }

        public static async Task LoadData(int docsId,CallBack callback)
        {
            if (OnlineMode)
                await LoadOnlineTableDataFromGoogleSheet(docsId, callback);
            else
                LoadTableDataFromCSVFile(docsId, callback);
        }

        public static async Task LoadData(int docsId, BinaryLoadCallback callback)
        {
            await LoadTableDataFromBinaryFile(docsId, callback);
        }

        static async Task DownloadOnlineCSV(int docsId, GoogleSheetDownloadCallback callback)
        {
            //StartCoroutine(RequestToDownloadGoogleDocs(docsId, callback));
            await RequestToDownloadGoogleDocs(docsId, callback); 
            
        }

        static async Task LoadOnlineCSVData(int docsId, CallBack callback)
        {
            await RequestToDownloadGoogleDocs(docsId, (string downloadedText) =>
            {
                data = new TableData();
                data.LoadRawCSVText(downloadedText);
                callback(data);

            });
        }

        static void LoadCSVFile(int docsId,CallBack callback)
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

        static async Task LoadBinaryFile(int docsId, BinaryLoadCallback callback)
        {
            await Task.Run(() =>
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
            });
            
           
        }


        //IEnumerator RequestToDownloadGoogleDocs(int docsId, GoogleSheetDownloadCallback callBack)
        //{
        //    string docsKey=TableManager.docsKey;

        //    string url = string.Format(baseUrl, docsKey, docsId);
        //    UnityWebRequest request = UnityWebRequest.Get(url);
          

        //    yield return request.SendWebRequest();

        //    if(request.result==UnityWebRequest.Result.ConnectionError||request.result==UnityWebRequest.Result.ProtocolError)
        //    {
        //        Debug.LogError("Network Error");
        //    }
        //    else
        //    {
        //        callBack(request.downloadHandler.text);
                
        //    }

        //}

         static async Task RequestToDownloadGoogleDocs(int docsId, GoogleSheetDownloadCallback callBack)
         {
            string docsKey = TableManager.docsKey;

            string url = string.Format(baseUrl, docsKey, docsId);
          
            var request = System.Net.WebRequest.Create(url);


            var response = await request.GetResponseAsync();

            using (System.IO.Stream stream = response.GetResponseStream())
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
                string responseString = reader.ReadToEnd();
                callBack(responseString);
            }

            response.Close();

        }

    }
    


}
