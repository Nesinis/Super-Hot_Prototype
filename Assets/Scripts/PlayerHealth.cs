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
        Die(); // 한 대 맞으면 즉시 사망 처리합니다.
    }

    void Die()
    {
        // 플레이어 사망 처리 로직을 추가합니다.
        Debug.Log("플레이어가 사망했습니다.");
        gameObject.SetActive(false);

        // 사망 이벤트 호출
        OnPlayerDeath.Invoke();
    }
}
