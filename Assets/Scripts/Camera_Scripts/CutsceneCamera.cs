using Cinemachine;
using System.Collections;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineCameraOffset offset;
    [Header("OffSet Settings")]
    [SerializeField] private float[] xOffset;
    [SerializeField] private float[] yOffset;
    [Header("Size Settings")]
    [SerializeField] private float[] size;
    [SerializeField] private float[] duration;
    private int index = 0;
    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        offset = virtualCamera.GetComponent<CinemachineCameraOffset>();
    }
    public void CutSceneCameraZoom()
    {
        StartCoroutine(CutSceneZoom());
    }
    IEnumerator CutSceneZoom()
    {
        if (index >= duration.Length)
        {
            yield break;
        }
        float elapsedTime = 0f;
        float startSize = virtualCamera.m_Lens.OrthographicSize;
        Vector3 startOffset = offset.m_Offset;
        Vector3 targetOffset = new Vector3(xOffset[index], yOffset[index], 0f);
        
        while (elapsedTime < duration[index])
        {

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration[index];
            // Smoothly interpolate the camera size and offset
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, size[index], t);
            offset.m_Offset = Vector3.Lerp(startOffset, targetOffset, t);
            yield return null;
        }
        // Ensure the final values are set
        virtualCamera.m_Lens.OrthographicSize = size[index];
        offset.m_Offset = targetOffset;
        index++;
    }
}
