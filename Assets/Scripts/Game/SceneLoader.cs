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

        UIManager uMan = UIManager.Instance;
        AudioManager auMan = AudioManager.Instance;
        GameManager gMan = GameManager.Instance;
        DialogueManager dMan = DialogueManager.Instance;
        CombatManager coMan = CombatManager.Instance;
        PlayerManager pMan = PlayerManager.Instance;
        AnimationManager anMan = AnimationManager.Instance;
        EventManager evMan = EventManager.Instance;
        EffectManager efMan = EffectManager.Instance;

        onSceneLoaderCallback = () =>
        {
            uMan.SetSkybar(false);
            uMan.SetSceneFader(false);
            auMan.StartStopSound("SFX_SceneLoading", null,
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
                    if (gMan.CurrentNarrative == null) chapterText = "Welcome to the Drift";
                    else chapterText = gMan.CurrentNarrative.NarrativeName;
                    break;
                case Scene.WorldMapScene:
                    chapterText = "World Map";
                    break;
                case Scene.HomeBaseScene:
                    chapterText = "Your Ship";
                    break;
                case Scene.DialogueScene:
                    chapterText = gMan.CurrentLocation.LocationFullName;
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
            lsd.TipText = gMan.CurrentTip;

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
                FunctionTimer.Create(() => uMan.SetSceneFader(true), 4);
                FunctionTimer.Create(() => SceneManager.LoadScene(scene.ToString()), 6);
                FunctionTimer.Create(() => uMan.SetSceneFader(false), 6);
            }
        };

        onSceneUpdateCallback = () =>
        {
            uMan.Start();
            auMan.CleanAudioSources();
            auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, true);
            SceneIsLoading = false;
            bool showSkybar = true;
            bool hideChildren = false;

            switch (scene)
            {
                case Scene.TitleScene:
                    showSkybar = false;
                    gMan.StartTitleScene();
                    break;
                case Scene.HeroSelectScene:
                    showSkybar = false;
                    gMan.StartHeroSelectScene();
                    break;
                case Scene.NarrativeScene:
                    showSkybar = false;
                    gMan.StartNarrative();
                    break;
                case Scene.WorldMapScene:
                    gMan.EnterWorldMap();
                    break;
                case Scene.HomeBaseScene:
                    gMan.EnterHomeBase();
                    break;
                case Scene.DialogueScene:
                    gMan.StartDialogue();
                    break;
                case Scene.CombatScene:
                    gMan.StartCombat();
                    hideChildren = true;
                    break;
                case Scene.CreditsScene:
                    showSkybar = false;
                    gMan.StartCredits();
                    break;
                default:
                    Debug.LogError("SCENE NOT FOUND!");
                    break;
            }
            uMan.SetSkybar(showSkybar, hideChildren);
        };

        // Stop Corotoutines
        gMan.StopAllCoroutines();
        anMan.StopAllCoroutines();
        dMan.StopAllCoroutines();
        uMan.StopAllCoroutines();

        // Reset Managers
        dMan.Reset_DialogueManager();
        evMan.Reset_EventManager();
        efMan.Reset_EffectManager();

        if (fadeTransition)
        {
            FunctionTimer.Create(() => uMan.SetSceneFader(true), 0f);
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
