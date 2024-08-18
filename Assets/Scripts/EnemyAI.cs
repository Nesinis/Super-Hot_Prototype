using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

    public GameObject thrownEnemyPistol;
    public GameObject throwRotaion;
    public float throwPower = 3f;

    public GameObject explosionParticlePrefab; // 폭죽 파티클 프리팹

    private AudioSource audioSource; // AudioSource 컴포넌트
    public AudioClip deathSound; // 적 사망 사운드
    private bool isStunned = false; // 적이 경직 상태인지 여부를 확인하는 변수

    public int punchHealth = 3; // 적의 펀치 공격에 대한 체력 변수

    public AudioClip gunshotSound; // 총알 발사 소리

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
        lastAttackTime = -attackCooldown; // 시작 시 바로 공격할 수 있도록 설정

        CheckGunPresence(); // 총의 존재 여부를 확인하여 애니메이터 초기 상태 설정

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
        if (isDead || playerIsDead || isStunned)
            return; // 적이나 플레이어가 죽었거나, 스턴 상태이면 업데이트를 중지합니다.

        if (isAttacking)
            return; // 공격 중이면 이동을 멈춥니다.

        LookAtPlayer();

        CheckGunPresence(); // 매 프레임마다 총의 존재 여부를 확인
        if (player != null)
        {
            agent.SetDestination(player.position);

            // 애니메이션 업데이트
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed); // 지속적으로 Speed 값을 업데이트

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (hasGun && distanceToPlayer <= attackRange)
                {
                    StartCoroutine(AttackWithGun());
                }
                else if (!hasGun && distanceToPlayer <= minAttackRange)
                {
                    StartCoroutine(AttackWithMelee());
                }
            }
        }
    }

    IEnumerator AttackWithMelee()
    {
        isAttacking = true;
        agent.isStopped = true; // 이동을 멈춥니다.

        // 근접 공격 로직이 들어갈 수 있습니다.
        animator.SetTrigger("MeleeAttack"); // 근접 공격 애니메이션 트리거

        yield return new WaitForSeconds(attackWaitTime);

        isAttacking = false;
        agent.isStopped = false; // 공격 후 다시 이동 시작
    }

    IEnumerator AttackWithGun()
    {
        isAttacking = true;
        agent.isStopped = true; // 이동을 멈춥니다.

        animator.SetTrigger("Shoot"); // 총격 애니메이션 트리거

        yield return StartCoroutine(ShootAfterDelay());

        yield return new WaitForSeconds(attackWaitTime);

        isAttacking = false;
        agent.isStopped = false; // 공격 후 다시 이동 시작
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            // gunshotSound가 할당되었는지 확인합니다.
            if (gunshotSound != null)
            {
                // 발사 소리 재생
                audioSource.PlayOneShot(gunshotSound);
            }
            else
            {
                Debug.LogWarning("gunshotSound is not assigned in " + gameObject.name);
            }

            Vector3 targetPosition = player.position;
            Vector3 direction = (targetPosition - firePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.useGravity = false;
                rb.velocity = direction * bulletSpeed;

                TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    trail.material = brightRedMaterial;
                    trail.time = 0.3f;
                    trail.startWidth = 0.05f;
                    trail.endWidth = 0.01f;
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

    IEnumerator ShootAfterDelay()
    {
        yield return new WaitForSeconds(shootDelay);

        if (player != null)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }

        Shoot();
    }

    public void Die()
    {
        if (!isDead)  // 사망 상태가 아닌 경우에만 처리
        {
            isDead = true;
            agent.enabled = false; // NavMeshAgent를 비활성화합니다.

            // 사망 사운드 재생
            if (audioSource != null && deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }

            // 폭죽 파티클 효과 생성
            if (explosionParticlePrefab != null)
            {
                GameObject explosion = Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);

                ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();

                    // 파티클의 수명에 맞추어 파티클 오브젝트 제거
                    Destroy(explosion, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }

            // 즉시 적 객체를 제거
            Destroy(gameObject); // 적 객체를 즉시 제거
        }
    }

    void HandlePlayerDeath()
    {
        playerIsDead = true;
        agent.isStopped = true; // 적의 움직임을 멈춥니다.
        animator.SetFloat("Speed", 0); // 이동 애니메이션을 멈춥니다.
    }

    public void CheckGunPresence()
    {
        if (pistol2 != null)
        {
            hasGun = pistol2.activeInHierarchy; // 총의 존재 여부를 확인
            animator.SetBool("HasGun", hasGun); // 애니메이터 상태 업데이트
        }
        else
        {
            hasGun = false; // pistol2가 null이면 총이 없는 상태로 설정
            animator.SetBool("HasGun", hasGun); // 애니메이터 상태 업데이트
        }
    }

    void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Y 축 회전을 방지

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ThrownPlayerPistol") || other.CompareTag("Player"))
        {
            // 피스톨 던지기 또는 근접 공격에 맞았을 때 스턴
            StartCoroutine(Stun());
        }

        if (other.gameObject.name.Contains("ThrownPlayerPistol"))
        {
            throwEnemyPistol();
        }
    }

    public void throwEnemyPistol()
    {
        if (pistol2 != null)
        {
            Destroy(pistol2);
            pistol2 = null; // 참조 초기화
            GameObject goThrownEnemyPistol = Instantiate(thrownEnemyPistol, throwRotaion.transform.position, throwRotaion.transform.rotation);
            Rigidbody rb = goThrownEnemyPistol.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 throwDir = (throwRotaion.transform.forward + throwRotaion.transform.up).normalized;
                rb.AddForce(throwDir * throwPower, ForceMode.Impulse);
            }

            // PlayerMovement에 있는 ThrownEnemyPistol 변수를 업데이트합니다.
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.UpdateThrownEnemyPistol(goThrownEnemyPistol);
            }

            // 피스톨을 던진 후 총의 존재 여부를 확인하여 상태 업데이트
            CheckGunPresence();
        }
    }

    public IEnumerator Stun()
    {
        Debug.Log(gameObject.name + " is now stunned.");  // 스턴 시작 디버그 메시지

        // 현재 객체가 파괴되었는지 확인
        if (this == null)
        {
            Debug.LogWarning("EnemyAI object has been destroyed. Exiting Stun coroutine.");
            yield break;  // 코루틴 종료
        }

        // NavMeshAgent가 유효하고 활성화된 경우에만 이동 중지를 설정합니다.
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;  // NavMeshAgent 이동 중지
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not active or not placed on a NavMesh. Cannot stop agent.");
        }

        animator.SetTrigger("Stun"); // 경직 애니메이션 트리거

        // 피스톨 떨어뜨리기
        throwEnemyPistol();

        yield return new WaitForSeconds(1.0f);  // 1초간 대기

        // 현재 객체가 파괴되었는지 확인
        if (this == null)
        {
            Debug.LogWarning("EnemyAI object has been destroyed during stun. Exiting coroutine.");
            yield break;  // 코루틴 종료
        }

        // NavMeshAgent가 유효하고 활성화된 경우에만 이동 재개를 설정합니다.
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;  // 이동 재개
        }

        Debug.Log(gameObject.name + " has recovered from stun.");  // 스턴 해제 디버그 메시지
    }

    public void TakePunchDamage()
    {
        punchHealth--; // 펀치 공격에 맞으면 체력을 감소시킵니다.
        Debug.Log(gameObject.name + " takes punch damage. Remaining punch health: " + punchHealth);

        if (punchHealth <= 0)
        {
            Die(); // 체력이 0 이하가 되면 사망 처리
        }
    }
}
