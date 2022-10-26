using UnityEngine;
using System.IO;
using TMPro;

using Poly.Data;

public class SaveDataSlotUI : MonoBehaviour
{
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

        // add event listener
        btn_save.onClick.AddListener(() => { saveManager.SaveAs(saveDataFilename); });
        btn_load.onClick.AddListener(() =>
        {
            // recent save data = last loaded save data
            cookieManager.GetCookie().RecentSaveData = saveDataFilename;

            saveManager.Open(saveDataFilename);
            Debug.LogWarning("TODO : LoadScene()");
        });
    }

    private void Update()
    {
        string fullFilepath = Application.persistentDataPath + "/saves/" + saveDataFilename;

        // save button is available when a SaveData is loaded on SaveManager
        if(saveManager.GetSaveData() != null)
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
            text_saveDataInfo.text = new FileInfo(fullFilepath).LastWriteTime.ToString();
            btn_load.interactable  = true;
        }
        else
        {
            text_saveDataInfo.text = "No Data";
            btn_load.interactable  = false;
        }
    }
}
