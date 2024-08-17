using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public ReplayManager replayManager;

    void Start()
    {
        foreach (EnemyAI enemy in FindObjectsOfType<EnemyAI>())
        {
            AddEnemyToReplayManager(enemy);
        }
    }

    public void AddEnemyToReplayManager(EnemyAI enemy)
    {
        if (replayManager != null)
        {
            replayManager.AddEnemy(enemy);
        }
    }

    public void OnEnemyDeath(EnemyAI enemy)
    {
        var enemyToRemove = enemy;
        if (replayManager != null)
        {
            replayManager.RemoveEnemy(enemyToRemove);
        }

    }
}
