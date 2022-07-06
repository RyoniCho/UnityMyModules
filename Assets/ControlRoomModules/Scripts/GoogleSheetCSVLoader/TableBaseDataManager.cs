using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
namespace ControlRoom
{
    public class TableBaseDataManager 
    {
        protected virtual TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.NONE;


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
            if (currentTableId== TableManager.GoogleDocsID.NONE)
            {
                UnityEngine.Debug.LogError("Google Docs ID is None.");
                return;

            }
            await TableDataLoader.LoadData((int)currentTableId, (System.IO.BinaryReader reader) =>
            {
                ConverBinaryData(reader);
                TableManager.Instance.LoadCompleteTableData(currentTableId);
                AfterLoadComplete();
            });

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

                TableManager.Instance.LoadCompleteTableData(currentTableId);
                AfterLoadComplete();
            });
        }

        private void ConvertTableData(TableData data)
        {
            foreach (var tableData in data.dicTableData)
            {
               
                DataForm dataform = new DataForm();
                dataform.SetDataValues(tableData.Value);

                SetTableData(dataform);

            }

        }

        private void ConverBinaryData(System.IO.BinaryReader reader)
        {
            DataForm data = new DataForm();
            data.ReadBinary(reader);

            SetTableData(data);

        }

        void ConvertAndWriteBinaryData(TableData data, System.IO.BinaryWriter writer)
        {
            foreach (var tableData in data.dicTableData)
            {
               
                DataForm dataform = new DataForm();
                dataform.SetDataValues(tableData.Value);

                dataform.WriteBinary(writer);

            }
        }
               
        protected virtual void SetTableData(DataForm data) { }
        protected virtual void AfterLoadComplete() { }
        
    }

}
