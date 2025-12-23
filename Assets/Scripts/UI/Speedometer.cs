using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField]
    private float minSpeed;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private Quaternion minRotation;
    [SerializeField]
    private Quaternion maxRotation;
    [SerializeField]
    private Transform speedometerArm;

    private float currentSpeed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Clamp((currentSpeed - minSpeed) / (maxSpeed - minSpeed), 0, 1);
        speedometerArm.rotation = Quaternion.Slerp(minRotation, maxRotation, t);
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
}
