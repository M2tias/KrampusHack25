using UnityEngine;

public class YRotater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 eulers = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
