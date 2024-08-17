using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // VideoPlayer가 없으면 더 이상 실행하지 않도록 수정
        if (videoPlayer == null)
        {
            Debug.LogWarning("VideoPlayer component is missing. Skipping video playback.");
            return;
        }

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("JumpScene");
    }
}
