using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    private List<EnemyAI> recordedEnemies = new List<EnemyAI>();
    private bool isReplaying = false;

    public void AddEnemy(EnemyAI enemy)
    {
        if (!recordedEnemies.Contains(enemy))
        {
            recordedEnemies.Add(enemy);
            Debug.Log($"Enemy {enemy.name} added to ReplayManager. Total enemies: {recordedEnemies.Count}");
        }
    }

    public void RemoveEnemy(EnemyAI enemy)
    {
        if (recordedEnemies.Contains(enemy))
        {
            recordedEnemies.Remove(enemy);
            Debug.Log($"Enemy {enemy.name} removed from ReplayManager. Remaining enemies: {recordedEnemies.Count}");

            if (recordedEnemies.Count == 0)
            {
                Debug.Log("All enemies defeated. Starting replay.");
                StartReplay();
            }
        }
        else
        {
            Debug.LogWarning($"Enemy {enemy.name} was not found in the list.");
        }
    }

    private void StartReplay()
    {
        if (isReplaying) return;

        isReplaying = true;
        Debug.Log("Replay started.");
        StartCoroutine(ReplayCoroutine());
    }

    private IEnumerator ReplayCoroutine()
    {
        Debug.Log("ReplayCoroutine started.");

        foreach (var enemy in recordedEnemies)
        {
            Debug.Log($"Replaying enemy: {enemy.name}");
            ReplayEnemy(enemy);
            yield return new WaitForSeconds(1f); // 각 적의 리플레이를 기본 속도로 보여줌
        }

        Debug.Log("ReplayCoroutine completed.");
    }

    private void ReplayEnemy(EnemyAI enemy)
    {
        // 각 적의 위치나 행동을 기록한 데이터를 기반으로 재생하는 로직을 구현
    }
}
