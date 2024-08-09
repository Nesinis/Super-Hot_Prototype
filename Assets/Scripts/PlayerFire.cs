using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab; // 발사할 총알의 프리팹
    public GameObject firePosition; // 총알이 발사될 위치
    public GameObject PlayerPistol; // 플레이어가 소유한 피스톨 오브젝트
    public GameObject pistolPrefab; // 던질 피스톨의 프리팹

    public Transform throwPos; // 피스톨을 던질 위치
    public float throwPower = 20f; // 피스톨을 던지는 힘
    public float punchRange = 0.2f; // 근접 공격 범위

    void Update()
    {
        // Fire1 버튼이 눌렸을 때
        if (Input.GetButtonDown("Fire1"))
        {
            if (PlayerPistol == null)
            {
                // 피스톨이 없으면 근접 공격을 실행
                FistAttack();
            }
            else
            {
                // 피스톨이 있으면 총을 발사
                bulletFire1();
            }
        }

        // 마우스 오른쪽 버튼이 눌렸고 피스톨이 있을 때
        if (Input.GetMouseButtonDown(1) && PlayerPistol != null)
        {
            // 피스톨을 던짐
            throwPistol();
        }
    }

    // 근접 공격을 수행하는 메소드
    void FistAttack()
    {
        RaycastHit hit;
        // 플레이어의 정면으로 레이캐스트를 쏘아 충돌 여부를 확인
        if (Physics.Raycast(transform.position, transform.forward, out hit, punchRange))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name); // 충돌한 객체의 이름을 로그로 출력

            // 충돌한 객체가 'Enemy' 태그를 가지고 있는 경우
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                // 적의 EnemyAI 컴포넌트를 가져옴
                EnemyAI enemyAI = hit.collider.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // 적을 스턴 상태로 만들고 총을 던지게 함
                    StartCoroutine(enemyAI.Stun());  // Enemy 스턴 적용
                    enemyAI.throwEnemyPistol();  // 적의 총을 던지는 함수 호출
                    Debug.Log("Enemy stunned and pistol thrown: " + hit.collider.gameObject.name); // 스턴 및 피스톨 던지기 적용 디버그 메시지
                }
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any enemy."); // 적을 맞추지 못한 경우 로그 출력
        }
    }

    // 총을 발사하는 메소드
    void bulletFire1()
    {
        // Fire1 버튼이 눌렸을 때
        if (Input.GetButtonDown("Fire1"))
        {
            // 메인 카메라의 정면 방향으로 총알을 발사
            Vector3 shootDirection = Camera.main.transform.forward;
            // 총알 인스턴스를 생성하고 위치와 회전을 설정
            GameObject bulletInstance = Instantiate(bulletPrefab, firePosition.transform.position, Quaternion.identity);
            BulletMove bulletMove = bulletInstance.GetComponent<BulletMove>();
            if (bulletMove != null)
            {
                // 총알에 발사 방향 설정
                bulletMove.SetDirection(shootDirection);
            }
        }
    }

    // 피스톨을 던지는 메소드
    void throwPistol()
    {
        if (PlayerPistol != null)
        {
            // 플레이어 피스톨을 파괴하고 참조를 초기화
            Destroy(PlayerPistol);
            PlayerPistol = null; // 참조 초기화

            // 피스톨을 던질 위치에서 인스턴스를 생성
            GameObject thrownPistol = Instantiate(pistolPrefab, firePosition.transform.position, Quaternion.identity);
            Rigidbody rb = thrownPistol.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 던질 방향을 설정하고 힘을 가하여 피스톨을 던짐
                Vector3 throwDir = (firePosition.transform.forward + firePosition.transform.up * 0.3f).normalized;
                rb.AddForce(throwDir * throwPower, ForceMode.Impulse);
            }
        }
    }
}
