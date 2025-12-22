using UnityEngine;

public class ConeFov : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private float fovLength = 5f;
    private float fovAngle = 45f;
    private float fovLenSqr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fovLenSqr = fovLength * fovLength;
    }

    // Update is called once per frame
    void Update()
    {
        bool isHit = checkHit();



        Debug.DrawLine(transform.position, transform.position + transform.forward * fovLength, isHit ? Color.red : Color.yellowGreen);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, fovAngle, 0) * transform.forward * fovLength, Color.green);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -fovAngle, 0) * transform.forward * fovLength, Color.green);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(-fovAngle, 0, 0) * transform.forward * fovLength, Color.green);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(fovAngle, 0, 0) * transform.forward * fovLength, Color.green);


    }

    bool checkHit()
    {
        if ((transform.position - target.position).sqrMagnitude > fovLenSqr)
        {
            return false;
        }

        bool rayHit = Physics.Raycast(transform.position, target.position - transform.position, fovLength, LayerMask.GetMask("UI"), QueryTriggerInteraction.Ignore);
        float angle = CalcAngle(target.position - transform.position);
        Debug.Log(angle);

        return angle <= fovAngle;
    }

    private float CalcAngle(Vector3 newDirection) {
        // the vector that we want to measure an angle from
        Vector3 referenceForward = transform.forward;/* some vector that is not Vector3.up */

        // the vector perpendicular to referenceForward (90 degrees clockwise)
        // (used to determine if angle is positive or negative)
        Vector3 referenceRight = transform.right;

        // Get the angle in degrees between 0 and 180
        float angle = Vector3.Angle(newDirection, referenceForward);

        // Determine if the degree value should be negative. Here, a positive value
        // from the dot product means that our vector is on the right of the reference vector
        // whereas a negative value means we're on the left.
        float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));

        return Mathf.Abs(sign * angle);
    }
}
