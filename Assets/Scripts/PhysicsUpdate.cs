using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUpdate : MonoBehaviour
{

    #region Component
    private BoxCollider2D m_BoxCollider2D;
    private Rigidbody2D m_Rigidbody2D;
    #endregion
    #region PrivateAttribute
    private Dictionary<string,VelocityAttribute> m_VelocityDictionary=new Dictionary<string,VelocityAttribute>();
    #endregion
    #region PublicAttribute
    public float Gravity=-9.81f;
    public bool IsGround { get; private set; }
    public bool IsPlatfond { get; private set; }
    public bool IsDied { get; set; }
    #endregion
    #region ParameterSetting
    [SerializeField, Range(0.01f, 0.4f), TooltipAttribute("距离地面多远则判断为在地面上")]
    private float m_SensitivityOfGround = 0.03f;
    [SerializeField, TooltipAttribute("落地时清除所有速度")]
    private bool m_IsGroundClear = true;
    public bool IsGroundClear { get { return m_IsGroundClear; } set { m_IsGroundClear = value; } }
    [SerializeField, TooltipAttribute("能碰撞的层级")]
    public LayerMask ColliderLayer;
    [SerializeField, TooltipAttribute("碰撞弹力系数"),Range(0,1)]
    private float m_ColliderBounciness=0;
    public float ColliderBounciness { get { return m_ColliderBounciness; } set { m_ColliderBounciness = value; } }
    #endregion
	// Use this for initialization
    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
    }
	void Start () {
		RegisterVelocity("Gravity",new VelocityAttribute(false,true,false,false,0));
	}

    void FixedUpdate()
    {
        RaycastHit2D[] hits;
        GravityVelocityUpdate();
        
        //结算速度
        m_Rigidbody2D.velocity = Vector2.zero;
        foreach(var v in m_VelocityDictionary.Values)
        {
            m_Rigidbody2D.velocity += v.Velocity;
        }
        //判断水平前方有无遮挡物
        hits = Physics2D.BoxCastAll((Vector2)transform.position + m_BoxCollider2D.offset, m_BoxCollider2D.size - new Vector2(0, 0.1f), 0, new Vector2(m_Rigidbody2D.velocity.x, 0), Mathf.Abs(m_Rigidbody2D.velocity.x * Time.fixedDeltaTime), ColliderLayer);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != gameObject)
            {
                if (hit.normal == Vector2.left || hit.normal == Vector2.right)
                {
                    foreach (var v in m_VelocityDictionary.Values)
                    {
                        if (v.IsCollider)
                            v.Velocity = new Vector2(-v.Velocity.x * m_ColliderBounciness, v.Velocity.y);
                    }
                    m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
                    break;
                }
            }
        }
        transform.parent = null;
        // IsPlatfond判断
        IsPlatfond = false;
        if (m_Rigidbody2D.velocity.y >= -0.1f)
        {
            hits = Physics2D.BoxCastAll((Vector2)transform.position + m_BoxCollider2D.offset + new Vector2(0, m_BoxCollider2D.size.y / 4), new Vector2(m_BoxCollider2D.size.x, m_BoxCollider2D.size.y / 2), 0, Vector2.up, m_SensitivityOfGround, ColliderLayer);
            foreach (var hit in hits)
            {
                if (hit.normal == Vector2.up || hit.normal == Vector2.down)
                {
                    if (hit.collider.gameObject != gameObject)
                    {
                        IsPlatfond = true;
                        foreach (var v in m_VelocityDictionary.Values)
                        {
                            if (v.IsCollider)
                                v.Velocity = new Vector2(v.Velocity.x, -v.Velocity.y * m_ColliderBounciness);
                        }
                        if (hit.collider.tag == "Ground")
                        {
                            transform.parent = hit.collider.transform;
                        }
                        break;
                    }
                }
            }
        }
        // IsGround判断
        IsGround = false;
        if (m_Rigidbody2D.velocity.y <= 0.1f)
        {
            hits = Physics2D.BoxCastAll((Vector2)transform.position + m_BoxCollider2D.offset - new Vector2(0, m_BoxCollider2D.size.y / 4), new Vector2(m_BoxCollider2D.size.x, m_BoxCollider2D.size.y / 2), 0, Vector2.down, m_SensitivityOfGround, ColliderLayer);
            foreach (var hit in hits)
            {
                if (hit.normal == Vector2.up)
                {
                    if (hit.collider.gameObject != gameObject)
                    {
                        IsGround = true;
                        foreach (var v in m_VelocityDictionary.Values)
                        {
                            if (v.IsCollider)
                                v.Velocity = new Vector2(v.Velocity.x, -v.Velocity.y * m_ColliderBounciness);
                            if (m_IsGroundClear&&v.IsGroundClear)
                                v.Velocity = Vector2.zero;
                        }
                        if (hit.collider.tag == "Ground")
                        {
                            transform.parent = hit.collider.transform;
                        }
                        else
                        {
                            transform.parent = hit.collider.transform.parent;
                        }
                        break;
                    }
                }
            }
        }
        AutoClearUpdate();
        AttenuationVelocityUpdate();
    }
	// Update is called once per frame
	void Update () {

    }
    private void GravityVelocityUpdate()
    {
        AddVelocity("Gravity",new Vector2(0, Gravity / m_Rigidbody2D.mass * Time.fixedDeltaTime));
    }
    private void AutoClearUpdate()
    {
        foreach(var v in m_VelocityDictionary.Values)
        {
            if (v.AutoClear)
                v.Velocity = Vector2.zero;
        }
    }
    private void AttenuationVelocityUpdate()
    {
        foreach(var v in m_VelocityDictionary.Values)
        {
            if(v.Velocity.x != 0)
            {
                float f = v.Velocity.x;
                v.Velocity -= new Vector2((v.Velocity.x > 0 ? v.AttenuationFactor : -v.AttenuationFactor) * Time.fixedDeltaTime,0);
                if (f / v.Velocity.x < 0)
                {
                    v.Velocity = new Vector2(0,v.Velocity.y);
                }
            } 
            if (v.Velocity.y != 0)
            {
                float f = v.Velocity.y;
                v.Velocity -= new Vector2(0,(v.Velocity.y > 0 ? v.AttenuationFactor : -v.AttenuationFactor) * Time.fixedDeltaTime);
                if (f / v.Velocity.y < 0)
                {
                    v.Velocity = new Vector2(v.Velocity.x,0);
                }
            }
        }
    }
    public Vector2 GetVelocity(string name)
    {
        return m_VelocityDictionary[name].Velocity;
    }
    public bool IsContainVelocity(string name)
    {
        return m_VelocityDictionary.ContainsKey(name);
    }
    public void RegisterVelocity(string name,VelocityAttribute v)
    {
        //是否字典里已经有这个速度
        if (!m_VelocityDictionary.ContainsKey(name))
        {
            m_VelocityDictionary.Add(name, v);
        }
    }
    public void CancelVelocity(string name)
    {
        //是否字典里已经有这个速度
        if (m_VelocityDictionary.ContainsKey(name))
        {
            m_VelocityDictionary.Remove(name);
        }
    }
    public void AddVelocity(string name, Vector2 v)
    {
        if (IsDied && name != "Gravity")
        {
            return;
        }
        //是否重置其它速度
        if (m_VelocityDictionary[name].IsAddClearOther)
        {
            foreach (var v2 in m_VelocityDictionary)
            {
                if (v2.Key != name&&v2.Value.IsOtherClear)
                    v2.Value.Velocity = Vector2.zero;
            }
        }
        //是否重置自己速度
        if (m_VelocityDictionary[name].IsAddClearSelf)
        {
            m_VelocityDictionary[name].Velocity = v;
        }
        else
        {
            m_VelocityDictionary[name].Velocity += v;
        }
    }
    public void ResetVelocity(string name = null)
    {
        if (name != null)
        {
            m_VelocityDictionary[name].Velocity = Vector2.zero;
        }
        else
        {
            foreach(var v in m_VelocityDictionary.Values)
            {
                v.Velocity = Vector2.zero;
            }
        }
    }
    public void InvertHorizontalVelocity()
    {
        foreach(var v in m_VelocityDictionary.Values)
        {
            v.Velocity = new Vector2(-v.Velocity.x,v.Velocity.y);
        }
    }
    public void ResetHorizontalVelocity()
    {
        foreach (var v in m_VelocityDictionary.Values)
        {
            v.Velocity = new Vector2(0, v.Velocity.y);
        }
    }
    public void InvertVerticalVelocity()
    {
        foreach (var v in m_VelocityDictionary.Values)
        {
            v.Velocity = new Vector2(v.Velocity.x, -v.Velocity.y);
        }
    }
    public void ResetVerticalVelocity()
    {
        foreach (var v in m_VelocityDictionary.Values)
        {
            v.Velocity = new Vector2(v.Velocity.x,0);
        }
    }
}
