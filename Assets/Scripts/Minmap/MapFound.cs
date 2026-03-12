using UnityEngine;

public class MapFound : MonoBehaviour
{
    [SerializeField] private GameObject newMap;
    [SerializeField] private GameObject mapLoc;
    [SerializeField] private GameObject[] mapLights;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < mapLights.Length; i++) {
                if (mapLights[i] != null)
                {
                    mapLights[i].SetActive(true);
                }
            }
            if(mapLoc != null) 
                mapLoc.SetActive(false);
            newMap.SetActive(true);
        }
    }
}
