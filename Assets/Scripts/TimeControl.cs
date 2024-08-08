using UnityEngine;

public class TimeControl : MonoBehaviour
{
    // Player �̵� �ӵ� ����
    public float moveSpeed = 5f;

    // �ð� ������ ����
    private float timeScale = 0f;

    // �ð� ��ȭ �ӵ�
    public float smoothTime = 0.1f;

    // ��ǥ �ð� ������
    public float targetTimeScale = 1f;

    // �ּ� �ð� ������
    public float minTimeScale = 0.1f;

    // �ִ� �ð� ������
    public float maxTimeScale = 1f;

    private void Update()
    {
        // �ε巯�� �ð� ��ȭ�� ���� SmoothDamp ���
        timeScale = Mathf.SmoothDamp(timeScale, targetTimeScale, ref moveSpeed, smoothTime);

        // �ð� ������ ����
        Time.timeScale = timeScale;
    }

  

    public void UpdateTimeScale(bool isPlayerMoving)
    {
        if (isPlayerMoving)
        {
            // �÷��̾ �����̰� �ִٸ� ��ǥ �ð� �������� �ִ�� ����
            targetTimeScale = maxTimeScale;
        }
        else
        {
            // �׷��� �ʴٸ� ��ǥ �ð� �������� �ּҷ� ����
            targetTimeScale = minTimeScale;
        }
    }
}