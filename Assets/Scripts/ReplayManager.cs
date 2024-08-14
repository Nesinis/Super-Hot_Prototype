using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    // 리플레이 데이터를 저장할 리스트
    private List<ReplayFrame> replayData = new List<ReplayFrame>();

    // 현재 녹화 중인지 여부를 나타내는 플래그
    private bool isRecording = false;

    // 현재 재생 중인지 여부를 나타내는 플래그
    private bool isPlaying = false;

    // 현재 재생 중인 프레임 인덱스
    private int currentFrame = 0;

    // ReplayFrame 클래스는 각 프레임의 데이터를 저장합니다.
    [System.Serializable]
    public class ReplayFrame
    {
        public Vector3 position;      // 플레이어의 위치
        public Quaternion rotation;   // 플레이어의 회전
        public float time;            // 해당 프레임의 시간
    }

    // 매 프레임마다 호출되는 Update 메서드
    void Update()
    {
        if (isRecording)
        {
            // 녹화 중이면 현재 프레임의 데이터를 기록
            RecordFrame();
        }
        else if (isPlaying)
        {
            // 재생 중이면 현재 프레임의 데이터를 사용해 재생
            PlayFrame();
        }
    }

    // 리플레이 녹화를 시작하는 메서드
    public void StartRecording()
    {
        isRecording = true;           // 녹화 시작 플래그 설정
        replayData.Clear();           // 이전에 기록된 데이터를 모두 삭제
    }

    // 리플레이 녹화를 중지하는 메서드
    public void StopRecording()
    {
        isRecording = false;          // 녹화 중지 플래그 설정
    }

    // 리플레이 재생을 시작하는 메서드
    public void StartPlayback()
    {
        isPlaying = true;             // 재생 시작 플래그 설정
        currentFrame = 0;             // 첫 번째 프레임부터 재생

        // 첫 프레임의 위치와 회전으로 플레이어를 설정
        if (replayData.Count > 0)
        {
            transform.position = replayData[0].position;
            transform.rotation = replayData[0].rotation;
        }
    }

    // 현재 프레임의 데이터를 기록하는 메서드
    void RecordFrame()
    {
        ReplayFrame frame = new ReplayFrame
        {
            position = transform.position,        // 현재 플레이어의 위치 기록
            rotation = transform.rotation,        // 현재 플레이어의 회전 기록
            time = Time.time                      // 현재 시간 기록
        };
        replayData.Add(frame);                    // 기록된 데이터를 리스트에 추가
    }

    // 기록된 프레임 데이터를 사용해 재생하는 메서드
    void PlayFrame()
    {
        if (currentFrame < replayData.Count)
        {
            transform.position = replayData[currentFrame].position;     // 저장된 위치로 이동
            transform.rotation = replayData[currentFrame].rotation;     // 저장된 회전으로 회전
            currentFrame++;                                             // 다음 프레임으로 이동

            if (currentFrame >= replayData.Count)
            {
                isPlaying = false;      // 모든 프레임이 재생되면 재생 중지
            }
        }
    }
}
