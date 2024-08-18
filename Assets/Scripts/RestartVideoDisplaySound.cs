using UnityEngine;

public class RestartVideoDisplaySound : MonoBehaviour
{
    public AudioClip soundClip; // 재생할 사운드 클립
    private AudioSource audioSource; // AudioSource 컴포넌트

    void Awake()
    {
        // AudioSource 컴포넌트를 가져옵니다.
        audioSource = GetComponent<AudioSource>();

        // AudioSource가 존재하지 않는 경우 오류 메시지 출력
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from this game object. Please add an AudioSource component.");
        }
    }

    void OnEnable()
    {
        PlaySound();
    }

    private void PlaySound()
    {
        // AudioSource와 SoundClip이 제대로 할당되었는지 확인 후 재생
        if (audioSource != null && soundClip != null)
        {
            audioSource.PlayOneShot(soundClip);
        }
        else
        {
            Debug.LogWarning("Sound clip or audio source is not assigned.");
        }
    }
}
