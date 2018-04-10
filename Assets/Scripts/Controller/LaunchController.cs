using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchController : MonoBehaviour {
    
    [SerializeField, TooltipAttribute("预设体")]
    private GameObject m_BulletPrefab;
    [SerializeField, TooltipAttribute("发射物体的位置")]
    private Vector2 m_LaunchPos;
    public Vector2 LaunchPos { get { return m_LaunchPos; } set { m_LaunchPos = value; } }
    [SerializeField, TooltipAttribute("扔出去的方向")]
    protected Vector2 m_LaunchDirection=Vector2.left;
    public Vector2 LaunchDirection { get { return m_LaunchDirection; } set { m_LaunchDirection = value; } }
    [SerializeField, TooltipAttribute("扔出去的速度")]
    protected float m_LaunchVelocity = 6;
    [SerializeField]
    private bool m_IsAutomatic = false;
    [SerializeField]
    private float m_LaunchTime=1.5f;
	// Use this for initialization
	void OnEnable () {
        if (m_IsAutomatic)
            StartCoroutine("LaunchUpdate");
	}
	IEnumerator LaunchUpdate()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_LaunchTime);
            Launch();
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
    public BulletController Launch()
    {
        BulletController bullet = GameObject.Instantiate(m_BulletPrefab).GetComponent<BulletController>();
        bullet.Init(m_LaunchPos + (Vector2)transform.position, m_LaunchDirection.normalized * m_LaunchVelocity);
        return bullet;
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 offset = transform.position;
        Gizmos.DrawWireSphere(m_LaunchPos + offset, 0.2f);

        Vector2 current = m_LaunchPos + offset;
        float gAdd = m_BulletPrefab.GetComponent<PhysicsUpdate>().Gravity / m_BulletPrefab.GetComponent<Rigidbody2D>().mass;
        float deltaTime = 0.1f;
        float gVelo = -0.5f * gAdd * deltaTime;
        for (int i = 0; i < 30; i++)
        {
            gVelo += (gAdd * deltaTime);
            Gizmos.DrawLine(current, current + (m_LaunchDirection.normalized * m_LaunchVelocity + new Vector2(0, gVelo)) * deltaTime);
            current = current + (m_LaunchDirection.normalized * m_LaunchVelocity + new Vector2(0, gVelo)) * deltaTime;
        }
    }
}
