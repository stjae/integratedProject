public class AudioSettingUI : UINode
{
    private SettingManager settingManager;

    // UI (assign in inspector)
    public UnityEngine.UI.Slider slider_master;
    public UnityEngine.UI.Slider slider_bgm;
    public UnityEngine.UI.Slider slider_sfx;

    public UnityEngine.UI.Toggle toggle_master;
    public UnityEngine.UI.Toggle toggle_bgm;
    public UnityEngine.UI.Toggle toggle_sfx;

    public UnityEngine.UI.Button btn_back;

    private void Awake()
    {
        settingManager = FindObjectOfType<SettingManager>();

        slider_master.minValue = SettingData.minVol;
        slider_bgm.minValue = SettingData.minVol;
        slider_sfx.minValue = SettingData.minVol;

        slider_master.maxValue = SettingData.maxVol;
        slider_bgm.maxValue = SettingData.maxVol;
        slider_sfx.maxValue = SettingData.maxVol;

        // add event listener
        slider_master.onValueChanged.AddListener((float value) => { settingManager.GetSettingData().VolMaster = value; settingManager.Apply(); });
        slider_bgm.onValueChanged.AddListener((float value)    => { settingManager.GetSettingData().VolBGM    = value; settingManager.Apply(); });
        slider_sfx.onValueChanged.AddListener((float value)    => { settingManager.GetSettingData().VolSFX    = value; settingManager.Apply(); });

        toggle_master.onValueChanged.AddListener((bool value) => { settingManager.GetSettingData().PlayMaster = value; settingManager.Apply(); });
        toggle_bgm.onValueChanged.AddListener((bool value)    => { settingManager.GetSettingData().PlayBGM    = value; settingManager.Apply(); });
        toggle_sfx.onValueChanged.AddListener((bool value)    => { settingManager.GetSettingData().PlaySFX    = value; settingManager.Apply(); });

        btn_back.onClick.AddListener(() => { settingManager.Save(); StepOut(); });
    }

    private void OnEnable()
    {
        SettingData sd = settingManager.GetSettingData();

        slider_master.value = sd.VolMaster;
        slider_bgm.value    = sd.VolBGM;
        slider_sfx.value    = sd.VolSFX;

        toggle_master.isOn = sd.PlayMaster;
        toggle_bgm.isOn    = sd.PlayBGM;
        toggle_sfx.isOn    = sd.PlaySFX;
    }
}
