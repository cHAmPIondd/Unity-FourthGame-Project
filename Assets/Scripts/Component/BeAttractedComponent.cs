using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeAttractedComponent : MonoBehaviour
{
    private PhysicsUpdate m_PhysicsUpdate;
    private List<float> m_AttractXList = new List<float>();
    private List<float> m_AttractYList = new List<float>();
    [SerializeField, TooltipAttribute("是否无法被吸动")]
    private bool m_IsDecorate = false;
    [SerializeField, TooltipAttribute("被磁铁吸Y轴重力转化的速度")]
    private float m_BeAttractedGravityVelocity = -4;
	// Use this for initialization
    void Awake()
    {
        if (!m_IsDecorate)
        {
            m_PhysicsUpdate = GetComponent<PhysicsUpdate>();
        }
    }
    void Start()
    {
        if (!m_IsDecorate)
        {
            m_PhysicsUpdate.RegisterVelocity("AttractX", new VelocityAttribute(true, false, false, false, 0));
            m_PhysicsUpdate.RegisterVelocity("AttractY", new VelocityAttribute(true, false, false, true, 0));
        }
    }
	// Update is called once per frame
    void FixedUpdate()
    {
        if (!m_IsDecorate)
        {
            //被磁铁吸力
            foreach (var x in m_AttractXList)
                m_PhysicsUpdate.AddVelocity("AttractX", new Vector2(x, 0));
            foreach (var y in m_AttractYList)
                m_PhysicsUpdate.AddVelocity("AttractY", new Vector2(0, y));
            //如果被吸，重新计算重力
            if (m_AttractYList.Count != 0)
            {
                m_PhysicsUpdate.ResetVelocity("Gravity");
                m_PhysicsUpdate.AddVelocity("Gravity", new Vector2(0, m_BeAttractedGravityVelocity));
            }
        }
    }
    public void BeAttracted(Vector2 v2)
    {
        if (v2.x != 0)
        {
            m_AttractXList.Add(v2.x);
        }
        else if (v2.y != 0)
        {
            m_AttractYList.Add(v2.y);
        }
        else
            Debug.LogError("v2 is wrong!");
    }
    public void CancelAttracted(Vector2 v2)
    {
        if (v2.x != 0)
        {
            m_AttractXList.Remove(v2.x);
        }
        else if (v2.y != 0)
        {
            m_AttractYList.Remove(v2.y);
        }
        else
            Debug.LogError("v2 is wrong!");
    }
}
