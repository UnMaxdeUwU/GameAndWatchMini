using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;



    public void LoadScene(string  sceneName)
    {
        _audioSource.Play();
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        _audioSource.Play();
        Application.Quit();
        Debug.Log("Quit");
    }
}
