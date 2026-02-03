using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlRoom;
using System.IO;

public class TConfigData : DataForm
{
	
	public TableType<string> key;
	public TableType<string> valueType;
	public TableType<string> value;
	
	public TConfigData()
	{
		key = new TableType<string>("key", this);
		valueType = new TableType<string>("valueType", this);
		value = new TableType<string>("value", this);
	}
}

public class ConfigData : ITableData, IKeyProvider<string>
{
	public string configKey;
	public string configValue;
	public string configValueType;


	public void SetValue(DataForm dataform)
	{
		var data = dataform as TConfigData;
		if (data != null)
		{
			configKey = data.key.Value;
			configValue = data.value.Value;
			configValueType = data.valueType.Value;
		}
	
	}
	public string GetKey()
	{
		return this.configKey;
	}

}

public class ConfigDataManager: TableBaseDataManager<ConfigData,TConfigData,string>
{
	private readonly Dictionary<string, string> dicStringConfigData = new Dictionary<string, string>();
	private readonly Dictionary<string, int> dicIntegerConfigData = new Dictionary<string, int>();
	private readonly Dictionary<string, bool> dicBooleanConfigData = new Dictionary<string, bool>();
	private readonly Dictionary<string, float> dicFloatConfigData = new Dictionary<string, float>();
	protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.CONFIG;
	protected override void AfterLoadComplete()
	{
		foreach (var configData in dicDatas.Values)
		{
			SetConfigDictionary(configData);
		}
	}
	
	private void SetConfigDictionary(ConfigData configData)
	{
		switch (configData.configValueType.ToLower().Trim())
		{
			case "bool":
				dicBooleanConfigData.Add(configData.configKey,Convert.ToBoolean(configData.configValue));
				break;
			case "int":
				if(int.TryParse(configData.configValue, out var intValue))
				{
					dicIntegerConfigData.Add(configData.configKey,intValue);
				}
				else
				{
					Debug.LogError($"{configData.configKey} is not integer");
				}
				break;
			case "float":
				if(float.TryParse(configData.configValue, out var floatValue))
				{
					dicFloatConfigData.Add(configData.configKey,floatValue);
				}
				else
				{
					Debug.LogError($"{configData.configKey} is not integer");
				}
				break;
			case "string":
				dicStringConfigData.Add(configData.configKey,configData.configValue);
				break;
			default:
				dicStringConfigData.Add(configData.configKey,configData.configValue);
				break;
		}
	}
	
	
	public bool GetInt(string key, out int intValue)
	{
		if (dicIntegerConfigData.TryGetValue(key, out intValue))
		{
			return true;
		}
		
		intValue = 0;
		return false;

	}

	public bool GetString(string key, out string strValue)
	{
		if (dicStringConfigData.TryGetValue(key, out strValue))
		{
			return true;
		}
		
		strValue=String.Empty;
		return false;
	}

	public bool GetFloat(string key, out float floatValue)
	{
		if (dicFloatConfigData.TryGetValue(key, out floatValue))
		{
			return true;
		}
		
		floatValue = 0f;
		return false;
	}

	public bool GetBool(string key, out bool boolValue)
	{
		if (dicBooleanConfigData.TryGetValue(key, out boolValue))
		{
			return true;
		}
		
		boolValue = false;
		return false;
	}
	
}