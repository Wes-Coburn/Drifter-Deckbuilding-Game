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
        HeroSelectScene,
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
        PlayerManager pMan = PlayerManager.Instance;
        FunctionTimer.Create(() => 
        auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, false, true), 1f);

        onSceneLoaderCallback = () =>
        {
            string chapterText;
            switch (scene)
            {
                case Scene.TitleScene:
                    chapterText = "MAIN MENU";
                    break;
                case Scene.HeroSelectScene:
                    chapterText = gMan.NextChapter;
                    break;
                case Scene.NarrativeScene:
                    chapterText = gMan.NextChapter;
                    break;
                case Scene.WorldMapScene:
                    chapterText = "WORLD MAP";
                    break;
                case Scene.DialogueScene:
                    chapterText = gMan.CurrentLocation.LocationFullName;
                    break;
                case Scene.CombatScene:
                    chapterText = "COMBAT";
                    break;
                default:
                    Debug.LogError("SCENE TYPE NOT FOUND!");
                    return;
            }
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
                case Scene.HeroSelectScene:
                    // blank
                    break;
                case Scene.NarrativeScene:
                    gMan.StartNarrative();
                    break;
                case Scene.WorldMapScene:
                    uMan.StartWorldMapScene();
                    gMan.EnterWorldMap(); // TESTING
                    break;
                case Scene.DialogueScene:
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
