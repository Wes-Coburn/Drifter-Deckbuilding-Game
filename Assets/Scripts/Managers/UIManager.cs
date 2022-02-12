using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            StartCoroutine(WaitForSplash());
        }
        else Destroy(gameObject);
    }

    [Header("SKYBAR")]
    [SerializeField] private GameObject skyBar;
    [SerializeField] private GameObject aetherCount;
    [SerializeField] private GameObject augmentBar;
    [SerializeField] private GameObject itemBar;

    [Header("SCENE FADER")]
    [SerializeField] private GameObject sceneFader;

    [Header("PREFABS")]
    [SerializeField] private GameObject screenDimmerPrefab;
    [SerializeField] private GameObject infoPopupPrefab;
    [SerializeField] private GameObject infoPopup_SecondaryPrefab;
    [SerializeField] private GameObject combatEndPopupPrefab;
    [SerializeField] private GameObject turnPopupPrefab;
    [SerializeField] private GameObject versusPopupPrefab;
    [SerializeField] private GameObject menuPopupPrefab;
    [SerializeField] private GameObject explicitLanguagePopupPrefab;
    [SerializeField] private GameObject newCardPopupPrefab;
    [SerializeField] private GameObject chooseCardPopupPrefab;
    [SerializeField] private GameObject aetherCellPopupPrefab;
    [SerializeField] private GameObject cardPagePopupPrefab;
    [SerializeField] private GameObject cardScrollPopupPrefab;
    [SerializeField] private GameObject learnSkillPopupPrefab;
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

    [Header("BETA PREFABS")]
    [SerializeField] private GameObject betaFinishPopupPrefab;

    [Header("COLORS")]
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color rejectedColor;

    private PlayerManager pMan;
    private DialogueManager dMan;
    private AnimationManager anMan;

    private GameObject screenDimmer;
    private GameObject infoPopup;
    private GameObject infoPopup_Secondary;
    private GameObject combatEndPopup;
    private GameObject turnPopup;
    private GameObject menuPopup;
    private GameObject explicitLanguagePopup;
    private GameObject newCardPopup;
    private GameObject chooseCardPopup;
    private GameObject aetherCellPopup;
    private GameObject cardPagePopup;
    private GameObject learnSkillPopup;
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

    private GameObject endTurnButton;
    private GameObject cancelEffectButton;
    private GameObject confirmEffectButton;
    private Coroutine sceneFadeRoutine;
    private GameObject playerZoneOutline;
    
    public GameObject NewCardPopup { get => newCardPopup; }
    public GameObject ChooseCardPopup { get => chooseCardPopup; }
    public GameObject CardPagePopup { get => cardPagePopup; }
    public GameObject ConfirmUseItemPopup { get; set; }
    public GameObject EndTurnButton { get => endTurnButton; }
    public GameObject AugmentBar { get => augmentBar; }
    public GameObject ItemBar { get => itemBar; }
    public Color HighlightedColor { get => highlightedColor; }

    // PLAYER_IS_TARGETTING
    public bool PlayerIsTargetting { get; set; }
    // WORLDSPACE
    public GameObject CurrentWorldSpace { get; private set; }
    // CANVAS
    public GameObject CurrentCanvas { get; private set; }
    // ZOOM CANVAS
    public GameObject CurrentZoomCanvas { get; private set; }
    // UI CANVAS
    private GameObject UICanvas { get; set; }

    public void Start()
    {
        pMan = PlayerManager.Instance;
        dMan = DialogueManager.Instance;
        anMan = AnimationManager.Instance;
        PlayerIsTargetting = false;
        CurrentWorldSpace = GameObject.Find("WorldSpace");
        CurrentCanvas = GameObject.Find("Canvas");
        UICanvas = GameObject.Find("UI_Canvas");
        CurrentZoomCanvas = GameObject.Find("Canvas_Zoom");
    }

    private IEnumerator WaitForSplash()
    {
        while (!UnityEngine.Rendering.SplashScreen.isFinished) 
            yield return null;
        yield return new WaitForSeconds(1);
        SetSceneFader(false);
    }

    /******
     * *****
     * ****** START_SCENE
     * *****
     *****/
    public void StartCombatScene()
    {
        cancelEffectButton = GameObject.Find("CancelEffectButton");
        confirmEffectButton = GameObject.Find("ConfirmEffectButton");
        endTurnButton = GameObject.Find("EndTurnButton");
        playerZoneOutline = GameObject.Find("PlayerZoneOutline");
        SetCancelEffectButton(false);
        SetConfirmEffectButton(false);
        SetPlayerZoneOutline(false, false);
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
        if (screenIsDimmed)
            screenDimmer = Instantiate(screenDimmerPrefab, CurrentZoomCanvas.transform);
    }
    /******
     * *****
     * ****** END_TURN_BUTTON
     * *****
     *****/
    public void UpdateEndTurnButton(bool isMyTurn, bool isInteractable = true)
    {
        EndTurnButtonDisplay etbd =
            endTurnButton.GetComponent<EndTurnButtonDisplay>();
        etbd.EndTurnSide.SetActive(isMyTurn);
        etbd.OpponentTurnSide.SetActive(!isMyTurn);

        if (isMyTurn)
        {
            Button[] buttons = endTurnButton.GetComponentsInChildren<Button>();
            foreach (Button b in buttons) b.interactable = isInteractable;
        }
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
    public void SelectTarget(GameObject target, 
        bool enabled, bool isSelected = false, bool isRejected = false)
    {
        if (target == null)
        {
            Debug.LogError("TARGET IS NULL!");
            return;
        }

        if (target.TryGetComponent(out CardSelect cs))
        {
            cs.CardOutline.SetActive(enabled);
            Image image = cs.CardOutline.GetComponent<Image>();
            if (enabled) SetColor(image);

            // TESTING
            if (isSelected && PlayerIsTargetting)
            {
                if (EffectManager.Instance.CurrentEffect is DrawEffect de && de.IsDiscardEffect)
                {
                    target.GetComponent<CardDisplay>().CardContainer.GetComponent
                        <CardContainer>().BufferDistance = new Vector2(0, -50);
                }
            }
            // TESTING
            if (!isSelected)
            {
                target.GetComponent<CardDisplay>().CardContainer.GetComponent
                    <CardContainer>().BufferDistance = new Vector2(0, 0);
            }
        }
        else if (target.TryGetComponent(out HeroSelect hs))
        {
            hs.HeroOutline.SetActive(enabled);
            Image image = hs.HeroOutline.GetComponentInChildren<Image>();
            if (enabled) SetColor(image);
        }
        else
        {
            Debug.LogError("SELECT TARGET SCRIPT NOT FOUND!");
            return;
        }

        void SetColor(Image image)
        {
            if (isSelected) image.color = selectedColor;
            else if (isRejected) image.color = rejectedColor;
            else image.color = highlightedColor;
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
                    case SceneLoader.Scene.HeroSelectScene:
                        position.Set(-15, -35);
                        scale.Set(1.3f, 1.3f);
                        break;
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
                    case SceneLoader.Scene.CombatScene:
                        position.Set(-120, -120);
                        scale.Set(3, 3);
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
     * ****** POPUPS
     * *****
     *****/
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
    public void CreateInfoPopup(string message,
        bool isCentered = false, bool isSecondary = false)
    {
        DestroyInfoPopup(isSecondary);
        Vector2 vec2 = new Vector2(0, 0);
        if (!isCentered) vec2.Set(750, 0);
        if (!isSecondary)
        {
            infoPopup = Instantiate(infoPopupPrefab, CurrentCanvas.transform);
            infoPopup.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
            infoPopup.transform.localPosition = vec2;
        }
        else
        {
            infoPopup_Secondary = Instantiate(infoPopup_SecondaryPrefab, UICanvas.transform);
            infoPopup_Secondary.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
            infoPopup_Secondary.transform.localPosition = vec2;
        }
    }
    public void CreateFleetingInfoPopup(string message, bool isCentered = false)
    {
        CreateInfoPopup(message, isCentered, true);
        AnimationManager.Instance.ChangeAnimationState(infoPopup_Secondary, "Enter_Exit");
    }
    public void InsufficientAetherPopup()
    {
        CreateFleetingInfoPopup("Not enough aether! (You have " + 
            pMan.AetherCells + " aether)", true);
    }
    public void DismissInfoPopup()
    {
        if (infoPopup != null)
            AnimationManager.Instance.ChangeAnimationState(infoPopup, "Exit");
    }
    public void DestroyInfoPopup(bool isSecondary = false)
    {
        if (!isSecondary)
        {
            if (infoPopup != null)
            {
                Destroy(infoPopup);
                infoPopup = null;
            }
        }
        else
        {
            if (infoPopup_Secondary != null)
            {
                Destroy(infoPopup_Secondary);
                infoPopup_Secondary = null;
            }
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
    }
    public void CreateCombatEndPopup(bool playerWins)
    {
        DestroyCombatEndPopup();
        combatEndPopup = Instantiate(combatEndPopupPrefab, CurrentCanvas.transform);
        CombatEndPopupDisplay cepd = combatEndPopup.GetComponent<CombatEndPopupDisplay>();
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
    public void CreateNewCardPopup(Card newCard, Card[] chooseCards = null)
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
        
        if (chooseCards == null) ncpd.NewCard = newCard;
        else ncpd.ChooseCards = chooseCards;
        // play sounds
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
    // Aether Cell Popup
    public void CreateAetherCellPopup(int quanity, int total)
    {
        DestroyAetherCellPopup();
        aetherCellPopup = Instantiate(aetherCellPopupPrefab, CurrentCanvas.transform);
        AetherCellPopupDisplay acpd =
            aetherCellPopup.GetComponent<AetherCellPopupDisplay>();
        acpd.AetherQuantity = quanity;
        acpd.TotalAether = total;
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
    public void CreateCardPagePopup(CardPageDisplay.CardPageType cardPageType, bool isScrollPopup = false, bool playSound = true)
    {
        float scrollValue = 1;
        if (cardPagePopup != null && isScrollPopup)
        {
            if (cardPagePopup.GetComponent<CardPageDisplay>().IsScrollPage)
            {
                Debug.LogWarning("IS SCROLL PAGE!");
                Scrollbar sBar = cardPagePopup.GetComponentInChildren<Scrollbar>();
                scrollValue = sBar.value;
            }
        }

        DestroyCardPagePopup();
        GameObject prefab;
        if (isScrollPopup) prefab = cardScrollPopupPrefab;
        else prefab = cardPagePopupPrefab;

        cardPagePopup = Instantiate(prefab, CurrentCanvas.transform);
        cardPagePopup.GetComponent<CardPageDisplay>().DisplayCardPage(cardPageType, playSound, scrollValue);
    }
    public void DestroyCardPagePopup()
    {
        if (cardPagePopup != null)
        {
            Destroy(cardPagePopup);
            cardPagePopup = null;
        }
        DestroyRemoveCardPopup();
        DestroyLearnSkillPopup();
        DestroyCloneUnitPopup();
    }
    // Learn Skill Popup
    public void CreateLearnSkillPopup(SkillCard skillCard)
    {
        DestroyLearnSkillPopup();
        learnSkillPopup = Instantiate(learnSkillPopupPrefab, CurrentCanvas.transform);
        learnSkillPopup.GetComponent<LearnSkillPopupDisplay>().SkillCard = skillCard;
    }
    public void DestroyLearnSkillPopup()
    {
        if (learnSkillPopup != null)
        {
            Destroy(learnSkillPopup);
            learnSkillPopup = null;
        }
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
    public void CreateLocationPopup(Location location)
    {
        if (travelPopup != null || narrativePopup != null) return;
        locationPopup = Instantiate(locationPopupPrefab, CurrentCanvas.transform);
        LocationPopupDisplay lpd = locationPopup.GetComponent<LocationPopupDisplay>();
        lpd.Location = location;
        lpd.TravelButtons.SetActive(false);
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
        lpd.TravelButtons.SetActive(true);
    }
    public void DestroyTravelPopup()
    {
        if (travelPopup != null)
        {
            Destroy(travelPopup);
            travelPopup = null;
        }
    }
    // World Map Popup
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
    public void SetSkybar(bool enabled, bool hideChildren = false)
    {
        skyBar.SetActive(enabled);
        if (enabled)
        {
            ClearAugmentBar();
            ClearItemBar();
            SetAetherCount(pMan.AetherCells, pMan.AetherCells); // TESTING
            foreach (HeroAugment ha in pMan.HeroAugments) 
                CreateAugmentIcon(ha);
            foreach (HeroItem hi in pMan.HeroItems)
                CreateItemIcon(hi);
            foreach (Transform augTran in augmentBar.transform)
                augTran.gameObject.SetActive(!hideChildren);
            foreach (Transform itemTran in itemBar.transform)
                itemTran.gameObject.SetActive(!hideChildren);
        }
    }
    public void SetAetherCount(int newCount, int previousCount)
    {
        if (!skyBar.activeSelf) return;
        TextMeshProUGUI tmpro = aetherCount.GetComponentInChildren<TextMeshProUGUI>();
        if (newCount != previousCount)
        {
            anMan.SkybarIconAnimation(aetherCount); // TESTING
            anMan.CountingText(tmpro, previousCount, newCount); // TESTING
        }
        else tmpro.SetText(newCount.ToString());
    }
    public void CreateAugmentIcon(HeroAugment augment, bool isNewAugment = false)
    {
        if (!skyBar.activeSelf) return;
        GameObject augmentIcon = Instantiate(augmentIconPrefab, augmentBar.transform);
        augmentIcon.GetComponent<AugmentIcon>().LoadedAugment = augment;
        if (isNewAugment)
        {
            anMan.SkybarIconAnimation(augmentIcon); // TESTING
        }
    }
    public void CreateItemIcon(HeroItem item, bool isNewItem = false)
    {
        if (!skyBar.activeSelf) return;
        GameObject itemIcon = Instantiate(itemIconPrefab, itemBar.transform);
        itemIcon.GetComponent<ItemIcon>().LoadedItem = item;
        if (isNewItem)
        {
            anMan.SkybarIconAnimation(itemIcon); // TESTING
        }
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
        float xPos = sourceIcon.transform.localPosition.x;
        augmentIconPopup.transform.localPosition = new Vector2(xPos - 450, 320);
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
        float xPos = sourceIcon.transform.localPosition.x;
        itemIconPopup.transform.localPosition = new Vector2(xPos, 320);
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
        itemAbilityPopup =
                Instantiate(abilityPopupBoxPrefab, CurrentZoomCanvas.transform);
        foreach (CardAbility linkedCa in item.LinkedAbilities)
            CreateAbilityPopup(linkedCa);

        void CreateAbilityPopup(CardAbility ca)
        {
            GameObject abilityPopup =
                    Instantiate(abilityPopupPrefab, itemAbilityPopup.transform);
            abilityPopup.GetComponent<AbilityPopupDisplay>().AbilityScript = ca;
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

    public void CreateBetaFinishPopup() =>
        Instantiate(betaFinishPopupPrefab, CurrentCanvas.transform);
}
