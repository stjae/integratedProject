using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// example: button class using CollisionTrigger
public class Button : MonoBehaviour
{
    CollisionTrigger ref_colTrig;
    GameObject ref_cap;

    float m_capInitPosY;
    float m_capMovementRange = 0.15f;

    private void Start()
    {
        // caution: references should be cached in Start().
        ref_colTrig = GetComponentInChildren<CollisionTrigger>();
        ref_cap = GameObject.Find("Cap");

        ref_colTrig.Activate();
        m_capInitPosY = ref_cap.transform.localPosition.y;
    }

    private void Update()
    {
        switch(ref_colTrig.GetState())
        {
            case CollisionTrigger.STATE.ACTIVE:
                ref_cap.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                if (ref_cap.transform.localPosition.y < m_capInitPosY) { ref_cap.transform.position += 2.0f * Time.deltaTime * Vector3.up; }
                break;

            case CollisionTrigger.STATE.TRIGGERED:
                ref_cap.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                if (m_capInitPosY - ref_cap.transform.localPosition.y < m_capMovementRange) { ref_cap.transform.position += 2.0f * Time.deltaTime * Vector3.down; }
                break;
        }
    }
}