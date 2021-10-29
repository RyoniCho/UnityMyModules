using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlRoom;

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


public class ItemDataManager:SingletonBase<ItemDataManager>
{
    Dictionary<int, ItemData> dicItemData = new Dictionary<int, ItemData>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadData()
    {
        TableDataLoader.LoadData((int)TableManager.GoogleDocsID.ITEM, ConvertTableData);

        TableManager.Instance.LoadCompleteTableData(TableManager.GoogleDocsID.ITEM);
    }

    public void LoadBinaryData()
    {
        TableDataLoader.LoadData((int)TableManager.GoogleDocsID.ITEM, ConverBinaryData);

        TableManager.Instance.LoadCompleteTableData(TableManager.GoogleDocsID.ITEM);
    }

    void ConvertTableData(TableData data)
    {
        foreach (var tableData in data.dicTableData)
        {
            TItemData tItemdata = new TItemData();

            tItemdata.SetDataValues(tableData.Value);
            SetItemData(tItemdata.itemIndex.Value, new ItemData(tItemdata));
        }

    }

    void ConverBinaryData(System.IO.BinaryReader reader)
    {
        TItemData tItemData = new TItemData();
        tItemData.ReadBinary(reader);

        SetItemData(tItemData.itemIndex.Value, new ItemData(tItemData));

    }

    void ConvertAndWriteBinaryData(TableData data,System.IO.BinaryWriter writer)
    {
        foreach (var tableData in data.dicTableData)
        {
            TItemData tItemdata = new TItemData();

            tItemdata.SetDataValues(tableData.Value);

            tItemdata.WriteBinary(writer);
        }
    }

    void SetItemData(int itemId, ItemData itemdata)
    {
        dicItemData[itemId] = itemdata;
    }

    public void BuildBinaryData()
    {
        TableDataBuilder.DownloadCSVAndCreateBinaryFile((int)TableManager.GoogleDocsID.ITEM, ConvertAndWriteBinaryData);
    }

}