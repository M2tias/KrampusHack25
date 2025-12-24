using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    [SerializeField]
    WheelCollider frontLeft;
    [SerializeField]
    WheelCollider frontRight;
    [SerializeField]
    WheelCollider backLeft;
    [SerializeField]
    WheelCollider backRight;

    [SerializeField]
    Transform frontLeftModel;
    [SerializeField]
    Transform frontRightModel;

    [SerializeField]
    private List<float> accelerationLevel;
    [SerializeField]
    private List<float> WheelLevelMaxRPMs;
    [SerializeField]
    private Transform centerOfMass;
    [SerializeField]
    private float frontAntiRoll;
    [SerializeField]
    private float backAntiRoll;

    private readonly float breakingForce = 650f;
    private readonly float maxTurnAngle = 30f;


    private float maxWheelRPM;
    private float acceleration;
    private float currentAcceleration = 0f;
    private float currentBreakForce = 0f;
    private float currentTurnAngle = 0f;

    InputAction moveAction;
    InputAction breakAction;
    Vector2 moveVector = Vector2.zero;
    bool isBreaking = false;

    bool wasUpsideDown = false;
    float wentUpsideDown = 0f;
    float upsideDownResetTime = 1f;
    private CharacterLoot loot;
    private Rigidbody rb;
    private bool isAIGrounded = false;
    private float aIAcceleration = 0f;
    private float aITurnAngle = 0f;
    private bool isAI = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        breakAction = InputSystem.actions.FindAction("Break");
        maxWheelRPM = WheelLevelMaxRPMs[0];
        acceleration = accelerationLevel[0];
        loot = GetComponent<CharacterLoot>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
        isAI = gameObject.tag == "Enemy";

        if (isAI)
        {
            GetComponent<EnemyLogic>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    void Update()
    {
        if (isAI && !isAIGrounded)
        {
            bool isAIGrounded = new List<WheelCollider>() { frontLeft, frontRight, backLeft, backRight }.All(x => x.isGrounded);
            if (isAIGrounded)
            {
                GetComponent<EnemyLogic>().enabled = true;
                GetComponent<NavMeshAgent>().enabled = false;
            }
        }

        if (!isAI)
        {
            moveVector = moveAction.ReadValue<Vector2>();
            isBreaking = breakAction.IsPressed();
        }

        int tireLevel = loot.GetPickupLevel(LootType.Tires);

        if (tireLevel > 0)
        {
            acceleration = accelerationLevel[tireLevel];
            maxWheelRPM = WheelLevelMaxRPMs[tireLevel];
        }
    }

    void FixedUpdate()
    {
        if (isAI)
        {
            currentAcceleration = acceleration * aIAcceleration;
        }
        else
        {

            currentAcceleration = acceleration * moveVector.y;
        }

        if (isBreaking || (isAI && aIAcceleration == 0))
        {
            currentBreakForce = breakingForce;
        }
        else
        {
            currentBreakForce = 0f;
        }

        if (frontRight.rpm < maxWheelRPM)
        {
            frontRight.motorTorque = currentAcceleration;
            frontLeft.motorTorque = currentAcceleration;
        }
        else
        {
            frontRight.motorTorque = 0f;
            frontLeft.motorTorque = 0f;
        }

        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;

        if (isAI)
        {
            currentTurnAngle = aITurnAngle;
        }
        else
        {
            currentTurnAngle = maxTurnAngle * moveVector.x;
        }

        frontRight.steerAngle = currentTurnAngle;
        frontRightModel.localRotation = Quaternion.Euler(0, currentTurnAngle, 90);
        frontLeft.steerAngle = currentTurnAngle;
        frontLeftModel.localRotation = Quaternion.Euler(0, currentTurnAngle, 90);
        // if (isAI) Debug.Log($"Steering, angle {currentTurnAngle}");

        bool upsideDown = Physics.Linecast(transform.position - transform.up * 0.3f, transform.position + transform.up * 1.2f, LayerMask.GetMask("Terrain"));
        Debug.DrawLine(transform.position - transform.up * 0.3f, transform.position + transform.up * 1.2f, Color.red);
        bool leftSideDown = Physics.Linecast(transform.position, transform.position + transform.right * -1.45f, LayerMask.GetMask("Terrain"));
        Debug.DrawLine(transform.position, transform.position + transform.right * -1.45f, Color.red);
        bool rightSideDown = Physics.Linecast(transform.position, transform.position + transform.right * 1.45f, LayerMask.GetMask("Terrain"));
        Debug.DrawLine(transform.position, transform.position + transform.right * 1.45f, Color.red);

        if (upsideDown || leftSideDown || rightSideDown)
        {
            if (!wasUpsideDown)
            {
                wentUpsideDown = Time.time;
                wasUpsideDown = true;
            }

            if (Time.time - wentUpsideDown > upsideDownResetTime)
            {
                Vector3 currentEuler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(new Vector3(currentEuler.x, currentEuler.y, 0f));
                transform.position = transform.position + Vector3.up * 2f;
                Debug.DrawLine(transform.position - transform.up * 1f, transform.position + transform.up * 3f, Color.green);
            }
        }
        else
        {
            wasUpsideDown = false;
            wentUpsideDown = Time.time;
        }

        ApplyAntiroll(frontLeft, frontRight, frontAntiRoll);
        ApplyAntiroll(backLeft, backRight, backAntiRoll);
        UIManager.main.SetSpeed(rb.linearVelocity.magnitude);
    }

    public void Accelerate(float acceleration)
    {
        aIAcceleration = acceleration;
    }

    public void Steer(float steeringAngle)
    {
        aITurnAngle = Mathf.Clamp(steeringAngle, -maxTurnAngle, maxTurnAngle);
        // Debug.Log($"Steering, angle {steeringAngle}, clamped {aITurnAngle}");
    }

    public bool Brake()
    {
        Debug.Log($"Braking! Velocity: {rb.linearVelocity.magnitude}");
        aIAcceleration = 0;
        return rb.linearVelocity.magnitude < 0.333f;
    }

    public float GetSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    public void ReverseTurnAngle() {
        aITurnAngle = -currentTurnAngle;
    }

    private void ApplyAntiroll(WheelCollider leftWheel, WheelCollider rightWheel, float antiRollAmount)
    {
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = leftWheel.GetGroundHit(out WheelHit hitL);

        if (groundedL)
        {
            travelL = (-leftWheel.transform.InverseTransformPoint(hitL.point).y - leftWheel.radius) / leftWheel.suspensionDistance;
        }

        bool groundedR = rightWheel.GetGroundHit(out WheelHit hitR);

        if (groundedR)
        {
            travelR = (-rightWheel.transform.InverseTransformPoint(hitR.point).y - rightWheel.radius) / rightWheel.suspensionDistance;
        }

        float antiRollForce = (travelL - travelR) * antiRollAmount;

        if (groundedL)
        {
            rb.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);
        }

        if (groundedR)
        {
            rb.AddForceAtPosition(rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
        }
    }
}
