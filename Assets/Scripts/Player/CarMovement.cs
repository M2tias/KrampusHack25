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
    private float acceleration;

    [SerializeField]
    private float maxWheelRPM;

    private readonly float breakingForce = 300f;
    private readonly float maxTurnAngle = 15f;

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

        if (upsideDown || leftSideDown || rightSideDown) {
            if (!wasUpsideDown){
                wentUpsideDown = Time.time;
                wasUpsideDown = true;
            }

            if(Time.time - wentUpsideDown > upsideDownResetTime) {
                Vector3 currentEuler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(new Vector3(currentEuler.x, currentEuler.y, 0f));
                transform.position = transform.position + Vector3.up * 2f;
                Debug.DrawLine(transform.position - transform.up * 1f, transform.position + transform.up * 3f, Color.green);
            }
        } else {
            wasUpsideDown = false;
            wentUpsideDown = Time.time;
        }
    }
}
