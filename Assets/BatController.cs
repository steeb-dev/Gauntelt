using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour {

    public float m_MinDamage;
    public float m_MaxDamage;
    public GameObject m_Player;
    private BoxCollider m_Collider;
    public float m_WarmUp = 0.2f;
    public float m_CoolDown = 0.5f;
    bool m_Attacking = false;

    // Use this for initialization
    void Start () {
        m_Collider = GetComponent<BoxCollider>();
        m_Collider.enabled = false;
    }

    public void Attack()
    {
        if (!m_Attacking)
            StartCoroutine(AttackCo());
    }

    IEnumerator AttackCo()
    {
        m_Attacking = true;
        yield return new WaitForSeconds(m_WarmUp);
        ActivateCollider();
        yield return new WaitForSeconds(m_CoolDown);
        DeactivateCollider();
        m_Attacking = false;
    }

   public void ActivateCollider()
    { m_Collider.enabled = true; }

    public void DeactivateCollider()
    {
        StopAllCoroutines();
        m_Collider.enabled = false; 
    }

    public float GetDamage()
    {
        return Random.Range(m_MinDamage, m_MaxDamage);
    }
}
