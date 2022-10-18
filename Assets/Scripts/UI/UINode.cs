using UnityEngine;

namespace Poly.UI
{
    public class UINode : MonoBehaviour
    {
        private UINode prevNode;

        protected void StepInto(UINode nextNode)
        {
            if (nextNode == this)
            {
                Debug.LogError("UINode can NOT step into itself.");
                return;
            }

            nextNode.prevNode = this;
            nextNode.gameObject.SetActive(true);

            gameObject.SetActive(false);
        }

        protected void StepOut()
        {
            if (prevNode != null)
            {
                prevNode.gameObject.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}