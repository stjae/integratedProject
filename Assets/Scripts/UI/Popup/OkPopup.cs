using TMPro;

using Poly.UI;

public class OkPopup : UIPopup
{
    // UI (assign in inspector)
    public TextMeshProUGUI       text_title;
    public TextMeshProUGUI       text_message;
    public UnityEngine.UI.Button btn_ok;

    private void Awake()
    {
        // add event listener
        btn_ok.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        text_title.text   = base.Title;
        text_message.text = base.Message;
    }
}
