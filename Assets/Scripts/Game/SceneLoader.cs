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
        NarrativeScene,
        DialogueScene,
        CombatScene
    }

    public static bool SceneIsLoading = false;
    private static Action onSceneLoaderCallback;
    private static Action onSceneUpdateCallback;

    public static void LoadScene(Scene scene)
    {
        if (SceneManager.GetActiveScene().name == scene.ToString()) return;
        if (SceneIsLoading) return;
        SceneIsLoading = true;

        onSceneLoaderCallback = () =>
        {
            FunctionTimer.Create(() => UIManager.Instance.SetSceneFader(true), 3f);
            FunctionTimer.Create(() => SceneManager.LoadScene(scene.ToString()), 5f);
            FunctionTimer.Create(() => UIManager.Instance.SetSceneFader(false), 5f);
        };

        onSceneUpdateCallback = () =>
        {
            SceneIsLoading = false;
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
                case Scene.NarrativeScene:
                    GameManager.Instance.StartNarrative();
                    break;
                case Scene.DialogueScene:
                    if (DialogueManager.Instance.EngagedHero == null)
                    {
                        DialogueManager.Instance.StartDialogue
                        (GameManager.Instance.GetActiveNPC(GameManager.Instance.NPCTestHero)); // FOR TESTING ONLY
                    }
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

        DialogueManager.Instance.StopTimedText();
        FunctionTimer.Create(() => UIManager.Instance.SetSceneFader(true), 0f);
        FunctionTimer.Create(() => SceneManager.LoadScene(Scene.LoadingScene.ToString()), 1.5f);
        FunctionTimer.Create(() => UIManager.Instance.SetSceneFader(false), 1.5f);
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
