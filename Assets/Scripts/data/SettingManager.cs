using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Audio;

namespace Poly.Data
{
    public class SettingManager : MonoBehaviour
    {
        // setting data
        private SettingData settingData = new SettingData();

        // file controller
        private const string predefinedFilepath = "setting.json";
        private FileController fileController = new FileController();

        // audio mixer
        private const string resourcePath_masterMixer = "Master"; // Assets/Resources/Master.mixer
        private const string exposedParamName_volMaster = "volMaster";
        private const string exposedParamName_volBGM = "volBGM";
        private const string exposedParamName_volSFX = "volSFX";
        AudioMixer masterMixer;

        // get, set
        public ref SettingData GetSettingData() { return ref settingData; }
        // public SettingData SettingData { get { return settingData; } set { settingData = value; } }

        // open setting.json
        public void Open()
        {
            string json = fileController.ReadFile();

            if (string.IsNullOrEmpty(json))
            {
                settingData = new SettingData();
            }
            else
            {
                try
                {
                    settingData = JsonConvert.DeserializeObject<SettingData>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarningFormat("Deserialization failed: {0}", e);
                    settingData = new SettingData();
                }
            }
        }

        // save setting.json
        public void Save()
        {
            string json = JsonConvert.SerializeObject(settingData);
            fileController.WriteFile(json);
        }

        // apply settingData to in-game setting
        public void Apply()
        {
            // graphics
            int width = Screen.mainWindowDisplayInfo.width;
            int height = Screen.mainWindowDisplayInfo.height;
            switch (settingData.Resolution)
            {
                case SettingData.ResolutionOption.FHD:
                    width = 1920; height = 1080;
                    break;
                case SettingData.ResolutionOption.HD:
                    width = 1280; height = 720;
                    break;
                case SettingData.ResolutionOption.SVGA:
                    width = 800; height = 600;
                    break;
                case SettingData.ResolutionOption.VGA:
                    width = 640; height = 480;
                    break;
                default:
                    break;
            }
            Screen.SetResolution(width, height, settingData.FullScreen);
            QualitySettings.vSyncCount = (settingData.VSync) ? 1 : 0;

            // audio
            const float muteVol = -80.0f; // [dB]
            masterMixer.SetFloat(exposedParamName_volMaster, (settingData.PlayMaster) ? 20 * Mathf.Log10(settingData.VolMaster) : muteVol);
            masterMixer.SetFloat(exposedParamName_volBGM, (settingData.PlayBGM) ? 20 * Mathf.Log10(settingData.VolBGM) : muteVol);
            masterMixer.SetFloat(exposedParamName_volSFX, (settingData.PlaySFX) ? 20 * Mathf.Log10(settingData.VolSFX) : muteVol);
        }

        // MonoBehaviour
        private void Awake()
        {
            // singleton
            var objs = FindObjectsOfType<SettingManager>();
            if (objs.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

            fileController.Filepath = predefinedFilepath;
            masterMixer = Resources.Load<AudioMixer>(resourcePath_masterMixer);
        }

        private void OnApplicationQuit()
        {
            // autosave
            Save();
        }
    }
}