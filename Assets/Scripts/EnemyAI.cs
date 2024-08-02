using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // 플레이어의 위치를 저장할 변수입니다.
    public Transform player;
    // NavMeshAgent 컴포넌트를 참조할 변수입니다.
    private NavMeshAgent agent;

    void Start()
    {
        // NavMeshAgent 컴포넌트를 가져옵니다.
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 플레이어의 위치로 NavMeshAgent의 목적지를 설정합니다.
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }
}   

