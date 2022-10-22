using UnityEngine;
using Newtonsoft.Json;

using Poly.Data.Cryptography;

namespace Poly.Data
{
    public class SaveManager : MonoBehaviour
    {
        // private const string predefinedFilepath = "save_00";
        private const string predefinedKey = "sample key text";

        private FileController fileController = new FileController();
        private SaveData saveData = new SaveData();

        // get, set
        public ref SaveData GetSaveData() { return ref saveData; }
        // public SaveData SaveData { get { return saveData; } set { saveData = value; } }

        // open setting.json
        public void Open()
        {
            string encryptedJson = fileController.ReadFile();

            if (string.IsNullOrEmpty(encryptedJson))
            {
                saveData = new SaveData();
            }
            else
            {
                try
                {
                    string json = AES.Decrypt(encryptedJson, predefinedKey);
                    saveData = JsonConvert.DeserializeObject<SaveData>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarningFormat("Deserialization failed: {0}", e);
                    saveData = new SaveData();
                }
            }
        }

        // save setting.json
        public void Save()
        {
            string json = JsonConvert.SerializeObject(saveData);
            fileController.WriteFile(json);
        }

        // apply settingData to in-game setting
        public void Apply() { }

        // MonoBehaviour
        private void Awake()
        {
            // singleton
            var objs = FindObjectsOfType<SaveManager>();
            if (objs.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

            fileController.Filepath = null;
        }
    }
}