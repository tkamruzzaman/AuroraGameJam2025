using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource sfxSource;
    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip callingAnimartionClip;
    [SerializeField] private AudioClip callingVoiceClip;
    [SerializeField] private AudioClip echoPointActivationClip;
    [SerializeField] private AudioClip snowBounceBigTreeClip;
    [SerializeField] private AudioClip snowBounceSmallTreeClip;
    [SerializeField] private AudioClip snowStepClip;
    [SerializeField] private AudioClip soundBounceSnowmanClip;
    [SerializeField] private AudioClip soundBounceStoneClip;
    [SerializeField] private AudioClip soundBounceWoodClip;
    [SerializeField] private AudioClip uswahCallingMamaClip1;
    [SerializeField] private AudioClip uswahCallingMamaClip2;
    [SerializeField] private AudioClip uswahCallingMamaClip3;


    public void PlaySound(AudioClip audioClip, float volume = 1.0f)
    {
        if (sfxSource != null && audioClip != null)
        {
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

    public void PlayButtonClickSound() => PlaySound(buttonClickClip);
}
