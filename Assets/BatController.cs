using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour {

    public float m_MinDamage;
    public float m_MaxDamage;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float GetDamage()
    {
        return Random.Range(m_MaxDamage, m_MaxDamage);
    }
}
