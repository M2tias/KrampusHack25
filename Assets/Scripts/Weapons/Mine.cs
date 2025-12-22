using UnityEngine;

public class Mine : MonoBehaviour
{
    private float damage;

    void Init(float dmg)
    {
        damage = dmg;
        // TODO: handle positioning when deployed
        // Five raycasts to determine angle on hills?
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider c)
    {
        Hp hp = c.gameObject.GetComponent<Hp>();
        hp.DoDamage(damage);
        // TODO: particles
        Destroy(gameObject);
    }
}
