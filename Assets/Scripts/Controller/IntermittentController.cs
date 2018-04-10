using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermittentController : MonoBehaviour {
    [SerializeField]
    private float m_RunTime;
    [SerializeField]
    private float m_RestTime;
    [SerializeField]
    private Behaviour[] m_IntermittentBehaviour;
    [SerializeField]
    private GameObject[] m_IntermittentGameObject;
	void Start () {
        StartCoroutine("IntermittentUpdate");
	}
	
	void Update () {
	}
    IEnumerator IntermittentUpdate()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_RunTime);

            foreach (var v in m_IntermittentBehaviour)
                v.enabled = false;
            foreach (var v in m_IntermittentGameObject)
                v.SetActive(false);


            yield return new WaitForSeconds(m_RestTime);

            foreach (var v in m_IntermittentBehaviour)
                v.enabled = true;
            foreach (var v in m_IntermittentGameObject)
                v.SetActive(true);
        }
    }
}
