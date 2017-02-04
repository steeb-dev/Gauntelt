using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatType
{
    Melee,
    Projectile
}

public enum EnemyState
{
    Chill,
    Alert
}


public class EnemyController : Woundable
{
    public EnemyState m_State;
    public float m_AlertRange;
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
    public GameObject m_ProjectilePrefab;

    private int movingBool;
    Rigidbody[] rigidBodies;
    public AudioClip m_RantClip;
    public int m_ScoreVal;
    private GameController m_GameController;
    public CombatType m_CombatType;

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
        this.OnKillConfirm += KillConfirmed;
        this.OnHitConfirm += HitConfirmed;
    }

    void KillConfirmed(GameObject other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if(pb != null)
        {
            pb.m_Score += this.m_ScoreVal;
        }
    }

    void HitConfirmed(GameObject other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            if (m_State == EnemyState.Chill)
            {
                m_State = EnemyState.Alert;
                m_TargetPlayer = pb;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_State == EnemyState.Alert)
        {
            if (m_TargetPlayer == null || m_TargetPlayer.m_Dead)
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
        else if (m_State == EnemyState.Chill)
        {
            if (m_GameController.GetMinPlayerDistance(this.transform.position) < m_AlertRange)
            {
                m_State = EnemyState.Alert;
            }
            m_TargetPlayer = m_GameController.GetTargetPlayer(transform.position);

        }
    }

    void FixedUpdate()
    {
        if (m_State == EnemyState.Alert)
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
                        switch (m_CombatType)
                        {
                            case CombatType.Melee:
                                if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !m_Anim.IsInTransition(0))
                                {
                                    m_Anim.SetTrigger("Attack");
                                    m_Bat.Attack();
                                }
                                break;
                            case CombatType.Projectile:
                                m_Bat.DeactivateCollider();
                                if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Projectile") && !(m_Anim.IsInTransition(0) && m_Anim.GetNextAnimatorStateInfo(0).IsName("Projectile")))
                                {
                                    m_Attacking = true;
                                    m_Anim.SetTrigger("Projectile");
                                    StartCoroutine(FireProjectile());
                                }
                                break;
                        }
                    }
                }
            }
            m_Anim.SetBool(movingBool, moving);
        }
    }

    IEnumerator FireProjectile()
    {
        WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();
        bool loop = true;
        while (loop)
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Projectile") || (m_Anim.GetNextAnimatorStateInfo(0).IsName("Projectile") && m_Anim.IsInTransition(0)))
            {
                loop = false;
            }
            yield return waitForFrame;
        }
        Vector3 trajectory = transform.forward;
        GameObject pObj = Instantiate(m_ProjectilePrefab);
        pObj.transform.position = this.transform.position;
        Projectile p = pObj.GetComponent<Projectile>();
        p.m_Player = this.gameObject;
        p.Init(trajectory);
        m_Attacking = false;
    }

}
