using System.Collections.Generic;
using UnityEngine;
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
    private List<float> accelerationLevel;
    [SerializeField]
    private List<float> WheelLevelMaxRPMs;
    [SerializeField]
    private Transform centerOfMass;
    [SerializeField]
    private float frontAntiRoll;
    [SerializeField]
    private float backAntiRoll;

    private readonly float breakingForce = 300f;
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
    }

    void Update()
    {
        moveVector = moveAction.ReadValue<Vector2>();
        isBreaking = breakAction.IsPressed();

        int tireLevel = loot.GetPickupLevel(LootType.Tires);

        if (tireLevel > 0)
        {
            acceleration = accelerationLevel[tireLevel];
            maxWheelRPM = WheelLevelMaxRPMs[tireLevel];
        }
    }

    void FixedUpdate()
    {
        currentAcceleration = acceleration * moveVector.y;

        if (isBreaking)
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

        currentTurnAngle = maxTurnAngle * (-1) * moveVector.x;
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;

        bool upsideDown = Physics.Linecast(transform.position - transform.up * 0.15f, transform.position + transform.up * 0.3f, LayerMask.GetMask("Terrain"));
        Debug.DrawLine(transform.position - transform.up * 0.15f, transform.position + transform.up * 0.3f, Color.red);
        bool leftSideDown = Physics.Linecast(transform.position, transform.position + transform.right * -0.8f, LayerMask.GetMask("Terrain"));
        Debug.DrawLine(transform.position, transform.position + transform.right * -0.8f, Color.red);
        bool rightSideDown = Physics.Linecast(transform.position, transform.position + transform.right * 0.8f, LayerMask.GetMask("Terrain"));
        Debug.DrawLine(transform.position, transform.position + transform.right * 0.8f, Color.red);

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

        applyAntiroll(frontLeft, frontRight, frontAntiRoll);
        applyAntiroll(backLeft, backRight, backAntiRoll);
    }

    private void applyAntiroll(WheelCollider leftWheel, WheelCollider rightWheel, float antiRollAmount)
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
