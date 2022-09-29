using UnityEngine;

public class SurfaceManager : PhysicsModule
{
    [SerializeField] bool _isGrounded;
    [SerializeField] bool _isOnSlope;

    Vector3 _slopeNormal;

    public bool IsGrounded { get { return _isGrounded; } }
    public bool IsOnSlope { get { return _isOnSlope; } }
    public Vector3 SlopeNormal { get { return _slopeNormal; } }

    public void AdjustVector()
    {
        if (IsOnSlope)
        {
            UserInput.moveVector = Vector3.ProjectOnPlane(UserInput.moveVector, SlopeNormal);
        }
    }

    public void CheckGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, GetDistanceToGround() + 0.2f);
        _rigBody.useGravity = !_isGrounded;
    }

    public void CheckOnSlope()
    {
        // TODO: maxSlopeAngle
        bool isHit = Physics.SphereCast(transform.position, CapsuleCollider.radius, Vector3.down, out RaycastHit hit, GetDistanceToGround() + 0.01f);

        _slopeNormal = hit.normal;
        _isOnSlope = hit.normal.y != 1.0f && isHit ? true : false;

        if (_isOnSlope)
        {
            _isGrounded = true;
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();
        CheckOnSlope();
    }
}