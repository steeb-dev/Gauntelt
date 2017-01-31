using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAfterDelay : MonoBehaviour {
    public float m_DelayTime;

	IEnumerator KillDelay()
    {
        yield return new WaitForSeconds(m_DelayTime);
        Destroy(this.gameObject);
   }

    private void OnEnable()
    {
        StartCoroutine(KillDelay());
    }
}
