using UnityEngine;
using UnityEngine.InputSystem;

public class DialogNPC : MonoBehaviour
{
    public DialogSystem dialogSystem;
    public float interactRange = 2.5f;

    private Transform player;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || dialogSystem == null) return;

        if (dialogSystem.isDialogActive) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactRange)
        {
            if (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                dialogSystem.StartDialog();
            }

            if (Input.GetMouseButtonDown(1))
            {
                dialogSystem.StartDialog();
            }
        }
    }
}