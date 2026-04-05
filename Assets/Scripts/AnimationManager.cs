using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    public Animator MovingInAndOut;
    public Animator FabricMotions;
    public Animator BGMotion;
    public GameManager gameManager;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TravelIn()
    {
       
    }
    public void SitDown()
    {
        FabricMotions.SetTrigger("Sit");
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

}
