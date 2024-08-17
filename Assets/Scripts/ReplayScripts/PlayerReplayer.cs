using System.Collections.Generic;
using UnityEngine;

public class PlayerReplayer : MonoBehaviour
{
    private List<Vector3> recordedPositions;
    private List<Quaternion> recordedRotations;
    private int replayIndex = 0;

    public void Replay()
    {
        if (replayIndex < recordedPositions.Count)
        {
            transform.position = recordedPositions[replayIndex];
            transform.rotation = recordedRotations[replayIndex];
            replayIndex++;
        }
    }

    public void ResetReplay()
    {
        replayIndex = 0;
        recordedPositions = GetComponent<PlayerRecorder>().GetRecordedPositions();
        recordedRotations = GetComponent<PlayerRecorder>().GetRecordedRotations();
    }
}
