using UnityEngine;
using Newtonsoft.Json;

using Poly.Data.Cryptography;

namespace Poly.Data
{
    public class CookieManager : MonoBehaviour
    {
        private const string predefinedFilepath = "cookie";
        private const string predefinedKey = "sample key text";

        private FileController fileController = new FileController();
        private Cookie cookie = new Cookie();

        // get, set
        public ref Cookie GetCookie() { return ref cookie; }

        // open recentAccount
        public void Open()
        {
            string encryptedJson = fileController.ReadFile();

            if (string.IsNullOrEmpty(encryptedJson))
            {
                cookie = new Cookie();
            }
            else
            {
                try
                {
                    string json = AES.Decrypt(encryptedJson, predefinedKey);
                    cookie = JsonConvert.DeserializeObject<Cookie>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarningFormat("Deserialization failed: {0}", e);
                    cookie = new Cookie();
                }
            }

            Debug.LogFormat("CookieManager.Open(): {0}", JsonConvert.SerializeObject(cookie));
        }

        // save recentAccount
        public void Save()
        {
            string json = JsonConvert.SerializeObject(cookie);
            string encryptedJson = AES.Encrypt(json, predefinedKey);

            fileController.WriteFile(encryptedJson);

            Debug.LogFormat("CookieManager.Save(): {0}", json);
        }

        // MonoBehaviour
        private void Awake()
        {
            // singleton
            var objs = FindObjectsOfType<CookieManager>();
            if (objs.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

            fileController.Filepath = predefinedFilepath;
        }

        private void OnApplicationQuit()
        {
            // autosave
            Save();
        }
    }
}