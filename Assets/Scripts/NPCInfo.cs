using UnityEngine;
using System;
using System.Linq;
using System.Collections;
public class NPCInfo : MonoBehaviour
{
    public string Name;
    public InteractionManager interactionManager;
    public StoryObject[] LostPossessions;
    
    public AnimationManager Body;
    public AudioManager Voice;

    void OnDisable()
    {
        StopFootsteps();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void StartLoop()
    {
        interactionManager.StoryStart(this);
        
    }
    public void RightSound(StoryObject currentObject)
    {
         int objectIndex = Array.IndexOf(LostPossessions, currentObject);
        Voice.PlayObjectDialogue(Name, objectIndex);
    }
    public void IntroSound()
    {
        Voice.PlayDialogue(Name, DialogueLineType.SceneStart);
    }
    public void OutroSound()
    {
         Voice.PlayDialogue(Name, DialogueLineType.SceneEnd);
    }
public void WrongSound(StoryObject checkedObject)
    {
        if (LostPossessions.Contains(checkedObject))
        {
            Voice.PlayDialogue(Name, DialogueLineType.NeedMoreEvidence);
        } else
        {
            Voice.PlayDialogue(Name, DialogueLineType.No);
        }

}
    Coroutine _footstepLoop;

    public void Footsteps()
    {
        if (_footstepLoop != null || Voice == null)
            return;
        _footstepLoop = StartCoroutine(FootstepRoutine());
    }

    public void StopFootsteps()
    {
        if (_footstepLoop != null)
        {
            StopCoroutine(_footstepLoop);
            _footstepLoop = null;
        }
    }

    public void SitDown()
    {
        Voice.PlaySfx(SfxId.SittingDown, 1);
    }

    IEnumerator FootstepRoutine()
    {
        while (true)
        {
            Voice.PlayRandomFootstep();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
