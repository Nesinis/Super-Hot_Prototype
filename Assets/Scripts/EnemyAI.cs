using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // �÷��̾��� ��ġ�� ������ ����
    private NavMeshAgent agent; // NavMeshAgent ������Ʈ�� ������ ����
    public float minAttackRange = 0.5f; // ���� ���� �Ÿ�
    public float attackRange = 10.0f; // ���Ÿ� ���� �Ÿ�
    public float attackCooldown = 2.0f; // ���� ��ٿ� �ð�
    private float lastAttackTime; // ������ ���� �ð��� ������ ����
    public GameObject pistol2; // �� ������Ʈ ����

    public GameObject bulletPrefab; // �Ѿ� ������
    public Transform firePoint; // �Ѿ� �߻� ��ġ
    public float bulletSpeed = 10.0f; // �Ѿ� �ӵ�

    private Animator animator; // �ִϸ����� ������Ʈ�� ������ ����
    private bool isDead = false; // ���� �׾����� ���θ� Ȯ���ϴ� ����
    private bool playerIsDead = false; // �÷��̾ �׾����� ���θ� Ȯ���ϴ� ����
    private bool isAttacking = false; // ���� ���� ������ ���θ� Ȯ���ϴ� ����

    public Material brightRedMaterial; // ���� ������ Material
    public bool hasGun = false; // ���� ���� ��� �ִ��� ���θ� ��Ÿ���� ����

    public float attackWaitTime = 1.0f; // ���� �� �߰� ��� �ð�
    public float shootDelay = 0.1f; // �Ѿ� �߻� ������ (�ִϸ��̼��� ���۵� ��)

    void Start()
    {
        // NavMeshAgent ������Ʈ�� �����ɴϴ�.
        agent = GetComponent<NavMeshAgent>();
        // Animator ������Ʈ�� �����ɴϴ�.
        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown; // ���� �� �ٷ� ������ �� �ֵ��� ����

        CheckGunPresence();

        // �÷��̾� ��� �̺�Ʈ ����
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
            return; // ���̳� �÷��̾ �׾����� ������Ʈ�� �����մϴ�.

        if (isAttacking)
            return; // ���� ���̸� �̵��� ����ϴ�.

        // ���� �÷��̾ ��� �ٶ󺸰� �մϴ�.
        LookAtPlayer();

        CheckGunPresence();
        if (player != null)
        {
            // �÷��̾��� ��ġ�� NavMeshAgent�� �������� �����մϴ�.
            agent.SetDestination(player.position);

            // �ִϸ��̼� ������Ʈ
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed); // ���������� Speed ���� ������Ʈ

            // �÷��̾���� �Ÿ��� ����մϴ�.
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // ���� ���� ���� �÷��̾ �ִ��� Ȯ���մϴ�.
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (hasGun && distanceToPlayer <= attackRange)
                {
                    // ���� ��� �ְ� ���� ���� ������ ������ ����
                    AttackWithGun();
                }
                else if (!hasGun && distanceToPlayer <= minAttackRange)
                {
                    // ���� ���� ���� ���� ���� ������ �ָ����� ����
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
        // �ָ� ���� �� ���·� ��ȯ�մϴ�.
        isAttacking = true;
        agent.isStopped = true; // �̵��� ����ϴ�.

        // �ָ� ���� �ִϸ��̼��� ����մϴ�.
        animator.SetTrigger("Punch");

        // ���� �ִϸ��̼��� ���� �� �̵��� �簳�մϴ�.
        StartCoroutine(ResumeMovementAfterAttack());
    }

    void AttackWithGun()
    {
        // �� ���� �� ���·� ��ȯ�մϴ�.
        isAttacking = true;
        agent.isStopped = true; // �̵��� ����ϴ�.

        // �� ���� �ִϸ��̼��� ����մϴ�.
        animator.SetTrigger("Shoot");

        // �Ѿ��� �߻��ϴ� �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(ShootAfterDelay());

        // ���� �ִϸ��̼��� ���� �� �̵��� �簳�մϴ�.
        StartCoroutine(ResumeMovementAfterAttack());
    }

    public void Shoot()
    {
        // �Ѿ��� �����ϰ� �߻��մϴ�.
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            Vector3 targetPosition = player.position; // �÷��̾��� �߽��� ��ǥ�� ����
            Vector3 direction = (targetPosition - firePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.useGravity = false; // �߷� ��� ��Ȱ��ȭ
                rb.velocity = direction * bulletSpeed; // �Ѿ��� �ʱ� �ӵ��� ����

                // �̹� �ִ� Trail Renderer ����
                TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    trail.material = brightRedMaterial;  // ���� ������ Material ����
                    trail.time = 0.3f; // ������ ���� �ð�
                    trail.startWidth = 0.05f; // ������ ���� ��
                    trail.endWidth = 0.01f;   // ������ �� ��
                }
                else
                {
                    Debug.LogWarning("Trail Renderer ������Ʈ�� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogWarning("Bullet �����տ� Rigidbody ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("BulletPrefab, FirePoint �Ǵ� Player�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    System.Collections.IEnumerator ShootAfterDelay()
    {
        // ���� �ִϸ��̼��� ���۵� �� ��� ����մϴ�.
        yield return new WaitForSeconds(shootDelay);

        // �÷��̾ �����մϴ�.
        if (player != null)
        {
            // ���� �÷��̾ ���ϵ��� ȸ����ŵ�ϴ�.
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }

        // �Ѿ��� �߻��մϴ�.
        Shoot();
    }

    System.Collections.IEnumerator ResumeMovementAfterAttack()
    {
        // ���� �ִϸ��̼��� ���� ������ ����մϴ�.
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // �߰� ��� �ð��� ��ٸ��ϴ�.
        yield return new WaitForSeconds(attackWaitTime);

        // ������ ������ �̵��� �簳�մϴ�.
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
        agent.enabled = false; // NavMeshAgent�� ��Ȱ��ȭ�մϴ�.
    }

    void HandlePlayerDeath()
    {
        playerIsDead = true;
        agent.isStopped = true; // ���� �������� ����ϴ�.
        animator.SetFloat("Speed", 0); // �̵� �ִϸ��̼��� ����ϴ�.
    }

    void CheckGunPresence()
    {
        hasGun = pistol2.activeInHierarchy; // ���� ���� ���θ� Ȯ��
        animator.SetBool("HasGun", hasGun); // �ִϸ����� ���� ������Ʈ
        Debug.Log("HasGun ����: " + hasGun);
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // ���� ȸ���� ����ϰ�, ���� �Ӹ��� �÷��̾ ���� �Ĵٺ��� �ʵ��� Y ���� 0���� �����մϴ�.
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f); // �ε巯�� ȸ���� ���� Slerp�� ����մϴ�.
    }
}
