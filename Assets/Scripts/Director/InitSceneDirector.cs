using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneDirector : MonoBehaviour
{
    SettingManager settingManager;
    AccountManager accountManager;

    private void Start()
    {
        settingManager = FindObjectOfType<SettingManager>();
        accountManager = FindObjectOfType<AccountManager>();

        // setting data
        settingManager.Open();
        settingManager.Apply();
        settingManager.Save();

        // account data
        accountManager.Open();
        accountManager.Save();

        SceneManager.LoadScene("MainScene");
    }
}
