using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10.0f; // 총알 속도

    private Rigidbody rb;

    private GameObject crosshair1; // Crosshair UI
    private GameObject crosshair2; // Crosshair UI

    public float restartHoldTime = 2.0f; // R 키를 길게 눌러야 하는 시간 (초)

    private float holdTime = 0f; // R 키가 눌린 시간을 추적

    private TimeControl timeControl; // TimeControl 스크립트를 참조하기 위한 변수

    private bool isMoving = false;

    private GameManager gameManager; // GameManager를 참조하기 위한 변수

    public AudioClip playerDeathSound; // 플레이어 사망 사운드
    public AudioClip enemyDeathSound; // 적 사망 사운드
    private AudioSource audioSource; // AudioSource 컴포넌트

    void Start()
    {
        timeControl = FindObjectOfType<TimeControl>();

        crosshair1 = GameObject.Find("Crosshair_1");
        crosshair2 = GameObject.Find("Crosshair_2");

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet 프리팹에 Rigidbody 컴포넌트가 없습니다.");
        }

        // AudioSource 컴포넌트를 가져옵니다.
        audioSource = GetComponent<AudioSource>();

        // audioSource가 할당되지 않은 경우에 대한 처리
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from this game object. Please add an AudioSource component.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 죽을 때 사망 사운드 재생
            if (audioSource != null && playerDeathSound != null)
            {
                audioSource.PlayOneShot(playerDeathSound);
            }
            else
            {
                Debug.LogWarning("Player death sound or audio source is not assigned.");
            }

            Transform playerCamera = other.transform.Find("Main Camera");

            if (playerCamera != null)
            {
                Vector3 cameraPosition = playerCamera.position;
                Quaternion cameraRotation = playerCamera.rotation;

                isMoving = true;

                playerCamera.SetParent(null);

                cameraPosition.y = 1.080001f;
                playerCamera.position = cameraPosition;
                playerCamera.rotation = cameraRotation;

                playerCamera.gameObject.AddComponent<SmoothFollow>();
                playerCamera.gameObject.SetActive(true);
            }

            if (crosshair1 != null)
            {
                crosshair1.SetActive(false);
            }
            if (crosshair2 != null)
            {
                crosshair2.SetActive(false);
            }

            Destroy(other.gameObject);

            if (gameManager != null)
            {
                Debug.Log("GameManager의 메소드 호출");
                gameManager.OnPlayerHit(); // GameManager의 메소드 호출
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            // 적이 죽을 때 사망 사운드 재생
            if (audioSource != null && enemyDeathSound != null)
            {
                audioSource.PlayOneShot(enemyDeathSound);
            }
            else
            {
                Debug.LogWarning("Enemy death sound or audio source is not assigned.");
            }

            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                Debug.Log("Bullet hit the enemy."); // 디버그 메시지 추가
                enemyAI.Die(); // 적을 즉시 사망 처리
            }
            Destroy(gameObject); // 총알을 파괴합니다.
        }
        else
        {
            Destroy(gameObject); // 다른 것에 충돌했을 때 총알을 파괴합니다.
        }
    }
}
