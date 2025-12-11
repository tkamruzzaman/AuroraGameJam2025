using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button playButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;

    [Header("Panels")]
    [SerializeField] GameObject creditsPanel;


    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonAction);
        creditsButton.onClick.AddListener(OnCreditsButtonAction);
        exitButton.onClick.AddListener(OnExitButtonAction);
        creditsPanel.SetActive(false);
    }

    private void OnPlayButtonAction()
    {
        GameServices.Instance.sceneNavigation.LoadScene(Scenes.Intro);
    }

    private void OnCreditsButtonAction()
    {
        creditsPanel.SetActive(true);
    }

    private void OnExitButtonAction()
    {
#if UNITY_EDITOR
        if(UnityEditor.EditorApplication.isPlaying)
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
