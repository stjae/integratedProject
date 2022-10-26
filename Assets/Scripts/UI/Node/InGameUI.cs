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

    // start or resume gameplay
    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1.0f;
    }

    // pause gameplay
    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0.0f;
    }
}