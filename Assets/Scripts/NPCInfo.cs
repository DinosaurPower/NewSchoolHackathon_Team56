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
public void Footsteps()
    {
  StartCoroutine(FootstepRoutine());
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
        yield return new WaitForSeconds(0.5f); // adjust timing here
    }
}
}
