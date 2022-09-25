using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTrigger : BaseTrigger
{
    protected override void Awake() { base.Awake(); }

    public void Trigger(GameObject obj)
    {
        if (m_state == STATE.INACTIVE) { return; }

        m_objectList.Add(obj);
    }

    public void ClearObjectList() { m_objectList.Clear(); }
}
