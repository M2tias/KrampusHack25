using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    public float Damage { get { return damage; } }

    public void Init(float damage)
    {
        this.damage = damage;
    }
    
    void Start()
    {

    }

    void Update()
    {

    }
}
