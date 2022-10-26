using UnityEngine;

// TODO: collisionAngle;
public class CollisionManager : MonoBehaviour
{
    [SerializeField] bool _isCollided;
    GameObject _contactObj;
    ContactPoint _contactPoint;
    float _collisionAngle;

    public bool IsCollided { get { return _isCollided; } }
    public ContactPoint ContactPoint { get { return _contactPoint; } }

    public void AdjustVector()
    {
        _collisionAngle = Vector3.SignedAngle(UserInput.moveVector, _contactPoint.normal, Vector3.up);

        if (_isCollided && _collisionAngle <= -90 && _collisionAngle >= -180 || _isCollided && _collisionAngle >= 90 && _collisionAngle <= 180)
        {
            UserInput.moveVector = Vector3.ProjectOnPlane(UserInput.moveVector, _contactPoint.normal);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name != "Ground")
        {
            _isCollided = true;
            _contactObj = collision.gameObject;
            _contactPoint = collision.GetContact(0);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        _isCollided = false;
        _contactObj = null;
    }
}
