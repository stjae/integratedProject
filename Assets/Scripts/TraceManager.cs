using System.Collections.Generic;
using UnityEngine;

public class TraceManager : MonoBehaviour
{
    bool _isHitCenterFront;
    bool _isHitCenterBack;
    List<bool> _isRayHitFront;
    List<bool> _isRayHitBack;

    int _pointCount;
    int _angle = 45;
    float _deltaAngle = 0;
    float _radius = 0.2f;

    public int PointCount { get { return _pointCount; } set { _pointCount = value; } }
    public int HitPositionCount { get { return _hitPositionFront.Count; } }
    public int Angle { get { return _angle; } set { _angle = value; } }
    public float Radius { set { _radius = value; } }

    RaycastHit _hitCenterFront;
    RaycastHit _hitCenterBack;
    List<Vector3> _hitPositionFront;
    List<Vector3> _hitPositionBack;
    List<RaycastHit> _frontHit;
    List<RaycastHit> _backHit;

    GameObject _sphere;
    GameObject _frontTrace;
    GameObject _backTrace;
    List<GameObject> _traceFront;
    List<GameObject> _traceBack;

    public List<GameObject> TraceFront { get { return _traceFront; } }
    public List<GameObject> TraceBack { get { return _traceBack; } }
    public List<RaycastHit> FrontHit { get { return _frontHit; } }
    public List<RaycastHit> BackHit { get { return _backHit; } }

    FirstPersonCamera _cam;

    int _traceLayer;
    int _playerLayer;

    void InitTrace()
    {
        _frontTrace = Instantiate(_sphere, Vector3.zero, Quaternion.identity);
        _backTrace = Instantiate(_sphere, Vector3.zero, Quaternion.identity);

        Renderer _frontTraceRenderer = _frontTrace.GetComponent<Renderer>();
        Renderer _backTraceRenderer = _backTrace.GetComponent<Renderer>();

        _frontTraceRenderer.material.SetColor("_Color", Color.red);
        _backTraceRenderer.material.SetColor("_Color", Color.blue);

        for (int i = 0; i < _pointCount; i++)
        {
            _traceFront.Add(Instantiate(_frontTrace, Vector3.zero, Quaternion.identity));
            _traceBack.Add(Instantiate(_backTrace, Vector3.zero, Quaternion.identity));
        }
    }

    public void AddTrace()
    {
        while (_traceFront.Count != _pointCount)
        {
            _traceFront.Add(Instantiate(_frontTrace, _traceFront[0].transform.position, Quaternion.identity));
            _traceBack.Add(Instantiate(_backTrace, _traceBack[0].transform.position, Quaternion.identity));

            _hitPositionFront.Add(Quaternion.AngleAxis(_angle * (_pointCount - _traceFront.Count), Vector3.forward) * Vector3.left * _radius);
            _hitPositionBack.Add(Quaternion.AngleAxis(_angle * (_pointCount - _traceBack.Count), Vector3.forward) * (Vector3.left * _radius + Vector3.forward * 10));

            _isRayHitFront.Add(false);
            _isRayHitBack.Add(false);

            RaycastHit hit = new RaycastHit();

            _frontHit.Add(hit);
            _backHit.Add(hit);
        }
    }

    public void RemoveTrace()
    {
        while (_pointCount != _traceFront.Count)
        {
            DestroyImmediate(_traceFront[_traceFront.Count - _pointCount]);
            _traceFront.RemoveAt(_traceFront.Count - _pointCount);
            DestroyImmediate(_traceBack[_traceBack.Count - _pointCount]);
            _traceBack.RemoveAt(_traceBack.Count - _pointCount);

            _hitPositionFront.RemoveAt(_traceBack.Count - _pointCount);
            _hitPositionBack.RemoveAt(_traceBack.Count - _pointCount);

            _isRayHitFront.RemoveAt(_traceBack.Count - _pointCount);
            _isRayHitBack.RemoveAt(_traceBack.Count - _pointCount);

            _frontHit.RemoveAt(_traceBack.Count - _pointCount);
            _backHit.RemoveAt(_traceBack.Count - _pointCount);
        }
    }

