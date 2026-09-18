using UnityEngine;

public static class GameBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureGameManagers()
    {
        CreateSingletonIfMissing<GameProgressManager>("GameProgressManager");
    }

    private static void CreateSingletonIfMissing<T>(string objectName) where T : Component
    {
        if (Object.FindAnyObjectByType<T>() != null)
        {
            return;
        }

        var gameObject = new GameObject(objectName);
        gameObject.AddComponent<T>();
    }
}
