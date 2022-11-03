using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class TextMessageUI : MonoBehaviour
{
    public TextMeshProUGUI text_message;

    public async void PrintMessage(string message, int duration)
    {
        text_message.text = message;

        gameObject.SetActive(true);
        await Task.Delay(duration);
        gameObject.SetActive(false);
    }
}
