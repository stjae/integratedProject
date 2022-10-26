using UnityEngine;

using Poly.UI;

public class SaveDataListUI : UINode
{
    // UI (assign in inspector)
    public SaveDataSlotUI slot0;
    public SaveDataSlotUI slot1;
    public SaveDataSlotUI slot2;

    public UnityEngine.UI.Button btn_back;

    private void Awake()
    {
        // set filepath of slot
        slot0.SaveDataFilename = "save_00";
        slot1.SaveDataFilename = "save_01";
        slot2.SaveDataFilename = "save_02";

        // add event listener
        btn_back.onClick.AddListener(StepOut);
    }
}
