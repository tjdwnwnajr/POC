using UnityEngine;

public class BtnMinimapActive : TriggerInteractionBase
{
    [SerializeField] private GameObject[] miniMap;
    private bool btnUsed = false;

    public override void Interact()
    {
        if (!btnUsed)
        {
            for (int i = 0; i < miniMap.Length; i++)
            {
                miniMap[i].SetActive(true);
            }
            btnUsed = true;
        }
        
    }
   
}
