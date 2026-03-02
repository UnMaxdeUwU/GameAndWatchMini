using UnityEngine;

public class SaveGameSystem : MonoBehaviour
{
    [SerializeField] private SO_PlayerDatas playerDatas;

    public void LoadSaveGame()
    {
        playerDatas.LoadDatas();
    }

    public void SaveGame()
    {
        playerDatas.SaveDatas();
    }
    
}
