using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{
    public static List<Vector3> PatrolPositions = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int mask = LayerMask.GetMask("Default", "Terrain");

        foreach (Transform t in transform)
        {
            t.GetComponent<MeshRenderer>().enabled = false;
            Physics.Raycast(t.position, Vector3.down, out RaycastHit hitInfo, 100f, mask,QueryTriggerInteraction.Ignore);

            PatrolPositions.Add(hitInfo.point);
            t.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
