using UnityEngine;

public class Player : BaseCharacter
{
    private void KeyInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveDir = transform.right * x + transform.forward * z;
        Move();

        if (Input.GetButtonDown("Jump") && characterState.isGrounded)
            Jump();
        if (Input.GetKey(KeyCode.LeftShift))
            characterState.isRunning = true;
        else
            characterState.isRunning = false;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    void InitComponent()
    {

    }

    void Update()
    {
        KeyInput();
    }
}
