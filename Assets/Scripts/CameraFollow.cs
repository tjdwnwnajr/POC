using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;

    [SerializeField] private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Zoom Values")]
    [SerializeField] private float startSize = 6f;
    [SerializeField] private float slowZoomTarget = 12f;
    [SerializeField] private float maxSize = 30f;

    [Header("Timing")]
    [SerializeField] private float slowZoomTime = 0.3f;
    [SerializeField] private float burstZoomTime = 0.15f;
    private bool isZooming;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = startSize;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void LateUpdate()
    {
        
        transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + offset, followSpeed);
    }
    public void OnZoom(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isZooming)
        {
            StartCoroutine(ZoomSequence());
        }
    }

    IEnumerator ZoomSequence()
    {
        isZooming = true;

        float t = 0f;
        float from = cam.orthographicSize;

        while (t < slowZoomTime)
        {
            t += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(from, slowZoomTarget, t / slowZoomTime);
            yield return null;
        }

        t = 0f;
        from = cam.orthographicSize;

        while (t < burstZoomTime)
        {
            t += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(from, maxSize, t / burstZoomTime);
            yield return null;
        }

        cam.orthographicSize = maxSize;
        isZooming = false;
    }
}
