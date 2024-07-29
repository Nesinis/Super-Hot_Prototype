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
        //�÷��̾ ���� �ٶ󺸴� �������� �Ѿ��� �߻��ϰ� �ʹ�.
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }
}
