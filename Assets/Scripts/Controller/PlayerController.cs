using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D),typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    #region Component
    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody2D;
    private BoxCollider2D m_BoxCollider2D;
    private PhysicsUpdate m_PhysicsUpdate;
    private LaunchController m_BoomLaunchController;
    private PlayerController m_OtherPlayerController;
    private Transform m_Model;
    #endregion
    #region PrivateAttribute
    [SerializeField]
    private int m_InputId=1;
    private bool m_IsJump = false;
    private bool m_IsRetaining;
    private bool m_IsAttracting;//吸
    private float m_JumpTimer;
    private float m_RetentionTimer;
    private float m_LastBoomTime;
    private bool IsLeft { get; set; }
    private Quaternion m_RotationTarget;
    private BoomController m_Boom;
    private List<BeAttractedComponent> m_BeAttractedList = new List<BeAttractedComponent>();
    private Vector2 m_CurrentAttractDirection;
    private float m_LastMoveVelocity;
    #endregion
    #region ParameterSetting
    [SerializeField, Range(0.01f, 0.4f), TooltipAttribute("与另一个玩家多远可以反弹")]
    private float m_SensitivityOfRebound = 0.1f;
    [SerializeField, TooltipAttribute("最大移动速度")]
    private float m_MoveRate;
    [SerializeField, TooltipAttribute("移动加速度")]
    private float m_MoveAddRate=10;
    [SerializeField, Range(1, 50), TooltipAttribute("转向速度")]
    private int m_RotateRate=5;
    [SerializeField, SetProperty("InitialJumpHeight"), TooltipAttribute("小跳跳跃高度")]
    private float m_InitialJumpHeight;
    public float InitialJumpHeight
    {
        get { return m_InitialJumpHeight; }
        set
        {
            if (m_Rigidbody2D == null)
                m_Rigidbody2D = GetComponent<Rigidbody2D>();
            if (m_PhysicsUpdate == null)
                m_PhysicsUpdate = GetComponent<PhysicsUpdate>();
            m_InitialJumpHeight = value;

            float m = m_Rigidbody2D.mass;
            float h = m_InitialJumpHeight;
            float g = m_PhysicsUpdate.Gravity;

            float t = Time.fixedDeltaTime;
            float t_2 = Mathf.Pow(Time.fixedDeltaTime, 2);

            float A = t_2 / (2 * g);
            float B = -t_2 / 2;
            float C = h;

            float a;
            if (A != 0)
            {
                a = (-B - Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);
            }
            else
            {
                a = -C / B;
            }
            m_JumpForce = a * m - g;
            JumpRetentionTime = m_JumpRetentionTime;
        }
    }
    [SerializeField, SetProperty("JumpForce"), TooltipAttribute("小跳跳跃力度")]
    private float m_JumpForce;
    public float JumpForce
    {
        get { return m_JumpForce; }
        set 
        {
            if (m_Rigidbody2D == null)
                m_Rigidbody2D = GetComponent<Rigidbody2D>();
            if (m_PhysicsUpdate==null)
                m_PhysicsUpdate=GetComponent<PhysicsUpdate>();
            m_JumpForce = value;

            float m=m_Rigidbody2D.mass;
            float t = Time.fixedDeltaTime;
            float f = m_JumpForce ;
            float h = m_InitialJumpHeight;
            float m_2 = Mathf.Pow(m, 2);
            float f_2 = Mathf.Pow(f,2);
            float t_2= Mathf.Pow(t, 2);


            float A = (m - 1) * t_2;
            float B = f * m * t_2 - 2 * f * t_2 - 2 * h * m_2;
            float C = -f_2 * t_2;
            if (A != 0)
            {
                m_PhysicsUpdate.Gravity = (-B - Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);
            }
            else
            {
                m_PhysicsUpdate.Gravity = -C / B;
            }
            JumpRetentionTime = m_JumpRetentionTime;
        }
    }
    [SerializeField, SetProperty("MaxJumpHeight"), TooltipAttribute("大跳跳跃高度")]
    private float m_MaxJumpHeight;
    public float MaxJumpHeight
    {
        get { return m_MaxJumpHeight; }
        set
        {
            if (m_Rigidbody2D == null)
                m_Rigidbody2D = GetComponent<Rigidbody2D>();
            if (m_PhysicsUpdate == null)
                m_PhysicsUpdate = GetComponent<PhysicsUpdate>();

            m_MaxJumpHeight = value;

            float m = m_Rigidbody2D.mass;
            float g = m_PhysicsUpdate.Gravity;
            float h2 = m_MaxJumpHeight;
            float f1 = m_JumpForce;
            float f2 = m_JumpRetentionForce;
            float a1 = (f1 + g) / m;
            float a2 = (f2 + g) / m;
            float t1 = Time.fixedDeltaTime;
            float t1_2 = Mathf.Pow(t1, 2);
            float a1_2 = Mathf.Pow(a1, 2);
            float a2_2 = Mathf.Pow(a2, 2);

            float A = g * a2 - a2_2;
            float B = 2 * a1 * t1 * g - 2 * a1 * a2 * t1;
            float C = a1 * g * t1_2 - a1_2 * t1_2 - 2 * g * h2;

            if (A != 0)
            {
                m_JumpRetentionTime = (-B - Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);
            }
            else
            {
                m_JumpRetentionTime = -C / B;
            }
        }
    }
    [SerializeField, SetProperty("JumpRetentionTime"), TooltipAttribute("小跳到大跳需要按的时长")]
    private float m_JumpRetentionTime;
    public float JumpRetentionTime
    {
        get { return m_JumpRetentionTime; }
        set
        {
            if (m_Rigidbody2D == null)
                m_Rigidbody2D = GetComponent<Rigidbody2D>();
            if (m_PhysicsUpdate == null)
                m_PhysicsUpdate = GetComponent<PhysicsUpdate>();

            m_JumpRetentionTime = value;

            float m = m_Rigidbody2D.mass;
            float g = m_PhysicsUpdate.Gravity;
            float h2 = m_MaxJumpHeight;
            float f1 = m_JumpForce;
            float a1 = (f1 + g) / m;
            float t1 = Time.fixedDeltaTime;
            float t2 = m_JumpRetentionTime;
            float t1_2 = Mathf.Pow(t1, 2);
            float t2_2 = Mathf.Pow(t2, 2);
            float a1_2 = Mathf.Pow(a1, 2);

            float A = -t2_2;
            float B = g*t2_2-2*a1*t1*t2;
            float C = a1*g*t1_2+2*g*a1*t1*t2-a1_2*t1_2-2*g*h2;

            float a2 = 0;
            if (A != 0)
            {
                a2 = (-B - Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);
            }
            else
            {
                a2 = -C / B;
            }
            m_JumpRetentionForce = m * a2 - g;
        }
    }
    [SerializeField, SetProperty("JumpRetentionForce"), TooltipAttribute("小跳到大跳中途不断加的力大小")]
    private float m_JumpRetentionForce;
    public float JumpRetentionForce
    {
        get { return JumpRetentionForce; }
        set
        {
            if (m_Rigidbody2D == null)
                m_Rigidbody2D = GetComponent<Rigidbody2D>();
            if (m_PhysicsUpdate == null)
                m_PhysicsUpdate = GetComponent<PhysicsUpdate>();

            m_JumpRetentionForce = value;

            float m = m_Rigidbody2D.mass;
            float g = m_PhysicsUpdate.Gravity;
            float h2 = m_MaxJumpHeight;
            float f1 = m_JumpForce;
            float f2 = m_JumpRetentionForce;
            float a1 = (f1 + g) / m;
            float a2 = (f2 + g) / m;
            float t1 = Time.fixedDeltaTime;
            float t1_2 = Mathf.Pow(t1, 2);
            float a1_2 = Mathf.Pow(a1, 2);
            float a2_2 = Mathf.Pow(a2, 2);

            float A = g * a2 - a2_2;
            float B = 2 * a1 * t1 * g - 2 * a1 * a2 * t1;
            float C = a1 * g * t1_2 - a1_2 * t1_2 - 2 * g * h2;

            if (A != 0)
            {
                m_JumpRetentionTime = (-B - Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);
            }
            else
            {
                m_JumpRetentionTime = -C / B;
            }
        }
    }

    [SerializeField, TooltipAttribute("人物互相反弹速度")]
    private float m_ReboundVelocity = 16f;
    [SerializeField, Range(0, 10), TooltipAttribute("反弹中每秒减少的速度（仅X轴）")]
    private float m_ReboundReduceRate=0.5f;
    [SerializeField, TooltipAttribute("被反弹后速度到达多小后才可以再次操作左右移动")]
    private float m_CanMoveReboundVelocity = 1;
    [SerializeField, TooltipAttribute("磁铁吸速度")]
    private float m_AttractedVelocity=6;
    [SerializeField, TooltipAttribute("磁铁吸距离")]
    private float m_AttractedLength = 5;
    [SerializeField, TooltipAttribute("磁铁吸Y轴重力转化的速度")]
    private float m_AttractedGravityVelocity = -4;
    #endregion
    #region CallBackFunctions
    void Awake()
    {
        m_Animator=GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
        m_PhysicsUpdate = GetComponent<PhysicsUpdate>();
        m_BoomLaunchController=GetComponent<LaunchController>();
        if (GameManager.Instance.Player1Transform==transform)
            m_OtherPlayerController = GameManager.Instance.Player2Transform.GetComponent<PlayerController>();
        else
            m_OtherPlayerController = GameManager.Instance.Player1Transform.GetComponent<PlayerController>();
        m_Model = transform.Find("Model");
    }
    void Start()
    {
        m_PhysicsUpdate.RegisterVelocity("Move", new VelocityAttribute(false, true, false, false, m_MoveAddRate / 4,false,false));
        m_PhysicsUpdate.RegisterVelocity("Jump", new VelocityAttribute(false, true, true, true, 0));
        m_PhysicsUpdate.RegisterVelocity("RetentionJump", new VelocityAttribute(false, true, false, false, 0));
        m_PhysicsUpdate.RegisterVelocity("Rebound", new VelocityAttribute(false, true, true, true, m_ReboundReduceRate));
    }
    void FixedUpdate()
    {
        RaycastHit2D[] hits;
        Collider2D[] colliders;
        m_Animator.SetBool("isGround", m_PhysicsUpdate.IsGround);
            //获取上一帧的移动速度
            m_LastMoveVelocity = m_PhysicsUpdate.GetVelocity("Move").x;
            //移动键判断
            if (Mathf.Abs(Vector2.SqrMagnitude(m_PhysicsUpdate.GetVelocity("Rebound"))) < m_CanMoveReboundVelocity * m_CanMoveReboundVelocity)
            {
                float horizontal = Input.GetAxis("Horizontal_" + m_InputId);

                //没继续按相同方向键并且在地上则移动速度置零
                if ((horizontal == 0 || m_LastMoveVelocity / horizontal <= 0) && m_PhysicsUpdate.IsGround)
                {
                    m_PhysicsUpdate.ResetVelocity("Move");
                    m_LastMoveVelocity = 0;
                } 
                //移动
                if (Mathf.Abs(horizontal) > 0.1f)
                {
                    float moveRate = horizontal * m_MoveAddRate * Time.fixedDeltaTime;
                    //超过最大速度则保持最大速度
                    if (moveRate + m_LastMoveVelocity > m_MoveRate)
                    {
                        moveRate = m_MoveRate - m_LastMoveVelocity;
                    }
                    else if (moveRate + m_LastMoveVelocity < -m_MoveRate)
                    {
                        moveRate = -m_MoveRate - m_LastMoveVelocity;
                    }
                    m_PhysicsUpdate.AddVelocity("Move", new Vector2(moveRate, 0));
                }
                TurnRotation(horizontal);
            }
            if (!m_IsAttracting)//没在用磁铁吸
            {
            //技能一炸弹
            if (Input.GetButtonDown("Skill1_" + m_InputId))
            {
                if (m_Boom)
                {
                    if (m_Boom.CanBoom)
                    {
                        m_Boom.Boom();
                        m_LastBoomTime = Time.time;
                    }
                }
                else if (Time.time > m_LastBoomTime + 0.1f)
                {
                    m_BoomLaunchController.LaunchPos=new Vector2((IsLeft?-1:1)*Mathf.Abs(m_BoomLaunchController.LaunchPos.x),m_BoomLaunchController.LaunchPos.y);
                    m_BoomLaunchController.LaunchDirection = IsLeft ? Vector2.left : Vector2.right;
                    m_Boom=(BoomController)m_BoomLaunchController.Launch();
                }
            }
        }
        else
        {
    //        m_PhysicsUpdate.ResetVelocity("Move");
        }
        //大跳
        if (Input.GetButton("Jump_" + m_InputId))
        {
            if (m_IsRetaining)
            {
                m_RetentionTimer += Time.fixedDeltaTime;
                if (m_RetentionTimer > m_JumpRetentionTime)
                {
                    m_RetentionTimer = 0;
                    m_IsRetaining = false;
                }
                else
                    m_PhysicsUpdate.AddVelocity("RetentionJump", new Vector2(0, m_JumpRetentionForce / m_Rigidbody2D.mass * Time.fixedDeltaTime));
            }
        }
        //跳跃键判断
        if (Input.GetButtonDown("Jump_" + m_InputId))
        {
            m_IsJump = true;
        }
        if (m_IsJump)
        {
            m_JumpTimer += Time.fixedDeltaTime;
            if (m_JumpTimer > 0.3f)
            {
                m_JumpTimer = 0;
                m_IsJump = false;
            }
            //反弹
            if (transform.parent == null)
            {
                colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + m_BoxCollider2D.offset, m_BoxCollider2D.size + new Vector2(m_SensitivityOfRebound, m_SensitivityOfRebound), 0, 1 << 9);
                foreach (var collider in colliders)
                {
                    if (collider.gameObject != gameObject)
                    {
                        if (m_OtherPlayerController.transform.parent == null)
                        {
                            BeRebound((transform.position - collider.transform.position).normalized * m_ReboundVelocity);
                            m_OtherPlayerController.BeRebound((-transform.position + collider.transform.position).normalized * m_ReboundVelocity);
                            break;
                        }
                    }
                }
            }
            //跳跃
            if (m_PhysicsUpdate.IsGround && m_IsJump)
            {
                //判断上方有没有东西
                bool canJump = true;
                hits = Physics2D.BoxCastAll((Vector2)transform.position + m_BoxCollider2D.offset, m_BoxCollider2D.size, 0, new Vector2(0, 1), 0.1f, m_PhysicsUpdate.ColliderLayer);
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject != gameObject)
                    {
                        canJump = false;
                        break;
                    }
                }
                if (canJump)
                {
                    m_JumpTimer = 0;
                    m_IsJump = false;
                    m_PhysicsUpdate.AddVelocity("Jump", new Vector2(0, m_JumpForce / m_Rigidbody2D.mass * Time.fixedDeltaTime));
                    m_IsRetaining = true;
                }
            }
        }
        //放开跳跃键
        if (Input.GetButtonUp("Jump_" + m_InputId))
        {
            m_RetentionTimer = 0;
            m_IsRetaining = false;
        }

        //技能二磁铁
        if (Input.GetButton("Skill2_" + m_InputId))
        {
            float horizontal = Input.GetAxis("Horizontal_" + m_InputId);
            float vertical = Input.GetAxis("Vertical_" + m_InputId);
          //  TurnRotation(horizontal);
            if (!m_IsAttracting)
            {//第一帧进入吸
                m_IsAttracting = true;
                if(m_CurrentAttractDirection==Vector2.zero)
                    m_CurrentAttractDirection = IsLeft ? Vector2.left : Vector2.right;
                if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
                {
                    if (horizontal > 0.1f)
                        m_CurrentAttractDirection = Vector2.right;
                    else if (horizontal < -0.1f)
                        m_CurrentAttractDirection = Vector2.left;
                }
                else if (Mathf.Abs(horizontal) < Mathf.Abs(vertical))
                {
                    if (vertical > 0.1f)
                        m_CurrentAttractDirection = Vector2.up;
                    else if (vertical < -0.1f)
                        m_CurrentAttractDirection = Vector2.down;
                }
            }
            foreach (var t in m_BeAttractedList)
            {
                t.CancelAttracted(-m_CurrentAttractDirection * m_AttractedVelocity);
            }
            m_BeAttractedList.Clear();
            if (m_CurrentAttractDirection.x != 0)
            {
                colliders = Physics2D.OverlapBoxAll(new Vector2(m_CurrentAttractDirection.x * (m_BoxCollider2D.size.x + m_AttractedLength) / 2, 0) + (Vector2)transform.position, new Vector2(m_AttractedLength, m_BoxCollider2D.size.y * 0.75f), 0);
                foreach (var c in colliders)
                {
                    if (c.gameObject != gameObject)
                    {
                        var beA = c.transform.GetComponent<BeAttractedComponent>();
                        if (beA != null&&beA.isActiveAndEnabled)
                        {
                            beA.BeAttracted(-m_CurrentAttractDirection * m_AttractedVelocity);
                            m_BeAttractedList.Add(beA);
                        }
                    }
                }
            }
            else if (m_CurrentAttractDirection.y != 0)
            {
                colliders = Physics2D.OverlapBoxAll(new Vector2(0, m_CurrentAttractDirection.y * (m_BoxCollider2D.size.y + m_AttractedLength) / 2) + (Vector2)transform.position, new Vector2(m_BoxCollider2D.size.x * 0.75f, m_AttractedLength), 0);
                foreach (var c in colliders)
                {
                    if (c.gameObject != gameObject)
                    {
                        var beA = c.transform.GetComponent<BeAttractedComponent>();
                        if (beA != null && beA.isActiveAndEnabled)
                        {
                            m_BeAttractedList.Add(beA);
                            beA.BeAttracted(-m_CurrentAttractDirection * m_AttractedVelocity);
                        }
                    }
                }
            }
            else
                Debug.LogError("m_CurrentAttractDirection is wrong!");
        }
        else if (m_IsAttracting)
        {//第一帧退出吸
            m_IsAttracting = false;
            foreach (var t in m_BeAttractedList)
            {
                t.CancelAttracted(-m_CurrentAttractDirection * m_AttractedVelocity);
            }
            m_BeAttractedList.Clear();
        }

        //磁铁反作用力
        if (m_BeAttractedList.Count > 0)
        {
            if (m_CurrentAttractDirection == Vector2.left || m_CurrentAttractDirection == Vector2.right)
                m_PhysicsUpdate.AddVelocity("AttractX", m_CurrentAttractDirection * m_AttractedVelocity);
            else
                m_PhysicsUpdate.AddVelocity("AttractY", m_CurrentAttractDirection * m_AttractedVelocity);
        }
        //上下吸时重新计算重力
        if (m_BeAttractedList.Count != 0 && m_CurrentAttractDirection != Vector2.left && m_CurrentAttractDirection != Vector2.right)
        {
            m_PhysicsUpdate.ResetVelocity("Gravity");
            m_PhysicsUpdate.AddVelocity("Gravity", new Vector2(0, m_AttractedGravityVelocity));
        }
    }
   
    public void BeRebound(Vector2 velocity)
    {
        m_IsJump = false;
        m_JumpTimer = 0;
        m_PhysicsUpdate.AddVelocity("Rebound", velocity);
    }

    private void TurnRotation(float horizontal)
    {
        //转变方向
        if (horizontal > 0.1f)
        {
            IsLeft = false;
            m_RotationTarget = Quaternion.Euler(new Vector3(0, -270, 0));
        }
        else if (horizontal < -0.1f)
        {
            IsLeft = true;
            m_RotationTarget = Quaternion.Euler(new Vector3(0, -90, 0));
        }
        m_Animator.SetFloat("rate", horizontal);

        m_Model.transform.rotation = Quaternion.Slerp(m_Model.transform.rotation, m_RotationTarget, m_RotateRate * Time.fixedDeltaTime);
    }
    #endregion
    private void OnDrawGizmosSelected()
    {
        m_BoxCollider2D=GetComponent<BoxCollider2D>();
        Vector2 currentLeft = (Vector2)transform.position + new Vector2(-m_BoxCollider2D.size.x / 2, m_BoxCollider2D.size.y / 2);
        Vector2 currentRight = (Vector2)transform.position + new Vector2(m_BoxCollider2D.size.x / 2, -m_BoxCollider2D.size.y / 2);
        Rigidbody2D rd= GetComponent<Rigidbody2D>();
        float gAdd = GetComponent<PhysicsUpdate>().Gravity / rd.mass;
        float deltaTime = 0.02f;
        float gVelo = -0.5f * gAdd * deltaTime;
        Gizmos.color = Color.blue;
        for (int i = 0; i < 80; i++)
        {
            gVelo += (gAdd * deltaTime);
            Gizmos.DrawLine(currentLeft, currentLeft + (new Vector2(0, m_JumpForce / rd.mass * Time.fixedDeltaTime) + new Vector2(0, gVelo) + new Vector2(m_MoveRate, 0)) * deltaTime);
            Gizmos.DrawLine(currentRight, currentRight + (new Vector2(0, m_JumpForce / rd.mass * Time.fixedDeltaTime) + new Vector2(0, gVelo) + new Vector2(m_MoveRate, 0)) * deltaTime );
            currentLeft = currentLeft + (new Vector2(0, m_JumpForce / rd.mass * Time.fixedDeltaTime) + new Vector2(0, gVelo) + new Vector2(m_MoveRate, 0)) * deltaTime;
            currentRight = currentRight + (new Vector2(0, m_JumpForce / rd.mass * Time.fixedDeltaTime) + new Vector2(0, gVelo) + new Vector2(m_MoveRate, 0)) * deltaTime;
        }
        currentLeft = (Vector2)transform.position + new Vector2(-m_BoxCollider2D.size.x / 2, m_BoxCollider2D.size.y / 2);
        currentRight = (Vector2)transform.position + new Vector2(m_BoxCollider2D.size.x / 2, -m_BoxCollider2D.size.y / 2);
        gAdd = GetComponent<PhysicsUpdate>().Gravity / rd.mass;
        float jumpRetentionAdd = m_JumpRetentionForce / rd.mass ;
        gVelo = -0.5f * gAdd  * deltaTime;
        gVelo -= jumpRetentionAdd * deltaTime;
        Gizmos.color = Color.red;
        for (int i = 0; i < 80; i++)
        {
            gVelo += (gAdd * deltaTime);
            if(i*deltaTime<=m_JumpRetentionTime)
                gVelo+=(jumpRetentionAdd*deltaTime);
            Gizmos.DrawLine(currentLeft, currentLeft + (new Vector2(0, m_JumpForce / rd.mass * Time.fixedDeltaTime) + new Vector2(0, gVelo) + new Vector2(m_MoveRate, 0)) * deltaTime);
            Gizmos.DrawLine(currentRight, currentRight + (new Vector2(0, m_JumpForce / rd.mass * Time.fixedDeltaTime) + new Vector2(0, gVelo) + new Vector2(m_MoveRate, 0)) * deltaTime);
            currentLeft = currentLeft + (new Vector2(0, m_JumpForce / rd.mass * Time.fixedDeltaTime) + new Vector2(0, gVelo) + new Vector2(m_MoveRate, 0)) * deltaTime;
            currentRight = currentRight + (new Vector2(0, m_JumpForce / rd.mass * Time.fixedDeltaTime) + new Vector2(0, gVelo) + new Vector2(m_MoveRate, 0)) * deltaTime;
        }
    }
}
