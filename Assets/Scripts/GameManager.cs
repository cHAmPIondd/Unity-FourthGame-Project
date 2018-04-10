using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    [SerializeField]
    private bool m_IsSinglePlayer;
    public LevelController LevelController { get; private set; }
    private Transform m_Player1Transform;
    private Transform m_Player2Transform;

    public Transform Player1Transform
    {
        get
        {
            if (m_Player1Transform == null)
                m_Player1Transform = GameObject.Find("Hero1").transform;
            return m_Player1Transform;
        }
    }
    public Transform Player2Transform
    {
        get
        {
            if (m_IsSinglePlayer)
                return Player1Transform;
            if (m_Player2Transform == null)
                m_Player2Transform = GameObject.Find("Hero2").transform;
            return m_Player2Transform;
        }
    }
	// Use this for initialization
	void Awake () {
        Init();
        Load();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Init()
    {
        SavePointController.Init();
    }
    public void Load()
    {
        LevelController = GameObject.Find("Level").GetComponent<LevelController>();
    }
}
