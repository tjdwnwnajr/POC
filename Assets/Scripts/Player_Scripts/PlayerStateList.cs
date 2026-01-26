using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool jumping;
    public bool dashing;
    public bool attacking;
    public bool recoilingX = false;
    public bool recoilingY = false;
    public static bool lookingRight =false;
    public bool invincible;
    public static bool isRope = false;
    public static bool isGrounded = true;
    public static bool canMove = true;
    public static bool isView = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static void ViewMode(bool on)
    {
        if (on)
        {
            isView = true;
            return;
        }
        else
        {
            isView = false;
            return;
        }
    }
}
