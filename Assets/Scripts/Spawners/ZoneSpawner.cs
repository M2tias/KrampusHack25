using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject zonePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<Vector3> positions = new();
        
        foreach (Transform t in transform)
        {
            positions.Add(t.position);
            Destroy(t.gameObject);
        }

        Instantiate(zonePrefab, positions.OrderBy(x => Guid.NewGuid()).First(), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
