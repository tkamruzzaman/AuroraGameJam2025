using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField ] private AudioSource bgMusicSource;
    [SerializeField ] private AudioSource sfxSource;
    [Header("Audio Clips")]
    [SerializeField ] private AudioClip backgroundMusicClip;
    [SerializeField ] private AudioClip buttonClickClip;


    public void PlaySound(AudioClip audioClip, float volume = 1.0f)
    {
        if (sfxSource != null && audioClip != null){
        sfxSource.PlayOneShot(audioClip, volume);
    }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClip != null && bgMusicSource != null)
        {
            bgMusicSource.clip = backgroundMusicClip;
            bgMusicSource.loop = true;
            bgMusicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        bgMusicSource?.Stop();
    }

    public void PlayButtonClickSound()
    {
        PlaySound(buttonClickClip);
    }
}
