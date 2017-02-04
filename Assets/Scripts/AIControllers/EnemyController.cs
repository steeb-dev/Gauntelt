using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatType
{
    Melee,
    Projectile
}
public class EnemyController : Woundable
{
    public CombatType m_Type;

    public ParticleSystem m_PSys;
    private Animator m_Anim;
    private Rigidbody m_RigidBody;
    public PlayerBehaviour m_TargetPlayer;
    private Vector3 m_PlayerDirection;
    private float m_TurnSpeed = 5.0f;
    public float m_RunSpeed = 1.0f;
    private int speedFloat;
    public float speedDampTime = 0.1f;
    public float m_AttackRange = 1f;
    public bool m_Attacking;
    public BatController m_Bat;
    private int movingBool;
    Rigidbody[] rigidBodies;
    public AudioClip m_RantClip;
    public int m_ScoreVal;
    private GameController m_GameController;

    // Use this for initialization
    void Start ()
    {
        m_GameController = FindObjectOfType<GameController>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        movingBool = Animator.StringToHash("Moving");
        m_Anim = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_TargetPlayer = m_GameController.GetTargetPlayer(transform.position);
        m_MeshRenderer.materials[0].color = m_DefaultColor;
        speedFloat = Animator.StringToHash("Speed");
        this.OnKillConfirm += Killed;
    }

    void Killed(GameObject other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if(pb != null)
        {
            pb.m_Score += this.m_ScoreVal;
        }
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

    
}
