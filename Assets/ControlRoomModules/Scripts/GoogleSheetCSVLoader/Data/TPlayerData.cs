using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ControlRoom;

public class TPlayerData : DataForm
{
   public TableType<int> level;
   public TableType<int> hp;
   public TableType<int> attack;
   public TableType<int> defence;
   public TableType<float> speed;
   public TableType<float> jumpPower;
   public TableType<float> dashDistance;

   public TPlayerData()
   {
      this.level = new TableType<int>("level", this);
      this.hp = new TableType<int>("hp", this);
      this.attack = new TableType<int>("attack", this);
      this.defence = new TableType<int>("defence", this);
      this.speed = new TableType<float>("speed", this);
      this.jumpPower = new TableType<float>("jumpPower", this);
      this.dashDistance = new TableType<float>("dashDistance", this);
   }
}

public class PlayerData : ITableData, IKeyProvider<int>
{
   public int level;
   public int hp;
   public int attack;
   public int defence;
   public float speed;
   public float jumpPower;
   public float dashDistance;

   public void SetValue(DataForm dataForm)
   {
      if (dataForm is TPlayerData tData)
      {
         this.level = tData.level.Value;
         this.hp = tData.hp.Value;
         this.attack = tData.attack.Value;
         this.defence = tData.defence.Value;
         this.speed = tData.speed.Value;
         this.jumpPower = tData.jumpPower.Value;
         this.dashDistance = tData.dashDistance.Value;
      }
   }

   public int GetKey()
   {
      return this.level;
   }
}

public class PlayerDataManager : TableBaseDataManager<PlayerData,TPlayerData,int>
{
   protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.PLAYER;
   public PlayerData GetPlayerData(int level)
   {
      if (dicDatas.TryGetValue(level, out var playerData))
         return playerData;

      return null;
}
  
}
