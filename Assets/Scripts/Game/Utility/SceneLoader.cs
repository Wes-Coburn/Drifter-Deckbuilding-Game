using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class SceneLoader
{
    public static bool SceneIsLoading = false;
    public static float LoadingProgress;

    private static Action onSceneLoaderCallback, onSceneUpdateCallback;
    //public static Func<IEnumerator> LoadAction_Async;

    public static Coroutine CurrentLoadRoutine; // Load next content ('Tutorial OR 'Combat Test')
    public static Coroutine BackgroundLoadRoutine; // Load main content in background ('New Game' OR 'Continue Game')

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

    public static void LoadScene(Scene scene, Func<IEnumerator> loadAction, bool loadSameScene = false, bool fadeTransition = true)
    {
        if (SceneIsLoading || (!loadSameScene && IsActiveScene(scene))) return;

        if (loadAction == null)
        {
            Debug.LogError("LOAD ACTION IS NULL!");
            return;
        }

        if (BackgroundLoadRoutine != null) // TESTING
        {
            Managers.G_MAN.StopCoroutine(BackgroundLoadRoutine);
            BackgroundLoadRoutine = null;
        }

        LoadScene(scene, loadSameScene, fadeTransition);
        CurrentLoadRoutine = Managers.G_MAN.StartCoroutine(loadAction());
    }
    public static void LoadScene(Scene scene, bool loadSameScene = false, bool fadeTransition = true)
    {
        if (SceneIsLoading || (!loadSameScene && IsActiveScene(scene))) return;
        SceneIsLoading = true;
        LoadingProgress = -1;

        onSceneLoaderCallback = () =>
        {
            Managers.U_MAN.SetSkybar(false);
            Managers.U_MAN.SetSceneFader(false);

            var lsd = UnityEngine.Object.FindObjectOfType<LoadingSceneDisplay>();
            lsd.ChapterText = "Drifter";
            lsd.TipText = "";

            if (CurrentLoadRoutine == null && BackgroundLoadRoutine == null)
            {
                if (scene is not Scene.TitleScene or Scene.CombatScene)
                {
                    LoadScene_Finish(scene, SceneFinishType.Immediate);
                    return;
                }
            }
            //else LoadingProgress = 0;

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

            lsd.ChapterText = chapterText;
            lsd.TipText = Managers.G_MAN.CurrentTip;

            if (CurrentLoadRoutine == null && BackgroundLoadRoutine == null) // TESTING
                LoadScene_Finish(scene, SceneFinishType.Delayed);
        };

        onSceneUpdateCallback = () =>
        {
            Managers.U_MAN.Start();
            Managers.AU_MAN.CleanAudioSources();
            Managers.AU_MAN.StopCurrentSoundscape();
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
                    // HeroSelectSceneDisplay.Start() handles scene start
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
        //Managers.G_MAN.StopAllCoroutines();
        Managers.AN_MAN.StopAllCoroutines();
        Managers.D_MAN.StopAllCoroutines();
        Managers.U_MAN.StopAllCoroutines();

        // Reset Managers
        Managers.D_MAN.Reset_DialogueManager();
        Managers.EV_MAN.Reset_EventManager();
        Managers.EF_MAN.Reset_EffectManager();

        if (fadeTransition)
        {
            //FunctionTimer.Create(() => Managers.U_MAN.SetSceneFader(true), 0);
            Managers.U_MAN.SetSceneFader(true); // TESTING
            FunctionTimer.Create(() => SceneManager.LoadScene(Scene.LoadingScene.ToString()), 1.5f);
        }
        else SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public enum SceneFinishType
    {
        NoDelay,
        Delayed,
        Immediate,
    }
    public static void LoadScene_Finish(Scene scene, SceneFinishType finishType = SceneFinishType.NoDelay)
    {
        float delay;

        switch (finishType)
        {
            case SceneFinishType.NoDelay:
                delay = 0;
                break;
            case SceneFinishType.Delayed:
                delay = 4;
                break;
            case SceneFinishType.Immediate:
                LoadScene();
                return;
            default:
                Debug.LogError("INVALID TYPE!");
                return;
        }

        FunctionTimer.Create(() => { if (LoadingProgress >= 0) LoadingProgress = 1; }, delay);
        FunctionTimer.Create(() => Managers.U_MAN.SetSceneFader(true), delay);
        FunctionTimer.Create(() => LoadScene(), delay + 2);
        FunctionTimer.Create(() => Managers.U_MAN.SetSceneFader(false), delay + 2);

        void LoadScene() => SceneManager.LoadScene(scene.ToString());
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
