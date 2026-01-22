using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private ArrowLauncher launcher;
    [SerializeField] private string playerTag = "Player";

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag(playerTag)) return;

        activated = true;
        launcher.Fire();
    }
}
