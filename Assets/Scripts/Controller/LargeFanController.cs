using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LargeFanController : MonoBehaviour {
    private enum Curve
    {
        Linear,
        Quad
    }
    [SerializeField]
    private float m_RotateAngle=0;
    [SerializeField]
    private float m_MaxLength=8;
    [SerializeField]
    private float m_MaxForce = 30;
    [SerializeField]
    private Curve m_Curve;

    private Vector2 m_Offset;
    private AudioSource m_AudioSource;
    private BoxCollider2D m_BoxCollider2D;
    private List<BeFanedComponent> m_CurrentList = new List<BeFanedComponent>();
    private List<BeFanedComponent> m_LastList;
    void Awake()
    {
        m_AudioSource=GetComponent<AudioSource>();
        m_BoxCollider2D=GetComponent<BoxCollider2D>();
    }
    void Start()
    {
    }
    void FixedUpdate()
    {
        float angle = m_RotateAngle % 360;
        Vector2 direction = new Vector2(1, Mathf.Tan(angle * Mathf.Deg2Rad)).normalized;
        if ((angle >= 90 && angle <= 270) || (angle <= -90 && angle >= -270))
            direction = -direction;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, m_BoxCollider2D.size, 0, direction,m_MaxLength);
        m_LastList = m_CurrentList;
        m_CurrentList = new List<BeFanedComponent>();
        if(m_LastList.Count!=0&&m_AudioSource.volume!=1)
        {
            m_AudioSource.volume = 1;
            m_AudioSource.Play();
        }
        else if(m_LastList.Count==0&&m_AudioSource.volume==1)
        {
            DOTween.To(() => m_AudioSource.volume, x => m_AudioSource.volume = x, 0, 1f);
        }
        foreach (var h in hits)
        {
            var bf = h.transform.GetComponent<BeFanedComponent>();
            if (bf != null && bf.isActiveAndEnabled)
            {
                Vector2 force = direction;
                float distance = Mathf.Sqrt(Vector2.SqrMagnitude((Vector2)transform.position - h.point))-m_BoxCollider2D.size.x;
                if (distance < m_MaxLength)
                {
                    m_CurrentList.Add(bf);
                    m_LastList.Remove(bf);
                    switch (m_Curve)
                    {
                        case Curve.Linear: force *= (-m_MaxForce / m_MaxLength * distance + m_MaxForce); break;
                        case Curve.Quad: force *= (-m_MaxForce / m_MaxLength / m_MaxLength * distance * distance + m_MaxForce); break;
                    }
                    bf.BeFaned(force);
                }
            }
        }
        foreach (var bf in m_LastList)
        {
            bf.CancelBeFaned();
        }
    }
    void OnDrawGizmosSelected()
    {
        float angle = m_RotateAngle % 360 ;
        Vector2 targetPos = new Vector2(1, Mathf.Tan(angle * Mathf.Deg2Rad)).normalized * m_MaxLength;
        if ((angle >= 90 && angle <= 270)||(angle<=-90&&angle>=-270))
            targetPos = -targetPos;
        targetPos += (Vector2)transform.position;
        float width = GetComponent<BoxCollider2D>().size.x;

        Vector2 v2 = targetPos - (Vector2)transform.position;
        Vector2 normal = new Vector2(-v2.y, v2.x).normalized;
        //画区域
        Gizmos.DrawLine((Vector2)transform.position - normal * width / 2 + v2.normalized * width / 2, (Vector2)transform.position + normal * width / 2 + v2.normalized * width / 2);
        Gizmos.DrawLine(targetPos  - normal * width / 2 + v2.normalized * width / 2, targetPos  + normal * width / 2 + v2.normalized * width / 2);
        Gizmos.DrawLine((Vector2)transform.position - normal * width / 2 + v2.normalized * width / 2, targetPos  - normal * width / 2 + v2.normalized * width / 2);
        Gizmos.DrawLine((Vector2)transform.position + normal * width / 2+ v2.normalized*width/2, targetPos  + normal * width / 2 + v2.normalized * width / 2);
        //画箭头
        Gizmos.color = Color.black;
        Gizmos.DrawLine((Vector2)transform.position + v2.normalized * width / 2, targetPos  + v2.normalized * width / 2);
        Gizmos.DrawLine(targetPos  - normal * width / 4 + v2.normalized * width / 4, targetPos  + v2.normalized * width / 2);
        Gizmos.DrawLine(targetPos  + normal * width / 4 + v2.normalized * width / 4, targetPos  + v2.normalized * width / 2);
    }
}
