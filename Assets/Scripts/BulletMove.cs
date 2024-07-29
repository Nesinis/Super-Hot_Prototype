using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float bulletSpeed = 20f;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어가 앞을 바라보는 방향으로 총알을 발사하고 싶다.
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }
}
