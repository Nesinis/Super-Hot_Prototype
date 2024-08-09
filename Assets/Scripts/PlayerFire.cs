using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject firePosition;
    public GameObject PlayerPistol;
    public GameObject pistolPrefab;

    public Transform throwPos;
    public float throwPower = 20f;

    void Start()
    {
        
    }

    void Update()
    {
        bulletFire1();
        throwPistol();
    }

    void bulletFire1()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Vector3 shootDirection = Camera.main.transform.forward;
            GameObject bulletInstance = Instantiate(bulletPrefab, firePosition.transform.position, firePosition.transform.rotation);
            BulletMove bulletMove = bulletInstance.GetComponent<BulletMove>();
            if(bulletMove != null )
            {
                bulletMove.SetDirection(shootDirection);
                //print(shootDirection.ToString());
            }
        }
    }
    void throwPistol()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if (PlayerPistol != null)
            {
                PlayerPistol.SetActive(false); // PlayerPistol을 비활성화합니다.
                GameObject thrownPistol = Instantiate(pistolPrefab, firePosition.transform.position, firePosition.transform.rotation);
                
                Rigidbody rb = thrownPistol.GetComponent<Rigidbody>();

                if(rb != null)
                {
                    Vector3 throwDir = (firePosition.transform.forward + firePosition.transform.up * 0.2f).normalized;
                    rb.AddForce(throwDir * throwPower, ForceMode.Impulse);
                }
            }

        }
    }

}   
