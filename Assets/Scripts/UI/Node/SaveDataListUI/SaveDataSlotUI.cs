using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;

using Poly.Data;
using Poly.Data.Cryptography;

public class SaveDataSlotUI : MonoBehaviour
{
    private FileController fileController = new FileController();
    private string fullFilepath;

    private CookieManager cookieManager;
    private SaveManager saveManager;

    private string saveDataFilename;
    public string SaveDataFilename { get { return saveDataFilename; } set { saveDataFilename = value; } }

    public TextMeshProUGUI text_saveDataInfo;
    public UnityEngine.UI.Button btn_save;
    public UnityEngine.UI.Button btn_load;

    private void Awake()
    {
        cookieManager = FindObjectOfType<CookieManager>();
        saveManager   = FindObjectOfType<SaveManager>();

        btn_save.onClick.AddListener(() =>
        {
            saveManager.SaveAs(saveDataFilename);
            Debug.Log("Game saved");
        });

        btn_load.onClick.AddListener(() =>
        {
            // recent save data = last loaded save data
            cookieManager.GetCookie().RecentSaveData = saveDataFilename;

            saveManager.Open(saveDataFilename);
            SceneLoader.LoadSceneWithSaveData();
        });
    }

    private void Start()
    {
        // SaveDataListUI assign this.saveDataFilename = "save_0n" on Awake()
        fileController.Filepath = SaveManager.predefinedDirectory + saveDataFilename;
        fullFilepath = Application.persistentDataPath + "/" + SaveManager.predefinedDirectory + saveDataFilename;
    }

    private void Update()
    {
        // save button is available when a SaveData is loaded on SaveManager
        if (saveManager.GetSaveData() != null)
        {
            btn_save.interactable = true;
        }
        else
        {
            btn_save.interactable = false;
        }

        // load button is available when a save_0n exist
        if (File.Exists(fullFilepath))
        {
            string encryptedJson = fileController.ReadFile();
            string json = AES.Decrypt(encryptedJson, SaveManager.predefinedKey);
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

            string savedTime = new FileInfo(fullFilepath).LastWriteTime.ToString();

            text_saveDataInfo.text = string.Format("Chapter {0}, Level {1}, CheckPoint {2}",
                saveData.Chapter, saveData.Level, saveData.Checkpoint);
            btn_load.interactable  = true;
        }
        else
        {
            text_saveDataInfo.text = "No Data";
            btn_load.interactable  = false;
        }
    }
}
