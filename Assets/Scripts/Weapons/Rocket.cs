using System.Runtime.CompilerServices;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float searchRange;
    [SerializeField]
    private float searchAngle;

    private int targetLayerMask;
    private Vector3 currentDirection;   
    private Transform target;
    private Vector3 currentTargetPos;
    private float redirectCD = 0.333f;
    private float lastRedirect;
    private float initialSpeed;
    private float targetSpeed;
    private float currentSpeed;
    private float startTime;

    private float rotateSpeed;

    private Rigidbody rb;

    public void Init(Vector3 initialDirection, Transform initialTarget, string targetLayer, float initialSpeed, float targetSpeed, float rotateSpeed)
    {
        if (targetLayer == "Enemy")
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        }
        else if (targetLayer == "Player")
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        }

        targetLayerMask = LayerMask.GetMask(targetLayer);
        currentDirection = initialDirection;
        target = initialTarget;
        transform.rotation = Quaternion.LookRotation(initialDirection);
        lastRedirect = Time.time - redirectCD * 2f;
        startTime = Time.time;
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = initialDirection * initialSpeed;
        currentSpeed = initialSpeed;
        this.initialSpeed = initialSpeed;
        this.targetSpeed = targetSpeed;
        this.rotateSpeed = rotateSpeed;
    }

    void Start()
    {
    }

    void Update()
    {
        currentSpeed = Mathf.Lerp(initialSpeed, targetSpeed, Time.time - startTime);

        if (Time.time - lastRedirect > redirectCD)
        {
            bool rayHit = Physics.Raycast(transform.position, target.position - transform.position, searchRange, targetLayerMask, QueryTriggerInteraction.Ignore);
            float angle = Vector3.Angle(target.position - transform.position, transform.forward);

            if (angle <= searchAngle)
            {
                currentTargetPos = target.position;
            }
        }

        var q = Quaternion.LookRotation(currentTargetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);

        rb.linearVelocity = transform.forward * currentSpeed;

        float targetDistance = (currentTargetPos - transform.position).magnitude;

        if (targetDistance < 1.5f)
        {
            Destroy(gameObject);
        }

        // Debug.Log($"Rocket distance from target {targetDistance}");//rotation {transform.rotation} towards {q} pointing at {target}");
    }
}