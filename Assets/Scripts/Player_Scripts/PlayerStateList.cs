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
    [SerializeField]public static bool isMirror = false;
    public bool mirrorOn = false;
    public static bool isRotate = false;
    public static bool keyOne = false;
    public static bool keyTwo = false;
    public static bool keyThree = false;
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
    private void Update()
    {
        if (mirrorOn)
        {
            isMirror = true;
        }
        else
        {
            isMirror = false;
        }
    }
}
