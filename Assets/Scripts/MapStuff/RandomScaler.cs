using UnityEngine;

public class RandomScaler : MonoBehaviour
{
    [SerializeField]
    private float minScale;

    [SerializeField]
    private float maxScale; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
