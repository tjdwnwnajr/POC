using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{

    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;

    //Rope
    public static bool RopeWasPressed;
    public static bool RopeIsHeld;
    public static bool RopeWasReleased;
    //Rotate
    public static float Rotate;

    //Check
    public static bool CheckIsHeld;
    //Attack
    public static bool AttackWasPressed;

    //Look
    public static bool LookWasPressed;
    public static bool LookIsHeld;
    public static bool LookWasReleased;
    public static Vector2 LookDirection;
    //UseTools
    public static bool UseToolWasPressed;
    //RopeUpandDown
    public static bool UpWasPressed;
    public static bool DownWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _ropeAction;
    private InputAction _rotateAction;
    private InputAction _checkAction;
    private InputAction _attackAction;
    private InputAction _lookAction;
    private InputAction _useTool;
    private InputAction _RopeUpAction;
    private InputAction _RopeDownAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(PlayerInput != null)
        {
            return;
        }
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _ropeAction = PlayerInput.actions["Rope"];
        _rotateAction = PlayerInput.actions["Rotate"];
        _checkAction = PlayerInput.actions["Check"];
        _attackAction = PlayerInput.actions["Attack"];
        _lookAction = PlayerInput.actions["Look"];
        _useTool = PlayerInput.actions["Use"];
        _RopeUpAction = PlayerInput.actions["RopeUp"];
        _RopeDownAction = PlayerInput.actions["RopeDown"];
    }

    // Update is called once per frame
    void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction.IsPressed();

        RopeWasPressed = _ropeAction.WasPressedThisFrame();
        RopeIsHeld = _ropeAction.IsPressed();
        RopeWasReleased = _ropeAction.WasReleasedThisFrame();

        Rotate = _rotateAction.ReadValue<float>();

        CheckIsHeld = _checkAction.IsPressed();

        AttackWasPressed = _attackAction.WasPressedThisFrame();

        LookDirection = _lookAction.ReadValue<Vector2>();
        LookWasPressed = _lookAction.WasPressedThisFrame();
        LookIsHeld = _lookAction.IsPressed();
        LookWasReleased = _lookAction.WasReleasedThisFrame();

        UseToolWasPressed = _useTool.WasPressedThisFrame();

        UpWasPressed = _RopeUpAction.WasPressedThisFrame();
        DownWasPressed = _RopeDownAction.WasPressedThisFrame();

        if(PlayerStateList.isMirror)
        {
            Movement.x *= -1;
            Rotate *= -1;
        }
    }
    public static void ActivatePlayerControls()
    {
        PlayerInput.currentActionMap.Enable();
    }

    public static void DeactivatePlayerControls()
    {
        PlayerInput.currentActionMap.Disable();
    }
}
