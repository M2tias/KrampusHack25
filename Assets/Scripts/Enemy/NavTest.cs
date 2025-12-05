using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(target.position);
        agent.isStopped = true;
        Debug.Log(agent.path.corners.Count());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i < agent.path.corners.Length; i++)
        {
            Debug.DrawLine(agent.path.corners[i - 1] + Vector3.up, agent.path.corners[i] + Vector3.up, Color.red);
        }
    }
}
