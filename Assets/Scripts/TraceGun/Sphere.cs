using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    Ray _ray;
    FirstPersonCamera _cam;

    GameObject _sphere;
    GameObject _redSphere;
    GameObject _blueSphere;
    GameObject _whiteTraceSphere;
    public GameObject RedSphere { get { return _redSphere; } }
    public GameObject BlueSphere { get { return _blueSphere; } }

    GameObject _frontCenterSphere;
    GameObject _backCenterSphere;
    List<GameObject> _frontSphere;
    List<GameObject> _backSphere;
    public List<GameObject> FrontSphere { get { return _frontSphere; } }
    public List<GameObject> BackSphere { get { return _backSphere; } }

    List<Vector3> _tracePoint;
    List<GameObject> _traceSphere;

    int _count;
    public int Count { get { return _count; } set { _count = value; } }

    Coroutine coroutine;

    void Start()
    {
        _ray = gameObject.GetComponent<Ray>();
        _cam = transform.GetComponentInParent<Player>().Camera;

        _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphere.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        _sphere.layer = Layer.HitSphere;

        _whiteTraceSphere = Instantiate(_sphere, Vector3.zero, Quaternion.identity);
        _tracePoint = new List<Vector3>();
        _traceSphere = new List<GameObject>();

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
        for (int i = 0; i < _ray.VertexCount; i++)
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
                // _frontSphere[i].SetActive(false);
                _frontSphere[i].transform.position = Vector3.Lerp(_frontSphere[i].transform.position, _cam.transform.TransformPoint(_ray.RayOriginFront[i] + Vector3.forward * 5), Time.deltaTime * 16f);
            }
            // if (_ray.IsRayHitBack[i])
            // {
            //     if (!_backSphere[i].activeSelf)
            //     {
            //         _backSphere[i].SetActive(true);
            //         _backSphere[i].transform.position = _ray.BackHit[i].point;
            //     }
            //     else
            //     {
            //         _backSphere[i].SetActive(true);
            //         _backSphere[i].transform.position = Vector3.Lerp(_backSphere[i].transform.position, _ray.BackHit[i].point, Time.deltaTime * 16f);
            //     }
            // }
            // else
            // {
            //     _backSphere[i].SetActive(false);
            // }
        }
    }

    public void CreateTracePoint()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        _tracePoint.Clear();
        GameObject[] trace = GameObject.FindGameObjectsWithTag("trace");
        if (trace.Length > 0)
        {
            foreach (GameObject obj in trace)
            {
                Destroy(obj);
            }
        }

        for (int i = 0; i < _ray.RayCount; i++)
        {
            RaycastHit frontHit;
            RaycastHit backHit;
            float random = Random.Range(-0.05f, 0.05f);
            for (int j = 0; j < 5; j++)
            {
                bool isHit = Physics.Raycast(_cam.transform.TransformPoint(_ray.RayOriginFront[i]) - _cam.transform.forward, _cam.transform.forward + Vector3.right * random + Vector3.up * random, out frontHit, 10f, 1 << Layer.TraceFace);
                if (isHit)
                    _tracePoint.Add(frontHit.point);
                bool isHit2 = Physics.Raycast(frontHit.point, _cam.transform.forward + Vector3.right * random + Vector3.up * random, out backHit, 10f, 1 << Layer.TraceFace);
                if (isHit2)
                    _tracePoint.Add(backHit.point);
            }
        }

        if (_tracePoint.Count > 0)
            coroutine = StartCoroutine(CreateTrace(0));
    }

    IEnumerator CreateTrace(int index)
    {
        Renderer sphereRenderer;
        GameObject whiteTrace = Instantiate(_whiteTraceSphere, _tracePoint[index], Quaternion.identity);
        whiteTrace.tag = "trace";
        sphereRenderer = whiteTrace.GetComponent<Renderer>();
        sphereRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));

        yield return new WaitForSeconds(0.0001f);

        if (index < _tracePoint.Count - 1)
            StartCoroutine(CreateTrace(index + 1));
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
