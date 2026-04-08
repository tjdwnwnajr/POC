using System.Collections;
using UnityEngine;

public class BtnTriggerInteraction : TriggerInteractionBase
{
    [SerializeField] private float coolTime;
    [SerializeField] private bool autoReleasing;
    [SerializeField] private bool forRock;
    private float heldTime;
    private bool isPressed;
    private Coroutine pressRoutine;
    private CreateRock rock;
    [SerializeField] private bool onlyOne;
    private bool isUsed;


    private Animator anim;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        heldTime = coolTime - 1.666f;
        isPressed = false;
        if (forRock)
        {
            rock = GetComponent<CreateRock>();
        }
    }


    public override void Interact()
    {
        if (pressRoutine != null) return;
        if (forRock)
        {
            if (rock.isCreated) return;
            rock.TrySpawn();
        }
        if (onlyOne)
        {
            if (!isUsed)
            {
                pressRoutine = StartCoroutine(PressedBtn());
                isUsed = true;
            }
        }
        else
        {
            pressRoutine = StartCoroutine(PressedBtn());
        }
            
        
    }
    IEnumerator PressedBtn()
    {
        isPressed = !isPressed;
        anim.SetBool("isPressed", isPressed);
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.button, transform, 0.1f);
        if (autoReleasing)
        {
            yield return new WaitForSeconds(heldTime);
            isPressed = !isPressed;
            anim.SetBool("isPressed", isPressed);
            
        }
        else
        {
            yield return null;
        }
        pressRoutine = null;
    }
}
