using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class DoorTriggerInteraction : TriggerInteractionBase
{
    public enum DoorToSpawnAt
    {
        None,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten
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

    [Header("CompleteUI")]
    [SerializeField] private GameObject completeUI;
    [SerializeField] private float uiDuration = 2f;
    [SerializeField] private GameObject keyObj;

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
        Animator anim = GetComponent<Animator>();
        //상자 여는 소리
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.chest, transform, 1f);

        anim.SetTrigger("isOpen");
        yield return new WaitForSeconds(0.1f);
        if(keyObj !=null)
        {
            SpriteRenderer spriteRenderer = keyObj.GetComponent<SpriteRenderer>();
            Vector3 position = keyObj.transform.position;
            float startY = position.y;  
            float targetY = position.y + 1.5f;
            Color a = spriteRenderer.color;
            float t = 0;
            while (t < 1.5f)
            {
                t += Time.deltaTime;
                float per = t / 1.5f;
                a.a = Mathf.Lerp(0f, 1f, per);
                position.y = Mathf.Lerp(startY, targetY, t);
                spriteRenderer.color = a;
                keyObj.transform.position = position;

                yield return null;
            }

            spriteRenderer.color = new Color(a.r, a.g, a.b, 1f);
        }
        yield return new WaitForSeconds(0.9f);
        if (completeUI != null)
        {
            completeUI.SetActive(true);
            yield return new WaitForSeconds(uiDuration);
            completeUI.SetActive(false);
        }

        
        if(_doorType == DoorType.DreamOne)
        {
            PlayerStateList.firstKeyFounded = true;
        }
        else if(_doorType == DoorType.DreamTwo)
        {
            PlayerStateList.secondKeyFounded = true;
        }
        else if(_doorType == DoorType.DreamThree)
        {
            PlayerStateList.thirdKeyFounded = true;
        }

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
