using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject[] firePosition = new GameObject[3];

    void Start()
    {
        
    }

    void Update()
    {
        for (int i = 0; i < firePosition.Length; i++)
        {
            bool mouseInput = Input.GetButtonDown("Fire1");

            if (mouseInput)
            {
                bulletFire(i);
            }
        }
    }

    void bulletFire(int i)
    {
        Vector3 shootDirection = Camera.main.transform.forward;
        GameObject bulletInstance = Instantiate(bulletPrefab, firePosition[i].transform.position, firePosition[i].transform.rotation);
        BulletMove bulletMove = bulletInstance.GetComponent<BulletMove>(); //질문: BulletMove라는 자료형은 어디에서 온 거지?
        if(bulletMove != null )
        {
            bulletMove.SetDirection(shootDirection);
        }
    }
}   
