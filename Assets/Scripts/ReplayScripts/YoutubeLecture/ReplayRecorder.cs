using System.Collections.Generic;
using UnityEngine;

public class ReplayRecorder : MonoBehaviour
{
    private List<Vector3> positions = new List<Vector3>();
    private List<Quaternion> rotations = new List<Quaternion>();
    private bool isRecording = true;

    public void RecordFrame(Transform target)
    {
        if (isRecording)
        {
            positions.Add(target.position);
            rotations.Add(target.rotation);
        }
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public (List<Vector3>, List<Quaternion>) GetRecordedData()
    {
        return (positions, rotations);
    }
}
