using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector director;

    [Header("Settings")]
    [SerializeField] private float delayBeforePlay = 0.3f;  // 씬 로드 직후 약간의 딜레이

    private void Awake()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // 씬 로드 직후 물리/카메라 안정화 대기
        yield return new WaitForSeconds(delayBeforePlay);

        // 입력 비활성화 (CutsceneWalker.Start()에서도 하지만 이중 보호)
        InputManager.DeactivatePlayerControls();

        // 타임라인 재생
        director.Play();
    }
}