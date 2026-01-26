using UnityEngine;

public class StopFirePlate : MonoBehaviour
{
    [SerializeField] private ArrowLauncher launcher;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            launcher.StopFiring();
    }
}
