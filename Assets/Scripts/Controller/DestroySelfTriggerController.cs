using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfTriggerController : MonoBehaviour {
    [SerializeField]
    private LayerMask m_DestroySelfLayer;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(((int)(Mathf.Pow(2,other.gameObject.layer))&m_DestroySelfLayer.value)!=0)
        {
            if(!other.isTrigger)
                Destroy(gameObject);
        }
    }
}
