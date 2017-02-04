using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Woundable {
    public float m_EnemySpawnTime;  
    public GameObject m_Enemy;
    private float m_Timer = 0f;
    private ParticleSystem m_PSys;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    public int m_ScoreVal;
    // Use this for initialization
	void Update () {
        m_Timer += Time.deltaTime;
        if(m_Timer > m_EnemySpawnTime)
        {
            if(m_PSys == null)
            {
                m_PSys = GetComponent<ParticleSystem>();
            }
            m_Timer = 0f;
            m_PSys.Emit(500);
            GameObject go = Instantiate(m_Enemy);
            go.transform.position = this.transform.position - new Vector3(0,0,1f);
            go.transform.parent = this.transform.parent;
        }
    }
    
}
