using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // VideoPlayer를 참조하기 위한 필드
    public RawImage restartVideoDisplay; // RawImage (RenderTexture) 참조를 위한 필드
    public SmoothFollow smoothFollow; // SmoothFollow 인스턴스 제어를 위한 필드

    void Start()
    {
        // RestartVideoDisplay (RawImage)를 처음에 비활성화
        if (restartVideoDisplay != null)
        {
            restartVideoDisplay.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("RestartVideoDisplay is not assigned!");
        }

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer is not assigned!");
        }

        if (smoothFollow == null)
        {
            Debug.LogError("SmoothFollow is not assigned!");
        }
    }

    // 플레이어가 차에 부딪히거나 총에 맞았는지 확인하는 메소드
    public void OnPlayerHit()
    {
        if (smoothFollow != null)
        {
            smoothFollow.enabled = true; // SmoothFollow 활성화
        }

        if (restartVideoDisplay != null)
        {
            restartVideoDisplay.gameObject.SetActive(true); // RawImage (RestartVideoDisplay) 활성화
        }

        if (videoPlayer != null)
        {
            videoPlayer.Play(); // 동영상 재생
        }
    }
}
