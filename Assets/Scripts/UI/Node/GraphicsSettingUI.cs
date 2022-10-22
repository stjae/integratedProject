using System;
using UnityEngine;
using TMPro;

using Poly.UI;
using Poly.Data;

public class GraphicsSettingUI : UINode
{
    // data manager
    private SettingManager settingManager;

    // UI (assign in inspector)
    public TMP_Dropdown dropdown_resolution;
    public UnityEngine.UI.Toggle toggle_fullScreen;
    public UnityEngine.UI.Toggle toggle_vsync;

    public UnityEngine.UI.Button btn_back;

    private void Awake()
    {
        settingManager = FindObjectOfType<SettingManager>();

        // prepare dropdown options
        dropdown_resolution.ClearOptions();
        foreach(SettingData.ResolutionOption ro in Enum.GetValues(typeof(SettingData.ResolutionOption)))
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            switch(ro)
            {
                case SettingData.ResolutionOption.FitToDisplay:
                    option.text = string.Format("Fit to Display ({0}x{1})", Screen.mainWindowDisplayInfo.width, Screen.mainWindowDisplayInfo.height);
                    break;
                case SettingData.ResolutionOption.FHD:
                    option.text = "1920x1080";
                    break;
                case SettingData.ResolutionOption.HD:
                    option.text = "1280x720";
                    break;
                case SettingData.ResolutionOption.SVGA:
                    option.text = "800x600";
                    break;
                case SettingData.ResolutionOption.VGA:
                    option.text = "640x480";
                    break;
                default:
                    continue;
            }

            dropdown_resolution.options.Add(option);
        }

        // add event listener
        dropdown_resolution.onValueChanged.AddListener((int call) => { settingManager.GetSettingData().Resolution = (SettingData.ResolutionOption)call; settingManager.Apply(); });

        toggle_fullScreen.onValueChanged.AddListener((bool value) => { settingManager.GetSettingData().FullScreen = value; settingManager.Apply(); });
        toggle_vsync.onValueChanged.AddListener((bool value)      => { settingManager.GetSettingData().VSync      = value; settingManager.Apply(); });

        btn_back.onClick.AddListener(() => { settingManager.Save(); StepOut(); });
    }

    private void OnEnable()
    {
        SettingData sd = settingManager.GetSettingData();

        dropdown_resolution.value = (int)sd.Resolution;
        toggle_fullScreen.isOn    = sd.FullScreen;
        toggle_vsync.isOn         = sd.VSync;
    }
}
