using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
namespace ControlRoom
{
    public interface ITableBaseDataManager
    {
        public Task LoadData();
        public Task LoadBinaryData();
        public Task BuildBinaryData();
    }
    
    public class TableBaseDataManager<TBaseClass,TTableClass,TDicKeyType> : ITableBaseDataManager
        where TTableClass: DataForm, new() 
        where TBaseClass : ITableData, IKeyProvider<TDicKeyType>, new() 
    {
        protected virtual TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.NONE;
        protected Dictionary<TDicKeyType, TBaseClass> dicDatas = new Dictionary<TDicKeyType, TBaseClass>();


        public async Task BuildBinaryData()
        {
            if (currentTableId == TableManager.GoogleDocsID.NONE)
            {
                UnityEngine.Debug.LogError("Google Docs ID is None.");
                return;

            }

            await TableDataBuilder.DownloadCSVAndCreateBinaryFile((int)currentTableId, ConvertAndWriteBinaryData);
        }

        public async Task LoadBinaryData()
        {
          
            if (currentTableId == TableManager.GoogleDocsID.NONE)
            {
                UnityEngine.Debug.LogError("Google Docs ID is None.");
                return;

            }

            await TableDataLoader.LoadData((int)currentTableId,
                (System.IO.BinaryReader reader) => { ConvertBinaryData(reader); });

            TableManager.Instance.LoadCompleteTableData(currentTableId);
            AfterLoadComplete();
           

        }

        public async Task LoadData()
        {
            if (currentTableId == TableManager.GoogleDocsID.NONE)
            {
                UnityEngine.Debug.LogError("Google Docs ID is None.");
                return;

            }

            await TableDataLoader.LoadData((int)currentTableId, (TableData data) =>
            {
                ConvertTableData(data);
               
            });
            
            TableManager.Instance.LoadCompleteTableData(currentTableId);
            AfterLoadComplete();
        }

        private void ConvertTableData(TableData data)
        {
            foreach (var tableData in data.dicTableData)
            {
                SetTableData(tableData.Value);
            }
        }

        private void ConvertBinaryData(System.IO.BinaryReader reader)
        {
            SetBinaryTableData(reader);
        }

        void ConvertAndWriteBinaryData(TableData data, System.IO.BinaryWriter writer)
        {
            foreach (var tableData in data.dicTableData)
            {

                ReadAndWriteBinaryTableData(tableData.Value, writer);

            }
        }

        protected virtual void SetTableData(Dictionary<string, string> tableData)
        {
            TTableClass tData = new TTableClass();
            tData.SetDataValues(tableData);

            TBaseClass baseData = new TBaseClass();
            baseData.SetValue(tData);
            
            TDicKeyType key = baseData.GetKey();
            if (!dicDatas.ContainsKey(key))
            {
                dicDatas.Add(key, baseData);
            }
            else
            {
                Debug.LogWarning($"Duplicate key detected: {key}");
            }
        }


        protected virtual void SetBinaryTableData(System.IO.BinaryReader reader)
        {
            TTableClass tData = new TTableClass();
            tData.ReadBinary(reader);

            TBaseClass baseData = new TBaseClass();
            baseData.SetValue(tData);

            TDicKeyType key = baseData.GetKey();
            if (!dicDatas.ContainsKey(key))
            {
                dicDatas.Add(key, baseData);
            }
            else
            {
                Debug.LogWarning($"Duplicate key detected: {key}");
            }

        }
        protected virtual void ReadAndWriteBinaryTableData(Dictionary<string, string> tableData, System.IO.BinaryWriter writer)
        {
            TTableClass tData = new TTableClass();
            tData.SetDataValues(tableData);
            tData.WriteBinary(writer);
        }
        protected virtual void AfterLoadComplete() { }

    }

}
