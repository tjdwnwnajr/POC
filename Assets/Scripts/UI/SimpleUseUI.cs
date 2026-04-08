using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleUseUI : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction useAction;

    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        useAction = playerInput.actions["Use"];
    }

    private void OnEnable()
    {
        PlayerStateList.canMove = false;
        PlayerStateList.isView = true;
    }

    private void Update()
    {
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