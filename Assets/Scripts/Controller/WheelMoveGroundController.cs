using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMoveGroundController : MonoBehaviour {
    [SerializeField]
    private bool m_IsRelative;
    [SerializeField]
    private bool m_Inverse;
    [SerializeField]
    private float m_RotateRate = 30;
    [SerializeField]
    private Vector2 m_RotateCenter;

    private Vector2 m_Offset;
	// Use this for initialization
	void Start () {
        if (m_IsRelative)
            m_Offset = transform.position;
        else
            m_Offset = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(m_RotateCenter + m_Offset, m_Inverse ? Vector3.back : Vector3.forward, m_RotateRate * Time.deltaTime);
        transform.rotation = Quaternion.Euler(Vector3.zero);
	}
    void OnDrawGizmosSelected()
    {
        Vector2 offset = Vector2.zero;
        if (m_IsRelative)
            offset = transform.position;
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(m_RotateCenter + offset, Mathf.Sqrt(Vector2.SqrMagnitude((Vector2)transform.position - m_RotateCenter - offset)));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_RotateCenter + offset, 0.25f);
    }
}
