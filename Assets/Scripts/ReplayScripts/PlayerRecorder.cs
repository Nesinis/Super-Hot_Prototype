using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    private List<Vector3> recordedPositions = new List<Vector3>();
    private List<Quaternion> recordedRotations = new List<Quaternion>();

    public void Record()
    {
        recordedPositions.Add(transform.position);
        recordedRotations.Add(transform.rotation);
    }

    public List<Vector3> GetRecordedPositions()
    {
        return recordedPositions;
    }

    public List<Quaternion> GetRecordedRotations()
    {
        return recordedRotations;
    }
}
