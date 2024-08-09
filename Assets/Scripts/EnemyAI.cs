using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

    public GameObject thrownEnemyPistol;
    public GameObject throwRotaion;
    public float throwPower = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown; // ���� �� �ٷ� ������ �� �ֵ��� ����

        CheckGunPresence();

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

        LookAtPlayer();

        CheckGunPresence();
        if (player != null)
        {
            agent.SetDestination(player.position);

            // �ִϸ��̼� ������Ʈ
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed); // ���������� Speed ���� ������Ʈ

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (hasGun && distanceToPlayer <= attackRange)
                {
                    AttackWithGun();
                }
                else if (!hasGun && distanceToPlayer <= minAttackRange)
                {
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
        isAttacking = true;
        agent.isStopped = true; // �̵��� ����ϴ�.

        StartCoroutine(ResumeMovementAfterAttack());
    }

    void AttackWithGun()
    {
        isAttacking = true;
        agent.isStopped = true; // �̵��� ����ϴ�.

        StartCoroutine(ShootAfterDelay());
        StartCoroutine(ResumeMovementAfterAttack());
    }

    public void Shoot()
    {
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
        yield return new WaitForSeconds(shootDelay);

        if (player != null)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }

        Shoot();
    }

    System.Collections.IEnumerator ResumeMovementAfterAttack()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(attackWaitTime);

        isAttacking = false;
        agent.isStopped = false;
    }

    public void TakeDamage()
    {
        if (!isDead)
        {
            StartCoroutine(Stun());
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
        if (pistol2 != null)
        {
            hasGun = pistol2.activeInHierarchy; // ���� ���� ���θ� Ȯ��
            animator.SetBool("HasGun", hasGun); // �ִϸ����� ���� ������Ʈ
            Debug.Log("HasGun ����: " + hasGun);
        }
        else
        {
            hasGun = false; // pistol2�� null�̸� ���� ���� ���·� ����
            animator.SetBool("HasGun", hasGun); // �ִϸ����� ���� ������Ʈ
        }
    }

    void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
            pistol2 = null; // ���� �ʱ�ȭ
            GameObject goThrownEnemyPistol = Instantiate(thrownEnemyPistol, throwRotaion.transform.position, throwRotaion.transform.rotation);
            Rigidbody rb = goThrownEnemyPistol.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 throwDir = (throwRotaion.transform.forward + throwRotaion.transform.up * 0.3f).normalized;
                rb.AddForce(throwDir * throwPower, ForceMode.Impulse);
            }
        }
    }

    public IEnumerator Stun()
    {
        Debug.Log(gameObject.name + " is now stunned.");  // ���� ���� ����� �޽���
        agent.isStopped = true;  // NavMeshAgent �̵� ����
        yield return new WaitForSeconds(1.0f);  // 1�ʰ� ���
        agent.isStopped = false;  // �̵� �簳
        Debug.Log(gameObject.name + " has recovered from stun.");  // ���� ���� ����� �޽���
    }
}
