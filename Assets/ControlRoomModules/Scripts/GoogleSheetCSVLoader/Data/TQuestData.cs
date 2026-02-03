using System;
using ControlRoom;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class TQuestData : DataForm
{
   public TableType<string> QuestID;
   public TableType<string> QuestType;
   public TableType<string> Name;
   public TableType<string> Description;

   public TableType<string> Prerequisites;   // 선행 퀘스트 ID
   public TableType<string> NextQuests;       // 다음 퀘스트 ID
   public TableType<string> Objectives; // 목표 리스트
   public TableType<string> StartTimelineID;
   public TableType<string> CompleteTimelineID;

   
   public TQuestData()
   {
      this.QuestID = new TableType<string>("QuestID", this);
      this.QuestType = new TableType<string>("QuestType", this);
      this.Name = new TableType<string>("Name", this);
      this.Description = new TableType<string>("Description", this);
      
      this.Prerequisites = new TableType<string>("Prerequisites", this, true);
      this.NextQuests = new TableType<string>("NextQuests", this, true);
      this.Objectives = new TableType<string>("Objectives", this, true);
      this.StartTimelineID = new TableType<string>("StartTimelineID", this);
      this.CompleteTimelineID = new TableType<string>("CompleteTimelineID", this);
   }
}

public class QuestData : ITableData, IKeyProvider<string>
{
   public enum QuestType
   {
      Main,
      Sub
   }
   
   public string QuestID;
   public string Name;
   public string Description;
   public QuestType Type;
   

   public List<string> Prerequisites;   // 선행 퀘스트 ID
   public List<string> NextQuests;       // 다음 퀘스트 ID
   public List<string> Objectives; // 목표 리스트
   public string StartTimelineID;
   public string CompleteTimelineID;
   public bool isComplete = false;

   
   public void SetValue(DataForm dataForm)
   {
      var tData = dataForm as TQuestData;
      if (tData != null)
      {
         this.QuestID = tData.QuestID.Value;
         this.Name = tData.Name.Value;
         this.Type = Enum.Parse<QuestType>(tData.QuestType.Value);
         this.Description = tData.Description.Value;
         this.Prerequisites = tData.Prerequisites.ListValue;
         this.NextQuests = tData.NextQuests.ListValue;
         this.Objectives = tData.Objectives.ListValue;
         this.StartTimelineID = tData.StartTimelineID.Value;
         this.CompleteTimelineID = tData.CompleteTimelineID.Value;
      }
      
   }

   public string GetKey()
   {
      return this.QuestID;
   }
}

public class QuestDataManager : TableBaseDataManager<QuestData,TQuestData,string>
{
  
   protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.QUEST;
  
   public QuestData GetQuestData(string questID)
   {
      if (dicDatas.TryGetValue(questID, out var questData))
         return questData;

      return null;
   }

   public List<QuestData> GetAllQuestData => dicDatas.Values.ToList();
}


