using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    public Animator MovingInAndOut;
    public Animator FabricMotions;
    public Animator BGMotion;
    public GameManager gameManager;
   public InteractionManager interactionManager;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (FabricMotions.GetCurrentAnimatorStateInfo(0).IsName("floatout")||FabricMotions.GetCurrentAnimatorStateInfo(0).IsName("floatin")) {
    //Debug.Log("Walking");
    interactionManager.currentNPC.Footsteps();
}
    }
    public void TravelIn()
    {
       
    }
    public void SitDown()
    {
        FabricMotions.SetTrigger("Sit");
        interactionManager.currentNPC.SitDown();
    }
    public void StandUp()
    {
        FabricMotions.SetTrigger("Stand");
    }
    public void Leave()
    {
        MovingInAndOut.SetTrigger("WalkOut");
    }
    public void NextStage()
    {
        gameManager.CallNextNPC();
    }

    public void Pause() {
        BGMotion.speed = 0f; // Pauses all animations on this animator
    }

    void Resume() {
        BGMotion.speed = 1f; // Resumes at normal speed
    }

     public void playIntroLine()
    {
        interactionManager.currentNPC.IntroSound();
    }
    public void playOutroLine()
    {
        interactionManager.currentNPC.OutroSound();
    }


}
