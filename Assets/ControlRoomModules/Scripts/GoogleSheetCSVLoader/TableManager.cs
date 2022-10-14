using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;

namespace ControlRoom
{
    public class TableManager : SingletonBase<TableManager>
    {

        [Header("Load Online GoogleSheet(CSV Only.Binary Load not supported)")]
        public bool IsOnlineLiveLoadMode = true;


        /// <summary>
        /// Google Docs ID
        ///
        /// When you check google spread sheet url,
        /// ex: https://docs.google.com/spreadsheets/d/d3j3kdi/edit#gid=1391482172
        /// 
        /// "d3j3kdi" is google docs key.
        /// 1391482172 is sheet id. you write your sheet id below.
        /// 
        /// </summary>


        public const string docsKey = "1j_1b-SU5f_llS-yaANJk-hR2kH8T0le6DdkqpfRNoRg";
        public enum GoogleDocsID
        {
            NONE=-1,
            LOCALIZATION = 1724096133,
            ITEM = 0,

        }

        /// <summary>
        /// Table Data manager (Singleton Design) Registration.
        /// </summary>
        public LocalizationDataManager localization;
        public ItemDataManager item;
        
        public List<TableBaseDataManager> listTableDataManager = new List<TableBaseDataManager>();
       
        private bool isSetTableDataManager = false;
        private bool isTableLoadComplete = false;
        public bool IsTableLoadComplete => isTableLoadComplete;

        public void SetTableDataManager()
        {
            if (isSetTableDataManager)
                return;

            localization = new LocalizationDataManager();
            item = new ItemDataManager();

            listTableDataManager.Clear();

            listTableDataManager.Add(localization);
            listTableDataManager.Add(item);

            isSetTableDataManager = true;

        }


        private HashSet<GoogleDocsID> hsTotalTable = new HashSet<GoogleDocsID>();
        private HashSet<GoogleDocsID> hsLoadCompleteTable = new HashSet<GoogleDocsID>();
        
        private Stopwatch stopwatch;

        
        protected override void Awake() 
        {
           
           base.Awake();
           if(Instance!=this)
			{
				Destroy(this.gameObject);
				return;
			}

			DontDestroyOnLoad(this.gameObject);

            RegisterTableDataForLoad();
            SetTableDataManager();

#if UNITY_EDITOR
            TableDataLoader.DataPath = UnityEngine.Application.dataPath;

#else
            TableDataLoader.DataPath = UnityEngine.Application.streamingAssetsPath;
#endif
            TableDataLoader.OnlineMode = IsOnlineLiveLoadMode;

            stopwatch = new Stopwatch();

       }

        public void LoadTableCSV()
        {
            LoadAllTable();
        }

        public void LoadTableBinary()
        {
            LoadAllTable(true);
        }

        async void LoadAllTable(bool binaryLoad = false)
        {
            isTableLoadComplete = false;

            stopwatch.Start();

            if (binaryLoad)
            {

                foreach (var manager in listTableDataManager)
                {
                    await manager.LoadBinaryData();
                }

            }
            else
            {
                foreach (var manager in listTableDataManager)
                {
                    await manager.LoadData();
                }


            }

            stopwatch.Stop();

            isTableLoadComplete = true;

            UnityEngine.Debug.Log($"Table Load ElapsTime:{stopwatch.ElapsedMilliseconds} / BinaryLoad: {binaryLoad}");

        }

        public float TableLoadProgressValue
        {
            get
            {
                if (hsTotalTable.Count == 0)
                    return 0f;


                return ((float)hsLoadCompleteTable.Count / (float)hsTotalTable.Count)*100;
            }
        }


        private void RegisterTableDataForLoad()
        {
           foreach (GoogleDocsID id in System.Enum.GetValues(typeof(GoogleDocsID)))
            {
                if(id != GoogleDocsID.NONE)
                    hsTotalTable.Add(id);
            }

        }

        public void LoadCompleteTableData(GoogleDocsID id)
        {
            hsLoadCompleteTable.Add(id);
        }

        public async Task BuildBinaryDataAll(System.Action onCompleteTablebuild)
        {
            SetTableDataManager();
            
            for (int i= 0; i< listTableDataManager.Count;i++)
            {
              
                await listTableDataManager[i].BuildBinaryData();
                onCompleteTablebuild();


            }

        }


    }

}
