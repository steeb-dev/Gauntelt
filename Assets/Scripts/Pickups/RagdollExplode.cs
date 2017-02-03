using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollExplode : MonoBehaviour
{

    Rigidbody[] rigidBodies;
    private float m_Timer = 0f;
    private float m_LifeTime = 225f;
    // Use this for initialization
    void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer > m_LifeTime)
        { Destroy(this.gameObject); }
    }   
}
