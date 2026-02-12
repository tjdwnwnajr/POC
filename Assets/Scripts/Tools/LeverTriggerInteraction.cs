using UnityEngine;

public class LeverTriggerInteraction : TriggerInteractionBase
{
    private Animator anim;
    private bool isRight;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        isRight = false;
    }

    public override void Interact()
    {
        anim.SetBool("isRight", !isRight);
        isRight = !isRight;
        Debug.Log(isRight);
    }
}
