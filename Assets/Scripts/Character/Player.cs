using UnityEngine;

public static class UserInput
{
    public static float x;
    public static float z;

    public static Vector3 moveVector;
}

public class Player : BaseCharacter
{
    FirstPersonCamera _cam;
    TraceGun _traceGun;

    public FirstPersonCamera Camera { get { return _cam; } }

    void GetInputVector()
    {
        UserInput.x = Input.GetAxis("Horizontal");
        UserInput.z = Input.GetAxis("Vertical");

        UserInput.moveVector = _characterModel.transform.right * UserInput.x + _characterModel.transform.forward * UserInput.z;
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
            _traceGun.Fire();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        transform.Find("CameraHolder").gameObject.AddComponent<FirstPersonCamera>();
        _cam = transform.Find("CameraHolder").gameObject.GetComponent<FirstPersonCamera>();

        transform.Find("TraceGun").gameObject.AddComponent<TraceGun>();
        _traceGun = transform.Find("TraceGun").gameObject.GetComponent<TraceGun>();
    }

    protected override void Update()
    {
        GetInputVector();
        SetMoveVector();
        SetCharacterAction();

        base.Update();
    }
}
