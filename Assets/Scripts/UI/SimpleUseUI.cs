using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SimpleUseUI : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction useAction;
    private bool canClose = false;

    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
            useAction = playerInput.actions["Use"];
    }

    private void OnEnable()
    {
        PlayerStateList.canMove = false;
        PlayerStateList.isView = true;

        canClose = false;
        StartCoroutine(EnableCloseNextFrame());
    }

    private IEnumerator EnableCloseNextFrame()
    {
        yield return null;
        canClose = true;
    }

    private void Update()
    {
        if (!canClose) return;
        if (useAction == null) return;

        if (useAction.WasPressedThisFrame())
        {
            CloseUI();
        }
    }

    private void CloseUI()
    {
        PlayerStateList.canMove = true;
        PlayerStateList.isView = false;
        gameObject.SetActive(false);
    }
}