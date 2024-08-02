using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10.0f; // 총알 속도

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 초기 속도를 설정하여 한 방향으로 날아가도록 합니다.
            rb.velocity = transform.forward * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet 프리팹에 Rigidbody 컴포넌트가 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // 플레이어에게 피해를 입힙니다.
            }
            Destroy(gameObject); // 총알을 파괴합니다.
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(); // 적에게 피해를 입힙니다.
            }
            Destroy(gameObject); // 총알을 파괴합니다.
        }
        else
        {
            Destroy(gameObject); // 다른 것에 충돌했을 때 총알을 파괴합니다.
        }
    }
}
