using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ControlRoom
{
    // interface IDataManager
    // {
    //     void RegisterDataManager();
    // }

    // public class DataManager : SingletonBase<DataManager>,IDataManager
    // {
    //     void IDataManager.RegisterDataManager()
    //     {
            
    //     }
    // }

    // public class exp :DataManager
    // {

    // }

    public class TableData 
    {
       
        public Dictionary<int, Dictionary<string, string>> dicTableData = new Dictionary<int, Dictionary<string, string>>();

        public void LoadRawText(string rawData)
        {
        

            string[] data = rawData.Split('\n');
            string[] names = data[0].Split(',');
            string[] types = data[1].Split(',');

            char[] trimChar = "\r\"".ToCharArray();
            for(int i=0;i<names.Length;i++)
            {
                names[i] = names[i].Trim(trimChar);
            }

            for(int i=2;i<data.Length;++i)
            {
                string[] values = data[i].Split(',');
                if(names.Length!=values.Length)
                {
                    Debug.LogError("Table Name length is not matching for Data Length");
                    continue;
                }

                if(values.Length==1)
                {
                    //Blank value
                    if (values[0].Trim(trimChar) == "")
                        continue;
                }

                try
                {
                    Dictionary<string, string> column = new Dictionary<string, string>();

                    dicTableData.Add(dicTableData.Count, column);

                    for(int j=0;j<names.Length;++j)
                    {
                        if (column.ContainsKey(names[j]))
                        {
                            Debug.LogError("["+j+"/"+names.Length+"]"+names[j]+"is Already Exist");
                            continue;
                        }
                        string value = values[j].Trim(trimChar);

                        //Localization 줄바꿈 예외처리
                        value = value.Replace("\\n", "\n");

                        column.Add(names[j], value);
                    }
                
                    
                }
                catch(System.Exception e)
                {
                    Debug.LogError(e);
                }
            }


        }

    }

}
