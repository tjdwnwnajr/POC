using UnityEngine;

public class StartDialog : MonoBehaviour
{
    public DialogSystem dialogSystem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (dialogSystem == null) return;

        dialogSystem.StartDialog();
    }
}