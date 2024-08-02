using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    // slowMotionFactor: �÷��̾ ������ �� �ð��� �帧 �ӵ��� �����ϴ� ����
    public float slowMotionFactor = 0.1f;
    // isPlayerMoving: �÷��̾ �����̰� �ִ��� ���θ� �����ϴ� ����
    private bool isPlayerMoving;

    void Update()
    {
        // �� ������ ���� �ð� ��� ó���ϴ� �Լ�
        ControlTime();
    }

    // CheckPlayerMovement: Player ��ũ��Ʈ���� ȣ��Ǿ� �÷��̾��� �������� �����ϴ� �Լ�
    public void CheckPlayerMovement(float moveX, float moveZ)
    {
        // �����̳� ���� �Է��� �ִ��� Ȯ���Ͽ� Player�� �����̴��� ���θ� ����
        isPlayerMoving = moveX != 0 || moveZ != 0;
    }

    // ControlTime: �÷��̾��� �����ӿ� ���� �ð��� �����ϴ� �Լ�
    void ControlTime()
    {
        // ���� �÷��̾ ������ ��
        if (isPlayerMoving)
        {
            // �ð��� �������� �ӵ��� ������
            Time.timeScale = 1f;
            // ���� ��� �ֱ⵵ ���� �ӵ��� ����
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        // �ƴ϶��
        else
        {
            // �÷��̾ ������ �� �ð��� ������ �帧
            Time.timeScale = slowMotionFactor;
            // ���� ��� �ֱ⵵ ���� �ð� �ӵ��� ����
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}
