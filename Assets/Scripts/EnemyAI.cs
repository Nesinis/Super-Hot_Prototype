using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // 플레이어의 위치를 저장할 변수
    private NavMeshAgent agent; // NavMeshAgent 컴포넌트를 참조할 변수
    public float minAttackRange = 5.0f; // 최소 공격 거리
    public float attackRange = 10.0f; // 최대 공격 거리
    public float attackCooldown = 2.0f; // 공격 쿨다운 시간
    private float lastAttackTime; // 마지막 공격 시간을 저장할 변수

    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint; // 총알 발사 위치
    public float bulletSpeed = 10.0f; // 총알 속도, 적의 이동 속도보다 빠르게 설정

    private Animator animator; // 애니메이터 컴포넌트를 참조할 변수
    private bool isDead = false; // 적이 죽었는지 여부를 확인하는 변수
    private bool playerIsDead = false; // 플레이어가 죽었는지 여부를 확인하는 변수
    private bool isAttacking = false; // 적이 공격 중인지 여부를 확인하는 변수

    public float attackWaitTime = 1.0f; // 공격 후 추가 대기 시간
    public float shootDelay = 0.1f; // 총알 발사 딜레이 (애니메이션이 시작된 후)
    void Start()
    {
        // NavMeshAgent 컴포넌트를 가져옵니다.
        agent = GetComponent<NavMeshAgent>();
        // Animator 컴포넌트를 가져옵니다.
        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown; // 시작 시 바로 공격할 수 있도록 설정

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

        // 플레이어의 위치로 NavMeshAgent의 목적지를 설정합니다.
        if (player != null)
        {
            //agent.SetDestination(player.position);

            // 애니메이션 업데이트
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed); // 지속적으로 Speed 값을 업데이트

            //Debug.Log("Current speed: " + speed);
            //Debug.Log("Is Walking State: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"));

            // 플레이어와의 거리를 계산합니다.
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 공격 범위 내에 플레이어가 있는지 확인합니다.
            if (distanceToPlayer >= minAttackRange && distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                // 공격 메커니즘을 호출합니다.
                AttackPlayer();
                lastAttackTime = Time.time; // 마지막 공격 시간을 갱신합니다.
            }
        }
    }

    void AttackPlayer()
    {
        // 공격 중 상태로 전환합니다.
        isAttacking = true;
        agent.isStopped = true; // 이동을 멈춥니다.

        // 공격 애니메이션을 재생합니다.
        animator.SetTrigger("Attack");

        // 총알을 발사하는 코루틴을 시작합니다.
        StartCoroutine(ShootAfterDelay());

        // 공격 애니메이션이 끝난 후 이동을 재개합니다.
        StartCoroutine(ResumeMovementAfterAttack());
    }

    public Material brightRedMaterial; // Inspector에서 설정된 밝은 빨간색 Material

    public void Shoot()
    {
        // 총알을 생성하고 발사합니다.
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            // 플레이어의 몸 중심을 향하도록 총알의 방향을 설정합니다.
            Vector3 targetPosition = player.position; // 플레이어의 중심을 목표로 설정
            Vector3 direction = (targetPosition - firePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.useGravity = false; // 중력 사용 비활성화
                rb.velocity = direction * bulletSpeed; // 총알의 초기 속도를 설정

                // Trail Renderer 추가 및 설정
                if (brightRedMaterial != null)
                {
                    TrailRenderer trail = bullet.AddComponent<TrailRenderer>();
                    trail.time = 0.3f; // 궤적의 지속 시간
                    trail.startWidth = 0.05f; // 궤적의 시작 폭
                    trail.endWidth = 0.01f;   // 궤적의 끝 폭
                }
            }
            else
            {
                //Debug.LogWarning("Bullet 프리팹에 Rigidbody 컴포넌트가 없습니다.");
            }
        }
        else
        {
            //Debug.LogWarning("BulletPrefab, FirePoint 또는 Player가 할당되지 않았습니다.");
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
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 총알인지 확인합니다.
        if (other.gameObject.CompareTag("Bullet"))
        {
            // 총알과 충돌했으므로 적을 제거합니다.
            Destroy(gameObject);

            // 총알도 제거합니다.
            Destroy(other.gameObject);
        }
    }
}
