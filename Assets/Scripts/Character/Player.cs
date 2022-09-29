using UnityEngine;

public static class UserInput
{
    public static float x;
    public static float z;

    public static Vector3 moveVector;
}

public class Player : BaseCharacter
{
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

        if (Input.GetKey(KeyCode.Space))
        {
            SetJump = true;
            Jump();
        }
    }

    protected override void Update()
    {
        GetInputVector();
        SetMoveVector();
        SetCharacterAction();

        base.Update();
    }
}
