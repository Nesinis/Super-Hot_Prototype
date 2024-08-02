using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public UnityEvent OnPlayerDeath;

    void Start()
    {
        if (OnPlayerDeath == null)
            OnPlayerDeath = new UnityEvent();
    }

    public void TakeDamage()
    {
        Die(); // �� �� ������ ��� ��� ó���մϴ�.
    }

    void Die()
    {
        // �÷��̾� ��� ó�� ������ �߰��մϴ�.
        Debug.Log("�÷��̾ ����߽��ϴ�.");
        gameObject.SetActive(false);

        // ��� �̺�Ʈ ȣ��
        OnPlayerDeath.Invoke();
    }
}
