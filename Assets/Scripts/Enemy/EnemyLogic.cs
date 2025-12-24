using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField]
    private List<WheelCollider> wheels;

    private EnemyState state;
    private EnemyState previousState;
    private NavMeshAgent agent;
    private CharacterLoot loot;
    private PathGenerator pathGen;

    private bool hasLootTarget = false;
    private Pickup lootTarget;
    private Transform findingLootTransform;
    private NavMeshPath currentPath;
    private Vector3 currentCorner;
    private int cornersTraversed = 0;

    private bool onGround = false;
    private bool init = false;
    private bool isReversing = false;
    private bool stuckReverse = false;
    private float stuckReverseTime = 0.4f;
    private float stuckReverseStarted = 0;
    private int stuckReverseCounter = 0;
    private float stuckReverseCounterResetTime = 10f;
    private float lastStuckReverseTime = 0f;
    private float stuckReverseTimeAdd = 0.25f;

    private CarMovement carMovement;
    private float minCornerSpeed = 5f;
    private float maxCornerSpeed = 20f;
    private float maxCornerAngle = 90f; // angle at which speed is at minimu
    private bool isPathReady = false;


    void Start()
    {
        state = EnemyState.FindingLoot;
        agent = GetComponent<NavMeshAgent>();
        carMovement = GetComponent<CarMovement>();
        loot = GetComponent<CharacterLoot>();
        pathGen = GetComponent<PathGenerator>();
        findingLootTransform = new GameObject().transform;
        currentPath = new();
    }

    void Update()
    {
        isPathReady = pathGen.IsPathReady();

        if (!stuckReverse)
        {
            stuckReverseStarted = Time.time;

            int breakMask = LayerMask.GetMask("Default", "Terrain", "NotWalkable");
            stuckReverse = Physics.Raycast(transform.position, transform.forward, 3f, breakMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position, transform.position + transform.forward * 3f, Color.chartreuse);
            stuckReverse |= Physics.Raycast(transform.position + transform.right, transform.forward, 3f, breakMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position + transform.right, transform.position + transform.right + transform.forward * 3f, Color.chartreuse);
            stuckReverse |= Physics.Raycast(transform.position - transform.right, transform.forward, 3f, breakMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position - transform.right, transform.position - transform.right + transform.forward * 3f, Color.chartreuse);

            if (stuckReverse)
            {
                carMovement.ReverseTurnAngle();
            }
        }

        if (Time.time - lastStuckReverseTime > stuckReverseCounterResetTime)
        {
            stuckReverseCounter = 0;
        }

        if (stuckReverse)
        {
            carMovement.Accelerate(-1);

            if (Time.time - stuckReverseStarted > (stuckReverseTime + stuckReverseTimeAdd * stuckReverseCounter))
            {
                stuckReverseCounter++;
                lastStuckReverseTime = Time.time;
                stuckReverse = false;
            }

            return;
        }

        switch (state)
        {
            case EnemyState.FindingLoot:
                if (!hasLootTarget)
                {
                    agent.enabled = true;
                    List<Pickup> pickups = FindObjectsByType<Pickup>(FindObjectsSortMode.None)
                        .Where(x => loot.GetPickupLevel(x.LootType) < x.LootLevel)
                        .ToList();
                    lootTarget = pickups.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).FirstOrDefault();
                    Physics.Raycast(lootTarget.transform.position + Vector3.up * 10f, Vector3.down, out RaycastHit hitInfo, 100f, LayerMask.GetMask("Terrain"));
                    Vector3 targetPosition = hitInfo.point;
                    cornersTraversed = 0;

                    // hasLootTarget = agent.CalculatePath(targetPosition, currentPath);
                    pathGen.GeneratePath(targetPosition, currentPath);
                    Debug.Log($"Path calculated: pickupcount {pickups.Count()} {currentPath.corners.Count()} {string.Join("|", currentPath.corners.Select(x => x.ToString()))}");
                }
                else if (isPathReady)
                {
                    agent.enabled = false;
                    for (int i = 1; i < currentPath.corners.Count(); i++)
                    {
                        Debug.DrawLine(currentPath.corners[i - 1], currentPath.corners[i], Color.magenta);
                    }

                    if (cornersTraversed >= currentPath.corners.Count())
                    {
                        state = EnemyState.Brake;
                        StopMoving();
                        Debug.Log($"Looted. Looting next item {cornersTraversed} | {currentPath.corners.Count()}");
                        hasLootTarget = false;
                        //state = EnemyState.Looting;
                    }
                    else
                    {
                        currentCorner = currentPath.corners[cornersTraversed];

                        if ((VecDiscardY(currentCorner) - VecDiscardY(transform.position)).magnitude <= 2f)
                        {
                            previousState = EnemyState.FindingLoot;
                            state = EnemyState.Brake;
                            StopMoving();
                            cornersTraversed++;
                        }
                        else
                        {
                            MoveTowardsTarget(currentCorner);
                        }
                    }
                }
                break;
            case EnemyState.Brake:
                bool hasStopped = carMovement.Brake();

                if (hasStopped)
                {
                    state = previousState;
                }
                break;
            default:
                Debug.Log("State is: " + state);
                break;
        }

        // Debug.Log($"Nav agent stuff: {agent.speed} {agent.acceleration} {agent.enabled} {agent.isActiveAndEnabled} {agent.isStopped}");
    }

    public void StopMoving()
    {
        carMovement.Steer(0);
        carMovement.Accelerate(0);
    }

    public void MoveTowardsTarget(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        float distance = toTarget.magnitude;
        Vector3 dir = (target - transform.position).normalized;

        float alignment = Vector3.Dot(transform.forward, dir);

        if (distance < 10f && alignment < 0f)
        {
            isReversing = true;
        }
        if (distance > 10f || alignment > 0.2f)
        {
            isReversing = false;
        }

        Vector3 forward = isReversing ? -transform.forward : transform.forward;

        // TODO: Raycast for obstacles

        Vector3 flatForward = VecDiscardY(forward);
        Vector3 flatDir = VecDiscardY(dir);

        float angle = Vector3.SignedAngle(flatForward, flatDir, Vector3.up);

        carMovement.Steer(angle);
        float maxSpeed = GetCornerSpeed(cornersTraversed);

        if (carMovement.GetSpeed() <= maxSpeed)
        {
            carMovement.Accelerate(isReversing ? -1f : 1f);
        }
    }

    private Vector3 VecDiscardY(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }

    private float GetCornerSpeed(int currentCornerIndex)
    {
        if (currentPath.corners.Length < 2 | (currentPath.corners.Length - currentCornerIndex) <= 1)
        {
            return maxCornerSpeed;
        }

        Vector3 currentPos = transform.position;
        Vector3 toCurrentCorner = (currentPath.corners[currentCornerIndex] - currentPos).normalized;
        Vector3 toNextCorner = (currentPath.corners[currentCornerIndex + 1] - currentPath.corners[currentCornerIndex]).normalized;

        float angle = Vector3.Angle(toCurrentCorner, toNextCorner);
        float t = Mathf.Clamp01(angle / maxCornerAngle);
        t = 1f - t;

        return Mathf.Lerp(minCornerSpeed, maxCornerSpeed, t);
    }
}

public enum EnemyState
{
    FindingLoot,
    Looting,
    Patrolling,
    Roaming,
    Fighting,
    Fleeing,
    Brake
}