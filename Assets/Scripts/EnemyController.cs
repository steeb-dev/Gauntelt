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
    private ParticleSystem m_PSys;
    private Animator m_Anim;
    private Rigidbody m_RigidBody;
    public GameObject m_RagDoll;
    public GameObject m_TargetPlayer;
    private Vector3 m_PlayerDirection;
    private float m_TurnSpeed = 5.0f;
    public float m_RunSpeed = 1.0f;
    private int speedFloat;
    public float speedDampTime = 0.1f;

    // Use this for initialization
    void Start () {
        m_PSys = GetComponent<ParticleSystem>();
        m_Anim = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_TargetPlayer = GameObject.FindWithTag("Player");
        m_MeshRenderer.materials[0].color = m_DefaultColor;
        speedFloat = Animator.StringToHash("Speed");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_TargetPlayer != null)
        {
            m_PlayerDirection = m_TargetPlayer.transform.position - m_RigidBody.position;
            m_PlayerDirection.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_PlayerDirection), m_TurnSpeed * Time.deltaTime);           
        }
    }

    void FixedUpdate()
    {
        m_RigidBody.AddForce(m_PlayerDirection * m_RunSpeed);
        m_Anim.SetFloat(speedFloat, m_RunSpeed, speedDampTime, Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Weapon")
        {
            StartCoroutine(Hit());
        
            BatController bc = other.gameObject.GetComponent<BatController>();
            this.m_HP -= (int)bc.GetDamage();
            var relativePoint = transform.InverseTransformPoint(bc.m_Player.transform.position);


            string[] hitArray = new string[] { "HitLeft", "HitRight", "HitFront", "HitBack" };
            float xWeight, yWeight;
            xWeight = Mathf.Abs(relativePoint.x);
            yWeight = Mathf.Abs(relativePoint.y);
            int animIndex = 0;
            if (xWeight > yWeight)
            {
                if(relativePoint.x < 0)
                {
                    animIndex = 0;
                }
                else
                {
                    animIndex = 1;
                }
            }
            else
            {
                if (relativePoint.z > 0)
                {
                    animIndex = 2;
                }
                else
                {
                    animIndex = 3;
                }
            }

            m_Anim.SetTrigger(hitArray[animIndex]);

            m_PSys.Emit(100);

            if (m_HP <= 0)
            {
                //m_Anim.SetBool("Dead", true);
                StartCoroutine(KillAfterDeath());
            }
        }
    }


    IEnumerator KillAfterDeath()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject ragdoll = Instantiate(m_RagDoll);
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
