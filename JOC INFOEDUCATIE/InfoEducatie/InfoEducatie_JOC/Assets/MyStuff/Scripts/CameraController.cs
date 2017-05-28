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
        public bool smoothFollow = true;
        public float smooth = 0.05f;

        [HideInInspector]
        public float newDistance = -8f;
        [HideInInspector]
        public float ajustmentDistance = -8f;


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
        public string ZOOM = "ZOOM";
    }

    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAjustedCollisionLines = true;

    }



    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();
    public DebugSettings debug = new DebugSettings();
    public CollisionHandler collision = new CollisionHandler();


    Vector3 targetPos = Vector3.zero;
    Vector3 destination = Vector3.zero;
    Vector3 ajustedDestination = Vector3.zero;
    Vector3 camVel = Vector3.zero;

    CharacterControl charController;
    float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput, mouseOrbitInput, vMouseOrbitInput;

    Vector3 previosMousePos = Vector3.zero;
    Vector3 currentMousePos = Vector3.zero;


    ChangePOV changePOV;

    void Start()
    {
        SetCameraTarget(target);

        vOrbitInput = hOrbitInput = zoomInput = hOrbitSnapInput = mouseOrbitInput = vMouseOrbitInput = 0;
       
        MoveToTarget();

        collision.Initialize(Camera.main);
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.ajustedCameraClipPoints);
        collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);

        transform.position = destination;

        previosMousePos = currentMousePos = Input.mousePosition;

        ChangePOV changePOV = new ChangePOV();

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
        ZoomInOutTarget();
    }


    void FixedUpdate()
    {
        MoveToTarget();
        LookAtTarget();
        OrbitTarget();
        //MouseOrbitTarget();


        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.ajustedCameraClipPoints);
        collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);

        for (int i = 0; i < 5; i++)
        {
            if (debug.drawDesiredCollisionLines == true)
            {
                Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.red);
            }
            if (debug.drawAjustedCollisionLines == true)
            {
                Debug.DrawLine(targetPos, collision.ajustedCameraClipPoints[i], Color.green);
            }
        }

        collision.CheckColliding(targetPos);
        position.ajustmentDistance = collision.GetAdjustedDistanceWithRayFrom(targetPos);

    }

    void MoveToTarget()
    {
        targetPos = target.position + position.targetPosOffset;
        destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * position.distFromTarget;
        destination += targetPos;
        transform.position = destination;

        if(collision.colliding)
        {
            ajustedDestination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * Vector3.forward * position.distFromTarget;
            ajustedDestination += targetPos;

            if (position.smoothFollow)
            {
                transform.position = Vector3.SmoothDamp(transform.position, ajustedDestination, ref camVel, position.smooth);

            }
            else transform.position = ajustedDestination;
        }
        else
        {
            if (position.smoothFollow)
            {
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, position.smooth);
            }
            else transform.position = ajustedDestination;
        }

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

    [System.Serializable]
    public class CollisionHandler
    {
        public LayerMask collisionLayer;

        [HideInInspector]
        public bool colliding = false;
        [HideInInspector]
        public Vector3[] ajustedCameraClipPoints;
        [HideInInspector]
        public Vector3[] desiredCameraClipPoints;


        Camera camera;

        public void Initialize(Camera cam)
        {
            camera = cam;
            ajustedCameraClipPoints = new Vector3[5];
            desiredCameraClipPoints = new Vector3[5];

        }

        public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotatation, ref Vector3[] intoArray)
        {
            if (!camera)
                return;

            intoArray = new Vector3[5];
            float z = camera.nearClipPlane;
            float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
            float y = x / camera.aspect;

            //stanga sus
            intoArray[0] = (atRotatation * new Vector3(-x, y, z)) + cameraPosition;

            //dreapta sus
            intoArray[1] = (atRotatation * new Vector3(x, y, z)) + cameraPosition;

            //stanga jos
            intoArray[2] = (atRotatation * new Vector3(-x, -y, z)) + cameraPosition;

            //dreapta jos
            intoArray[3] = (atRotatation * new Vector3(x, -y, z)) + cameraPosition;

            //camera position
            intoArray[4] = cameraPosition - camera.transform.forward;
        }


        bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 FromPosition)
        {
            for(int i= 0;i<clipPoints.Length;i++)
            {
                Ray ray = new Ray(FromPosition,clipPoints[i] - FromPosition);
                float distance = Vector3.Distance(clipPoints[i],FromPosition);
                if (Physics.Raycast(ray, distance, collisionLayer)) return true;
                 
            }

            return false;
        }

     

        public float GetAdjustedDistanceWithRayFrom(Vector3 from)
        {
            float distance = -1;

            for(int i=0;i<desiredCameraClipPoints.Length;i++)
            {
                Ray ray = new Ray(from,desiredCameraClipPoints[i]-from);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    if (distance == -1)
                        distance = hit.distance;
                    else
                    {
                        if (hit.distance < distance)
                            distance = hit.distance;
                    }
                }

            }

            if (distance == -1)
                return 0;
            else
                return distance;
        }

        public void CheckColliding(Vector3 targetPosition)
        {
            if(CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
            {
                colliding = true;
          
            }
            else
            {
                colliding = false;
            }
        }

    }



}
