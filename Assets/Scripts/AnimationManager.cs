using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    public Animator MovingInAndOut;
    public Animator FabricMotions;
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
}
