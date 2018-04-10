using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    [SerializeField]
    protected float m_BeThrownReduceRate=6;
    protected PhysicsUpdate m_PhysicsUpdate;
	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public virtual void Init(Vector3 position, Vector2 velocity)
    {
        m_PhysicsUpdate = GetComponent<PhysicsUpdate>();
        m_PhysicsUpdate.RegisterVelocity("Thrown", new VelocityAttribute(false, true, true, false, m_BeThrownReduceRate));
        transform.position = position;
        m_PhysicsUpdate.AddVelocity("Thrown", velocity);

    }
}
