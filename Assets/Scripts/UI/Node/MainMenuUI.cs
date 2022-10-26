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
    private SaveManager saveManager;

    // popup UI
    private OkPopup okPopup;

    // UINode
    private UINode saveDataListUI;
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

    public UnityEngine.UI.Button btn_upload;
    public UnityEngine.UI.Button btn_download;

    private async void OnUploadButtonClick()
    {
        // lock UI
        btn_upload.interactable   = false;
        btn_download.interactable = false;

        await DatabaseManagement.UploadSaveData();
        okPopup.Open("Success", "Data uploaded successfully!");

        // unlock UI
        btn_upload.interactable   = true;
        btn_download.interactable = true;
    }

    private async void OnDownloadButtonClick()
    {
        // lock UI
        btn_upload.interactable   = false;
        btn_download.interactable = false;

        await DatabaseManagement.DownloadSaveData();
        okPopup.Open("Success", "Data downloaded successfully!");

        // unlock UI
        btn_upload.interactable   = true;
        btn_download.interactable = true;
    }

    private void Awake()
    {
        cookieManager = FindObjectOfType<CookieManager>();
        loginManager  = FindObjectOfType<LoginManager>();
        saveManager  = FindObjectOfType<SaveManager>();

        okPopup = FindObjectOfType<OkPopup>(true);

        // link UINode
        saveDataListUI  = FindObjectOfType<SaveDataListUI>(true);
        settingMenuUI   = FindObjectOfType<SettingMenuUI>(true);
        creditsUI       = FindObjectOfType<CreditsUI>(true);
        loginUI         = FindObjectOfType<LoginUI>(true);
        createAccountUI = FindObjectOfType<CreateAccountUI>(true);

        // add event listener
        btn_continue.onClick.AddListener(() =>
        {
            saveManager.Open(cookieManager.GetCookie().RecentSaveData);
            SceneLoader.LoadSceneWithSaveData();
        });
        btn_loadGame.onClick.AddListener(() => StepInto(saveDataListUI));
        btn_newGame.onClick.AddListener(() =>
        {
            saveManager.New();
            SceneLoader.LoadSceneWithSaveData();
        });
        btn_setting.onClick.AddListener(() => StepInto(settingMenuUI));
        btn_credits.onClick.AddListener(() => StepInto(creditsUI));
        btn_exit.onClick.AddListener(Application.Quit);

        btn_login.onClick.AddListener(() => StepInto(loginUI));
        btn_createAccount.onClick.AddListener(() => StepInto(createAccountUI));
        btn_logout.onClick.AddListener(() => { loginManager.LogOut(); });

        btn_upload.onClick.AddListener(OnUploadButtonClick);
        btn_download.onClick.AddListener(OnDownloadButtonClick);
    }

    private void Update()
    {
        btn_continue.interactable = !string.IsNullOrEmpty(cookieManager.GetCookie().RecentSaveData);

        text_username.text = (loginManager.IsLoggedIn) ? loginManager.User.DisplayName : "Guest";
        btn_login.interactable         = !loginManager.IsLoggedIn;
        btn_createAccount.interactable = !loginManager.IsLoggedIn;
        btn_logout.interactable        =  loginManager.IsLoggedIn;

        btn_upload.interactable   = loginManager.IsLoggedIn;
        btn_download.interactable = loginManager.IsLoggedIn;
    }
}
