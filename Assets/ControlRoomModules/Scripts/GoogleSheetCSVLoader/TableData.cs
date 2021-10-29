using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

        public void LoadRawCSVText(string rawData)
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

    public class DataForm
    {
        protected List<IValue> valueList = new List<IValue>();

        public void AddValue(IValue value)
        {
            valueList.Add(value);
        }


        public void SetDataValues(Dictionary<string, string> row)
        {
            foreach (var value in valueList)
            {
                value.Load(row);
            }
        }

        public void WriteBinary(System.IO.BinaryWriter writer)
        {
            foreach (var value in valueList)
            {
                value.Write(writer);
            }
        }

        public void ReadBinary(System.IO.BinaryReader reader)
        {
            foreach (var value in valueList)
            {
                value.ReadBinary(reader);
            }
        }
    }


    public interface IValue
    {
        string Name { get; }

        bool Load(Dictionary<string, string> row);
        bool Write(System.IO.BinaryWriter writer);
        bool ReadBinary(System.IO.BinaryReader reader);
    }

    public class TInt : IValue
    {
        private string name;
        private int value;

        public string Name { get { return name; } }
        public int Value { get { return value; } }

        public TInt(string name, DataForm dataform)
        {
            this.name = name;
            dataform.AddValue(this);
        }

        bool IValue.Load(Dictionary<string, string> row)
        {
            string strValue;
            bool get = row.TryGetValue(this.name, out strValue);

            if (get)
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

        bool IValue.Write(System.IO.BinaryWriter writer)
        {
            try
            {
                writer.Write(this.value);
            }
            catch
            {
                Debug.LogError("ERROR:" + this.name + "Could not write(Binary)");
                return false;
            }

            return true;

        }

        bool IValue.ReadBinary(System.IO.BinaryReader reader)
        {
            try
            {
                this.value = reader.ReadInt32();
            }
            catch
            {
                return false;
            }

            return true;

        }

    }

    public class TString : IValue
    {
        private string name;
        private string value;

        public string Name { get { return this.name; } }
        public string Value { get { return this.value; } }

        public TString(string name, DataForm dataForm)
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

        bool IValue.Write(System.IO.BinaryWriter writer)
        {
            try
            {
                writer.Write(this.value);
            }
            catch
            {
                Debug.LogError("ERROR:" + this.name + "Could not write(Binary)");
                return false;
            }

            return true;
        }

        bool IValue.ReadBinary(System.IO.BinaryReader reader)
        {
            try
            {
                this.value = reader.ReadString();
            }
            catch
            {
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

        public TFloat(string name, DataForm dataForm)
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

        bool IValue.Write(System.IO.BinaryWriter writer)
        {
            try
            {
                writer.Write(this.value);
            }
            catch
            {
                Debug.LogError("ERROR:" + this.name + "Could not write(Binary)");
                return false;
            }

            return true;
        }

        bool IValue.ReadBinary(System.IO.BinaryReader reader)
        {
            try
            {
                this.value = reader.ReadSingle();
            }
            catch
            {
                return false;
            }

            return true;

        }

    }

    public class TDouble : IValue
    {
        private string name;
        private double value;

        public string Name { get { return this.name; } }
        public double Value { get { return this.value; } }

        public TDouble(string name, DataForm dataForm)
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

        bool IValue.Write(System.IO.BinaryWriter writer)
        {
            try
            {
                writer.Write(this.value);
            }
            catch
            {
                Debug.LogError("ERROR:" + this.name + "Could not write(Binary)");
                return false;
            }

            return true;
        }

        bool IValue.ReadBinary(System.IO.BinaryReader reader)
        {
            try
            {
                this.value = reader.ReadDouble();
            }
            catch
            {
                return false;
            }

            return true;

        }
    }
    public class TBool : IValue
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
            if (get)
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

        bool IValue.Write(System.IO.BinaryWriter writer)
        {
            try
            {
                writer.Write(this.value);
            }
            catch
            {
                Debug.LogError("ERROR:" + this.name + "Could not write(Binary)");
                return false;
            }

            return true;
        }

        bool IValue.ReadBinary(System.IO.BinaryReader reader)
        {
            try
            {
                this.value = reader.ReadBoolean();
            }
            catch
            {
                return false;
            }

            return true;

        }
    }
    public class TDateTime : IValue
    {
        private string name;
        private DateTime value;

        public string Name { get { return this.name; } }
        public DateTime Value { get { return this.value; } }
        public TDateTime(string name, DataForm dataForm)
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

        bool IValue.Write(System.IO.BinaryWriter writer)
        {
            try
            {
                writer.Write(this.value.ToString());
            }
            catch
            {
                Debug.LogError("ERROR:" + this.name + "Could not write(Binary)");
                return false;
            }

            return true;
        }

        bool IValue.ReadBinary(System.IO.BinaryReader reader)
        {
            try
            {
                this.value = DateTime.Parse(reader.ReadString());
            }
            catch
            {
                return false;
            }

            return true;

        }
    }

}
