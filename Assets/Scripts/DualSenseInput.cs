using System;
using UnityEngine;
using System.Collections;

public class DualSenseInput : MonoBehaviour
{
    public static DualSenseInput Instance;

    private int[] deviceHandles;
    private int controllerCount = 0;

    // =========================
    // Axis (GetAxis)
    // =========================
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    // =========================
    // Button (GetButton)
    // =========================
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    // =========================
    // ButtonDown / ButtonUp
    // =========================
    public bool JumpDown { get; private set; }
    public bool JumpUp { get; private set; }

    public bool DashDown { get; private set; }
    public bool DashUp { get; private set; }

    public bool AttackDown { get; private set; }
    public bool AttackUp { get; private set; }

    // 이전 프레임 상태
    private bool prevJump;
    private bool prevDash;
    private bool prevAttack;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        controllerCount = JSL.JslConnectDevices();

        if (controllerCount > 0)
        {
            deviceHandles = new int[controllerCount];
            JSL.JslGetConnectedDeviceHandles(deviceHandles, controllerCount);
            Debug.Log($"DualSense 연결됨: {controllerCount}개");
        }
        else
        {
            Debug.Log("DualSense 컨트롤러 없음");
        }
    }

    private void Update()
    {
        // 컨트롤러 없을 때 입력 초기화
        if (controllerCount == 0)
        {
            Horizontal = 0;
            Vertical = 0;

            JumpPressed = DashPressed = AttackPressed = false;
            JumpDown = DashDown = AttackDown = false;
            JumpUp = DashUp = AttackUp = false;

            prevJump = prevDash = prevAttack = false;
            return;
        }

        // 첫 번째 컨트롤러 상태 읽기
        JSL.JOY_SHOCK_STATE state = JSL.JslGetSimpleState(deviceHandles[0]);

        // =========================
        // Axis
        // =========================
        Horizontal = state.stickLX;
        Vertical = state.stickLY;

        // =========================
        // Raw Button (Pressed)
        // =========================
        bool jump = (state.buttons & (1 << JSL.ButtonMaskS)) != 0; // X
        bool dash = (state.buttons & (1 << JSL.ButtonMaskE)) != 0; // □
        bool attack = (state.buttons & (1 << JSL.ButtonMaskW)) != 0; // ○

        JumpPressed = jump;
        DashPressed = dash;
        AttackPressed = attack;

        // =========================
        // ButtonDown / ButtonUp
        // =========================
        JumpDown = jump && !prevJump;
        JumpUp = !jump && prevJump;

        DashDown = dash && !prevDash;
        DashUp = !dash && prevDash;

        AttackDown = attack && !prevAttack;
        AttackUp = !attack && prevAttack;

        // 이전 상태 저장
        prevJump = jump;
        prevDash = dash;
        prevAttack = attack;
    }

    // =========================
    // Vibration
    // =========================
    public void Vibrate(float lowFreq, float highFreq, float duration)
    {
        if (controllerCount == 0) return;

        int small = Mathf.Clamp((int)(lowFreq * 255), 0, 255);
        int big = Mathf.Clamp((int)(highFreq * 255), 0, 255);

        StartCoroutine(RumbleCoroutine(small, big, duration));
    }

    private IEnumerator RumbleCoroutine(int small, int big, float duration)
    {
        JSL.JslSetRumble(deviceHandles[0], small, big);
        yield return new WaitForSeconds(duration);
        JSL.JslSetRumble(deviceHandles[0], 0, 0);
    }
}
