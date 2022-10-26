using UnityEngine;
using UnityEngine.SceneManagement;

using Poly.Data;
using Poly.DB;

public class InitSceneDirector : MonoBehaviour
{
    SettingManager settingManager;
    CookieManager cookieManager;

    private void Start()
    {
        settingManager = FindObjectOfType<SettingManager>();
        cookieManager = FindObjectOfType<CookieManager>();

        // setting data
        settingManager.Open();
        settingManager.Apply();
        settingManager.Save();

        // cookie
        cookieManager.Open();
        cookieManager.Save();

        SceneManager.LoadScene("MainScene");
    }
}
