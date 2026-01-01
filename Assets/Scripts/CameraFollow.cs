using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Zoom Values")]
    [SerializeField] private float startSize = 6f;
    [SerializeField] private float slowZoomTarget = 12f;
    [SerializeField] private float maxSize = 30f;

    [Header("Timing")]
    [SerializeField] private float slowZoomTime = 0.3f;
    [SerializeField] private float burstZoomTime = 0.15f;
    [SerializeField] private float zoominTime = 0.3f;
    private bool isZooming;

    private CinemachineVirtualCamera cam;



    void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        cam.m_Lens.OrthographicSize = startSize;



    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
  
    public void OnZoomIn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isZooming)
        {
            StartCoroutine(ZoomInSequence());
        }
    }
    public void OnZoomOut(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isZooming)
        {
            StartCoroutine(ZoomOutSequence());
        }
    }

    IEnumerator ZoomOutSequence()
    {
        isZooming = true;

        float t = 0f;
        float from = cam.m_Lens.OrthographicSize;

        while (t < slowZoomTime)
        {
            t += Time.deltaTime;
            cam.m_Lens.OrthographicSize = Mathf.Lerp(from, slowZoomTarget, t / slowZoomTime);
            yield return null;
        }

        t = 0f;
        from = cam.m_Lens.OrthographicSize;

        while (t < burstZoomTime)
        {
            t += Time.deltaTime;
            cam.m_Lens.OrthographicSize = Mathf.Lerp(from, maxSize, t / burstZoomTime);
            yield return null;
        }

        cam.m_Lens.OrthographicSize = maxSize;
        isZooming = false;
    }

    IEnumerator ZoomInSequence()
    {
        isZooming = true;

        float t = 0f;
        float from = cam.m_Lens.OrthographicSize;

        while (t < zoominTime)
        {
            t += Time.deltaTime;
            cam.m_Lens.OrthographicSize = Mathf.Lerp(from, startSize, t / zoominTime);
            yield return null;
        }

        isZooming = false;
    }
}
