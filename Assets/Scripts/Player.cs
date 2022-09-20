using UnityEngine;
using UnityEngine.UI;

public class Player : BaseCharacter
{
    private Camera cam;
    private Image crossHair;
    private float crossHairDist = 3.0f;

    private void KeyInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        inputDir = transform.right * x + transform.forward * z;
        Move();

        if (Input.GetKey(KeyCode.LeftShift))
            characterState.isRunning = true;
        else
            characterState.isRunning = false;

        if (Input.GetButtonDown("Jump"))
            Jump();

        if (Input.GetKey(KeyCode.E))
            Interact();
    }

    protected override void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        crossHair = GameObject.Find("CrossHair").GetComponent<Image>();
        base.Awake();
    }

    void Start()
    {
        // cam = 
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    void Update()
    {
        KeyInput();
        CheckCrossHair();
    }

    void CheckCrossHair()
    {
        bool isHit = Physics.SphereCast(cam.transform.position, 0.015f, cam.transform.forward, out RaycastHit hit, capsCollider.radius * 2 * crossHairDist);

        var tempColor = crossHair.color;

        if (isHit)
            tempColor.a = 1.0f;
        else
            tempColor.a = 0.5f;

        crossHair.color = tempColor;
    }

    void Interact()
    {

    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     if (cam != null)
    //         Gizmos.DrawWireSphere(cam.transform.position + cam.transform.forward * capsCollider.radius * 2 * 2, 0.015f);
    // }
}
