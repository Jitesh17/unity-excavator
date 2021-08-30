using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ExcavatorAgent : Agent
{
    public float maxSpeed = 3F;
    public float creepSpeed = 1F;
    public float initialAccel = 15F;
    public float deltaAccel = 0.5F;
    public float maxAccel = 45F;
    public float deltaDirection = 2F;

    public bool enableRearCameras = false;

    public bool useHook = false;

    private DriveParams driveParams;
    // private ExcavatorController exController;
    private Excavator excavator;

    bool travel = false;
    Image travelIndicator;
    Image operationIndicator;

    
    private LeverAngles rightOperationLeverAngles = new LeverAngles(0F, 0F);
    private LeverAngles leftOperationLeverAngles = new LeverAngles(0F, 0F);
    private LeverAngles rightTravelLeverAngles = new LeverAngles(0F, 0F);
    private LeverAngles leftTravelLeverAngles = new LeverAngles(0F, 0F);
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Initialize()
    {
        excavator = new Excavator(transform.root.gameObject);

        travelIndicator = GameObject.FindWithTag("TravelIndicator").GetComponent<Image>();
        operationIndicator = GameObject.FindWithTag("OperationIndicator").GetComponent<Image>();
        travelIndicator.color = new Color(0, 0, 0);
        operationIndicator.color = new Color(0, 1F, 0);
        float mass = gameObject.GetComponent<Rigidbody>().mass;
        driveParams = new DriveParams(mass, maxSpeed, creepSpeed, initialAccel, deltaAccel, maxAccel, deltaDirection);

        excavator.EnableRearCameras(enableRearCameras);

        excavator.useHook = useHook;
    }


    public override void OnEpisodeBegin()
    {
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(excavator.rb.velocity.normalized);   // (3 observations)
    }
    public override void OnActionReceived(ActionBuffers actions)

    { 
        // if (frozen) return;
        excavator.OrientHook();

        var delta = Time.deltaTime * 30F;

        // Control
        if (true)
        {

            /* Swing */
            if (actions.ContinuousActions[0] != 0F)
            {
                Debug.Log(excavator.swingAngle);
                excavator.swingRotate(delta * 1F * actions.ContinuousActions[0]);
                leftOperationLeverAngles.leftRight = 5F * actions.ContinuousActions[0];
            }

            /* Arm */
            if (actions.ContinuousActions[1] != 0F)
            {
                excavator.armRotate(-delta * 1F * actions.ContinuousActions[1]);
                leftOperationLeverAngles.upDown = 5F * actions.ContinuousActions[1];
            }

            /* Boom */
            if (actions.ContinuousActions[2] != 0F)
            {
                excavator.boomRotate(-delta * 0.6F * actions.ContinuousActions[2]);
                rightOperationLeverAngles.upDown = 5F * actions.ContinuousActions[2];
            }

            /* Bucket */
            if (actions.ContinuousActions[3] != 0F)
            {
                excavator.bucketRotate(delta * 1.5F * actions.ContinuousActions[3]);
                rightOperationLeverAngles.leftRight = 5F * actions.ContinuousActions[3];
            }

            // /* Tracks */
            // if (actions.ContinuousActions[4] != 0F || actions.ContinuousActions[5] != 0F)
            // {
            //     float rotationRight = -delta * 0.6F * actions.ContinuousActions[4];
            //     float rotationLeft = delta * 0.6F * actions.ContinuousActions[5];
            //     float deltaRotation = rotationRight + rotationLeft;
            //     float input = actions.ContinuousActions[4] + actions.ContinuousActions[5];
            //     if (input > 1F) input = 1F;
            //     else if (input < -1F) input = -1F;
            //     float accel = 0F;
            //     if (input != 0F) accel = driveParams.initialAccel * Mathf.Sign(input) + (driveParams.maxAccel - driveParams.initialAccel) * input;
            //     excavator.Move(deltaRotation, accel, driveParams);
            //     rightTravelLeverAngles.upDown = 5F * actions.ContinuousActions[4];
            //     leftTravelLeverAngles.upDown = 5F * actions.ContinuousActions[5];
            // }

            // inputEvents.clear();
        }

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        // Swing
        if (Input.GetKey(KeyCode.RightArrow)) { continuousActionsOut.Array[0] = 1F; }  // Swing right
        else if (Input.GetKey(KeyCode.LeftArrow)) { continuousActionsOut.Array[0] = -1F; }  // Swing left

        // Arm
        if (Input.GetKey(KeyCode.Q)) { continuousActionsOut.Array[1] = 1F; }  // Arm roll out
        else if (Input.GetKey(KeyCode.A)) { continuousActionsOut.Array[1] = -1F; }  // Arm rool in

        // Boom
        if (Input.GetKey(KeyCode.DownArrow)) { continuousActionsOut.Array[2] = 1F; }  // Boom roll in
        else if (Input.GetKey(KeyCode.UpArrow)) { continuousActionsOut.Array[2] = -1F; }  // Boom roll out

        // Bucket
        if (Input.GetKey(KeyCode.W)) { continuousActionsOut.Array[3] = 1F; }  // Bucket roll out
        else if (Input.GetKey(KeyCode.S)) { continuousActionsOut.Array[3] = -1F; }  // Bucket roll in

        // Track
        // if (Input.GetKey(KeyCode.U)) { continuousActionsOut.Array[4] = 1F; }  // Track right
        // else if (Input.GetKey(KeyCode.O)) { continuousActionsOut.Array[4] = -1F; }
        // if (Input.GetKey(KeyCode.Y)) { continuousActionsOut.Array[5] = 1F; }  // Track left
        // else if (Input.GetKey(KeyCode.R)) { continuousActionsOut.Array[5] = -1F; }

    }
}