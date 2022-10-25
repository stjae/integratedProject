using System.Collections.Generic;
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

    List<Vector3> _vertexPosition;
    List<int> _triangle;

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

            for (int i = lastIndex; i > lastIndex - (360 / _ray.Angle); i--)
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

        RaycastHit frontCenterHit;
        RaycastHit backCenterHit;
        Physics.Raycast(_cam.transform.position, frontCenterPosition - _cam.transform.position, out frontCenterHit, 10f, (-1) - (1 << _ray.TraceLayer | 1 << _ray.PlayerLayer));
        Physics.Raycast(_cam.transform.TransformPoint(Vector3.forward * 10), backCenterPosition - _cam.transform.TransformPoint(Vector3.forward * 10), out backCenterHit, 10f - frontCenterHit.distance, (-1) - (1 << _ray.TraceLayer | 1 << _ray.PlayerLayer));

        // GameObject frontCenterTrace = Instantiate(_sphere.RedSphere, frontCenterHit.point, Quaternion.identity);
        // Destroy(frontCenterTrace, 2f);
        // GameObject backCenterTrace = Instantiate(_sphere.BlueSphere, backCenterHit.point, Quaternion.identity);
        // Destroy(backCenterTrace, 2f);

        CreateTriangle();
        CreateShape();
    }

    void CreateTriangle()
    {
        for (int i = 0; i < _ray.Triangle.Count; i++)
            _ray.Triangle[i] = 0;

        for (int i = 0; i < (360 / _ray.Angle) * 3; i++)
        {
            if (_ray.IsRayHitFront[i])
            {
                for (int j = 0; j < 3; j++)
                {
                    switch (j)
                    {
                        case 0:
                            _ray.Triangle[(i * 3) + j] = i;
                            break;
                        case 1:
                            _ray.Triangle[(i * 3) + j] = i + (360 / _ray.Angle);
                            break;
                        case 2:
                            _ray.Triangle[(i * 3) + j] = (i + 1) % (360 / _ray.Angle) == 0 ? i + 1 - (360 / _ray.Angle) : (i + 1);
                            break;
                        default:
                            break;
                    }

                }
            }
        }
        // Debug.Log(_ray.RayCount);
        string str = "";
        int count = 0;

        foreach (var item in _ray.Triangle)
        {
            str += item + " ";
            count++;

            if (count % 3 == 0)
                str += "|";
        }

        Debug.Log(str);
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

        _vertexPosition = new List<Vector3>();
        _triangle = new List<int>();

        emptyObj = new GameObject("EmptyObject");
        mesh = new Mesh();
        emptyObj.AddComponent<MeshFilter>();
        emptyObj.AddComponent<MeshRenderer>();
        emptyObj.GetComponent<MeshFilter>().mesh = mesh;
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
            _ray.Angle = _angle;
        }
    }
}