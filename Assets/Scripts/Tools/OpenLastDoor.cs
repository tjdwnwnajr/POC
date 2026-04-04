using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OpenLastDoor : TriggerInteractionBase
{

    [SerializeField] private SpriteRenderer doorRenderer;
    private bool isTriggered = false;

    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    public override void Interact()
    {
        if(PlayerStateList.keyReady&&!isTriggered)
        {
            //문 이미지 변화-
            isTriggered = true;
            CameraEventManager.instance.CameraShakeEvent(profile, impulseSource);
            StartCoroutine(VibrateTimes());
            

            if (doorRenderer !=null)
                StartCoroutine(doorOff());
            //문 들어가기
            StartCoroutine(BirghtOutAndTheEnd());
        }
        else if(PlayerStateList.keyReady == false )
        {
            isTriggered = true;
            //열쇠 없음 ui나오게

            isTriggered = false;
        }
    }
    private IEnumerator VibrateTimes()
    {
        DualSenseInput.Instance.Vibrate(0.1f, 0.05f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.1f, 0.1f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.15f, 0.15f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.2f, 0.2f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.5f, 0.4f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.6f, 0.6f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.2f, 0.2f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.1f, 0.1f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.05f, 0.1f, 0.3f);

    }
    private IEnumerator doorOff()
    {
        Color a = doorRenderer.color;
        float t = 0;
        while (t < 5f)
        {
            t += Time.deltaTime;
            float per = t / 5f;
            a.a = Mathf.Lerp(1f, 0f, per);

            doorRenderer.color = a;

            yield return null;
        }

        doorRenderer.color = new Color(a.r, a.g, a.b, 0f);
        
    }
    private IEnumerator BirghtOutAndTheEnd() {
        InputManager.DeactivatePlayerControls();
        yield return new WaitForSeconds(7f);
        
        SceneBrightManager.instance.StartBrightOut();

        //keep fading out
        while (SceneBrightManager.instance.IsBrightOut)
        {
            yield return null;
        }
    }
}
