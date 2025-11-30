using System;
using UnityEngine;
using System.Collections;

public class DualSenseInput : MonoBehaviour
{
    public static DualSenseInput Instance; // 어디서나 접근 가능

    private int[] deviceHandles;
    private int controllerCount = 0;

    // 스틱 값
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    // 버튼 상태
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject); // 씬 전환에도 유지

        // 컨트롤러 연결
        controllerCount = JSL.JslConnectDevices();
        if (controllerCount > 0)
        {
            deviceHandles = new int[controllerCount];
            JSL.JslGetConnectedDeviceHandles(deviceHandles, controllerCount);
            Debug.Log("DualSense 연결됨: " + controllerCount + "개");
        }
        else
        {
            Debug.Log("DualSense 컨트롤러 없음");
        }
    }

    private void Update()
    {
        if (controllerCount == 0)
        {
            // 연결 안되면 초기화
            Horizontal = 0;
            Vertical = 0;
            JumpPressed = false;
            DashPressed = false;
            AttackPressed = false;
            return;
        }

        // 첫 번째 컨트롤러 상태 읽기
        JSL.JOY_SHOCK_STATE state = JSL.JslGetSimpleState(deviceHandles[0]);

        // 아날로그 스틱
        Horizontal = state.stickLX;
        Vertical = state.stickLY;

        // 버튼 상태 (비트 연산)
        JumpPressed = (state.buttons & (1 << JSL.ButtonMaskS)) != 0;    // X 버튼 → Jump
        DashPressed = (state.buttons & (1 << JSL.ButtonMaskE)) != 0;    // □ 버튼 → Dash
        AttackPressed = (state.buttons & (1 << JSL.ButtonMaskW)) != 0;  // ○ 버튼 → Attack
    }

    public void Vibrate(float lowFreq, float highFreq, float duration)
    {
        if (controllerCount == 0) return;

        int small = Mathf.Clamp((int)(lowFreq * 255), 0, 255);
        int big = Mathf.Clamp((int)(highFreq * 255), 0, 255);

        StartCoroutine(RumbleCoroutine(small, big, duration));
    }

    private IEnumerator RumbleCoroutine(int small, int big, float duration)
    {
        // 시작 진동
        JSL.JslSetRumble(deviceHandles[0], small, big);

        // duration 초 동안 유지
        yield return new WaitForSeconds(duration);

        // 진동 끄기
        JSL.JslSetRumble(deviceHandles[0], 0, 0);
    }

}
