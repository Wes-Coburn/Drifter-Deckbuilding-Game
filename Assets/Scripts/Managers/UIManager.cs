using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;

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

            SetSkybar(false);
            //StartCoroutine(WaitForSplash());
        }
        else Destroy(gameObject);
    }

    [Header("SKYBAR")]
    [SerializeField] private GameObject skyBar;
    [SerializeField] private GameObject augmentBar;
    [SerializeField] private GameObject augmentsDropdown;
    [SerializeField] private GameObject itemBar;
    [SerializeField] private GameObject itemsDropdown;
    [SerializeField] private GameObject itemsCount;
    [SerializeField] private GameObject reputationBar;
    [SerializeField] private GameObject reputationsDropdown;
    [SerializeField] private GameObject aetherCount;
    [SerializeField] private GameObject aetherIcon;

    [Header("REPUTATION")]
    [SerializeField] private GameObject reputation_Mages;
    [SerializeField] private GameObject reputation_Mutants;
    [SerializeField] private GameObject reputation_Rogues;
    [SerializeField] private GameObject reputation_Techs;
    [SerializeField] private GameObject reputation_Warriors;

    [Header("SCENE FADER")]
    [SerializeField] private GameObject sceneFader;

    [Header("PREFABS")]
    [SerializeField] private GameObject screenDimmerPrefab;
    [SerializeField] private GameObject tooltipPopupPrefab;
    [SerializeField] private GameObject infoPopupPrefab;
    [SerializeField] private GameObject infoPopup_SecondaryPrefab;
    [SerializeField] private GameObject combatEndPopupPrefab;
    [SerializeField] private GameObject turnPopupPrefab;
    [SerializeField] private GameObject versusPopupPrefab;
    [SerializeField] private GameObject menuPopupPrefab;
    [SerializeField] private GameObject explicitLanguagePopupPrefab;
    [SerializeField] private GameObject tutorialPopupPrefab;
    [SerializeField] private GameObject tutorialActionPopupPrefab;
    [SerializeField] private GameObject newCardPopupPrefab;
    [SerializeField] private GameObject chooseCardPopupPrefab;
    [SerializeField] private GameObject chooseRewardPopupPrefab;
    [SerializeField] private GameObject aetherCellPopupPrefab;
    [SerializeField] private GameObject cardPagePopupPrefab;
    [SerializeField] private GameObject cardScrollPopupPrefab;
    [SerializeField] private GameObject recruitUnitPopupPrefab;
    [SerializeField] private GameObject removeCardPopupPrefab;
    [SerializeField] private GameObject cloneUnitPopupPrefab;
    [SerializeField] private GameObject newAugmentPopupPrefab;
    [SerializeField] private GameObject locationPopupPrefab;
    [SerializeField] private GameObject narrativePopupPrefab;
    [SerializeField] private GameObject augmentIconPrefab;
    [SerializeField] private GameObject augmentIconPopupPrefab;
    [SerializeField] private GameObject itemPagePopupPrefab;
    [SerializeField] private GameObject buyItemPopupPrefab;
    [SerializeField] private GameObject itemIconPrefab;
    [SerializeField] private GameObject itemIconPopupPrefab;
    [SerializeField] private GameObject abilityPopupPrefab;
    [SerializeField] private GameObject abilityPopupBoxPrefab;
    [SerializeField] private GameObject reputationPopupPrefab;
    [SerializeField] private GameObject gameEndPopupPrefab;

    [Header("COLORS")]
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color rejectedColor;
    [SerializeField] private Color playableColor;

    private PlayerManager pMan;
    private DialogueManager dMan;
    private AnimationManager anMan;
    private GameManager gMan;

    private GameObject screenDimmer;
    private GameObject tooltipPopup;
    private GameObject infoPopup;
    private GameObject infoPopup_Secondary;
    private GameObject infoPopup_Tutorial;
    private GameObject combatEndPopup;
    private GameObject turnPopup;
    private GameObject menuPopup;
    private GameObject explicitLanguagePopup;
    private GameObject tutorialPopup;
    private GameObject tutorialActionPopup;
    private GameObject newCardPopup;
    private GameObject chooseCardPopup;
    private GameObject chooseRewardPopup;
    private GameObject aetherCellPopup;
    private GameObject cardPagePopup;
    private GameObject recruitUnitPopup;
    private GameObject removeCardPopup;
    private GameObject cloneUnitPopup;
    private GameObject newAugmentPopup;
    private GameObject itemPagePopup;
    private GameObject buyItemPopup;
    private GameObject locationPopup;
    private GameObject travelPopup;
    private GameObject narrativePopup;
    private GameObject augmentIconPopup;
    private GameObject itemIconPopup;
    private GameObject itemAbilityPopup;
    private GameObject reputationPopup;

    private GameObject endTurnButton;
    private GameObject cancelEffectButton;
    private GameObject confirmEffectButton;
    private Coroutine sceneFadeRoutine;
    private GameObject playerZoneOutline;
    private CombatLog combatLog;

    public const string TOOLTIP_TIMER = "TooltipTimer";

    public GameObject NewCardPopup { get => newCardPopup; }
    public GameObject ChooseCardPopup { get => chooseCardPopup; }
    public GameObject CardPagePopup { get => cardPagePopup; }
    public GameObject ConfirmUseItemPopup { get; set; }
    public GameObject EndTurnButton { get => endTurnButton; }
    public GameObject AugmentBar { get => augmentBar; }
    public GameObject AugmentsDropdown { get => augmentsDropdown; }
    public GameObject ItemBar { get => itemBar; }
    public GameObject ItemsDropdown { get => itemsDropdown; }
    public GameObject ReputationBar { get => reputationBar; }
    public GameObject ReputationsDropdown { get => reputationsDropdown; }
    public Color HighlightedColor { get => highlightedColor; }
    public GameObject CombatLog { get => combatLog.gameObject; }

    // PLAYER_IS_TARGETTING
    public bool PlayerIsTargetting { get; set; }
    // WORLDSPACE
    public GameObject CurrentWorldSpace { get; private set; }
    // CANVAS
    public GameObject CurrentCanvas { get; private set; }
    // ZOOM CANVAS
    public GameObject CurrentZoomCanvas { get; private set; }
    // UI CANVAS
    public GameObject UICanvas { get; set; }

    public void Start()
    {
        pMan = PlayerManager.Instance;
        dMan = DialogueManager.Instance;
        anMan = AnimationManager.Instance;
        gMan = GameManager.Instance;

        PlayerIsTargetting = false;
        CurrentWorldSpace = GameObject.Find("WorldSpace");
        CurrentCanvas = GameObject.Find("Canvas");
        UICanvas = GameObject.Find("UI_Canvas");
        CurrentZoomCanvas = GameObject.Find("Canvas_Zoom");
    }

    /*
    private IEnumerator WaitForSplash()
    {
        while (!UnityEngine.Rendering.SplashScreen.isFinished) 
            yield return null;
        yield return new WaitForSeconds(1);
        SetSceneFader(false);
    }
    */

    /******
     * *****
     * ****** START_COMBAT_SCENE
     * *****
     *****/
    public void StartCombatScene()
    {
        cancelEffectButton = GameObject.Find("CancelEffectButton");
        confirmEffectButton = GameObject.Find("ConfirmEffectButton");
        endTurnButton = GameObject.Find("EndTurnButton");
        playerZoneOutline = GameObject.Find("PlayerZoneOutline");
        combatLog = FindObjectOfType<CombatLog>();
        
        SetCancelEffectButton(false);
        SetConfirmEffectButton(false);
        SetPlayerZoneOutline(false, false);
    }

    /******
     * *****
     * ****** SHAKE_CAMERA
     * *****
     *****/
    public void ShakeCamera(CameraShakeInstance shake) =>
        CameraShaker.Instance.Shake(shake);

    public static CameraShakeInstance Bump_Light
    {
        get
        {
            CameraShakeInstance c = new CameraShakeInstance(2.5f, 4, 0.1f, 0.75f)
            {
                PositionInfluence = Vector3.one * 0.15f,
                RotationInfluence = Vector3.one * 0.5f // TESTING
            };
            return c;
        }
    }

    /******
     * *****
     * ****** PLAYER_ZONE_OUTLINE
     * *****
     *****/
    public void SetPlayerZoneOutline(bool enabled, bool selected)
    {
        playerZoneOutline.SetActive(enabled);
        if (enabled)
        {
            SpriteRenderer sr = playerZoneOutline.GetComponentInChildren<SpriteRenderer>();
            Color col;
            if (!selected) col = selectedColor;
            else col = highlightedColor;
            col.a = 0.3f;
            sr.color = col;
        }
    }

    /******
     * *****
     * ****** SCENE_FADER
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
     * ****** SCREEN_DIMMER
     * *****
     *****/
    public void SetScreenDimmer(bool screenIsDimmed)
    {
        if (screenDimmer != null)
        {
            Destroy(screenDimmer);
            screenDimmer = null;
        }
        if (screenIsDimmed) screenDimmer =
                Instantiate(screenDimmerPrefab, CurrentZoomCanvas.transform);
    }
    /******
     * *****
     * ****** END_TURN_BUTTON
     * *****
     *****/
    public void UpdateEndTurnButton(bool isInteractable = true)
    {
        if (endTurnButton == null) return;

        bool isMyTurn = pMan.IsMyTurn;
        EndTurnButtonDisplay etbd = endTurnButton.GetComponent<EndTurnButtonDisplay>();
        etbd.EndTurnSide.SetActive(isMyTurn);
        etbd.OpponentTurnSide.SetActive(!isMyTurn);

        if (!isMyTurn) isInteractable = false;
        Button[] buttons = endTurnButton.GetComponentsInChildren<Button>();
        foreach (Button b in buttons) b.interactable = isInteractable;
    }
    public void SetReadyEndTurnButton(bool isReady)
    {
        if (endTurnButton == null) return;

        EndTurnButtonDisplay etbd = endTurnButton.GetComponent<EndTurnButtonDisplay>();
        Button etb = etbd.EndTurnSide.GetComponent<Button>();

        Color normalColor;
        Color highlightedColor;
        Color disabledColor;

        if (isReady)
        {
            normalColor = Color.white;
            highlightedColor = Color.gray;
            disabledColor = Color.gray;
        }
        else
        {
            normalColor = Color.black;
            highlightedColor = Color.red;
            disabledColor = Color.black;
        }

        disabledColor.a = 0.3f;

        var btnClr = etb.colors;
        btnClr.normalColor = normalColor;
        btnClr.highlightedColor = highlightedColor;
        btnClr.disabledColor = disabledColor;
        etb.colors = btnClr;
    }
    /******
     * *****
     * ****** CANCEL/CONFIRM_EFFECT_BUTTON
     * *****
     *****/
    public void SetCancelEffectButton(bool isEnabled) =>
        cancelEffectButton.SetActive(isEnabled);
    public void SetConfirmEffectButton(bool isEnabled) =>
        confirmEffectButton.SetActive(isEnabled);
    /******
     * *****
     * ****** SELECT_TARGET
     * *****
     *****/
    public enum SelectionType
    {
        Disabled,
        Highlighted,
        Selected,
        Rejected,
        Playable
    }
    public void SelectTarget(GameObject target, SelectionType selectionType)
    {
        if (target == null)
        {
            Debug.LogError("TARGET IS NULL!");
            return;
        }

        if (target.TryGetComponent(out CardSelect cs))
        {
            if (selectionType is SelectionType.Disabled)
            {
                cs.CardOutline.SetActive(false);
                RemoveBuffer(); // TESTING
                return;
            }
            else cs.CardOutline.SetActive(true);

            Image image = cs.CardOutline.GetComponent<Image>();
            if (enabled) SetColor(image);

            if (selectionType is SelectionType.Selected && PlayerIsTargetting)
            {
                if (EffectManager.Instance.CurrentEffect is DrawEffect de && de.IsDiscardEffect)
                {
                    target.GetComponent<CardDisplay>().CardContainer.GetComponent
                        <CardContainer>().BufferDistance = new Vector2(0, -50);
                }
            }
            else RemoveBuffer();

            void RemoveBuffer() => target.GetComponent<CardDisplay>().CardContainer.GetComponent
                <CardContainer>().BufferDistance = new Vector2(0, 0);
        }
        else if (target.TryGetComponent(out HeroSelect hs))
        {
            if (selectionType is SelectionType.Disabled) // TESTING
            {
                hs.HeroOutline.SetActive(false);
                return;
            }
            else hs.HeroOutline.SetActive(true);

            Image image = hs.HeroOutline.GetComponentInChildren<Image>();
            if (enabled) SetColor(image);
        }
        else
        {
            Debug.LogError("INVALID TARGET!");
            return;
        }

        void SetColor(Image image)
        {
            Color color;
            switch (selectionType)
            {
                case SelectionType.Highlighted:
                    color = highlightedColor;
                    break;
                case SelectionType.Selected:
                    color = selectedColor;
                    break;
                case SelectionType.Rejected:
                    color = rejectedColor;
                    break;
                case SelectionType.Playable:
                    color = playableColor;
                    break;
                default:
                    Debug.LogError("INVALID TYPE!");
                    return;
            }
            image.color = color;
        }
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

        if (SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene))
            HideSkybar(false);

        SetScreenDimmer(false);
        CardZoom.ZoomCardIsCentered = false;
        FunctionTimer.StopTimer(CardZoom.ZOOM_CARD_TIMER);
        FunctionTimer.StopTimer(CardZoom.ABILITY_POPUP_TIMER);

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
     * ****** GET_PORTRAIT_POSITION
     * *****
     *****/
    public void GetPortraitPosition(string heroName, out Vector2 position,
        out Vector2 scale, SceneLoader.Scene scene)
    {
        position = new Vector2();
        scale = new Vector2();
        switch (heroName)
        {
            // FAYDRA
            case "Faydra, Rogue Cyborg":
                switch (scene)
                {
                    case SceneLoader.Scene.HeroSelectScene:
                        position.Set(-40, 0);
                        scale.Set(1.3f, 1.3f);
                        break;
                    case SceneLoader.Scene.DialogueScene:
                        position.Set(-150, -75);
                        scale.Set(2.5f, 2.5f);
                        break;
                    case SceneLoader.Scene.CombatScene: // done
                        position.Set(-90, -20);
                        scale.Set(1.2f, 1.2f);
                        break;
                }
                break;
            // GENZER
            case "Genzer, Evolved Mutant":
                switch (scene)
                {
                    case SceneLoader.Scene.HeroSelectScene:
                        position.Set(0, 0);
                        scale.Set(1.3f, 1.3f);
                        break;
                    case SceneLoader.Scene.DialogueScene:
                        position.Set(0, -100);
                        scale.Set(2f, 2f);
                        break;
                    case SceneLoader.Scene.CombatScene: // done
                        position.Set(5, -20);
                        scale.Set(1f, 1f);
                        break;
                }
                break;
            // KILI
            case "Kili, Neon Rider":
                switch (scene)
                {
                    case SceneLoader.Scene.HeroSelectScene:
                        position.Set(0, -30);
                        scale.Set(1.2f, 1.2f);
                        break;
                    case SceneLoader.Scene.DialogueScene:
                        position.Set(-90, -325);
                        scale.Set(3, 3);
                        break;
                    case SceneLoader.Scene.CombatScene: // done
                        position.Set(-30, -90);
                        scale.Set(1.2f, 1.2f);
                        break;
                }
                break;
            // MARDRIK
            case "Madrik, Obsessed Mage":
                switch (scene)
                {
                    case SceneLoader.Scene.HeroSelectScene:
                        position.Set(0, -10);
                        scale.Set(1.4f, 1.4f);
                        break;
                    case SceneLoader.Scene.DialogueScene:
                        position.Set(0, -45);
                        scale.Set(2f, 2f);
                        break;
                    case SceneLoader.Scene.CombatScene: // done
                        position.Set(0, -35);
                        scale.Set(1.2f, 1.2f);
                        break;
                }
                break;
            // YERGOV
            case "Yergov, Biochemist":
                switch (scene)
                {
                    case SceneLoader.Scene.HeroSelectScene:
                        position.Set(-15, -35);
                        scale.Set(1.3f, 1.3f);
                        break;
                    case SceneLoader.Scene.DialogueScene:
                        position.Set(-145, -145);
                        scale.Set(2, 2);
                        break;
                    case SceneLoader.Scene.CombatScene:
                        position.Set(-15, -45);
                        scale.Set(0.9f, 0.9f);
                        break;
                }
                break;
            default:
                Debug.LogError("HERO NAME NOT FOUND!");
                return;
        }
    }

    /******
     * *****
     * ****** COMBAT_LOG_ENTRY
     * *****
     *****/
    public void CombatLogEntry(string entry) =>
        combatLog.NewLogEntry(entry);
    public void CombatLog_PlayCard(GameObject card) =>
        combatLog.NewLogEntry_PlayCard(card);

    /******
     * *****
     * ****** POPUPS
     * *****
     *****/
    // Destroy Interactable Popup
    public void DestroyInteractablePopup(GameObject popup) =>
        anMan.ChangeAnimationState(popup, "Exit");
    // Tooltip Popup
    public void CreateTooltipPopup(Vector2 position, string text)
    {
        tooltipPopup = Instantiate(tooltipPopupPrefab, CurrentWorldSpace.transform);
        tooltipPopup.transform.localPosition = position;
        tooltipPopup.GetComponentInChildren<TextMeshPro>().SetText(text);
    }
    public void DestroyTooltipPopup()
    {
        FunctionTimer.StopTimer(TOOLTIP_TIMER);

        if (tooltipPopup != null)
        {
            Destroy(tooltipPopup);
            tooltipPopup = null;
        }
    }

    // Explicit Language Popup
    public void CreateExplicitLanguagePopup()
    {
        if (explicitLanguagePopup != null) return;
        explicitLanguagePopup = Instantiate(explicitLanguagePopupPrefab,
            CurrentCanvas.transform);
    }
    public void DestroyExplicitLanguagePopup()
    {
        if (explicitLanguagePopup != null)
        {
            Destroy(explicitLanguagePopup);
            explicitLanguagePopup = null;
        }
    }
    // Tutorial Popup
    public void CreateTutorialPopup()
    {
        if (tutorialPopup != null) return;
        tutorialPopup = Instantiate(tutorialPopupPrefab,
            CurrentCanvas.transform);
    }
    public void DestroyTutorialPopup()
    {
        if (tutorialPopup != null)
        {
            Destroy(tutorialPopup);
            tutorialPopup = null;
        }
    }
    // Tutorial Action Popup
    public void CreateTutorialActionPopup()
    {
        if (tutorialActionPopup != null) return;
        tutorialActionPopup = Instantiate(tutorialActionPopupPrefab,
            CurrentCanvas.transform);
    }
    public void DestroyTutorialActionPopup()
    {
        if (tutorialActionPopup != null)
        {
            Destroy(tutorialActionPopup);
            tutorialActionPopup = null;
        }
    }
    // Menu Popup
    public void CreateMenuPopup()
    {
        if (menuPopup != null) return;
        menuPopup = Instantiate(menuPopupPrefab, UICanvas.transform);

    }
    public void DestroyMenuPopup()
    {
        if (menuPopup != null)
        {
            Destroy(menuPopup);
            menuPopup = null;
        }
    }
    // Info Popups
    public enum InfoPopupType
    {
        Default,
        Secondary,
        Tutorial
    }
    public void CreateInfoPopup(string message, InfoPopupType infoPopupType, bool isCentered = false)
    {
        DestroyInfoPopup(infoPopupType);
        Vector2 vec2 = new Vector2();
        if (!isCentered) vec2.Set(750, 0);

        switch (infoPopupType)
        {
            case InfoPopupType.Default:
                infoPopup = Instantiate(infoPopupPrefab, CurrentCanvas.transform);
                infoPopup.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
                infoPopup.transform.localPosition = vec2;
                break;
            case InfoPopupType.Secondary:
                infoPopup_Secondary = Instantiate(infoPopup_SecondaryPrefab, UICanvas.transform);
                infoPopup_Secondary.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
                infoPopup_Secondary.transform.localPosition = vec2;
                break;
            case InfoPopupType.Tutorial:
                infoPopup_Tutorial = Instantiate(infoPopupPrefab, CurrentZoomCanvas.transform); // TESTING
                infoPopup_Tutorial.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
                Destroy(infoPopup_Tutorial.GetComponent<Animator>());
                infoPopup_Tutorial.transform.localPosition = vec2;
                break;
        }
    }
    public void CreateFleetingInfoPopup(string message, bool isCentered = false)
    {
        CreateInfoPopup(message, InfoPopupType.Secondary, isCentered);
        anMan.ChangeAnimationState(infoPopup_Secondary, "Enter_Exit");
    }
    public void InsufficientAetherPopup()
    {
        CreateFleetingInfoPopup("Not enough aether! (You have " + 
            pMan.AetherCells + " aether)", true);
    }
    public void DismissInfoPopup()
    {
        if (infoPopup != null)
            anMan.ChangeAnimationState(infoPopup, "Exit");
    }
    public void DestroyInfoPopup(InfoPopupType infoPopupType)
    {
        switch (infoPopupType)
        {
            case InfoPopupType.Default:
                if (infoPopup != null)
                {
                    Destroy(infoPopup);
                    infoPopup = null;
                }
                break;
            case InfoPopupType.Secondary:
                if (infoPopup_Secondary != null)
                {
                    Destroy(infoPopup_Secondary);
                    infoPopup_Secondary = null;
                }
                break;
            case InfoPopupType.Tutorial:
                if (infoPopup_Tutorial != null)
                {
                    Destroy(infoPopup_Tutorial);
                    infoPopup_Tutorial = null;
                }
                break;
        }
    }
    // Turn Popup
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
    // Versus Popup
    public void CreateVersusPopup(bool isBossBattle = false)
    {
        DestroyTurnPopup();
        turnPopup = Instantiate(versusPopupPrefab, CurrentCanvas.transform);
        turnPopup.GetComponent<VersusPopupDisplay>().IsBossBattle = isBossBattle;
        anMan.CreateParticleSystem(turnPopup, ParticleSystemHandler.ParticlesType.Explosion, 1);
    }
    public void CreateCombatEndPopup(bool playerWins)
    {
        DestroyCombatEndPopup();
        combatEndPopup = Instantiate(combatEndPopupPrefab, CurrentCanvas.transform);
        CombatEndPopupDisplay cepd = combatEndPopup.GetComponent<CombatEndPopupDisplay>();
        GameObject particleParent;
        if (playerWins) particleParent = cepd.VictoryText;
        else particleParent = cepd.DefeatText;
        anMan.CreateParticleSystem(particleParent, ParticleSystemHandler.ParticlesType.Drag);
        anMan.CreateParticleSystem(particleParent, ParticleSystemHandler.ParticlesType.NewCard);
        cepd.VictoryText.SetActive(playerWins);
        cepd.DefeatText.SetActive(!playerWins);
    }
    public void DestroyCombatEndPopup()
    {
        if (combatEndPopup != null)
        {
            Destroy(combatEndPopup);
            combatEndPopup = null;
        }
    }
    // New Card Popup
    public void CreateNewCardPopup(Card newCard, string title, Card[] chooseCards = null)
    {
        DestroyNewCardPopup();
        NewCardPopupDisplay ncpd;
        if (chooseCards == null)
        {
            newCardPopup = Instantiate(newCardPopupPrefab, CurrentCanvas.transform);
            ncpd = newCardPopup.GetComponent<NewCardPopupDisplay>();
        }
        else
        {
            chooseCardPopup = Instantiate(chooseCardPopupPrefab, CurrentCanvas.transform);
            ncpd = chooseCardPopup.GetComponent<NewCardPopupDisplay>();
        }
        
        ncpd.PopupTitle = title; // TESTING
        if (chooseCards == null) ncpd.NewCard = newCard;
        else ncpd.ChooseCards = chooseCards;
    }
    public void DestroyNewCardPopup()
    {
        if (newCardPopup != null)
        {
            Destroy(newCardPopup);
            newCardPopup = null;
        }
        if (chooseCardPopup != null)
        {
            Destroy(chooseCardPopup);
            chooseCardPopup = null;
        }
    }
    // Choose Reward Popup
    public void CreateChooseRewardPopup()
    {
        chooseRewardPopup = Instantiate(chooseRewardPopupPrefab, CurrentCanvas.transform); // TESTING
    }
    public void DestroyChooseRewardPopup()
    {
        if (chooseRewardPopup != null)
        {
            Destroy(chooseRewardPopup);
            chooseRewardPopup = null;
        }
    }
    // Aether Cell Popup
    public void CreateAetherCellPopup(int quanity)
    {
        DestroyAetherCellPopup();
        aetherCellPopup = Instantiate(aetherCellPopupPrefab, CurrentCanvas.transform);
        AetherCellPopupDisplay acpd =
            aetherCellPopup.GetComponent<AetherCellPopupDisplay>();
        acpd.AetherQuantity = quanity;
        acpd.TotalAether = pMan.AetherCells + quanity;
    }
    public void DestroyAetherCellPopup()
    {
        if (aetherCellPopup != null)
        {
            Destroy(aetherCellPopup);
            aetherCellPopup = null;
        }
    }
    // Card Page Popups
    public void CreateCardPagePopup(CardPageDisplay.CardPageType cardPageType, bool playSound = true, bool isScrollPopup = true)
    {
        float scrollValue = 1;
        if (cardPagePopup != null && isScrollPopup)
        {
            if (cardPagePopup.GetComponent<CardPageDisplay>().IsScrollPage)
            {
                ScrollRect sRect = cardPagePopup.GetComponentInChildren<ScrollRect>();
                scrollValue = sRect.verticalNormalizedPosition;
            }
        }

        DestroyCardPagePopup();
        GameObject prefab;
        if (isScrollPopup) prefab = cardScrollPopupPrefab;
        else prefab = cardPagePopupPrefab;

        cardPagePopup = Instantiate(prefab, CurrentCanvas.transform);
        cardPagePopup.GetComponent<CardPageDisplay>().DisplayCardPage(cardPageType, playSound, scrollValue);
    }
    public void DestroyCardPagePopup(bool childPopupsOnly = false)
    {
        anMan.ProgressBarRoutine_Stop();
        if (!childPopupsOnly && cardPagePopup != null)
        {
            Destroy(cardPagePopup);
            cardPagePopup = null;
        }
        DestroyRemoveCardPopup();
        DestroyRecruitUnitPopup();
        DestroyCloneUnitPopup();
    }
    // Recruit Unit Popup
    public void CreateRecruitUnitPopup(UnitCard unitCard)
    {
        DestroyRecruitUnitPopup();
        recruitUnitPopup = Instantiate(recruitUnitPopupPrefab, CurrentCanvas.transform);
        recruitUnitPopup.GetComponent<RecruitUnitPopupDisplay>().UnitCard = unitCard;
    }
    public void DestroyRecruitUnitPopup()
    {
        if (recruitUnitPopup != null)
        {
            Destroy(recruitUnitPopup);
            recruitUnitPopup = null;
        }
    }
    // Remove Card Popup
    public void CreateRemoveCardPopup(Card card)
    {
        DestroyRemoveCardPopup();
        removeCardPopup = Instantiate(removeCardPopupPrefab, CurrentCanvas.transform);
        removeCardPopup.GetComponent<RemoveCardPopupDisplay>().Card = card;
    }
    public void DestroyRemoveCardPopup()
    {
        if (removeCardPopup != null)
        {
            Destroy(removeCardPopup);
            removeCardPopup = null;
        }
    }
    // Clone Unit Popup
    public void CreateCloneUnitPopup(UnitCard unitCard)
    {
        DestroyCloneUnitPopup();
        cloneUnitPopup = Instantiate(cloneUnitPopupPrefab, CurrentCanvas.transform);
        cloneUnitPopup.GetComponent<CloneUnitPopupDisplay>().UnitCard = unitCard;
    }
    public void DestroyCloneUnitPopup()
    {
        if (cloneUnitPopup != null)
        {
            Destroy(cloneUnitPopup);
            cloneUnitPopup = null;
        }
    }
    // New Augment Popup
    public void CreateNewAugmentPopup()
    {
        DestroyNewAugmentPopup();
        newAugmentPopup = Instantiate(newAugmentPopupPrefab, CurrentCanvas.transform);
    }
    public void DestroyNewAugmentPopup()
    {
        if (newAugmentPopup != null)
        {
            Destroy(newAugmentPopup);
            newAugmentPopup = null;
        }
    }
    // Item Page Popup
    public void CreateItemPagePopup()
    {
        DestroyItemPagePopup();
        itemPagePopup = Instantiate(itemPagePopupPrefab, CurrentCanvas.transform);
    }
    public void DestroyItemPagePopup()
    {
        anMan.ProgressBarRoutine_Stop();
        if (itemPagePopup != null)
        {
            Destroy(itemPagePopup);
            itemPagePopup = null;
        }
        DestroyBuyItemPopup();
    }
    // Buy Item Popup
    public void CreateBuyItemPopup(HeroItem heroItem)
    {
        DestroyBuyItemPopup();
        buyItemPopup = Instantiate(buyItemPopupPrefab, CurrentCanvas.transform);
        buyItemPopup.GetComponent<BuyItemPopupDisplay>().HeroItem = heroItem;
    }
    public void DestroyBuyItemPopup()
    {
        if (buyItemPopup != null)
        {
            Destroy(buyItemPopup);
            buyItemPopup = null;
        }
    }
    // Location Popup
    public void CreateLocationPopup(Location location, bool isFleeting = false)
    {
        if (travelPopup != null || narrativePopup != null) return;
        locationPopup = Instantiate(locationPopupPrefab, CurrentZoomCanvas.transform);
        LocationPopupDisplay lpd = locationPopup.GetComponent<LocationPopupDisplay>();
        lpd.Location = location;
        lpd.TravelButtons.SetActive(false);

        if (isFleeting)
        {
            anMan.ChangeAnimationState(locationPopup, "Enter_Exit");
            locationPopup.transform.position = new Vector2(500, 0);
            locationPopup = null;
        }
        else lpd.ClosePopupButton.SetActive(false);
    }
    public void DestroyLocationPopup()
    {
        if (locationPopup != null)
        {
            Destroy(locationPopup);
            locationPopup = null;
        }
    }
    // Travel Popup
    public void CreateTravelPopup(Location location)
    {
        if (narrativePopup != null) return;
        DestroyTravelPopup();
        travelPopup = Instantiate(locationPopupPrefab, CurrentCanvas.transform);
        LocationPopupDisplay lpd = travelPopup.GetComponent<LocationPopupDisplay>();
        lpd.Location = location;
        //lpd.TravelButtons.SetActive(true);
        //lpd.ClosePopupButton.SetActive(true);
    }
    public void DestroyTravelPopup()
    {
        if (travelPopup != null)
        {
            Destroy(travelPopup);
            travelPopup = null;
        }
    }
    // Narrative Popup
    public void CreateNarrativePopup(Narrative narrative)
    {
        DestroyNarrativePopup();
        narrativePopup = Instantiate(narrativePopupPrefab, CurrentCanvas.transform);
        NarrativePopupDisplay npd = narrativePopup.GetComponent<NarrativePopupDisplay>();
        npd.LoadedNarrative = narrative;
    }
    public void DestroyNarrativePopup()
    {
        if (dMan.CurrentTextRoutine != null)
            dMan.StopTimedText();

        if (narrativePopup != null)
        {
            Destroy(narrativePopup);
            narrativePopup = null;
        }
    }

    /******
     * *****
     * ****** SKYBAR
     * *****
     *****/
    public void UpdateItemsCount()
    {
        itemsCount.GetComponent<TextMeshProUGUI>().SetText(pMan.HeroItems.Count.ToString());
    }
    public void AugmentsButton_OnClick(bool forceOpen)
    {
        FunctionTimer.StopTimer(AnimationManager.CLOSE_SKYBAR_TIMER);
        bool setActive;
        if (forceOpen) setActive = true;
        else setActive = !augmentsDropdown.activeSelf;

        augmentsDropdown.SetActive(setActive);
        itemsDropdown.SetActive(false);
        reputationsDropdown.SetActive(false);
    }
    public void ItemsButton_OnClick(bool forceOpen)
    {
        FunctionTimer.StopTimer(AnimationManager.CLOSE_SKYBAR_TIMER);
        bool setActive;
        if (forceOpen) setActive = true;
        else setActive = !itemsDropdown.activeSelf;

        itemsDropdown.SetActive(setActive);
        augmentsDropdown.SetActive(false);
        reputationsDropdown.SetActive(false);
    }
    public void ReputationsButton_OnClick(bool forceOpen)
    {
        FunctionTimer.StopTimer(AnimationManager.CLOSE_SKYBAR_TIMER);
        bool setActive;
        if (forceOpen) setActive = true;
        else setActive = !reputationsDropdown.activeSelf;

        reputationsDropdown.SetActive(setActive);
        augmentsDropdown.SetActive(false);
        itemsDropdown.SetActive(false);
    }

    public void HideSkybar(bool isHidden)
    {
        if (skyBar.activeInHierarchy == isHidden)
            skyBar.SetActive(!isHidden);
    }
    public void SetSkybar(bool enabled, bool hideChildren = false)
    {
        if (skyBar.activeInHierarchy != enabled)
            skyBar.SetActive(enabled);

        if (enabled)
        {
            ClearAugmentBar();
            ClearItemBar();
            SetAetherCount(pMan.AetherCells, pMan.AetherCells);
            foreach (HeroAugment ha in pMan.HeroAugments) 
                CreateAugmentIcon(ha);
            foreach (HeroItem hi in pMan.HeroItems)
                CreateItemIcon(hi);
            foreach (Transform augTran in augmentBar.transform)
                augTran.gameObject.SetActive(!hideChildren);
            foreach (Transform itemTran in itemBar.transform)
                itemTran.gameObject.SetActive(!hideChildren);
            foreach (Transform repTran in reputationBar.transform)
                repTran.gameObject.SetActive(!hideChildren);

            UpdateItemsCount();
            SetAllReputation();
        }
    }
    public void SetAetherCount(int newCount, int previousCount)
    {
        if (!skyBar.activeSelf) return;

        TextMeshProUGUI tmpro = aetherCount.GetComponentInChildren<TextMeshProUGUI>();
        if (newCount != previousCount)
        {
            anMan.SkybarIconAnimation(aetherIcon);
            anMan.CountingText(tmpro, previousCount, newCount);
        }
        else tmpro.SetText(newCount.ToString());
    }
    public void CreateAugmentIcon(HeroAugment augment, bool isNewAugment = false)
    {
        if (!skyBar.activeSelf) return;
        GameObject augmentIcon = Instantiate(augmentIconPrefab, augmentBar.transform);
        augmentIcon.GetComponent<AugmentIcon>().LoadedAugment = augment;
        if (isNewAugment) anMan.SkybarIconAnimation(augmentIcon);
    }
    public void CreateItemIcon(HeroItem item, bool isNewItem = false)
    {
        if (!skyBar.activeSelf) return;
        GameObject itemIcon = Instantiate(itemIconPrefab, itemBar.transform);
        itemIcon.GetComponent<ItemIcon>().LoadedItem = item;
        if (isNewItem) anMan.SkybarIconAnimation(itemIcon);
    }
    public void ClearAugmentBar()
    {
        if (!skyBar.activeSelf) return;
        foreach (Transform tran in augmentBar.transform) 
            Destroy(tran.gameObject);
    }
    public void ClearItemBar()
    {
        if (!skyBar.activeSelf) return;
        foreach (Transform tran in itemBar.transform)
            Destroy(tran.gameObject);
    }
    public void CreateAugmentIconPopup(HeroAugment augment, GameObject sourceIcon)
    {
        DestroyAugmentIconPopup();
        augmentIconPopup = Instantiate(augmentIconPopupPrefab, CurrentCanvas.transform);
        Vector2 sourcePos = sourceIcon.transform.localPosition;
        augmentIconPopup.transform.localPosition = new Vector2(sourcePos.x - 275, sourcePos.y + 250);
        augmentIconPopup.GetComponent<AugmentIconPopupDisplay>().HeroAugment = augment;
    }
    public void DestroyAugmentIconPopup()
    {
        if (augmentIconPopup != null)
        {
            Destroy(augmentIconPopup);
            augmentIconPopup = null;
        }
    }
    public void CreateItemIconPopup(HeroItem item, GameObject sourceIcon, bool isUseItemConfirm = false)
    {
        DestroyItemIconPopup();
        itemIconPopup = Instantiate(itemIconPopupPrefab, CurrentZoomCanvas.transform);
        Vector2 sourcePos = sourceIcon.transform.localPosition;
        itemIconPopup.transform.localPosition = new Vector2(sourcePos.x - 125, sourcePos.y + 250);
        ItemIconPopupDisplay iipd = itemIconPopup.GetComponent<ItemIconPopupDisplay>();
        iipd.LoadedItem = item;
        iipd.SourceIcon = sourceIcon;
        iipd.Buttons.SetActive(isUseItemConfirm);
        if (isUseItemConfirm) ConfirmUseItemPopup = itemIconPopup;
    }
    public void DestroyItemIconPopup()
    {
        if (itemIconPopup != null)
        {
            Destroy(itemIconPopup);
            itemIconPopup = null;

            if (ConfirmUseItemPopup != null)
                ConfirmUseItemPopup = null;
        }
    }
    public void CreateItemAbilityPopup(HeroItem item)
    {
        DestroyItemAbilityPopup();
        itemAbilityPopup = Instantiate(abilityPopupBoxPrefab, CurrentZoomCanvas.transform);
        foreach (CardAbility linkedCa in item.LinkedAbilities)
            CreateAbilityPopup(linkedCa);

        void CreateAbilityPopup(CardAbility ca)
        {
            GameObject abilityPopup =
                    Instantiate(abilityPopupPrefab, itemAbilityPopup.transform);
            abilityPopup.GetComponent<AbilityPopupDisplay>().DisplayAbilityPopup(ca, false, true);
        }
    }
    public void DestroyItemAbilityPopup()
    {
        if (itemAbilityPopup != null)
        {
            Destroy(itemAbilityPopup);
            itemAbilityPopup = null;
        }
    }

    public void SetReputation(GameManager.ReputationType type, int valueChange = 0, bool triggerOnly = false)
    {
        if (!skyBar.activeSelf) return;

        GameObject repIcon;
        int repScore;
        switch (type)
        {
            case GameManager.ReputationType.Mages:
                repIcon = reputation_Mages;
                repScore = gMan.Reputation_Mages;
                break;
            case GameManager.ReputationType.Mutants:
                repIcon = reputation_Mutants;
                repScore = gMan.Reputation_Mutants;
                break;
            case GameManager.ReputationType.Rogues:
                repIcon = reputation_Rogues;
                repScore = gMan.Reputation_Rogues;
                break;
            case GameManager.ReputationType.Techs:
                repIcon = reputation_Techs;
                repScore = gMan.Reputation_Techs;
                break;
            case GameManager.ReputationType.Warriors:
                repIcon = reputation_Warriors;
                repScore = gMan.Reputation_Warriors;
                break;
            default:
                Debug.LogError("INVALID REPUTATION TYPE!");
                return;
        }

        if (triggerOnly)
        {
            ReputationTrigger();
            return;
        }

        repIcon.GetComponentInChildren<TextMeshProUGUI>().SetText(repScore.ToString());

        Button button = repIcon.GetComponent<Button>();
        var colors = button.colors;
        if (repScore >= GameManager.REPUTATION_TIER_1) colors.normalColor = Color.green;
        else colors.normalColor = Color.white;
        button.colors = colors;

        if (valueChange != 0)
        {
            ReputationTrigger();
            anMan.ValueChanger(repIcon.transform, valueChange, -100);
        }

        void ReputationTrigger()
        {
            string sound;
            if (triggerOnly) sound = "SFX_Trigger";
            else sound = "SFX_Reputation";
            AudioManager.Instance.StartStopSound(sound);
            anMan.SkybarIconAnimation(repIcon);
        }
    }

    public void SetAllReputation()
    {
        List<GameManager.ReputationType> repTypes = new List<GameManager.ReputationType>()
        {
            GameManager.ReputationType.Mages,
            GameManager.ReputationType.Mutants,
            GameManager.ReputationType.Rogues,
            GameManager.ReputationType.Techs,
            GameManager.ReputationType.Warriors
        };

        foreach (GameManager.ReputationType type in repTypes)
            SetReputation(type);
    }

    public void CreateReputationPopup(GameManager.ReputationType repType, GameObject sourceIcon)
    {
        DestroyReputationPopup();
        reputationPopup = Instantiate(reputationPopupPrefab, UICanvas.transform);
        Vector2 sourcePos = sourceIcon.transform.localPosition;
        reputationPopup.transform.localPosition = new Vector2(sourcePos.x - 300, sourcePos.y + 100);
        reputationPopup.GetComponent<ReputationPopupDisplay>().DisplayReputationPopup
            (gMan.GetReputation(repType), gMan.GetReputationTier(repType), gMan.GetReputationBonuses(repType));
    }

    public void DestroyReputationPopup()
    {
        if (reputationPopup != null)
        {
            Destroy(reputationPopup);
            reputationPopup = null;
        }
    }

    public void CreateGameEndPopup() =>
        Instantiate(gameEndPopupPrefab, CurrentCanvas.transform);
}
