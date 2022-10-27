using UnityEngine;

public class TraceGun : MonoBehaviour
{
    [Range(0.1f, 2.0f)]
    [SerializeField] float _radius = 0.2f;
    [Range(1, 45)]
    [SerializeField] int _angle = 45;

    FirstPersonCamera _cam;
    Ray _ray;
    Sphere _sphere;

    RaycastHit _frontCenterHit;
    RaycastHit _backCenterHit;
    bool _isFrontCenterHit;
    bool _isBackCenterHit;

    GameObject emptyObj;
    Mesh mesh;

    public void Fire()
    {
        Vector3 frontCenterPosition = Vector3.zero;
        Vector3 backCenterPosition = Vector3.zero;
        int frontHitCount;
        int backHitCount;
        // for (int i = 0; i < _ray.RayCount; i++)
        // {
        //     GameObject _frontTrace = Instantiate(_sphere.FrontSphere[i], _ray.FrontHit[i].point, Quaternion.identity);
        //     GameObject _backTrace = Instantiate(_sphere.BackSphere[i], _ray.BackHit[i].point, Quaternion.identity);

        //     Destroy(_frontTrace, 2f);
        //     Destroy(_backTrace, 2f);
        // }

        int firstIndex = 0;
        int lastIndex = _ray.RayCount - 1;
        bool isFrontCenterPositionSet = false;
        bool isBackCenterPositionSet = false;

        for (int j = 0; j < 4; j++)
        {
            frontHitCount = 0;
            backHitCount = 0;
            bool isHitFrontCenter = false;
            bool isHitBackCenter = false;

            for (int i = lastIndex; i > lastIndex - _ray.VertexCount; i--)
            {
                if (_ray.IsRayHitFront[i] && !isFrontCenterPositionSet)
                {
                    isHitFrontCenter = true;
                    frontCenterPosition += _ray.FrontHit[i].point;
                    frontHitCount++;
                }
                if (_ray.IsRayHitBack[i] && !isBackCenterPositionSet)
                {
                    isHitBackCenter = true;
                    backCenterPosition += _ray.BackHit[i].point;
                    backHitCount++;
                }

                firstIndex = i;
            }
            lastIndex = firstIndex - 1;

            if (isHitFrontCenter && !isFrontCenterPositionSet)
            {
                frontCenterPosition.x /= frontHitCount;
                frontCenterPosition.y /= frontHitCount;
                frontCenterPosition.z /= frontHitCount;

                isFrontCenterPositionSet = true;
            }
            if (isHitBackCenter && !isBackCenterPositionSet)
            {
                backCenterPosition.x /= backHitCount;
                backCenterPosition.y /= backHitCount;
                backCenterPosition.z /= backHitCount;

                isBackCenterPositionSet = true;
            }
        }

        _isFrontCenterHit = Physics.Raycast(_cam.transform.position, frontCenterPosition - _cam.transform.position, out _frontCenterHit, 10f, (-1) - (1 << _ray.TraceLayer | 1 << _ray.PlayerLayer));
        _isBackCenterHit = Physics.Raycast(_cam.transform.TransformPoint(Vector3.forward * 10), backCenterPosition - _cam.transform.TransformPoint(Vector3.forward * 10), out _backCenterHit, 10f - _frontCenterHit.distance, (-1) - (1 << _ray.TraceLayer | 1 << _ray.PlayerLayer));

        _ray.FrontHit[_ray.FrontHit.Count - 1] = _frontCenterHit;
        _ray.FrontHitPoint[_ray.FrontHitPoint.Count - 1] = _frontCenterHit.point;
        _ray.IsRayHitFront[_ray.IsRayHitFront.Count - 1] = _isFrontCenterHit;

        // GameObject frontCenterTrace = Instantiate(_sphere.RedSphere, frontCenterHit.point, Quaternion.identity);
        // Destroy(frontCenterTrace, 2f);
        // GameObject backCenterTrace = Instantiate(_sphere.BlueSphere, backCenterHit.point, Quaternion.identity);
        // Destroy(backCenterTrace, 2f);

        CreateTriangle();
        CreateShape();
    }

