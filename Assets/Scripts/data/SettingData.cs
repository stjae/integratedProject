using UnityEngine;

[System.Serializable]
public class SettingData : Data
{
    // graphics
    ivec2 resolution; // x = width, y = height
    bool vSync;

    // audio
    // volume range: [-80, 20] [dB]
    float volMaster;
    float volBGM;
    float volSFX;

    bool muteMaster;
    bool muteBGM;
    bool muteSFX;

    private float ClampVolume(float vol) { return Mathf.Clamp(vol, -80.0f, 20.0f); }

    // get, set
    public ivec2 Resolution { get { return resolution; } set { resolution.X = Mathf.Max(value.X, 0); resolution.Y = Mathf.Max(value.Y, 0); } }
    public bool  VSync      { get { return vSync; } set { vSync = value; } }

    public float VolMaster { get { return volMaster; } set { volMaster = ClampVolume(value); } }
    public float VolBGM    { get { return volBGM;    } set { volBGM    = ClampVolume(value); } }
    public float VolSFX    { get { return volSFX;    } set { volSFX    = ClampVolume(value); } }

    public bool MuteMaster { get { return muteMaster; } set { muteMaster = value; } }
    public bool MuteBGM    { get { return muteBGM;    } set { muteBGM    = value; } }
    public bool MuteSFX    { get { return muteSFX;    } set { muteSFX    = value; } }

    public override void Init()
    {
        // graphics
        resolution = new ivec2(Screen.width, Screen.height); // retrieve display information
        vSync = false;

        // audio
        // default volume: 0 [dB]
        volMaster = 0.0f;
        volBGM = 0.0f;
        volSFX = 0.0f;

        muteMaster = false;
        muteBGM = false;
        muteSFX = false;
    }
}