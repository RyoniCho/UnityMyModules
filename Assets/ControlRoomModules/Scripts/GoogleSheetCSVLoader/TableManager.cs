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


        public const string docsKey = "1GzWqze_hxEi45B6PU5VSylnzkLeWUHk5ddh69SV0vdE";
        public enum GoogleDocsID
        {
            NONE=-1,
            LOCALIZATION = 671532158,
            CONFIG = 1393723135,
            ITEM = 0,
            ENEMY=315463931,
            PLAYER=782103566,
            TIMELINE=1346251040,
            QUEST= 1194202679,
            QUEST_OBJECTIVES=2104579158,
            DIALOGUE=795091221,

        }

        /// <summary>
        /// Table Data manager (Singleton Design) Registration.
        /// </summary>
        public LocalizationDataManager localization;
        public ItemDataManager item;
        public ConfigDataManager config;
        public EnemyDataManager enemy;
        public PlayerDataManager player;
        public TimelineDataManager timeline;
        public QuestDataManager quest;
        public DialogueDataManager dialogue;
        public QuestObjectivesDataManager questObjectives;
        
        public List<ITableBaseDataManager> listTableDataManager = new List<ITableBaseDataManager>();

        private bool isInitialzed = false;
        private bool isSetTableDataManager = false;
        private bool isTableLoadComplete = false;
        public bool IsTableLoadComplete => isTableLoadComplete;

        public void SetTableDataManager()
        {
            if (isSetTableDataManager)
                return;

            localization = new LocalizationDataManager();
            item = new ItemDataManager();
            config = new ConfigDataManager();
            enemy = new EnemyDataManager();
            player = new PlayerDataManager();
            timeline = new TimelineDataManager();
            quest = new QuestDataManager();
            dialogue = new DialogueDataManager();
            questObjectives = new QuestObjectivesDataManager();
            

            listTableDataManager.Clear();

            listTableDataManager.Add(localization);
            listTableDataManager.Add(item);
            listTableDataManager.Add(config);
            listTableDataManager.Add(enemy);
            listTableDataManager.Add(player);
            listTableDataManager.Add(timeline);
            listTableDataManager.Add(quest);
            listTableDataManager.Add(dialogue);
            listTableDataManager.Add(questObjectives);

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

            Initialize();

       }

        public void Initialize()
        {
            if (isInitialzed)
                return;
            
            RegisterTableDataForLoad();
            SetTableDataManager();

#if UNITY_EDITOR
            TableDataLoader.DataPath = UnityEngine.Application.dataPath;

#else
            TableDataLoader.DataPath = UnityEngine.Application.streamingAssetsPath;
#endif
            TableDataLoader.OnlineMode = IsOnlineLiveLoadMode;

            stopwatch = new Stopwatch();
            
            isInitialzed = true;
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
            
            if (stopwatch == null)
                stopwatch = new Stopwatch();
            
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
