using UnityEngine;

namespace Poly.UI
{
    public class UINode : MonoBehaviour
    {
        private UINode prevNode;

        protected void StepInto(UINode nextNode)
        {
            // UINode should NOT form a loop
            // union find algorithm
            for (UINode node = this; node != null; node = node.prevNode)
            {
                if(node == nextNode)
                {
                    Debug.LogError("UINode should NOT form a loop!");
                    return;
                }
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