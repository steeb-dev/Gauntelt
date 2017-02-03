﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public float m_EnemySpawnTime;  
    public GameObject m_Enemy;
    private float m_Timer = 0f;
    private ParticleSystem m_PSys;
    public float m_HP;
    private MeshRenderer m_MeshRenderer;

    public Color m_DefaultColor;
    public Color m_HitColor;
    public float m_HitFlashTime = 0.2f;
    public bool m_Dead = false;

    public int m_ScoreVal;
    // Use this for initialization
    void Start () {
        m_PSys = GetComponent<ParticleSystem>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MeshRenderer.materials[0].color = m_DefaultColor;
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
            go.transform.parent = this.transform.parent;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_Dead)
        {
            PlayerBehaviour pb = null;
            int damage = 0;
            if (other.gameObject.tag == "Weapon")
            {
                BatController bc = other.gameObject.GetComponent<BatController>();
                damage = (int)bc.GetDamage();
                pb = bc.m_Player.GetComponent<PlayerBehaviour>();

            }
            else if (other.gameObject.tag == "Projectile")
            {
                Projectile p = other.gameObject.GetComponent<Projectile>();
                damage = (int)p.GetDamage();
                Destroy(other.gameObject);
                pb = p.m_Player.GetComponent<PlayerBehaviour>();
            }
            if (damage > 0)
            {
                this.m_HP -= damage;
                StartCoroutine(Hit());
     
                m_PSys.Emit(100);

                if (m_HP <= 0)
                {
                    pb.m_Score += m_ScoreVal;
                    m_Dead = true;
                    StopAllCoroutines();
                    StartCoroutine(KillAfterDeath());
                }
            }
        }
    }


    IEnumerator KillAfterDeath()
    {
        m_MeshRenderer.materials[0].color = m_HitColor;
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

    IEnumerator Hit()
    {
        float t = 0;
        m_MeshRenderer.materials[0].color = m_HitColor;
        while (t < m_HitFlashTime)
        {
            m_MeshRenderer.materials[0].color = Color.Lerp(m_HitColor, m_DefaultColor, t / m_HitFlashTime);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}