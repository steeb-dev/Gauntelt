using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public float m_HP;
    public Color m_DefaultColor;
    public Color m_HitColor;
    public float m_HitFlashTime = 0.2f;

    public SkinnedMeshRenderer m_MeshRenderer;
    public ParticleSystem m_PSys;
    private Animator m_Anim;
    private Rigidbody m_RigidBody;
    public GameObject m_RagDoll;
    public GameObject m_TargetPlayer;
    private Vector3 m_PlayerDirection;
    private float m_TurnSpeed = 5.0f;
    public float m_RunSpeed = 1.0f;
    private int speedFloat;
    public float speedDampTime = 0.1f;
    public float m_AttackRange = 1f;
    public bool m_Attacking;
    public BatController m_Bat;
    public bool m_Dead;
    private int movingBool;
    Rigidbody[] rigidBodies;

    // Use this for initialization
    void Start ()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        movingBool = Animator.StringToHash("Moving");
        m_Anim = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_TargetPlayer = GameObject.FindWithTag("Player");
        m_MeshRenderer.materials[0].color = m_DefaultColor;
        speedFloat = Animator.StringToHash("Speed");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Explode"))
        {
            Vector3 explosionPos = new Vector3(this.transform.position.x, this.transform.position.y - 2, this.transform.position.z);
            foreach (Rigidbody rb in rigidBodies)
            {
                rb.AddExplosionForce(100000, explosionPos, 100);
            }
        }
        if (m_TargetPlayer != null)
        {
            m_PlayerDirection = m_TargetPlayer.transform.position - m_RigidBody.position;
            m_PlayerDirection.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_PlayerDirection), m_TurnSpeed * Time.deltaTime);           
        }  
    }

    void FixedUpdate()
    {
        bool moving = true;
        if (m_TargetPlayer != null)
        {
            float distance = Vector3.Distance(this.transform.position, m_TargetPlayer.transform.position);
            if (distance > m_AttackRange)
            {
                m_Bat.DeactivateCollider();
                m_RigidBody.AddForce(m_PlayerDirection * m_RunSpeed);
                m_Anim.SetFloat(speedFloat, m_RunSpeed, speedDampTime, Time.deltaTime);  
            }
            else
            {
                moving = false;
                if (!m_Attacking)
                {
                    m_RigidBody.AddForce(-m_PlayerDirection * m_RunSpeed);
                    m_Anim.SetFloat(speedFloat, 0, 0.05f, Time.deltaTime);
                    m_Attacking = true;                   
                }
                else
                {
                    m_Bat.ActivateCollider();
                    m_Anim.SetTrigger("Attack");
                }
            }
        }
        m_Anim.SetBool(movingBool, moving);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_Dead)
        {
            Vector3 relativePoint = new Vector3(0, 0, 0);
            Vector3 inverseDir = new Vector3(0, 0, 0);
            int damage = 0;
            if (other.gameObject.tag == "Weapon")
            {
                BatController bc = other.gameObject.GetComponent<BatController>();
                damage = (int)bc.GetDamage();
                relativePoint = transform.InverseTransformPoint(bc.m_Player.transform.position);     
                inverseDir = new Vector3(-relativePoint.x, relativePoint.y, -relativePoint.z);
            }
            else if (other.gameObject.tag == "Projectile")
            {
                Projectile p = other.gameObject.GetComponent<Projectile>();
                if (p != null)
                {
                    damage = (int)p.GetDamage();
                    relativePoint = transform.InverseTransformPoint(other.gameObject.transform.position);
                    Destroy(other.gameObject);
                }
            }
            if (damage > 0)
            {
                this.m_HP -= damage;
                StartCoroutine(Hit());
                HandleReactionAnimation(relativePoint);
                inverseDir = relativePoint;
                m_PSys.transform.rotation = Quaternion.Slerp(m_PSys.transform.rotation, Quaternion.LookRotation(relativePoint), 1f);
                m_PSys.Emit(100);

                if (m_HP <= 0)
                {
                    m_Dead = true;
                    StartCoroutine(KillAfterDeath());
                }
            }
        }
    }

    void HandleReactionAnimation(Vector3 relativePoint)
    {        
        string[] hitArray = new string[] { "HitLeft", "HitRight", "HitFront", "HitBack" };
        float xWeight, yWeight;
        xWeight = Mathf.Abs(relativePoint.x);
        yWeight = Mathf.Abs(relativePoint.y);
        int animIndex = 0;
        if (xWeight > yWeight)
        {
            if (relativePoint.x < 0){animIndex = 0;}
            else{animIndex = 1;}
        }
        else
        {
            if (relativePoint.z > 0){animIndex = 2;}
            else{animIndex = 3;}
        }

        m_Anim.SetTrigger(hitArray[animIndex]);
    }


    IEnumerator KillAfterDeath()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject ragdoll = Instantiate(m_RagDoll);
        ragdoll.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = m_DefaultColor;
        ragdoll.transform.position = this.transform.position;
        ragdoll.transform.rotation= this.transform.rotation;
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
