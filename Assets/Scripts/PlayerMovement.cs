using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 velocity;

    private TimeControl timeControl;

    private float verticalRotation = 0f;
    private bool isJumping = false;

    public GameObject PlayerPistol;
    private GameObject ThrownEnemyPistol;
    public float pickupRange = 2.0f;

    public Transform punchOrigin;
    public float punchRange = 1.0f;

    public GameObject deathCameraPrefab;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        timeControl = FindObjectOfType<TimeControl>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleJumpAndGravity();
        UpdateTimeControl();

        getPistol();
        HandleAttack();

        // R 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key pressed - Restarting scene");
            SceneManager.LoadScene("JumpScene"); // "JumpScene"은 재시작할 씬의 이름입니다.
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = transform.right * moveX + transform.forward * moveZ;

        if (moveDirection.magnitude > 0)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    void HandleJumpAndGravity()
    {
        if (!isJumping && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, velocity.y, rb.velocity.z);
            isJumping = true;
        }

        if (isJumping)
        {
            velocity.y += gravity * Time.deltaTime;
            rb.velocity = new Vector3(rb.velocity.x, velocity.y, rb.velocity.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }


    void UpdateTimeControl()
    {
        if (timeControl != null)
        {
            bool currentMovingState = rb.velocity.magnitude > 0;
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

                        yield return new WaitForSeconds(0.05f);

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
            if (Input.GetButtonDown("Fire1"))
            {
                // 총 발사!
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Punch();
            }
        }
    }

    void getPistol()
    {
        StartCoroutine(getPistolCoroutine());
    }

    void Punch()
    {
        RaycastHit hit;
        if (Physics.Raycast(punchOrigin.position, punchOrigin.forward, out hit, punchRange))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakePunchDamage();
            }
        }
    }

    public void UpdateThrownEnemyPistol(GameObject thrownPistol)
    {
        ThrownEnemyPistol = thrownPistol;
    }

    public void OnDeath()
    {
        Vector3 deathPosition = transform.position;
        Quaternion deathRotation = transform.rotation;

        GameObject deathCamera = Instantiate(deathCameraPrefab, deathPosition, deathRotation);

        deathCamera.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}
