using System;
using UnityEngine;

public class BaseCharacter : PhysicsModule
{
    // Property
    [Range(1f, 10f)]
    float _speed = 3f;
    [Range(1f, 3f)]
    float _runningCoef = 1.5f;
    [Range(1f, 10f)]
    float _jumpForce = 5.5f;

    // State
    bool _isMoving;
    [SerializeField] bool _isRunning;
    [SerializeField] bool _isJumping;

    // Vector
    [SerializeField] Vector3 _moveVector;

    protected CollisionManager _collision;
    protected SurfaceManager _surface;

    // Property Getter
    public float Speed { get { return _speed; } }
    public float RunningCoef { get { return _runningCoef; } }
    public float JumpForce { get { return _jumpForce; } }

    // State Getter
    public bool IsGrounded { get { return _surface.IsGrounded; } }
    public bool IsOnSlope { get { return _surface.IsOnSlope; } }
    public bool IsRunning { get { return _isRunning; } }
    public bool IsJumping { get { return _isJumping; } }

    // State Setter
    public bool SetRun { set { _isRunning = value; } }
    public bool SetJump { set { _isJumping = value; } }

    // Vector
    public ref Vector3 MoveVector { get { return ref _moveVector; } }

    virtual protected void Jump()
    {
        _rigBody.velocity = new Vector3(_moveVector.x, transform.up.y * _jumpForce, _moveVector.z);
    }

    void Awake()
    {
        _rigBody = GetComponent<Rigidbody>();
        _capsCollider = GetComponent<CapsuleCollider>();

        _collision = gameObject.AddComponent<CollisionManager>();
        _surface = gameObject.AddComponent<SurfaceManager>();
    }

    protected virtual void Update()
    {
        if (IsGrounded || IsOnSlope)
        {
            float verticalForce = IsJumping ? _rigBody.velocity.y : _moveVector.y;
            _rigBody.velocity = new Vector3(_moveVector.x, verticalForce, _moveVector.z);
        }

        if (IsGrounded && IsJumping)
            SetJump = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, MoveVector * 5);
    }
}