    void CastRay()
    {
        _isHitCenterFront = Physics.Raycast(_cam.transform.position, _cam.transform.forward, out _hitCenterFront, 10f);
        if (_isHitCenterFront)
        {
            _isHitCenterBack = Physics.Raycast(_cam.transform.TransformPoint(Vector3.forward * 10f), _cam.transform.TransformDirection(Vector3.back), out _hitCenterBack, 10f);
        }

        for (int i = 0; i < _hitPositionFront.Count; i++)
        {
            RaycastHit frontHit;
            RaycastHit backHit;
            _isRayHitFront[i] = Physics.Raycast(_cam.transform.TransformPoint(_hitPositionFront[i]), _cam.transform.forward, out frontHit, 10f, (-1) - (1 << _traceLayer | 1 << _playerLayer));
            _frontHit[i] = frontHit;

            if (_isRayHitFront[i])
            {
                _isRayHitBack[i] = Physics.Raycast(_cam.transform.TransformPoint(_hitPositionBack[i]), -_cam.transform.forward, out backHit, 10f - frontHit.distance, (-1) - (1 << _traceLayer | 1 << _playerLayer));
                _backHit[i] = backHit;
            }
            else
                _isRayHitBack[i] = false;
        }

        for (int i = 0; i < _hitPositionFront.Count; i++)
        {
            if (!_isRayHitFront[i])
            {
                _traceFront[i].SetActive(false);

                for (int j = 0; j < _hitPositionFront.Count; j++)
                {
                    if (_isRayHitFront[j])
                    {
                        RaycastHit frontHit = _frontHit[i];
                        frontHit.point = _frontHit[j].point;
                        _frontHit[i] = frontHit;
                    }
                    else
                    {
                        RaycastHit frontHit = _frontHit[i];
                        frontHit.point = _cam.transform.TransformPoint(Vector3.forward * 10f);
                        _frontHit[i] = frontHit;
                    }
                }
            }
            else
                _traceFront[i].SetActive(true);

            if (!_isRayHitBack[i])
            {
                _traceBack[i].SetActive(false);

                for (int j = 0; j < _hitPositionFront.Count; j++)
                {
                    if (_isRayHitBack[j])
                    {
                        RaycastHit backHit = _backHit[i];
                        backHit.point = _backHit[j].point;
                        _backHit[i] = backHit;
                    }
                    else
                    {
                        RaycastHit backHit = _backHit[i];
                        backHit.point = _cam.transform.TransformPoint(Vector3.forward * 10f);
                        _backHit[i] = backHit;
                    }
                }
            }
            else
                _traceBack[i].SetActive(true);
        }
    }

    void Start()
    {
        _cam = transform.GetComponentInParent<Player>().Camera;
        _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        _traceLayer = LayerMask.NameToLayer("Trace");
        _playerLayer = LayerMask.NameToLayer("Player");

        _sphere.layer = _traceLayer;

        _pointCount = 360 / _angle;

        _hitPositionFront = new List<Vector3>();
        _hitPositionBack = new List<Vector3>();

        _frontHit = new List<RaycastHit>();
        _backHit = new List<RaycastHit>();

        _traceFront = new List<GameObject>();
        _traceBack = new List<GameObject>();

        _isRayHitFront = new List<bool>();
        _isRayHitBack = new List<bool>();

        for (int i = 0; i < _pointCount; i++)
        {
            _hitPositionFront.Add(Quaternion.AngleAxis(_angle * i, Vector3.forward) * Vector3.left * _radius);
            _hitPositionBack.Add(Quaternion.AngleAxis(_angle * i, Vector3.forward) * (Vector3.left * _radius + Vector3.forward * 10));

            _isRayHitFront.Add(false);
            _isRayHitBack.Add(false);

            RaycastHit hit = new RaycastHit();

            _frontHit.Add(hit);
            _backHit.Add(hit);
        }

        InitTrace();
    }

    void Update()
    {
        // Debug.Log(string.Format("PointCount = {0}, TraceFrontCount = {1}, IsRayHitFrontCount = {2}, HitPositionFront.Count = {3}, FrontHitCount = {4}, BackHitCount = {5}", _pointCount, _traceFront.Count, _isRayHitFront.Count, _hitPositionFront.Count, _frontHit.Count, _backHit.Count));
        // Debug.Log(string.Format("PointCount = {0}, 360 / _angle = {1}", _pointCount, 360 / _angle));
        for (int i = 0; i < _hitPositionFront.Count; i++)
        {
            _hitPositionFront[i] = Quaternion.AngleAxis(_angle * (i + _deltaAngle), Vector3.forward) * Vector3.left * _radius;
            _hitPositionBack[i] = Quaternion.AngleAxis(_angle * (i + _deltaAngle), Vector3.forward) * (Vector3.left * _radius + Vector3.forward * 10);
        }
    }

    void FixedUpdate()
    {
        _deltaAngle += 0.02f;
        _deltaAngle %= 360;

        CastRay();
    }
}