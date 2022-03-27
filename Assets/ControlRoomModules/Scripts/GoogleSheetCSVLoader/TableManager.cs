using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace ControlRoom
{
    public class TableManager : SingletonBase<TableManager>
    {


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


        public const string docsKey = "==Google Docs Key Here==";
        public enum GoogleDocsID
        {

            LOCALIZATION = 1724096133,
            ITEM = 0,

        }

        /// <summary>
        /// Table Data manager (Singleton Design) Registration.
        /// </summary>
        public List<ITableDataManager> listTableDataManager = new List<ITableDataManager>();
        private bool isSetTableDataManager = false;

        public void SetTableDataManager()
        {
            if (isSetTableDataManager)
                return;

            listTableDataManager.Add(LocalizationDataManager.Instance);

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

            SetTableDataManager();

            stopwatch = new Stopwatch();

       }

        public void LoadTableCSV()
        {
            StartCoroutine(LoadAllTable());
        }

        public void LoadTableBinary()
        {
            StartCoroutine(LoadAllTable(true));
        }


       IEnumerator LoadAllTable(bool binaryLoad=false)
       {
            stopwatch.Start();
            RegisterTableDataForLoad();

            if(binaryLoad)
            {

                foreach(var manager in listTableDataManager)
                {
                    manager.LoadBinaryData();
                }
               
            }
            else
            {
                foreach(var manager in listTableDataManager)
                {
                    manager.LoadData();
                }
               
             
            }
            

            while (true)
            {
                if (LoadComplete)
                {
                   
                    stopwatch.Stop();
                    UnityEngine.Debug.Log($"Table Load ElapsTime:{stopwatch.ElapsedMilliseconds} / BinaryLoad: {binaryLoad}");
                   
                    yield break;
                }

                yield return null;
            }

         
       }

        public bool LoadComplete
        {
            get
            {
                return (TableLoadProgressValue >= 1f);
            }
        }

        public float TableLoadProgressValue
        {
            get
            {
                if (hsTotalTable.Count == 0)
                    return 0f;


                return ((float)hsLoadCompleteTable.Count / (float)hsTotalTable.Count);
            }
        }


        public void RegisterTableDataForLoad()
        {
            foreach (GoogleDocsID id in System.Enum.GetValues(typeof(GoogleDocsID)))
            {
                hsTotalTable.Add(id);
            }

        }

        public void LoadCompleteTableData(GoogleDocsID id)
        {
            hsLoadCompleteTable.Add(id);
        }

        public void BuildBinaryDataAll()
        {
            SetTableDataManager();

            if (stopwatch == null)
                stopwatch = new Stopwatch();


            UnityEngine.Debug.Log("Build Binary Data Start");

            stopwatch.Start();
            
            for (int i= 0; i< listTableDataManager.Count;i++)
            {
              
                listTableDataManager[i].BuildBinaryData();
               
            }

            stopwatch.Stop();
           
            UnityEngine.Debug.Log($"Build Binary Data End : {stopwatch.ElapsedMilliseconds}");


        }


    }

}
