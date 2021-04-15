using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ControlRoom
{
    public class TableManager : SingletonBase<TableManager>
    {
        public bool IsOnlineLiveLoadMode=false;
        public string docsKey = "=======Google Docs Key Here========";

        private HashSet<GoogleDocsID> hsTotalTable = new HashSet<GoogleDocsID>();
        private HashSet<GoogleDocsID> hsLoadCompleteTable = new HashSet<GoogleDocsID>();
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

       }

       IEnumerator LoadAllTable()
       {
           RegisterTableDataForLoad();

            LocalizationDataManager.Instance.LoadData();
            ItemDataManager.Instance.LoadData();
          
            

            while (true)
            {
                if (LoadComplete)
                {
                    //currentGameState = GAMESTATE.TABLELOAD_COMPLETE; 
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
