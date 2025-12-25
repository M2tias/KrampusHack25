using UnityEngine;

public class MinimapCompass : MonoBehaviour
{
    [SerializeField]
    private Transform compassArt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateCompassDir(Vector2 dir)
    {
        float angle = -1f * Vector2.SignedAngle(Vector2.up, dir);
        compassArt.rotation = Quaternion.Euler(0, 0, angle);
    }
}
