using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Vector3 initPos;
    private Vector3 finalPos;
    public float moveX;
    public float moveY;
    public Button button;

    public float duration = 0.5f;
    private float timer = 0.0f;
    private bool isTriggered;
    private bool useButton;

    private void Awake()
    {
        initPos = transform.position;
        finalPos = transform.position + moveX * Vector3.right + moveY * Vector3.up;
        isTriggered = false;

        useButton = button != null;
    }

    private void Update()
    {
        if(useButton)
        {
            // door with button
            if (button.IsTriggered)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer -= Time.deltaTime;
            }
            timer = Mathf.Clamp(timer, 0.0f, duration);
            transform.position = timer * finalPos + (1 - timer) * initPos;
        }
        else
        {
            // automatic door
            if (isTriggered)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer -= Time.deltaTime;
            }
            timer = Mathf.Clamp(timer, 0.0f, duration);
            transform.position = timer * finalPos + (1 - timer) * initPos;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
        {
            isTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            isTriggered = false;
        }
    }
}
