using UnityEngine;

public class TimeControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float timeScale = 0f;
    public float smoothTime = 0.002f;

    public float targetTimeScale = 0.001f; // 게임 시작 시 느리게 시작
    public float minTimeScale = 0.001f;
    public float maxTimeScale = 1f;

    private void Start()
    {
        // 초기 시간 스케일을 느리게 설정
        Time.timeScale = minTimeScale;
        timeScale = minTimeScale;
    }

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
            if (targetTimeScale != maxTimeScale)
            {
                //Debug.Log("Player is moving. Setting targetTimeScale to maxTimeScale.");
            }
            targetTimeScale = maxTimeScale; // 플레이어가 움직이고 있을 때는 시간을 정상 속도로 설정
        }
        else
        {
            if (targetTimeScale != minTimeScale)
            {
                //Debug.Log("Player is not moving. Setting targetTimeScale to minTimeScale.");
            }
            targetTimeScale = minTimeScale; // 플레이어가 움직이지 않을 때는 시간을 느리게 설정
        }
    }
}
