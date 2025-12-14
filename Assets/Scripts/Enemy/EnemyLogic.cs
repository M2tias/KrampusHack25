using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLogic : MonoBehaviour
{
    private CarAI carAI;
    private EnemyState state;

    bool hasLootTarget = false;
    Pickup lootTarget;
    Transform findingLootTransform;

    void Start()
    {
        state = EnemyState.FindingLoot;
        carAI = GetComponent<CarAI>();
        findingLootTransform = new GameObject().transform;
    }

    void Update()
    {
        switch (state) {
            case EnemyState.FindingLoot:
                if (!hasLootTarget) {
                    Vector3 findingLootTargetPos;
                    List<Pickup> pickups = FindObjectsByType<Pickup>(FindObjectsSortMode.None).ToList();
                    lootTarget = pickups.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).FirstOrDefault();
                    Vector2 randomVec = Random.insideUnitCircle.normalized;
                    findingLootTargetPos = lootTarget.transform.position + new Vector3(randomVec.x, 0, randomVec.y) * 20f;
                    Physics.Raycast(findingLootTargetPos + Vector3.up * 10f, Vector3.down, out RaycastHit hitInfo, 100f, LayerMask.GetMask("Terrain"));
                    findingLootTransform.position = hitInfo.point;
                    hasLootTarget = true;
                }
                carAI.CustomDestination = findingLootTransform;
                break;
            default:

                break;
        }
    }
}

public enum EnemyState
{
    FindingLoot,
    Looting,
    Patrolling,
    Roaming,
    Fighting,
    Fleeing
}