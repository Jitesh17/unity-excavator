using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

// public class CountStones
// {
//     public int inBucket = 0;
//     public int inTruck = 0;
// }
public class ExcavatorAgent : Agent
{

    public float maxSpeed = 3F;
    public float RotationSpeed = 30F;
    public float creepSpeed = 1F;
    public float initialAccel = 15F;
    public float deltaAccel = 0.5F;
    public float maxAccel = 45F;
    public float deltaDirection = 2F;

    public bool enableRearCameras = false;

    public bool useHook = false;

    private DriveParams driveParams;
    // private ExcavatorController exController;
    private CustomExcavator excavator;

    bool travel = false;
    Image travelIndicator;
    Image operationIndicator;

    
    private LeverAngles rightOperationLeverAngles = new LeverAngles(0F, 0F);
    private LeverAngles leftOperationLeverAngles = new LeverAngles(0F, 0F);
    private LeverAngles rightTravelLeverAngles = new LeverAngles(0F, 0F);
    private LeverAngles leftTravelLeverAngles = new LeverAngles(0F, 0F);
    
    public int countInBucket = 0;
    public int countInTruck = 0;
    public int countInContainer = 0;
    public int countChangeInBucket = 0;
    public int countChangeInTruck = 0;
    
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
        excavator = new CustomExcavator(transform.root.gameObject);

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
        StartCoroutine(excavator.Reset());
        
