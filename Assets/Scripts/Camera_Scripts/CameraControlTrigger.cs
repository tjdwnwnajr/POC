using UnityEngine;
using UnityEditor;
using Cinemachine;


public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorOvjects customInspectorObjects;

    private Collider2D _coll;
    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
     if(collision.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                //pan the camera
                CameraManager.Instance.PanCameraOnContact(customInspectorObjects.panDistance,
                   customInspectorObjects.panTime, customInspectorObjects.panDirection,
                   false);
            }
        }   
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;
            if(customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                //swap Camera
                CameraManager.Instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            }

            if (customInspectorObjects.panCameraOnContact)
            {
                //pan the camera
                CameraManager.Instance.PanCameraOnContact(customInspectorObjects.panDistance,
                    customInspectorObjects.panTime, customInspectorObjects.panDirection,
                    true);
            }
        }
    }
}
[System.Serializable]
public class CustomInspectorOvjects
{

    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}
public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}


