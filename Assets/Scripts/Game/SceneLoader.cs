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
        WorldMapScene,
        DialogueScene,
        CombatScene
    }
    public static bool SceneIsLoading = false;

    public static void LoadScene(Scene scene, bool loadSameScene = false)
    {
        if (SceneIsLoading) return;
        if (!loadSameScene && SceneManager.GetActiveScene().name == scene.ToString()) return;
        SceneIsLoading = true;

        UIManager uMan = UIManager.Instance;
        AudioManager auMan = AudioManager.Instance;
        GameManager gMan = GameManager.Instance;
        DialogueManager dMan = DialogueManager.Instance;
        CombatManager coMan = CombatManager.Instance;
        FunctionTimer.Create(() => 
        auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, false, true), 1f);

        onSceneLoaderCallback = () =>
        {
            string chapterText;
            if (scene == Scene.CombatScene) chapterText = "Combat!";
            else chapterText = gMan.NextChapter;
            UnityEngine.Object.FindObjectOfType<LoadingSceneDisplay>().ChapterText = chapterText;
            FunctionTimer.Create(() => uMan.SetSceneFader(true), 5f);
            FunctionTimer.Create(() => SceneManager.LoadScene(scene.ToString()), 7f);
            FunctionTimer.Create(() => uMan.SetSceneFader(false), 7f);
        };

        onSceneUpdateCallback = () =>
        {
            auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, true);
            uMan.Start();
            auMan.CleanAudioSources();
            SceneIsLoading = false;

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
                case Scene.WorldMapScene:
                    // blank
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
