using UnityEngine;

public class doorActivebyKey : MonoBehaviour
{
    
    void Update()
    {
        transform.gameObject.SetActive(WorldStateManager.Instance.doorOpened);    
    }
}
