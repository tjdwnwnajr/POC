// This doesn't reflect the latest features in JoyShockLibrary 3.0. But this is a good starting point for filling in the new functions and structs found in JoyShockLibrary.h.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public static class JSL
{
    public const int ButtonMaskUp = 0;             // D-Pad ↑
    public const int ButtonMaskDown = 1;           // D-Pad ↓
    public const int ButtonMaskLeft = 2;           // D-Pad ←
    public const int ButtonMaskRight = 3;          // D-Pad →
    public const int ButtonMaskPlus = 4;           // PS4/5 “Options” 버튼 / Switch “Plus” 버튼
    public const int ButtonMaskOptions = 4;        // Options 버튼과 동일
    public const int ButtonMaskMinus = 5;          // PS4/5 “Share” 버튼 / Switch “Minus” 버튼
    public const int ButtonMaskShare = 5;          // Share 버튼과 동일
    public const int ButtonMaskLClick = 6;         // 왼쪽 스틱 클릭 (L3)
    public const int ButtonMaskRClick = 7;         // 오른쪽 스틱 클릭 (R3)
    public const int ButtonMaskL = 8;              // L1 버튼
    public const int ButtonMaskR = 9;              // R1 버튼
    public const int ButtonMaskZL = 10;            // L2 / ZL 트리거
    public const int ButtonMaskZR = 11;            // R2 / ZR 트리거
    public const int ButtonMaskS = 12;             // South 버튼 → DualSense: X (점프용)
    public const int ButtonMaskE = 13;             // East 버튼 → DualSense: ○ (서클)
    public const int ButtonMaskW = 14;             // West 버튼 → DualSense: □ (스퀘어)
    public const int ButtonMaskN = 15;             // North 버튼 → DualSense: △ (트라이앵글)
    public const int ButtonMaskHome = 16;          // PS 버튼 / Home 버튼
    public const int ButtonMaskPS = 16;            // PS 버튼과 동일
    public const int ButtonMaskCapture = 17;       // Capture / 터치패드 클릭
    public const int ButtonMaskTouchpadClick = 17; // 터치패드 클릭과 동일
    public const int ButtonMaskSL = 18;            // Joy-Con SL 버튼 (좌측)
    public const int ButtonMaskSR = 19;            // Joy-Con SR 버튼 (우측)

    [StructLayout(LayoutKind.Sequential)]
    public struct JOY_SHOCK_STATE
    {
        public int buttons;      // 현재 눌린 버튼들을 비트 단위로 저장
        public float lTrigger;   // 왼쪽 트리거(L2/ZL) 아날로그 값 (0~1)
        public float rTrigger;   // 오른쪽 트리거(R2/ZR) 아날로그 값 (0~1)
        public float stickLX;    // 왼쪽 스틱 X축 값 (-1~1)
        public float stickLY;    // 왼쪽 스틱 Y축 값 (-1~1)
        public float stickRX;    // 오른쪽 스틱 X축 값 (-1~1)
        public float stickRY;    // 오른쪽 스틱 Y축 값 (-1~1)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IMU_STATE
    {
        public float accelX;  // X축 가속도
        public float accelY;  // Y축 가속도
        public float accelZ;  // Z축 가속도
        public float gyroX;   // X축 자이로(회전) 값
        public float gyroY;   // Y축 자이로(회전) 값
        public float gyroZ;   // Z축 자이로(회전) 값
    }

    public delegate void EventCallback(int handle, JOY_SHOCK_STATE state, JOY_SHOCK_STATE lastState,
        IMU_STATE imuState, IMU_STATE lastImuState, float deltaTime);

    [DllImport("JoyShockLibrary")]
    public static extern int JslConnectDevices();
    [DllImport("JoyShockLibrary")]
    public static extern int JslGetConnectedDeviceHandles(int[] deviceHandleArray, int size);
    [DllImport("JoyShockLibrary")]
    public static extern void JslDisconnectAndDisposeAll();

    [DllImport("JoyShockLibrary", CallingConvention = CallingConvention.Cdecl)]
    public static extern JOY_SHOCK_STATE JslGetSimpleState(int deviceId);
    [DllImport("JoyShockLibrary", CallingConvention = CallingConvention.Cdecl)]
    public static extern IMU_STATE JslGetIMUState(int deviceId);

    [DllImport("JoyShockLibrary")]
    public static extern float JslGetStickStep(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern float JslGetTriggerStep(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern float JslGetPollRate(int deviceId);

    [DllImport("JoyShockLibrary")]
    public static extern void JslResetContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslStartContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslPauseContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslGetCalibrationOffset(int deviceId, ref float xOffset, ref float yOffset, ref float zOffset);
    [DllImport("JoyShockLibrary")]
    public static extern void JslGetCalibrationOffset(int deviceId, float xOffset, float yOffset, float zOffset);

    [DllImport("JoyShockLibrary")]
    public static extern void JslSetCallback(EventCallback callback);
    
    [DllImport("JoyShockLibrary")]
    public static extern int JslGetControllerType(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern int JslGetControllerSplitType(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern int JslGetControllerColour(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslSetLightColour(int deviceId, int colour);
    [DllImport("JoyShockLibrary")]
    public static extern void JslSetRumble(int deviceId, int smallRumble, int bigRumble);
    [DllImport("JoyShockLibrary")]
    public static extern void JslSetPlayerNumber(int deviceId, int number);
}