using UnityEngine;
using System.Collections;
using Cinemachine;

[System.Serializable]
public class PlatformData
{
    public GameObject platformObject;    // 발판의 GameObject
    public float lifetime = 3f;          // 발판이 유지되는 시간
    public float delayBeforeSpawn = 0f;  // 이전 발판 이후 생성 지연시간
}

public class PlatformGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PlatformData[] platforms = new PlatformData[0];

    [Header("Visual")]
    [SerializeField] private bool showDebugMessages = true;

    [Header("Camera Shake")]
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;
    [SerializeField] private bool shakeOn;

    private Coroutine platformCoroutine;
    private bool canTrigger = false;
    private bool isGenerating = false;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void Update()
    {
        if (canTrigger&&InputManager.UseToolWasPressed)
        {
            if (!isGenerating)
            {
                StartPlatformSequence();
            }
        }
    }

    public void StartPlatformSequence()
    {
        if (isGenerating) return;

        isGenerating = true;

        platformCoroutine = StartCoroutine(GeneratePlatformsSequence());

    }



    private IEnumerator GeneratePlatformsSequence()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            PlatformData platformData = platforms[i];

            // 지연시간만큼 대기
            yield return new WaitForSeconds(platformData.delayBeforeSpawn);
            DualSenseInput.Instance.Vibrate(0.15f, 0.15f, 0.07f);
            CameraEventManager.instance.CameraShakeEvent(profile, impulseSource);
            // 발판 활성화
            if (platformData.platformObject != null)
            {
                platformData.platformObject.SetActive(true);

 

                // 발판이 사라지는 것을 별도 코루틴으로 처리 
                StartCoroutine(DisablePlatformAfterDelay(platformData.platformObject, platformData.lifetime, i + 1));
            }
        }

        isGenerating = false;

    }

    private IEnumerator DisablePlatformAfterDelay(GameObject platform, float delay, int platformIndex)
    {
        yield return new WaitForSeconds(delay);

        platform.SetActive(false);

        if (showDebugMessages)
            Debug.Log($"발판 {platformIndex} 사라짐!");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canTrigger = false;
        }
    }
}