using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeBouncedComponent : MonoBehaviour {
    private PhysicsUpdate m_PhysicsUpdate;
    private Rigidbody2D m_Rigidbody2D;
    void Awake()
    {
        m_PhysicsUpdate=GetComponent<PhysicsUpdate>();
        m_Rigidbody2D=GetComponent<Rigidbody2D>();
    }
    public void BeBounced(float velocity)
    {
        m_PhysicsUpdate.AddVelocity("Jump", new Vector2(0, velocity));
    }
    public bool CanBounced()
    {
        if (m_PhysicsUpdate.IsDied)
            return false;
        return m_Rigidbody2D.velocity.y <= 0;
    }
}
