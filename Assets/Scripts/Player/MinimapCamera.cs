using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField]
    private Transform minimapCam;
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Quaternion rotation;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        Vector3 mainCamDir = Camera.main.transform.forward;
        float angle = -1f * Vector2.SignedAngle(Vector2.up, new Vector2(mainCamDir.x, mainCamDir.z));
        minimapCam.transform.position = transform.position + position;
        minimapCam.transform.rotation = Quaternion.Euler(90f, 0, -angle);
    }
}
