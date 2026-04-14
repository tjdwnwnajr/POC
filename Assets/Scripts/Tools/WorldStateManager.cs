using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    public bool keyOneTaken;
    public bool doorOpened;
    public bool headBtnTaken;
    public bool buttonFixed;
    public bool fakeWallOpened;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}