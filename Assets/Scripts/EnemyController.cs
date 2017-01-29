using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float m_HP;
    public Color m_DefaultColor;
    public Color m_HitColor;
    public SkinnedMeshRenderer m_MeshRenderer;
    public float m_HitFlashTime = 0.2f;
    public ParticleSystem m_PSys;
	// Use this for initialization
	void Start () {
        m_MeshRenderer.materials[0].color = m_DefaultColor;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Weapon")
        {
            m_PSys.Emit(100);
            StartCoroutine(Hit());
        }
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
