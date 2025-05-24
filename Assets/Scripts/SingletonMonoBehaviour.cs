using Sirenix.OdinInspector;
using UnityEngine;

public class SingletonMonoBehaviour<T> : SerializedMonoBehaviour where T : SingletonMonoBehaviour<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance == null) Instance = this as T;
        else Destroy(gameObject);
    }
}
