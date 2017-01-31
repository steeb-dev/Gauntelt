using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollExplode : MonoBehaviour {

    Rigidbody[] rigidBodies;

	// Use this for initialization
	void Start () {
        rigidBodies = GetComponentsInChildren<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Explode"))
        {
            Vector3 explosionPos = new Vector3(this.transform.position.x, this.transform.position.y - 2, this.transform.position.z);
            foreach (Rigidbody rb in rigidBodies)
            {
                rb.AddExplosionForce(10000, explosionPos, 100);
            }
        }
    }
}
