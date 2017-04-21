using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    [System.Serializable]
    public class PositionSettings
    {
        public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);
        public float lookSmooth = 100f;
        public float distFromTarget = -8;
        public float zoomSmooth = 10;
        public float maxZoom = -2;
        public float minZoom = -15;
    }

    [System.Serializable]
    public class OrbitSettings
    {
        public float xRotation = -20f;
        public float yRotation = -180;
        public float maxXRotation = 25f;
        public float minXRotation = -85f;
        public float vOrbitSmooth = 150f;
        public float hOrbitSmooth = 150f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
        public string ORBIT_HORIZONTAL = "OrbitHorizontal";
        public string ORBIT_VERTICAL = "OrbitVertical";
        public string ZOOM = "Mouse ScrollWheel";
    }

    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();


    Vector3 targetPos = Vector3.zero;
    Vector3 destination = Vector3.zero;
    CharacterControl charController;
    float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput; 


    void Start()
    {
        SetCameraTarget(target);
        targetPos = target.position + position.targetPosOffset;
        destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation, 0) * -Vector3.forward * position.distFromTarget;
        destination += targetPos;
        transform.position = destination;
    }

    public void SetCameraTarget(Transform t)
    {
        target = t;
        if (target != null)
        {
            if(target.GetComponent<CharacterControl>())
            {
                charController = target.GetComponent<CharacterControl>();
            }
            else
            {
                Debug.LogError("Camera needs a char controller");
            }
        }
        else Debug.LogError("Camera needs a target!");
    }

    void GetInput()
    {

        vOrbitInput = Input.GetAxisRaw(input.ORBIT_VERTICAL);
        hOrbitInput = Input.GetAxisRaw(input.ORBIT_HORIZONTAL);
        hOrbitSnapInput = Input.GetAxisRaw(input.ORBIT_HORIZONTAL_SNAP);
        zoomInput = Input.GetAxisRaw(input.ZOOM);
    }

    void Update()
    {
        GetInput();
        OrbitTarget();
        ZoomInOutTarget();
    }


    void LateUpdate()
    {
        MoveToTarget();
        LookAtTarget();
    }

    void MoveToTarget()
    {
        targetPos = target.position + position.targetPosOffset;
        destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * position.distFromTarget;
        destination += targetPos;
        transform.position = destination;
    }

    void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, position.lookSmooth * Time.deltaTime);
    } 

    void OrbitTarget()
    {
        if(hOrbitSnapInput > 0)
        {
            orbit.yRotation = -180f;
        }
        orbit.xRotation += -vOrbitInput * orbit.vOrbitSmooth * Time.deltaTime;
        orbit.yRotation += -hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime;

        if(orbit.xRotation > orbit.maxXRotation)
        {
            orbit.xRotation = orbit.maxXRotation;
        }

        if(orbit.xRotation < orbit.minXRotation)
        {
            orbit.xRotation = orbit.minXRotation;
        }
    }

    void ZoomInOutTarget()
    {
        position.distFromTarget += zoomInput * position.zoomSmooth * Time.deltaTime ;

        if(position.distFromTarget > position.maxZoom)
        {
            position.distFromTarget = position.maxZoom;
        }

        if(position.distFromTarget < position.minZoom)
        {
            position.distFromTarget = position.minZoom;
        }
    }

}
