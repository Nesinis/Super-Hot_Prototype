using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public float minAttackRange = 0.5f;
    public float attackRange = 10.0f;
    public float attackCooldown = 2.0f;
    private float lastAttackTime;
    public GameObject pistol2;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10.0f;

    private Animator animator;
    private bool isDead = false;
    private bool playerIsDead = false;
    private bool isAttacking = false;
    private bool isStunned = false;

    public Material brightRedMaterial;
    public bool hasGun = false;

    public float attackWaitTime = 1.0f;
    public float shootDelay = 0.1f;

    public GameObject thrownEnemyPistol;
    public GameObject throwRotaion;
    public float throwPower = 3f;

    public GameObject explosionParticlePrefab;

    private AudioSource audioSource;
    public AudioClip deathSound;

    private EnemyManager enemyManager;

    public int health = 3;

    private List<Vector3> recordedPositions = new List<Vector3>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing from this game object.");
        }

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        lastAttackTime = -attackCooldown;

        CheckGunPresence();

        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnPlayerDeath.AddListener(HandlePlayerDeath);
            }
        }

        enemyManager = FindObjectOfType<EnemyManager>();
        RecordPosition();
    }

    void Update()
    {
        if (isDead || playerIsDead || isStunned)
            return;

        if (isAttacking)
            return;

        CheckGunPresence();
        if (player != null)
        {
            agent.SetDestination(player.position);

            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);

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
        }
    }

    void AttackWithMelee()
    {
        isAttacking = true;
        agent.isStopped = true;

        StartCoroutine(ResumeMovementAfterAttack());
    }

    void AttackWithGun()
    {
        isAttacking = true;
        agent.isStopped = true;

        StartCoroutine(ShootAfterDelay());
        StartCoroutine(ResumeMovementAfterAttack());
    }

    public void Shoot()
    {
        if (isDead)
        {
            return;
        }

        if (bulletPrefab != null && firePoint != null && player != null)
        {
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

    IEnumerator ResumeMovementAfterAttack()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(attackWaitTime);

        isAttacking = false;
        agent.isStopped = false;
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            agent.enabled = false;

            if (enemyManager != null)
            {
                enemyManager.OnEnemyDeath(this); // 수정된 부분
            }

            if (audioSource != null && deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }

            gameObject.SetActive(false);
        }
    }

    void HandlePlayerDeath()
    {
        playerIsDead = true;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
    }

    void CheckGunPresence()
    {
        if (pistol2 != null)
        {
            hasGun = pistol2.activeInHierarchy;
            animator.SetBool("HasGun", hasGun);
        }
        else
        {
            hasGun = false;
            animator.SetBool("HasGun", hasGun);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }

    public void throwEnemyPistol()
    {
        if (pistol2 != null)
        {
            Destroy(pistol2);
            pistol2 = null;
            GameObject goThrownEnemyPistol = Instantiate(thrownEnemyPistol, throwRotaion.transform.position, throwRotaion.transform.rotation);
            Rigidbody rb = goThrownEnemyPistol.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 throwDir = (throwRotaion.transform.forward + throwRotaion.transform.up * 0.3f).normalized;
                rb.AddForce(throwDir * throwPower, ForceMode.Impulse);
            }

            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.UpdateThrownEnemyPistol(goThrownEnemyPistol);
            }

            CheckGunPresence();
        }
    }

    public IEnumerator Stun()
    {
        if (isStunned || isDead)
        {
            yield break;
        }

        isStunned = true;

        if (agent != null)
        {
            agent.isStopped = true;
        }

        if (animator != null)
        {
            animator.SetTrigger("Stun");
        }

        throwEnemyPistol();

        yield return new WaitForSeconds(1.0f);

        if (this != null && !isDead)
        {
            agent.isStopped = false;
            isStunned = false;
        }
    }

    public void TakePunchDamage()
    {
        health--;

        if (health > 0)
        {
            StartCoroutine(Stun());
        }
        else
        {
            Die();
        }
    }

    public void TakeDamage()
    {
        Die();
    }

    public void TriggerDeath()
    {
        if (!isDead)
        {
            isDead = true;
            enemyManager.OnEnemyDeath(this); // `this`를 전달
            gameObject.SetActive(false);
        }
    }

    public interface IDamageable
    {
        void TakeDamage();
        void Die();
    }

    void RecordPosition()
    {
        recordedPositions.Add(transform.position);
    }

    public List<Vector3> GetRecordedPositions()
    {
        return recordedPositions;
    }
}
