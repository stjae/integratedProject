using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    FirstPersonCamera _cam;
    TraceGun _traceGun;
    Sphere _sphere;

    int _rayCount;
    int _vertexCount;
    int _angle;
    float _radius = 0.2f;
    [SerializeField] bool _deltaAngleSwitch = false;
    [SerializeField] float _deltaAngle = 0;
    public int RayCount { get { return _rayCount; } set { _rayCount = value; } }
    public int VertexCount { get { return _vertexCount; } set { _vertexCount = value; } }
    public int Angle { get { return _angle; } set { _angle = value; } }
    public float Radius { get { return _radius; } set { _radius = value; } }

    List<bool> _isRayHitFront;
    List<bool> _isRayHitBack;
    public List<bool> IsRayHitFront { get { return _isRayHitFront; } }
    public List<bool> IsRayHitBack { get { return _isRayHitBack; } }

    List<RaycastHit> _frontHit;
    List<RaycastHit> _backHit;
    List<Vector3> _frontHitPoint;
    List<Vector3> _backHitPoint;
    public List<RaycastHit> FrontHit { get { return _frontHit; } }
    public List<RaycastHit> BackHit { get { return _backHit; } }
    public List<Vector3> FrontHitPoint { get { return _frontHitPoint; } }
    public List<Vector3> BackHitPoint { get { return _backHitPoint; } }

    List<int> _frontTriangle;
    List<int> _backTriangle;
    List<int> _sideTriangle;
    public List<int> FrontTriangle { get { return _frontTriangle; } }
    public List<int> BackTriangle { get { return _backTriangle; } }
    public List<int> SideTriangle { get { return _sideTriangle; } }

    List<Vector3> _rayOriginFront;
    List<Vector3> _rayOriginBack;
    Dictionary<string, int> _rayIndex;
    Dictionary<int, string> _rayIndexReversed;
    public List<Vector3> RayOriginFront { get { return _rayOriginFront; } }
    public List<Vector3> RayOriginBack { get { return _rayOriginBack; } }
    public Dictionary<string, int> RayIndex { get { return _rayIndex; } }
    public Dictionary<int, string> RayIndexReversed { get { return _rayIndexReversed; } }

    List<bool> _isBorderVertex;
    public List<bool> IsBorderVertex { get { return _isBorderVertex; } }

    void Start()
    {
        _cam = transform.GetComponentInParent<Player>().Camera;
        _traceGun = gameObject.GetComponent<TraceGun>();
        _sphere = gameObject.GetComponent<Sphere>();

        _frontHit = new List<RaycastHit>();
        _backHit = new List<RaycastHit>();
        _frontHitPoint = new List<Vector3>();
        _backHitPoint = new List<Vector3>();

        _isRayHitFront = new List<bool>();
        _isRayHitBack = new List<bool>();

        _rayOriginFront = new List<Vector3>();
        _rayOriginBack = new List<Vector3>();
        _rayIndex = new Dictionary<string, int>();
        _rayIndexReversed = new Dictionary<int, string>();

        _frontTriangle = new List<int>();
        _backTriangle = new List<int>();
        _sideTriangle = new List<int>();

        _isBorderVertex = new List<bool>();
    }

    void UpdateRayOriginCount()
    {
        while (_rayOriginFront.Count < _rayCount)
        {
            _rayOriginFront.Add(Vector3.zero);
            _rayOriginBack.Add(Vector3.zero);

            _sphere.Count += 1;
        }
        while (_isRayHitFront.Count < _rayCount + 1)
        {
            _isRayHitFront.Add(false);
            _isRayHitBack.Add(false);

            RaycastHit hit = new RaycastHit();

            _frontHit.Add(hit);
            _backHit.Add(hit);

            _frontHitPoint.Add(Vector3.zero);
            _backHitPoint.Add(Vector3.zero);

            _isBorderVertex.Add(false);
        }
        while (_frontTriangle.Count < _rayCount * 6)
        {
            _frontTriangle.Add(0);
            _backTriangle.Add(0);
            _sideTriangle.Add(0);
        }

        while (_rayOriginFront.Count > _rayCount)
        {
            _rayOriginFront.RemoveAt(_rayOriginFront.Count - _rayCount);
            _rayOriginBack.RemoveAt(_rayOriginBack.Count - _rayCount);

            _sphere.Count -= 1;
        }
        while (_isRayHitFront.Count > _rayCount + 1)
        {
            _isRayHitFront.RemoveAt(_isRayHitFront.Count - _rayCount);
            _isRayHitBack.RemoveAt(_isRayHitBack.Count - _rayCount);

            _frontHit.RemoveAt(_frontHit.Count - _rayCount);
            _backHit.RemoveAt(_backHit.Count - _rayCount);

            _frontHitPoint.RemoveAt(_frontHitPoint.Count - _rayCount);
            _backHitPoint.RemoveAt(_backHitPoint.Count - _rayCount);

            _isBorderVertex.RemoveAt(_isBorderVertex.Count - _rayCount);
        }
        while (_frontTriangle.Count > _rayCount * 6)
        {
            _frontTriangle.RemoveAt(_frontTriangle.Count - _rayCount * 6);
            _backTriangle.RemoveAt(_backTriangle.Count - _rayCount * 6);
            _sideTriangle.RemoveAt(_backTriangle.Count - _rayCount * 6);
        }
    }

    void UpdateRayOriginPosition()
    {
        int firstIndex = 0;
        int lastIndex = 0;

        for (int j = 0; j < 4; j++)
        {
            for (int i = firstIndex; i < firstIndex + (360 / _angle); i++)
            {
                _rayIndex[string.Format("{0}{1}", j, i - firstIndex)] = i;
                _rayIndexReversed[i] = string.Format("{0} {1}", j, i - firstIndex);

                _rayOriginFront[i] = Quaternion.AngleAxis(_angle * (i + _deltaAngle), Vector3.forward) * (Vector3.left * (_radius - ((_radius / 4) * j)));
                _rayOriginBack[i] = Quaternion.AngleAxis(_angle * (i + _deltaAngle), Vector3.forward) * (Vector3.left * (_radius - ((_radius / 4) * j)) + Vector3.forward * 10);

                lastIndex = i + 1;
            }

            firstIndex = lastIndex;
        }
    }

    void CastRay()
    {
        for (int i = 0; i < _rayOriginFront.Count; i++)
        {
            RaycastHit frontHit;
            RaycastHit backHit;

            _isRayHitFront[i] = Physics.Raycast(_cam.transform.TransformPoint(_rayOriginFront[i]), _cam.transform.forward, out frontHit, 10f, (-1) - (1 << Layer.HitSphere | 1 << Layer.TraceFace | 1 << Layer.Player));
            _isRayHitBack[i] = Physics.Raycast(_cam.transform.TransformPoint(_rayOriginBack[i]), -_cam.transform.forward, out backHit, 10f - frontHit.distance, (-1) - (1 << Layer.HitSphere | 1 << Layer.TraceFace | 1 << Layer.Player));
            if (_isRayHitFront[i] && _isRayHitBack[i])
            {
                _frontHit[i] = frontHit;
                _frontHitPoint[i] = frontHit.point - _cam.transform.forward * 0.01f;

                _backHit[i] = backHit;
                _backHitPoint[i] = backHit.point + _cam.transform.forward * 0.01f;
            }
            else
            {
                _isRayHitFront[i] = false;
                _isRayHitBack[i] = false;
            }
        }
    }

    public void GetRayHitCenter()
    {
        Vector3 frontCenterPosition = Vector3.zero;
        Vector3 backCenterPosition = Vector3.zero;
        int frontHitCount;
        int backHitCount;

        int firstIndex = 0;
        int lastIndex = _rayCount - 1;
        bool isFrontCenterPositionSet = false;
        bool isBackCenterPositionSet = false;

        for (int j = 0; j < 4; j++)
        {
            frontHitCount = 0;
            backHitCount = 0;
            bool isHitFrontCenter = false;
            bool isHitBackCenter = false;

            for (int i = lastIndex; i > lastIndex - _vertexCount; i--)
            {
                if (_isRayHitFront[i] && !isFrontCenterPositionSet)
                {
                    isHitFrontCenter = true;
                    frontCenterPosition += _frontHit[i].point;
                    frontHitCount++;
                }
                if (_isRayHitBack[i] && !isBackCenterPositionSet)
                {
                    isHitBackCenter = true;
                    backCenterPosition += _backHit[i].point;
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
        _isRayHitFront[_isRayHitFront.Count - 1] = Physics.Raycast(_cam.transform.position, frontCenterPosition - _cam.transform.position, out frontCenterHit, 10f, (-1) - (1 << Layer.HitSphere | 1 << Layer.Player));
        _isRayHitBack[_isRayHitBack.Count - 1] = Physics.Raycast(_cam.transform.TransformPoint(Vector3.forward * 10), backCenterPosition - _cam.transform.TransformPoint(Vector3.forward * 10), out backCenterHit, 10f - frontCenterHit.distance, (-1) - (1 << Layer.HitSphere | 1 << Layer.Player));

        _frontHit[_frontHit.Count - 1] = frontCenterHit;
        _backHit[_backHit.Count - 1] = backCenterHit;
        _frontHitPoint[_frontHitPoint.Count - 1] = frontCenterHit.point;
        _backHitPoint[_backHitPoint.Count - 1] = backCenterHit.point;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < _frontHit.Count; i++)
        {
            // if (_isRayHitFront[i])
            // {
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawLine(_cam.transform.TransformPoint(_rayOriginFront[i]), _frontHit[i].point);
            // }

            //     if (_isRayHitBack[i])
            //     {
            //         Gizmos.color = Color.blue;
            //         Gizmos.DrawLine(_cam.transform.TransformPoint(_rayOriginBack[i]), _backHit[i].point);
            //     }
        }
    }

    void FixedUpdate()
    {
        // Debug.Log(string.Format("RayCount = {0}, RayOriginFront.Count = {1}, FrontHitCount = {2}, IsRayHitFrontCount = {3}", _rayCount, _rayOriginFront.Count, _frontHit.Count, _isRayHitFront.Count));
        // Debug.Log(string.Format("TriangleCount = {0}", _triangle.Count));
        if (_deltaAngleSwitch) _deltaAngle += 0.02f;
        _deltaAngle %= 360;

        CastRay();
    }

    void Update()
    {
        UpdateRayOriginCount();
        UpdateRayOriginPosition();
    }
}