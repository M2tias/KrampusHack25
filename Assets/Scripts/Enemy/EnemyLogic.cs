using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;
using System;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField]
    private List<WheelCollider> wheels;

    private EnemyState state;
    private EnemyState previousState;
    private NavMeshAgent agent;
    private CharacterLoot loot;
    private PathGenerator pathGen;
    private Shooting shooting;
    private Hp hp;

    private bool hasTarget = false;
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
    private float maxCornerAngle = 90f; // angle at which speed is at minimum

    private float findFightRadius = 120f;
    private float shootRadius = 70f;
    private Transform fightTarget;
    private float fightPathGeneratedTime = 0f;
    private float fightRePathTime = 1f;

    void Start()
    {
        state = EnemyState.FindingLoot;
        agent = GetComponent<NavMeshAgent>();
        carMovement = GetComponent<CarMovement>();
        loot = GetComponent<CharacterLoot>();
        pathGen = GetComponent<PathGenerator>();
        shooting = GetComponent<Shooting>();
        hp = GetComponent<Hp>();
        findingLootTransform = new GameObject().transform;
        currentPath = new();
    }

    void Update()
    {
        hasTarget = pathGen.IsPathReady();
        DrawCircle(transform.position, findFightRadius, Color.chocolate);
        DrawCircle(transform.position, shootRadius, Color.chocolate);

        if (!stuckReverse)
        {
            stuckReverseStarted = Time.time;

            int breakMask = LayerMask.GetMask("Default", "Terrain", "NotWalkable");
            stuckReverse = Physics.Raycast(transform.position, transform.forward, 3f, breakMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position, transform.position + transform.forward * 3f, Color.chartreuse);
            stuckReverse |= Physics.Raycast(transform.position + transform.right * 1.05f, transform.forward, 3f, breakMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position + transform.right * 1.05f, transform.position + transform.right * 1.05f + transform.forward * 3f, Color.chartreuse);
            stuckReverse |= Physics.Raycast(transform.position - transform.right * 1.05f, transform.forward, 3f, breakMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position - transform.right * 1.05f, transform.position - transform.right * 1.05f + transform.forward * 3f, Color.chartreuse);

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

        if (currentPath != null)
        {
            for (int i = 1; i < currentPath.corners.Count(); i++)
            {
                Debug.DrawLine(currentPath.corners[i - 1], currentPath.corners[i], Color.magenta);
            }
        }

        if (FightRequirements())
        {
            var targetEnemy = CharacterSpawner.main.Enemies
                .Select(x => new { distance = (x.transform.position - transform.position).magnitude, gameObject = x })
                .Where(x =>
                    x.gameObject.GetInstanceID() != gameObject.GetInstanceID() &&
                    x.distance < findFightRadius)
                .OrderBy(x => x.distance)
                .FirstOrDefault();

            GameObject player = CharacterSpawner.main.Player;
            float playerDistance = (player.transform.position - transform.position).magnitude;

            // targetEnemy is null or within fingFightRadius
            if (targetEnemy == null)
            {
                if (playerDistance < findFightRadius)
                {
                    fightTarget = player.transform;
                }
            }
            else if (targetEnemy.distance > playerDistance)
            {
                fightTarget = player.transform;
            }
            else
            {
                fightTarget = targetEnemy.gameObject.transform;
            }

            if (fightTarget != null)
            {
                state = EnemyState.Fighting;
            }
        }

        switch (state)
        {
            case EnemyState.FindingLoot:
                if (LootRequirements())
                {
                    // Debug.Log("Looting done. Roaming!");
                    state = EnemyState.Roaming;
                }

                if (!hasTarget)
                {
                    bool isMaxHP = hp.IsMaxHP();
                    
                    List<Pickup> pickups = FindObjectsByType<Pickup>(FindObjectsSortMode.None)
                        .Where(x => loot.GetPickupLevel(x.LootType) < x.LootLevel)
                        .Where(x => !isMaxHP || (x.LootType != LootType.Health))
                        .ToList();
                    lootTarget = pickups.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).FirstOrDefault();
                    Physics.Raycast(lootTarget.transform.position + Vector3.up * 10f, Vector3.down, out RaycastHit hitInfo, 100f, LayerMask.GetMask("Terrain"));
                    Vector3 targetPosition = hitInfo.point;
                    cornersTraversed = 0;

                    // hasLootTarget = agent.CalculatePath(targetPosition, currentPath);
                    pathGen.GeneratePath(targetPosition, currentPath);
                    // Debug.Log($"Path calculated: pickupcount {pickups.Count()} {currentPath.corners.Count()} {string.Join("|", currentPath.corners.Select(x => x.ToString()))}");
                }
                else
                {
                    //agent.enabled = false;
                    TravelToTarget();
                }
                break;
            case EnemyState.Brake:
                bool hasStopped = carMovement.Brake();

                if (hasStopped)
                {
                    state = previousState;
                }
                break;
            case EnemyState.Roaming:
                if (!hasTarget)
                {
                    List<Vector3> positions = PatrolPoints.PatrolPositions;
                    int count = positions.Count;
                    int randomCount = 4;
                    int halfIsh = Mathf.RoundToInt(((float)positions.Count) / 2f + randomCount / 2f);

                    Vector3 target = PatrolPoints.PatrolPositions
                        .OrderBy(x => (x - transform.position).sqrMagnitude)
                        .Skip(halfIsh).Take(randomCount)
                        .OrderBy(x => Guid.NewGuid()).First();
                    pathGen.GeneratePath(target, currentPath);
                }
                else
                {
                    TravelToTarget();
                }
                break;
            case EnemyState.Fighting:
                if (!hasTarget)
                {
                    pathGen.GeneratePath(fightTarget.position, currentPath);
                    fightPathGeneratedTime = Time.time;
                }
                else
                {
                    TravelToTarget();
                    if (loot.GetPickupLevel(LootType.Minigun) > 0)
                    {
                        if ((fightTarget.position - transform.position).magnitude <= shootRadius)
                        {
                            shooting.JustShoot();
                        }
                    }
                    else if (loot.GetPickupLevel(LootType.RammingSpike) > 0)
                    {
                        // TODO: Do I need to do anything here?
                    }

                    if (Time.time - fightPathGeneratedTime > fightRePathTime) {
                        hasTarget = false;
                        pathGen.PathFinished();
                    }
                }
                break;
            default:
                // Debug.Log("State is: " + state);
                break;
        }

        // Debug.Log($"Nav agent stuff: {agent.speed} {agent.acceleration} {agent.enabled} {agent.isActiveAndEnabled} {agent.isStopped}");
    }

    private void TravelToTarget()
    {
        if (cornersTraversed >= currentPath.corners.Count())
        {
            state = EnemyState.Brake;
            StopMoving();
            // Debug.Log($"Looted. Looting next item {cornersTraversed} | {currentPath.corners.Count()}");
            hasTarget = false;
            pathGen.PathFinished();
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

    private bool LootRequirements()
    {
        int minigunLevel = loot.GetPickupLevel(LootType.Minigun);
        int rammingSpikeLevel = loot.GetPickupLevel(LootType.RammingSpike);
        int mineLevel = loot.GetPickupLevel(LootType.Mines);
        int rocketLevel = loot.GetPickupLevel(LootType.Rockets);
        int tireLevel = loot.GetPickupLevel(LootType.Tires);
        int shieldLevel = loot.GetPickupLevel(LootType.Shield);

        bool minigunOrSpikes = minigunLevel > 0 || rammingSpikeLevel > 0;
        bool minesOrRockets = mineLevel > 0 || rocketLevel > 0;
        bool tiresOrShield = tireLevel > 0 || shieldLevel > 0;

        return minigunOrSpikes && minesOrRockets && tiresOrShield;
    }

    private bool FightRequirements()
    {
        int minigunLevel = loot.GetPickupLevel(LootType.Minigun);
        int rammingSpikeLevel = loot.GetPickupLevel(LootType.RammingSpike);
        int rocketLevel = loot.GetPickupLevel(LootType.Rockets);

        return minigunLevel > 0 || rammingSpikeLevel > 0 || rocketLevel > 0;
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

    private static void DrawCircle(Vector3 center, float radius, Color color)
    {
        int segments = 32;
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prevPoint, nextPoint, color);
            prevPoint = nextPoint;
        }
    }
}

public enum EnemyState
{
    FindingLoot,
    Looting,
    Roaming,
    Fighting,
    Fleeing,
    Brake
}