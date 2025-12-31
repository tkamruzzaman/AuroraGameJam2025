using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class DialogueData
{
    [Tooltip("Number of points that need to be activated to trigger this dialogue")]
    public int activationThreshold;
    
    [Tooltip("The dialogue text to display")]
    [TextArea(3, 5)]
    public string dialogueText;
    
    [Tooltip("How long to display this dialogue (in seconds)")]
    public float displayDuration = 3f;
    
    [Tooltip("Has this dialogue already been shown?")]
    [HideInInspector]
    public bool hasBeenShown = false;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Dialogue Settings")]
    [Tooltip("Dialogues that trigger at specific activation counts")]
    [SerializeField] private List<DialogueData> activationDialogues = new List<DialogueData>();
    
    [Header("Level Completion Dialogue")]
    [Tooltip("Dialogue shown when level is complete")]
    [TextArea(3, 5)]
    [SerializeField] private string levelCompleteDialogue = "Level Complete!";
    
    [Tooltip("Duration to show level complete dialogue before loading next scene")]
    [SerializeField] private float levelCompleteDisplayDuration = 2f;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    [Header("Scene Management")]
    [Tooltip("Should DialogueManager handle scene loading? (If false, game will handle it)")]
    [SerializeField] private bool handleSceneLoading = false;
    
    [Tooltip("Next scene to load after level completion (leave empty to use build index + 1)")]
    [SerializeField] private string nextSceneName = "";
    
    [Tooltip("Delay after dialogue before loading next scene")]
    [SerializeField] private float sceneLoadDelay = 0.5f;
    
    private int currentActivatedCount = 0;
    private bool isShowingDialogue = false;
    private bool levelCompleteDialogueShown = false;
    
    void Awake()
    {
        Instance = this;
        
        // Setup canvas group if not assigned
        if (canvasGroup == null && dialoguePanel != null)
        {
            canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
            }
        }
    }
    
    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }
        
        // Reset dialogue states when scene loads
        ResetDialogueStates();
    }
    
    /// <summary>
    /// Call this when a point is activated
    /// </summary>
    public void OnPointActivated()
    {
        currentActivatedCount++;
        Debug.Log($"DialogueManager: Point activated. Current count: {currentActivatedCount}");
        CheckActivationDialogues();
    }
    
    /// <summary>
    /// Set the activated count directly (useful for initialization or reset)
    /// Only updates if the new count is higher than current (prevents resetting backwards)
    /// </summary>
    public void SetActivatedCount(int count)
    {
        // Only update if the new count is higher, to maintain cumulative/global count
        if (count > currentActivatedCount)
        {
            currentActivatedCount = count;
            Debug.Log($"DialogueManager: Count updated to {currentActivatedCount} (was set externally)");
            CheckActivationDialogues();
        }
    }
    
    /// <summary>
    /// Get the current number of activated points
    /// </summary>
    public int GetActivatedCount()
    {
        return currentActivatedCount;
    }
    
    /// <summary>
    /// Call this when the level is complete (all points activated)
    /// </summary>
    public void OnLevelComplete()
    {
        if (!levelCompleteDialogueShown)
        {
            levelCompleteDialogueShown = true;
            ShowLevelCompleteDialogue();
        }
    }
    
    private void CheckActivationDialogues()
    {
        if (isShowingDialogue) 
        {
            // If a dialogue is showing, we'll check again after it finishes
            // This is handled in HideDialogue()
            return;
        }
        
        // Find the highest threshold that hasn't been shown yet and matches current count
        DialogueData dialogueToShow = null;
        int highestThreshold = -1;
        
        foreach (var dialogue in activationDialogues)
        {
            if (!dialogue.hasBeenShown && 
                currentActivatedCount >= dialogue.activationThreshold &&
                dialogue.activationThreshold > highestThreshold)
            {
                dialogueToShow = dialogue;
                highestThreshold = dialogue.activationThreshold;
            }
        }
        
        if (dialogueToShow != null)
        {
            Debug.Log($"DialogueManager: Showing dialogue for threshold {dialogueToShow.activationThreshold} (Current count: {currentActivatedCount})");
            ShowDialogue(dialogueToShow);
        }
        else
        {
            Debug.Log($"DialogueManager: No dialogue to show. Current count: {currentActivatedCount}, isShowingDialogue: {isShowingDialogue}");
        }
    }
    
    private void ShowDialogue(DialogueData dialogue)
    {
        if (dialogueText == null || dialoguePanel == null)
        {
            Debug.LogWarning("DialogueManager: UI references not set!");
            return;
        }
        
        isShowingDialogue = true;
        dialogue.hasBeenShown = true;
        
        dialogueText.text = dialogue.dialogueText;
        dialoguePanel.SetActive(true);
        
        // Fade in animation using DOTween
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);
        }
        
        // Hide after duration
        StartCoroutine(HideDialogueAfterDelay(dialogue.displayDuration));
    }
    
    private void ShowLevelCompleteDialogue()
    {
        if (dialogueText == null || dialoguePanel == null)
        {
            Debug.LogWarning("DialogueManager: UI references not set!");
            LoadNextScene();
            return;
        }
        
        isShowingDialogue = true;
        
        dialogueText.text = levelCompleteDialogue;
        dialoguePanel.SetActive(true);
        
        // Fade in animation using DOTween
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);
        }
        
        // Hide after duration and load next scene
        StartCoroutine(HideDialogueAndLoadScene(levelCompleteDisplayDuration));
    }
    
    private IEnumerator HideDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideDialogue();
    }
    
    private IEnumerator HideDialogueAndLoadScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideDialogue();
        
        // Only load scene if handleSceneLoading is enabled
        if (handleSceneLoading)
        {
            yield return new WaitForSeconds(sceneLoadDelay);
            LoadNextScene();
        }
    }
    
    private void HideDialogue()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    if (dialoguePanel != null)
                    {
                        dialoguePanel.SetActive(false);
                    }
                    isShowingDialogue = false;
                    // Check for more dialogues after hiding the current one
                    CheckActivationDialogues();
                });
        }
        else
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            isShowingDialogue = false;
            // Check for more dialogues after hiding the current one
            CheckActivationDialogues();
        }
    }
    
    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // Load next scene in build order
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;
            
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("DialogueManager: No next scene available. Current scene is the last one.");
            }
        }
    }
    
    private void ResetDialogueStates()
    {
        foreach (var dialogue in activationDialogues)
        {
            dialogue.hasBeenShown = false;
        }
        levelCompleteDialogueShown = false;
        currentActivatedCount = 0;
    }
    
    /// <summary>
    /// Public method to manually trigger a dialogue (for testing)
    /// </summary>
    public void ShowDialogueManually(string text, float duration = 3f)
    {
        DialogueData tempDialogue = new DialogueData
        {
            dialogueText = text,
            displayDuration = duration,
            hasBeenShown = false
        };
        ShowDialogue(tempDialogue);
    }
    
    /// <summary>
    /// Skip current dialogue (useful for player input)
    /// </summary>
    public void SkipDialogue()
    {
        if (isShowingDialogue)
        {
            StopAllCoroutines();
            HideDialogue();
        }
    }
}

