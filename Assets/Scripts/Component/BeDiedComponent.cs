using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeDiedComponent : MonoBehaviour {

    [SerializeField]
    private float m_DiedTime;
    [SerializeField]
    private MonoBehaviour m_Controller;
    [SerializeField]
    private bool m_CanResurrect;
    [SerializeField]
    private float m_ResurrectTime;
    [SerializeField]
    private Vector2 m_ResurrectPos;
    public Vector2 ResurrectPos { private get { return m_ResurrectPos; } set { m_ResurrectPos = value; } }

    private PhysicsUpdate m_PhysicsUpdate;
    void Awake()
    {
        m_PhysicsUpdate =GetComponent<PhysicsUpdate>();
    }
    public void BeDied()
    {
        if (m_Controller.isActiveAndEnabled)
        {
            //播放死亡动画
            GetComponent<Animator>().SetTrigger("died");
            m_PhysicsUpdate.IsDied = true;
            Vector2 gVelo = m_PhysicsUpdate.GetVelocity("Gravity");
            m_PhysicsUpdate.ResetVelocity("Move");
            m_Controller.enabled = false;

            if (!m_CanResurrect)
            {//真死
                Destroy(gameObject, m_DiedTime);
            }
            else
            {//假死
                Invoke("FeignDeath", m_DiedTime);
            }
        }
    }
    private void FeignDeath()
    {
        Invoke("Resurrect", m_ResurrectTime);
        gameObject.SetActive(false);
    }
    private void Resurrect()
    {
        //-->播放复活动画

        gameObject.SetActive(true);
        m_PhysicsUpdate.IsDied = false;
        m_Controller.enabled = true;
        transform.position = m_ResurrectPos;
    }
}
