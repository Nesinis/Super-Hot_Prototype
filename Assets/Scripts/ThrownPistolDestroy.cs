using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownPistolDestroy : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        // 땅과 충돌했을 때 피스톨 오브젝트 삭제
        // 이때 충돌한 물체가 'Ground' 태그를 가졌는지 확인하거나,
        // 특정 시간 이후에 사라지도록 할 수 있습니다.

        // 여기서는 간단히 충돌 시 바로 삭제
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // 피스톨 오브젝트 삭제
        }
    }
}
