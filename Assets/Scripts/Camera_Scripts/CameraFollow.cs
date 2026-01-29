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

    [Header("Virtual Camera")]
    public CinemachineVirtualCamera cam;
    [Header("Offset Settings")]
    private CinemachineCameraOffset offset;
    private Vector3 defaultOffset;
    private bool offsetMode = false;
    private Coroutine moveCoroutine;
    private float lookbtnCount;


    void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        offset = cam.GetComponent<CinemachineCameraOffset>();
        //cam.m_Lens.OrthographicSize = startSize;
        defaultOffset = offset.m_Offset;

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerStateList.isView)
        {
            if (InputManager.LookWasPressed)
            {
                lookbtnCount = 0;
                offsetMode = false;
            }
            if (InputManager.LookIsHeld && !offsetMode)
            {
                lookbtnCount += Time.deltaTime;
                if (lookbtnCount > 1.0f)
                {
                    offsetMode = true;
                    LookOffset(InputManager.LookDirection);
                }
            }
         
            if (InputManager.LookWasReleased)
            {
                lookbtnCount = 0;
                offsetMode = false;
                LookOffset(defaultOffset);
            }
        }

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

    private void LookOffset(Vector2 direction)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        Vector3 d = new Vector3(direction.x*5, direction.y*8, 0f);
        moveCoroutine = StartCoroutine(MoveOffset(d));
    }

    private IEnumerator MoveOffset(Vector3 targetOffset)
    {
        Vector3 startOffset = offset.m_Offset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            offset.m_Offset =
                Vector3.Lerp(startOffset, targetOffset, t);
            yield return null;
        }

        offset.m_Offset = targetOffset;
    }
}
