using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameServices : MonoBehaviour
{
    private static GameServices _instance;
    public static GameServices Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameServices>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameServices");
                    _instance = singletonObject.AddComponent<GameServices>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    
    public EventManager eventManager;
    public SceneNavigation sceneNavigation;
    public AudioManager audioManager;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        eventManager = new EventManager();

        eventManager.FireGameServiceInitialized();


        sceneNavigation ??= FindFirstObjectByType<SceneNavigation>();
        audioManager ??= FindFirstObjectByType<AudioManager>();
    }

    private void Start()
    {
        audioManager.PlayBackgroundMusic();
    }


}
