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

    public AudioClip deathSound; // 플레이어 사망 사운드
    private AudioSource audioSource; // AudioSource 컴포넌트

    void Start()
    {
        StartCoroutine(DisableCollisionAfterTime(1f));
        gameManager = FindObjectOfType<GameManager>(); // GameManager 찾기

        // AudioSource 컴포넌트를 가져옵니다.
        audioSource = GetComponent<AudioSource>();

        // audioSource가 할당되지 않은 경우에 대한 처리
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from this game object. Please add an AudioSource component.");
        }
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
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collisionEnabled)
            {
                Debug.Log("충돌이 활성화된 상태에서 플레이어와 충돌함");

                // 사망 사운드 재생
                if (audioSource != null && deathSound != null)
                {
                    audioSource.PlayOneShot(deathSound);
                }
                else
                {
                    Debug.LogWarning("Death sound or audio source is not assigned.");
                }

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

                gameObject.SetActive(false);
                collision.gameObject.SetActive(false);

                if (gameManager != null)
                {
                    gameManager.OnPlayerHit(); // GameManager의 메소드 호출
                }
            }
        }
    }
}
