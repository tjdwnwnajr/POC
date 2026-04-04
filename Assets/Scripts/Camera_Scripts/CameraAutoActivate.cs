using UnityEngine;
using Cinemachine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _camera;
    private CinemachineVirtualCamera[] _allCameras;

    private void Start()
    {
        _allCameras = CameraManager.Instance._allVirtualCameras;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (var cam in _allCameras)
                cam.enabled = false;

            _camera.enabled = true;
            //카메라 초기화
            CameraUtility.InvalidateCache();

            //카메라 다시찾아서 흔들기위한 리스너 연결하기
            //CameraShakeManager.instance.FindAndSetupVirtualCamera();
            CameraShakeManager.instance.RefreshCamera();

            //카메라 event offset 을 위한 카메라 다시 찾기
            CameraEventFocus.instance.RefreshCamera();
        }
    }
}