using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ControlRoom;

public class TItemData : DataForm
{

    public TableType<int> itemIndex;
    public TableType<string> itemName;
    public TableType<string> itemType;
    public TableType<int> itemValue;
    public TableType<int> grade;
    public TableType<string> desc;

    public TItemData()
    {
        itemIndex = new TableType<int>("itemIndex", this);
        itemName = new TableType<string>("itemName", this);
        itemType = new TableType<string>("itemType", this);
        itemValue = new TableType<int>("itemValue", this);
        grade = new TableType<int>("grade", this);
        desc = new TableType<string>("desc", this);
    }
}

public class ItemData : ITableData ,IKeyProvider<int>
{
    public int itemIndex;
    public string itemName;
    public string itemType;
    public int itemValue;
    public int itemGrade;
    public string itemDesc;

    public void SetValue(DataForm dataform)
    {
        TItemData tData = dataform as TItemData;

        if (tData != null)
        {
            itemIndex = tData.itemIndex.Value;
            itemName = tData.itemName.Value;
            itemValue = tData.itemValue.Value;
            itemDesc = tData.desc.Value;
            itemGrade = tData.grade.Value;
        }
    }

    public int GetKey()
    {
        return itemIndex;
    }
 
}


public class ItemDataManager: TableBaseDataManager<ItemData,TItemData,int>
{
  
    protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.ITEM;
    public ItemData GetItemData(int itemIndex)
    {
        if (dicDatas.TryGetValue(itemIndex, out var itemData))
            return itemData;

        return null;
    }

}