using UnityEngine;

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
    // 플레이어의 이동 상태를 저장하는 변수
    private bool isMoving = false;

    void Start()
    {
        // CharacterController 컴포넌트를 가져온다
        controller = GetComponent<CharacterController>();
        // Scene 에서 TimeControl 스크립트를 찾아서 참조한다
        timeControl = FindObjectOfType<TimeControl>();

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
        // 수평과 수직 입력값을 받는다
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 입력값을 기반하여 이동 방향을 설정
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        // CharacterController를 사용하여 이동값을 처리
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 이동 상태 업데이트
        isMoving = (moveX != 0 || moveZ != 0);
    }

    void HandleJumpAndGravity()
    {
        // 점프 처리
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isMoving = true;
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 수직 이동 확인
        float currentYPosition = transform.position.y;
        if (Mathf.Abs(currentYPosition - lastYPosition) > 0.000001f)
        {
            isMoving = true;
        }
        lastYPosition = currentYPosition;
    }

    void UpdateTimeControl()
    {
        // TimeControl 스크립트에 플레이어 이동 상태 전달
        if (timeControl != null)
        {
            timeControl.UpdateTimeScale(isMoving);
        }
    }
}