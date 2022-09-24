using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTrigger : BaseTrigger
{
    public void Trigger(GameObject obj)
    {
        if (m_state == STATE.INACTIVE) { return; }

        m_objects.Add(obj);
    }
}
