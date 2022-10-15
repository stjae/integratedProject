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
    public int Angle { set { _angle = value; } }
    public float Radius { set { _radius = value; } }

    Vector3[] _hitPosition;
    RaycastHit[] _hit;

    GameObject _sphere;
    GameObject[] _trace;

    public RaycastHit[] Hit { get { return _hit; } }
    public GameObject[] Trace { get { return _trace; } }

    FirstPersonCamera _cam;

    int _traceLayer;
    int _playerLayer;

    void InitTrace()
    {
        GameObject _frontTrace = Instantiate(_sphere, Vector3.zero, Quaternion.identity);
        GameObject _backTrace = Instantiate(_sphere, Vector3.zero, Quaternion.identity);

        Renderer _frontTraceRenderer = _frontTrace.GetComponent<Renderer>();
        Renderer _backTraceRenderer = _backTrace.GetComponent<Renderer>();

        _frontTraceRenderer.material.SetColor("_Color", Color.red);
        _backTraceRenderer.material.SetColor("_Color", Color.blue);

        for (int i = 0; i < _pointCount; i++)
        {
            _trace[i] = Instantiate(_frontTrace, transform.position, Quaternion.identity);
            _trace[_pointCount + i] = Instantiate(_backTrace, transform.position, Quaternion.identity);
        }
    }

    void CastRay()
    {
        for (int i = 0; i < _pointCount; i++)
        {
            _isRayHitFront[i] = Physics.Raycast(_cam.transform.TransformPoint(_hitPosition[i]), _cam.transform.forward, out _hit[i], 10f, (-1) - (1 << _traceLayer | 1 << _playerLayer));

            if (_isRayHitFront[i])
                _isRayHitBack[i] = Physics.Raycast(_cam.transform.TransformPoint(_hitPosition[_pointCount + i]), -_cam.transform.forward, out _hit[_pointCount + i], 10f - _hit[i].distance, (-1) - (1 << _traceLayer | 1 << _playerLayer));
            else
                _isRayHitBack[i] = false;
        }

        for (int i = 0; i < _pointCount; i++)
        {
            if (!_isRayHitFront[i])
            {
                _trace[i].SetActive(false);

                for (int j = 0; j < _pointCount; j++)
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
                _trace[i].SetActive(true);

            if (!_isRayHitBack[i])
            {
                _trace[_pointCount + i].SetActive(false);

                for (int j = 0; j < _pointCount; j++)
                {
                    if (_isRayHitBack[j])
                    {
                        _hit[_pointCount + i].point = _hit[_pointCount + j].point;
                    }
                    else
                    {
                        _hit[_pointCount + i].point = _cam.transform.TransformPoint(Vector3.forward * 10f);
                    }
                }
            }
            else
                _trace[_pointCount + i].SetActive(true);
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
        _hitPosition = new Vector3[_pointCount * 2];

        _hit = new RaycastHit[_pointCount * 2];
        _trace = new GameObject[_pointCount * 2];

        _isRayHitFront = new bool[_pointCount];
        _isRayHitBack = new bool[_pointCount];

        for (int i = 0; i < _pointCount; i++)
        {
            _hitPosition[i] = Quaternion.AngleAxis(_angle * i, Vector3.forward) * Vector3.left * _radius;
            _hitPosition[_pointCount + i] = Quaternion.AngleAxis(_angle * i, Vector3.forward) * (Vector3.left * _radius + Vector3.forward * 10);
        }

        InitTrace();
    }

    void Update()
    {
        for (int i = 0; i < _pointCount; i++)
        {
            _hitPosition[i] = Quaternion.AngleAxis(_angle * (i + _deltaAngle), Vector3.forward) * Vector3.left * _radius;
            _hitPosition[_pointCount + i] = Quaternion.AngleAxis(_angle * (i + _deltaAngle), Vector3.forward) * (Vector3.left * _radius + Vector3.forward * 10);
        }
    }

    void FixedUpdate()
    {
        _deltaAngle += 0.02f;
        _deltaAngle %= 360;

        CastRay();
    }
}