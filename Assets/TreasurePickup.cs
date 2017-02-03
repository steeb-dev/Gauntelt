using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePickup : MonoBehaviour {
    public int m_Score;

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour pb = other.gameObject.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            pb.m_Score += m_Score;
            Destroy(this.gameObject);
        }
    }
}
