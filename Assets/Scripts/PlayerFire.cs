using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab; // �߻��� �Ѿ��� ������
    public GameObject firePosition; // �Ѿ��� �߻�� ��ġ
    public GameObject PlayerPistol; // �÷��̾ ������ �ǽ��� ������Ʈ
    public GameObject pistolPrefab; // ���� �ǽ����� ������

    public Transform throwPos;
    public float throwPower = 20f;

    public Animator animator; // �÷��̾��� �ִϸ�����

    void Start()
    {
        // �ʱ�ȭ ������ �ʿ��� ��� ���⿡ �ۼ�
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
            animator.SetTrigger("Punch"); // �ָ� ���� �ִϸ��̼� ���
        }
    }
}
