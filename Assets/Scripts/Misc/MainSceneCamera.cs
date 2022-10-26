using UnityEngine;

namespace Poly.Misc
{
    public class MainSceneCamera : MonoBehaviour
    {
        public bool isIncreasing;
        public float angle;
        public float maxAngle = 26.0f;

        private void Awake()
        {
            isIncreasing = true;
            angle = 0.0f;
        }

        private void LateUpdate()
        {
            if (isIncreasing)
            {
                angle += Time.deltaTime;
                transform.Rotate(Vector3.up, Time.deltaTime);
                if (angle >= maxAngle) { isIncreasing = false; }
            }
            else
            {
                angle -= Time.deltaTime;
                transform.Rotate(Vector3.up, -Time.deltaTime);
                if (angle <= 0.0f) { isIncreasing = true; }
            }
        }
    }
}