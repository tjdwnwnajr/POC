using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager Instance;
    private static bool _loadFromDoor;
    public static bool[] isDreamCleared = new bool[3] { false, false, false };


    private bool _isBox;
    private GameObject _player;
    private Collider2D _playerColl;
    private Collider2D _doorColl;
    private Vector3 _playerSpawnPosition;

    private DoorTriggerInteraction.DoorToSpawnAt _doorToSpawnTo;




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        _player = GameObject.FindGameObjectWithTag("Player");
        if(_player !=null)
            _playerColl = _player.GetComponent<Collider2D>();

    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public static void SwapSceneFromDoorUse(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt, bool isBox = false, bool sceneswapsounduse = true)
    {
        
        if (!isBox)
        {
            _loadFromDoor = true;
            Instance.StartCoroutine(Instance.FadeOutThenChangeScene(myScene, doorToSpawnAt, sceneswapsounduse));
        }
        else
        {
            _loadFromDoor = true;
            Instance.StartCoroutine(Instance.BrightOutThenChangeScene(myScene, doorToSpawnAt));
            
        }
    }


    private IEnumerator FadeOutThenChangeScene(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None, bool sceneswapsounduse = true)
    {
        //start fading to black
        InputManager.DeactivatePlayerControls();
        SceneFadeManager.instance.StartFadeOut();
        if(sceneswapsounduse)
            SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.fadeout, transform, 1f);
        //keep fading out
        while (SceneFadeManager.instance.IsFadingOut)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        _doorToSpawnTo = doorToSpawnAt;
        //카메라 초기화
        CameraUtility.InvalidateCache();
        SceneManager.LoadScene(myScene);
        #region this code load scene first and then fade in
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(myScene);

        //// 씬 로드가 완료될 때까지 대기
        //while (!asyncLoad.isDone)
        //{
        //    yield return null;
        //}

        //// 씬 로드 완료 후 페이드인
        //yield return new WaitForSeconds(0.1f);
        //if (SceneFadeManager.instance != null)
        //{
        //    SceneFadeManager.instance.StartFadeIn();
        //}
        #endregion

    }
    private IEnumerator BrightOutThenChangeScene(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None)
    {
        _isBox = true;
        //start fading to black
        InputManager.DeactivatePlayerControls();
        SceneBrightManager.instance.StartBrightOut();
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.brightout, transform, 1f);
        //keep fading out
        while (SceneBrightManager.instance.IsBrightOut)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        _doorToSpawnTo = doorToSpawnAt;
        //카메라 초기화
        CameraUtility.InvalidateCache();
        
        
        SceneManager.LoadScene(myScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //플레이어 다시찾아서 위치시키기
        if (_loadFromDoor)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerColl = _player.GetComponent<Collider2D>();
            FindDoor(_doorToSpawnTo);
            _player.transform.position = _playerSpawnPosition;
            _loadFromDoor = false;
        }
        if (MapRoomManager.instance != null)
        {
            MapRoomManager.instance.RevealRoom();
        }
        

        //카메라 초기화
        CameraUtility.InvalidateCache();


        CameraManager.Instance.InitCameraByPlayerPosition();
        //카메라 다시찾아서 흔들기위한 리스너 연결하기
        //CameraShakeManager.instance.FindAndSetupVirtualCamera();
        CameraShakeManager.instance.RefreshCamera();

        //카메라 event offset 을 위한 카메라 다시 찾기
        CameraEventFocus.instance.RefreshCamera();

        //미니맵 로드를위한 카메라 찾기
        MapManager.instance.FindMapCam();

        // 한 프레임 대기 (Cinemachine이 새 위치로 업데이트되도록)
        if (!_isBox)
            StartCoroutine(DelayedFadeIn());
        else
        {
            StartCoroutine(DelayedBrightIn());
            _isBox = false;
        }
     }

    private IEnumerator DelayedFadeIn()
    {
        yield return new WaitForSeconds(0.5f) ; // Cinemachine 업데이트 대기

        if (SceneFadeManager.instance != null)
        {
            SceneFadeManager.instance.StartFadeIn();
            StartCoroutine(ActivatePlayerControl());
        }
    }
    #region this code make camera move to upside
    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{

    //    if (_loadFromDoor)
    //    {
    //        FindDoor(_doorToSpawnTo);
    //        _player.transform.position = _playerSpawnPosition;
    //        _loadFromDoor = false;

    //    }

    //    SceneFadeManager.instance.StartFadeIn();
    //}
    #endregion
    private IEnumerator DelayedBrightIn()
    {
        yield return new WaitForSeconds(0.5f); // Cinemachine 업데이트 대기

        if (SceneBrightManager.instance != null)
        {
            SceneBrightManager.instance.StartBrightIn();
            StartCoroutine(ActivatePlayerControl());
        }
    }

    private IEnumerator ActivatePlayerControl()
    {
        while(SceneFadeManager.instance.IsFadingIn)
        {
            yield return null;
        }
        InputManager.ActivatePlayerControls();
    }
    private void FindDoor(DoorTriggerInteraction.DoorToSpawnAt doorSpawnNumber)
    {
        DoorTriggerInteraction[] doors = FindObjectsByType<DoorTriggerInteraction>(FindObjectsSortMode.None);

        for (int i = 0; i<doors.Length; i++)
        {
            if (doors[i].CurrentDoorPosition == doorSpawnNumber)
            {
                _doorColl = doors[i].GetComponent<Collider2D>();

                //calculate spwan position 
                CalculateSpawnPosition();

                return;
            }
        }
    }

    private void CalculateSpawnPosition()
    {
        float colliderHeight = _playerColl.bounds.extents.y;
        _playerSpawnPosition = _doorColl.transform.position - new Vector3(0f, colliderHeight, 0f);

    }

    public static void ResetSceneSwapManager()
    {
        _loadFromDoor = false;
        for (int i = 0; i < isDreamCleared.Length; i++)
        {
            isDreamCleared[i] = false;
        }
    }
}
