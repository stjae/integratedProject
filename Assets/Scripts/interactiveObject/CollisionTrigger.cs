using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : BaseTrigger
{
    protected override void Awake() { base.Awake(); }

    private void OnTriggerEnter(Collider other)
    {
        if (m_state == STATE.INACTIVE) { return; }

        m_objectList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_state == STATE.INACTIVE) { return; }

        m_objectList.Remove(other.gameObject);
    }
}
