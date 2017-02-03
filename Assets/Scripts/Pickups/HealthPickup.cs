using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {
    public int m_Points;
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour pb = other.gameObject.GetComponent<PlayerBehaviour>();
        if(pb != null)
        {
            pb.m_HP += m_Points;
            AudioSource asrc = GetComponent<AudioSource>();
            asrc.Play();
            Destroy(this.gameObject.GetComponent<SphereCollider>());
            Destroy(this.gameObject.GetComponent<MeshRenderer>());
            Destroy(this);
        }
    }
}
