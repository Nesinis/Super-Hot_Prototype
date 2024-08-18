using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameClearController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // 첫 번째 비디오 플레이어
    public RawImage videoImage; // 첫 번째 비디오가 표시될 Raw Image
    public string enemyTag = "Enemy"; // 적 오브젝트의 태그

    public VideoPlayer endVideoPlayer; // 두 번째 비디오 플레이어 (EndVideoPlayerObject에 부착)
    public RawImage endVideoImage; // 두 번째 비디오가 표시될 Raw Image (EndVideoDisplay)

    private GameObject crosshair1; // Crosshair UI
    private GameObject crosshair2; // Crosshair UI
    private bool firstVideoPlayed = false; // 첫 번째 비디오가 재생되었는지 확인하기 위한 플래그

    void Start()
    {
        // 초기에는 두 비디오 모두 비활성화
        videoImage.gameObject.SetActive(false);
        endVideoImage.gameObject.SetActive(false);

        // Scene에서 Crosshair UI 오브젝트를 찾아서 할당한다
        crosshair1 = GameObject.Find("Crosshair_1");
        crosshair2 = GameObject.Find("Crosshair_2");
    }

    void Update()
    {
        CheckForRemainingEnemies();
    }

    void CheckForRemainingEnemies()
    {
        // 씬에 남아있는 적 오브젝트가 있는지 확인
        GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (remainingEnemies.Length == 0)
        {
            PlayGameClearVideo(); // 모든 적이 제거되었을 때 비디오 재생

            // Crosshair_1과 Crosshair_2 비활성화
            if (crosshair1 != null)
            {
                crosshair1.SetActive(false);
            }
            if (crosshair2 != null)
            {
                crosshair2.SetActive(false);
            }
        }
    }

    void PlayGameClearVideo()
    {
        // 첫 번째 비디오 UI 활성화 및 재생
        videoImage.gameObject.SetActive(true);
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnFirstVideoEndReached; // 첫 번째 비디오가 끝날 때 호출되는 이벤트 등록
    }

    void OnFirstVideoEndReached(VideoPlayer vp)
    {
        // 첫 번째 비디오가 끝났으므로 두 번째 비디오 재생
        firstVideoPlayed = true;

        // 첫 번째 비디오 이벤트 등록 해제
        videoPlayer.loopPointReached -= OnFirstVideoEndReached;

        PlaySecondVideo();
    }

    void PlaySecondVideo()
    {
        // 두 번째 비디오 UI 활성화 및 재생
        Debug.Log("두 번째 비디오 UI 활성화 및 재생(void PlaySecondVideo())");

        endVideoImage.gameObject.SetActive(true);
        endVideoPlayer.Play();
        endVideoPlayer.loopPointReached += OnSecondVideoEndReached; // 두 번째 비디오가 끝날 때 호출되는 이벤트 등록
    }

    void OnSecondVideoEndReached(VideoPlayer vp)
    {
        // 두 번째 비디오 이벤트 등록 해제
        endVideoPlayer.loopPointReached -= OnSecondVideoEndReached;

        // 두 번째 비디오가 끝난 후 프로그램 종료
        endVideoImage.gameObject.SetActive(false); // 두 번째 비디오 UI 비활성화
        Application.Quit();

        // 에디터에서 실행 중일 때 강제로 종료되도록 함
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}