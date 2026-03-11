using UnityEngine;
using UnityEngine.SceneManagement;

public class MapRoomManager : MonoBehaviour
{
    public static MapRoomManager instance;

    private MapContainerData[] _rooms;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        _rooms = GetComponentsInChildren<MapContainerData>(true);

    }

    public void RevealRoom()
    {
        string newLodedScene = SceneManager.GetActiveScene().name;

        for (int i = 0; i < _rooms.Length; i++)
        {
            if (_rooms[i].RoomScene.SceneName == newLodedScene && !_rooms[i].HasBeenRevealed)
            {
                _rooms[i].gameObject.SetActive(true);
                _rooms[i].HasBeenRevealed = true;

                return;
            }
        }
    }
}
