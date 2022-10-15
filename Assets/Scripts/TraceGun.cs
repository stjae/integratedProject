using UnityEngine;

public class TraceGun : MonoBehaviour
{
    bool[] _isHit;
    bool[] _isHitOppos;
    RaycastHit _hitCenter;
    RaycastHit _hitCenterOppos;
    RaycastHit[] _hitArray;
    Vector3[] _hitRadius;
    GameObject[] _trace;
    int _hitAngle = 45;
    int _hitAmount;

    GameObject _traceSphere;

    GameObject _testObj;
    MeshFilter _meshFilter;
    Mesh _mesh;

    FirstPersonCamera _cam;

    void CreateTrace()
    {
        GameObject _frontTrace = Instantiate(_traceSphere, Vector3.zero, Quaternion.identity);
        GameObject _backTrace = Instantiate(_traceSphere, Vector3.zero, Quaternion.identity);

        Renderer _frontTraceRenderer = _frontTrace.GetComponent<Renderer>();
        Renderer _backTraceRenderer = _backTrace.GetComponent<Renderer>();

        _frontTraceRenderer.material.SetColor("_Color", Color.red);
        _backTraceRenderer.material.SetColor("_Color", Color.blue);

        for (int i = 0; i < _hitAmount; i++)
        {
            _trace[i] = Instantiate(_frontTrace, transform.position, Quaternion.identity);
            _trace[_hitAmount + i] = Instantiate(_backTrace, transform.position, Quaternion.identity);
        }
    }

    void Mark()
    {
        for (int i = 0; i < _hitAmount; i++)
        {
            _trace[i].transform.position = Vector3.Lerp(_trace[i].transform.position, _hitArray[i].point, Time.deltaTime * 16f);
            _trace[_hitAmount + i].transform.position = Vector3.Lerp(_trace[_hitAmount + i].transform.position, _hitArray[_hitAmount + i].point, Time.deltaTime * 16f);
        }
    }

    public void Fire()
    {
        GameObject _frontTrace = Instantiate(_traceSphere, _hitCenter.point, Quaternion.identity);
        GameObject _backTrace = Instantiate(_traceSphere, _hitCenterOppos.point, Quaternion.identity);

        Renderer _frontTraceRenderer = _frontTrace.GetComponent<Renderer>();
        Renderer _backTraceRenderer = _backTrace.GetComponent<Renderer>();

        _frontTraceRenderer.material.SetColor("_Color", Color.red);
        _backTraceRenderer.material.SetColor("_Color", Color.blue);

        Destroy(_frontTrace, 2f);
        Destroy(_backTrace, 2f);

        for (int i = 0; i < _hitAmount; i++)
        {
            GameObject _frontTraceOnRadius = Instantiate(_frontTrace, _hitArray[i].point, Quaternion.identity);
            GameObject _backTraceOnRadius = Instantiate(_backTrace, _hitArray[_hitAmount + i].point, Quaternion.identity);

            Destroy(_frontTraceOnRadius, 2f);
            Destroy(_backTraceOnRadius, 2f);
        }
    }

    void Start()
    {
        _cam = transform.GetComponentInParent<Player>().Camera;

        _traceSphere = GameObject.Find("TraceSphere");

        _hitAmount = 360 / _hitAngle;
        _hitRadius = new Vector3[_hitAmount * 2];
        _hitArray = new RaycastHit[_hitAmount * 2];
        _trace = new GameObject[_hitAmount * 2];
        _isHit = new bool[_hitAmount];
        _isHitOppos = new bool[_hitAmount];

        for (int i = 0; i < _hitAmount; i++)
        {
            _hitRadius[i] = Quaternion.AngleAxis(_hitAngle * i, Vector3.forward) * Vector3.left * 0.2f;
            _hitRadius[_hitAmount + i] = Quaternion.AngleAxis(_hitAngle * i, Vector3.forward) * (Vector3.left * 0.2f + Vector3.forward * 10);
        }

        CreateTrace();
    }

    void FixedUpdate()
    {
        // _isHit = Physics.Raycast(_cam.transform.position, _cam.transform.forward, out _hitCenter, 10f);
        // if (_isHit)
        // {
        //     _isHitOppos = Physics.Raycast(_cam.transform.TransformPoint(Vector3.forward * 10f), _cam.transform.TransformDirection(Vector3.back), out _hitCenterOppos, 10f);
        // }

        for (int i = 0; i < _hitAmount; i++)
        {
            _isHit[i] = Physics.Raycast(_cam.transform.TransformPoint(_hitRadius[i]), _cam.transform.forward, out _hitArray[i], 10f, (-1) - (1 << 9));

            if (_isHit[i])
                _isHitOppos[i] = Physics.Raycast(_cam.transform.TransformPoint(_hitRadius[_hitAmount + i]), -_cam.transform.forward, out _hitArray[_hitAmount + i], 10f - _hitArray[i].distance, (-1) - (1 << 9));
            else
                _isHitOppos[i] = false;
        }

        for (int i = 0; i < _hitAmount; i++)
        {
            if (!_isHit[i])
            {
                _trace[i].SetActive(false);

                for (int j = 0; j < _hitAmount; j++)
                {
                    if (_isHit[j])
                    {
                        _hitArray[i].point = _hitArray[j].point;
                    }
                    else
                    {
                        _hitArray[i].point = _cam.transform.TransformPoint(Vector3.forward * 10f);
                    }
                }
            }
            else
                _trace[i].SetActive(true);

            if (!_isHitOppos[i])
            {
                _trace[_hitAmount + i].SetActive(false);

                for (int j = 0; j < _hitAmount; j++)
                {
                    if (_isHitOppos[j])
                    {
                        _hitArray[_hitAmount + i].point = _hitArray[_hitAmount + j].point;
                    }
                    else
                    {
                        _hitArray[_hitAmount + i].point = _cam.transform.TransformPoint(Vector3.forward * 10f);
                    }
                }
            }
            else
                _trace[_hitAmount + i].SetActive(true);
        }
    }

    void Update()
    {
        Mark();
    }
}
