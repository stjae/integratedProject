using UnityEngine;
using Newtonsoft.Json;

public class SettingDataManager : DataManager<SettingData>
{
    const string predefinedFilepath = "setting.json";

    protected override void Awake()
    {
        base.Awake();
        Open(predefinedFilepath);
    }

    public override void Open(string filepath)
    {
        fileController.Filepath = predefinedFilepath; // ignore argument, filepath
        string json = fileController.ReadFile();

        if (string.IsNullOrEmpty(json))
        {
            data.Init();
        }
        else
        {
            data = JsonConvert.DeserializeObject<SettingData>(json);
        }

        Debug.Log("SettingDataManager.Open(): " + json);
    }

    public override void Save()
    {
        string json = JsonConvert.SerializeObject(data);

        fileController.WriteFile(json);

        Debug.Log("SettingDataManager.Save(): " + json);
    }
}