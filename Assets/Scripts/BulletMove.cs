using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float bulletSpeed = 20f;
    private Vector3 direction;
    GameObject bulletPrefab;

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
        //�÷��̾ ���� �ٶ󺸴� �������� �Ѿ��� �߻��ϰ� �ʹ�.
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Enemy"))   
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }
}
