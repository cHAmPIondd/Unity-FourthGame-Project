using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGateController : MonoBehaviour {
    [SerializeField]
    private Collider2D m_Collider2D;
    [SerializeField]
    private Transform m_GateModel;

    private BoxCollider2D m_BoxCollider2D;
    private bool m_IsOpening;
    private Vector3 m_InitialPos;
    private Vector3 m_TargetPos;
    void Awake()
    {
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        m_InitialPos = m_GateModel.position;
    }
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + m_BoxCollider2D.offset, m_BoxCollider2D.size, 0);
        m_IsOpening=false;
        foreach(var c in colliders)
        {
            if (c.GetComponent<CanTriggerGateComponent>() != null)
            {
                if (!m_IsOpening)
                {
                    m_IsOpening = true;
                    break;
                }
            }
        }
        if (m_IsOpening)
            OpenGate();
        else
            CloseGate();
        m_GateModel.position = Vector3.Lerp(m_GateModel.position,m_TargetPos,5*Time.deltaTime);
    }
    private void OpenGate()
    {
        m_TargetPos = m_InitialPos + new Vector3(0,0,1);
        m_Collider2D.enabled = false;
    }
    private void CloseGate()
    {
        m_TargetPos = m_InitialPos;
        m_Collider2D.enabled = true;
    }
}
