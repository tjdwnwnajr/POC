using UnityEngine;

public class CameraLateUpdate : MonoBehaviour
{
    void LateUpdate()
    {
        float pixelsPerUnit = 16f;
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
        pos.y = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;
        transform.position = pos;
    }
}
