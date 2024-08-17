using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaySystemManager : MonoBehaviour
{
    public PlayerReplayRecorder playerRecorder;
    public PlayerReplayExecutor playerExecutor;
    public EnemyReplayRecorder[] enemyRecorders;
    public EnemyReplayExecutor[] enemyExecutors;

    private bool isReplaying = false;

    void Start()
    {
        // 초기화 및 시작 시 데이터 기록
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReplaying) // R 키로 리플레이 시작
        {
            StartReplay();
        }
    }

    public void StartReplay()
    {
        if (isReplaying)
        {
            Debug.Log("Replay is already in progress.");
            return;
        }
        isReplaying = true;
        Debug.Log("Replay started.");


        // 플레이어 데이터 설정
        var (playerPositions, playerRotations) = playerRecorder.GetRecordedData();
        playerExecutor.StartReplay(playerPositions, playerRotations);

        // 적 데이터 설정
        for (int i = 0; i < enemyRecorders.Length; i++)
        {
            var (enemyPositions, enemyRotations) = enemyRecorders[i].GetRecordedData();
            enemyExecutors[i].StartReplay(enemyPositions, enemyRotations);
        }
    }
}
