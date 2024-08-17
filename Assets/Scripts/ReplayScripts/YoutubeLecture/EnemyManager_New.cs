using UnityEngine;

public class EnemyManager_New : MonoBehaviour
{
    public ReplaySystemManager replaySystemManager;
    private int aliveEnemies;

    void Start()
    {
        aliveEnemies = FindObjectsOfType<EnemyReplayExecutor>().Length;
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        enemy.SetActive(false);
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            replaySystemManager.StartReplay();
        }
    }
}
