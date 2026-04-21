using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [SerializeField] private Camera _minimapCam;
    [SerializeField] private GameObject _largeMap;
    private float maxSize = 28.5f;
    private float minSize = 20f;
    public bool IsLargeMapOpen { get; private set; }

    private Dictionary<string, int> _activatedPartCount = new();

    private void Awake()
    {
        FindMapCam();
        _minimapCam.orthographicSize = minSize;
        if(instance == null)
        {
            instance = this;
        }
        
        CloseLargeMap();
        
    }

    private void Update()
    {
        if (InputManager.mapWasPressed&&IsLargeMapOpen)
        {
           
            CloseLargeMap();
            
            
        }
        else if(PlayerStateList.isGrounded && InputManager.mapWasPressed && !IsLargeMapOpen)
        {
            OpenLargeMap();
        }
        if (IsLargeMapOpen)
        {
            float newSize = _minimapCam.orthographicSize + InputManager.MapSizeDirection.y * (-0.1f);
            if (newSize > minSize && newSize < maxSize)
            {
                _minimapCam.orthographicSize = newSize;
            }

        }
    }
    public void FindMapCam()
    {
        GameObject cam = GameObject.FindWithTag("MapCam");
        if(cam !=null)
            _minimapCam = cam.GetComponent<Camera>();
    }

    private void OpenLargeMap()
    {
        if (PlayerStateList.mapRotation)
            return;
        Time.timeScale = 0f;
        PlayerStateList.isView = true;
        _largeMap.SetActive(true);
        IsLargeMapOpen = true;
    }
    public void CloseLargeMap()
    {
        Time.timeScale = 1f;
        PlayerStateList.isView = false;
        _largeMap.SetActive(false);
        IsLargeMapOpen = false;
    }

    public void SavePartCount(SceneField scene, int count)
    {
        string key = scene.SceneName;
        if (!_activatedPartCount.ContainsKey(key) || count > _activatedPartCount[key])
            _activatedPartCount[key] = count;
    }

    public int GetPartCount(SceneField scene)
    {
        return _activatedPartCount.TryGetValue(scene.SceneName, out int count) ? count : 1;
    }

}
