using UnityEngine;
using Newtonsoft.Json;

public class AccountManager : MonoBehaviour
{
    private const string predefinedFilepath = "recentAccount";
    private const string predefinedKey = "sample key text";

    private FileController fileController = new FileController();
    private AccountData accountData = new AccountData();

    // get, set
    public ref AccountData GetAccountData() { return ref accountData; }

    // open recentAccount
    public void Open()
    {
        string encryptedJson = fileController.ReadFile();

        if (string.IsNullOrEmpty(encryptedJson))
        {
            accountData = new AccountData();
        }
        else
        {
            try
            {
                string json = AES.Decrypt(encryptedJson, predefinedKey);
                accountData = JsonConvert.DeserializeObject<AccountData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogWarningFormat("Deserialization failed: {0}", e);
                accountData = new AccountData();
            }
        }

        Debug.LogFormat("AccountDataManager.Open(): {0}", JsonConvert.SerializeObject(accountData));
    }

    // save recentAccount
    public void Save()
    {
        string json = JsonConvert.SerializeObject(accountData);
        string encryptedJson = AES.Encrypt(json, predefinedKey);

        fileController.WriteFile(encryptedJson);

        Debug.LogFormat("AccountDataManager.Save(): {0}", json);
    }

    // MonoBehaviour
    private void Awake()
    {
        // singleton
        var objs = FindObjectsOfType<AccountManager>();
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        fileController.Filepath = predefinedFilepath;
    }
}
