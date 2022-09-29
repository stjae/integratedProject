using UnityEngine;

public class PhysicsModule : MonoBehaviour
{
    protected Rigidbody _rigBody;
    protected CapsuleCollider _capsCollider;

    void Awake()
    {
        _rigBody = gameObject.GetComponent<Rigidbody>();
        _capsCollider = gameObject.GetComponent<CapsuleCollider>();

        _rigBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public Rigidbody Rigidbody
    {
        get { return _rigBody; }
    }

    public CapsuleCollider CapsuleCollider
    {
        get { return _capsCollider; }
    }

    public float GetDistanceToGround()
    {
        return _capsCollider.height * 0.5f;
    }
}