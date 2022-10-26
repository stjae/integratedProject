using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    FirstPersonCamera _cam;
    Sphere _sphere;

    int _rayCount;
    int _angle;
    float _radius = 0.2f;
    float _deltaAngle = 0;
    public int RayCount { get { return _rayCount; } set { _rayCount = value; } }
    public int Angle { get { return _angle; } set { _angle = value; } }
    public float Radius { get { return _radius; } set { _radius = value; } }

    List<bool> _isRayHitFront;
    List<bool> _isRayHitBack;
    public List<bool> IsRayHitFront { get { return _isRayHitFront; } }
    public List<bool> IsRayHitBack { get { return _isRayHitBack; } }

    List<RaycastHit> _frontHit;
    List<RaycastHit> _backHit;
    public List<RaycastHit> FrontHit { get { return _frontHit; } }
    public List<RaycastHit> BackHit { get { return _backHit; } }

    List<Vector3> _rayOriginFront;
    List<Vector3> _rayOriginBack;
    public List<Vector3> RayOriginFront { get { return _rayOriginFront; } }
    public List<Vector3> RayOriginBack { get { return _rayOriginBack; } }

    int _traceLayer;
    int _playerLayer;
    public int TraceLayer { get { return _traceLayer; } }
    public int PlayerLayer { get { return _playerLayer; } }

    void Start()
    {
        _cam = transform.GetComponentInParent<Player>().Camera;
        _sphere = gameObject.GetComponent<Sphere>();

        _frontHit = new List<RaycastHit>();
        _backHit = new List<RaycastHit>();

        _isRayHitFront = new List<bool>();
        _isRayHitBack = new List<bool>();

        _rayOriginFront = new List<Vector3>();
        _rayOriginBack = new List<Vector3>();

        _traceLayer = LayerMask.NameToLayer("Trace");
        _playerLayer = LayerMask.NameToLayer("Player");
    }

    void UpdateRayOriginCount()
    {
        while (_rayOriginFront.Count < _rayCount)
        {
            _rayOriginFront.Add(Vector3.zero);
            _rayOriginBack.Add(Vector3.zero);

            _isRayHitFront.Add(false);
            _isRayHitBack.Add(false);

            RaycastHit hit = new RaycastHit();

            _frontHit.Add(hit);
            _backHit.Add(hit);

            _sphere.Count = _sphere.Count + 1;
        }

        while (_rayOriginFront.Count > _rayCount)
        {
            _rayOriginFront.RemoveAt(_rayOriginFront.Count - _rayCount);
            _rayOriginBack.RemoveAt(_rayOriginBack.Count - _rayCount);

            _isRayHitFront.RemoveAt(_isRayHitFront.Count - _rayCount);
            _isRayHitBack.RemoveAt(_isRayHitBack.Count - _rayCount);

            _frontHit.RemoveAt(_frontHit.Count - _rayCount);
            _backHit.RemoveAt(_backHit.Count - _rayCount);

            _sphere.Count = _sphere.Count - 1;
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

            _isRayHitFront[i] = Physics.Raycast(_cam.transform.TransformPoint(_rayOriginFront[i]), _cam.transform.forward, out frontHit, 10f, (-1) - (1 << _traceLayer | 1 << _playerLayer));
            if (_isRayHitFront[i])
            {
                _frontHit[i] = frontHit;
                _isRayHitBack[i] = Physics.Raycast(_cam.transform.TransformPoint(_rayOriginBack[i]), -_cam.transform.forward, out backHit, 10f - frontHit.distance, (-1) - (1 << _traceLayer | 1 << _playerLayer));
                if (_isRayHitBack[i])
                    _backHit[i] = backHit;
            }
            else
                _isRayHitBack[i] = false;
        }
    }

    void OnDrawGizmos()
    {
        // for (int i = 0; i < _frontHit.Count; i++)
        // {
        //     if (_isRayHitFront[i])
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawLine(_cam.transform.TransformPoint(_rayOriginFront[i]), _frontHit[i].point);
        //     }

        //     if (_isRayHitBack[i])
        //     {
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawLine(_cam.transform.TransformPoint(_rayOriginBack[i]), _backHit[i].point);
        //     }
        // }
    }

    void FixedUpdate()
    {
        // Debug.Log(string.Format("RayCount = {0}, RayOriginFront.Count = {1}, FrontHitCount = {2}, IsRayHitFrontCount = {3}", _rayCount, _rayOriginFront.Count, _frontHit.Count, _isRayHitFront.Count));
        // _deltaAngle += 0.02f;
        // _deltaAngle %= 360;

        CastRay();
    }

    void Update()
    {
        UpdateRayOriginCount();
        UpdateRayOriginPosition();
    }
}