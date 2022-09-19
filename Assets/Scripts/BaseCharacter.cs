using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class BaseCharacter : MonoBehaviour
{
    [HideInInspector] public Rigidbody rigBody;
    [HideInInspector] public CapsuleCollider capsCollider;
    [HideInInspector] public string targetName;
    public GameObject targetObj;
    public Vector3 moveDir;
    public Vector3 inputDir;

    private class ModelInfo
    {
        public float height = 0f;
        public float distToGround = 0f;
    }

    [Serializable]
    public class MovementProperty
    {
        [Range(1f, 10f)]
        public float speed = 3f;
        [Range(1f, 3f)]
        public float RunningCoef = 1.5f;
        [Range(1f, 10f)]
        public float jumpForce = 5.5f;
    }

    [Serializable]
    public class CharacterState
    {
        public bool isMoving;
        public bool isRunning;
        public bool isJumping;
        public bool isGrounded;
        public bool isOnSlope;
        public bool isCollided;
        public bool isArrived;
    }

    public class SurfaceProperty
    {
        public float maxSlopeAngle;
        public Vector3 slopeNormal;
        public ContactPoint[] contactPoints;
        public GameObject contactObj;
        public float collisionAngle;
        public bool multiCollision;
        public Vector3 rotatedNormal;
    }

    public CharacterState characterState = new CharacterState();
    public MovementProperty moveProp = new MovementProperty();
    private ModelInfo modelInfo = new ModelInfo();
    private SurfaceProperty surfaceProp = new SurfaceProperty();

    protected virtual void Awake()
    {
        InitComponent();
        GetModelInfo();

    }

    protected virtual void FixedUpdate()
    {
        CheckGrounded();
        CheckMoving();
        CheckSlope();
        CheckTarget();
        HandleMultiCollision();
    }

    void InitComponent()
    {
        rigBody = GetComponent<Rigidbody>();
        rigBody.constraints = RigidbodyConstraints.FreezeRotation;

        capsCollider = GetComponent<CapsuleCollider>();
    }

    #region Character Model Information
    void GetModelInfo()
    {
        modelInfo.height = capsCollider.height;
        modelInfo.distToGround = modelInfo.height * 0.5f;
    }
    #endregion
    #region Character State
    void CheckGrounded()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, modelInfo.distToGround + 0.2f);
        characterState.isGrounded = isGrounded;
        rigBody.useGravity = !isGrounded;
        characterState.isJumping = !isGrounded;
    }

    void CheckSlope()
    {
        // TODO: maxSlopeAngle
        bool isHit = Physics.SphereCast(transform.position, capsCollider.radius, Vector3.down, out RaycastHit hit, modelInfo.distToGround + 0.01f);
        surfaceProp.slopeNormal = hit.normal;
        characterState.isOnSlope = hit.normal.y != 1.0f && isHit ? true : false;
        if (characterState.isOnSlope) characterState.isGrounded = true;
    }

    void CheckMoving()
    {
        characterState.isMoving = (rigBody.velocity.sqrMagnitude > 0) ? true : false;
    }
    #endregion
    #region Target Information
    void CheckArrived()
    {
        bool isArrived = Vector3.Distance(transform.position, targetObj.transform.position) < 2f;
        characterState.isArrived = isArrived;
        targetName = targetObj.name;
    }

    void CheckTargetChange()
    {
        if (targetObj != null && targetName != targetObj.name)
            characterState.isArrived = false;
    }

    Vector3 GetDirectionToTarget()
    {
        Vector3 direction = targetObj.transform.position - transform.position;
        return direction.normalized;
    }

    Quaternion GetAngleToTarget()
    {
        // rotation towards target
        Vector3 direction = GetDirectionToTarget();
        Quaternion rotation = Quaternion.LookRotation(direction);

        float angle = rotation.y * 100;
        if (angle > 90) angle += 90;

        return Quaternion.Euler(new Vector3(0, angle, 0));
    }

    void CheckTarget()
    {
        CheckTargetChange();

        // select object in inspector to move character to selected object, it will stop moving when arrived
        if (targetObj != null && !characterState.isArrived)
        {
            Quaternion targetAngle = GetAngleToTarget();
            Vector3 targetDirection = GetDirectionToTarget();
            inputDir = targetDirection;

            Rotate(targetAngle);
            Move();

            CheckArrived();
        }
    }
    #endregion

    protected virtual void Move()
    {
        moveDir = inputDir;

        if (characterState.isOnSlope)
        {
            moveDir = Vector3.ProjectOnPlane(inputDir, surfaceProp.slopeNormal);
        }

        if (characterState.isCollided &&
        surfaceProp.collisionAngle <= -90 && surfaceProp.collisionAngle >= -180 ||
        surfaceProp.collisionAngle >= 90 && surfaceProp.collisionAngle <= 180)
        {
            if (surfaceProp.contactPoints != null)
                moveDir = Vector3.ProjectOnPlane(inputDir, surfaceProp.contactPoints[0].normal);
        }

        moveDir *= characterState.isRunning ? moveProp.speed * moveProp.RunningCoef : moveProp.speed;

        float yDir = characterState.isOnSlope && surfaceProp.contactPoints != null && surfaceProp.contactPoints[0].separation == 0 ? moveDir.y : rigBody.velocity.y; // when character is on slope

        // if (characterState.isGrounded)
        rigBody.velocity = new Vector3(moveDir.x, yDir, moveDir.z);
    }

    protected virtual void Jump()
    {
        if (characterState.isGrounded)
        {
            characterState.isJumping = true;
            moveDir = new Vector3(moveDir.x, transform.up.y * moveProp.jumpForce, moveDir.z);
            rigBody.velocity = moveDir;
        }
    }

    protected virtual void Rotate(Quaternion angle)
    {
        transform.rotation = angle;
    }

    void HandleMultiCollision()
    {
        if (surfaceProp.multiCollision)
        {

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawRay(transform.position, rigBody.velocity * 5);

        if (surfaceProp.contactPoints != null && surfaceProp.contactObj != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, moveDir * 2);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Debug.Log("collisionEnter with " + collision.gameObject.name);
        if (collision.gameObject.name != "Ground")
        {
            surfaceProp.contactPoints = collision.contacts;
            characterState.isCollided = true;
            if (surfaceProp.contactObj != null && surfaceProp.contactObj.name != collision.gameObject.name)
            {
                surfaceProp.multiCollision = true;
                Debug.Log("multicollision");
            }
            surfaceProp.contactObj = collision.gameObject;
            surfaceProp.collisionAngle = Vector3.SignedAngle(inputDir, surfaceProp.contactPoints[0].normal, Vector3.up);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        characterState.isCollided = false;
        surfaceProp.contactObj = null;
        surfaceProp.contactPoints = null;
        surfaceProp.multiCollision = false;
    }
}