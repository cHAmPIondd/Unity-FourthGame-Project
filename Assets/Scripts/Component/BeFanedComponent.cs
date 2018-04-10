using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeFanedComponent : MonoBehaviour {
    private PhysicsUpdate m_PhysicsUpdate;
    private Rigidbody2D m_Rigidbody2D;
	// Use this for initialization
	void Awake () {
        m_PhysicsUpdate = GetComponent<PhysicsUpdate>();
        m_Rigidbody2D=GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void BeFaned(Vector2 force)
    {
        Vector2 velocity = force / m_Rigidbody2D.mass * Time.fixedDeltaTime;
        if (m_PhysicsUpdate.IsContainVelocity("ContinuedAddForceNotGroundClear"))
        {
            m_PhysicsUpdate.AddVelocity("ContinuedAddForceNotGroundClear", velocity);
        }
        else
        {
            m_PhysicsUpdate.RegisterVelocity("ContinuedAddForceNotGroundClear", new VelocityAttribute(false, true, false, false, 0, false));
            Vector2 ground = Vector2.zero;
            if (m_PhysicsUpdate.IsContainVelocity("ContinuedAddForce"))
            {
                ground = m_PhysicsUpdate.GetVelocity("ContinuedAddForce");
                m_PhysicsUpdate.CancelVelocity("ContinuedAddForce");
            }
            m_PhysicsUpdate.AddVelocity("ContinuedAddForceNotGroundClear", ground + velocity);
            Debug.Log(ground);
        }
    }
    public void CancelBeFaned()
    {
        m_PhysicsUpdate.RegisterVelocity("ContinuedAddForce", new VelocityAttribute(false, true, false, false, 0));
        m_PhysicsUpdate.AddVelocity("ContinuedAddForce", m_PhysicsUpdate.GetVelocity("ContinuedAddForceNotGroundClear"));
        m_PhysicsUpdate.CancelVelocity("ContinuedAddForceNotGroundClear");
    }
}
