using UnityEngine;

public class LootAnimation : MonoBehaviour
{
    [SerializeField]
    private Transform model;
    [SerializeField]
    private float heightDif;
    [SerializeField]
    private float heightSpeed;
    [SerializeField]
    private float rotationSpeed;

    private Vector3 originalPos;
    private Quaternion originalRotation;

    void Start()
    {
        originalPos = model.position;
        originalRotation = model.rotation;
    }

    void Update()
    {
        model.position = originalPos + Vector3.up * Mathf.Sin(Time.time * heightSpeed) * heightDif;
        model.rotation = Quaternion.Euler(model.rotation.x, 360f * Mathf.Sin(Time.time * rotationSpeed), model.rotation.z);
    }
}
