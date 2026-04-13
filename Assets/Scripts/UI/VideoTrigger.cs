using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class VideoTrigger : MonoBehaviour
{
    [SerializeField] private GameObject videoCanvas;
    [SerializeField] private GameObject videoPlayerObject; // VideoPlayer들이 붙어있는 오브젝트 하나만 연결
    [SerializeField] private int currentVideoIndex = 0;
    [SerializeField] private GameObject interactionUI;

    private PlayerController player;
    private VideoPlayer[] videoPlayers;
    private PlayerInput playerInput;
    private InputAction useAction;
    private bool isPlaying = false;
    private bool isPlayerInside = false;
    private bool hasAutoPlayedOnce = false;
    private bool pendingAutoPlay = false;

    private VideoPlayer CurrentVideo => videoPlayers[currentVideoIndex];

    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        useAction = playerInput.actions["Use"];

        // 연결한 오브젝트에서 자동으로 모든 VideoPlayer 가져옴
        videoPlayers = videoPlayerObject.GetComponents<VideoPlayer>();

        // 태그로 플레이어 찾아오기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.GetComponent<PlayerController>();
        else
            Debug.LogWarning("Player 태그를 가진 오브젝트를 찾을 수 없어요!");
    }

    private void Start()
    {
        if (videoCanvas != null)
            videoCanvas.SetActive(false);

        if (interactionUI != null)
            interactionUI.SetActive(false);

        // 모든 VideoPlayer 처음 상태로 초기화
        foreach (VideoPlayer vp in videoPlayers)
        {
            if (vp != null)
            {
                vp.Stop();
                vp.time = 0;
                vp.loopPointReached += OnVideoEnd;
            }
        }
    }

    private void Update()
    {
        if (player == null || videoPlayers == null || videoCanvas == null)
            return;

        if (isPlaying)
        {
            // 액션맵이 꺼져있어도 작동하도록 직접 게임패드에서 읽기
            bool skipPressed = false;

            if (Gamepad.current != null)
                skipPressed |= Gamepad.current.buttonWest.wasPressedThisFrame;

            skipPressed |= Input.GetMouseButtonDown(0);

            if (skipPressed)
                CloseVideo();

            return;
        }

        if (!hasAutoPlayedOnce && pendingAutoPlay && player.Grounded())
        {
            pendingAutoPlay = false;
            hasAutoPlayedOnce = true;
            PlayVideo();
            return;
        }

        if (hasAutoPlayedOnce && isPlayerInside)
        {
            if (interactionUI != null)
                interactionUI.SetActive(true);

            bool interactPressed = false;

            if (Gamepad.current != null)
                interactPressed |= Gamepad.current.buttonWest.wasPressedThisFrame;

            interactPressed |= Input.GetMouseButtonDown(0);

            if (interactPressed)
            {
                if (interactionUI != null)
                    interactionUI.SetActive(false);
                PlayVideo();
            }
        }
        else
        {
            if (interactionUI != null)
                interactionUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = true;

        if (!hasAutoPlayedOnce)
            pendingAutoPlay = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = false;

        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    public void PlayVideo()
    {
        if (currentVideoIndex < 0 || currentVideoIndex >= videoPlayers.Length)
        {
            Debug.LogWarning("VideoIndex가 범위를 벗어났어요!");
            return;
        }

        Debug.Log($"재생되는 VideoPlayer : {CurrentVideo.name}, 클립 : {CurrentVideo.clip}");

        isPlaying = true;

        if (videoCanvas != null)
            videoCanvas.SetActive(true);

        if (interactionUI != null)
            interactionUI.SetActive(false);

        PlayerStateList.canMove = false;
        PlayerStateList.isView = true;

        // 처음부터 재생
        CurrentVideo.Stop();
        CurrentVideo.time = 0;
        CurrentVideo.Play();
    }

    public void CloseVideo()
    {
        isPlaying = false;
        CurrentVideo.Stop();

        if (videoCanvas != null)
            videoCanvas.SetActive(false);

        PlayerStateList.canMove = true;
        PlayerStateList.isView = false;
    }

    public void SetVideoIndex(int index)
    {
        if (index < 0 || index >= videoPlayers.Length)
        {
            Debug.LogWarning("VideoIndex가 범위를 벗어났어요!");
            return;
        }
        currentVideoIndex = index;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (vp == CurrentVideo)
        {
            CloseVideo();
        }
    }
}