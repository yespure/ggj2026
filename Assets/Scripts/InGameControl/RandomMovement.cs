using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomMovement : MonoBehaviour
{
    private float moveRadius = 10f;
    private float waitTime = Random.Range(0,3);
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RandomLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RandomLoop()
    {
        while (true)
        {
            Vector3 target = GetRandomPoint();
            agent.SetDestination(target);
            yield return new WaitUntil(() =>
               !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance);
            yield return new WaitForSeconds(waitTime);

        }

        Vector3 GetRandomPoint()
        {
            Vector3 randomPos = Random.insideUnitSphere * moveRadius + transform.position;
            NavMeshHit hit;

            NavMesh.SamplePosition(randomPos, out hit, moveRadius, NavMesh.AllAreas);
            return hit.position;
        }
    }
}
