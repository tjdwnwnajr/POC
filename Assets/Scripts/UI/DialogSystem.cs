using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // Unity 6 New Input System

public class DialogSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Speaker[] speakers;
    [SerializeField] private DialogData[] dialogs;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    // New Input System Action (패드 서쪽 버튼 & 마우스 좌클릭 바인딩용)
    [SerializeField] private InputActionReference nextDialogAction;

    private int currentDialogIndex = -1;
    private int currentSpeakerIndex = 0;
    private bool isTypingEffect = false;
    private bool isDialogActive = false;

    private void Awake()
    {
        Setup();
    }

    private void OnEnable() => nextDialogAction.action.Enable();
    private void OnDisable() => nextDialogAction.action.Disable();

    private void Setup()
    {
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
            //speakers[i].spriteRenderer.gameObject.SetActive(false);
        }
    }

    private bool canNext = false; // 대화 시작 직후 입력을 방지하기 위한 변수

    // 이 함수가 public이어야 DialogNPC에서 호출 가능합니다!
    public void StartDialog()
    {
        if (isDialogActive) return;

        isDialogActive = true;
        currentDialogIndex = -1;
        canNext = false; // 시작 시 입력 잠금

        for (int i = 0; i < speakers.Length; ++i)
            speakers[i].spriteRenderer.gameObject.SetActive(true);

        SetNextDialog();

        // 0.1초 뒤에 입력을 받을 수 있게 함 (프레임 겹침 방지)
        StartCoroutine(EnableInputDelay());
    }

    private IEnumerator EnableInputDelay()
    {
        yield return new WaitForSeconds(0.1f);
        canNext = true;
    }

    private void Update()
    {
        if (!isDialogActive || !canNext) return; // canNext가 true일 때만 입력 처리

        if (Input.GetMouseButtonDown(0) || nextDialogAction.action.WasPressedThisFrame())
        {
            HandleNextInput();
        }
    }

    private void HandleNextInput()
    {
        if (isTypingEffect)
        {
            isTypingEffect = false;
            StopCoroutine("OnTypingText");
            speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue;
            speakers[currentSpeakerIndex].objectArrow.SetActive(true);
        }
        else if (dialogs.Length > currentDialogIndex + 1)
        {
            SetNextDialog();
        }
        else
        {
            EndDialog();
        }
    }

    private void SetNextDialog()
    {
        // 이전 화자 UI 끄기
        if (currentSpeakerIndex < speakers.Length)
            SetActiveObjects(speakers[currentSpeakerIndex], false);

        currentDialogIndex++;
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        // 새 화자 UI 켜기
        SetActiveObjects(speakers[currentSpeakerIndex], true);
        speakers[currentSpeakerIndex].textName.text = dialogs[currentDialogIndex].name;

        // ★ 중요: 텍스트를 확실히 비우고 코루틴 시작
        speakers[currentSpeakerIndex].textDialogue.text = "";

        StopAllCoroutines(); // 기존 돌아가던 타이핑 코루틴 중복 방지
        StartCoroutine(OnTypingText());
        StartCoroutine(EnableInputDelay()); // 대사마다 아주 짧은 딜레이를 주면 연타 스킵 방지 가능
    }

    private void EndDialog()
    {
        isDialogActive = false;
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
            //speakers[i].spriteRenderer.gameObject.SetActive(false);
        }
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
        speaker.objectArrow.SetActive(false);

        Color color = speaker.spriteRenderer.color;
        speaker.spriteRenderer.color = color;
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;
        string fullText = dialogs[currentDialogIndex].dialogue;

        while (index <= fullText.Length)
        {
            // isTypingEffect가 false가 되면(HandleNextInput에서 강제 종료 시) 루프 탈출
            if (!isTypingEffect) break;

            speakers[currentSpeakerIndex].textDialogue.text = fullText.Substring(0, index);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;
        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
    }
}

[System.Serializable]
public struct Speaker
{
    public SpriteRenderer spriteRenderer;
    public Image imageDialog;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDialogue;
    public GameObject objectArrow;
}

[System.Serializable]
public struct DialogData
{
    public int speakerIndex;
    public string name;
    [TextArea(3, 5)] public string dialogue;
}