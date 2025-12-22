using UnityEngine;

public class RammingSpikes : MonoBehaviour
{
    [SerializeField]
    private float maxSpeedDamage;
    [SerializeField]
    private float minSpeedDamage;

    private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetInstanceID() == transform.parent.GetInstanceID())
        {
            return; // TODO: is this needed? :D
        }

        if (c.TryGetComponent(out Hp hp))
        {
            hp.DoDamage(20);
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
