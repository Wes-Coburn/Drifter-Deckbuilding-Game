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

    [Header("SCENE FADER")]
    [SerializeField] private GameObject sceneFader;

    [Header("PREFABS")]
    [SerializeField] private GameObject augmentIconPrefab;
    [SerializeField] private GameObject augmentIconPopupPrefab;
    [SerializeField] private GameObject screenDimmerPrefab;
    [SerializeField] private GameObject infoPopupPrefab;
    [SerializeField] private GameObject combatEndPopupPrefab;
    [SerializeField] private GameObject turnPopupPrefab;
    [SerializeField] private GameObject versusPopupPrefab;
    [SerializeField] private GameObject menuPopupPrefab;
    [SerializeField] private GameObject explicitLanguagePopupPrefab;
    [SerializeField] private GameObject newCardPopupPrefab;
    [SerializeField] private GameObject chooseCardPopupPrefab;
    [SerializeField] private GameObject aetherCellPopupPrefab;
    [SerializeField] private GameObject cardPagePopupPrefab;
    [SerializeField] private GameObject learnSkillPopupPrefab;
    [SerializeField] private GameObject recruitUnitPopupPrefab;
    [SerializeField] private GameObject removeCardPopupPrefab;
    [SerializeField] private GameObject cloneUnitPopupPrefab;
    [SerializeField] private GameObject acquireAugmentPopupPrefab;
    [SerializeField] private GameObject locationPopupPrefab;

    [Header("COLORS")]
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color rejectedColor;

    private PlayerManager pMan;

    private GameObject augmentIconPopup;
    private GameObject screenDimmer;
    private GameObject infoPopup;
    private GameObject combatEndPopup;
    private GameObject turnPopup;
    private GameObject menuPopup;
    private GameObject explicitLanguagePopup;
    private GameObject newCardPopup;
    private GameObject aetherCellPopup;
    private GameObject cardPagePopup;
    private GameObject learnSkillPopup;
    private GameObject recruitUnitPopup;
    private GameObject removeCardPopup;
    private GameObject cloneUnitPopup;
    private GameObject acquireAugmentPopup;
    private GameObject locationPopup;
    private GameObject travelPopup;

    private GameObject endTurnButton;
    private GameObject cancelEffectButton;
    private Coroutine sceneFadeRoutine;
    private GameObject playerZoneOutline;
    
    public GameObject NewCardPopup { get => newCardPopup; }
    public GameObject CardPagePopup { get => cardPagePopup; }
    public GameObject EndTurnButton { get => endTurnButton; }

    // PLAYER_IS_TARGETTING
    public bool PlayerIsTargetting { get; set; }
    // WORLDSPACE
    public GameObject CurrentWorldSpace { get; private set; }
    // CANVAS
    public GameObject CurrentCanvas { get; private set; }
    // ZOOM CANVAS
    public GameObject CurrentZoomCanvas { get; private set; }

    public void Start()
    {
        CurrentWorldSpace = GameObject.Find("WorldSpace");
        CurrentCanvas = GameObject.Find("Canvas");
        CurrentZoomCanvas = GameObject.Find("Canvas_Zoom"); // TESTING
        pMan = PlayerManager.Instance;
        PlayerIsTargetting = false;
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
        endTurnButton = GameObject.Find("EndTurnButton");
        playerZoneOutline = GameObject.Find("PlayerZoneOutline");
        SetCancelEffectButton(false);
        SetPlayerZoneOutline(false, false);
    }
    public void StartWorldMapScene()
    {
        Debug.LogWarning("BLANK!");
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
    public void UpdateEndTurnButton(bool isMyTurn)
    {
        Button button = endTurnButton.GetComponent<Button>();
        button.interactable = isMyTurn;
        EndTurnButtonDisplay etbd =
            endTurnButton.GetComponent<EndTurnButtonDisplay>();
        etbd.EndTurnSide.SetActive(isMyTurn);
        etbd.OpponentTurnSide.SetActive(!isMyTurn);
    }
    /******
     * *****
     * ****** CANCEL_EFFECT_BUTTON
     * *****
     *****/
    public void SetCancelEffectButton(bool isEnabled) =>
        cancelEffectButton.SetActive(isEnabled);
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
        GameObject UICanvas = GameObject.Find("UI_Canvas");
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
    public void CreateInfoPopup(string message, bool isCentered = false)
    {
        DestroyInfoPopup();
        Vector2 vec2 = new Vector2(0, 0);
        if (!isCentered) vec2.Set(680, 50);
        infoPopup = Instantiate(infoPopupPrefab, vec2, 
            Quaternion.identity, CurrentWorldSpace.transform);
        infoPopup.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
    }
    public void CreateFleetingInfoPopup(string message, bool isCentered = false)
    {
        CreateInfoPopup(message, isCentered);
        AnimationManager.Instance.ChangeAnimationState(infoPopup, "Enter_Exit");
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
    public void DestroyInfoPopup()
    {
        if (infoPopup != null)
        {
            Destroy(infoPopup);
            infoPopup = null;
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
    public void CreateVersusPopup()
    {
        DestroyTurnPopup();
        turnPopup = Instantiate(versusPopupPrefab, CurrentCanvas.transform);
    }
    public void CreateCombatEndPopup(bool playerWins)
    {
        DestroyCombatEndPopup();
        combatEndPopup = Instantiate(combatEndPopupPrefab, CurrentCanvas.transform);
        CombatEndPopupDisplay cepd =
            combatEndPopup.GetComponent<CombatEndPopupDisplay>();
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
    // New Card Popup
    public void CreateNewCardPopup(Card newCard, Card[] chooseCards = null)
    {
        DestroyNewCardPopup();
        if (chooseCards == null) newCardPopup = Instantiate(newCardPopupPrefab, CurrentCanvas.transform);
        else newCardPopup = Instantiate(chooseCardPopupPrefab, CurrentCanvas.transform);
        NewCardPopupDisplay ncpd = newCardPopup.GetComponent<NewCardPopupDisplay>();
        if (chooseCards == null) ncpd.NewCard = newCard;
        else ncpd.ChooseCards = chooseCards; // TESTING
        // play sounds
    }
    public void DestroyNewCardPopup()
    {
        if (newCardPopup != null)
        {
            Destroy(newCardPopup);
            newCardPopup = null;
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
    public void CreateCardPagePopup(CardPageDisplay.CardPageType cardPageType)
    {
        DestroyCardPagePopup();
        cardPagePopup = Instantiate(cardPagePopupPrefab, CurrentCanvas.transform);
        cardPagePopup.GetComponent<CardPageDisplay>().DisplayCardPage(cardPageType);
    }
    public void DestroyCardPagePopup()
    {
        if (cardPagePopup != null)
        {
            Destroy(cardPagePopup);
            cardPagePopup = null;
        }
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
    // Acquire Augment Popup
    public void CreateAcquireAugmentPopup(HeroAugment heroAugment)
    {
        DestroyLearnSkillPopup();
        acquireAugmentPopup = Instantiate(acquireAugmentPopupPrefab, CurrentCanvas.transform);
        acquireAugmentPopup.GetComponent<AcquireAugmentPopupDisplay>().HeroAugment = heroAugment;
    }
    public void DestroyAcquireAugmentPopup()
    {
        if (acquireAugmentPopup != null)
        {
            Destroy(acquireAugmentPopup);
            acquireAugmentPopup = null;
        }
    }
    
    // Location Popup
    public void CreateLocationPopup(Location location)
    {
        if (travelPopup != null) return;
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

    /******
     * *****
     * ****** SKYBAR
     * *****
     *****/
    public void SetSkybar(bool enabled)
    {
        skyBar.SetActive(enabled);
        if (enabled)
        {
            ClearAugmentBar();
            SetAetherCount(pMan.AetherCells);
            foreach (HeroAugment ha in pMan.HeroAugments) 
                CreateAugmentIcon(ha);
        }
    }
    public void SetAetherCount(int count)
    {
        if (!skyBar.activeSelf) return;
        aetherCount.GetComponentInChildren<TextMeshProUGUI>().
            SetText(count.ToString());
    }
    public void CreateAugmentIcon(HeroAugment augment)
    {
        if (!skyBar.activeSelf) return;
        GameObject augmentIcon = Instantiate(augmentIconPrefab, augmentBar.transform);
        augmentIcon.GetComponent<AugmentIcon>().LoadedAugment = augment;
    }
    public void ClearAugmentBar()
    {
        if (!skyBar.activeSelf) return;
        foreach (Transform tran in augmentBar.transform) 
            Destroy(tran.gameObject);
    }
    public void CreateAugmentIconPopup(HeroAugment augment, GameObject sourceIcon)
    {
        DestroyAugmentIconPopup();
        augmentIconPopup = Instantiate(augmentIconPopupPrefab, CurrentCanvas.transform);
        float xPos = sourceIcon.transform.localPosition.x;
        augmentIconPopup.transform.localPosition = new Vector2(xPos - 300, 320);
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
}
