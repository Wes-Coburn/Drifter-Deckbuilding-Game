using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        LoadingScene,
        MenuScene,
        NewGameScene,
        DialogueScene,
        CombatScene
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
            switch (scene)
            {
                case Scene.MenuScene:
                    AudioManager.Instance.StartStopSound("Soundtrack_MenuScene", null, AudioManager.SoundType.Soundtrack);
                    break;
                case Scene.NewGameScene:
                    // blank
                    break;
                case Scene.DialogueScene:
                    GameManager.Instance.NewGame(); // FOR TESTING ONLY!!!
                    break;
                case Scene.CombatScene:
                    UIManager.Instance.StartCombatScene();
                    CardManager.Instance.StartCombatScene();
                    GameManager.Instance.StartCombat();
                    DialogueManager.Instance.EndDialogue(); // TESTING
                    break;
                default:
                    Debug.LogError("SCENE NOT FOUND!");
                    break;
            }
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
