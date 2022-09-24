using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// example button class using CollisionTrigger
public class Button : MonoBehaviour
{
    CollisionTrigger m_colTrig;
    GameObject m_cap;

    float m_capInitPosY;
    float m_capMovementRange = 0.15f;

    private void Awake()
    {
        m_colTrig = GetComponentInChildren<CollisionTrigger>();
        m_cap = GameObject.Find("Cap");

        m_capInitPosY = m_cap.transform.localPosition.y;

        m_colTrig.Activate();
    }

    private void Update()
    {
        switch(m_colTrig.GetState())
        {
            case CollisionTrigger.STATE.ACTIVE:
                m_cap.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

                if (m_cap.transform.localPosition.y < m_capInitPosY)
                {
                    m_cap.transform.position += 2.0f * Time.deltaTime * Vector3.up;
                }
                break;
            case CollisionTrigger.STATE.TRIGGERED:
                m_cap.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

                if (m_capInitPosY - m_cap.transform.localPosition.y < m_capMovementRange)
                {
                    m_cap.transform.position += 2.0f * Time.deltaTime * Vector3.down;
                }
                break;
        }
    }
}