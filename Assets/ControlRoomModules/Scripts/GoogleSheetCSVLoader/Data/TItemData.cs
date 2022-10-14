using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlRoom;
using System.IO;

public class TItemData : DataForm
{

    public TInt itemIndex;
    public TString itemName;
    public TString itemType;
    public TInt grade;
    public TString desc;

    

    public TItemData()
    {
        itemIndex = new TInt("itemIndex", this);
        itemName = new TString("itemName", this);
        itemType = new TString("itemType", this);
        grade = new TInt("grade", this);
        desc = new TString("desc", this);
    }
}

public class ItemData
{
    public int itemIndex;
    public string itemName;

    public ItemData(TItemData data)
    {
        itemIndex = data.itemIndex.Value;
        itemName = data.itemName.Value;
        
    }
}


public class ItemDataManager: TableBaseDataManager
{
    Dictionary<int, ItemData> dicItemData = new Dictionary<int, ItemData>();
    protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.ITEM;
    

    protected override void AfterLoadComplete() {}


    protected override void SetTableData(Dictionary<string, string> tableData)
    {

        TItemData tItemData = new TItemData();
        tItemData.SetDataValues(tableData);


        ItemData itemData = new ItemData(tItemData);
        dicItemData.Add(itemData.itemIndex, itemData);
    }
    protected override void SetBinaryTableData(System.IO.BinaryReader reader)
    {
        TItemData tItemData = new TItemData();
        tItemData.ReadBinary(reader);

        ItemData itemData = new ItemData(tItemData);
        dicItemData.Add(itemData.itemIndex, itemData);
    }
    protected override void ReadAndWriteBinaryTableData(Dictionary<string, string> tableData, BinaryWriter writer)
    {
        TItemData tItemData = new TItemData();
        tItemData.SetDataValues(tableData);
        tItemData.WriteBinary(writer);
    }

    public ItemData GetItemData(int index)
    {
        if (dicItemData.ContainsKey(index))
        {
            return dicItemData[index];
        }

        UnityEngine.Debug.LogError($"{index} is not contain itemdata");
        return null;
    }



}