using System.Collections.Generic;
using UnityEngine;

public class EnemyReplayExecutor : MonoBehaviour
{
    private List<Vector3> recordedPositions;
    private List<Quaternion> recordedRotations;
    private int replayIndex = 0;

    public void StartReplay(List<Vector3> positions, List<Quaternion> rotations)
    {
        if (positions == null || rotations == null || positions.Count == 0 || rotations.Count == 0)
        {
            Debug.LogError("Replay data is missing or empty.");
            return;
        }

        recordedPositions = positions;
        recordedRotations = rotations;
        replayIndex = 0;
    }

    private void Update()
    {
        if (recordedPositions == null || recordedRotations == null || replayIndex >= recordedPositions.Count)
        {
            return;
        }

        transform.position = recordedPositions[replayIndex];
        transform.rotation = recordedRotations[replayIndex];
        replayIndex++;
    }
}
