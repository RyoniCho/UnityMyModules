using System;
using UnityEngine;
using ControlRoom;

public class TQuestObjectivesData : DataForm
{
    public TableType<string> Id;
    public TableType<string> ObjectiveType;
    public TableType<int> TargetId;
    public TableType<int> TargetCount;
    public TableType<string> Description;
    

    public TQuestObjectivesData()
    {
        this.Id = new TableType<string>("id", this);
        this.ObjectiveType = new TableType<string>("objectiveType", this);
        this.TargetId = new TableType<int>("targetId", this);
        this.TargetCount = new TableType<int>("targetCount", this);
        this.Description = new TableType<string>("Description", this);
    }
}

public class QuestObjectivesData : ITableData, IKeyProvider<string>
{
    public enum ObjectiveType
    {
        TalkToNPC,
        CollectItem,
        KillMonster,
        ReachLocation
    }
    
    public string objectiveId;
    public ObjectiveType objectiveType;
    public int targetId;
    public int targetCount;
    public string description;

    public void SetValue(DataForm dataform)
    {
        var tData = dataform as TQuestObjectivesData;
        if (tData != null)
        {
            this.objectiveId = tData.Id.Value;
            this.objectiveType = Enum.Parse<ObjectiveType>(tData.ObjectiveType.Value);
            this.targetId = tData.TargetId.Value;
            this.targetCount = tData.TargetCount.Value;
            this.description = tData.Description.Value;
        }
    }

    public string GetKey()
    {
        return this.objectiveId;
    }
}


public class QuestObjectivesDataManager : TableBaseDataManager<QuestObjectivesData,TQuestObjectivesData,string>
{
    protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.QUEST_OBJECTIVES;

    public QuestObjectivesData GetQuestObjectivesData(string objectiveId)
    {
        if (dicDatas.TryGetValue(objectiveId, out var data))
        {
            return data;
        }
        
        return null;
    }
}


