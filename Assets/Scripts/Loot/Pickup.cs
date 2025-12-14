using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private PickupData data;

    public LootType LootType { get { return data.LootType; } }
    public int LootLevel { get { return data.LootLevel; } }
    public PickupData Data { get { return data; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}


[Serializable]
public class PickupData
{
    public int LootLevel;
    public LootType LootType;
}