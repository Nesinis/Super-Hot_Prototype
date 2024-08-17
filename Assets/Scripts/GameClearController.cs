using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameClearController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // 비디오 플레이어
    public RawImage videoImage; // 비디오가 표시될 Raw Image
    public string enemyTag = "Enemy"; // 적 오브젝트의 태그

    private GameObject crosshair1; // Crosshair UI
    private GameObject crosshair2; // Crosshair UI

    void Start()
    {
        videoImage.gameObject.SetActive(false); // 초기에는 비디오 UI 비활성화

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
        videoImage.gameObject.SetActive(true); // 비디오 UI 활성화
        videoPlayer.Play(); // 비디오 플레이어 시작
        videoPlayer.loopPointReached += EndReached; // 비디오가 끝날 때 호출되는 이벤트 등록
    }

    void EndReached(VideoPlayer vp)
    {
        videoImage.gameObject.SetActive(false); // 비디오 UI 비활성화
        // 게임 클리어 후 다른 추가 동작이 필요하다면 여기에 추가
    }
}
