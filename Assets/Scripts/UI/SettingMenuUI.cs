public class SettingMenuUI : UINode
{
    // UINode
    private UINode GraphicsSetting;
    private UINode AudioSetting;

    // UI (assign in inspector)
    public UnityEngine.UI.Button btn_graphics;
    public UnityEngine.UI.Button btn_audio;
    public UnityEngine.UI.Button btn_back;

    private void Awake()
    {
        // link UINode
        GraphicsSetting = FindObjectOfType<GraphicsSettingUI>(true);
        AudioSetting    = FindObjectOfType<AudioSettingUI>(true);

        // add event listener
        btn_graphics.onClick.AddListener(() => StepInto(GraphicsSetting));
        btn_audio.onClick.AddListener(() => StepInto(AudioSetting));
        btn_back.onClick.AddListener(StepOut);
    }
}
