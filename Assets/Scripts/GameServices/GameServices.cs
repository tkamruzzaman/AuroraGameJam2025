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
           public EventManager eventManager ;
            public SceneNavigation sceneNavigation;


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


        sceneNavigation = FindFirstObjectByType<SceneNavigation>();
    }

    private void Start()
    {

    }


}
