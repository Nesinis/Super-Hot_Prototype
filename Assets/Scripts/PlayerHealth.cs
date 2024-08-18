using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public UnityEvent OnPlayerDeath;
    public AudioClip deathSound; // 플레이어 사망 사운드
    private AudioSource audioSource; // AudioSource 컴포넌트

    void Start()
    {
        if (OnPlayerDeath == null)
            OnPlayerDeath = new UnityEvent();

        // AudioSource 컴포넌트를 가져옵니다.
        audioSource = GetComponent<AudioSource>();

        // audioSource가 할당되지 않은 경우에 대한 처리
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from this game object. Please add an AudioSource component.");
        }
    }

    public void TakeDamage()
    {
        Die(); // 한 대 맞으면 즉시 사망 처리합니다.
    }

    void Die()
    {
        // 플레이어 사망 처리 로직을 추가합니다.
        Debug.Log("플레이어가 사망했습니다.");

        // 사망 사운드 재생
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        else
        {
            Debug.LogWarning("Death sound or audio source is not assigned.");
        }

        gameObject.SetActive(false);

        // 사망 이벤트 호출
        OnPlayerDeath.Invoke();
    }
}
