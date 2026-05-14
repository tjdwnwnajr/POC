using UnityEngine;

public class ElderDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogSystem dialogSystem;
    [SerializeField] private ElderCutsceneWalker elderWalker;

    public void StartElderDialogue()
    {
        // DialogSystem 내부에서 마우스클릭/게임패드로 알아서 진행됨
        dialogSystem.StartDialog();

        // 대화 끝난 후 처리를 위해 감시 시작
        StartCoroutine(WaitForDialogEnd());
    }

    private System.Collections.IEnumerator WaitForDialogEnd()
    {
        // 대화 시작될 때까지 잠깐 대기
        yield return new WaitForSeconds(0.2f);

        // 대화 끝날 때까지 대기
        while (dialogSystem.isDialogActive)
            yield return null;

        // 대화 끝났지만 플레이어 못움직이게 다시 막기
        PlayerStateList.canMove = false;
        PlayerStateList.isDialogue = false;
        PlayerStateList.isDialogue = false;
        InputManager.DeactivatePlayerControls();

        // 노인 문으로 입장
        if (elderWalker != null)
            elderWalker.KeepWalking();
    }
}