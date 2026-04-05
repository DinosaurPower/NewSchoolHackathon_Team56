using System.Collections;
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
    [Tooltip("Second music source for crossfades. If unset, a matching AudioSource is added at runtime.")]
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private AudioSource ambienceSource;

    Coroutine _bgmCrossfadeRoutine;

    [Header("BGM")]
    [Tooltip("Play the first BGM entry in the library (index 0) when the scene starts. CrossfadeBgmTrack1ToTrack2 then replaces it with track 2.")]
    [SerializeField] private bool playFirstBgmOnStart = true;

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
        EnsureMusicSourceB();
        ApplyMixerRouting();
    }

    void Start()
    {
        if (playFirstBgmOnStart)
            PlayBgm(0);
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
        if (musicSourceB != null && bgmMixerGroup != null)
            musicSourceB.outputAudioMixerGroup = bgmMixerGroup;
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
        if (_bgmCrossfadeRoutine != null)
        {
            StopCoroutine(_bgmCrossfadeRoutine);
            _bgmCrossfadeRoutine = null;
        }
        if (musicSource != null)
            musicSource.Stop();
        if (musicSourceB != null)
            musicSourceB.Stop();
    }

    /// <summary>
    /// Crossfades BGM from library track index 0 to index 1 (first → second entry in <see cref="AudioLibrary.bgmTracks"/>).
    /// </summary>
    public void CrossfadeBgmTrack1ToTrack2(float duration = 2f)
    {
        if (_bgmCrossfadeRoutine != null)
            StopCoroutine(_bgmCrossfadeRoutine);
        _bgmCrossfadeRoutine = StartCoroutine(CrossfadeBgmRoutine(0, 1, duration));
    }

    void EnsureMusicSourceB()
    {
        if (musicSourceB != null || musicSource == null)
            return;
        musicSourceB = gameObject.AddComponent<AudioSource>();
        musicSourceB.playOnAwake = false;
        musicSourceB.loop = true;
        musicSourceB.spatialBlend = musicSource.spatialBlend;
        musicSourceB.priority = musicSource.priority;
    }

    IEnumerator CrossfadeBgmRoutine(int fromIndex, int toIndex, float duration)
    {
        EnsureMusicSourceB();
        if (library == null || musicSource == null || musicSourceB == null)
        {
            _bgmCrossfadeRoutine = null;
            yield break;
        }

        var from = library.GetBgm(fromIndex);
        var to = library.GetBgm(toIndex);
        if (to.clip == null)
        {
            _bgmCrossfadeRoutine = null;
            yield break;
        }

        if (from.clip == null)
        {
            PlayBgm(toIndex);
            _bgmCrossfadeRoutine = null;
            yield break;
        }

        if (duration <= 0f)
        {
            musicSourceB.Stop();
            PlayBgm(toIndex);
            _bgmCrossfadeRoutine = null;
            yield break;
        }

        musicSource.clip = from.clip;
        musicSource.loop = true;
        musicSource.volume = from.volume;
        if (!musicSource.isPlaying)
            musicSource.Play();

        musicSourceB.clip = to.clip;
        musicSourceB.loop = true;
        musicSourceB.volume = 0f;
        musicSourceB.time = 0f;
        musicSourceB.Play();

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            musicSource.volume = from.volume * (1f - k);
            musicSourceB.volume = to.volume * k;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = to.clip;
        musicSource.loop = true;
        musicSource.time = musicSourceB.time;
        musicSource.volume = to.volume;
        musicSource.Play();

        musicSourceB.Stop();
        musicSourceB.clip = null;
        musicSourceB.volume = to.volume;

        _bgmCrossfadeRoutine = null;
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
