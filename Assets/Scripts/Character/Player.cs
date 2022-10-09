using UnityEngine;

public static class UserInput
{
    public static float x;
    public static float z;

    public static Vector3 moveVector;
}

public class Player : BaseCharacter
{
    Camera _cam;

    void GetInputVector()
    {
        UserInput.x = Input.GetAxis("Horizontal");
        UserInput.z = Input.GetAxis("Vertical");

        UserInput.moveVector = transform.right * UserInput.x + transform.forward * UserInput.z;
    }

    void SetMoveVector()
    {
        _surface.AdjustVector();
        _collision.AdjustVector();

        MoveVector = UserInput.moveVector;
    }

    void SetCharacterAction()
    {
        SetRun = Input.GetKey(KeyCode.LeftShift) ? true : false;
        MoveVector *= IsRunning ? Speed * RunningCoef : Speed;

        if (Input.GetKey(KeyCode.Space) && IsGrounded)
        {
            SetJump = true;
            Jump();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            // Fire();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _cam = GetComponentInChildren<Camera>();
        _cam.gameObject.AddComponent<FirstPersonCamera>();
    }

    protected override void Update()
    {
        GetInputVector();
        SetMoveVector();
        SetCharacterAction();

        base.Update();
    }
}
