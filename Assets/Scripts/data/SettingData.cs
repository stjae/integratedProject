using UnityEngine;

namespace Poly.Data
{
    [System.Serializable]
    public class SettingData
    {
        public enum ResolutionOption
        {
            FitToDisplay,
            FHD,  // 1920x1080
            HD,   // 1280x720
            SVGA, // 800x600
            VGA   // 640x480
        };

        // volume min/max: [0.01, 1] [100 * %]
        public const float minVol = 0.01f;
        public const float maxVol = 1.0f;

        // graphics
        private ResolutionOption resolution;
        private bool fullScreen;
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
        public ResolutionOption Resolution { get { return resolution; } set { resolution = value; } }
        public bool FullScreen { get { return fullScreen; } set { fullScreen = value; } }
        public bool VSync { get { return vSync; } set { vSync = value; } }

        public float VolMaster { get { return volMaster; } set { volMaster = ClampVolume(value); } }
        public float VolBGM { get { return volBGM; } set { volBGM = ClampVolume(value); } }
        public float VolSFX { get { return volSFX; } set { volSFX = ClampVolume(value); } }

        public bool PlayMaster { get { return playMaster; } set { playMaster = value; } }
        public bool PlayBGM { get { return playBGM; } set { playBGM = value; } }
        public bool PlaySFX { get { return playSFX; } set { playSFX = value; } }

        public SettingData()
        {
            // graphics
            resolution = ResolutionOption.FitToDisplay;
            fullScreen = true;
            vSync = false;

            // audio
            // default volume: 1 [100 * %]
            volMaster = maxVol;
            volBGM = maxVol;
            volSFX = maxVol;

            playMaster = true;
            playBGM = true;
            playSFX = true;
        }
    }
}