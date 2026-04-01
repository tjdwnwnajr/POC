using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class VideoTrigger : MonoBehaviour
{
    public GameObject videoCanvas;    // VideoCanvas 오브젝트 연결
    public VideoPlayer videoPlayer;   // Video Player 오브젝트 연결
    public Transform player;          // 플레이어 Transform 연결 (거리 체크용)
    public MonoBehaviour playerMovementScript; // 플레이어 이동 스크립트를 여기에 드래그 (예: PlayerController)

    public float interactRange = 2.5f;
    private bool isPlaying = false;    // 현재 영상 재생 중인지 확인

    private void Start()
    {
        if (videoCanvas != null) videoCanvas.SetActive(false);

        // 영상이 끝까지 재생되었을 때 자동으로 닫히는 기능 연결
        if (videoPlayer != null)
            videoPlayer.loopPointReached += (vp) => CloseVideo();
    }

    void Update()
    {
        if (player == null || videoCanvas == null) return;

        // 1. 영상이 재생 중일 때 닫기 입력 체크
        if (isPlaying)
        {
            if ((Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame) ||
                Input.GetMouseButtonDown(1))
            {
                CloseVideo();
            }
            return; // 영상 재생 중엔 아래 거리 체크 로직 실행 안 함
        }

        // 2. 영상 재생 전 거리 체크 및 시작 입력
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= interactRange)
        {
            if ((Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame) ||
                Input.GetMouseButtonDown(1))
            {
                PlayVideo();
            }
        }
    }

    public void PlayVideo()
    {
        isPlaying = true;
        videoCanvas.SetActive(true);
        videoPlayer.Play();

        // 플레이어 이동 스크립트 비활성화
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;
    }

    public void CloseVideo()
    {
        isPlaying = false;
        videoPlayer.Stop();
        videoCanvas.SetActive(false);

        // 플레이어 이동 스크립트 다시 활성화
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
    }
}