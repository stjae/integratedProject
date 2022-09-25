using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class BaseTrigger : MonoBehaviour
{
    BoxCollider m_boxCollider;

    public enum STATE { INACTIVE, ACTIVE, TRIGGERED };
    protected STATE m_state;
    protected List<GameObject> m_objectList;

    public STATE GetState() { return m_state; }

    public void Deactivate()
    {
        m_boxCollider.enabled = false;
        m_state = STATE.INACTIVE;
        m_objectList.Clear();
    }
    public void Activate()
    {
        if (m_state == STATE.INACTIVE)
        {
            m_boxCollider.enabled = true;
            m_state = STATE.ACTIVE;
            m_objectList.Clear();
        }
    }

    protected virtual void Awake()
    {
        m_boxCollider = GetComponent<BoxCollider>();
        m_boxCollider.isTrigger = true;

        m_objectList = new List<GameObject>();

        Deactivate();
    }

    private void Update()
    {
        if(m_state == STATE.INACTIVE) { return; }

        m_state = (m_objectList.Count > 0) ? STATE.TRIGGERED : STATE.ACTIVE;
    }
}
