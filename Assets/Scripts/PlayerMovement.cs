using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player �̵� �ӵ� ����
    public float moveSpeed = 5f;
    // ���콺 ���� ����
    public float mouseSensitivity = 2f;
    // ������ ����
    public float jumpForce = 5f;
    // �߷� ����
    public float gravity = -9.81f;

    // CharacterController ������Ʈ�� �����ϱ� ���� ����
    private CharacterController controller;
    // �÷��̾� �̵� ������ �����ϴ� ����
    private Vector3 moveDirection;
    // �÷��̾��� ���� �ӵ��� �����ϴ� ����
    private Vector3 velocity;
    // TimeControl ��ũ��Ʈ�� �����ϱ� ���� ����
    private TimeControl timeControl;

    // ���� ȸ������ �����ϴ� ����
    private float verticalRotation = 0f;
    // ���� �������� Y ��ġ�� �����ϴ� ����
    private float lastYPosition;
    // �÷��̾��� �̵� ���¸� �����ϴ� ����
    private bool isMoving = false;


    public GameObject PlayerPistol; // �÷��̾ ��� �ִ� ��
    private GameObject ThrownEnemyPistol;
    public float pickupRange = 2.0f; // ���� ���� �� �ִ� ����


    void Start()
    {
        // CharacterController ������Ʈ�� �����´�
        controller = GetComponent<CharacterController>();
        // Scene ���� TimeControl ��ũ��Ʈ�� ã�Ƽ� �����Ѵ�
        timeControl = FindObjectOfType<TimeControl>();

        // ���콺 Ŀ���� ����� ����
        Cursor.lockState = CursorLockMode.Locked;
        // �ʱ� Y ��ġ�� ����
        lastYPosition = transform.position.y;
    }

    void Update()
    {
        // ȸ�� ó��
        HandleRotation();
        // �̵� ó��
        HandleMovement();
        // ������ �߷� ó��
        HandleJumpAndGravity();

        // �ð� ���� ������Ʈ
        UpdateTimeControl();

        getPistol(); // �÷��̾ ���� ��´�.
    }

    void HandleRotation()
    {
        // ���콺 �Է��� �޴´�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // �¿� ȸ��
        transform.Rotate(Vector3.up * mouseX);

        // ���� ȸ�� (ī�޶� ����)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // ����� ���� �Է°��� �޴´�
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // �Է°��� ����Ͽ� �̵� ������ ����
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        // CharacterController�� ����Ͽ� �̵����� ó��
        controller.Move(move * moveSpeed * Time.deltaTime);

        // �̵� ���� ������Ʈ
        isMoving = (moveX != 0 || moveZ != 0);
    }

    void HandleJumpAndGravity()
    {
        // ���� ó��
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isMoving = true;
        }

        // �߷� ����
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ���� �̵� Ȯ��
        float currentYPosition = transform.position.y;
        if (Mathf.Abs(currentYPosition - lastYPosition) > 1f)
        {
            isMoving = true;
        }
        lastYPosition = currentYPosition;
    }

    void UpdateTimeControl()
    {
        // TimeControl ��ũ��Ʈ�� �÷��̾� �̵� ���� ����
        if (timeControl != null)
        {
            timeControl.UpdateTimeScale(isMoving);
        }
    }
    void getPistol()
    {
        if (ThrownEnemyPistol != null && PlayerPistol != null)
        {
            // 1. �ٴڿ� �ִ� ThrownEnemyPistol�� ���� �Ÿ� ���� �ִ��� Ȯ��
            float distanceToPistol = Vector3.Distance(transform.position, ThrownEnemyPistol.transform.position);

            if (distanceToPistol <= pickupRange)
            {
                // 2. PlayerPistol�� ��Ȱ��ȭ�Ǿ� �ִ� �������� Ȯ��
                if (!PlayerPistol.activeInHierarchy)
                {
                    // 3. ���콺 ��Ŭ���� �ߴ��� Ȯ��
                    if (Input.GetMouseButtonDown(0))
                    {
                        // ThrownEnemyPistol�� ��Ȱ��ȭ�ϰ�, PlayerPistol�� Ȱ��ȭ
                        ThrownEnemyPistol.SetActive(false);
                        PlayerPistol.SetActive(true);
                        print("Pistol picked up and activated");
                    }
                }
            }
        }
    }
    public void UpdateThrownEnemyPistol(GameObject thrownPistol)
    {
        ThrownEnemyPistol = thrownPistol;
    }
}