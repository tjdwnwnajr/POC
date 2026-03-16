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
    private void Awake()
    {
        _minimapCam.orthographicSize = minSize;
        if(instance == null)
        {
            instance = this;
        }
        
        CloseLargeMap();
    }

    private void Update()
    {
        if (InputManager.mapWasPressed)
        {
           
            if (IsLargeMapOpen)
            {
                CloseLargeMap();
            }
            else
            {
                OpenLargeMap();
            }
        }
        if (IsLargeMapOpen)
        {
            float newSize = _minimapCam.orthographicSize + InputManager.MapSizeDirection.y*(-0.1f); 
            if(newSize > minSize&&newSize<maxSize)
            {
                _minimapCam.orthographicSize = newSize;
            }
            
        }
    }

    private void OpenLargeMap()
    {
        PlayerStateList.isView = true;
        _largeMap.SetActive(true);
        IsLargeMapOpen = true;
    }
    private void CloseLargeMap()
    {
        PlayerStateList.isView = false;
        _largeMap.SetActive(false);
        IsLargeMapOpen = false;
    }


}
