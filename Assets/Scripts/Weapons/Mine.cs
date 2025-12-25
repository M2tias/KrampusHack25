using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private float deployTime;

    private float damage;
    private float deploymentStarted;
    private bool isActive = false;

    public void Init(float dmg)
    {
        damage = dmg;
        deploymentStarted = Time.time;
        // TODO: handle positioning when deployed
        // Five raycasts to determine angle on hills?
    }

    void Start()
    {

    }

    void Update()
    {
        if (Time.time - deploymentStarted > deployTime)
        {
            isActive = true;
            // TODO: make a sound
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (!isActive)
        {
            return;
        }

        Hp hp = c.gameObject.GetComponent<Hp>();
        hp.DoDamage(damage);
        hp.PlayMineExplosion();
        // TODO: particles
        Destroy(gameObject);
    }
}
