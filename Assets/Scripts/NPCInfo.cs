using UnityEngine;

public class NPCInfo : MonoBehaviour
{
    public string Name;
    public InteractionManager interactionManager;
    public StoryObject[] LostPossessions;
    public AnimationManager Body;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void StartLoop()
    {
        interactionManager.StoryStart(this);
    }

}
