using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePOV : MonoBehaviour {
    
     [SerializeField]
     Camera FPCamera;
     [SerializeField]
     Camera TPCamera;

    CharacterControl charController;

    public bool usingFPCamera;
    public bool usingTPCamera;


    private bool switchCam = false;
   


   

	void Start ()
    {
        FPCamera.GetComponent<Camera>().enabled = false;
        TPCamera.GetComponent<Camera>().enabled = true;
        usingFPCamera = false;
        usingTPCamera = true;
	}
	
	

	void Update ()
    {
		if(Input.GetKeyDown("v"))
        {
            switchCam = !switchCam;
        }

        if(switchCam == true)
        {
            FPCamera.GetComponent<Camera>().enabled = true;
            TPCamera.GetComponent<Camera>().enabled = false;
            usingFPCamera = true;
            usingTPCamera = false;

        }
        else
        {
            FPCamera.GetComponent<Camera>().enabled = false;
            TPCamera.GetComponent<Camera>().enabled = true;
            usingFPCamera = false;
            usingTPCamera = true;
        }

	}
}
