using UnityEngine;
using UnityEngine.SceneManagement;

public class SmoothFollow : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // 회전 속도
    public float recoilAmount = 40.0f; // 반동 강도
    public float recoilRecoverySpeed = 2.0f; // 반동 회복 속도

    private float angle = 0f;
    private Quaternion initialRotation; // 초기 회전값 저장
    private Quaternion targetRotation; // 목표 회전값

    void Start()
    {
        // 스크립트가 시작될 때 초기 회전값을 저장
        initialRotation = transform.rotation;
        targetRotation = initialRotation; // 초기 목표는 기본 회전값

        ApplyRecoil();

    }

    void Update()
    {

        // 반동 회복 - 카메라가 서서히 원래 위치로 돌아옴
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * recoilRecoverySpeed);

        // 부드럽게 회전
        angle += rotationSpeed * Time.deltaTime;
        targetRotation = initialRotation * Quaternion.Euler(0, angle, 0); // 목표 회전값 업데이트

        // 마우스 오른쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right mouse button pressed - Restarting scene");
            SceneManager.LoadScene("JumpScene"); // "JumpScene"은 재시작할 씬의 이름입니다.
        }
    }

    void ApplyRecoil()
    {
        // 반동 적용 - 카메라를 순간적으로 위로 튕기듯이 회전
        transform.rotation *= Quaternion.Euler(-recoilAmount, 0, 0);
    }
}
