using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    Ray _ray;

    GameObject _sphere;
    GameObject _redSphere;
    GameObject _blueSphere;
    public GameObject RedSphere { get { return _redSphere; } }
    public GameObject BlueSphere { get { return _blueSphere; } }

    GameObject _frontCenterSphere;
    GameObject _backCenterSphere;
    List<GameObject> _frontSphere;
    List<GameObject> _backSphere;
    public List<GameObject> FrontSphere { get { return _frontSphere; } }
    public List<GameObject> BackSphere { get { return _backSphere; } }

    int _count;
    public int Count { get { return _count; } set { _count = value; } }

    void Start()
    {
        _ray = gameObject.GetComponent<Ray>();

        _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        _sphere.layer = Layer.HitSphere;

        _redSphere = Instantiate(_sphere, Vector3.zero, Quaternion.identity);
        Renderer redSphereRenderer = _redSphere.GetComponent<Renderer>();
        redSphereRenderer.material.SetColor("_Color", Color.red);

        _blueSphere = Instantiate(_sphere, Vector3.zero, Quaternion.identity);
        Renderer blueSphereRenderer = _blueSphere.GetComponent<Renderer>();
        blueSphereRenderer.material.SetColor("_Color", Color.blue);

        _frontCenterSphere = Instantiate(_redSphere, Vector3.zero, Quaternion.identity);
        _backCenterSphere = Instantiate(_blueSphere, Vector3.zero, Quaternion.identity);
        _frontSphere = new List<GameObject>();
        _backSphere = new List<GameObject>();
    }

    void UpdateSphereCount()
    {
        while (_frontSphere.Count < _count)
        {
            _frontSphere.Add(Instantiate(_redSphere, Vector3.zero, Quaternion.identity));
            _backSphere.Add(Instantiate(_blueSphere, Vector3.zero, Quaternion.identity));
        }

        while (_frontSphere.Count > _count)
        {
            DestroyImmediate(_frontSphere[_frontSphere.Count - _count]);
            _frontSphere.RemoveAt(_frontSphere.Count - _count);
            DestroyImmediate(_backSphere[_backSphere.Count - _count]);
            _backSphere.RemoveAt(_backSphere.Count - _count);
        }
    }

    void UpdateSpherePosition()
    {
        for (int i = 0; i < _ray.RayCount; i++)
        {
            if (_ray.IsRayHitFront[i])
            {
                if (!_frontSphere[i].activeSelf)
                {
                    _frontSphere[i].SetActive(true);
                    _frontSphere[i].transform.position = _ray.FrontHit[i].point;
                }
                else
                {
                    _frontSphere[i].SetActive(true);
                    _frontSphere[i].transform.position = Vector3.Lerp(_frontSphere[i].transform.position, _ray.FrontHit[i].point, Time.deltaTime * 16f);
                }
            }
            else
            {
                _frontSphere[i].SetActive(false);
            }

            if (_ray.IsRayHitBack[i])
            {
                if (!_backSphere[i].activeSelf)
                {
                    _backSphere[i].SetActive(true);
                    _backSphere[i].transform.position = _ray.BackHit[i].point;
                }
                else
                {
                    _backSphere[i].SetActive(true);
                    _backSphere[i].transform.position = Vector3.Lerp(_backSphere[i].transform.position, _ray.BackHit[i].point, Time.deltaTime * 16f);
                }
            }
            else
            {
                _backSphere[i].SetActive(false);
            }
        }
    }

    public void InstantiateObject()
    {
        for (int i = 0; i < _ray.RayCount; i++)
        {
            GameObject _frontTrace = Instantiate(_frontSphere[i], _ray.FrontHit[i].point, Quaternion.identity);
            GameObject _backTrace = Instantiate(_backSphere[i], _ray.BackHit[i].point, Quaternion.identity);

            Destroy(_frontTrace, 2f);
            Destroy(_backTrace, 2f);
        }
    }

    void LateUpdate()
    {
        UpdateSphereCount();
        UpdateSpherePosition();
    }
}
