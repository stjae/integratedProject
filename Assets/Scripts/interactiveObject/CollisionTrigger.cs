using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : BaseTrigger
{
    private void OnTriggerEnter(Collider other)
    {
        if (m_state == STATE.INACTIVE) { return; }

        m_objects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_state == STATE.INACTIVE) { return; }

        m_objects.Remove(other.gameObject);
    }
}
