using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // �÷��̾��� ��ġ�� ������ �����Դϴ�.
    public Transform player;
    // NavMeshAgent ������Ʈ�� ������ �����Դϴ�.
    private NavMeshAgent agent;

    void Start()
    {
        // NavMeshAgent ������Ʈ�� �����ɴϴ�.
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // �÷��̾��� ��ġ�� NavMeshAgent�� �������� �����մϴ�.
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }
}   

