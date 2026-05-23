using UnityEngine;

public class Resetbug : MonoBehaviour
{
  
    // Update is called once per frame
    void Update()
    {
        if (InputManager.l2IsHeld && InputManager.setwasPressed)
        {
            transform.position = Resetposition.GetRespawnPosition();
            PlayerStateList.canMove = true;
            PlayerStateList.isView = false;
            PlayerStateList.isDialogue = false;
            Time.timeScale = 1f;
        }
    }
}
