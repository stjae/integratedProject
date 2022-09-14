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
    public ContactPoint[] contactPoints;
    public GameObject contactObj;

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

    public class GroundProperty
    {
        public float maxSlopeAngle;
        public Vector3 slopeNormal;
    }

    public CharacterState characterState = new CharacterState();
    public MovementProperty moveProp = new MovementProperty();
    private ModelInfo modelInfo = new ModelInfo();
    private GroundProperty groundProp = new GroundProperty();

    protected virtual void Awake()
    {
        InitComponent();
        GetModelInfo();

    }

    protected virtual void FixedUpdate()
    {
        CheckGrounded();
        CheckSlope();
        CheckMoving();
        // CheckForwardBlocked();
        CheckTarget();
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
        if (characterState.isJumping && isGrounded && rigBody.velocity.y == 0f) characterState.isJumping = false;
    }

    void CheckSlope()
    {
        // TODO: maxSlopeAngle
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, modelInfo.distToGround + 0.2f);
        groundProp.slopeNormal = hit.normal;
        characterState.isOnSlope = hit.normal != Vector3.up && characterState.isGrounded ? true : false;
        rigBody.useGravity = !characterState.isOnSlope;
    }

    void CheckMoving()
    {
        characterState.isMoving = (rigBody.velocity.sqrMagnitude > 0) ? true : false;
    }

    // void CheckForwardBlocked()
    // {
    //     bool isBlocked = Physics.Raycast(transform.position, Vector3.forward, 0.1f);
    //     characterState.isForwardBlocked = isBlocked;
    // }
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

            Rotate(targetAngle);
            Move(targetDirection);

            CheckArrived();
        }
    }
    #endregion

    protected virtual void Move(Vector3 direction)
    {
        if (characterState.isOnSlope)
            moveDir = Vector3.ProjectOnPlane(direction, groundProp.slopeNormal);
        else if (!characterState.isGrounded) // for gizmo check
            moveDir = rigBody.velocity;
        else if (characterState.isCollided && direction.z > 0)
        {
            float dotProduct = Vector3.Dot(contactObj.transform.TransformDirection(Vector3.left), rigBody.transform.TransformDirection(new Vector3(0f, 0f, direction.z)));
            moveDir = contactObj.transform.TransformDirection(new Vector3(-dotProduct, 0f, 0f));
        }
        else
            moveDir = direction;
        moveDir *= characterState.isRunning ? moveProp.speed * moveProp.RunningCoef : moveProp.speed;

        float yDir = characterState.isOnSlope && !characterState.isJumping ? moveDir.y : rigBody.velocity.y;

        if (characterState.isGrounded)
            rigBody.velocity = new Vector3(moveDir.x, yDir, moveDir.z);
    }

    protected virtual void Jump()
    {
        characterState.isJumping = true;
        // rigBody.AddForce(Vector3.up * moveProp.jumpForce, ForceMode.Impulse);
        // rigBody.velocity = transform.up * moveProp.jumpForce;

        if (characterState.isCollided)
            moveDir = new Vector3(moveDir.x, transform.up.y * moveProp.jumpForce, 0f);
        else
            moveDir = new Vector3(moveDir.x, transform.up.y * moveProp.jumpForce, moveDir.z);
        // moveDir = new Vector3(0f, transform.up.y * moveProp.jumpForce, 0f);
        rigBody.velocity = moveDir;
    }

    protected virtual void Rotate(Quaternion angle)
    {
        transform.rotation = angle;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, moveDir * 5);
        // Gizmos.color = Color.blue;

        // if (contactPoints != null && contactObj != null)
        // {
        //     foreach (ContactPoint cp in contactPoints)
        //     {
        //         Gizmos.color = Color.red;
        //         // Gizmos.DrawRay(cp.point, contactObj.transform.TransformDirection(Vector3.forward) * 5);
        //         Gizmos.DrawRay(cp.point, rigBody.transform.TransformDirection(new Vector3(moveDir.x, 0f, 0f)) * 5); // this
        //         Gizmos.color = Color.green;
        //         // Gizmos.DrawRay(cp.point, contactObj.transform.TransformDirection(Vector3.left) * 5); // this
        //         Gizmos.DrawRay(cp.point, rigBody.transform.TransformDirection(new Vector3(0f, moveDir.y, 0f)) * 5);
        //         Gizmos.color = Color.blue;
        //         // Gizmos.DrawRay(cp.point, contactObj.transform.TransformDirection(Vector3.up) * 5);
        //         Gizmos.DrawRay(cp.point, rigBody.transform.TransformDirection(new Vector3(0f, 0f, moveDir.z)) * 5);
        //         Gizmos.color = Color.cyan;
        //         // Gizmos.DrawRay()
        //     }
        // }
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("collisionEnter with " + collision.gameObject.name);
        contactPoints = collision.contacts;
        if (collision.gameObject.name != "Ground")
        {
            characterState.isCollided = true;
            contactObj = collision.gameObject;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        characterState.isCollided = false;
        contactObj = null;
        contactPoints = null;
    }
}