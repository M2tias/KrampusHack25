using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    InputAction shootAction;
    Minigun minigun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
        minigun = GetComponent<Minigun>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: hold to fire & cooldown
        if (shootAction.IsPressed())
        {
            minigun.Shoot();
        }
    }
}
