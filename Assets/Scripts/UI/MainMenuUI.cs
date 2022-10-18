using UnityEngine;

using Poly.UI;

public class MainMenuUI : UINode
{
    // UINode
    private UINode SettingMenu;

    // UI (assign in inspector)
    public UnityEngine.UI.Button btn_continue;
    public UnityEngine.UI.Button btn_loadGame;
    public UnityEngine.UI.Button btn_newGame;
    public UnityEngine.UI.Button btn_logIn;
    public UnityEngine.UI.Button btn_setting;
    public UnityEngine.UI.Button btn_credits;
    public UnityEngine.UI.Button btn_exit;

    private void Awake()
    {
        // link UINode
        SettingMenu = FindObjectOfType<SettingMenuUI>(true);

        // add event listener
        btn_setting.onClick.AddListener(() => StepInto(SettingMenu));
        btn_exit.onClick.AddListener(Application.Quit);
    }
}
