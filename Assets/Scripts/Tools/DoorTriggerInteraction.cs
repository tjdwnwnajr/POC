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

    [Space(10f)]
    [Header("This Door")]
    public DoorToSpawnAt CurrentDoorPosition;
    public override void Interact()
    {
        //load new Scene
        SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, _doorToSpawnTo);
    }
}
