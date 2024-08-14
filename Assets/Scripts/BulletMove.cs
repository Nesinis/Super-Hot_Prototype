using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float bulletSpeed = 20f;
    private Vector3 direction;

    // 총알의 방향을 설정하는 메서드
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        // 설정된 방향으로 총알을 이동시킵니다.
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }

    // 충돌 처리 메서드
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Enemy"))
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject); // 충돌 시 총알을 파괴
    }
}
