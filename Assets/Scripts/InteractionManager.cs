using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public NPCInfo starterNPC;
    public NPCInfo currentNPC;
    public AnimationManager currentAnimationManager;

    public List<StoryObject> currentStoryObjects = new List<StoryObject>();

    void Start()
    {
        StoryStart(starterNPC);
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                StoryObject checkedObject = hit.collider.GetComponent<StoryObject>();
                Debug.Log("Checking " + checkedObject.ObjectName);
                if (checkedObject != null && currentStoryObjects.Contains(checkedObject))
                {
                    Debug.Log("Found " + checkedObject.ObjectName);
                    currentStoryObjects.Remove(checkedObject);
                }
            }
            else
            {
                Debug.Log("No object hit");
            }
        }

        if (currentStoryObjects.Count == 0)
        {
             currentNPC.Body.Leave();
            Debug.Log("You cleared!");
        }
    }

    public void StoryStart(NPCInfo CurrentNPC)
    {
        currentNPC = CurrentNPC;
        currentStoryObjects.Clear(); // IMPORTANT
        currentStoryObjects.AddRange(currentNPC.LostPossessions);
    }
}