using UnityEngine;

public class TraceGun : MonoBehaviour
{
    // RaycastHit _hitCenter;
    // RaycastHit _hitCenterOppos;
    [Range(0.1f, 2.0f)]
    [SerializeField] float _radius = 0.2f;
    [Range(1, 45)]
    [SerializeField] int _angle = 45;

    FirstPersonCamera _cam;
    TraceManager _traceManager;

    void Mark()
    {
        for (int i = 0; i < _traceManager.HitPositionCount; i++)
        {
            _traceManager.TraceFront[i].transform.position = Vector3.Lerp(_traceManager.TraceFront[i].transform.position, _traceManager.Hit[i].point, Time.deltaTime * 16f);
            _traceManager.TraceBack[i].transform.position = Vector3.Lerp(_traceManager.TraceBack[i].transform.position, _traceManager.Hit[_traceManager.HitPositionCount + i].point, Time.deltaTime * 16f);
        }
    }

    public void Fire()
    {
        // GameObject _frontTrace = Instantiate(_traceSphere, _hitCenter.point, Quaternion.identity);
        // GameObject _backTrace = Instantiate(_traceSphere, _hitCenterOppos.point, Quaternion.identity);

        // Renderer _frontTraceRenderer = _frontTrace.GetComponent<Renderer>();
        // Renderer _backTraceRenderer = _backTrace.GetComponent<Renderer>();

        // _frontTraceRenderer.material.SetColor("_Color", Color.red);
        // _backTraceRenderer.material.SetColor("_Color", Color.blue);

        // Destroy(_frontTrace, 2f);
        // Destroy(_backTrace, 2f);

        for (int i = 0; i < _traceManager.PointCount; i++)
        {
            GameObject _frontTraceOnRadius = Instantiate(_traceManager.TraceFront[i], _traceManager.Hit[i].point, Quaternion.identity);
            GameObject _backTraceOnRadius = Instantiate(_traceManager.TraceBack[i], _traceManager.Hit[_traceManager.PointCount + i].point, Quaternion.identity);

            Destroy(_frontTraceOnRadius, 2f);
            Destroy(_backTraceOnRadius, 2f);
        }
    }

    void Start()
    {
        _cam = transform.GetComponentInParent<Player>().Camera;
        _traceManager = gameObject.AddComponent<TraceManager>();
    }

    void Update()
    {
        // if (Input.GetKey(KeyCode.LeftCommand))
        // {
        _radius -= Input.mouseScrollDelta.y / 10;
        // }
        _radius = Mathf.Clamp(_radius, 0.1f, 2.0f);
        _traceManager.Radius = _radius;

        if (_traceManager.Angle > _angle)
        {
            _traceManager.PointCount = 360 / _angle;
            _traceManager.Angle = _angle;
            _traceManager.AddTrace();
        }
        else if (_traceManager.Angle < _angle)
        {
            _traceManager.PointCount = 360 / _angle;
            _traceManager.Angle = _angle;
            _traceManager.RemoveTrace();
        }

        Mark();
    }
}
