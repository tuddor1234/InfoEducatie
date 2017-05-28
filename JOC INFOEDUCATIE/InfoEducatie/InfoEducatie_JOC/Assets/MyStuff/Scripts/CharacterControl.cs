using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    [System.Serializable]
    public class MoveSettings
    {
        public float forwardVel = 12f;
        public float rotateVel = 100f;
        public float jumpVel = 25f;
        public float distToGround = 0.1f;
        public LayerMask ground;
        public bool broken;

    }

    [System.Serializable]
    public class PhysSettings
    {
        public float downAccel = 0.75f;

    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;
        public string FORWARD_AXIS = "Vertical";
        public string TURN_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";

    }

    public MoveSettings moveSettings = new MoveSettings();
    public PhysSettings physSettings = new PhysSettings();
    public InputSettings inputSettings = new InputSettings();


    Vector3 velocity = Vector3.zero;
    Quaternion targetRotation;
    Rigidbody rBody;
    float forwardInput, turnInput, jumpInput;


    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down,moveSettings.distToGround, moveSettings.ground);
    }


    void Start()
    {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
            rBody = GetComponent<Rigidbody>();
        else
            Debug.LogError("No RigidBody");

        moveSettings.broken = false;// NU UITA SA SCHIMBI!!!!!!!!!!!!

        forwardInput = turnInput = jumpInput = 0;
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS);
        turnInput = Input.GetAxis(inputSettings.TURN_AXIS);
        jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS);

    }

    void Update()
    {
        GetInput();
        Turn();
        
    }

    void FixedUpdate()
    {
         Run();
         Jump();

        rBody.velocity = transform.TransformDirection(velocity);
    }

    void Run()
    {

        if (Mathf.Abs(forwardInput) > inputSettings.inputDelay)
        {
            if (moveSettings.broken == true)
            {
                velocity.z = moveSettings.forwardVel * forwardInput / 5f;
                //add broken animation
            }
            else
            {
                velocity.z = moveSettings.forwardVel * forwardInput;
                //remove broken animation
            }
        }
        else
        {
            velocity.z = 0;  
        }
       
    }

    void Turn()
    {
        if (Mathf.Abs(turnInput) > inputSettings.inputDelay)
        {
            targetRotation *= Quaternion.AngleAxis(moveSettings.rotateVel * turnInput * Time.deltaTime, Vector3.up); 
        }
        transform.rotation = targetRotation;
    }

    void Jump()
    {
        if(jumpInput > 0 && Grounded())
        {
            velocity.y = moveSettings.jumpVel;
         //   Debug.Log("JUMP");
        }
        else if(jumpInput == 0 && Grounded())
        {
            velocity.y = 0;
             //Debug.Log("On ground");
        }
        else
        {
            velocity.y -= physSettings.downAccel;
           // Debug.Log("Fall");
        }
                 
    }


}
