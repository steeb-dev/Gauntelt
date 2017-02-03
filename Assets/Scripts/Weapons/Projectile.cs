﻿using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float m_MinDamage;
    public float m_MaxDamage;
    public float m_LifeTime;
    private float m_TimeAlive;

    public AudioSource m_Audio;
    public AudioClip[] m_PunchClips;
    private Rigidbody m_RigidBody;
    public GameObject m_Player;

    public void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_TimeAlive += Time.deltaTime;
        if(m_TimeAlive > m_LifeTime)
        { Destroy(this.gameObject); }
    }

    public void Init(Vector3 trajectory)
    {
        m_RigidBody.AddForce(trajectory  * speed * m_RigidBody.mass * 10, ForceMode.Impulse);
    }

    public float GetDamage()
    {
        return Random.Range(m_MinDamage, m_MaxDamage);
    }

    public void PlaySound()
    {
        if (m_Audio != null)
        {
            m_Audio.Stop();
            m_Audio.PlayOneShot(m_PunchClips[Random.Range(0, m_PunchClips.Length)]);
        }
    }
}
