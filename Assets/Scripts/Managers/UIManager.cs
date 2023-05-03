using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

            SetSkybar(false);
            //StartCoroutine(WaitForSplash());
        }
        else Destroy(gameObject);
    }

    #region FIELDS
    [Header("COLORS"), SerializeField] private Color highlightedColor;
    [SerializeField] private Color selectedColor, rejectedColor, playableColor;

    [Header("SCENE FADER"), SerializeField] private GameObject sceneFader;

    [Header("SKYBAR"), SerializeField] private GameObject skyBar;
    [SerializeField]
    private GameObject
        // Augments
        augmentBar, augmentsDropdown, augmentsCount,
        // Items
        itemBar,
        // Reputation
        reputationBar, reputationsDropdown,
        // Aether
        aetherCount, aetherIcon,
        // Health
        currentHealth, healthValue;

    [Header("REPUTATION"), SerializeField] private GameObject reputation_Mages;
    [SerializeField] private GameObject reputation_Mutants,
        reputation_Rogues, reputation_Techs, reputation_Warriors;

    [Header("PREFABS"), SerializeField] private GameObject screenDimmerPrefab;
    [SerializeField] private GameObject tooltipPopupPrefab, infoPopupPrefab, infoPopup_SecondaryPrefab,
        combatEndPopupPrefab, turnPopupPrefab, versusPopupPrefab, menuPopupPrefab,
        explicitLanguagePopupPrefab, tutorialPopupPrefab, tutorialActionPopupPrefab, newHeroPopupPrefab,
        newCardPopupPrefab, chooseCardPopupPrefab, chooseRewardPopupPrefab,
        cardPagePrefab, cardScrollPagePrefab, cardPagePopupPrefab, newAugmentPopupPrefab, locationPopupPrefab,
        narrativePopupPrefab, augmentIconPrefab, augmentIconPopupPrefab, itemPagePopupPrefab,
        buyItemPopupPrefab, removeItemPopupPrefab, itemIconPrefab, itemIconPopupPrefab, abilityPopupPrefab,
        abilityPopupBoxPrefab, reputationPopupPrefab, gameEndPopupPrefab;

    private GameObject screenDimmer, tooltipPopup, infoPopup, infoPopup_Secondary,
        infoPopup_Tutorial, combatEndPopup, turnPopup, menuPopup, explicitLanguagePopup, tutorialPopup,
        tutorialActionPopup, newHeroPopup, newCardPopup, chooseCardPopup, chooseRewardPopup,
        cardPage, cardPagePopup, newAugmentPopup, itemPagePopup, buyItemPopup, removeItemPopup, locationPopup,
        travelPopup, narrativePopup, augmentIconPopup, itemIconPopup, itemAbilityPopup, reputationPopup,
        endTurnButton, cancelEffectButton, confirmEffectButton, playerZoneOutline;

    private Coroutine sceneFadeRoutine;
    private CombatLog combatLog;

    public const string TOOLTIP_TIMER = "TooltipTimer";

    public const string LOCK_TEXT = "<b>???</b>";
    public const string LOCK_TEXT_SHORT = "<b>?</b>";
    #endregion

    #region PROPERTIES
    public GameObject NewCardPopup { get => newCardPopup; }
    public GameObject ConfirmUseItemPopup { get; set; }
    public GameObject EndTurnButton { get => endTurnButton; }
    public GameObject AugmentBar { get => augmentBar; }
    public GameObject AugmentsDropdown { get => augmentsDropdown; }
    public GameObject ItemBar { get => itemBar; }
    public GameObject ReputationBar { get => reputationBar; }
    public GameObject ReputationsDropdown { get => reputationsDropdown; }
    public GameObject CombatLog { get => combatLog.gameObject; }
    public Color HighlightedColor { get => highlightedColor; }

    public Coroutine SceneFadeRoutine { get => sceneFadeRoutine; }

    public GameObject CurrentWorldSpace { get; private set; }
    public GameObject CurrentCanvas { get; private set; }
    public GameObject CurrentZoomCanvas { get; private set; }
    public GameObject UICanvas { get; set; }

    public bool PlayerIsTargetting { get; set; }
    public bool PlayerCanTravel => !(newCardPopup != null || chooseCardPopup != null ||
        narrativePopup != null || tutorialActionPopup != null);
    #endregion

    #region METHODS
    public void Start()
    {
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
     * ****** CANCEL/CONFIRM_EFFECT_BUTTON
     * *****
     *****/
    public void SetCancelEffectButton(bool isEnabled) => cancelEffectButton.SetActive(isEnabled);
    public void SetConfirmEffectButton(bool isEnabled) => confirmEffectButton.SetActive(isEnabled);

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
            var sr = playerZoneOutline.GetComponentInChildren<SpriteRenderer>();
            var col = selected ? highlightedColor : selectedColor;
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
        var img = sceneFader.GetComponent<Image>();
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

            sceneFadeRoutine = null; // TESTING
        }
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
        if (screenIsDimmed) screenDimmer = Instantiate(screenDimmerPrefab, CurrentZoomCanvas.transform);
    }
    /******
     * *****
     * ****** END_TURN_BUTTON
     * *****
     *****/
    public void UpdateEndTurnButton(bool isInteractable = true)
    {
        if (endTurnButton == null) return;

        bool isMyTurn = Managers.P_MAN.IsMyTurn;
        var etbd = endTurnButton.GetComponent<EndTurnButtonDisplay>();
        etbd.PlayerTurnSide.SetActive(isMyTurn);
        etbd.EnemyTurnSide.SetActive(!isMyTurn);

        if (!isMyTurn) isInteractable = false;
        var buttons = endTurnButton.GetComponentsInChildren<Button>();
        foreach (var b in buttons) b.interactable = isInteractable;
    }
    public void SetReadyEndTurnButton(bool isReady)
    {
        if (endTurnButton == null) return;

        var etbd = endTurnButton.GetComponent<EndTurnButtonDisplay>();
        var etb = etbd.PlayerTurnSide.GetComponent<Button>();

        var normalColor = isReady ? Color.green : Color.grey;
        normalColor.a = 0.7f;
        var highlightedColor = isReady ? Color.green : Color.black;
        var disabledColor = Color.gray;
        disabledColor.a = 0.3f;

        var btnClr = etb.colors;
        btnClr.normalColor = normalColor;
        btnClr.highlightedColor = highlightedColor;
        btnClr.disabledColor = disabledColor;
        etb.colors = btnClr;
    }

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
                RemoveBuffer();
                return;
            }
            else cs.CardOutline.SetActive(true);

            var image = cs.CardOutline.GetComponent<Image>();
            if (enabled) SetColor(image);

            if (selectionType is SelectionType.Selected && PlayerIsTargetting)
            {
                if (Managers.EF_MAN.CurrentEffect is DrawEffect de && de.IsDiscardEffect)
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
            if (selectionType is SelectionType.Disabled)
            {
                hs.HeroOutline.SetActive(false);
                return;
            }
            else hs.HeroOutline.SetActive(true);

            var image = hs.HeroOutline.GetComponentInChildren<Image>();
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
        // Skybar
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene)) HideSkybar(false);

        // Screen Dimmer
        SetScreenDimmer(false);

        // CardZoom
        CardZoom.ZoomCardIsCentered = false;
        CardZoom.BaseZoomCard = null;
        CardZoom.ActiveZoomCard = 0;

        // Function Timers
        FunctionTimer.StopTimer(CardZoom.ZOOM_CARD_TIMER);
        FunctionTimer.StopTimer(CardZoom.ABILITY_POPUP_TIMER);

        List<GameObject> objectsToDestroy = new()
        {
            CardZoom.CurrentZoomCard,
            CardZoom.DescriptionPopup,
            CardZoom.AbilityPopupBox,
            AbilityZoom.AbilityPopup
        };

        foreach (var go in objectsToDestroy) Destroy(go);
    }

    /******
     * *****
     * ****** GET_PORTRAIT_POSITION
     * *****
     *****/
    public void GetPortraitPosition(string heroName, out Vector2 position, out Vector2 scale)
    {
        position = new Vector2();
        scale = new Vector2();

        bool isCombat = SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene);

        switch (heroName)
        {
            // FAYDRA
            case "Faydra, Rogue Cyborg":
                position.Set(-90, -20);
                scale.Set(1.2f, 1.2f);
                break;
            // GENZER
            case "Genzer, Evolved Mutant":
                if (isCombat)
                {
                    position.Set(5, -20);
                    scale.Set(1f, 1f);
                }
                else
                {
                    position.Set(5, 15);
                    scale.Set(1f, 1f);
                }
                break;
            // KILI
            case "Kili, Neon Rider":
                if (isCombat)
                {
                    position.Set(-30, -90);
                    scale.Set(1.2f, 1.2f);
                }
                else
                {
                    position.Set(-30, -70);
                    scale.Set(1.2f, 1.2f);
                }
                break;
            // MARDRIK
            case "Madrik, Obsessed Mage":
                if (isCombat)
                {
                    position.Set(0, -35);
                    scale.Set(1.2f, 1.2f);
                }
                else
                {
                    position.Set(0, -15);
                    scale.Set(1.2f, 1.2f);
                }
                break;
            // YERGOV
            case "Yergov, Biochemist":
                if (isCombat)
                {
                    position.Set(-15, -45);
                    scale.Set(0.9f, 0.9f);
                }
                else
                {
                    position.Set(-75, -75);
                    scale.Set(1.2f, 1.2f);
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
    public void CombatLogEntry(string entry) => combatLog.NewLogEntry(entry);
    public void CombatLog_PlayCard(GameObject card) => combatLog.NewLogEntry_PlayCard(card);

    /******
     * *****
     * ****** POPUPS
     * *****
     *****/
    // Destroy Interactable Popup
    public void DestroyInteractablePopup(GameObject popup) => Managers.AN_MAN.ChangeAnimationState(popup, "Exit");
    // Tooltip Popup
    public void CreateTooltipPopup(Vector2 position, string text)
    {
        DestroyTooltipPopup();
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
        explicitLanguagePopup = Instantiate(explicitLanguagePopupPrefab, CurrentCanvas.transform);
    }
    // Tutorial Popup
    public void CreateTutorialPopup()
    {
        if (tutorialPopup != null) return;
        tutorialPopup = Instantiate(tutorialPopupPrefab, CurrentCanvas.transform);
    }
    // Tutorial Action Popup
    public void CreateTutorialActionPopup
        (TutorialActionPopupDisplay.Type tutorialType = TutorialActionPopupDisplay.Type.Tutorial)
    {
        if (tutorialActionPopup != null) return;
        tutorialActionPopup = Instantiate(tutorialActionPopupPrefab, CurrentZoomCanvas.transform);
        tutorialActionPopup.GetComponent<TutorialActionPopupDisplay>().TutorialType = tutorialType;
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
        Tutorial,
    }
    public void CreateInfoPopup(string message, InfoPopupType infoPopupType, bool isCentered = false, bool showContinue = false)
    {
        DestroyInfoPopup(infoPopupType);
        Vector2 vec2 = new();
        if (!isCentered) vec2.Set(750, 0);

        InfoPopupDisplay ipd;
        switch (infoPopupType)
        {
            case InfoPopupType.Default:
                infoPopup = Instantiate(infoPopupPrefab, CurrentZoomCanvas.transform); // TESTING on ZOOM
                ipd = infoPopup.GetComponent<InfoPopupDisplay>();
                ipd.DisplayInfoPopup(message, showContinue);
                infoPopup.transform.localPosition = vec2;
                break;
            case InfoPopupType.Secondary:
                infoPopup_Secondary = Instantiate(infoPopup_SecondaryPrefab, UICanvas.transform);
                ipd = infoPopup_Secondary.GetComponent<InfoPopupDisplay>();
                ipd.DisplayInfoPopup(message, showContinue);
                infoPopup_Secondary.transform.localPosition = vec2;
                break;
            case InfoPopupType.Tutorial:
                infoPopup_Tutorial = Instantiate(infoPopupPrefab, CurrentZoomCanvas.transform);
                ipd = infoPopup_Tutorial.GetComponent<InfoPopupDisplay>();
                ipd.DisplayInfoPopup(message, showContinue);
                Destroy(infoPopup_Tutorial.GetComponent<Animator>());
                infoPopup_Tutorial.transform.localPosition = vec2;
                break;
        }
    }
    public void CreateFleetingInfoPopup(string message)
    {
        CreateInfoPopup(message, InfoPopupType.Secondary, true);
        Managers.AN_MAN.ChangeAnimationState(infoPopup_Secondary, "Enter_Exit");
    }
    public void InsufficientAetherPopup()
    {
        CreateFleetingInfoPopup($"Not enough aether! (You have {Managers.P_MAN.CurrentAether} aether)");
        Managers.AU_MAN.StartStopSound("SFX_Error");
    }
    public void DismissInfoPopup()
    {
        if (infoPopup != null) Managers.AN_MAN.ChangeAnimationState(infoPopup, "Exit");
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
        turnPopup = Instantiate(turnPopupPrefab, CurrentZoomCanvas.transform);
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
        turnPopup = Instantiate(versusPopupPrefab, CurrentZoomCanvas.transform);
        turnPopup.GetComponent<VersusPopupDisplay>().IsBossBattle = isBossBattle;
        Managers.AN_MAN.CreateParticleSystem(turnPopup, ParticleSystemHandler.ParticlesType.Explosion, 1);
    }
    public void CreateCombatEndPopup(bool playerWins)
    {
        DestroyCombatEndPopup();
        combatEndPopup = Instantiate(combatEndPopupPrefab, CurrentZoomCanvas.transform);
        var cepd = combatEndPopup.GetComponent<CombatEndPopupDisplay>();
        GameObject particleParent;
        if (playerWins) particleParent = cepd.VictoryText;
        else particleParent = cepd.DefeatText;
        Managers.AN_MAN.CreateParticleSystem(particleParent, ParticleSystemHandler.ParticlesType.Drag);
        Managers.AN_MAN.CreateParticleSystem(particleParent, ParticleSystemHandler.ParticlesType.NewCard);
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
    // New Hero Popup
    public void CreateNewHeroPopup(string title, PlayerHero newHero, HeroPower newPower, HeroPower newUltimate)
    {
        DestroyNewHeroPopup();
        newHeroPopup = Instantiate(newHeroPopupPrefab, CurrentZoomCanvas.transform);
        var nhpd = newHeroPopup.GetComponent<NewHeroPopupDisplay>();
        nhpd.DisplayNewHeroPopup(title, newHero, newPower, newUltimate);
    }
    public void DestroyNewHeroPopup()
    {
        if (newHeroPopup != null)
        {
            Destroy(newHeroPopup);
            newHeroPopup = null;
        }
    }
    // New Card Popup
    public void CreateNewCardPopup(Card newCard, string title, Card[] chooseCards = null)
    {
        DestroyNewCardPopup();
        NewCardPopupDisplay ncpd;
        if (chooseCards == null)
        {
            newCardPopup = Instantiate(newCardPopupPrefab, CurrentZoomCanvas.transform);
            ncpd = newCardPopup.GetComponent<NewCardPopupDisplay>();
        }
        else
        {
            chooseCardPopup = Instantiate(chooseCardPopupPrefab, CurrentZoomCanvas.transform);
            ncpd = chooseCardPopup.GetComponent<NewCardPopupDisplay>();
        }

        ncpd.PopupTitle = title;
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
        DestroyZoomObjects(); // For combat end
        chooseRewardPopup = Instantiate(chooseRewardPopupPrefab, CurrentZoomCanvas.transform);
        var nextClip = Managers.D_MAN.EngagedHero != null ? Managers.D_MAN.EngagedHero.NextDialogueClip : null;
        
        // Homebase Scene
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene))
        {
            // blank
        }
        /* Dialogue does not have ChooseRewardPopup functionality
        // Dialogue Scene
        else if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
        {
            var dp = nextClip as DialoguePrompt;
            if (dp.AetherCells > 0) Managers.P_MAN.AetherCells += dp.AetherCells;
        }
        */
        // Combat Scene
        else if (SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene))
        {
            if (nextClip is CombatRewardClip)
            {
                int aetherReward = Managers.G_MAN.GetAetherReward((Managers.D_MAN.EngagedHero as EnemyHero).EnemyLevel);
                Managers.P_MAN.CurrentAether += aetherReward;
            }
            else Debug.LogError("NEXT CLIP IS NOT COMBAT REWARD CLIP!");
        }
    }
    // Card Page
    public void CreateCardPage(CardPageDisplay.CardPageType cardPageType,
        bool isReload = false, bool isScrollPopup = true)
    {
        float scrollValue = 1;
        if (cardPage != null && isScrollPopup)
        {
            if (cardPage.GetComponent<CardPageDisplay>().IsScrollPage)
            {
                var sRect = cardPage.GetComponentInChildren<ScrollRect>();
                scrollValue = sRect.verticalNormalizedPosition;
            }
        }

        DestroyCardPage();
        var prefab = isScrollPopup ? cardScrollPagePrefab : cardPagePrefab;
        cardPage = Instantiate(prefab, CurrentCanvas.transform);
        if (isReload) cardPage.GetComponent<Animator>().SetBool("Static", true); // TESTING
        cardPage.GetComponent<CardPageDisplay>().DisplayCardPage(cardPageType, isReload, scrollValue);
    }
    public void DestroyCardPage(bool childrenOnly = false)
    {
        Managers.AN_MAN.ProgressBarRoutine_Stop();
        if (!childrenOnly && cardPage != null)
        {
            Destroy(cardPage);
            cardPage = null;
        }
        DestroyCardPagePopup();
        DestroyTooltipPopup();
    }
    // Card Page Popup
    public void CreateCardPagePopup(Card card, int cardCost, CardPageDisplay.CardPageType cardPageType)
    {
        DestroyCardPagePopup();
        cardPagePopup = Instantiate(cardPagePopupPrefab, CurrentCanvas.transform);
        cardPagePopup.GetComponent<CardPagePopupDisplay>().SetCard(card, cardCost, cardPageType);
    }
    public void DestroyCardPagePopup()
    {
        if (cardPagePopup != null)
        {
            Destroy(cardPagePopup);
            cardPagePopup = null;
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
    public void CreateItemPagePopup(bool isItemRemoval, bool playSound = true)
    {
        DestroyItemPagePopup();
        itemPagePopup = Instantiate(itemPagePopupPrefab, CurrentCanvas.transform);
        itemPagePopup.GetComponent<ItemPageDisplay>().DisplayItems(isItemRemoval, playSound);
    }
    public void DestroyItemPagePopup(bool childrenOnly = false)
    {
        Managers.AN_MAN.ProgressBarRoutine_Stop();
        if (!childrenOnly && itemPagePopup != null)
        {
            Destroy(itemPagePopup);
            itemPagePopup = null;
        }
        DestroyBuyItemPopup();
        DestroyRemoveItemPopup();
        DestroyTooltipPopup();
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
    // Remove Item Popup
    public void CreateRemoveItemPopup(HeroItem heroItem)
    {
        DestroyRemoveItemPopup();
        removeItemPopup = Instantiate(removeItemPopupPrefab, CurrentCanvas.transform);
        removeItemPopup.GetComponent<RemoveItemPopupDisplay>().HeroItem = heroItem;
    }
    public void DestroyRemoveItemPopup()
    {
        if (removeItemPopup != null)
        {
            Destroy(removeItemPopup);
            removeItemPopup = null;
        }
    }
    // Location Popup
    public void CreateLocationPopup(Location location, bool isFleeting = false)
    {
        if (travelPopup != null || narrativePopup != null) return;
        DestroyLocationPopup();
        locationPopup = Instantiate(locationPopupPrefab, CurrentZoomCanvas.transform);
        var lpd = locationPopup.GetComponent<LocationPopupDisplay>();
        lpd.TravelButtons.SetActive(false);
        lpd.DifficultyLevel.SetActive(false);
        lpd.Location = location;

        if (isFleeting)
        {
            Managers.AN_MAN.ChangeAnimationState(locationPopup, "Enter_Exit");
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
        if (narrativePopup != null || chooseRewardPopup != null) return;
        DestroyTravelPopup();
        DestroyLocationPopup();
        travelPopup = Instantiate(locationPopupPrefab, CurrentCanvas.transform);
        var lpd = travelPopup.GetComponent<LocationPopupDisplay>();
        // Augmenter is only non-recurring location w/o combat
        if (location.IsRecurring || location.IsAugmenter) lpd.DifficultyLevel.SetActive(false);
        lpd.Location = location;
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
        var npd = narrativePopup.GetComponent<NarrativePopupDisplay>();
        npd.LoadedNarrative = narrative;
    }
    public void DestroyNarrativePopup()
    {
        if (Managers.D_MAN.CurrentTextRoutine != null) Managers.D_MAN.StopTimedText();

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
    public void AugmentsButton_OnClick(bool forceOpen)
    {
        FunctionTimer.StopTimer(AnimationManager.CLOSE_SKYBAR_TIMER);
        bool setActive = forceOpen || !augmentsDropdown.activeSelf;

        augmentsDropdown.SetActive(setActive);
        reputationsDropdown.SetActive(false);
    }
    public void ReputationsButton_OnClick(bool forceOpen)
    {
        FunctionTimer.StopTimer(AnimationManager.CLOSE_SKYBAR_TIMER);
        bool setActive = forceOpen || !reputationsDropdown.activeSelf;

        reputationsDropdown.SetActive(setActive);
        augmentsDropdown.SetActive(false);
    }

    public void HideSkybar(bool isHidden)
    {
        if (skyBar.activeInHierarchy == isHidden) skyBar.SetActive(!isHidden);
    }
    public void SetSkybar(bool enabled, bool hideChildren = false)
    {
        if (skyBar.activeInHierarchy != enabled) skyBar.SetActive(enabled);

        if (enabled)
        {
            ClearAugmentBar();
            ClearItemBar();
            var augTmpro = augmentsCount.GetComponent<TextMeshProUGUI>();

            if (Managers.G_MAN.IsTutorial)
            {
                augTmpro.SetText(0 + "");
                SetAllReputation(true);
            }
            else
            {
                // Augments
                foreach (var ha in Managers.P_MAN.HeroAugments)
                    CreateAugmentIcon(ha);
                foreach (Transform augTran in augmentBar.transform)
                    augTran.gameObject.SetActive(!hideChildren);
                augTmpro.SetText(Managers.P_MAN.HeroAugments.Count.ToString());
                // Items
                foreach (var hi in Managers.P_MAN.HeroItems)
                    CreateItemIcon(hi);
                foreach (Transform itemTran in itemBar.transform)
                    itemTran.gameObject.SetActive(!hideChildren);
                // Reputation
                foreach (Transform repTran in reputationBar.transform)
                    repTran.gameObject.SetActive(!hideChildren);
                SetAllReputation();
            }

            SetAetherCount(0);
            currentHealth.SetActive(!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene));
            SetCurrentHealth(0);
        }

        augmentsDropdown.SetActive(false);
        reputationsDropdown.SetActive(false);
    }
    public void SetAetherCount(int valueChange)
    {
        if (!skyBar.activeInHierarchy) return;

        var tmpro = aetherCount.GetComponentInChildren<TextMeshProUGUI>();
        tmpro.SetText(Managers.P_MAN.CurrentAether - valueChange + "");
        tmpro.color = Color.white; // In case the coroutine is stopped while the text is red

        if (valueChange != 0)
        {
            new AnimationManager.CountingTextObject(tmpro, valueChange, Color.red);

            Managers.AN_MAN.SkybarIconAnimation(aetherIcon);
            Managers.AN_MAN.CountingText();
            Managers.AN_MAN.ValueChanger(aetherIcon.transform, valueChange, true, -275, 75);
        }
    }
    public void SetCurrentHealth(int valueChange)
    {
        if (!skyBar.activeInHierarchy || !currentHealth.activeInHierarchy) return;

        var tmpro = healthValue.GetComponent<TextMeshProUGUI>();
        var slider = currentHealth.GetComponentInChildren<Slider>();

        tmpro.SetText(Managers.P_MAN.CurrentHealth + "");
        slider.maxValue = Managers.P_MAN.MaxHealth;
        slider.value = Managers.P_MAN.CurrentHealth;

        if (valueChange != 0)
        {
            Managers.AU_MAN.StartStopSound("SFX_StatPlus");
            Managers.AN_MAN.SkybarIconAnimation(healthValue);
            Managers.AN_MAN.ValueChanger(healthValue.transform, valueChange, true, -250, 75);
        }
    }
    public void CreateAugmentIcon(HeroAugment augment, bool isNewAugment = false)
    {
        if (!skyBar.activeInHierarchy) return;
        var augmentIcon = Instantiate(augmentIconPrefab, augmentBar.transform);
        augmentIcon.GetComponent<AugmentIcon>().LoadedAugment = augment;
        if (isNewAugment) Managers.AN_MAN.SkybarIconAnimation(augmentIcon);
    }
    public void CreateItemIcon(HeroItem item, bool isNewItem = false)
    {
        if (!skyBar.activeInHierarchy) return;
        var itemIcon = Instantiate(itemIconPrefab, itemBar.transform);
        itemIcon.GetComponent<ItemIcon>().LoadedItem = item;
        if (isNewItem) Managers.AN_MAN.SkybarIconAnimation(itemIcon);
    }
    public void ClearAugmentBar()
    {
        if (!skyBar.activeInHierarchy) return;
        foreach (Transform tran in augmentBar.transform)
            Destroy(tran.gameObject);
    }
    public void ClearItemBar()
    {
        if (!skyBar.activeInHierarchy) return;
        foreach (Transform tran in itemBar.transform)
            Destroy(tran.gameObject);
    }
    public void CreateAugmentIconPopup(HeroAugment augment, GameObject sourceIcon)
    {
        DestroyAugmentIconPopup();
        augmentIconPopup = Instantiate(augmentIconPopupPrefab, UICanvas.transform); // UICanvas!
        Vector2 sourcePos = sourceIcon.transform.position;
        augmentIconPopup.transform.position = new Vector2(sourcePos.x - 250, sourcePos.y - 50);
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
        itemIconPopup = Instantiate(itemIconPopupPrefab, UICanvas.transform); // UICanvas!
        Vector2 sourcePos = sourceIcon.transform.localPosition;
        itemIconPopup.transform.localPosition = new Vector2(sourcePos.x + 250, 375);// sourcePos.y - 140);
        var iipd = itemIconPopup.GetComponent<ItemIconPopupDisplay>();
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
        foreach (var linkedCa in item.LinkedAbilities)
            CreateAbilityPopup(linkedCa);

        void CreateAbilityPopup(CardAbility ca)
        {
            var abilityPopup = Instantiate(abilityPopupPrefab, itemAbilityPopup.transform);
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

    public void SetReputation(GameManager.ReputationType type, int valueChange = 0, bool triggerOnly = false, bool setToZero = false)
    {
        if (!skyBar.activeSelf) return;

        GameObject repIcon;
        int repScore;

        switch (type)
        {
            case GameManager.ReputationType.Mages:
                repIcon = reputation_Mages;
                repScore = setToZero ? 0 : Managers.G_MAN.Reputation_Mages;
                break;
            case GameManager.ReputationType.Mutants:
                repIcon = reputation_Mutants;
                repScore = setToZero ? 0 : Managers.G_MAN.Reputation_Mutants;
                break;
            case GameManager.ReputationType.Rogues:
                repIcon = reputation_Rogues;
                repScore = setToZero ? 0 : Managers.G_MAN.Reputation_Rogues;
                break;
            case GameManager.ReputationType.Techs:
                repIcon = reputation_Techs;
                repScore = setToZero ? 0 : Managers.G_MAN.Reputation_Techs;
                break;
            case GameManager.ReputationType.Warriors:
                repIcon = reputation_Warriors;
                repScore = setToZero ? 0 : Managers.G_MAN.Reputation_Warriors;
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

        var button = repIcon.GetComponent<Button>();
        var colors = button.colors;
        if (repScore >= GameManager.REPUTATION_TIER_1) colors.normalColor = Color.green;
        else colors.normalColor = Color.white;
        button.colors = colors;

        if (valueChange != 0)
        {
            ReputationTrigger();
            Managers.AN_MAN.ValueChanger(repIcon.transform, valueChange, true, -100);
        }

        void ReputationTrigger()
        {
            string sound;
            if (triggerOnly) sound = "SFX_Trigger";
            else sound = "SFX_Reputation";
            Managers.AU_MAN.StartStopSound(sound);
            Managers.AN_MAN.SkybarIconAnimation(repIcon);
        }
    }

    public void SetAllReputation(bool setToZero = false)
    {
        foreach (GameManager.ReputationType type in
            Enum.GetValues(typeof(GameManager.ReputationType)))
            SetReputation(type, 0, false, setToZero);
    }

    public void CreateReputationPopup(GameManager.ReputationType repType, GameObject sourceIcon)
    {
        DestroyReputationPopup();
        reputationPopup = Instantiate(reputationPopupPrefab, UICanvas.transform); // UICanvas!
        Vector2 sourcePos = sourceIcon.transform.position;
        reputationPopup.transform.position = new Vector2(sourcePos.x - 300, 750);
        reputationPopup.GetComponent<ReputationPopupDisplay>().DisplayReputationPopup
            (Managers.G_MAN.GetReputation(repType), Managers.G_MAN.GetReputationTier(repType),
            Managers.G_MAN.GetReputationBonuses(repType));
    }

    public void DestroyReputationPopup()
    {
        if (reputationPopup != null)
        {
            Destroy(reputationPopup);
            reputationPopup = null;
        }
    }

    public void CreateGameEndPopup() => Instantiate(gameEndPopupPrefab, CurrentCanvas.transform);
    #endregion
}
