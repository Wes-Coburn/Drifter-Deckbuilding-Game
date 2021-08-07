using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        LoadingScene,
        TitleScene,
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
            AudioManager.Instance.CleanAudioSources();
            switch (scene)
            {
                case Scene.TitleScene:
                    AudioManager.Instance.StartStopSound("Soundtrack_TitleScene", null, AudioManager.SoundType.Soundtrack);
                    break;
                case Scene.NewGameScene:
                    // blank
                    break;
                case Scene.DialogueScene:
                    if (DialogueManager.Instance.EngagedHero == null) 
                        GameManager.Instance.NewGame(); // FOR TESTING ONLY!
                    else
                        DialogueManager.Instance.StartDialogue(DialogueManager.Instance.EngagedHero);
                    break;
                case Scene.CombatScene:
                    UIManager.Instance.StartCombatScene();
                    CardManager.Instance.StartCombatScene();
                    GameManager.Instance.StartCombat();
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
