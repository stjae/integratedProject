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

    GameObject emptyObj;
    Mesh mesh;

    public void Fire()
    {
        Vector3 frontCenterPosition = Vector3.zero;
        Vector3 backCenterPosition = Vector3.zero;
        int frontHitCount;
        int backHitCount;
        for (int i = 0; i < _ray.RayCount; i++)
        {
            GameObject _frontTrace = Instantiate(_sphere.FrontSphere[i], _ray.FrontHit[i].point, Quaternion.identity);
            GameObject _backTrace = Instantiate(_sphere.BackSphere[i], _ray.BackHit[i].point, Quaternion.identity);

            Destroy(_frontTrace, 2f);
            Destroy(_backTrace, 2f);
        }

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
            Debug.Log(string.Format("FirstIndex = {0}, LastIndex = {1}, RayCount = {2}", firstIndex, lastIndex, _ray.RayCount));
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

        GameObject frontCenterTrace = Instantiate(_sphere.RedSphere, frontCenterHit.point, Quaternion.identity);
        Destroy(frontCenterTrace, 2f);
        GameObject backCenterTrace = Instantiate(_sphere.BlueSphere, backCenterHit.point, Quaternion.identity);
        Destroy(backCenterTrace, 2f);

        // Vector3[] vertices = new Vector3[_traceManager.PointCount];

        // for (int i = 0; i < _traceManager.PointCount; i++)
        // {
        //     vertices[i] = _traceManager.FrontHit[i].point;
        // }

        // mesh.vertices = vertices;
    }

    void Start()
    {
        _cam = transform.GetComponentInParent<Player>().Camera;
        _ray = gameObject.AddComponent<Ray>();
        _sphere = gameObject.AddComponent<Sphere>();

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
