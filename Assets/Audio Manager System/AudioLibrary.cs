using System;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueLineType
{
    NeedMoreEvidence,
    No,
    SceneEnd,
    SceneStart
}

public enum SfxId
{
    DoorOpen,
    DoorClose,
    Footstep1,
    Footstep2,
    Footstep3,
    SittingDown,
    Click,
    TrainStop,
    TrainGo,
    LastStop
}

public enum AmbienceId
{
    TrainTunnel,
    TrainCar
}

/// <summary>
/// Clip plus per-asset volume (Inspector slider). Multiplied with optional runtime scales in AudioManager.
/// </summary>
[Serializable]
public class SoundEntry
{
    public AudioClip clip;
    [Range(0f, 1f)]
    [Tooltip("Per-clip level in the library (1 = full).")]
    public float volume = 1f;
}

/// <summary>
/// One character’s dialogue clips. Add entries in the Inspector array; use CharacterId from other scripts.
/// </summary>
[Serializable]
public class CharacterDialogue
{
    [Tooltip("Used by code, e.g. GetDialogue(\"Detective\", ...)")]
    public string characterId = "Default";

    public SoundEntry needMoreEvidence = new SoundEntry();
    public SoundEntry no = new SoundEntry();
    public SoundEntry sceneEnd = new SoundEntry();
    public SoundEntry sceneStart = new SoundEntry();

    [Header("Object dialogue")]
    [Tooltip("Index in this array = object index in your game (e.g. inspectable 0, 1, 2…).")]
    public SoundEntry[] dialogLinesByObjectIndex = Array.Empty<SoundEntry>();

    public SoundEntry footsteps = new SoundEntry();
}

/// <summary>
/// Optional named BGM slot so you can add/reorder tracks in the Inspector.
/// </summary>
[Serializable]
public class BgmTrack
{
    public string trackName = "Track";
    public SoundEntry track = new SoundEntry();
}

/// <summary>
/// All one-shot SFX referenced by SfxId.
/// </summary>
[Serializable]
public class SfxClips
{
    public SoundEntry doorOpen = new SoundEntry();
    public SoundEntry doorClose = new SoundEntry();
    public SoundEntry footstep1 = new SoundEntry();
    public SoundEntry footstep2 = new SoundEntry();
    public SoundEntry footstep3 = new SoundEntry();
    public SoundEntry sittingDown = new SoundEntry();
    public SoundEntry click = new SoundEntry();
    public SoundEntry trainStop = new SoundEntry();
    public SoundEntry trainGo = new SoundEntry();
     public SoundEntry lastStop = new SoundEntry();
}

/// <summary>
/// Looping or long ambience beds.
/// </summary>
[Serializable]
public class AmbienceClips
{
    public SoundEntry trainTunnelAmbience = new SoundEntry();
    public SoundEntry trainCarAmbience = new SoundEntry();
}

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [Header("Dialogue")]
    [Tooltip("Per-character lines: Need more evidence, No, Scene end.")]
    public CharacterDialogue[] characters = Array.Empty<CharacterDialogue>();

    [Header("BGM")]
    public BgmTrack[] bgmTracks = Array.Empty<BgmTrack>();

    [Header("SFX")]
    public SfxClips sfx = new SfxClips();

    [Header("Ambience")]
    public AmbienceClips ambience = new AmbienceClips();

    public SoundEntry GetDialogue(string characterId, DialogueLineType line)
    {
        var entry = FindCharacter(characterId);
        if (entry == null)
            return new SoundEntry();

        return line switch
        {
            DialogueLineType.NeedMoreEvidence => entry.needMoreEvidence ?? new SoundEntry(),
            DialogueLineType.No => entry.no ?? new SoundEntry(),
            DialogueLineType.SceneEnd => entry.sceneEnd ?? new SoundEntry(),
            DialogueLineType.SceneStart => entry.sceneStart ?? new SoundEntry(),
            _ => new SoundEntry()
        };
    }

    /// <summary>Line when this character comments on the object at <paramref name="objectIndex"/>.</summary>
    public SoundEntry GetObjectDialogue(string characterId, int objectIndex)
    {
        var entry = FindCharacter(characterId);
        if (entry == null || entry.dialogLinesByObjectIndex == null)
            return new SoundEntry();
        if (objectIndex < 0 || objectIndex >= entry.dialogLinesByObjectIndex.Length)
            return new SoundEntry();
        return entry.dialogLinesByObjectIndex[objectIndex] ?? new SoundEntry();
    }

    public CharacterDialogue FindCharacter(string characterId)
    {
        if (string.IsNullOrEmpty(characterId) || characters == null)
            return null;

        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null &&
                string.Equals(characters[i].characterId, characterId, StringComparison.OrdinalIgnoreCase))
                return characters[i];
        }

        return null;
    }

    public SoundEntry GetBgm(int index)
    {
        if (bgmTracks == null || index < 0 || index >= bgmTracks.Length)
            return new SoundEntry();
        var t = bgmTracks[index];
        return t?.track ?? new SoundEntry();
    }

    public SoundEntry GetBgmByName(string trackName)
    {
        if (string.IsNullOrEmpty(trackName) || bgmTracks == null)
            return new SoundEntry();

        for (int i = 0; i < bgmTracks.Length; i++)
        {
            if (bgmTracks[i] != null &&
                string.Equals(bgmTracks[i].trackName, trackName, StringComparison.OrdinalIgnoreCase))
                return bgmTracks[i].track ?? new SoundEntry();
        }

        return new SoundEntry();
    }

    public SoundEntry GetSfx(SfxId id)
    {
        return id switch
        {
            SfxId.DoorOpen => sfx.doorOpen ?? new SoundEntry(),
            SfxId.DoorClose => sfx.doorClose ?? new SoundEntry(),
            SfxId.Footstep1 => sfx.footstep1 ?? new SoundEntry(),
            SfxId.Footstep2 => sfx.footstep2 ?? new SoundEntry(),
            SfxId.Footstep3 => sfx.footstep3 ?? new SoundEntry(),
            SfxId.SittingDown => sfx.sittingDown ?? new SoundEntry(),
            SfxId.Click => sfx.click ?? new SoundEntry(),
            SfxId.TrainStop => sfx.trainStop ?? new SoundEntry(),
            SfxId.TrainGo => sfx.trainGo ?? new SoundEntry(),
            SfxId.LastStop => sfx.lastStop ?? new SoundEntry(),
            _ => new SoundEntry()
        };
    }

    /// <summary>Random non-null footstep from footstep1–3.</summary>
    public SoundEntry GetRandomFootstep()
    {
        var pool = new List<SoundEntry>(3);
        if (sfx.footstep1 != null && sfx.footstep1.clip != null) pool.Add(sfx.footstep1);
        if (sfx.footstep2 != null && sfx.footstep2.clip != null) pool.Add(sfx.footstep2);
        if (sfx.footstep3 != null && sfx.footstep3.clip != null) pool.Add(sfx.footstep3);
        if (pool.Count == 0)
            return new SoundEntry();
        return pool[UnityEngine.Random.Range(0, pool.Count)];
    }

    public SoundEntry GetAmbience(AmbienceId id)
    {
        return id switch
        {
            AmbienceId.TrainTunnel => ambience.trainTunnelAmbience ?? new SoundEntry(),
            AmbienceId.TrainCar => ambience.trainCarAmbience ?? new SoundEntry(),
            _ => new SoundEntry()
        };
    }
}
