using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")] 
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource narrationSource;
    [SerializeField] private AudioSource echoAudioSource;
    [SerializeField] AudioMixerGroup audioMixerGroup;
    [SerializeField] AudioMixer audioMixer;

    [Header("Audio Clips")] [SerializeField]
    public AudioClip backgroundMusicClip;

    [SerializeField] public AudioClip buttonClickClip;
    [SerializeField] public AudioClip callingAnimartionClip;
    [SerializeField] public AudioClip callingVoiceClip;
    [SerializeField] public AudioClip echoPointActivationClip;
    [SerializeField] public AudioClip snowBounceBigTreeClip;
    [SerializeField] public AudioClip snowBounceSmallTreeClip;
    [SerializeField] public AudioClip snowStepClip;
    [SerializeField] public AudioClip soundBounceSnowmanClip;
    [SerializeField] public AudioClip soundBounceStoneClip;
    [SerializeField] public AudioClip soundBounceWoodClip;
    [SerializeField] public AudioClip uswahCallingMamaClip1;
    [SerializeField] public AudioClip uswahCallingMamaClip2;
    [SerializeField] public AudioClip uswahCallingMamaClip3;


    public void PlaySound(AudioClip audioClip, float volume = 1.0f)
    {
        if (sfxSource && audioClip)
        {
            sfxSource.PlayOneShot(audioClip, volume);
        }
    }

    public void PlayNarration(AudioClip audioClip, float volume = 1.0f)
    {
        if (narrationSource && audioClip)
        {
            StopNarration();
            narrationSource.clip = audioClip;
            narrationSource.volume = volume;
            narrationSource.Play();
        }
    }

    public void StopNarration()
    {
        if (narrationSource && narrationSource.isPlaying)
        {
            narrationSource.Stop();
        }
    }

    public void PlayEchoSound(AudioClip audioClip, float volume, int bounceCount)
    {
        if (echoAudioSource == null || audioClip == null)
        {
            return;
        }

        // Apply decay based on bounce count
        float decayFactor = 0.5f; // Each bounce reduces volume by half (adjust as needed)
        float adjustedVolume = volume * Mathf.Pow(decayFactor, bounceCount);

        // Play the main sound
        echoAudioSource.PlayOneShot(audioClip, adjustedVolume);

        // Apply echo effect using AudioEchoFilter if not already added
        AudioEchoFilter echoFilter = echoAudioSource.GetComponent<AudioEchoFilter>();
        if (echoFilter == null)
            echoFilter = echoAudioSource.gameObject.AddComponent<AudioEchoFilter>();

        // Adjust echo parameters (these can be tweaked for better effect)
        echoFilter.delay = 300f; // milliseconds between echoes
        echoFilter.decayRatio = 0.5f; // how quickly echo decays
        echoFilter.wetMix = 0.5f; // how much of echo is heard
        echoFilter.dryMix = 1f;
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