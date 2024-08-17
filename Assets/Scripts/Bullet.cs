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

    // TimeControl 스크립트를 참조하기 위한 변수
    private TimeControl timeControl;

    private bool isMoving = false;

    private GameManager gameManager; // GameManager를 참조하기 위한 변수

    void Start()
    {
        // Scene 에서 TimeControl 스크립트를 찾아서 참조한다
        timeControl = FindObjectOfType<TimeControl>();

        // Scene에서 Crosshair UI 오브젝트를 찾아서 할당한다
        crosshair1 = GameObject.Find("Crosshair_1");
        crosshair2 = GameObject.Find("Crosshair_2");

        // Rigidbody 설정
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 초기 속도를 설정하여 한 방향으로 날아가도록 합니다.
            rb.velocity = transform.forward * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet 프리팹에 Rigidbody 컴포넌트가 없습니다.");
        }
    }

    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어의 자식 오브젝트로 있는 카메라를 찾음
            Transform playerCamera = other.transform.Find("Main Camera");

            if (playerCamera != null)
            {
                // 카메라의 월드 좌표를 저장
                Vector3 cameraPosition = playerCamera.position;
                Quaternion cameraRotation = playerCamera.rotation;

                isMoving = true;

                // 카메라를 플레이어 오브젝트에서 분리
                playerCamera.SetParent(null);

                // 카메라의 월드 좌표를 설정 (Y 좌표 고정)
                cameraPosition.y = 1.080001f;
                playerCamera.position = cameraPosition;
                playerCamera.rotation = cameraRotation; // 회전값을 월드 좌표계 기준으로 유지

                // 부드럽게 회전하도록 SmoothFollow 스크립트를 추가
                playerCamera.gameObject.AddComponent<SmoothFollow>();

                // 카메라 활성화 (필요시)
                playerCamera.gameObject.SetActive(true);
            }

            // Crosshair_1과 Crosshair_2 비활성화
            if (crosshair1 != null)
            {
                crosshair1.SetActive(false);
            }
            if (crosshair2 != null)
            {
                crosshair2.SetActive(false);
            }


            // 플레이어 사망 처리
            Destroy(other.gameObject);

            if (gameManager != null)
            {
                Debug.Log("GameManager의 메소드 호출");
                gameManager.OnPlayerHit(); // GameManager의 메소드 호출
            }
        }
        else if (other.CompareTag("Enemy"))
        {
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
