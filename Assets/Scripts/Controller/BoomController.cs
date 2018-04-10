using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomController : BulletController
{
    #region PublicAttribute
    public bool CanBoom { get; private set; }
    #endregion
    #region PrivateAttribute
    private Rigidbody2D m_Rigidbody2D;
    private AudioSource m_AudioSource;
    private float m_BoomTimer;
    private bool m_IsColliderBounciness;
    #endregion
    #region ParameterSetting
    [SerializeField, TooltipAttribute("扔出去多少时间才可以引爆")]
    private float m_CanBoomTime = 1f;
    [SerializeField, TooltipAttribute("可以被炸的层级")]
    private LayerMask m_BoomLayer;
    [SerializeField, TooltipAttribute("爆炸范围")]
    private float m_BoomRange=1f;
    [SerializeField, TooltipAttribute("爆炸速度")]
    private float m_BoomVelocity = 6f;
    [SerializeField, TooltipAttribute("被炸后每秒减少速度")]
    protected float m_BoomReduceRate=2;
    #endregion
    // Use this for initialization
	// Update is called once per frame
    void Awake()
    {
        m_Rigidbody2D=GetComponent<Rigidbody2D>();
        m_AudioSource = GetComponent<AudioSource>();
    }
	void FixedUpdate () {
        if (!CanBoom)
        {
            m_BoomTimer += Time.fixedDeltaTime;
            if (m_BoomTimer > m_CanBoomTime)
            {
                CanBoom = true;
            }
        }
        else if (!m_IsColliderBounciness)
        {
            if (m_Rigidbody2D.velocity.y == 0)
            {
                m_IsColliderBounciness = true;
                m_PhysicsUpdate.ColliderBounciness = 0;
                m_PhysicsUpdate.IsGroundClear = true;
            }
        }
	}
    public void Boom()
    {
        m_AudioSource.Play();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_BoomRange, m_BoomLayer);
        foreach(var c in colliders)
        {
            var pu = c.transform.GetComponent<PhysicsUpdate>();
            if (pu!=null&&pu.isActiveAndEnabled)
            {
                pu.RegisterVelocity("BoomVelocity", new VelocityAttribute(false, true, true, true, m_BoomReduceRate));
                pu.AddVelocity("BoomVelocity", (c.transform.position - transform.position).normalized * m_BoomVelocity);
            }
        }
        GetComponent<Collider2D>().enabled = false;
        GetComponent<PhysicsUpdate>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;//等改Model
        Destroy(this);
        Destroy(gameObject,2f);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_BoomRange);
    }
}
