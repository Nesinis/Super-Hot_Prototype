using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject firePosition;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            bulletFire();
            //print("마우스 입력");
            //for (int i = 0; i < firePosition.Length; i++)
            //{
            //}
        }
    }

    void bulletFire()
    {
        Vector3 shootDirection = Camera.main.transform.forward;
        GameObject bulletInstance = Instantiate(bulletPrefab, firePosition.transform.position, firePosition.transform.rotation);
        BulletMove bulletMove = bulletInstance.GetComponent<BulletMove>();
        if(bulletMove != null )
        {
            bulletMove.SetDirection(shootDirection);
            print(shootDirection.ToString());
        }
    }
}   
