using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Assign an Audio Library asset and AudioSources in the Inspector. Other scripts can call this to play clips.
/// Drag a <b>library asset</b> (Create → Audio → Audio Library), not the .cs script file.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioLibrary library;

    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;

    [Header("Mixer routing")]
    [Tooltip("Dialogue / voice lines (e.g. Dialoug group on AudioMixer).")]
    [SerializeField] private AudioMixerGroup dialogueMixerGroup;
    [Tooltip("One-shots: doors, UI, footsteps, etc. (e.g. SFX group).")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [Tooltip("Music loops (e.g. BGM group).")]
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [Tooltip("Ambient beds (e.g. Ambience group).")]
    [SerializeField] private AudioMixerGroup ambienceMixerGroup;

    public AudioLibrary Library => library;

    private void Awake()
    {
        ApplyMixerRouting();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyMixerRouting();
    }
#endif

    private void ApplyMixerRouting()
    {
        if (voiceSource != null && dialogueMixerGroup != null)
            voiceSource.outputAudioMixerGroup = dialogueMixerGroup;
        if (sfxSource != null && sfxMixerGroup != null)
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        if (musicSource != null && bgmMixerGroup != null)
            musicSource.outputAudioMixerGroup = bgmMixerGroup;
        if (ambienceSource != null && ambienceMixerGroup != null)
            ambienceSource.outputAudioMixerGroup = ambienceMixerGroup;
    }

    private void Reset()
    {
        sfxSource = GetComponent<AudioSource>();
    }

    public void PlayDialogue(string characterId, DialogueLineType line)
    {
        if (library == null || voiceSource == null)
            return;

        var sound = library.GetDialogue(characterId, line);
        if (sound.clip != null)
            voiceSource.PlayOneShot(sound.clip, sound.volume);
    }

    public void PlayObjectDialogue(string characterId, int objectIndex)
    {
        if (library == null || voiceSource == null)
            return;

        var sound = library.GetObjectDialogue(characterId, objectIndex);
        if (sound.clip != null)
            voiceSource.PlayOneShot(sound.clip, sound.volume);
    }

    public void PlaySfx(SfxId id, float volumeScale = 1f)
    {
        if (library == null || sfxSource == null)
            return;

        var sound = library.GetSfx(id);
        if (sound.clip != null)
            sfxSource.PlayOneShot(sound.clip, sound.volume * volumeScale);
    }

    public void PlayRandomFootstep(float volumeScale = 1f)
    {
        if (library == null || sfxSource == null)
            return;

        var sound = library.GetRandomFootstep();
        if (sound.clip != null)
            sfxSource.PlayOneShot(sound.clip, sound.volume * volumeScale);
    }

    public void PlayBgm(int trackIndex, bool loop = true)
    {
        if (library == null || musicSource == null)
            return;

        var sound = library.GetBgm(trackIndex);
        if (sound.clip == null)
            return;

        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlayBgmByName(string trackName, bool loop = true)
    {
        if (library == null || musicSource == null)
            return;

        var sound = library.GetBgmByName(trackName);
        if (sound.clip == null)
            return;

        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopBgm()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayAmbience(AmbienceId id, bool loop = true)
    {
        if (library == null || ambienceSource == null)
            return;

        var sound = library.GetAmbience(id);
        if (sound.clip == null)
            return;

        ambienceSource.clip = sound.clip;
        ambienceSource.volume = sound.volume;
        ambienceSource.loop = loop;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        if (ambienceSource != null)
            ambienceSource.Stop();
    }
}
