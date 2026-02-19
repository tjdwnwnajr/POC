using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections;

public class ButtonCinematic : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Platform Parents")]
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject down;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform cameraZone;   // BoxCollider2D 있는 영역 오브젝트
    [SerializeField] private float cameraMoveTime = 1f;
    [SerializeField] private float extraMargin = 1.1f; // 1.1 = 10% 여백

    private float originalZoom;
    private Transform originalFollow;

    private bool playerInRange;
    private bool busy;

    private void Start()
    {
        originalZoom = vcam.m_Lens.OrthographicSize;
    }

    private void OnEnable()
    {
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange || busy) return;
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        busy = true;

        // 🔒 플레이어 잠금
        PlayerStateList.isView = true;
        PlayerStateList.canMove = false;

        // 🎥 카메라 영역 이동
        originalFollow = vcam.Follow;
        vcam.Follow = null;

        yield return StartCoroutine(MoveCameraToZone());

        bool isUpActive = up.activeSelf;

        if (isUpActive)
        {
            yield return StartCoroutine(FadeChildren(up, 1f, 0f));
            up.SetActive(false);

            down.SetActive(true);
            SetChildrenAlpha(down, 0f);
            yield return StartCoroutine(FadeChildren(down, 0f, 1f));
        }
        else
        {
            yield return StartCoroutine(FadeChildren(down, 1f, 0f));
            down.SetActive(false);

            up.SetActive(true);
            SetChildrenAlpha(up, 0f);
            yield return StartCoroutine(FadeChildren(up, 0f, 1f));
        }

        yield return new WaitForSeconds(0.3f);

        // 🎥 카메라 복귀
        yield return StartCoroutine(ReturnCamera());

        // 🔓 플레이어 해제
        PlayerStateList.isView = false;
        PlayerStateList.canMove = true;

        busy = false;
    }

    // =========================
    // 카메라 영역 이동 + 자동 줌아웃
    // =========================
    IEnumerator MoveCameraToZone()
    {
        BoxCollider2D zoneCollider = cameraZone.GetComponent<BoxCollider2D>();
        Bounds bounds = zoneCollider.bounds;

        Vector3 targetPos = new Vector3(
            bounds.center.x,
            bounds.center.y,
            vcam.transform.position.z
        );

        float verticalSize = bounds.size.y * 0.5f;
        float horizontalSize = bounds.size.x * 0.5f / vcam.m_Lens.Aspect;

        float requiredSize = Mathf.Max(verticalSize, horizontalSize) * extraMargin;

        Vector3 startPos = vcam.transform.position;
        float startZoom = vcam.m_Lens.OrthographicSize;

        float timer = 0f;

        while (timer < cameraMoveTime)
        {
            timer += Time.deltaTime;
            float t = timer / cameraMoveTime;

            vcam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, requiredSize, t);

            yield return null;
        }
    }

    // =========================
    // 카메라 복귀
    // =========================
    IEnumerator ReturnCamera()
    {
        Vector3 startPos = vcam.transform.position;
        float startZoom = vcam.m_Lens.OrthographicSize;

        Vector3 targetPos = new Vector3(
            PlayerController.Instance.transform.position.x,
            PlayerController.Instance.transform.position.y,
            startPos.z
        );

        float timer = 0f;

        while (timer < cameraMoveTime)
        {
            timer += Time.deltaTime;
            float t = timer / cameraMoveTime;

            vcam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, originalZoom, t);

            yield return null;
        }

        vcam.Follow = originalFollow;
    }

    // =========================
    // 자식 전체 Fade
    // =========================
    IEnumerator FadeChildren(GameObject parent, float start, float end)
    {
        SpriteRenderer[] renderers = parent.GetComponentsInChildren<SpriteRenderer>();

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            float alpha = Mathf.Lerp(start, end, t);

            foreach (var sr in renderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            yield return null;
        }

        foreach (var sr in renderers)
        {
            Color c = sr.color;
            c.a = end;
            sr.color = c;
        }
    }

    void SetChildrenAlpha(GameObject parent, float alpha)
    {
        SpriteRenderer[] renderers = parent.GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in renderers)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
