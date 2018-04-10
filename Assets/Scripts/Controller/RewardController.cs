using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardController : MonoBehaviour {
    private AudioSource m_AudioSource;
    private Transform m_Model;
	// Use this for initialization
    void Awake()
    {
        m_Model = transform.Find("Model");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="Hero")
        {
            GetComponent<AudioSource>().Play();
            GetComponent<BoxCollider2D>().enabled = false;
            m_Model.gameObject.SetActive(false);
            Destroy(gameObject, 2);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
