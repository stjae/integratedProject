using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;

using Firebase.Database;

using Poly.Data;
using Poly.Data.Cryptography;

namespace Poly.DB
{
    public sealed class DatabaseManagement : MonoBehaviour
    {
        /// <summary>
        /// upload /users/<UserId>/setting <br/><br/>
        /// <para>
        /// return = <br/>
        /// true (success) <br/>
        /// false (failed) <br/>
        /// </para>
        /// </summary>
        public static async Task<bool> UploadSettingData()
        {
            DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;

            LoginManager loginMgr = FindObjectOfType<LoginManager>();
            if(!loginMgr.IsLoggedIn)
            {
                Debug.LogError("To upload setting from FirebaseDB, you should be logged-in.");
                return false;
            }

            string uid = loginMgr.User.UserId;
            SettingData setting = FindObjectOfType<SettingManager>().GetSettingData();

            string json = JsonConvert.SerializeObject(setting);
            await root.Child("users").Child(uid).Child("setting").SetRawJsonValueAsync(json);

            return true;
        }

        /// <summary>
        /// download /users/<UserId>/setting <br/><br/>
        /// <para>
        /// return = <br/>
        /// true (success) <br/>
        /// false (failed) <br/>
        /// </para>
        /// </summary>
        public static async Task<bool> DownloadSettingData()
        {
            DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;

            LoginManager loginMgr = FindObjectOfType<LoginManager>();
            if (!loginMgr.IsLoggedIn)
            {
                Debug.LogError("To download setting data from FirebaseDB, you should be logged-in.");
                return false;
            }

            string uid = loginMgr.User.UserId;
            SettingData setting = FindObjectOfType<SettingManager>().GetSettingData();

            bool isSuccess = false;
            await root.Child("users").Child(uid).Child("setting").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    isSuccess = false;

                    Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                    return;
                }
                else if (task.IsCompleted)
                {
                    isSuccess = true;

                    DataSnapshot snapshot = task.Result;
                    string json = snapshot.GetRawJsonValue();

                    setting = (string.IsNullOrEmpty(json)) ? new SettingData() : JsonConvert.DeserializeObject<SettingData>(json);
                    Debug.LogFormat("Read remote setting data successfully : {0}", snapshot.GetRawJsonValue());
                    return;
                }
            });

            return isSuccess;
        }

        // ================================

        /// <summary>
        /// upload /users/<UserId>/saves/save_00 ~ 02 <br/><br/>
        /// </summary>
        public static async Task UploadSaveData()
        {
            DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;

            LoginManager loginMgr = FindObjectOfType<LoginManager>();
            if (!loginMgr.IsLoggedIn)
            {
                Debug.LogError("To upload save data from FirebaseDB, you should be logged-in.");
                return;
            }

            string uid = loginMgr.User.UserId;
            string[] filenames = { "save_00", "save_01", "save_02" };
            FileController fc = new FileController();
            string json;
            SaveData sd;

            foreach(string filename in filenames)
            {
                fc.Filepath = SaveManager.predefinedDirectory + filename;
                string encryptedJson = fc.ReadFile();

                if (string.IsNullOrEmpty(encryptedJson))
                {
                    continue;
                }
                else
                {
                    try
                    {
                        // apply constraints of SaveData
                        json = AES.Decrypt(encryptedJson, SaveManager.predefinedKey);
                        sd = JsonConvert.DeserializeObject<SaveData>(json);
                        json = JsonConvert.SerializeObject(sd);

                        await root.Child("users").Child(uid).Child("saves").Child(filename).SetRawJsonValueAsync(json);
                        Debug.LogFormat("Uploaded {0} : {1}", filename, json);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarningFormat("Deserialization failed: {0}", e);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// download /users/<UserId>/saves/save_00 ~ 02 <br/><br/>
        /// </summary>
        public static async Task DownloadSaveData()
        {
            DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;

            LoginManager loginMgr = FindObjectOfType<LoginManager>();
            if (!loginMgr.IsLoggedIn)
            {
                Debug.LogError("To download save data from FirebaseDB, you should be logged-in.");
                return;
            }

            string uid = loginMgr.User.UserId;
            string[] filenames = { "save_00", "save_01", "save_02" };
            FileController fc = new FileController();
            string json = null;
            SaveData sd;

            foreach (string filename in filenames)
            {
                await root.Child("users").Child(uid).Child("saves").Child(filename).GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                        return;
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        json = snapshot.GetRawJsonValue();
                        return;
                    }
                });

                if (string.IsNullOrEmpty(json)) { continue; }
                Debug.LogFormat("Downloaded {0} : {1}", filename, json);

                // apply constraints of SaveData
                sd = JsonConvert.DeserializeObject<SaveData>(json);
                json = JsonConvert.SerializeObject(sd);

                string encryptedJson = AES.Encrypt(json, SaveManager.predefinedKey);
                fc.Filepath = SaveManager.predefinedDirectory + filename;
                fc.WriteFile(encryptedJson);
            }

            return;
        }
    }
}