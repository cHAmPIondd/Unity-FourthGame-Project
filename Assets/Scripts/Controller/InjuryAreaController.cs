using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjuryAreaController : MonoBehaviour {
    [SerializeField]
    private bool m_IsDestroySelf=false;
    void OnTriggerEnter2D(Collider2D other)
    {
        var beD = other.gameObject.GetComponent<BeDiedComponent>();
        if (beD != null && beD.isActiveAndEnabled)
        {
            beD.BeDied();
            if (m_IsDestroySelf)
                Destroy(gameObject);
        }
    }
}
