using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip[] audioClips;  // Array to hold different audio clips
    public AudioSource audioSource;  // AudioSource component to play the clips

    private void Start()
    {
        // Play the first audio clip from the array when the game starts
        if (audioClips.Length > 0)
        {
            PlaySound(audioClips[0]);
        }
        else
        {
            Debug.LogWarning("No audio clips assigned in the array.");
        }
    }

    public void PlaySound(AudioClip clip)
    {
        // Check if the provided audio clip is not null
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null.");
            return;
        }

        // Play the audio clip if it's not already playing or if it's a different clip
        if (!audioSource.isPlaying || audioSource.clip != clip)
        {
            audioSource.clip = clip;  // Set the clip to the AudioSource
            audioSource.Play();      // Play the clip
        }
    }

    public void StopSound()
    {
        // Stop playing the audio if it is currently playing
        if (audioSource.isPlaying)
        {
            audioSource.Stop();  // Stop the AudioSource
        }
    }
}
