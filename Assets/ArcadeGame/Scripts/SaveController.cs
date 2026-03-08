using System.IO;
using UnityEngine;

[System.Serializable]

public class PlayerDatas   
{
    public string Name = "None";
    public int Score =0;
    public int Level = 1;
}

public class SaveController
{
    public string GetPath()
    {
        return Application.persistentDataPath + "/save.json";
    }
    
    public void Save(PlayerDatas datas)
    {
        string json = JsonUtility.ToJson(datas, true);
        File.WriteAllText(GetPath(), json);
    }

    public PlayerDatas Load()
    {
        if (File.Exists(GetPath()))
        {
            string json = File.ReadAllText(GetPath());
            return JsonUtility.FromJson<PlayerDatas>(json);
        }
        return new PlayerDatas();
    }
    
}
