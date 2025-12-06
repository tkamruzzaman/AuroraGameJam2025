using UnityEngine;

public class GameServices : MonoBehaviour
{
    private static GameServices _instance;

    public static GameServices Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameServices>();
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

        //EventManager.FireGameServiceInitialized();
    }

    // Add your game service methods and properties here
}
