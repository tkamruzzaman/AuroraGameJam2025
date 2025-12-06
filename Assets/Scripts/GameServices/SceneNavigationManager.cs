using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationManager : MonoBehaviour
{

private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;

}

    private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);

    }

    public void LoadScene(Scenes scene)
    {
        SceneManager.LoadScene((int)scene);
    }
}

public enum Scenes
{
    Menu = 0,
    Game = 1,
}
