using UnityEngine;
using UnityEngine.Video;

public class StartingVideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public GameObject videoDisplayObject;  // 비디오가 출력되는 오브젝트 (RawImage 또는 다른 오브젝트)

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // 비디오가 끝난 후 실행할 동작
        videoDisplayObject.SetActive(false);  // 비디오 출력 오브젝트를 비활성화
    }
}
