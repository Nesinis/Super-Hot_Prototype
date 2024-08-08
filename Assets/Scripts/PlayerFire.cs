using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab; // 발사할 총알의 프리팹
    public GameObject firePosition; // 총알이 발사될 위치
    public GameObject PlayerPistol; // 플레이어가 소유한 피스톨 오브젝트
    public GameObject pistolPrefab; // 던질 피스톨의 프리팹

    public Transform throwPos;
    public float throwPower = 20f;

    public Animator animator; // 플레이어의 애니메이터

    void Start()
    {
        // 초기화 로직이 필요한 경우 여기에 작성
    }

    void Update()
    {
        if (PlayerPistol == null)
        {
            FistAttack();
        }
        else
        {
            bulletFire1();
            throwPistol();
        }
    }

    void bulletFire1()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 shootDirection = Camera.main.transform.forward;
            GameObject bulletInstance = Instantiate(bulletPrefab, firePosition.transform.position, firePosition.transform.rotation);
            BulletMove bulletMove = bulletInstance.GetComponent<BulletMove>();
            if (bulletMove != null)
            {
                bulletMove.SetDirection(shootDirection);
                print(shootDirection.ToString());
            }
        }
    }

    void throwPistol()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (PlayerPistol != null)
            {
                Destroy(PlayerPistol);
                GameObject thrownPistol = Instantiate(pistolPrefab, firePosition.transform.position, firePosition.transform.rotation);
                print("pistol generated");
                Rigidbody rb = thrownPistol.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    print("rb != null");
                    Vector3 throwDir = (firePosition.transform.forward + firePosition.transform.up * 0.3f).normalized;
                    rb.AddForce(throwDir * throwPower, ForceMode.Impulse);
                }
            }
        }
    }

    void FistAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Punch"); // 주먹 공격 애니메이션 재생
        }
    }
}