    void CreateTriangle()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < _ray.VertexCount; j++)
            {
                if (_ray.IsRayHitFront[_ray.RayIndex[string.Format("{0}{1}", i, j)]])
                {
                    for (int k = 0; k < 6; k++)
                    {
                        switch (k)
                        {
                            case 0:
                                _ray.Triangle[(i * _ray.VertexCount + j) * 6 + k] = _ray.RayIndex[string.Format("{0}{1}", i, j)];
                                break;
                            case 1:
                                if (i < 3)
                                {
                                    int index = (j == 0) ? _ray.RayIndex[string.Format("{0}{1}", i + 1, _ray.VertexCount - 1)] : _ray.RayIndex[string.Format("{0}{1}", i + 1, j - 1)];
                                    if (_ray.IsRayHitFront[index])
                                    {
                                        _ray.Triangle[((i * _ray.VertexCount + j) * 6 + k)] = index;
                                    }
                                    else
                                    {
                                        int increment = 0;
                                        while (index + _ray.VertexCount * increment < _ray.RayCount)
                                        {
                                            if (_ray.IsRayHitFront[index + _ray.VertexCount * increment])
                                            {
                                                _ray.Triangle[(i * _ray.VertexCount + j) * 6 + k] = index + _ray.VertexCount * increment;
                                                break;
                                            }
                                            else if (index + _ray.VertexCount * (increment + 1) >= _ray.RayCount)
                                            {
                                                ClearTriangle(i, j, 0, 3);
                                            }
                                            increment++;
                                        }
                                    }
                                }
                                break;
                            case 2:
                                if (i < 3)
                                {
                                    if (_ray.IsRayHitFront[_ray.RayIndex[string.Format("{0}{1}", i + 1, j)]])
                                        _ray.Triangle[(i * _ray.VertexCount + j) * 6 + k] = _ray.RayIndex[string.Format("{0}{1}", i + 1, j)];
                                    else
                                        ClearTriangle(i, j, 0, 3);
                                }
                                break;

                            case 3:
                                _ray.Triangle[(i * _ray.VertexCount + j) * 6 + k] = _ray.RayIndex[string.Format("{0}{1}", i, j)]; // 00
                                break;
                            case 4:
                                int idx4 = (i < 3) ? _ray.RayIndex[string.Format("{0}{1}", i + 1, j)] : _ray.FrontHit.Count - 1; // 10
                                if (_ray.IsRayHitFront[idx4])
                                    _ray.Triangle[(i * _ray.VertexCount + j) * 6 + k] = idx4;
                                else
                                {
                                    ClearTriangle(i, j, 3, 6);
                                }
                                break;
                            case 5:
                                int idx = (j == _ray.VertexCount - 1) ? _ray.RayIndex[string.Format("{0}{1}", i, 0)] : _ray.RayIndex[string.Format("{0}{1}", i, j + 1)]; // 01
                                if (_ray.IsRayHitFront[idx])
                                {
                                    _ray.Triangle[((i * _ray.VertexCount + j) * 6 + k)] = idx;
                                }
                                else
                                {
                                    int inc = 0;
                                    while (idx + _ray.VertexCount * inc < _ray.RayCount)
                                    {
                                        if (_ray.IsRayHitFront[idx + _ray.VertexCount * inc])
                                        {
                                            _ray.Triangle[(i * _ray.VertexCount + j) * 6 + k] = idx + _ray.VertexCount * inc;
                                            break;
                                        }
                                        else if (idx + _ray.VertexCount * (inc + 1) >= _ray.RayCount)
                                        {
                                            ClearTriangle(i, j, 3, 6);
                                        }
                                        inc++;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    ClearTriangle(i, j, 0, 6);
                }
            }
        }

        // string str = "";
        // int count = 0;

        // foreach (var item in _ray.Triangle)
        // {
        //     str += item + " ";
        //     count++;

        //     if (count % 3 == 0)
        //         str += "| ";
        // }

        // Debug.Log(str);
    }

    void ClearTriangle(int i, int j, int a, int b)
    {
        for (int k = a; k < b; k++)
        {
            _ray.Triangle[(i * _ray.VertexCount + j) * 6 + k] = 0;
        }
    }

    void CreateShape()
    {
        mesh.Clear();
        mesh.vertices = _ray.FrontHitPoint.ToArray();
        mesh.triangles = _ray.Triangle.ToArray();
    }

    void Start()
    {
        _cam = transform.GetComponentInParent<Player>().Camera;
        _ray = gameObject.AddComponent<Ray>();
        _sphere = gameObject.AddComponent<Sphere>();

        emptyObj = new GameObject("EmptyObject");
        mesh = new Mesh();
        emptyObj.AddComponent<MeshFilter>();
        emptyObj.AddComponent<MeshRenderer>();
        emptyObj.GetComponent<MeshFilter>().mesh = mesh;

        MeshRenderer meshRenderer = emptyObj.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));

        Renderer renderer = emptyObj.GetComponent<Renderer>();
        Color oldColor = Color.yellow;
        renderer.material.SetColor("_Color", new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f));
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