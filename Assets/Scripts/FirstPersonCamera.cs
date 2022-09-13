using System;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Serializable]
    public class CameraProperty
    {
        [Range(1f, 1000f)]
        public float sensitivity = 600f;
        [HideInInspector] public float xRotation = 0f;
    }

    public GameObject player;
    public CameraProperty camProp = new CameraProperty();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Awake()
    {
        InitComponent();
    }

    void InitComponent()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * camProp.sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * camProp.sensitivity * Time.deltaTime;

        camProp.xRotation -= mouseY;
        camProp.xRotation = Mathf.Clamp(camProp.xRotation, -90f, 90f);

        player.transform.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(camProp.xRotation, 0f, 0f);
    }
}
