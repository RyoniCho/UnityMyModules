using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace ControlRoom
{
    public class TableManager : SingletonBase<TableManager>
    {
        public bool IsOnlineLiveLoadMode=false;
        public string docsKey = "=======Google Docs Key Here========";

        private HashSet<GoogleDocsID> hsTotalTable = new HashSet<GoogleDocsID>();
        private HashSet<GoogleDocsID> hsLoadCompleteTable = new HashSet<GoogleDocsID>();
        private Stopwatch stopwatch;

        public enum GoogleDocsID
        {
            LOCALIZATION= 1724096133,
            ITEM =0,
                
        }

       protected override void Awake() 
       {
           
           base.Awake();
           if(Instance!=this)
			{
				Destroy(this.gameObject);
				return;
			}

			DontDestroyOnLoad(this.gameObject);

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
                LocalizationDataManager.Instance.LoadBinaryData();
                ItemDataManager.Instance.LoadBinaryData();
            }
            else
            {
                LocalizationDataManager.Instance.LoadData();
                ItemDataManager.Instance.LoadData();
            }
            
          
            

            while (true)
            {
                if (LoadComplete)
                {
                    //currentGameState = GAMESTATE.TABLELOAD_COMPLETE;
                    stopwatch.Stop();
                    UnityEngine.Debug.Log($"Table Load ElapsTime:{stopwatch.ElapsedMilliseconds} / BinaryLoad: {binaryLoad}");
                    //var d = LocalizationDataManager.Instance.GetLocalizationData(10001);
                    //UnityEngine.Debug.Log($"Local 1001: {d}");
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

    }

}
