using UnityEngine;
using System;

public class TraceGun : MonoBehaviour
{
    [Range(0.1f, 2.0f)]
    [SerializeField] float _radius = 0.2f;
    [Range(1, 45)]
    [SerializeField] int _angle = 45;

    Ray _ray;
    Triangle _triangle;
<<<<<<< HEAD
    Sphere _sphere;

    public void Fire()
    {
        _triangle.CreateTriangle();
        _triangle.CreateShape();
        _sphere.CreateTracePoint();
=======

    public void Fire()
    {
        _ray.GetRayHitCenter();

        _triangle.CreateTriangle();
        _triangle.CreateShape();
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
    }

    void Start()
    {
        _ray = gameObject.AddComponent<Ray>();
        _triangle = gameObject.AddComponent<Triangle>();
<<<<<<< HEAD
        _sphere = gameObject.AddComponent<Sphere>();
=======

        gameObject.AddComponent<Sphere>();
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
    }

    void Update()
    {
        _radius -= Input.mouseScrollDelta.y / 10;
        _radius = Mathf.Clamp(_radius, 0.5f, 2.0f);

        _angle = (Mathf.CeilToInt(_radius * 20));
        _angle = Mathf.Clamp(_angle, 10, 45);

        _ray.Radius = _radius;

        if (_ray.Angle != _angle && 360 % _angle == 0)
        {
            _ray.RayCount = 4 * (360 / _angle);
            _ray.VertexCount = 360 / _angle;
            _ray.Angle = _angle;
        }
    }
}