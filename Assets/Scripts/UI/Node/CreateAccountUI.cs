using TMPro;

using Firebase.Auth;

using Poly.UI;
using Poly.Data;
using Poly.DB;

public class CreateAccountUI : UINode
{
    // data manager
    private CookieManager cookieManager;

    // popup UI
    private OkPopup okPopup;

    // UI (assign in inspector)
    public TMP_InputField inputField_username;
    public TMP_InputField inputField_email;
    public TMP_InputField inputField_password;
    public UnityEngine.UI.Button btn_createAccount;
    public UnityEngine.UI.Button btn_back;

    private async void OnCreateAccountButtonClick()
    {
        string username = inputField_username.text;
        string email    = inputField_email.text;
        string password = inputField_password.text;

        if (string.IsNullOrEmpty(username)) { okPopup.Open("Error", "Username cannot be empty.");      return; }
        if (string.IsNullOrEmpty(email))    { okPopup.Open("Error", "Email address cannot be empty."); return; }
        if (string.IsNullOrEmpty(password)) { okPopup.Open("Error", "Password cannot be empty.");      return; }

        // lock UI
        inputField_username.interactable = false;
        inputField_email.interactable    = false;
        inputField_password.interactable = false;
        btn_createAccount.interactable   = false;

        FirebaseUser newUser = await UserManagement.CreateUser(email, password);

        if (newUser != null)
        {
            cookieManager.GetCookie().RecentEmail    = email;
            cookieManager.GetCookie().RecentPassword = password;

            // set username
            UserProfile profile = new UserProfile { DisplayName = username };
            await UserManagement.UpdateUserProfile(newUser, profile);

            // send verification email
            await UserManagement.SendEmailVerification(newUser);
            okPopup.Open("Verify your account", string.Format("A verification email has been sent to {0}.", email));
        }
        else
        {
            bool isInUse = await UserManagement.CheckIfEmailExists(email);

            if (isInUse)
            {
                okPopup.Open("Error", "The given email is already in use.");
            }
            else
            {
                okPopup.Open("Error", "Invalid email or password");
            }
        }

        // unlock UI
        inputField_username.interactable = true;
        inputField_email.interactable    = true;
        inputField_password.interactable = true;
        btn_createAccount.interactable   = true;
    }

    private void Awake()
    {
        cookieManager = FindObjectOfType<CookieManager>();

        okPopup = FindObjectOfType<OkPopup>(true);

        // add event listener
        btn_createAccount.onClick.AddListener(() => { OnCreateAccountButtonClick(); });
        btn_back.onClick.AddListener(StepOut);

        // InputField.ContentType
        inputField_username.contentType = TMP_InputField.ContentType.Alphanumeric;
        inputField_email.contentType    = TMP_InputField.ContentType.EmailAddress;
        inputField_password.contentType = TMP_InputField.ContentType.Password;
    }

    private void OnDisable()
    {
        inputField_username.text = "";
        inputField_email.text    = "";
        inputField_password.text = "";
    }
}
