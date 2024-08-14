using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosition : MonoBehaviour
{
    public Vector3 fixedPosition; // 고정할 위치 값
    public Vector3 fixedRotation; // 고정할 회전 값

    void LateUpdate()
    {
        // 고정된 위치와 회전 값으로 설정
        transform.localPosition = fixedPosition;
        transform.localEulerAngles = fixedRotation;
    }
}