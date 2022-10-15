using UnityEngine;

public class TraceGun : MonoBehaviour
{
    // RaycastHit _hitCenter;
    // RaycastHit _hitCenterOppos;
    [Range(0.1f, 2.0f)]
    float _radius = 0.2f;

    FirstPersonCamera _cam;
    TraceManager _traceManager;

    void Mark()
    {
        for (int i = 0; i < _traceManager.PointCount; i++)
        {
            _traceManager.Trace[i].transform.position = Vector3.Lerp(_traceManager.Trace[i].transform.position, _traceManager.Hit[i].point, Time.deltaTime * 16f);
            _traceManager.Trace[_traceManager.PointCount + i].transform.position = Vector3.Lerp(_traceManager.Trace[_traceManager.PointCount + i].transform.position, _traceManager.Hit[_traceManager.PointCount + i].point, Time.deltaTime * 16f);
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
            GameObject _frontTraceOnRadius = Instantiate(_traceManager.Trace[i], _traceManager.Hit[i].point, Quaternion.identity);
            GameObject _backTraceOnRadius = Instantiate(_traceManager.Trace[_traceManager.PointCount + i], _traceManager.Hit[_traceManager.PointCount + i].point, Quaternion.identity);

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
        _radius -= Input.mouseScrollDelta.y / 10;
        _radius = Mathf.Clamp(_radius, 0.1f, 2.0f);

        _traceManager.Radius = _radius;

        Mark();
    }
}