        this.countInBucket = 0;
        this.countInTruck = 0;
        this.countInContainer = 0;
        this.countChangeInBucket = 0;
        this.countChangeInTruck = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(excavator.rb.velocity.normalized);   // (3 observations)
        sensor.AddObservation(excavator.swingAngle/360 + 0.5f);  // (1 observations)
        sensor.AddObservation(excavator.boomAngle/360 + 0.5f);  // (1 observations)
        sensor.AddObservation(excavator.armAngle/360 + 0.5f);  // (1 observations)
        sensor.AddObservation(excavator.bucketAngle/360 + 0.5f);  // (1 observations)
        // Debug.Log("excavator.swingAngle");
        // Debug.Log(excavator.swingAngle/360 + 0.5f);
    }
    public override void OnActionReceived(ActionBuffers actions)

    { 

        // if(this.countInBucket>0)
        //     Debug.Log(this.countInBucket);
        // if (frozen) return;
        excavator.OrientHook();

        var delta = Time.deltaTime * RotationSpeed;

        // Control
        if (true)
        {

            /* Swing */
            if (actions.DiscreteActions[0] != 0)
            {
                // Debug.Log(excavator.swingAngle);
                excavator.swingRotate(delta * 1F * (3 - actions.DiscreteActions[0]*2));
                leftOperationLeverAngles.leftRight = 5F * (3 - actions.DiscreteActions[0]*2);
            }

            /* Arm */
            if (actions.DiscreteActions[1] != 0F)
            {
                excavator.armRotate(-delta * 1F * (3 - actions.DiscreteActions[1]*2));
                leftOperationLeverAngles.upDown = 5F * (3 - actions.DiscreteActions[1]*2);
            }

            /* Boom */
            if (actions.DiscreteActions[2] != 0F)
            {
                excavator.boomRotate(-delta * 0.6F * (3 - actions.DiscreteActions[2]*2));
                rightOperationLeverAngles.upDown = 5F * (3 - actions.DiscreteActions[2]*2);
            }

            /* Bucket */
            if (actions.DiscreteActions[3] != 0F)
            {
                excavator.bucketRotate(delta * 1.5F * (3 - actions.DiscreteActions[3]*2));
                rightOperationLeverAngles.leftRight = 5F * (3 - actions.DiscreteActions[3]*2);
            }
            // 
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
            // this.AddReward(0.01f);
        }

        this.AddReward(this.countChangeInBucket*0.001f);
        this.AddReward(this.countChangeInTruck*0.01f);
        this.countChangeInBucket = 0;
        this.countChangeInTruck = 0;
        Debug.Log($"this.GetCumulativeReward(): {this.GetCumulativeReward()}");

        if(this.countInContainer<10)
        {
            this.EndEpisode();
        }
        if(Mathf.Abs(this.transform.rotation.x)>20 || Mathf.Abs(this.transform.rotation.z)>20 )
        {
            this.EndEpisode();
        }


    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteActionsOut = actionsOut.DiscreteActions;
        DiscreteActionsOut.Array[0] = 0;
        DiscreteActionsOut.Array[1] = 0;
        DiscreteActionsOut.Array[2] = 0;
        DiscreteActionsOut.Array[3] = 0;
        // Swing
        if (Input.GetKey(KeyCode.RightArrow)) { DiscreteActionsOut.Array[0] = 1; }  // Swing right
        else if (Input.GetKey(KeyCode.LeftArrow)) { DiscreteActionsOut.Array[0] = 2; }  // Swing left

        // Arm
        if (Input.GetKey(KeyCode.Q)) { DiscreteActionsOut.Array[1] = 1; }  // Arm roll out
        else if (Input.GetKey(KeyCode.A)) { DiscreteActionsOut.Array[1] = 2; }  // Arm rool in

        // Boom
        if (Input.GetKey(KeyCode.DownArrow)) { DiscreteActionsOut.Array[2] = 1; }  // Boom roll in
        else if (Input.GetKey(KeyCode.UpArrow)) { DiscreteActionsOut.Array[2] = 2; }  // Boom roll out

        // Bucket
        if (Input.GetKey(KeyCode.W)) { DiscreteActionsOut.Array[3] = 1; }  // Bucket roll out
        else if (Input.GetKey(KeyCode.S)) { DiscreteActionsOut.Array[3] = 2; }  // Bucket roll in


    // public override void Heuristic(in ActionBuffers actionsOut)
    // {
    //     var continuousActionsOut = actionsOut.ContinuousActions;
    //     // Swing
    //     if (Input.GetKey(KeyCode.RightArrow)) { continuousActionsOut.Array[0] = 1F; }  // Swing right
    //     else if (Input.GetKey(KeyCode.LeftArrow)) { continuousActionsOut.Array[0] = -1F; }  // Swing left

    //     // Arm
    //     if (Input.GetKey(KeyCode.Q)) { continuousActionsOut.Array[1] = 1F; }  // Arm roll out
    //     else if (Input.GetKey(KeyCode.A)) { continuousActionsOut.Array[1] = -1F; }  // Arm rool in

    //     // Boom
    //     if (Input.GetKey(KeyCode.DownArrow)) { continuousActionsOut.Array[2] = 1F; }  // Boom roll in
    //     else if (Input.GetKey(KeyCode.UpArrow)) { continuousActionsOut.Array[2] = -1F; }  // Boom roll out

    //     // Bucket
    //     if (Input.GetKey(KeyCode.W)) { continuousActionsOut.Array[3] = 1F; }  // Bucket roll out
    //     else if (Input.GetKey(KeyCode.S)) { continuousActionsOut.Array[3] = -1F; }  // Bucket roll in

        // Track
        // if (Input.GetKey(KeyCode.U)) { continuousActionsOut.Array[4] = 1F; }  // Track right
        // else if (Input.GetKey(KeyCode.O)) { continuousActionsOut.Array[4] = -1F; }
        // if (Input.GetKey(KeyCode.Y)) { continuousActionsOut.Array[5] = 1F; }  // Track left
        // else if (Input.GetKey(KeyCode.R)) { continuousActionsOut.Array[5] = -1F; }


        // // --- Code for autonomous operations from this line ---
        // if (Input.GetKey(KeyCode.Alpha1))
        // {
        //     excavator.EnableCuttingEdges(false);
        //     // StartCoroutine(excavator.Reset());
        //     GameObject target = GameObject.FindWithTag("Target1");
        //     StartCoroutine(excavator.MoveToTarget(target, driveParams, 40F, 40F));
        // }

        // if (Input.GetKey(KeyCode.Alpha2))
        // {
        //     excavator.EnableCuttingEdges(false);
        //     // StartCoroutine(excavator.Reset());
        //     GameObject target = GameObject.FindWithTag("Target2");
        //     StartCoroutine(excavator.MoveToTarget(target, driveParams, 40F, 40F));
        // }

    }
}