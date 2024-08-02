using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10.0f; // �Ѿ� �ӵ�

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // �ʱ� �ӵ��� �����Ͽ� �� �������� ���ư����� �մϴ�.
            rb.velocity = transform.forward * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet �����տ� Rigidbody ������Ʈ�� �����ϴ�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // �÷��̾�� ���ظ� �����ϴ�.
            }
            Destroy(gameObject); // �Ѿ��� �ı��մϴ�.
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(); // ������ ���ظ� �����ϴ�.
            }
            Destroy(gameObject); // �Ѿ��� �ı��մϴ�.
        }
        else
        {
            Destroy(gameObject); // �ٸ� �Ϳ� �浹���� �� �Ѿ��� �ı��մϴ�.
        }
    }
}
