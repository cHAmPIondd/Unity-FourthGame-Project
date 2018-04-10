using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferController : MonoBehaviour {
    public enum Direction
    {
        Left=-1,
        Right=1,
        Up=-2,
        Down=2,
    }
    [SerializeField, Tooltip("入口朝哪开")]
    private Direction m_EntryDirection;
    [SerializeField]
    private BoxCollider2D m_EntryGate;
    [SerializeField, Tooltip("出口与入口是否同向")]
    private bool m_IsSameDirection;
    [SerializeField]
    private BoxCollider2D m_ExitGate;
    [SerializeField, Tooltip("是否为双向门,即能从出口走到入口")]
    private bool m_IsTwoWay;

    private AudioSource m_AudioSource;
	// Use this for initialization
	void Awake () {
		m_AudioSource=GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll((Vector2)m_EntryGate.transform.position + m_EntryGate.offset, m_EntryGate.size, 0);
        foreach (var c in colliders)
        {
            var bt = c.GetComponent<BeTransferComponent>();
            if (bt != null && bt.isActiveAndEnabled)
            {
                if (bt.CanBeTransfer(m_EntryDirection,transform))
                {
                    m_AudioSource.Play();
                    bt.BeTransfer(m_EntryDirection, m_IsSameDirection, (Vector2)m_ExitGate.transform.position + m_ExitGate.offset, transform);
                }
            }
        }
        if (m_IsTwoWay)
        {
            colliders = Physics2D.OverlapBoxAll((Vector2)m_ExitGate.transform.position + m_ExitGate.offset, m_ExitGate.size, 0);
            foreach (var c in colliders)
            {
                var bt = c.GetComponent<BeTransferComponent>();
                if (bt != null && bt.isActiveAndEnabled)
                {
                    if (bt.CanBeTransfer((Direction)(m_IsSameDirection ? (int)m_EntryDirection : -(int)m_EntryDirection), transform))
                    {
                        m_AudioSource.Play();
                        bt.BeTransfer((Direction)(m_IsSameDirection ? (int)m_EntryDirection : -(int)m_EntryDirection), m_IsSameDirection, (Vector2)m_EntryGate.transform.position + m_EntryGate.offset, transform);
                    }
                }
            }
        }
    }
}
