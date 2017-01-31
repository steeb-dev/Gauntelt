using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRotat : MonoBehaviour {

    UnityEngine.UI.RawImage m_Image;
    float speed;
	// Use this for initialization
	void Start () {
        m_Image = GetComponent<UnityEngine.UI.RawImage>();
        speed = Random.Range(0.0125f, 0.03f);
    }
	
	// Update is called once per frame
	void Update () {
        m_Image.rectTransform.RotateAroundLocal(new Vector3(0, 0, 1), speed);

    }
}
