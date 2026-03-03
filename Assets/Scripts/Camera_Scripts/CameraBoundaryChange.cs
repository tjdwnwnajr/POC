using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraBoundaryChange :TriggerInteractionBase
{
    [SerializeField] private CompositeCollider2D defaultBoundary;
    [SerializeField] private CompositeCollider2D newBoundary;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineConfiner2D boundary;
    [SerializeField] private float duration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        
    }

    public override void Interact()
    {
        StartCoroutine(ChangeBoundary());
    }
    
    private IEnumerator ChangeBoundary()
    {
        boundary.m_BoundingShape2D = newBoundary;

        yield return new WaitForSeconds(duration);

        boundary.m_BoundingShape2D = defaultBoundary;
    }
   
}
