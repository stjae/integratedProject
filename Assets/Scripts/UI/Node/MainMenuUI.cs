using UnityEngine;
using TMPro;

using Poly.UI;
using Poly.Data;
using Poly.DB;

public class MainMenuUI : UINode
{
    // data manager
    private CookieManager cookieManager;
    private LoginManager loginManager;

    // UINode
    private UINode settingMenuUI;
    private UINode creditsUI;

    private UINode loginUI;
    private UINode createAccountUI;

    // UI (assign in inspector)
    public UnityEngine.UI.Button btn_continue;
    public UnityEngine.UI.Button btn_loadGame;
    public UnityEngine.UI.Button btn_newGame;
    public UnityEngine.UI.Button btn_setting;
    public UnityEngine.UI.Button btn_credits;
    public UnityEngine.UI.Button btn_exit;

    public TextMeshProUGUI       text_username;
    public UnityEngine.UI.Button btn_login;
    public UnityEngine.UI.Button btn_createAccount;
    public UnityEngine.UI.Button btn_logout;

    private void Awake()
    {
        cookieManager = FindObjectOfType<CookieManager>();
        loginManager  = FindObjectOfType<LoginManager>();

        // link UINode
        settingMenuUI   = FindObjectOfType<SettingMenuUI>(true);
        creditsUI       = FindObjectOfType<CreditsUI>(true);
        loginUI         = FindObjectOfType<LoginUI>(true);
        createAccountUI = FindObjectOfType<CreateAccountUI>(true);

        // add event listener
        btn_setting.onClick.AddListener(() => StepInto(settingMenuUI));
        btn_credits.onClick.AddListener(() => StepInto(creditsUI));
        btn_exit.onClick.AddListener(Application.Quit);

        btn_login.onClick.AddListener(() => StepInto(loginUI));
        btn_createAccount.onClick.AddListener(() => StepInto(createAccountUI));
        btn_logout.onClick.AddListener(() => { loginManager.SignOut(); });
    }

    private void Update()
    {
        btn_continue.interactable = !string.IsNullOrEmpty(cookieManager.GetCookie().RecentSaveData);

        text_username.text = (loginManager.IsLoggedIn) ? loginManager.User.DisplayName : "Guest";
        btn_login.interactable         = !loginManager.IsLoggedIn;
        btn_createAccount.interactable = !loginManager.IsLoggedIn;
        btn_logout.interactable        =  loginManager.IsLoggedIn;
    }
}
