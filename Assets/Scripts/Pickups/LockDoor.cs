using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDoor : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        PlayerBehaviour pb = collision.gameObject.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            if (pb.m_Keys >= 1)
            {
                pb.m_Keys -= 1;
                Destroy(this.gameObject);
            }
        }
    }
}
