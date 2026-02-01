using UnityEngine;
using Cinemachine;

public static class CameraUtility
{
   
    private static CinemachineVirtualCamera _cachedCamera;
    private static bool _isCached = false;

    public static CinemachineVirtualCamera GetActiveVirtualCamera()
    {
        // 캐시된 카메라가 유효한지 확인
        // (카메라가 null이 아니고, 활성화되어 있으면 그냥 반환)
        if (_isCached && _cachedCamera != null && _cachedCamera.enabled)
        {
            return _cachedCamera;
        }

        // 캐시가 없거나 유효하지 않으면 새로 찾기
        CinemachineVirtualCamera[] allCameras = Object.FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None);

        // 모든 VirtualCamera를 순회하면서 enabled된 것 찾기
        foreach (var camera in allCameras)
        {
            if (camera.enabled)
            {
                _cachedCamera = camera;
                _isCached = true;
                return _cachedCamera;
            }
        }

        // 활성화된 카메라를 못 찾았으면 에러 출력
        Debug.LogError("활성화된 VirtualCamera를 찾을 수 없습니다!");
        _isCached = false;
        return null;
    }

    // ===========================================
    // 2. 씬 전환 후 캐시 초기화 (필수!)
    // ===========================================
    /// <summary>
    /// 씬이 전환되면 이전 카메라 정보는 더 이상 유효하지 않음
    /// 따라서 캐시를 초기화해서 다음 번에 새로운 카메라를 찾도록 함
    /// 
    /// SceneSwapManager의 OnSceneLoaded에서 호출해야 함!
    /// 
    /// 사용 예:
    /// CameraUtility.InvalidateCache();
    /// </summary>
    public static void InvalidateCache()
    {
        _cachedCamera = null;
        _isCached = false;
        Debug.Log("[CameraUtility] 카메라 캐시 초기화됨");
    }


}