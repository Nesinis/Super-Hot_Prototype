using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // �÷��̾��� ��ġ�� ������ ����
    private NavMeshAgent agent; // NavMeshAgent ������Ʈ�� ������ ����
    public float minAttackRange = 5.0f; // �ּ� ���� �Ÿ�
    public float attackRange = 10.0f; // �ִ� ���� �Ÿ�
    public float attackCooldown = 2.0f; // ���� ��ٿ� �ð�
    private float lastAttackTime; // ������ ���� �ð��� ������ ����

    public GameObject bulletPrefab; // �Ѿ� ������
    public Transform firePoint; // �Ѿ� �߻� ��ġ
    public float bulletSpeed = 10.0f; // �Ѿ� �ӵ�, ���� �̵� �ӵ����� ������ ����

    private Animator animator; // �ִϸ����� ������Ʈ�� ������ ����
    private bool isDead = false; // ���� �׾����� ���θ� Ȯ���ϴ� ����
    private bool playerIsDead = false; // �÷��̾ �׾����� ���θ� Ȯ���ϴ� ����
    private bool isAttacking = false; // ���� ���� ������ ���θ� Ȯ���ϴ� ����

    public float attackWaitTime = 1.0f; // ���� �� �߰� ��� �ð�
    public float shootDelay = 0.1f; // �Ѿ� �߻� ������ (�ִϸ��̼��� ���۵� ��)
    void Start()
    {
        // NavMeshAgent ������Ʈ�� �����ɴϴ�.
        agent = GetComponent<NavMeshAgent>();
        // Animator ������Ʈ�� �����ɴϴ�.
        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown; // ���� �� �ٷ� ������ �� �ֵ��� ����

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

        // �÷��̾��� ��ġ�� NavMeshAgent�� �������� �����մϴ�.
        if (player != null)
        {
            //agent.SetDestination(player.position);

            // �ִϸ��̼� ������Ʈ
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed); // ���������� Speed ���� ������Ʈ

            //Debug.Log("Current speed: " + speed);
            //Debug.Log("Is Walking State: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"));

            // �÷��̾���� �Ÿ��� ����մϴ�.
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // ���� ���� ���� �÷��̾ �ִ��� Ȯ���մϴ�.
            if (distanceToPlayer >= minAttackRange && distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                // ���� ��Ŀ������ ȣ���մϴ�.
                AttackPlayer();
                lastAttackTime = Time.time; // ������ ���� �ð��� �����մϴ�.
            }
        }
    }

    void AttackPlayer()
    {
        // ���� �� ���·� ��ȯ�մϴ�.
        isAttacking = true;
        agent.isStopped = true; // �̵��� ����ϴ�.

        // ���� �ִϸ��̼��� ����մϴ�.
        animator.SetTrigger("Attack");

        // �Ѿ��� �߻��ϴ� �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(ShootAfterDelay());

        // ���� �ִϸ��̼��� ���� �� �̵��� �簳�մϴ�.
        StartCoroutine(ResumeMovementAfterAttack());
    }

    public Material brightRedMaterial; // Inspector���� ������ ���� ������ Material

    public void Shoot()
    {
        // �Ѿ��� �����ϰ� �߻��մϴ�.
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            // �÷��̾��� �� �߽��� ���ϵ��� �Ѿ��� ������ �����մϴ�.
            Vector3 targetPosition = player.position; // �÷��̾��� �߽��� ��ǥ�� ����
            Vector3 direction = (targetPosition - firePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.useGravity = false; // �߷� ��� ��Ȱ��ȭ
                rb.velocity = direction * bulletSpeed; // �Ѿ��� �ʱ� �ӵ��� ����

                // Trail Renderer �߰� �� ����
                if (brightRedMaterial != null)
                {
                    TrailRenderer trail = bullet.AddComponent<TrailRenderer>();
                    trail.time = 0.3f; // ������ ���� �ð�
                    trail.startWidth = 0.05f; // ������ ���� ��
                    trail.endWidth = 0.01f;   // ������ �� ��
                }
            }
            else
            {
                //Debug.LogWarning("Bullet �����տ� Rigidbody ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            //Debug.LogWarning("BulletPrefab, FirePoint �Ǵ� Player�� �Ҵ���� �ʾҽ��ϴ�.");
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
    void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� �Ѿ����� Ȯ���մϴ�.
        if (other.gameObject.CompareTag("Bullet"))
        {
            // �Ѿ˰� �浹�����Ƿ� ���� �����մϴ�.
            Destroy(gameObject);

            // �Ѿ˵� �����մϴ�.
            Destroy(other.gameObject);
        }
    }
}
