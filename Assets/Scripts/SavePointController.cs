using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointController : MonoBehaviour {
    private static BeDiedComponent s_Player1BeDiedComponent;
    private static BeDiedComponent s_Player2BeDiedComponent;
    private AudioSource m_AudioSource;
    private TextMesh m_TextMesh;
    void Awake()
    {
        m_AudioSource=GetComponent<AudioSource>();
        m_TextMesh = transform.Find("Model").GetComponent<TextMesh>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="Hero")
        {
            m_AudioSource.Play();
            s_Player1BeDiedComponent.ResurrectPos = transform.position;
            s_Player2BeDiedComponent.ResurrectPos = transform.position;
            m_TextMesh.color = Color.white;
            GetComponent<Collider2D>().enabled = false;
        }
    }
    public static void Init()
    {
        s_Player1BeDiedComponent = GameManager.Instance.Player1Transform.GetComponent<BeDiedComponent>();
        s_Player2BeDiedComponent = GameManager.Instance.Player2Transform.GetComponent<BeDiedComponent>();
    }
}
