using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallActive : MonoBehaviour
{
    private Transform parentObj;
    private Vector3 rot;
    private TilemapCollider2D col;
    private TilemapRenderer render;
    private void Start()
    {
        col = GetComponent<TilemapCollider2D>();
        render= GetComponent<TilemapRenderer>();
        col.enabled = false;
        render.enabled = false;
        parentObj = transform.parent;
        rot = parentObj.eulerAngles;
    }

    private void Update()
    {
        rot = parentObj.eulerAngles;
        Debug.Log(rot.z);
        if (rot.z < 5f && rot.z > -5f) 
        {
            col.enabled=false;
            render.enabled=false;
        }
        else
        {
            col.enabled = true;
            render.enabled = true;
        }
    }
}
