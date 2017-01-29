using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public float m_EnemySpawnTime;  
    public GameObject m_Enemy;
    private float m_Timer = 0f;
    private ParticleSystem m_PSys;

	// Use this for initialization
	void Start () {
        m_PSys = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        m_Timer += Time.deltaTime;
        if(m_Timer > m_EnemySpawnTime)
        {
            m_Timer = 0f;
            m_PSys.Emit(500);
            GameObject go = Instantiate(m_Enemy);
            go.transform.position = this.transform.position - new Vector3(0,0,1f);
        }
    }
}
