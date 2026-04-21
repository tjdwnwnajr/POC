using UnityEngine;

public class MapLightOff : MonoBehaviour
{
    [SerializeField] private GameObject linkedMap;
    void Update()
    {
        if(linkedMap != null)
        {
            if(linkedMap.activeSelf)
            {
                transform.gameObject.SetActive(false);
            }
            
        }
    }
}
