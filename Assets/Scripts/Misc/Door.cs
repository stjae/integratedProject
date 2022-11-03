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

    private AudioSource audioSource;
    public AudioClip openingSFX;
    private bool isSFXplayed;

    private void Awake()
    {
        initPos = transform.position;
        finalPos = transform.position + moveX * Vector3.right + moveY * Vector3.up;
        isTriggered = false;

        useButton = button != null;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = openingSFX;
        isSFXplayed = false;
    }

    private void Update()
    {
        if(useButton)
        {
            // door with button
            if (button.IsTriggered)
            {
                timer += Time.deltaTime;

                if (openingSFX != null && !isSFXplayed)
                {
                    audioSource.Play();
                    isSFXplayed = true;
                }
            }
            else
            {
                timer -= Time.deltaTime;
                isSFXplayed = false;
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

                if (openingSFX != null && !isSFXplayed)
                {
                    audioSource.Play();
                    isSFXplayed = true;
                }
            }
            else
            {
                timer -= Time.deltaTime;
                isSFXplayed = false;
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
