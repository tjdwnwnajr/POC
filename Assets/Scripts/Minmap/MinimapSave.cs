using UnityEngine;

public class MinimapRestorer : MonoBehaviour
{
    [SerializeField] private SceneField thisScene; // 인스펙터에서 이 씬 드래그

    private void Start()
    {
        if (MapManager.instance == null) return;

        int savedCount = MapManager.instance.GetPartCount(thisScene);

        for (int i = 0; i < savedCount && i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}