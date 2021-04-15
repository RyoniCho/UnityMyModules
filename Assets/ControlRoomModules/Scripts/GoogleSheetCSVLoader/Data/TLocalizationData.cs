using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlRoom;

public class TLocalizationData : DataForm
{
    public TInt index;
    public TString english;
    public TString korea;
    public TString taiwanese;
    public TString portuguese;
    public TString italian;
    public TString turkish;
    public TString russian;
    public TString french;
    public TString spainish;
    public TString japan;

    public TLocalizationData()
    {
        index = new TInt("index", this);
        english = new TString("English", this);
        korea = new TString("Korea", this);
        taiwanese = new TString("Taiwanese",this);
        portuguese = new TString("Portuguese",this);
        italian = new TString("Italian", this);
        turkish = new TString("Turkish", this);
        russian = new TString("Russian", this);
        french = new TString("French", this);
        spainish= new TString("Spanish", this);
        japan= new TString("Japan", this);
}
}
public class LocalizationData
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
    public string spainish;
    public string japan;


    public LocalizationData(TLocalizationData tData)
    {
        this.index = tData.index.Value;
        this.english = tData.english.Value;
        this.korea = tData.korea.Value;
   
    }
}

public class LocalizationDataManager : SingletonBase<LocalizationDataManager>
{
    public System.Action<bool> listener;
    public bool LoadComplete = false;
    private UnityEngine.SystemLanguage language= SystemLanguage.English;

    public SystemLanguage CurrentLanguage
    {
        get
        {
            return language;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
       
       
    }

    private Dictionary<int, LocalizationData> dicLocalization = new Dictionary<int, LocalizationData>();

    private void GetCurrentLanguageSettings()
    {
        var sysLanguage= Application.systemLanguage;

        int currentLanguage = PlayerPrefs.GetInt("Language", (int)sysLanguage);

        switch(currentLanguage)
        {
            case (int)SystemLanguage.English:
                language = SystemLanguage.English;
                break;
            case (int)SystemLanguage.Korean:
                language = SystemLanguage.Korean;
                break;
                
            default:
                language = SystemLanguage.English;
                break;
        }

    }

    private void SetTableData(int index, LocalizationData data)
    {
        dicLocalization.Add(index, data);
    }


    public string GetLocalizationData(int stageIndex)
    {
        if (!dicLocalization.ContainsKey(stageIndex))
        {
            Debug.LogError("Not Exist Stage Data");
            return null;
        }

        switch(this.language)
        {
            case SystemLanguage.English:
                return dicLocalization[stageIndex].english;
               
            case SystemLanguage.Korean:
                return dicLocalization[stageIndex].korea;

            default:
                return dicLocalization[stageIndex].english;
        }
      

    }


    public void LoadData()
    {

        TableDataLoader.StartDownload((int)TableManager.GoogleDocsID.LOCALIZATION, delegate (TableData data)
        {
            foreach (var tableData in data.dicTableData)
            {


                TLocalizationData tLocalization = new TLocalizationData();

                tLocalization.SetDataValues(tableData.Value);
                SetTableData(tLocalization.index.Value, new LocalizationData(tLocalization));

            }

            TableManager.Instance.LoadCompleteTableData(TableManager.GoogleDocsID.LOCALIZATION);
            LoadComplete = true;

            GetCurrentLanguageSettings();

            if(listener!=null)
            listener.Invoke(false);
        });


    }

    public void DownLoadAndSaveData()
    {
        TableDataLoader.StartDownload((int)TableManager.GoogleDocsID.LOCALIZATION, null, true);
    }

    public void UpdateLanguageText(SystemLanguage language)
    {
        this.language = language;
        PlayerPrefs.SetInt("Language", (int)this.language);

        if(listener!=null)
        listener.Invoke(true);
    }
}