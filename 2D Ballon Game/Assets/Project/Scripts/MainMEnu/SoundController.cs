using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip[] audioClips; // Array to hold audio clips
    public AudioSource audioSource;

    private void Start()
    {
       PlaySound(audioClips[0]);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null.");
            return;
        }

        if (!audioSource.isPlaying || audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void StopSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
