using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerDatas", menuName = "Scriptable Objects/SO_PlayerDatas")]
public class SO_PlayerDatas : ScriptableObject
{
    public string Name;
    public int Score;
    public int Level;
    
    private SaveController saveSystem;

    public void LoadDatas()
    {
        CheckSaveSystem();
        // utiliser fonction de savesystem pour load les datas
        // elle renvoie des playersdatas
        PlayerDatas datas = saveSystem.Load();
        // donc je dois les affecter aux variables de mon scriptable object
        Name = datas.Name;
        Score = datas.Score;
        Level = datas.Level;
    }
    

    public void SaveDatas()
    {
        CheckSaveSystem();
        // pour utiliser la fonction save de ma savesystem j'ai besoin de playerdatas
        // je dois créer un playerdatas à partir de mon SO
        PlayerDatas datas  = new PlayerDatas();
        datas.Name = Name;
        datas.Score = Score;
        datas.Level = Level;
        // j'envoie ca à la fonction de savesystem
        saveSystem.Save(datas);
    }

    private void CheckSaveSystem()
    {
        // vérifier si savesystem contient un objet du type savesystem (qlqchose dedans??)
        if (saveSystem == null)
        {
            // si rien , j'en instancie un (créer)
            saveSystem = new SaveController();
        }

    }
}
