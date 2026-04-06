using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Speaker[] speakers;
    [SerializeField] private DialogData[] dialogs;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    //  nextDialogAction 지웠어요

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;

    public bool isDialogActive { get; private set; } = false;
    private int currentDialogIndex = -1;
    private int currentSpeakerIndex = 0;
    private bool isTypingEffect = false;
    private bool canInput = false;
    private bool isExitCooldown = false;

    private void Awake() => Setup();
    // OnEnable, OnDisable 지웠어요

    private void Setup()
    {
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
            if (speakers[i].spriteRenderer != null)
            {
                speakers[i].spriteRenderer.gameObject.SetActive(true);
                Color c = speakers[i].spriteRenderer.color;
                speakers[i].spriteRenderer.color = c;
            }
        }
    }

    public void StartDialog()
    {
        if (isDialogActive || isExitCooldown) return;

        isDialogActive = true;
        currentDialogIndex = -1;
        canInput = false;

        PlayerStateList.canMove = false;
        PlayerStateList.isView = true;
        InputManager.DeactivatePlayerControls();

        SetNextDialog();
        StartCoroutine(EnableInputDelay());
    }

    private IEnumerator EnableInputDelay()
    {
        yield return new WaitForSeconds(0.1f);
        canInput = true;
    }

    private void Update()
    {
        if (!isDialogActive || !canInput) return;

        bool nextPressed = Input.GetMouseButtonDown(0);

        // InputManager 대신 직접 게임패드에서 읽기
        if (Gamepad.current != null)
            nextPressed |= Gamepad.current.buttonWest.wasPressedThisFrame;

        if (nextPressed)
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
        else if (currentDialogIndex + 1 < dialogs.Length)
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
        if (currentSpeakerIndex < speakers.Length)
            SetActiveObjects(speakers[currentSpeakerIndex], false);

        currentDialogIndex++;
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        SetActiveObjects(speakers[currentSpeakerIndex], true);
        speakers[currentSpeakerIndex].textName.text = dialogs[currentDialogIndex].name;
        speakers[currentSpeakerIndex].textDialogue.text = "";

        StopCoroutine("OnTypingText");
        StartCoroutine("OnTypingText");
    }

    private void EndDialog()
    {
        isDialogActive = false;
        canInput = false;

        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
            if (speakers[i].spriteRenderer != null)
            {
                Color c = speakers[i].spriteRenderer.color;
                speakers[i].spriteRenderer.color = c;
            }
        }

        PlayerStateList.canMove = true;
        PlayerStateList.isView = false;
        InputManager.ActivatePlayerControls();

        StartCoroutine(ExitCooldownRoutine());
    }

    private IEnumerator ExitCooldownRoutine()
    {
        isExitCooldown = true;
        yield return new WaitForSeconds(0.5f);
        isExitCooldown = false;
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
        speaker.objectArrow.SetActive(false);

        if (speaker.spriteRenderer != null)
        {
            Color color = speaker.spriteRenderer.color;
            speaker.spriteRenderer.color = color;
        }
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;
        string fullText = dialogs[currentDialogIndex].dialogue;

        while (index <= fullText.Length)
        {
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