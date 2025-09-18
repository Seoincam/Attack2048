using UnityEngine;

public class SingleTone<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    
    protected virtual void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this as T;
        //     DontDestroyOnLoad(Instance);
        // }
        // else if (Instance != this)
        // {
        //     Destroy(gameObject);
        // }
    }
}
