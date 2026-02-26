using UnityEngine;
using Cinemachine;
public class ParallaxImageOnOff : MonoBehaviour
{
    [SerializeField]private bool imageActive = false;
    [SerializeField]private GameObject backGround;
    [SerializeField]private CinemachineVirtualCamera[] targetCams;
    private float offTimer = 0f;
    private float offSpeed = 1.5f;
    private bool anyActivated;
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
        if (targetCams == null || targetCams.Length == 0) return;
        anyActivated = false;

        for (int i = 0; i < targetCams.Length; i++)
        {
            if (targetCams[i].enabled == true)
            {
                anyActivated = true;
                break;

            }
            
        } 
        if (anyActivated)
        {
            imageActive = true;
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
