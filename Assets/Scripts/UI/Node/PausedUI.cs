using UnityEngine;
using UnityEngine.SceneManagement;

using Poly.UI;

public class PausedUI : UINode
{
    // UINode
    private UINode saveDataListUI;
    private UINode settingMenuUI;

    // UI (assign in inspector)
    public UnityEngine.UI.Button btn_resume;
    public UnityEngine.UI.Button btn_save;
    public UnityEngine.UI.Button btn_setting;
    public UnityEngine.UI.Button btn_returnToTitle;

    private void Awake()
    {
        // link UINode
        saveDataListUI = FindObjectOfType<SaveDataListUI>(true);
        settingMenuUI = FindObjectOfType<SettingMenuUI>(true);

        // add event listener
        btn_resume.onClick.AddListener(() => StepOut());
        btn_save.onClick.AddListener(() => StepInto(saveDataListUI));
        btn_setting.onClick.AddListener(() => StepInto(settingMenuUI));
        btn_returnToTitle.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StepOut();
        }
    }
}
