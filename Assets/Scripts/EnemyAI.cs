using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // 플레이어의 위치를 저장할 변수
    private NavMeshAgent agent; // NavMeshAgent 컴포넌트를 참조할 변수
    public float minAttackRange = 0.5f; // 근접 공격 거리
    public float attackRange = 10.0f; // 원거리 공격 거리
    public float attackCooldown = 2.0f; // 공격 쿨다운 시간
    private float lastAttackTime; // 마지막 공격 시간을 저장할 변수
    public GameObject pistol2; // 총 오브젝트 참조

    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint; // 총알 발사 위치
    public float bulletSpeed = 10.0f; // 총알 속도

    private Animator animator; // 애니메이터 컴포넌트를 참조할 변수
    private bool isDead = false; // 적이 죽었는지 여부를 확인하는 변수
    private bool playerIsDead = false; // 플레이어가 죽었는지 여부를 확인하는 변수
    private bool isAttacking = false; // 적이 공격 중인지 여부를 확인하는 변수

    public Material brightRedMaterial; // 밝은 빨간색 Material
    public bool hasGun = false; // 적이 총을 들고 있는지 여부를 나타내는 변수

    public float attackWaitTime = 1.0f; // 공격 후 추가 대기 시간
    public float shootDelay = 0.1f; // 총알 발사 딜레이 (애니메이션이 시작된 후)

    void Start()
    {
        // NavMeshAgent 컴포넌트를 가져옵니다.
        agent = GetComponent<NavMeshAgent>();
        // Animator 컴포넌트를 가져옵니다.
        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown; // 시작 시 바로 공격할 수 있도록 설정

        CheckGunPresence();

        // 플레이어 사망 이벤트 구독
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnPlayerDeath.AddListener(HandlePlayerDeath);
            }
        }
    }

    void Update()
    {
        if (isDead || playerIsDead)
            return; // 적이나 플레이어가 죽었으면 업데이트를 중지합니다.

        if (isAttacking)
            return; // 공격 중이면 이동을 멈춥니다.

        // 적이 플레이어를 계속 바라보게 합니다.
        LookAtPlayer();

        CheckGunPresence();
        if (player != null)
        {
            // 플레이어의 위치로 NavMeshAgent의 목적지를 설정합니다.
            agent.SetDestination(player.position);

            // 애니메이션 업데이트
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed); // 지속적으로 Speed 값을 업데이트

            // 플레이어와의 거리를 계산합니다.
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 공격 범위 내에 플레이어가 있는지 확인합니다.
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (hasGun && distanceToPlayer <= attackRange)
                {
                    // 총을 들고 있고 범위 내에 있으면 총으로 공격
                    AttackWithGun();
                }
                else if (!hasGun && distanceToPlayer <= minAttackRange)
                {
                    // 총이 없고 근접 범위 내에 있으면 주먹으로 공격
                    AttackWithMelee();
                }
            }
            if (speed > 0.1f)
            {
                Debug.Log("Transition to Unarmed_Walk should occur.");
            }
        }
        Debug.Log("HasGun: " + hasGun);
    }

    void AttackWithMelee()
    {
        // 주먹 공격 중 상태로 전환합니다.
        isAttacking = true;
        agent.isStopped = true; // 이동을 멈춥니다.

        // 주먹 공격 애니메이션을 재생합니다.
        animator.SetTrigger("Punch");

        // 공격 애니메이션이 끝난 후 이동을 재개합니다.
        StartCoroutine(ResumeMovementAfterAttack());
    }

    void AttackWithGun()
    {
        // 총 공격 중 상태로 전환합니다.
        isAttacking = true;
        agent.isStopped = true; // 이동을 멈춥니다.

        // 총 공격 애니메이션을 재생합니다.
        animator.SetTrigger("Shoot");

        // 총알을 발사하는 코루틴을 시작합니다.
        StartCoroutine(ShootAfterDelay());

        // 공격 애니메이션이 끝난 후 이동을 재개합니다.
        StartCoroutine(ResumeMovementAfterAttack());
    }

    public void Shoot()
    {
        // 총알을 생성하고 발사합니다.
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            Vector3 targetPosition = player.position; // 플레이어의 중심을 목표로 설정
            Vector3 direction = (targetPosition - firePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.useGravity = false; // 중력 사용 비활성화
                rb.velocity = direction * bulletSpeed; // 총알의 초기 속도를 설정

                // 이미 있는 Trail Renderer 설정
                TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    trail.material = brightRedMaterial;  // 밝은 빨간색 Material 적용
                    trail.time = 0.3f; // 궤적의 지속 시간
                    trail.startWidth = 0.05f; // 궤적의 시작 폭
                    trail.endWidth = 0.01f;   // 궤적의 끝 폭
                }
                else
                {
                    Debug.LogWarning("Trail Renderer 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("Bullet 프리팹에 Rigidbody 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("BulletPrefab, FirePoint 또는 Player가 할당되지 않았습니다.");
        }
    }

    System.Collections.IEnumerator ShootAfterDelay()
    {
        // 공격 애니메이션이 시작된 후 잠시 대기합니다.
        yield return new WaitForSeconds(shootDelay);

        // 플레이어를 조준합니다.
        if (player != null)
        {
            // 적이 플레이어를 향하도록 회전시킵니다.
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }

        // 총알을 발사합니다.
        Shoot();
    }

    System.Collections.IEnumerator ResumeMovementAfterAttack()
    {
        // 공격 애니메이션이 끝날 때까지 대기합니다.
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // 추가 대기 시간을 기다립니다.
        yield return new WaitForSeconds(attackWaitTime);

        // 공격이 끝나면 이동을 재개합니다.
        isAttacking = false;
        agent.isStopped = false;
    }

    public void TakeDamage()
    {
        if (!isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        agent.enabled = false; // NavMeshAgent를 비활성화합니다.
    }

    void HandlePlayerDeath()
    {
        playerIsDead = true;
        agent.isStopped = true; // 적의 움직임을 멈춥니다.
        animator.SetFloat("Speed", 0); // 이동 애니메이션을 멈춥니다.
    }

    void CheckGunPresence()
    {
        hasGun = pistol2.activeInHierarchy; // 총의 존재 여부를 확인
        animator.SetBool("HasGun", hasGun); // 애니메이터 상태 업데이트
        Debug.Log("HasGun 상태: " + hasGun);
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // 수평 회전만 고려하고, 적의 머리가 플레이어를 직접 쳐다보지 않도록 Y 축은 0으로 설정합니다.
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f); // 부드러운 회전을 위해 Slerp를 사용합니다.
    }
}
