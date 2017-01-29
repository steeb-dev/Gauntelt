using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float m_HP;
    public Color m_DefaultColor;
    public Color m_HitColor;
    public float m_HitFlashTime = 0.2f;

    public SkinnedMeshRenderer m_MeshRenderer;
    private ParticleSystem m_PSys;
    private Animator m_Anim;
    private Rigidbody m_RigidBody;
    public GameObject m_RagDoll;

	// Use this for initialization
	void Start () {
        m_PSys = GetComponent<ParticleSystem>();
        m_Anim = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();


        m_MeshRenderer.materials[0].color = m_DefaultColor;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Weapon")
        {
            StartCoroutine(Hit());

            BatController bc = other.gameObject.GetComponent<BatController>();
            this.m_HP -= (int)bc.GetDamage();

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
