using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class Woundable : MonoBehaviour
{
    public int m_HP;
    public bool m_Invincible;
    public bool m_Dead;
    public Color m_HitColor;
    public Color m_DefaultColor;
    public float m_HitFlashTime;
    public GameObject m_RagDoll;
    private AudioSource m_Source;
    public AudioClip m_HitClip;
    public SkinnedMeshRenderer m_MeshRenderer;
    public delegate void KillConfirm(GameObject other);
    public KillConfirm OnKillConfirm;

    private void Start()
    {
        m_Source = GetComponent<AudioSource>();
        m_MeshRenderer.materials[0].color = m_DefaultColor;
 

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_Dead && !m_Invincible)
        {
            string ownerTag = "";
            GameObject ownerObject = other.gameObject;

            Vector3 relativePoint = new Vector3(0, 0, 0);
            Vector3 inverseDir = new Vector3(0, 0, 0);
            int damage = 0;
            if (other.gameObject.tag == "Weapon")
            {
                BatController bc = other.gameObject.GetComponent<BatController>();
                damage = (int)bc.GetDamage();
                bc.PlaySound();
                if (bc.m_Player.gameObject != null)
                {
                    ownerTag = bc.m_Player.gameObject.tag;
                    ownerObject = bc.m_Player.gameObject;
                }
            }
            else if (other.gameObject.tag == "Projectile")
            {
                Projectile p = other.gameObject.GetComponent<Projectile>();
                if (p != null)
                {
                    damage = (int)p.GetDamage();
                    p.PlaySound();
                    if (p.m_Player.gameObject != null)
                    {
                        ownerTag = p.m_Player.gameObject.tag;
                        ownerObject = p.m_Player.gameObject;
                    }
                }
            }
            if (damage > 0 && ownerTag != this.gameObject.tag)
            {
                this.m_HP -= damage;
                if (m_Source != null && m_HitClip != null)
                {
                    m_Source.clip = m_HitClip;
                    m_Source.Play();
                }
                if (m_HP <= 0)
                {
                    if(OnKillConfirm != null)
                    { OnKillConfirm(ownerObject); }
                    m_Dead = true;
                    StopAllCoroutines();
                    StartCoroutine(KillAfterDeath());
                }
                else
                {
                    StartCoroutine(Hit());
                }
            }
        }
    }

    IEnumerator Hit()
    {
        float t = 0;
        if (m_MeshRenderer != null)
        {
            m_MeshRenderer.materials[0].color = m_HitColor;
            while (t < m_HitFlashTime)
            {
                m_MeshRenderer.materials[0].color = Color.Lerp(m_HitColor, m_DefaultColor, t / m_HitFlashTime);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }



    IEnumerator KillAfterDeath()
    {
        yield return new WaitForSeconds(0.1f);
        if (m_RagDoll != null)
        {
            GameObject ragdoll = Instantiate(m_RagDoll);
            ragdoll.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = m_DefaultColor;
            ragdoll.transform.position = this.transform.position;
            ragdoll.transform.rotation = this.transform.rotation;
            ragdoll.transform.localScale = this.transform.localScale;
        }
        if (this.gameObject.tag != "Player")
        {
            Destroy(this.gameObject);
        }
    }

}