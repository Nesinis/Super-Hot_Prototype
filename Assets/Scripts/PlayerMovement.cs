using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // Player 이동 속도 변수
    public float moveSpeed = 5f;
    // 마우스 감도 변수
    public float mouseSensitivity = 2f;
    // 점프력 변수
    public float jumpForce = 5f;
    // 중력 변수
    public float gravity = -9.81f;

    // CharacterController 컴포넌트를 참조하기 위한 변수
    private CharacterController controller;
    // 플레이어 이동 방향을 저장하는 변수
    private Vector3 moveDirection;
    // 플레이어의 수직 속도를 저장하는 변수
    private Vector3 velocity;
    // TimeControl 스크립트를 참조하기 위한 변수
    private TimeControl timeControl;

    // 수직 회전값을 저장하는 변수
    private float verticalRotation = 0f;
    // 이전 프레임의 Y 위치를 저장하는 변수
    private float lastYPosition;
    // 플레이어의 이동/점프 상태를 저장하는 변수
    private bool isMoving = false;
    private bool isJumping = false;

    public GameObject PlayerPistol; // 플레이어가 들고 있는 총
    private GameObject ThrownEnemyPistol;
    public float pickupRange = 2.0f; // 총을 집을 수 있는 범위

    public Transform punchOrigin; // 주먹 공격의 시작 지점
    public float punchRange = 1.0f; // 주먹 공격의 범위

    private Animator animator; // Animator 컴포넌트를 참조하기 위한 변수

    void Start()
    {
        // CharacterController 컴포넌트를 가져온다
        controller = GetComponent<CharacterController>();
        // Scene 에서 TimeControl 스크립트를 찾아서 참조한다
        timeControl = FindObjectOfType<TimeControl>();

        // Animator 컴포넌트를 가져온다
        animator = GetComponent<Animator>();

        // 마우스 커서를 숨기고 고정
        Cursor.lockState = CursorLockMode.Locked;
        // 초기 Y 위치를 저장
        lastYPosition = transform.position.y;
    }

    void Update()
    {
        // 회전 처리
        HandleRotation();
        // 이동 처리
        HandleMovement();
        // 점프와 중력 처리
        HandleJumpAndGravity();
        // 시간 조절 업데이트
        UpdateTimeControl();

        getPistol(); // 플레이어가 총을 잡는다.
        HandleAttack(); // 플레이어 공격 처리
        

        // 다른 업데이트 로직

        // 총을 들고 있는지 확인하고 애니메이터 파라미터를 설정
        if (PlayerPistol != null)
        {
            bool hasGun = PlayerPistol.activeInHierarchy;
            animator.SetBool("HasGun", hasGun);

            // 확인을 위한 디버그 로그
            Debug.Log("HasGun: " + hasGun);
        }
    }

    void HandleRotation()
    {
        // 마우스 입력을 받는다
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 좌우 회전
        transform.Rotate(Vector3.up * mouseX);

        // 상하 회전 (카메라에 적용)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 이동이 발생하는지 감지
        if (move.magnitude > 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    //bool IsGrounded()
    //{
    //    return Physics.Raycast(transform.position, Vector3.down, controller.height / 2 + 0.1f);
    //}

    void HandleJumpAndGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false; // 착지 시 점프 상태 해제
        }

        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true; // 점프 시 상태 업데이트
            isMoving = true;  // 점프 시에도 이동 중으로 간주
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (!controller.isGrounded)
        {
            isMoving = true; // 공중에 있을 때는 이동 중으로 간주
        }
    }

    void UpdateTimeControl()
    {
        if (timeControl != null)
        {
            // 한 프레임 내에서 isMoving 상태가 일관되게 유지되도록
            bool currentMovingState = isMoving;
            timeControl.UpdateTimeScale(currentMovingState);
        }
    }
    IEnumerator getPistolCoroutine()
    {
        if (ThrownEnemyPistol != null && PlayerPistol != null)
        {
            float distanceToPistol = Vector3.Distance(transform.position, ThrownEnemyPistol.transform.position);

            if (distanceToPistol <= pickupRange)
            {
                if (!PlayerPistol.activeInHierarchy)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        ThrownEnemyPistol.SetActive(false);

                        yield return new WaitForSeconds(0.1f); // 0.1초 대기

                        PlayerPistol.SetActive(true);

                        PlayerFire playerFire = GetComponent<PlayerFire>();
                        if (playerFire != null)
                        {
                            playerFire.ResetAmmoCount();
                        }
                        else
                        {
                            Debug.LogError("PlayerFire 스크립트를 Player 오브젝트에서 찾을 수 없습니다.");
                        }

                        print("Pistol picked up and activated");
                    }
                }
            }
        }
    }

    void HandleAttack()
    {
        if (PlayerPistol != null && PlayerPistol.activeInHierarchy)
        {
            // 총이 활성화되어 있을 때는 총으로 공격
            if (Input.GetButtonDown("Fire1"))
            {
                animator.SetTrigger("Shoot"); // 총 발사 애니메이션 트리거
                // 총 발사 처리 (다른 스크립트에서 처리하는 경우 여기에선 호출하지 않음)
                Debug.Log("총 발사!");
            }
        }
        else
        {
            // 총이 없을 때는 주먹으로 공격
            if (Input.GetButtonDown("Fire1"))
            {
                animator.SetTrigger("Punch"); // 주먹 공격 애니메이션 트리거
                Debug.Log("주먹 공격!");
                Punch();
            }
        }
    }

    void getPistol()
    {
        StartCoroutine(getPistolCoroutine()); // 코루틴 시작
    }

    void Punch()
    {
        // 주먹 공격을 할 때 적과 충돌을 감지
        RaycastHit hit;
        if (Physics.Raycast(punchOrigin.position, punchOrigin.forward, out hit, punchRange))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakePunchDamage(); // 적에게 펀치 데미지를 입힘
            }
        }
    }

    public void UpdateThrownEnemyPistol(GameObject thrownPistol)
    {
        ThrownEnemyPistol = thrownPistol;
    }


}
