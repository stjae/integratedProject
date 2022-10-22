using TMPro;

using Poly.UI;
using Poly.Data;
using Poly.DB;

public class LoginUI : UINode
{
    // data manager
    private CookieManager cookieManager;
    private LoginManager loginManager;

    // popup UI
    private OkPopup okPopup;

    // UI (assign in inspector)
    public TMP_InputField inputField_email;
    public TMP_InputField inputField_password;
    public UnityEngine.UI.Button btn_login;
    public UnityEngine.UI.Button btn_back;

    private async void OnLoginButtonClick()
    {
        if(loginManager.IsLoggedIn) { okPopup.Open("Error", "You are already logged in."); return; }

        string email    = inputField_email.text;
        string password = inputField_password.text;

        if (string.IsNullOrEmpty(email))    { okPopup.Open("Error", "Email address cannot be empty."); return; }
        if (string.IsNullOrEmpty(password)) { okPopup.Open("Error", "Password cannot be empty.");      return; }

        // lock UI
        inputField_email.interactable    = false;
        inputField_password.interactable = false;
        btn_login.interactable           = false;

        cookieManager.GetCookie().RecentEmail    = email;
        cookieManager.GetCookie().RecentPassword = password;

        await loginManager.SignIn(email, password);

        if(loginManager.IsLoggedIn)
        {
            okPopup.Open("Success", "Welcome, " + loginManager.User.DisplayName);
        }
        else if(!loginManager.User.IsEmailVerified)
        {
            await UserManagement.SendEmailVerification(loginManager.User);
            okPopup.Open("Verify your account", string.Format("A verification email has been sent to {0}.", email));
        }
        else
        {
            okPopup.Open("Error", "Invalid email or password");
        }

        // unlock UI
        inputField_email.interactable    = true;
        inputField_password.interactable = true;
        btn_login.interactable           = true;

        if (loginManager.IsLoggedIn) { StepOut(); }
    }

    private void Awake()
    {
        cookieManager = FindObjectOfType<CookieManager>();
        loginManager  = FindObjectOfType<LoginManager>();

        okPopup = FindObjectOfType<OkPopup>(true);

        // add event listener
        btn_login.onClick.AddListener(OnLoginButtonClick);
        btn_back.onClick.AddListener(StepOut);

        // InputField.ContentType
        inputField_email.contentType    = TMP_InputField.ContentType.EmailAddress;
        inputField_password.contentType = TMP_InputField.ContentType.Password;
    }

    private void OnEnable()
    {
        inputField_email.text    = cookieManager.GetCookie().RecentEmail;
        inputField_password.text = cookieManager.GetCookie().RecentPassword;
    }
}
