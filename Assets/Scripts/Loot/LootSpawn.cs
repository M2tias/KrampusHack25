using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootSpawn : MonoBehaviour
{
    [SerializeField]
    private List<string> collisionLayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Debug.Log($"Loot spawned to pos {transform.position}");
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 100f, LayerMask.GetMask(collisionLayers.ToArray()));

        if (hitInfo.collider != null)
        {
            transform.position = hitInfo.point + Vector3.up * 0.5f;
        }
        else
        {
            Debug.Log("Loot didn't find collision");
            // Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
