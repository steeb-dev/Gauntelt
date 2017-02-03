using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float m_HP;
    public Color m_DefaultColor;
    public float m_HitFlashTime = 0.2f;

    public SkinnedMeshRenderer m_MeshRenderer;
    public ParticleSystem m_PSys;
    private Animator m_Anim;
    private Rigidbody m_RigidBody;
    public GameObject m_RagDoll;
    public PlayerBehaviour m_TargetPlayer;
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
    public AudioClip m_RantClip;
    public AudioClip m_HitClip;
    AudioSource m_Source;
    public int m_ScoreVal;
    private GameController m_GameController;

    // Use this for initialization
    void Start ()
    {
        m_Source = GetComponent<AudioSource>();
        m_GameController = FindObjectOfType<GameController>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        movingBool = Animator.StringToHash("Moving");
        m_Anim = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_TargetPlayer = m_GameController.GetTargetPlayer(transform.position);
        m_MeshRenderer.materials[0].color = m_DefaultColor;
        speedFloat = Animator.StringToHash("Speed");
    }

    // Update is called once per frame
    void Update()
    {
        if(m_TargetPlayer == null || m_TargetPlayer.m_Dead)
        {
            m_TargetPlayer = m_GameController.GetTargetPlayer(transform.position);
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
                    if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !m_Anim.IsInTransition(0))
                    {
                        m_Anim.SetTrigger("Attack");
                        m_Bat.Attack();
                    }
                }
            }
        }
        m_Anim.SetBool(movingBool, moving);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_Dead)
        {
            PlayerBehaviour pb = null;
            Vector3 relativePoint = new Vector3(0, 0, 0);
            Vector3 inverseDir = new Vector3(0, 0, 0);
            int damage = 0;
            if (other.gameObject.tag == "Weapon")
            {
                BatController bc = other.gameObject.GetComponent<BatController>();
                damage = (int)bc.GetDamage();
                bc.PlaySound();
                relativePoint = transform.InverseTransformPoint(bc.m_Player.transform.position);     
                inverseDir = new Vector3(-relativePoint.x, relativePoint.y, -relativePoint.z);
                pb = bc.m_Player.GetComponent<PlayerBehaviour>(); 
            }
            else if (other.gameObject.tag == "Projectile")
            {
                Projectile p = other.gameObject.GetComponent<Projectile>();
                if (p != null)
                {
                    damage = (int)p.GetDamage();
                    p.PlaySound();
                    relativePoint = transform.InverseTransformPoint(other.gameObject.transform.position);
                    pb = p.m_Player.GetComponent<PlayerBehaviour>();
                }
            }
            if (damage > 0)
            {
                this.m_HP -= damage;
                StartCoroutine(Hit());
                m_Source.clip = m_HitClip;
                m_Source.Play();
                inverseDir = relativePoint;
                m_PSys.transform.rotation = Quaternion.Slerp(m_PSys.transform.rotation, Quaternion.LookRotation(relativePoint), 1f);
                m_PSys.Emit(100);
             
                if (m_HP <= 0)
                {
                    if (pb != null)
                    {
                        pb.m_Score += m_ScoreVal;
                    }
                    m_Dead = true;
                    StartCoroutine(KillAfterDeath());
                }
            }
        }
    }

 
    IEnumerator KillAfterDeath()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject ragdoll = Instantiate(m_RagDoll);
        ragdoll.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = m_DefaultColor;
        ragdoll.transform.position = this.transform.position;
        ragdoll.transform.rotation= this.transform.rotation;
        ragdoll.transform.localScale = this.transform.localScale;
        Destroy(this.gameObject);
    }

    IEnumerator Hit()
    {
        float t = 0;
        while (t < m_HitFlashTime)
        {
            m_MeshRenderer.materials[0].color = Random.ColorHSV();
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m_MeshRenderer.materials[0].color = m_DefaultColor;
    }
}
