using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBoardController : MonoBehaviour {
    [SerializeField]
    private float m_BounceVelocity=10;
    private BoxCollider2D m_BoxCollider2D;
    private AudioSource m_AudioSource;
    void Awake()
    {
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
        m_AudioSource=GetComponent<AudioSource>();
    }
    void FixedUpdate()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + m_BoxCollider2D.offset, m_BoxCollider2D.size, 0, Vector2.up, 0.2f);
        foreach(var h in hits)
        {
            if(h.normal==Vector2.down)
            {
                var bb = h.transform.GetComponent<BeBouncedComponent>();
                if (bb != null && bb.isActiveAndEnabled)
                {
                    if (bb.CanBounced())
                    {
                        m_AudioSource.Play();
                        bb.BeBounced(m_BounceVelocity);
                    }
                }
            }
        }
    }
}
