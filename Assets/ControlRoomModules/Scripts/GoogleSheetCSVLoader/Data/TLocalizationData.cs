using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlRoom;

public class TLocalizationData : DataForm
{
    public TableType<int> index;
    public TableType<string> english;
    public TableType<string> korea;
    public TableType<string> taiwanese;
    public TableType<string> portuguese;
    public TableType<string> italian;
    public TableType<string> turkish;
    public TableType<string> russian;
    public TableType<string> french;
    public TableType<string> spainish;
    public TableType<string> japan;

    public TLocalizationData()
    {
        index = new TableType<int>("index", this);
        english = new TableType<string>("English", this);
        korea = new TableType<string>("Korea", this);
        taiwanese = new TableType<string>("Taiwanese",this);
        portuguese = new TableType<string>("Portuguese",this);
        italian = new TableType<string>("Italian", this);
        turkish = new TableType<string>("Turkish", this);
        russian = new TableType<string>("Russian", this);
        french = new TableType<string>("French", this);
        spainish= new TableType<string>("Spanish", this);
        japan= new TableType<string>("Japan", this);
}
}
public class LocalizationData : ITableData, IKeyProvider<int>
{
    public int index;
    public string english;
    public string korea;
    public string taiwanese;
    public string portuguese;
    public string italian;
    public string turkish;
    public string russian;
    public string french;
    public string spanish;
    public string japan;


    public void SetValue(DataForm dataForm)
    {
        var tData = dataForm as TLocalizationData;

        if (tData != null)
        {
            this.index = tData.index.Value;
            this.english = tData.english.Value;
            this.korea = tData.korea.Value;
            this.taiwanese = tData.taiwanese.Value;
            this.portuguese = tData.portuguese.Value;
            this.italian = tData.italian.Value;
            this.french = tData.french.Value;
            this.russian = tData.russian.Value;
            this.spanish = tData.spainish.Value;
            this.japan = tData.japan.Value;
            this.turkish = tData.turkish.Value;

        }
    }

    public int GetKey()
    {
        return this.index;
    }
}

public class LocalizationDataManager: TableBaseDataManager<LocalizationData,TLocalizationData,int>
{
    public static System.Action listener;
    public bool LoadComplete = false;
    private UnityEngine.SystemLanguage language= SystemLanguage.English;
    protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.LOCALIZATION;
    private bool initializeCurrentLanguage = false;
    private int languageIndex = 0;

    public SystemLanguage[] SupportLanguages =
    {
        SystemLanguage.English, 
        SystemLanguage.ChineseTraditional,
        SystemLanguage.Italian,
        SystemLanguage.Russian,
        SystemLanguage.French,
        SystemLanguage.Spanish,
        SystemLanguage.Japanese,
        SystemLanguage.Portuguese,
        SystemLanguage.Turkish,
        SystemLanguage.Korean
        
    };

    public SystemLanguage CurrentLanguage
    {
        get
        {
            return language;
        }
    }
    

    public void GetCurrentLanguageSettings()
    {
        var sysLanguage= Application.systemLanguage;

        int currentLanguage = PlayerPrefs.GetInt("Language", (int)sysLanguage);

        switch(currentLanguage)
        {
            case (int)SystemLanguage.English:
                language = SystemLanguage.English;
                languageIndex = 0;
                break;
            case (int)SystemLanguage.Korean:
                language = SystemLanguage.Korean;
                languageIndex = 8;
                break;
            case (int)SystemLanguage.Japanese:
                language = SystemLanguage.Japanese;
                languageIndex = 6;
                break;
            case (int)SystemLanguage.Turkish:
                language = SystemLanguage.Turkish;
                languageIndex = 7;
                break;
            case (int)SystemLanguage.Italian:
                language = SystemLanguage.Italian;
                languageIndex = 2;
                break;
            case (int)SystemLanguage.Portuguese:
                language = SystemLanguage.Portuguese;
                languageIndex = 7;
                break;
            case (int)SystemLanguage.Russian:
                language = SystemLanguage.Russian;
                languageIndex = 3;
                break;
            case (int)SystemLanguage.ChineseTraditional:
                language = SystemLanguage.ChineseTraditional;
                languageIndex = 1;
                break;
            case (int)SystemLanguage.French:
                language = SystemLanguage.French;
                languageIndex = 4;
                break;
            case (int)SystemLanguage.Spanish:
                language = SystemLanguage.Spanish;
                languageIndex = 5;
                break;
           
            default:
                language = SystemLanguage.English;
                languageIndex = 0;
                break;
        }

    }

   
    protected override void AfterLoadComplete()
    {
        LoadComplete = true;

        GetCurrentLanguageSettings();
        
        if (listener != null)
            listener.Invoke();
    }


    public string GetLocalizationData(int stageIndex)
    {
        if (!dicDatas.ContainsKey(stageIndex))
        {
            Debug.LogError("Not Exist Stage Data");
            return null;
        }

        if (!initializeCurrentLanguage)
        {
            GetCurrentLanguageSettings();
            initializeCurrentLanguage = true;
        }

        switch (this.language)
        {
            case SystemLanguage.English:
                return dicDatas[stageIndex].english;
            case SystemLanguage.Korean:
                return dicDatas[stageIndex].korea;
            case SystemLanguage.Japanese:
                return dicDatas[stageIndex].japan;
            case SystemLanguage.Turkish:
                return dicDatas[stageIndex].turkish;
            case SystemLanguage.Italian:
                return dicDatas[stageIndex].italian;
            case SystemLanguage.Russian:
                return dicDatas[stageIndex].russian;
            case SystemLanguage.ChineseTraditional:
                return dicDatas[stageIndex].taiwanese;
            case SystemLanguage.French:
                return dicDatas[stageIndex].french;
            case SystemLanguage.Spanish:
                return dicDatas[stageIndex].spanish;
            case SystemLanguage.Portuguese:
                return dicDatas[stageIndex].portuguese;

            default:
                return dicDatas[stageIndex].english;
        }
      

    }

    public void UpdateLanguageText(SystemLanguage lan)
    {
        this.language = lan;
        PlayerPrefs.SetInt("Language", (int)this.language);

        GetCurrentLanguageSettings();

        if(listener!=null)
            listener.Invoke();
    }

    public int GetCurrentLanguageIndex => languageIndex;
}