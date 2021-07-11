using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{    
    [SerializeField] Transform player;
    NavMeshAgent navMeshAgent;
    
    [SerializeField] float chaseRange = 30f;
    float distanceToTarget;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    void Update()
    {
        distanceToTarget = Vector3.Distance(player.position, transform.position);

        if(distanceToTarget <= chaseRange)
        {
            navMeshAgent.SetDestination(player.position);  //NavMesh yardýmýyla düþmanýn oyuncuyu takibi
        }
    }    
}
