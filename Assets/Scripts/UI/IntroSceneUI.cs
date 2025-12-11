using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class IntroSceneUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] introImageGroups;

    [Range(0, 1)][SerializeField] float fadeDuration = 0.5f;
    [Range(0, 10)][SerializeField] float displayDuration = 2.0f;

    [SerializeField] Button nextButton;

    private CanvasGroup currentCanvasGroup;
    int index = 0;

    private void Awake()
    {
        nextButton.onClick.AddListener(NextButtonAction);

        nextButton.gameObject.SetActive(false);

        foreach (var canvasGroup in introImageGroups)
        {
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        StartCoroutine(IE_PlayIntro());
    }

    IEnumerator IE_PlayIntro()
    {
        currentCanvasGroup = null;
        currentCanvasGroup = introImageGroups[index];

        currentCanvasGroup.gameObject.SetActive(true);
        currentCanvasGroup.DOFade(1, fadeDuration);

        yield return new WaitForSeconds(displayDuration);

        nextButton.gameObject.SetActive(true);
        yield return null;
    }

    void NextButtonAction()
    {
        GameServices.Instance.audioManager.PlayButtonClickSound();

        nextButton.gameObject.SetActive(false);
        currentCanvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            currentCanvasGroup.gameObject.SetActive(false);

            index++;

            if (index < introImageGroups.Length)
            {
                StartCoroutine(IE_PlayIntro());
            }
            else
            {
                GameServices.Instance.sceneNavigation.LoadScene(Scenes.Game);
            }
        });
    }
}
