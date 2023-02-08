using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static Action onSceneLoaderCallback;
    private static Action onSceneUpdateCallback;
    public static bool SceneIsLoading = false;
    public static Action LoadAction;

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

    public static bool IsActiveScene(Scene scene)
    {
        if (SceneManager.GetActiveScene().name == scene.ToString()) return true;
        else return false;
    }

    public static void LoadScene(Scene scene, bool loadSameScene = false, bool fadeTransition = true)
    {
        if (SceneIsLoading) return;
        if (!loadSameScene && IsActiveScene(scene)) return;
        SceneIsLoading = true;

        onSceneLoaderCallback = () =>
        {
            ManagerHandler.U_MAN.SetSkybar(false);
            ManagerHandler.U_MAN.SetSceneFader(false);
            ManagerHandler.AU_MAN.StartStopSound("SFX_SceneLoading", null,
                AudioManager.SoundType.SFX, false, true);

            string chapterText;
            switch (scene)
            {
                case Scene.TitleScene:
                    chapterText = "Main Menu";
                    break;
                case Scene.HeroSelectScene:
                    chapterText = "Choose Your Drifter";
                    break;
                case Scene.NarrativeScene:
                    if (ManagerHandler.G_MAN.CurrentNarrative == null) chapterText = "Welcome to the Drift";
                    else chapterText = ManagerHandler.G_MAN.CurrentNarrative.NarrativeName;
                    break;
                case Scene.WorldMapScene:
                    chapterText = "World Map";
                    break;
                case Scene.HomeBaseScene:
                    chapterText = "Your Ship";
                    break;
                case Scene.DialogueScene:
                    chapterText = ManagerHandler.G_MAN.CurrentLocation.LocationFullName;
                    break;
                case Scene.CombatScene:
                    chapterText = "Combat";
                    break;
                case Scene.CreditsScene:
                    chapterText = "Credits";
                    break;
                default:
                    chapterText = "Drifter";
                    Debug.LogError("SCENE TYPE NOT FOUND!");
                    break;
            }

            LoadingSceneDisplay lsd = UnityEngine.Object.FindObjectOfType<LoadingSceneDisplay>();
            lsd.ChapterText = chapterText;
            lsd.TipText = ManagerHandler.G_MAN.CurrentTip;

            if (LoadAction != null) FunctionTimer.Create(() => InvokeLoadAction(), 2);
            else LoadScene_Finish();

            void InvokeLoadAction()
            {
                LoadAction?.Invoke();
                LoadAction = null;
                LoadScene_Finish();
            }
            void LoadScene_Finish()
            {
                FunctionTimer.Create(() => ManagerHandler.U_MAN.SetSceneFader(true), 4);
                FunctionTimer.Create(() => SceneManager.LoadScene(scene.ToString()), 6);
                FunctionTimer.Create(() => ManagerHandler.U_MAN.SetSceneFader(false), 6);
            }
        };

        onSceneUpdateCallback = () =>
        {
            ManagerHandler.U_MAN.Start();
            ManagerHandler.AU_MAN.CleanAudioSources();
            ManagerHandler.AU_MAN.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, true);
            SceneIsLoading = false;
            bool showSkybar = true;
            bool hideChildren = false;

            switch (scene)
            {
                case Scene.TitleScene:
                    showSkybar = false;
                    ManagerHandler.G_MAN.StartTitleScene();
                    break;
                case Scene.HeroSelectScene:
                    showSkybar = false;
                    ManagerHandler.G_MAN.StartHeroSelectScene();
                    break;
                case Scene.NarrativeScene:
                    showSkybar = false;
                    ManagerHandler.G_MAN.StartNarrative();
                    break;
                case Scene.WorldMapScene:
                    ManagerHandler.G_MAN.EnterWorldMap();
                    break;
                case Scene.HomeBaseScene:
                    ManagerHandler.G_MAN.EnterHomeBase();
                    break;
                case Scene.DialogueScene:
                    ManagerHandler.G_MAN.StartDialogue();
                    break;
                case Scene.CombatScene:
                    ManagerHandler.G_MAN.StartCombat();
                    hideChildren = true;
                    break;
                case Scene.CreditsScene:
                    showSkybar = false;
                    ManagerHandler.G_MAN.StartCredits();
                    break;
                default:
                    Debug.LogError("SCENE NOT FOUND!");
                    break;
            }
            ManagerHandler.U_MAN.SetSkybar(showSkybar, hideChildren);
        };

        // Stop Corotoutines
        ManagerHandler.G_MAN.StopAllCoroutines();
        ManagerHandler.AN_MAN.StopAllCoroutines();
        ManagerHandler.D_MAN.StopAllCoroutines();
        ManagerHandler.U_MAN.StopAllCoroutines();

        // Reset Managers
        ManagerHandler.D_MAN.Reset_DialogueManager();
        ManagerHandler.EV_MAN.Reset_EventManager();
        ManagerHandler.EF_MAN.Reset_EffectManager();

        if (fadeTransition)
        {
            FunctionTimer.Create(() => ManagerHandler.U_MAN.SetSceneFader(true), 0f);
            FunctionTimer.Create(() => SceneManager.LoadScene(Scene.LoadingScene.ToString()), 1.5f);
        }
        else SceneManager.LoadScene(Scene.LoadingScene.ToString());
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
