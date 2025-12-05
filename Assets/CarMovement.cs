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

    private readonly float acceleration = 500f;
    private readonly float breakingForce = 300f;
    private readonly float maxTurnAngle = 15f;

    private float currentAcceleration = 0f;
    private float currentBreakForce = 0f;
    private float currentTurnAngle = 0f;

    InputAction moveAction;
    InputAction breakAction;
    Vector2 moveVector = Vector2.zero;
    bool isBreaking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        breakAction = InputSystem.actions.FindAction("Break");
    }

    void Update()
    {
        moveVector = moveAction.ReadValue<Vector2>();
        isBreaking = breakAction.IsPressed();
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

        frontRight.motorTorque = currentAcceleration;
        frontLeft.motorTorque = currentAcceleration;

        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;

        currentTurnAngle = maxTurnAngle * (-1) * moveVector.x;
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;


    }
}
