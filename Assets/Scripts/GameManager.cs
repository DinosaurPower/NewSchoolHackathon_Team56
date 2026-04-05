using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public AnimationManager animationManager;
    public NPCInfo[] NPCInfos;
    public int NpcCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CallNextNPC()
    {
        if (NpcCount <= NPCInfos.Length-1){
        NPCInfos[NpcCount].StartLoop();
        NpcCount++;
        }
    }

   
}
