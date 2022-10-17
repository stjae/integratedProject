public class GraphicsSettingUI : UINode
{
    // UI (assign in inspector)
    public UnityEngine.UI.Button btn_back;

    private void Awake()
    {
        // add event listener
        btn_back.onClick.AddListener(StepOut);
    }
}
