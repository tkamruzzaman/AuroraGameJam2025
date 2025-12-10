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


        StartCoroutine(IE_PlayIntroSequence());
    }

int index;
    IEnumerator IE_PlayIntroSequence()
    {
        //foreach (var canvasGroup in introImageGroups)
        //{
            currentCanvasGroup = introImageGroups[index];
            StartCoroutine(IE_PlayIntro());
            yield return new WaitForSeconds(fadeDuration + displayDuration + fadeDuration);
        //}
        // StartCoroutine(IE_PlayIntro(introImageGroups[0]));
        // yield return new WaitForSeconds(fadeDuration + displayDuration + fadeDuration);
        // StartCoroutine(IE_PlayIntro(introImageGroups[1]));
        // yield return new WaitForSeconds(fadeDuration + displayDuration + fadeDuration);
        // StartCoroutine(IE_PlayIntro(introImageGroups[2]));
        // yield return new WaitForSeconds(fadeDuration + displayDuration + fadeDuration);
        //    StartCoroutine(IE_PlayIntro(introImageGroups[3]));
        // yield return new WaitForSeconds(fadeDuration + displayDuration + fadeDuration);
        //    StartCoroutine(IE_PlayIntro(introImageGroups[4]));
        // yield return new WaitForSeconds(fadeDuration + displayDuration + fadeDuration);
        // StartCoroutine(IE_PlayIntro(introImageGroups[5]));
        // yield return new WaitForSeconds(fadeDuration + displayDuration + fadeDuration);
    }

    IEnumerator IE_PlayIntro()
    {
        currentCanvasGroup.gameObject.SetActive(true);
        currentCanvasGroup.DOFade(1, fadeDuration);

        yield return new WaitForSeconds(displayDuration);

        nextButton.gameObject.SetActive(true);



        yield return null;
    }

    void NextButtonAction()
    {
        currentCanvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            currentCanvasGroup.gameObject.SetActive(false);
        });
        index++;
        nextButton.gameObject.SetActive(false);
        if (index < introImageGroups.Length)
        {
            currentCanvasGroup = introImageGroups[index];
            StartCoroutine(IE_PlayIntro());
        }
        else
        {
            //all done, load next scene
        GameServices.Instance.sceneNavigation.LoadScene(Scenes.Game);
        }
    
    }
}
