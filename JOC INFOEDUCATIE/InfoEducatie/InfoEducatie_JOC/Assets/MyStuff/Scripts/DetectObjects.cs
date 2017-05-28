using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectObjects : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {
        RaycastHit hit = new RaycastHit();

        Debug.DrawRay(transform.position, Vector3.forward * 10);

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 10.0f))
        {
            Debug.Log(hit.transform.name);
        }
    }
}
