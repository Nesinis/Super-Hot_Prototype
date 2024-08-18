using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float driftAmount = 5f;

    private bool hasDrifted = false;
    private bool collisionEnabled = true;

    public GameObject crosshair1;
    public GameObject crosshair2;
    public GameObject restartInstructionImage;

    public VideoPlayer videoPlayer;
    public RawImage restartVideoDisplay;

    private GameManager gameManager; // GameManager를 참조하기 위한 변수

    void Start()
    {
        StartCoroutine(DisableCollisionAfterTime(1f));
        gameManager = FindObjectOfType<GameManager>(); // GameManager 찾기
    }

    void Update()
    {
        if (!hasDrifted)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            StartCoroutine(Drift());
        }
    }

    IEnumerator Drift()
    {
        yield return new WaitForSeconds(1f);
        transform.Rotate(Vector3.up, driftAmount);

        hasDrifted = true;
        speed = 0f;
    }

    IEnumerator DisableCollisionAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        collisionEnabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("충돌 감지됨");

        // 충돌한 객체가 플레이어인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collisionEnabled)
            {
                Debug.Log("충돌이 활성화된 상태에서 플레이어와 충돌함");

                // 차량과 플레이어 즉시 비활성화
                DeactivateCarAndPlayer(collision);
            }
        }
    }

    void DeactivateCarAndPlayer(Collision collision)
    {
        // 플레이어 카메라 설정 및 UI 처리 코드...
        Transform playerCamera = collision.transform.Find("Main Camera");

        if (playerCamera != null)
        {
            Vector3 cameraPosition = playerCamera.position;
            Quaternion cameraRotation = playerCamera.rotation;

            playerCamera.SetParent(null);
            cameraPosition.y = 1.080001f;
            playerCamera.position = cameraPosition;
            playerCamera.rotation = cameraRotation;

            playerCamera.gameObject.AddComponent<SmoothFollow>();
            playerCamera.gameObject.SetActive(true);
        }

        if (crosshair1 != null) crosshair1.gameObject.SetActive(false);
        if (crosshair2 != null) crosshair2.gameObject.SetActive(false);
        if (restartInstructionImage != null) restartInstructionImage.gameObject.SetActive(true);

        if (videoPlayer != null && restartVideoDisplay != null)
        {
            restartVideoDisplay.gameObject.SetActive(true);
            videoPlayer.Play();
        }

        collision.gameObject.SetActive(false); // 플레이어 비활성화
        gameObject.SetActive(false); // 차량 비활성화
    }
}
