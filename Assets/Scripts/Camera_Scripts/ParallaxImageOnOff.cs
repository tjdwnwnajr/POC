using UnityEngine;
using Cinemachine;
public class ParallaxImageOnOff : MonoBehaviour
{
    [SerializeField]private bool imageActive = false;
    [SerializeField]private GameObject backGround;
    [SerializeField]private CinemachineVirtualCamera targetCam;
    private float offTimer = 0f;
    private float offSpeed = 1.5f;
    
    private void Start()
    {
        backGround.SetActive(imageActive);
    }
    private void Update()
    {
        CheckActive();
        backGround.SetActive(imageActive);
    }
    private void CheckActive()
    {
        if (targetCam != null)
        {
            if (targetCam.enabled == true) { 
                imageActive= true;
                offTimer = 0f;
            }
            else
            {
                offTimer += Time.deltaTime;
                if (offTimer > offSpeed)
                {
                    imageActive = false;
                }
            }
        }
    }
}
