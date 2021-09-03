using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private GameObject screenDimmer;
    private GameObject infoPopup;
    private GameObject combatEndPopup;
    private GameObject turnPopup;
    private GameObject menuPopup;
    private GameObject endTurnButton;
    private Coroutine sceneFadeRoutine;
    private GameObject playerZoneOutline;

    [SerializeField] private GameObject screenDimmerPrefab;
    [SerializeField] private GameObject infoPopupPrefab;
    [SerializeField] private GameObject combatEndPopupPrefab;
    [SerializeField] private GameObject turnPopupPrefab;
    [SerializeField] private GameObject versusPopupPrefab;
    [SerializeField] private GameObject sceneFader;
    [SerializeField] private GameObject menuPopupPrefab;
    [SerializeField] private GameObject explicitLanguagePopupPrefab;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color selectedColor;

    public GameObject ExplicitLanguagePopup { get; private set; }
    
    public bool PlayerIsTargetting { get; set; }
    public bool PlayerIsDiscarding { get; set; }
    public GameObject CurrentWorldSpace { get; private set; }
    public GameObject CurrentCanvas { get; private set; }

    public void Start()
    {
        CurrentWorldSpace = GameObject.Find("WorldSpace");
        CurrentCanvas = GameObject.Find("Canvas");
        PlayerIsTargetting = false;
    }

    /******
     * *****
     * ****** SET_PLAYER_ZONE_OUTLINE
     * *****
     *****/
    public void SetPlayerZoneOutline(bool enabled, bool selected)
    {
        playerZoneOutline.SetActive(enabled);
        if (enabled)
        {
            SpriteRenderer sr = playerZoneOutline.GetComponentInChildren<SpriteRenderer>();
            if (selected) sr.color = selectedColor;
            else sr.color = highlightedColor;
        }
    }

    /******
     * *****
     * ****** SET_SCENE_FADER
     * *****
     *****/
    public void SetSceneFader(bool fadeOut)
    {
        if (sceneFadeRoutine != null)
        {
            StopCoroutine(sceneFadeRoutine);
            sceneFadeRoutine = null;
        }
        sceneFadeRoutine = StartCoroutine(FadeSceneNumerator(fadeOut));
    }
    private IEnumerator FadeSceneNumerator(bool fadeOut)
    {
        Image img = sceneFader.GetComponent<Image>();
        float alphaChange = 0.015f;

        if (fadeOut)
        {
            while (img.color.a < 1)
            {
                var tempColor = img.color;
                tempColor.a = img.color.a + alphaChange;
                if (tempColor.a > 1) tempColor.a = 1;
                img.color = tempColor;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (img.color.a > 0)
            {
                var tempColor = img.color;
                tempColor.a = img.color.a - alphaChange;
                if (tempColor.a < 0) tempColor.a = 0;
                img.color = tempColor;
                yield return new WaitForFixedUpdate();
            }
        }
        sceneFadeRoutine = null;
    }

    /******
     * *****
     * ****** CREATE_DESTROY_EXPLICIT_LANGUAGE_POPUP
     * *****
     *****/
    public void CreateExplicitLanguagePopup()
    {
        if (ExplicitLanguagePopup != null) return;
        ExplicitLanguagePopup = Instantiate(explicitLanguagePopupPrefab, CurrentCanvas.transform);
    }
    public void DestroyExplicitLanguagePopup()
    {
        if (ExplicitLanguagePopup != null)
        {
            Destroy(ExplicitLanguagePopup);
            ExplicitLanguagePopup = null;
        }
    }

    /******
     * *****
     * ****** CREATE_DESTROY_MENU_POPUP
     * *****
     *****/
    public void CreateMenuPopup()
    {
        if (menuPopup != null) return;
        GameObject uiCanvas = GameObject.Find("UI_Canvas");
        menuPopup = Instantiate(menuPopupPrefab, uiCanvas.transform);
    }
    public void DestroyMenuPopup()
    {
        if (menuPopup != null)
        {
            Destroy(menuPopup);
            menuPopup = null;
        }
    }

    /******
     * *****
     * ****** START_COMBAT_SCENE
     * *****
     *****/
    public void StartCombatScene()
    {
        endTurnButton = GameObject.Find("EndTurnButton");
        playerZoneOutline = GameObject.Find("PlayerZoneOutline");
        playerZoneOutline.SetActive(false);
    }

    /******
     * *****
     * ****** SELECT_TARGET
     * *****
     *****/
    public void SelectTarget(GameObject target, bool enabled, bool isSelected = false)
    {
        if (target.TryGetComponent(out CardSelect cs))
        {
            cs.CardOutline.SetActive(enabled);
            SpriteRenderer sr = cs.CardOutline.GetComponent<SpriteRenderer>();
            if (enabled)
            {
                if (isSelected) sr.color = cs.SelectedColor;
                else sr.color = cs.HighlightedColor;
            }
        }
        else if (target.TryGetComponent(out HeroSelect hs))
        {
            hs.HeroOutline.SetActive(enabled);
            SpriteRenderer sr = hs.HeroOutline.GetComponentInChildren<SpriteRenderer>();
            if (enabled)
            {
                if (isSelected) sr.color = hs.SelectedColor;
                else sr.color = hs.HighlightedColor;
            }
        }
        else
        {
            Debug.LogError("TARGET SELECT SCRIPT NOT FOUND!");
            return;
        }
    }

    /******
     * *****
     * ****** SETTERS
     * *****
     *****/
    public void UpdateEndTurnButton(bool isMyTurn)
    {
        BoxCollider2D bc = endTurnButton.GetComponent<BoxCollider2D>();
        bc.enabled = isMyTurn;
        EndTurnButtonDisplay etbd = endTurnButton.GetComponent<EndTurnButtonDisplay>();
        etbd.EndTurnSide.SetActive(isMyTurn);
        etbd.OpponentTurnSide.SetActive(!isMyTurn);
    }

    /******
     * *****
     * ****** DESTROY_ZOOM_OBJECT(S)
     * *****
     *****/
    public void DestroyZoomObjects()
    {
        static void DestroyObject(GameObject go)
        {
            Destroy(go);
            go = null;
        }

        SetScreenDimmer(false);
        CardZoom.ZoomCardIsCentered = false;
        List<GameObject> objectsToDestroy = new List<GameObject>
        {
            CardZoom.CurrentZoomCard,
            CardZoom.DescriptionPopup,
            CardZoom.AbilityPopupBox,
            AbilityZoom.AbilityPopup
        };
        foreach (GameObject go in objectsToDestroy)
            if (go != null) DestroyObject(go);
    }

    /******
     * *****
     * ****** SET_SCREEN_DIMMER
     * *****
     *****/
    public void SetScreenDimmer(bool screenIsDimmed)
    {
        if (screenDimmer != null)
        {
            Destroy(screenDimmer);
            screenDimmer = null;
        }
        if (screenIsDimmed)
        {
            GameObject prefab = screenDimmerPrefab;
            GameObject parent = CurrentWorldSpace;
            screenDimmer = Instantiate(prefab, new Vector3(0, 0, -3), Quaternion.identity, parent.transform);
        }
    }

    /******
     * *****
     * ****** INFO_POPUP
     * *****
     *****/
    public void CreateInfoPopup(string message)
    {
        DestroyInfoPopup();
        infoPopup = Instantiate(infoPopupPrefab, new Vector2(680, 0), Quaternion.identity, CurrentWorldSpace.transform);
        infoPopup.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
    }
    public void CreateFleetinInfoPopup(string message)
    {
        CreateInfoPopup(message);
        AnimationManager.Instance.ChangeAnimationState(infoPopup, "Enter_Exit");
    }
    public void DismissInfoPopup()
    {
        if (infoPopup != null)
            AnimationManager.Instance.ChangeAnimationState(infoPopup, "Exit");
    }
    public void DestroyInfoPopup()
    {
        if (infoPopup != null)
        {
            Destroy(infoPopup);
            infoPopup = null;
        }
    }

    /******
     * *****
     * ****** TURN_POPUP
     * *****
     *****/
    public void CreateTurnPopup(bool isPlayerTurn)
    {
        DestroyTurnPopup();
        turnPopup = Instantiate(turnPopupPrefab, CurrentCanvas.transform);
        turnPopup.GetComponent<TurnPopupDisplay>().DisplayTurnPopup(isPlayerTurn);
    }
    public void DestroyTurnPopup()
    {
        if (turnPopup != null)
        {
            Destroy(turnPopup);
            turnPopup = null;
        }
    }

    /******
     * *****
     * ****** VERSUS_POPUP
     * *****
     *****/
    public void CreateVersusPopup()
    {
        DestroyTurnPopup();
        turnPopup = Instantiate(versusPopupPrefab, CurrentCanvas.transform);
    }

    /******
     * *****
     * ****** CREATE_COMBAT_END_POPUP
     * *****
     *****/
    public void CreateCombatEndPopup(bool playerWins)
    {
        combatEndPopup = Instantiate(combatEndPopupPrefab, CurrentCanvas.transform);
        CombatEndPopupDisplay cepd = combatEndPopup.GetComponent<CombatEndPopupDisplay>();
        cepd.VictoryText.SetActive(playerWins);
        cepd.DefeatText.SetActive(!playerWins);
        cepd.GetComponent<SoundPlayer>().PlaySound(0);
    }
    public void DestroyCombatEndPopup()
    {
        if (combatEndPopup != null)
        {
            Destroy(combatEndPopup);
            combatEndPopup = null;
        }
    }

    /******
     * *****
     * ****** GET_PORTRAIT_POSITION
     * *****
     *****/
    public class PositionAndScale
    {
        public PositionAndScale(Vector2 position, Vector2 scale)
        {
            Position = position;
            Scale = scale;
        }
        public Vector2 Position;
        public Vector2 Scale;
    }
    public PositionAndScale GetPortraitPosition(string heroName, SceneLoader.Scene scene)
    {
        Vector2 position = new Vector2();
        Vector2 scale = new Vector2();
        switch (heroName)
        {
            // KILI
            case "Kili, Neon Rider":
                switch(scene)
                {
                    case SceneLoader.Scene.DialogueScene:
                        position.Set(-90, -325);
                        scale.Set(3, 3);
                        break;
                    case SceneLoader.Scene.CombatScene:
                        position.Set(-60, -190);
                        scale.Set(2.5f, 2.5f);
                        break;
                }
                break;
            // YERGOV
            case "Yergov, Biochemist":
                switch (scene)
                {
                    case SceneLoader.Scene.DialogueScene:
                        position.Set(-145, -145);
                        scale.Set(2, 2);
                        break;
                    case SceneLoader.Scene.CombatScene:
                        position.Set(-100, -110);
                        scale.Set(2, 2);
                        break;
                }
                break;
            // FENTIS
            case "Fentis, Rogue Cyborg":
                switch (scene)
                {
                    case SceneLoader.Scene.DialogueScene:
                        position.Set(0, 0);
                        scale.Set(1, 1);
                        break;
                    case SceneLoader.Scene.CombatScene:
                        position.Set(0, 0);
                        scale.Set(3, 3);
                        break;
                }
                break;
            default:
                Debug.LogError("HERO NAME NOT FOUND!");
                return null;
        }
        return new PositionAndScale(position, scale);
    }
}
