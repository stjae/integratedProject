using UnityEngine;

namespace Poly.UI
{
    public class UIPopup : MonoBehaviour
    {
        private string title;
        private string message;

        // get, set
        public string Title   { get { return title;   } set { title = value;   } }
        public string Message { get { return message; } set { message = value; } }

        public void Open(string title, string message)
        {
            this.title   = title;
            this.message = message;

            gameObject.SetActive(true);
        }

        public void Close()
        {
            title   = null;
            message = null;

            gameObject.SetActive(false);
        }
    }
}