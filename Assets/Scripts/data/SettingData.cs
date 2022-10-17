using UnityEngine;
using JsonDataType;

[System.Serializable]
public class SettingData
{
    // volume min/max: [0.01, 1] [100 * %]
    public const float minVol = 0.01f;
    public const float maxVol = 1.0f;

    // graphics
    private ivec2 resolution; // x = width, y = height
    private bool vSync;

    // audio
    // volume range: [0.01, 1] [100 * %]
    private float volMaster;
    private float volBGM;
    private float volSFX;

    private bool playMaster;
    private bool playBGM;
    private bool playSFX;

    private float ClampVolume(float vol) { return Mathf.Clamp(vol, minVol, maxVol); }

    // get, set
    public ivec2 Resolution { get { return resolution; } set { resolution.X = Mathf.Max(value.X, 0); resolution.Y = Mathf.Max(value.Y, 0); } }
    public bool  VSync      { get { return vSync; } set { vSync = value; } }

    public float VolMaster { get { return volMaster; } set { volMaster = ClampVolume(value); } }
    public float VolBGM    { get { return volBGM;    } set { volBGM    = ClampVolume(value); } }
    public float VolSFX    { get { return volSFX;    } set { volSFX    = ClampVolume(value); } }

    public bool PlayMaster { get { return playMaster; } set { playMaster = value; } }
    public bool PlayBGM    { get { return playBGM;    } set { playBGM    = value; } }
    public bool PlaySFX    { get { return playSFX;    } set { playSFX    = value; } }

    public SettingData()
    {
        // graphics
        resolution = new ivec2(Screen.width, Screen.height); // retrieve display information
        vSync = false;

        // audio
        // default volume: 1 [100 * %]
        volMaster = maxVol;
        volBGM    = maxVol;
        volSFX    = maxVol;

        playMaster = true;
        playBGM    = true;
        playSFX    = true;
    }
}