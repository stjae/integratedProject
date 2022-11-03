using UnityEngine;
using UnityEngine.SceneManagement;

using Poly.Data;

public sealed class SceneLoader : MonoBehaviour
{
    public static void LoadSceneWithSaveData()
    {
        // popup UI
        OkPopup okPopup = FindObjectOfType<OkPopup>(true);

        // retrieve current loaded savedata
        SaveData sd = FindObjectOfType<SaveManager>().GetSaveData();

        if(sd.Chapter == 0)
        {
            switch(sd.Level)
            {
                case 0:
                    SceneManager.LoadScene("ch0_level0");
                    break;
            }
        }
        else if(sd.Chapter == 1)
        {
            switch(sd.Level)
            {
                case 0:
                    SceneManager.LoadScene("ch1_level0");
                    break;
            }
        }
        else
        {
            // if failed to load scene, open popup
            okPopup.Open("Failed", "Invalid save data");
        }
    }
}
