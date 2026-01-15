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

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _ropeAction;
    private InputAction _rotateAction;
    private InputAction _checkAction;
    private InputAction _attackAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _ropeAction = PlayerInput.actions["Rope"];
        _rotateAction = PlayerInput.actions["Rotate"];
        _checkAction = PlayerInput.actions["Check"];
        _attackAction = PlayerInput.actions["Attack"];
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
    }
}
