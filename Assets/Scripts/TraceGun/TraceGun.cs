using UnityEngine;
using System;
using System.Linq;

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

    GameObject _frontFaceObj;
    GameObject _backFaceObj;
    Mesh _frontFaceMesh;
    Mesh _backFaceMesh;

    int _traceFaceLayer;
    public int TraceFaceLayer { get { return _traceFaceLayer; } }

    const int VERTEX_PER_TRI = 3;

    public void Fire()
    {
        Vector3 frontCenterPosition = Vector3.zero;
        Vector3 backCenterPosition = Vector3.zero;
        int frontHitCount;
        int backHitCount;
        // InstantiateObject();

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
        _ray.BackHit[_ray.BackHit.Count - 1] = _backCenterHit;
        _ray.FrontHitPoint[_ray.FrontHitPoint.Count - 1] = _frontCenterHit.point;
        _ray.BackHitPoint[_ray.BackHitPoint.Count - 1] = _backCenterHit.point;
        _ray.IsRayHitFront[_ray.IsRayHitFront.Count - 1] = _isFrontCenterHit;
        _ray.IsRayHitBack[_ray.IsRayHitBack.Count - 1] = _isBackCenterHit;

        CreateTriangle();
        CreateShape();
    }

    void InstantiateObject()
    {
        for (int i = 0; i < _ray.RayCount; i++)
        {
            GameObject _frontTrace = Instantiate(_sphere.FrontSphere[i], _ray.FrontHit[i].point, Quaternion.identity);
            GameObject _backTrace = Instantiate(_sphere.BackSphere[i], _ray.BackHit[i].point, Quaternion.identity);

            Destroy(_frontTrace, 2f);
            Destroy(_backTrace, 2f);
        }
    }

    void CreateTriangle()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < _ray.VertexCount; j++)  // if first vertex of triangle hit
            {
                if (_ray.IsRayHitFront[_ray.RayIndex[string.Format("{0}{1}", i, j)]])
                {
                    AssembleVertex("front", i, j);
                }
                else
                {
                    ClearTriangle("front", i, j, 0, 6);
                }

                if (_ray.IsRayHitBack[_ray.RayIndex[string.Format("{0}{1}", i, j)]])
                {
                    AssembleVertex("back", i, j);
                }
                else
                {
                    ClearTriangle("back", i, j, 0, 6);
                }
            }
        }
    }

    void AssembleVertex(string side, int radiusIdx, int vertexIdx)
    {
        for (int triVertexIdx = 0; triVertexIdx < VERTEX_PER_TRI * 2; triVertexIdx++)
        {
            switch (triVertexIdx)
            {
                case 0:
                    SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx)]);
                    break;
                case 1:
                    if (radiusIdx < 3)
                    {
                        int rayIdx = (vertexIdx == 0) ? _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, _ray.VertexCount - 1)] : _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx - 1)];
                        if (IsHit(side, rayIdx))
                        {
                            SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx);
                        }
                        else
                        {
                            int multiplier = 0;
                            while (rayIdx + _ray.VertexCount * multiplier < _ray.RayCount)
                            {
                                if (IsHit(side, rayIdx + _ray.VertexCount * multiplier))
                                {
                                    SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx + _ray.VertexCount * multiplier);
                                    break;
                                }
                                else if (rayIdx + _ray.VertexCount * (multiplier + 1) >= _ray.RayCount)
                                {
                                    ClearTriangle(side, radiusIdx, vertexIdx, 0, 3);
                                }
                                multiplier++;
                            }
                        }
                    }
                    break;
                case 2:
                    if (radiusIdx < 3)
                    {
                        if (IsHit(side, _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)]))
                            SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * 6 + triVertexIdx, _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)]);
                        else
                            ClearTriangle(side, radiusIdx, vertexIdx, 0, 3);
                    }
                    break;

                case 3:
                    SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx)]);
                    break;
                case 4:
                    int rayIdx1 = (radiusIdx < 3) ? _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)] : _ray.FrontHit.Count - 1;
                    if (IsHit(side, rayIdx1))
                        SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * VERTEX_PER_TRI * 2 + triVertexIdx, rayIdx1);
                    else
                    {
                        ClearTriangle(side, radiusIdx, vertexIdx, 3, 6);
                    }
                    break;
                case 5:
                    int rayIdx2 = (vertexIdx == _ray.VertexCount - 1) ? _ray.RayIndex[string.Format("{0}{1}", radiusIdx, 0)] : _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx + 1)]; // 01
                    if (IsHit(side, rayIdx2))
                    {
                        SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx2);
                    }
                    else
                    {
                        int multiplier1 = 0;
                        while (rayIdx2 + _ray.VertexCount * multiplier1 < _ray.RayCount)
                        {
                            if (IsHit(side, rayIdx2 + _ray.VertexCount * multiplier1))
                            {
                                SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx2 + _ray.VertexCount * multiplier1);
                                break;
                            }
                            else if (rayIdx2 + _ray.VertexCount * (multiplier1 + 1) >= _ray.RayCount)
                            {
                                ClearTriangle(side, radiusIdx, vertexIdx, 3, 6);
                            }
                            multiplier1++;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void SetTriangle(string side, int vertexIdx, int rayIdx)
    {
        if (side == "front")
        {
            _ray.FrontTriangle[vertexIdx] = rayIdx;
        }
        else if (side == "back")
        {
            _ray.BackTriangle[vertexIdx] = rayIdx;
        }
    }

    bool IsHit(string side, int rayIdx)
    {
        if (side == "front")
        {
            return _ray.IsRayHitFront[rayIdx];
        }
        else if (side == "back")
        {
            return _ray.IsRayHitBack[rayIdx];
        }
        else
        {
            throw new ArgumentException();
        }
    }

    void ClearTriangle(string side, int radiusIdx, int vertexIdx, int a, int b)
    {
        for (int triVertexIdx = a; triVertexIdx < b; triVertexIdx++)
        {
            SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, 0);
        }
    }

    void CreateShape()
    {
        _frontFaceMesh.Clear();
        _frontFaceMesh.vertices = _ray.FrontHitPoint.ToArray();
        _frontFaceMesh.triangles = _ray.FrontTriangle.ToArray();

        _backFaceMesh.Clear();
        _backFaceMesh.vertices = _ray.BackHitPoint.ToArray();
        _backFaceMesh.triangles = _ray.BackTriangle.ToArray();
        _backFaceMesh.triangles = _backFaceMesh.triangles.Reverse().ToArray();
    }

    void Start()
    {
        _cam = transform.GetComponentInParent<Player>().Camera;
        _ray = gameObject.AddComponent<Ray>();
        _sphere = gameObject.AddComponent<Sphere>();

        _traceFaceLayer = LayerMask.NameToLayer("TraceFace");

        _frontFaceObj = new GameObject("FrontFace");
        _backFaceObj = new GameObject("BackFace");
        _frontFaceObj.layer = _traceFaceLayer;
        _backFaceObj.layer = _traceFaceLayer;
        _frontFaceMesh = new Mesh();
        _backFaceMesh = new Mesh();
        _frontFaceObj.AddComponent<MeshFilter>();
        _backFaceObj.AddComponent<MeshFilter>();
        _frontFaceObj.AddComponent<MeshRenderer>();
        _backFaceObj.AddComponent<MeshRenderer>();
        _frontFaceObj.GetComponent<MeshFilter>().mesh = _frontFaceMesh;
        _backFaceObj.GetComponent<MeshFilter>().mesh = _backFaceMesh;

        MeshRenderer frontFaceMeshRenderer = _frontFaceObj.GetComponent<MeshRenderer>();
        MeshRenderer backFaceMeshRenderer = _backFaceObj.GetComponent<MeshRenderer>();
        frontFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        backFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));

        Renderer frontFaceRenderer = _frontFaceObj.GetComponent<Renderer>();
        Renderer backFaceRenderer = _backFaceObj.GetComponent<Renderer>();
        Color yellow = Color.yellow;
        Color green = Color.green;
        frontFaceRenderer.material.SetColor("_Color", new Color(yellow.r, yellow.g, yellow.b, 0.5f));
        backFaceRenderer.material.SetColor("_Color", new Color(green.r, green.g, green.b, 0.5f));
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

    void PrintTriangleVertex()
    {
        string str = "";
        int count = 0;

        foreach (var item in _ray.FrontTriangle)
        {
            str += item + " ";
            count++;

            if (count % 3 == 0)
                str += "| ";
        }

        Debug.Log(str);
    }
}