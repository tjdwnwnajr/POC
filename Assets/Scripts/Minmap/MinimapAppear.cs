using UnityEngine;
using UnityEngine.Tilemaps;

public class MinimapAppear : MonoBehaviour
{
    [SerializeField] private GameObject linkedObj;
    
    private TilemapRenderer render;

    private void Start()
    {
        render = GetComponent<TilemapRenderer>();
        render.enabled = false;

    }
    private void Update()
    {
        if (linkedObj.activeSelf) { 
            render.enabled = true;
        }
    }
}
