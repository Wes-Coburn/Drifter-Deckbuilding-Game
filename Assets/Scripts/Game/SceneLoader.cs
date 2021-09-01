using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static Action onSceneLoaderCallback;
    private static Action onSceneUpdateCallback;

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

    public static void LoadScene(Scene scene, bool loadSameScene = false)
    {
        if (SceneIsLoading) return;
        if (!loadSameScene && SceneManager.GetActiveScene().name == scene.ToString()) return;

        UIManager uMan = UIManager.Instance;
        AudioManager auMan = AudioManager.Instance;
        GameManager gMan = GameManager.Instance;
        DialogueManager dMan = DialogueManager.Instance;
        CombatManager coMan = CombatManager.Instance;
        SceneIsLoading = true;
        FunctionTimer.Create(() => 
        auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, false, true), 1f);

        onSceneLoaderCallback = () =>
        {
            UnityEngine.Object.FindObjectOfType<LoadingSceneDisplay>().ChapterText = gMan.NextChapter;
            FunctionTimer.Create(() => uMan.SetSceneFader(true), 3f);
            FunctionTimer.Create(() => SceneManager.LoadScene(scene.ToString()), 5f);
            FunctionTimer.Create(() => uMan.SetSceneFader(false), 5f);
        };

        onSceneUpdateCallback = () =>
        {
            SceneIsLoading = false;
            auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, true);
            uMan.Start();
            auMan.CleanAudioSources();
            switch (scene)
            {
                case Scene.TitleScene:
                    auMan.StartStopSound("Soundtrack_TitleScene", null, AudioManager.SoundType.Soundtrack);
                    break;
                case Scene.NewGameScene:
                    // blank
                    break;
                case Scene.NarrativeScene:
                    gMan.StartNarrative();
                    break;
                case Scene.DialogueScene:
                    if (dMan.EngagedHero == null)
                    {
                        dMan.StartDialogue(gMan.GetActiveNPC(gMan.NPCTestHero)); // FOR TESTING ONLY
                    }
                    else
                        dMan.StartDialogue(dMan.EngagedHero);
                    break;
                case Scene.CombatScene:
                    uMan.StartCombatScene();
                    coMan.StartCombatScene();
                    gMan.StartCombat();
                    break;
                default:
                    Debug.LogError("SCENE NOT FOUND!");
                    break;
            }
        };

        dMan.StopTimedText();
        FunctionTimer.Create(() => uMan.SetSceneFader(true), 0f);
        FunctionTimer.Create(() => SceneManager.LoadScene(Scene.LoadingScene.ToString()), 1.5f);
        FunctionTimer.Create(() => uMan.SetSceneFader(false), 1.5f);
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
