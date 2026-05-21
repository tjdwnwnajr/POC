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
    public static bool isMirror = false;
    public static bool mapRotation = false;
    public static bool mapOpen = false;
    //public bool mirrorOn = false;
    public static bool isDialogue = false;
    public static bool menuOpen = false;
    public static bool isRotate = false;
    public static bool keyOne = false;
    public static bool headBtn = false;
    public static bool isDead = false;
    public static bool firstKeyFounded = true;
    public static bool secondKeyFounded = true;
    public static bool thirdKeyFounded = true;
    public static bool keyReady = false;
    public static int deathCount = 0;

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
        if(firstKeyFounded && secondKeyFounded && thirdKeyFounded)
        {
            keyReady = true;
        }
        else
        {
            keyReady = false;
        }
        
    }
    public static void ResetPlayerState()
    {
        isRope = false;
        isGrounded = true;
        canMove = true;
        isView = false;
        isMirror = false;
        isRotate = false;
        keyOne = false;
        headBtn = false;
        isDead = false;
        firstKeyFounded = false;
        secondKeyFounded = false;
        thirdKeyFounded = true;
        keyReady = false;
        deathCount = 0;
        isDialogue = false;
        menuOpen = false;
    }
}
