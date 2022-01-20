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
        CombatScene,
        CreditsScene
    }
    public static bool SceneIsLoading = false;
    
    public static bool IsActiveScene(Scene scene)
    {
        if (SceneManager.GetActiveScene().name == scene.ToString()) return true;
        else return false;
    }

    public static void LoadScene(Scene scene, bool loadSameScene = false)
    {
        if (SceneIsLoading) return;
        if (!loadSameScene && IsActiveScene(scene)) return;
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
                case Scene.CreditsScene:
                    chapterText = "CREDITS";
                    break;
                default:
                    Debug.LogError("SCENE TYPE NOT FOUND!");
                    return;
            }
            uMan.SetSkybar(false);
            UnityEngine.Object.FindObjectOfType<LoadingSceneDisplay>().ChapterText = chapterText;
            FunctionTimer.Create(() => uMan.SetSceneFader(true), 4f);
            FunctionTimer.Create(() => SceneManager.LoadScene(scene.ToString()), 6f);
            FunctionTimer.Create(() => uMan.SetSceneFader(false), 6f);
        };

        onSceneUpdateCallback = () =>
        {
            uMan.Start();
            auMan.CleanAudioSources();
            auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, true);
            SceneIsLoading = false;
            bool showSkybar = true;

            switch (scene)
            {
                case Scene.TitleScene:
                    showSkybar = false;
                    gMan.StartTitleScene();
                    break;
                case Scene.HeroSelectScene:
                    showSkybar = false;
                    auMan.StopCurrentSoundscape(); // TESTING
                    break;
                case Scene.NarrativeScene:
                    showSkybar = false;
                    gMan.StartNarrative();
                    break;
                case Scene.WorldMapScene:
                    gMan.EnterWorldMap();
                    break;
                case Scene.HomeBaseScene:
                    auMan.StopCurrentSoundscape(); // TESTING
                    break;
                case Scene.DialogueScene:
                    gMan.StartDialogue();
                    break;
                case Scene.CombatScene:
                    gMan.StartCombat();
                    break;
                case Scene.CreditsScene:
                    showSkybar = false;
                    gMan.StartCredits(); // TESTING
                    break;
                default:
                    Debug.LogError("SCENE NOT FOUND!");
                    break;
            }
            uMan.SetSkybar(showSkybar);
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
