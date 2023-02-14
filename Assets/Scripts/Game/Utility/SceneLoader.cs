using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static Action onSceneLoaderCallback;
    private static Action onSceneUpdateCallback;

    public static bool SceneIsLoading = false;
    public static Action LoadAction;
    public static Func<IEnumerator> LoadAction_Async;
    public static float LoadingProgress;

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
    
    public static bool IsActiveScene(Scene scene) => SceneManager.GetActiveScene().name == scene.ToString();

    public static void LoadScene(Scene scene, bool loadSameScene = false, bool fadeTransition = true)
    {
        if (SceneIsLoading || (!loadSameScene && IsActiveScene(scene))) return;
        SceneIsLoading = true;
        LoadingProgress = -1;

        onSceneLoaderCallback = () =>
        {
            Managers.U_MAN.SetSkybar(false);
            Managers.U_MAN.SetSceneFader(false);

            if (LoadAction == null && LoadAction_Async == null)
            {
                if (scene != Scene.TitleScene && scene != Scene.CombatScene)
                {
                    LoadScene_Finish(scene, true);
                    return;
                }
            }
            else LoadingProgress = 0;

            Managers.AU_MAN.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, false, true);

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
                    if (Managers.G_MAN.CurrentNarrative == null) chapterText = "Welcome to the Drift";
                    else chapterText = Managers.G_MAN.CurrentNarrative.NarrativeName;
                    break;
                case Scene.WorldMapScene:
                    chapterText = "World Map";
                    break;
                case Scene.HomeBaseScene:
                    chapterText = "Your Ship";
                    break;
                case Scene.DialogueScene:
                    chapterText = Managers.G_MAN.CurrentLocation.LocationFullName;
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

            var lsd = UnityEngine.Object.FindObjectOfType<LoadingSceneDisplay>();
            lsd.ChapterText = chapterText;
            lsd.TipText = Managers.G_MAN.CurrentTip;

            if (LoadAction != null) FunctionTimer.Create(() => InvokeLoadAction(), 2);
            else if (LoadAction_Async != null)
            {
                Managers.G_MAN.StartCoroutine(LoadAction_Async());
                LoadAction_Async = null;
            }
            else LoadScene_Finish(scene);

            void InvokeLoadAction()
            {
                LoadAction();
                LoadAction = null;
                LoadScene_Finish(scene);
            }
        };

        onSceneUpdateCallback = () =>
        {
            Managers.U_MAN.Start();
            Managers.AU_MAN.CleanAudioSources();
            Managers.AU_MAN.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, true);
            SceneIsLoading = false;
            bool showSkybar = true;
            bool hideChildren = false;

            switch (scene)
            {
                case Scene.TitleScene:
                    showSkybar = false;
                    Managers.G_MAN.StartTitleScene();
                    break;
                case Scene.HeroSelectScene:
                    showSkybar = false;
                    Managers.G_MAN.StartHeroSelectScene();
                    break;
                case Scene.NarrativeScene:
                    showSkybar = false;
                    Managers.G_MAN.StartNarrativeScene();
                    break;
                case Scene.WorldMapScene:
                    Managers.G_MAN.StartWorldMapScene();
                    break;
                case Scene.HomeBaseScene:
                    Managers.G_MAN.StartHomeBaseScene();
                    break;
                case Scene.DialogueScene:
                    Managers.D_MAN.StartDialogue();
                    break;
                case Scene.CombatScene:
                    Managers.CO_MAN.StartCombat();
                    hideChildren = true;
                    break;
                case Scene.CreditsScene:
                    showSkybar = false;
                    Managers.G_MAN.StartCreditsScene();
                    break;
                default:
                    Debug.LogError("SCENE NOT FOUND!");
                    break;
            }
            Managers.U_MAN.SetSkybar(showSkybar, hideChildren);
        };

        // Stop Corotoutines
        Managers.G_MAN.StopAllCoroutines();
        Managers.AN_MAN.StopAllCoroutines();
        Managers.D_MAN.StopAllCoroutines();
        Managers.U_MAN.StopAllCoroutines();

        // Reset Managers
        Managers.D_MAN.Reset_DialogueManager();
        Managers.EV_MAN.Reset_EventManager();
        Managers.EF_MAN.Reset_EffectManager();

        if (fadeTransition)
        {
            FunctionTimer.Create(() => Managers.U_MAN.SetSceneFader(true), 0f);
            FunctionTimer.Create(() => SceneManager.LoadScene(Scene.LoadingScene.ToString()), 1.5f);
        }
        else SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadScene_Finish(Scene scene, bool immediate = false)
    {
        if (immediate)
        {
            SceneManager.LoadScene(scene.ToString());
            return;
        }

        FunctionTimer.Create(() => Managers.U_MAN.SetSceneFader(true), 4);
        FunctionTimer.Create(() => SceneManager.LoadScene(scene.ToString()), 6);
        FunctionTimer.Create(() => Managers.U_MAN.SetSceneFader(false), 6);
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
