using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ControlRoom
{
    public class TableDataLoader: MonoBehaviour
    {
        public const string baseUrl= "https://docs.google.com/spreadsheet/pub?key={0}&single=true&output=csv&gid={1}";

        private TableData data;
    
        public delegate void CallBack(TableData tData);

        public static void StartDownload(int docsId,CallBack callBack,bool saveData=false)
        {
            GameObject obj = TableManager.Instance.gameObject;
            var dataLoader= obj.AddComponent<TableDataLoader>();

            dataLoader.RequestToLoadGoogleDocs(docsId, callBack,saveData);
        }

        void RequestToLoadGoogleDocs(int docsId, CallBack callBack,bool saveData=false)
        {
            if(TableManager.Instance.IsOnlineLiveLoadMode||saveData)
                StartCoroutine(LoadGoogleDocs(docsId, callBack,saveData));
            else
            {
                var text = Resources.Load<TextAsset>(string.Format("Table/{0}", ((TableManager.GoogleDocsID)docsId).ToString().ToLower()));
                if(text!=null)
                {
                    data = new TableData();
                    
                    data.LoadRawText(text.text);
                    callBack(data);
                }
                else
                {
                    Debug.LogError(string.Format("{0} - local docs data could not loaded", docsId));
                }


            }
        }


        IEnumerator LoadGoogleDocs(int docsId,CallBack callBack,bool saveData=false)
        {
            string docsKey=TableManager.Instance.docsKey;

            string url = string.Format(baseUrl, docsKey, docsId);
            UnityWebRequest request = UnityWebRequest.Get(url);


            yield return request.SendWebRequest();

            if(request.result==UnityWebRequest.Result.ConnectionError||request.result==UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network Error");

            }
            else
            {
                if(saveData)
                {
                    //Table Directory Check
                    if(!System.IO.Directory.Exists(Application.dataPath+"/Resources/Table"))
                    {
                        System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/Table");
                    }

                    string path = string.Format("{0}/Resources/Table/{1}.csv", Application.dataPath, ((TableManager.GoogleDocsID)docsId).ToString().ToLower());
                    System.IO.File.WriteAllText(path, request.downloadHandler.text);
                    Debug.Log("Table Save:" + path);
                }
                else
                {
                    data = new TableData();
                    data.LoadRawText(request.downloadHandler.text);

                    callBack(data);
                }

            
            }

        }


    }
    public class DataForm 
    {
        protected List<IValue> valueList = new List<IValue>();

        public void AddValue(IValue value)
        {
            valueList.Add(value);
        }


        public void SetDataValues(Dictionary<string,string> row)
        {
            foreach(var value in valueList)
            {
                value.Load(row);
            }
        }
    }


    public interface IValue
    {
        string Name { get; }

        bool Load(Dictionary<string, string> row);
    }

    public class TInt: IValue
    {
        private string name;
        private int value;

        public string Name { get { return name; } }
        public int Value { get { return value; } }

        public TInt(string name,DataForm dataform)
        {
            this.name = name;
            dataform.AddValue(this);
        }

        bool IValue.Load(Dictionary<string,string> row)
        {
            string strValue;
            bool get = row.TryGetValue(this.name, out strValue);

            if(get)
            {
                try
                {
                    this.value = int.Parse(strValue);
                }
                catch
                {
                    Debug.LogError("ERROR:" + this.name + "is not integer value");
                    return false;
                }

            }
            else
            {
                Debug.LogError("ERROR:" + this.name + "is not found in column");
                return false;
            }
            return true;
        }

    }

    public class TString: IValue
    {
        private string name;
        private string value;

        public string Name { get { return this.name; } }
        public string Value { get { return this.value; } }

        public TString(string name,DataForm dataForm)
        {
            this.name = name;
            dataForm.AddValue(this);
        }

        bool IValue.Load(Dictionary<string, string> row)
        {
            string strValue;
            bool get = row.TryGetValue(this.name, out strValue);

            if (get)
            {
                try
                {
                    this.value = strValue;
                }
                catch
                {
                    Debug.LogError("ERROR:" + this.name + "is not integer value");
                    return false;
                }

            }
            else
            {
                Debug.LogError("ERROR:" + this.name + "is not found in column");
                return false;
            }
            return true;
        }

    }

    public class TFloat : IValue
    {
        private string name;
        private float value;

        public string Name { get { return this.name; } }
        public float Value { get { return this.value; } }

        public TFloat(string name,DataForm dataForm)
        {
            this.name = name;
            dataForm.AddValue(this);
        }

        bool IValue.Load(Dictionary<string, string> row)
        {
            string strValue;
            bool get = row.TryGetValue(this.name, out strValue);

            if (get)
            {
                try
                {
                    this.value = float.Parse(strValue);
                }
                catch
                {
                    Debug.LogError("ERROR:" + this.name + "is not integer value");
                    return false;
                }

            }
            else
            {
                Debug.LogError("ERROR:" + this.name + "is not found in column");
                return false;
            }
            return true;
        }

    }

    public class TDouble: IValue
    {
        private string name;
        private double value;

        public string Name { get { return this.name; } }
        public double Value { get { return this.value; } }

        public TDouble (string name,DataForm dataForm)
        {
            this.name = name;
            dataForm.AddValue(this);
        }

        bool IValue.Load(Dictionary<string, string> row)
        {
            string strValue;
            bool get = row.TryGetValue(this.name, out strValue);

            if (get)
            {
                try
                {
                    this.value = double.Parse(strValue);
                }
                catch
                {
                    Debug.LogError("ERROR:" + this.name + "is not integer value");
                    return false;
                }

            }
            else
            {
                Debug.LogError("ERROR:" + this.name + "is not found in column");
                return false;
            }
            return true;
        }
    }
    public class TBool: IValue
    {
        private string name;
        private bool value;

        public string Name { get { return this.name; } }
        public bool Value { get { return this.value; } }

        public TBool(string name, DataForm dataForm)
        {
            this.name = name;
            dataForm.AddValue(this);
        }
        bool IValue.Load(Dictionary<string, string> row)
        {
            string strValue;
            bool get = row.TryGetValue(this.name, out strValue);
            if(get)
            {
                try
                {
                    this.value = bool.Parse(strValue.ToLower()); 
                }
                catch
                {
                    Debug.LogError("ERROR:" + this.name + "is not bool value");
                    return false;
                }
            }
            else
            {
                Debug.LogError("ERROR:" + this.name + "is not found in column");
                return false;
            }

            return true;
        }
    }
    public class TDateTime: IValue
    {
        private string name;
        private DateTime value;

        public string Name { get { return this.name; } }
        public DateTime Value { get { return this.value; } }
        public TDateTime(string name,DataForm dataForm)
        {
            this.name = name;
            dataForm.AddValue(this);
        }

        bool IValue.Load(Dictionary<string, string> row)
        {
            string strValue;
            bool get = row.TryGetValue(this.name, out strValue);

            if (get)
            {
                try
                {
                    this.value = DateTime.Parse(strValue);
                }
                catch
                {
                    Debug.LogError("ERROR:" + this.name + "is not DateTime value");
                    return false;
                }

            }
            else
            {
                Debug.LogError("ERROR:" + this.name + "is not found in column");
                return false;
            }
            return true;
        }
    }


}
