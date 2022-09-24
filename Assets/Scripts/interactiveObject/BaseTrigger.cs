using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class BaseTrigger : MonoBehaviour
{
    BoxCollider m_boxCollider;

    public enum STATE { INACTIVE, ACTIVE, TRIGGERED };
    protected STATE m_state;
    protected List<GameObject> m_objects;

    public STATE GetState() { return m_state; }

    public void Inactivate()
    {
        if(m_state != STATE.INACTIVE)
        {
            m_boxCollider.enabled = false;
            m_state = STATE.INACTIVE;
            m_objects.Clear();
        }
    }
    public void Activate()
    {
        if (m_state == STATE.INACTIVE)
        {
            m_boxCollider.enabled = true;
            m_state = STATE.ACTIVE;
            m_objects.Clear();
        }
    }

    private void Awake()
    {
        m_boxCollider = GetComponent<BoxCollider>();
        m_boxCollider.isTrigger = true;

        m_objects = new List<GameObject>();

        Inactivate();
    }

    private void Update()
    {
        if(m_state == STATE.INACTIVE) { return; }

        m_state = (m_objects.Count > 0) ? STATE.TRIGGERED : STATE.ACTIVE;
    }
}
