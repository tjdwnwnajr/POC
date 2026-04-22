using UnityEngine;
using UnityEngine.Playables;
using TMPro;

/// <summary>
/// 타임라인 재생 중 스킵 버튼을 누르면
/// 현재 시간 이후 가장 가까운 skipTimes 값으로 점프
///
/// [skipTimes 설정 예시]
/// [3.9, 8.4, 12.9]
/// → 0~3.9초 구간 스킵 → 3.9로 점프
/// → 3.9~8.4초 구간 스킵 → 8.4로 점프
/// → 마지막 값은 씬전환 시그널 바로 앞 시간
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class TimelineSkipper : MonoBehaviour
{
    [Header("Skip Points (오름차순 입력)")]
    [Tooltip("스킵 시 점프할 시간(초) 목록")]
    [SerializeField] private double[] skipTimes;

    [Header("TypeWriter (선택)")]
    [SerializeField] private TypeWriterFade typeWriter;

    [Header("Walk Cutscene (선택)")]
    [SerializeField] private CutsceneWalker walker;

    [Header("Buble Cutscene (선택)")]
    [SerializeField] private CutsceneBubble bubble;

    [SerializeField] private bool isCutSceneCanInput = false;

    [System.Serializable]
    public struct SkipEntry
    {
        [Tooltip("skipTimes 배열 인덱스 (0부터 시작)")]
        public int skipTimeIndex;
        [Tooltip("해당 포인트에서 캐릭터가 위치할 곳")]
        public Transform targetPos;
    }
    [SerializeField] private SkipEntry[] skipEntries;

    private PlayableDirector _director;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (isCutSceneCanInput) InputManager.ActivatePlayerControls();
        if (!InputManager.UseToolWasPressed) return;
        if (_director.state == PlayState.Playing)
        { TrySkip();}
       
    }

    private void TrySkip()
    {
        
        if (DoorDialogueSystem.instance !=null && DoorDialogueSystem.instance.isPlaying)
        {
            DoorDialogueSystem.instance.Skip();
        }
       
        double currentTime = _director.time;
        int targetIndex = FindNextSkipIndex(currentTime);
        if (targetIndex < 0) return;

        double targetTime = skipTimes[targetIndex];

        // TypeWriter: 코루틴 정지 + 알파값 0으로 초기화 + 텍스트 비우기
        if (typeWriter != null)
        {
            typeWriter.StopAllCoroutines();

            typeWriter.textUI.ForceMeshUpdate();
            TMP_TextInfo textInfo = typeWriter.textUI.textInfo;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;
                int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertIndex = textInfo.characterInfo[i].vertexIndex;
                Color32[] colors = textInfo.meshInfo[matIndex].colors32;
                for (int j = 0; j < 4; j++)
                    colors[vertIndex + j].a = 0;
            }
            typeWriter.textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            typeWriter.textUI.text = "";
        }
        if(targetIndex == 0&&bubble!=null)
        {
            bubble.Skip();
        }

        // 걷기 컷씬: 정지 + 텔레포트
        if (walker != null)
        {
            
            ApplyWalkerPosition(targetIndex);
        }

        _director.time = targetTime;
        _director.Evaluate();
    }

    private int FindNextSkipIndex(double currentTime)
    {
        if (skipTimes == null || skipTimes.Length == 0) return -1;

        for (int i = 0; i < skipTimes.Length; i++)
        {
            if (skipTimes[i] > currentTime)
                return i;
        }
        return -1;
    }

    private void ApplyWalkerPosition(int targetIndex)
    {
        if (skipEntries == null) return;

        foreach (var entry in skipEntries)
        {
            if (entry.skipTimeIndex == targetIndex && entry.targetPos != null)
            {
                walker.transform.position = entry.targetPos.position;
                return;
            }
        }
    }

    public void ResetSkip() { }
}