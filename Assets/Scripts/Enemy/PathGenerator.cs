using UnityEngine;
using UnityEngine.AI;

public class PathGenerator : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;
    private bool pathReady = false;
    private NavMeshPath currentPath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.isStopped = true;
        agent.speed = 0;
        agent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPath.status == NavMeshPathStatus.PathComplete)
        {
            pathReady = true;
        }
    }

    public bool IsPathReady()
    {
        return pathReady;
    }

    public void GeneratePath(Vector3 targetPosition, NavMeshPath currentPath)
    {
        agent.enabled = true;
        agent.transform.position = transform.position;
        agent.CalculatePath(targetPosition, currentPath);
        this.currentPath = currentPath;
    }
}
