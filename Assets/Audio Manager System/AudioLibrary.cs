using System;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueLineType
{
    NeedMoreEvidence,
    No,
    SceneEnd
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
    TrainGo
}

public enum AmbienceId
{
    TrainTunnel,
    TrainCar
}

/// <summary>
/// One character’s dialogue clips. Add entries in the Inspector array; use CharacterId from other scripts.
/// </summary>
[Serializable]
public class CharacterDialogue
{
    [Tooltip("Used by code, e.g. GetDialogueClip(\"Detective\", ...)")]
    public string characterId = "Default";

    public AudioClip needMoreEvidence;
    public AudioClip no;
    public AudioClip sceneEnd;

    [Header("Object dialogue")]
    [Tooltip("Index in this array = object index in your game (e.g. inspectable 0, 1, 2…).")]
    public AudioClip[] dialogLinesByObjectIndex = Array.Empty<AudioClip>();
}

/// <summary>
/// Optional named BGM slot so you can add/reorder tracks in the Inspector.
/// </summary>
[Serializable]
public class BgmTrack
{
    public string trackName = "Track";
    public AudioClip clip;
}

/// <summary>
/// All one-shot SFX referenced by SfxId.
/// </summary>
[Serializable]
public class SfxClips
{
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip footstep3;
    public AudioClip sittingDown;
    public AudioClip click;
    public AudioClip trainStop;
    public AudioClip trainGo;
}

/// <summary>
/// Looping or long ambience beds.
/// </summary>
[Serializable]
public class AmbienceClips
{
    public AudioClip trainTunnelAmbience;
    public AudioClip trainCarAmbience;
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

    public AudioClip GetDialogueClip(string characterId, DialogueLineType line)
    {
        var entry = FindCharacter(characterId);
        if (entry == null)
            return null;

        return line switch
        {
            DialogueLineType.NeedMoreEvidence => entry.needMoreEvidence,
            DialogueLineType.No => entry.no,
            DialogueLineType.SceneEnd => entry.sceneEnd,
            _ => null
        };
    }

    /// <summary>Line when this character comments on the object at <paramref name="objectIndex"/>.</summary>
    public AudioClip GetObjectDialogueClip(string characterId, int objectIndex)
    {
        var entry = FindCharacter(characterId);
        if (entry == null || entry.dialogLinesByObjectIndex == null)
            return null;
        if (objectIndex < 0 || objectIndex >= entry.dialogLinesByObjectIndex.Length)
            return null;
        return entry.dialogLinesByObjectIndex[objectIndex];
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

    public AudioClip GetBgmClip(int index)
    {
        if (bgmTracks == null || index < 0 || index >= bgmTracks.Length)
            return null;
        return bgmTracks[index].clip;
    }

    public AudioClip GetBgmClipByName(string trackName)
    {
        if (string.IsNullOrEmpty(trackName) || bgmTracks == null)
            return null;

        for (int i = 0; i < bgmTracks.Length; i++)
        {
            if (bgmTracks[i] != null &&
                string.Equals(bgmTracks[i].trackName, trackName, StringComparison.OrdinalIgnoreCase))
                return bgmTracks[i].clip;
        }

        return null;
    }

    public AudioClip GetSfx(SfxId id)
    {
        return id switch
        {
            SfxId.DoorOpen => sfx.doorOpen,
            SfxId.DoorClose => sfx.doorClose,
            SfxId.Footstep1 => sfx.footstep1,
            SfxId.Footstep2 => sfx.footstep2,
            SfxId.Footstep3 => sfx.footstep3,
            SfxId.SittingDown => sfx.sittingDown,
            SfxId.Click => sfx.click,
            SfxId.TrainStop => sfx.trainStop,
            SfxId.TrainGo => sfx.trainGo,
            _ => null
        };
    }

    /// <summary>Random non-null footstep from footstep1–3.</summary>
    public AudioClip GetRandomFootstepClip()
    {
        var pool = new List<AudioClip>(3);
        if (sfx.footstep1 != null) pool.Add(sfx.footstep1);
        if (sfx.footstep2 != null) pool.Add(sfx.footstep2);
        if (sfx.footstep3 != null) pool.Add(sfx.footstep3);
        if (pool.Count == 0)
            return null;
        return pool[UnityEngine.Random.Range(0, pool.Count)];
    }

    public AudioClip GetAmbience(AmbienceId id)
    {
        return id switch
        {
            AmbienceId.TrainTunnel => ambience.trainTunnelAmbience,
            AmbienceId.TrainCar => ambience.trainCarAmbience,
            _ => null
        };
    }
}
