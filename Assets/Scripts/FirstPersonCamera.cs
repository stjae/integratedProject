using System;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Range(1f, 1000f)]
    float _sensitivity = 600f;
    float _xRotation = 0f;

    GameObject player;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Awake()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        player.transform.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}
