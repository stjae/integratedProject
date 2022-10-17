using System.Collections.Generic;
using UnityEngine;

public class TraceManager : MonoBehaviour
{
    bool[] _isRayHitFront;
    bool[] _isRayHitBack;

    int _pointCount;
    int _angle = 45;
    float _deltaAngle = 0;
    float _radius = 0.2f;

    public int PointCount { get { return _pointCount; } set { _pointCount = value; } }
    public int HitPositionCount { get { return _hitPositionFront.Count; } }
    public int Angle { get { return _angle; } set { _angle = value; } }
    public float Radius { set { _radius = value; } }

    List<Vector3> _hitPositionFront;
    List<Vector3> _hitPositionBack;
    RaycastHit[] _hit;

    GameObject _sphere;
    GameObject _frontTrace;
    GameObject _backTrace;
    List<GameObject> _traceFront;
    List<GameObject> _traceBack;

    public RaycastHit[] Hit { get { return _hit; } }
    public List<GameObject> TraceFront { get { return _traceFront; } }
    public List<GameObject> TraceBack { get { return _traceBack; } }

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
        for (int i = _traceFront.Count; i < _pointCount; i++)
        {
            _traceFront.Add(Instantiate(_frontTrace, Vector3.zero, Quaternion.identity));
            _traceBack.Add(Instantiate(_backTrace, Vector3.zero, Quaternion.identity));
        }
    }

    public void RemoveTrace()
    {
        for (int i = _pointCount; i < _traceFront.Count; i++)
        {
            DestroyImmediate(_traceFront[i]);
            _traceFront.RemoveAt(i);
            DestroyImmediate(_traceBack[i]);
            _traceBack.RemoveAt(i);
        }
    }

    void CastRay()
    {
        for (int i = 0; i < _hitPositionFront.Count; i++)
        {
            _isRayHitFront[i] = Physics.Raycast(_cam.transform.TransformPoint(_hitPositionFront[i]), _cam.transform.forward, out _hit[i], 10f, (-1) - (1 << _traceLayer | 1 << _playerLayer));

            if (_isRayHitFront[i])
                _isRayHitBack[i] = Physics.Raycast(_cam.transform.TransformPoint(_hitPositionBack[i]), -_cam.transform.forward, out _hit[_hitPositionFront.Count + i], 10f - _hit[i].distance, (-1) - (1 << _traceLayer | 1 << _playerLayer));
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
                        _hit[i].point = _hit[j].point;
                    }
                    else
                    {
                        _hit[i].point = _cam.transform.TransformPoint(Vector3.forward * 10f);
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
                        _hit[_hitPositionFront.Count + i].point = _hit[_hitPositionFront.Count + j].point;
                    }
                    else
                    {
                        _hit[_hitPositionFront.Count + i].point = _cam.transform.TransformPoint(Vector3.forward * 10f);
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

        _hit = new RaycastHit[_pointCount * 2];

        _traceFront = new List<GameObject>();
        _traceBack = new List<GameObject>();

        _isRayHitFront = new bool[_pointCount];
        _isRayHitBack = new bool[_pointCount];

        for (int i = 0; i < _pointCount; i++)
        {
            _hitPositionFront.Add(Quaternion.AngleAxis(_angle * i, Vector3.forward) * Vector3.left * _radius);
            _hitPositionBack.Add(Quaternion.AngleAxis(_angle * i, Vector3.forward) * (Vector3.left * _radius + Vector3.forward * 10));
        }

        InitTrace();
    }

    void Update()
    {
        Debug.Log(string.Format("PointCount = {0}, TraceFrontCount = {1}", _pointCount, _traceFront.Count));

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