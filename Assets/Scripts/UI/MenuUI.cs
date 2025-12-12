using DG.Tweening;
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
        creditsPanel.transform.DOScale(0, 0.2f);
    }

    private void OnPlayButtonAction()
    {
        GameServices.Instance.audioManager.PlayButtonClickSound();
        GameServices.Instance.sceneNavigation.LoadScene(Scenes.Intro);
    }

    private void OnCreditsButtonAction()
    {
        GameServices.Instance.audioManager.PlayButtonClickSound();
        creditsPanel.SetActive(true);
        creditsPanel.transform.DOScale(1, 0.2f);
    }

    private void OnExitButtonAction()
    {
        GameServices.Instance.audioManager.PlayButtonClickSound();

#if UNITY_EDITOR
        if(UnityEditor.EditorApplication.isPlaying)
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CloseCreditsPanel()
    {
        GameServices.Instance.audioManager.PlayButtonClickSound();
        creditsPanel.transform.DOScale(0, 0.2f).OnComplete(()=> creditsPanel.SetActive(false));
    }
}
