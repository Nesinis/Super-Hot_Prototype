using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    // slowMotionFactor: 플레이어가 멈췄을 때 시간의 흐름 속도를 설정하는 변수
    public float slowMotionFactor = 0.1f;
    // isPlayerMoving: 플레이어가 움직이고 있는지 여부를 저장하는 변수
    private bool isPlayerMoving;

    void Update()
    {
        // 매 프레임 마다 시간 제어를 처리하는 함수
        ControlTime();
    }

    // CheckPlayerMovement: Player 스크립트에서 호출되어 플레이어의 움직임을 감지하는 함수
    public void CheckPlayerMovement(float moveX, float moveZ)
    {
        // 수평이나 수직 입력이 있는지 확인하여 Player가 움직이는지 여부를 결정
        isPlayerMoving = moveX != 0 || moveZ != 0;
    }

    // ControlTime: 플레이어의 움직임에 따라 시간을 제어하는 함수
    void ControlTime()
    {
        // 만약 플레이어가 움직일 때
        if (isPlayerMoving)
        {
            // 시간이 정상적인 속도로 움직임
            Time.timeScale = 1f;
            // 물리 계산 주기도 정상 속도로 맞춤
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        // 아니라면
        else
        {
            // 플레이어가 멈췄을 때 시간이 느리게 흐름
            Time.timeScale = slowMotionFactor;
            // 물리 계산 주기도 느린 시간 속도에 맞춤
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}
