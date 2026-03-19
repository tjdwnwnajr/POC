using UnityEngine;

public class MapFound : MonoBehaviour
{
    [SerializeField] private GameObject newMap;
    [SerializeField] private GameObject mapLoc;
    [SerializeField] private GameObject[] mapLights;
    [SerializeField] private SceneField thisScene;

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
            if (!newMap.activeSelf)
            {
                newMap.SetActive(true);
                if (newMap != null)
                {
                    int count = MapManager.instance.GetPartCount(thisScene);
                    count++;
                    MapManager.instance.SavePartCount(thisScene, count);
                }
                
            }
            
        }
    }
}
