using UnityEngine;

public class EnemyReplayRecorder : ReplayRecorder
{
    private void Update()
    {
        RecordFrame(transform);
    }
}
