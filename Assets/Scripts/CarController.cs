using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float driftAmount = 5f;

    private bool hasDrifted = false;

    void Update()
    {
        if (!hasDrifted)
        {
            // 차가 앞으로 이동
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // 드리프트를 시작
            StartCoroutine(Drift());
        }
    }

    IEnumerator Drift()
    {
        // 드리프트 실행
        yield return new WaitForSeconds(1f); // 드리프트 시간
        transform.Rotate(Vector3.up, driftAmount); // 단순한 회전으로 드리프트 구현

        // 드리프트가 완료되면 플래그 설정
        hasDrifted = true;

        // 차를 멈춤
        speed = 0f;
    }
}