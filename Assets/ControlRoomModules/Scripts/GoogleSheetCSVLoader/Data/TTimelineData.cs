using ControlRoom;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TTimelineData : DataForm
{
    public TableType<string> timelineId;
    public TableType<string> assetPath;
    public TableType<string> description;

    public TTimelineData()
    {
        this.timelineId = new TableType<string>("timelineId", this);
        this.assetPath = new TableType<string>("assetPath", this);
        this.description = new TableType<string>("description", this);
    }
    
}

public class TimelineData : ITableData, IKeyProvider<string>
{
    public string TimelineID;
    public string AssetPath;
    public string Description;

   
    public void SetValue(DataForm dataform)
    {
        var tData = dataform as TTimelineData;
        if (tData != null)
        {
            this.TimelineID = tData.timelineId.Value;
            this.AssetPath = tData.assetPath.Value;
            this.Description = tData.description.Value;
        }
    }
    public string GetKey()
    {
        return this.TimelineID;
    }
}

public class TimelineDataManager : TableBaseDataManager<TimelineData,TTimelineData,string>
{
    protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.TIMELINE;
   
    public TimelineData GetTimelineData(string timelineID)
    {
        if (dicDatas.TryGetValue(timelineID, out var timelineData))
            return timelineData;

        return null;
    }
}
