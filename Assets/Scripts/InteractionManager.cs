using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    /// <summary>Same cursor ray used for hover/click this frame (corrected screen → <see cref="Camera.main"/>).</summary>
    public Ray CurrentCursorRay { get; private set; }

    [Tooltip("Layers the interaction ray hits. Exclude Plane/backdrop so rays reach props in front.")]
    [SerializeField] LayerMask interactionRaycastLayers = -1;
    public GameObject Menu;
    public NPCInfo starterNPC;
    public NPCInfo currentNPC;
    public AnimationManager currentAnimationManager;
    public GameObject NPCSprite;
    public List<StoryObject> currentStoryObjects = new List<StoryObject>();

    StoryObject _lastHovered;
    StoryObject _lastClicked;

    void Start()
    {
       NPCSprite.GetComponent<Animator>();
    }

    void Update()
    {
        if (_lastClicked != null)
        {
            _lastClicked.IsClicked = false;
            _lastClicked = null;
        }

        if (_lastHovered != null)
        {
            _lastHovered.IsHovered = false;
            _lastHovered = null;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        if (CursorCorrection.Instance != null)
            screenPos = CursorCorrection.Instance.Correct(screenPos);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        CurrentCursorRay = ray;
        StoryObject hitStory = null;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactionRaycastLayers, QueryTriggerInteraction.UseGlobal))
        {
            hitStory = hit.collider.GetComponent<StoryObject>();
            if (hitStory != null)
            {
                hitStory.IsHovered = true;
                _lastHovered = hitStory;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (hitStory != null)
            {
                hitStory.IsClicked = true;
                _lastClicked = hitStory;
                if (currentStoryObjects.Contains(hitStory))
                {
                   
                    currentNPC.RightSound(hitStory);
                    Debug.Log("Found " + hitStory.ObjectName);
                    currentStoryObjects.Remove(hitStory);
                } else if (hitStory != null)
                {
                   currentNPC.WrongSound(hitStory); 
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
        currentStoryObjects.Clear();
        currentStoryObjects.AddRange(currentNPC.LostPossessions);
        currentNPC.Body.MovingInAndOut.SetTrigger("WalkIn");
    }

    public void StartGame()
    {
        Menu.SetActive(false);
        StoryStart(starterNPC);
        NPCSprite.SetActive(true);
        
        
    }
}
