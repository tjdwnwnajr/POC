using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 게임 플레이타임 추적 및 UI 표시
///
/// 타이머 시작 조건:
///   - Inspector에서 지정한 씬에 진입했을 때부터 시작
///
/// 타이머 멈추는 조건:
///   - 사망했을 때       (PlayerStateList.isDead)
///   - 메뉴 열었을 때    (Time.timeScale == 0 && 지도는 열리지 않은 상태)
///   - 대화/설명창 열림  (PlayerStateList.isDialogue)
///
/// 타이머 계속 흐르는 조건:
///   - 지도(미니맵) 열었을 때
///   - 카메라 이벤트 / 블록 이벤트 중
///   - 그 외 모든 상황
/// </summary>
public class PlaytimeManager : MonoBehaviour
{
    public static PlaytimeManager instance;
    [Header("Key UI")]
    [SerializeField] private GameObject keyoneObject;
    [SerializeField] private GameObject keytwoObject;
    [SerializeField] private Sprite keyoneSprite;
    [SerializeField] private Sprite keytwoSprite;
    

    [Header("Death Count UI")]
    [SerializeField] private GameObject deathFaceObject;
    [SerializeField] private TMP_Text deathCountText;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    [Header("Start Scene Settings")]
    [Tooltip("이 씬 이름들 중 하나에 진입하면 타이머가 시작됩니다")]
    [SerializeField] private string[] startSceneNames;

    [Header("Settings")]
    [Tooltip("씬 전환 후에도 타이머를 유지할지 여부")]
    [SerializeField] private bool persistAcrossScenes = true;

    [Tooltip("표시 형식: true = HH:MM:SS / false = MM:SS")]
    [SerializeField] private bool showHours = true;

    private float _elapsedSeconds = 0f;
    private bool _timerStarted = false;

    [HideInInspector]public float ElapsedSeconds => _elapsedSeconds;
    [HideInInspector] public bool TimerStarted => _timerStarted;

    public float addtime = 10f;
    private void Awake()
    {
        if(instance ==null){
            instance = this;
        }
        //if (instance != null && instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //instance = this;

        //if (persistAcrossScenes)
        //    DontDestroyOnLoad(gameObject);
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        if (deathFaceObject != null)
            deathFaceObject.SetActive(false);
        if (deathCountText != null)
            deathCountText.gameObject.SetActive(false);
        if(keyoneObject != null)
            keyoneObject.SetActive(false);
        if(keytwoObject != null)
            keytwoObject.SetActive(false);

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 이미 시작됐으면 체크 불필요
        if (_timerStarted) return;
        
        foreach (string sceneName in startSceneNames)
        {
            if (scene.name == sceneName)
            {
                _timerStarted = true;
                //Debug.Log($"[PlaytimeManager] 씬 '{scene.name}' 진입 - 타이머 시작");
                if (timerText != null)
                    timerText.gameObject.SetActive(true);
                if (deathFaceObject != null)
                    deathFaceObject.SetActive(true);
                if (deathCountText != null)
                    deathCountText.gameObject.SetActive(true);
                if(keyoneObject != null)
                    keyoneObject.SetActive(true);
                if(keytwoObject != null)
                    keytwoObject.SetActive(true);
                
                break;
            }
        }
    }

    private void Update()
    {
        if (!_timerStarted) return;
        if (ShouldPause()) return;

        _elapsedSeconds += Time.unscaledDeltaTime;
        UpdateUI();
    }

    private bool ShouldPause()
    {
        // 1. 사망했을 때
        if (PlayerStateList.isDead)
            return true;

        // 2. 대화/설명창이 열렸을 때
        if (PlayerStateList.isDialogue)
            return true;

        // 3. 메뉴(일시정지)가 열렸을 때
        //    지도는 timeScale=0이지만 타이머를 멈추지 않으므로 구분
        bool isMenuPaused = Time.timeScale == 0f &&
                            !(MapManager.instance != null && MapManager.instance.IsLargeMapOpen);
        if (isMenuPaused)
            return true;

        return false;
    }

    private void UpdateUI()
    {
        if (timerText == null) return;
        timerText.text = FormatTime(_elapsedSeconds);
        if (deathCountText != null)
            deathCountText.text = $"x {PlayerStateList.deathCount}";
        if (keyoneObject != null)
        {
            if (PlayerStateList.firstKeyFounded)
                keyoneObject.GetComponent<Image>().sprite = keyoneSprite;
        }
        if (keytwoObject != null)
        {
            if (PlayerStateList.secondKeyFounded)
                keytwoObject.GetComponent<Image>().sprite = keytwoSprite;
        }
       
    }

    private string FormatTime(float totalSeconds)
    {
        int hours = (int)(totalSeconds / 3600f);
        int minutes = (int)((totalSeconds % 3600f) / 60f);
        int seconds = (int)(totalSeconds % 60f);

        return showHours
            ? $"{hours:D2}:{minutes:D2}:{seconds:D2}"
            : $"{minutes:D2}:{seconds:D2}";
    }

    // -----------------------------------------------
    // 공개 메서드
    // -----------------------------------------------

    /// <summary>타이머를 강제로 시작합니다.</summary>
    public void StartTimer()
    {
        _timerStarted = true;
    }

    /// <summary>타이머를 0으로 초기화하고 정지합니다.</summary>
    public void ResetTimer()
    {
        _elapsedSeconds = 0f;
        _timerStarted = false;
        UpdateUI();
    }

    /// <summary>저장된 플레이타임을 불러올 때 사용합니다.</summary>
    public void SetTime(float seconds)
    {
        _elapsedSeconds = Mathf.Max(0f, seconds);
        UpdateUI();
    }
    public void AddTime(float seconds)
    {
        _elapsedSeconds = Mathf.Max(0f, _elapsedSeconds + seconds);
        UpdateUI();
    }

    /// <summary>현재 플레이타임을 문자열로 반환합니다.</summary>
    public string GetFormattedTime() => FormatTime(_elapsedSeconds);
}