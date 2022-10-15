using System;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Range(1f, 1000f)]
    float _sensitivity = 600f;

    float _xRotateAngle = 0f;
    float _yRotateAngle = 0f;

    Vector3 _xRotateVector;
    Vector3 _yRotateVector;

    Vector3 _smoothedVector;
    Vector3 _velocityVector = Vector3.zero;

    GameObject _player;
    GameObject _playerModel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Awake()
    {
        _player = transform.GetComponentInParent<Player>().gameObject;
        _playerModel = _player.transform.Find("Model").gameObject;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime; // horizontal mouse movement
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity * Time.deltaTime; // vertical mouse movement

        _xRotateAngle -= mouseY;
        _xRotateAngle = Mathf.Clamp(_xRotateAngle, -90f, 90f);

        _yRotateAngle += mouseX;

        _xRotateVector = Vector3.right * _xRotateAngle;
        _yRotateVector = Vector3.up * _yRotateAngle;

        Vector3 rotateVector = _xRotateVector + _yRotateVector;
        _smoothedVector = Vector3.SmoothDamp(_smoothedVector, rotateVector, ref _velocityVector, 0.02f);

        _playerModel.transform.rotation = Quaternion.Euler(0f, _smoothedVector.y, 0f);
        transform.localRotation = Quaternion.Euler(_smoothedVector);
    }
}
