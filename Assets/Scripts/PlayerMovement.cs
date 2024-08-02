using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player �̵� �ӵ� ����
    public float moveSpeed = 5f;
    // CharacterController ������Ʈ�� �����ϱ� ���� ����
    private CharacterController controller;
    // �÷��̾� �̵� ������ �����ϴ� ����
    private Vector3 moveDirection;
    // TimeControl ��ũ��Ʈ�� �����ϱ� ���� ����
    private TimeControl timeControl;

    void Start()
    {
        // CharacterController ������Ʈ�� �����´�
        controller = GetComponent<CharacterController>();
        // Scene ���� TimeControl ��ũ��Ʈ�� ã�Ƽ� �����Ѵ�
        timeControl = FindObjectOfType<TimeControl>();
    }

    void Update()
    {
        // ����� ���� �Է°��� �޴´�
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // �Է°��� ����Ͽ� �̵� ������ ����
        moveDirection = new Vector3(moveX, 0, moveZ);
        // �̵� ������ ���� ��ǥ�迡�� ���� ��ǥ�� ��ȯ
        moveDirection = transform.TransformDirection(moveDirection);
        // �̵� �ӵ��� ����
        moveDirection *= moveSpeed;

        // CharacterController�� ����Ͽ� �̵����� ó��
        controller.Move(moveDirection * Time.deltaTime);

        // Player �������� TimeControl ��ũ��Ʈ�� �����մϴ�.
        timeControl.CheckPlayerMovement(moveX, moveZ);
    }
}
