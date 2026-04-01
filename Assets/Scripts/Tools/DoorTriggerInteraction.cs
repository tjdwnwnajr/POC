using System.Collections;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractionBase
{
    public enum DoorToSpawnAt
    {
        None,
        One,
        Two,
        Three,
        Four
    }

    [Header("Spwan To")]
    [SerializeField] private DoorToSpawnAt _doorToSpawnTo;
    [SerializeField] private SceneField _sceneToLoad;
    [SerializeField] private bool isBox;
    [SerializeField] private bool isForRespawn;
    private bool isTrigger = false;
    [Space(10f)]
    [Header("This Door")]
    public DoorToSpawnAt CurrentDoorPosition;
    public override void Interact()
    {
        if (isForRespawn)
        {
            return;
        }
        if (isBox&&!isTrigger)
        {

            StartCoroutine(FindKeyAndWakeUp());

        }
        //load new Scene
        else
        {
            SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, _doorToSpawnTo);
        }
            
    }
    IEnumerator FindKeyAndWakeUp()
    {
        isTrigger = true;
        //열쇠를 찾는 애니메이션 재생
        yield return new WaitForSeconds(2f);

        //잠에서깬다
        SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, _doorToSpawnTo, isBox);

    }
}
