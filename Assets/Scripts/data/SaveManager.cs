using UnityEngine;
using System.IO;
using Newtonsoft.Json;

using Poly.Data.Cryptography;

namespace Poly.Data
{
    public class SaveManager : MonoBehaviour
    {
        public const string predefinedDirectory = "saves/";
        public const string predefinedKey = "sample key text";

        private FileController fileController = new FileController();
        private SaveData saveData;

        // get, set
        public ref SaveData GetSaveData() { return ref saveData; }
        // public SaveData SaveData { get { return saveData; } set { saveData = value; } }

        /// <summary>
        /// open SaveData <br/><br/>
        /// <para>
        /// saveDataFilename = filepath under Application.persistentDataPath/saves
        /// </para>
        /// </summary>
        public void Open(string saveDataFilename)
        {
            fileController.Filepath = predefinedDirectory + saveDataFilename;
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

        /// <summary>
        /// save SaveData to current file <br/><br/>
        /// </summary>
        public void Save()
        {
            string json = JsonConvert.SerializeObject(saveData);
            string encryptedJson = AES.Encrypt(json, predefinedKey);

            fileController.WriteFile(encryptedJson);

            Debug.LogFormat("SaveManager.Save(): {0}", json);
        }

        /// <summary>
        /// save SaveData to other file <br/><br/>
        /// <para>
        /// saveDataFilename = filepath under Application.persistentDataPath/saves
        /// </para>
        /// </summary>
        public void SaveAs(string saveDataFilename)
        {
            fileController.Filepath = predefinedDirectory + saveDataFilename;
            Save();
        }

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

            // ensure "saves" folder exist under Application.persistentDataPath
            if (!Directory.Exists(Application.persistentDataPath + predefinedDirectory))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + predefinedDirectory);
            }

            fileController.Filepath = null;
            saveData = null;
        }
    }
}