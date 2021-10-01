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
        HomeBaseScene,
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
        FunctionTimer.Create(() => auMan.StartStopSound("SFX_SceneLoading", null, 
            AudioManager.SoundType.SFX, false, true), 1f);

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
                case Scene.HomeBaseScene:
                    chapterText = "YOUR SHIP";
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
            uMan.SetSkybar(false); // TESTING
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
            bool showSkybar = true; // TESTING

            switch (scene)
            {
                case Scene.TitleScene:
                    showSkybar = false;
                    auMan.StartStopSound("Soundtrack_TitleScene", null, 
                        AudioManager.SoundType.Soundtrack);
                    break;
                case Scene.HeroSelectScene:
                    showSkybar = false;
                    UnityEngine.Object.FindObjectOfType<HeroSelectSceneDisplay>().DisplaySelectedHero();
                    break;
                case Scene.NarrativeScene:
                    showSkybar = false;
                    gMan.StartNarrative();
                    break;
                case Scene.WorldMapScene:
                    uMan.StartWorldMapScene();
                    gMan.EnterWorldMap();
                    break;
                case Scene.HomeBaseScene:
                    Debug.LogWarning("BLANK!");
                    break;
                case Scene.DialogueScene:
                    dMan.StartDialogue();
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
            uMan.SetSkybar(showSkybar); // TESTING
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
