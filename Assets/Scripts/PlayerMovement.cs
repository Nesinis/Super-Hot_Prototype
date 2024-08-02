using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player 이동 속도 변수
    public float moveSpeed = 5f;
    // CharacterController 컴포넌트를 참조하기 위한 변수
    private CharacterController controller;
    // 플레이어 이동 방향을 저장하는 변수
    private Vector3 moveDirection;
    // TimeControl 스크립트를 참조하기 위한 변수
    private TimeControl timeControl;

    void Start()
    {
        // CharacterController 컴포넌트를 가져온다
        controller = GetComponent<CharacterController>();
        // Scene 에서 TimeControl 스크립트를 찾아서 참조한다
        timeControl = FindObjectOfType<TimeControl>();
    }

    void Update()
    {
        // 수평과 수직 입력값을 받는다
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 입력값을 기반하여 이동 방향을 설정
        moveDirection = new Vector3(moveX, 0, moveZ);
        // 이동 방향을 월드 좌표계에서 로컬 좌표로 변환
        moveDirection = transform.TransformDirection(moveDirection);
        // 이동 속도를 적용
        moveDirection *= moveSpeed;

        // CharacterController를 사용하여 이동값을 처리
        controller.Move(moveDirection * Time.deltaTime);

        // Player 움직임을 TimeControl 스크립트에 전달합니다.
        timeControl.CheckPlayerMovement(moveX, moveZ);
    }
}
