using System;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        LoadingScene,
        MenuScene,
        GameScene
    }

    private static Action onSceneLoaderCallback;
    private static Action onSceneUpdateCallback;

    public static void LoadScene(Scene scene)
    {
        UIManager.Instance.OnWaitForSecondsCallback = () =>
        {
            SceneManager.LoadScene(scene.ToString());
        };
        
        onSceneLoaderCallback = () =>
        {
            UIManager.Instance.WaitForSeconds(3.5f);
        };

        onSceneUpdateCallback = () =>
        {
            UIManager.Instance.Start();
            UIManager.Instance.LoadGameScene();
            CardManager.Instance.StartGame(); // FOR TESTING ONLY
            GameManager.Instance.StartGame(); // FOR TESTING ONLY
            //AudioManager.Instance.StartStopSound("Soundscape_" + scene.ToString(), AudioManager.SoundType.Soundscape);
            //AudioManager.Instance.StartStopSound("Soundtrack_WorldTheme", AudioManager.SoundType.Soundtrack);
        };
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void SceneLoaderCallback()
    {
        if (onSceneLoaderCallback != null)
        {
            onSceneLoaderCallback();
            onSceneLoaderCallback = null;
        }
    }

    public static void SceneUpdateCallback()
    {
        if (onSceneUpdateCallback != null)
        {
            onSceneUpdateCallback();
            onSceneUpdateCallback = null;
        }
    }
}
