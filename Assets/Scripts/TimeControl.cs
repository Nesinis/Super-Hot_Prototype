using UnityEngine;

public class TimeControl : MonoBehaviour
{
    // Player 이동 속도 변수
    public float moveSpeed = 5f;

    // 시간 스케일 변수
    private float timeScale = 0f;

    // 시간 변화 속도
    public float smoothTime = 0.1f;

    // 목표 시간 스케일
    public float targetTimeScale = 1f;

    // 최소 시간 스케일
    public float minTimeScale = 0.1f;

    // 최대 시간 스케일
    public float maxTimeScale = 1f;

    private void Update()
    {
        // 부드러운 시간 변화를 위한 SmoothDamp 사용
        timeScale = Mathf.SmoothDamp(timeScale, targetTimeScale, ref moveSpeed, smoothTime);

        // 시간 스케일 적용
        Time.timeScale = timeScale;
    }

  

    public void UpdateTimeScale(bool isPlayerMoving)
    {
        if (isPlayerMoving)
        {
            // 플레이어가 움직이고 있다면 목표 시간 스케일을 최대로 설정
            targetTimeScale = maxTimeScale;
        }
        else
        {
            // 그렇지 않다면 목표 시간 스케일을 최소로 설정
            targetTimeScale = minTimeScale;
        }
    }
}