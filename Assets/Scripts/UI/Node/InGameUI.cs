using UnityEngine;

using Poly.UI;

public class InGameUI : UINode
{
    // UINode
    private UINode inGameUI;

    private void Awake()
    {
        // link UINode
        inGameUI = FindObjectOfType<PausedUI>(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StepInto(inGameUI);
        }
    }
}