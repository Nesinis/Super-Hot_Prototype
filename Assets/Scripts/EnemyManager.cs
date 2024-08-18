using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private AudioSource audioSource; // EnemyManager에 붙어 있는 AudioSource
    public AudioClip deathSound; // 적 사망 사운드 클립

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from EnemyManager. Please add an AudioSource component.");
        }
    }

    public void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        else
        {
            Debug.LogWarning("Death sound or audio source is not assigned in EnemyManager.");
        }
    }
}
