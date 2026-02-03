using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlRoom;
using System.IO;

public class TEnemyData : DataForm
{
    public TableType<int> index;
    public TableType<string> name;
    public TableType<string> attackerType;
    public TableType<int> hp;
    public TableType<int> attack;
    public TableType<int> defence;
    public TableType<float> stiffValue;
    public TableType<float> hitStiffTime;
    public TableType<float> walkSpeed;
    public TableType<float> runSpeed;
    public TableType<float> distanceToAbleToDash;
    public TableType<float> dashAttackSpeed;
    public TableType<float> jumpPower;
    public TableType<float> knockbackForce;
    public TableType<float> knockbackTime;
    public TableType<int> moneyValue;
    public TableType<int> moneyObjectCount;
  
    

    public TEnemyData()
    {
        index = new TableType<int>("index", this);
        name = new TableType<string>("name", this);
        attackerType = new TableType<string>("attackerType", this);
        hp = new TableType<int>("hp", this);
        attack = new TableType<int>("attack", this);
        defence = new TableType<int>("defence", this);
        stiffValue = new TableType<float>("stiffValue", this);
        hitStiffTime = new TableType<float>("hitStiffTime", this);
        walkSpeed = new TableType<float>("walkSpeed", this);
        runSpeed = new TableType<float>("runSpeed", this);
        jumpPower = new TableType<float>("jumpPower", this);
        dashAttackSpeed = new TableType<float>("dashAttackSpeed", this);
        distanceToAbleToDash = new TableType<float>("distanceToBeAbleToDash", this);
        knockbackForce = new TableType<float>("knockbackForce", this);
        knockbackTime = new TableType<float>("knockbackTime", this);
        moneyValue = new TableType<int>("moneyValue", this);
        moneyObjectCount = new TableType<int>("moneyObjectCount", this);

    }
}

public class EnemyData : IKeyProvider<int>, ITableData
{
    public int index;
    public string name;
    public string attackerType;
    public int hp;
    public int attack;
    public int defence;
    public float stiffValue;
    public float hitStiffTime;
    public float walkSpeed;
    public float runSpeed;
    public float distanceToAbleToDash;
    public float dashAttackSpeed;
    public float jumpPower;
    public float knockbackForce;
    public float knockbackTime;
    public int moneyValue;
    public int moneyObjectCount;
    

    public void SetValue(DataForm dataForm)
    {
        var data = dataForm as TEnemyData;
        
        if (data != null)
        {
            index = data.index.Value;
            name = data.name.Value;
            attackerType = data.attackerType.Value;
            hp = data.hp.Value;
            attack = data.attack.Value;
            defence = data.defence.Value;
            stiffValue = data.stiffValue.Value;
            hitStiffTime = data.hitStiffTime.Value;
        
            walkSpeed = data.walkSpeed.Value;
            runSpeed = data.runSpeed.Value;
            distanceToAbleToDash = data.distanceToAbleToDash.Value;
            dashAttackSpeed = data.dashAttackSpeed.Value;
            jumpPower = data.jumpPower.Value;
            knockbackForce = data.knockbackForce.Value;
            knockbackTime = data.knockbackTime.Value;
            moneyValue = data.moneyValue.Value;
            moneyObjectCount = data.moneyObjectCount.Value;
        }
        
    }

    public int GetKey()
    {
        return index;
    }
}

public class EnemyDataManager: TableBaseDataManager<EnemyData,TEnemyData,int>
{
   
    protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.ENEMY;
    public EnemyData GetEnemyData(int index)
    {
        if (dicDatas.TryGetValue(index, out var enemyData))
            return enemyData;

        return null;
    }

}
