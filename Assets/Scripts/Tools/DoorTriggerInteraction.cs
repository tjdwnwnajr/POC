using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public enum DoorType
    {
        DreamOne = 0,
        DreamTwo = 1,
        DreamThree = 2,
        None = 3
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
    [Space(10f)]
    [Header("Door Type")]
    [SerializeField] private DoorType _doorType = DoorType.None;
    private Transform lightObj;
    protected override void Start()
    {
        base.Start();
        if (transform.childCount > 0)
        {
            lightObj = transform.GetChild(0);
        }
    }
    protected override void Update()
    {
        base.Update();
        if(CheckDoorType(_doorType))
        {
            if(lightObj != null)
                lightObj.gameObject.SetActive(false);
        }
    }
    public override void Interact()
    {
        for(int i = 0;  i<3; i++)
        {
            Debug.Log(SceneSwapManager.isDreamCleared[i]);
        }
        
        if (isForRespawn)
        {
            return;
        }
        if(CheckDoorType(_doorType))
        {
            return;
        }


        if (isBox && !isTrigger)
        {
            InputManager.DeactivatePlayerControls();
            StartCoroutine(FindKeyAndWakeUp());

        }
        //load new Scene
        else if (!isBox)
        {
            SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, _doorToSpawnTo);
        }
            
    }
    IEnumerator FindKeyAndWakeUp()
    {
        isTrigger = true;
        if(_doorType != DoorType.None)
            SceneSwapManager.isDreamCleared[(int)_doorType] = true;
        //열쇠를 찾는 애니메이션 재생
        yield return new WaitForSeconds(2f);
        

        //잠에서깬다
        SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, _doorToSpawnTo, isBox);

    }
    private bool CheckDoorType(DoorType type)
    {
        if (type == DoorType.None)
            return false;
        if (SceneSwapManager.isDreamCleared[(int)type])
            return true;
        else
            return false;

    }
}
