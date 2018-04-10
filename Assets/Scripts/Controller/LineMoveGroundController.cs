using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMoveGroundController : MonoBehaviour
{
    [SerializeField]
    private bool m_IsRelative;
    [SerializeField,Tooltip("米/秒")]
    private float m_MoveRate=2;
    [Serializable]
    public class Point
    {
        public Vector2 Pos;
        public float WaitTime;
    }
    [SerializeField]
    private float m_InitialWaitTime=0;
    [SerializeField]
    private List<Point> m_PointList=new List<Point>();


    private int m_CurrentIndex=1;
    private Vector2 m_Offset;
    // Use this for initialization
    void Start()
    {
        if (m_IsRelative)
        {
            m_PointList[0].Pos = Vector2.zero;
            m_Offset = transform.position;
        }
        else
        {
            transform.position = m_PointList[0].Pos;
            m_Offset = Vector2.zero;
        }
        StartCoroutine("MoveUpdate");
	}
	
    IEnumerator MoveUpdate()
    {
        yield return new WaitForSeconds(m_InitialWaitTime);
        while (true)
        {
            while (true)
            {
                if (Vector2.SqrMagnitude((m_PointList[m_CurrentIndex].Pos+m_Offset)-(Vector2)transform.position) < Mathf.Pow(m_MoveRate * Time.deltaTime, 2))
                {
                    transform.position = m_PointList[m_CurrentIndex].Pos+m_Offset;
                    break;
                }
                else
                {
                    transform.position += (Vector3)((m_PointList[m_CurrentIndex].Pos + m_Offset)-(Vector2)transform.position).normalized * m_MoveRate * Time.deltaTime;
                }
                yield return 0;
            }
            yield return new WaitForSeconds(m_PointList[m_CurrentIndex].WaitTime);
            m_CurrentIndex = (m_CurrentIndex + 1) % m_PointList.Count;
        }
    }
    void OnDrawGizmosSelected()
    {
        Vector2 offset = Vector2.zero;
        if(m_IsRelative)
            offset = transform.position;
        if (m_PointList.Count > 0)
        {
            Gizmos.color=Color.red;
            Gizmos.DrawWireCube(m_PointList[0].Pos + offset, Vector3.one * 0.5f);
            for(int i=0;i<m_PointList.Count-1;i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(m_PointList[i].Pos + offset, m_PointList[i + 1].Pos + offset);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(m_PointList[i + 1].Pos + offset, 0.25f);
            }
            Gizmos.color = Color.black;
            Gizmos.DrawLine(m_PointList[m_PointList.Count - 1].Pos + offset, m_PointList[0].Pos + offset);
        }
    }
}
