using UnityEngine;

public class PlayerReplayRecorder : ReplayRecorder
{
    private void Update()
    {
        RecordFrame(transform);
    }
}
