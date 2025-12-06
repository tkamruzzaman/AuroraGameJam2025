using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;

    private void Awake() {
        playButton.onClick.AddListener(OnPlayButtonAction);
        creditsButton.onClick.AddListener(OnCreditsButtonAction);
        exitButton.onClick.AddListener(OnExitButtonAction);
    }

    private void OnPlayButtonAction()
    {
        
    }

    private void OnCreditsButtonAction()
    {
        
    }

    private void OnExitButtonAction()
    {
        
    }
}
