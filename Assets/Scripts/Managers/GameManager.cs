using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    #region FIELDS
    [Header("LOCATION BACKGROUNDS")]
    [SerializeField] private Sprite locationBG_City;
    [SerializeField] private Sprite locationBG_Wasteland;
    [Header("LOCATION ICON")]
    [SerializeField] GameObject locationIconPrefab;
    [Header("LOADING TIPS")]
    [SerializeField][TextArea] private string[] loadingTips;
    [Header("REPUTATION BONUSES")]
    [SerializeField] private ReputationBonuses reputation_Mages;
    [SerializeField] private ReputationBonuses reputation_Mutants;
    [SerializeField] private ReputationBonuses reputation_Rogues;
    [SerializeField] private ReputationBonuses reputation_Techs;
    [SerializeField] private ReputationBonuses reputation_Warriors;
    [Header("MULLIGAN EFFECT")]
    [SerializeField] private EffectGroup mulliganEffect;
    [Header("COGNITIVE MAGNIFIER EFFECT")]
    [SerializeField] private EffectGroup cognitiveMagnifierEffect;

    private Narrative settingNarrative;
    private Narrative newGameNarrative;

    private int currentTip;

    // Player Preferences
    public const string MUSIC_VOLUME = "MusicVolume";
    public const string SFX_VOLUME = "SFXVolume";
    public const string HIDE_EXPLICIT_LANGUAGE = "HideExplicitLanguage";

    // Universal
    public const int WOUNDED_VALUE = 5;
    public const int START_HAND_SIZE = 4;
    public const int MAX_HAND_SIZE = 10;
    public const int MAX_UNITS_PLAYED = 6;
    public const int START_ENERGY_PER_TURN = 0;

    // Player
    public const string PLAYER = "Player";
    public const int MINIMUM_DECK_SIZE = 15;
    public const int PLAYER_STARTING_HEALTH = 30;
    //public const int PLAYER_STARTING_HEALTH = 1; // FOR TESTING ONLY
    public const int PLAYER_START_UNITS = 3;
    public const int MAXIMUM_ENERGY_PER_TURN = 10;
    public const int MAXIMUM_ENERGY = 10;
    public const int MAXIMUM_ITEMS = 2;
    public const int HERO_ULTMATE_GOAL = 3;
    public const int PLAYER_START_AETHER = 30;

    // Enemy
    public const string ENEMY = "Enemy";
    public const int ENEMY_STARTING_HEALTH = 30;
    //public const int ENEMY_STARTING_HEALTH = 1; // FOR TESTING ONLY
    // Tutorial Enemy
    public const int TUTORIAL_STARTING_HEALTH = 10;
    // Boss Enemy
    public const int BOSS_BONUS_ENERGY = 0; // Unnecessary if 0

    // Aether Rewards
    public const int IGNORE_CARD_AETHER = 15;
    public const int REDRAW_CARDS_AETHER = 5;
    // Sell Cards
    public const int SELL_COMMON_CARD_VALUE = 10;
    public const int SELL_RARE_CARD_VALUE = 15;
    public const int SELL_LEGEND_CARD_VALUE = 20;
    // Recruits
    public const int RECRUIT_COMMON_UNIT_COST = 30;
    public const int RECRUIT_RARE_UNIT_COST = 40;
    public const int RECRUIT_LEGEND_UNIT_COST = 50;
    public const int RECRUIT_LOYALTY_GOAL = 3;
    // Actions
    public const int BUY_COMMON_ACTION_COST = 30;
    public const int BUY_RARE_ACTION_COST = 40;
    public const int BUY_LEGEND_ACTION_COST = 50;
    public const int ACTION_LOYALTY_GOAL = 3;
    // Cloning
    public const int CLONE_COMMON_UNIT_COST = 35;
    public const int CLONE_RARE_UNIT_COST = 45;
    public const int CLONE_LEGEND_UNIT_COST = 55;
    // Items
    public const int BUY_ITEM_COST = 35;
    public const int BUY_RARE_ITEM_COST = 50;
    public const int SHOP_LOYALTY_GOAL = 3;
    // Sell Items
    public const int SELL_ITEM_VALUE = 15;
    public const int SELL_RARE_ITEM_VALUE = 20;
    // Reputation
    public const int REPUTATION_TIER_1 = 10;
    public const int REPUTATION_TIER_2 = 15;
    public const int REPUTATION_TIER_3 = 20;

    // Combat Reward
    public const int AETHER_COMBAT_REWARD_1 = 30;
    public const int AETHER_COMBAT_REWARD_2 = 35;
    public const int AETHER_COMBAT_REWARD_3 = 40;

    public const int AETHER_COMBAT_REWARD_BOSS_1 = 60;
    public const int AETHER_COMBAT_REWARD_BOSS_2 = 70;
    public const int AETHER_COMBAT_REWARD_BOSS_3 = 80;

    public const int ADDITIONAL_AETHER_REWARD = 10;

    // Augments
    public const int AETHER_MAGNET_REWARD = 20;
    #endregion

    #region PROPERTIES
    public string CurrentTip
    {
        get
        {
            string tip = loadingTips[currentTip++];
            if (currentTip > loadingTips.Length - 1)
                currentTip = 0;
            return tip;
        }
    }

    public ReputationBonuses ReputationBonus_Mages { get => reputation_Mages; }
    public ReputationBonuses ReputationBonus_Mutants { get => reputation_Mutants; }
    public ReputationBonuses ReputationBonus_Rogues { get => reputation_Rogues; }
    public ReputationBonuses ReputationBonus_Techs { get => reputation_Techs; }
    public ReputationBonuses ReputationBonus_Warriors { get => reputation_Warriors; }

    public bool IsCombatTest { get; set; }
    public bool IsTutorial { get; set; }
    public bool HideExplicitLanguage { get; set; }

    public bool IsNewHour { get; set; }
    public int CurrentHour { get; set; }
    public Narrative CurrentNarrative { get; set; }
    public Location CurrentLocation { get; set; }
    public List<NPCHero> ActiveNPCHeroes { get; private set; }
    public List<Location> ActiveLocations { get; private set; }
    public List<string> VisitedLocations { get; private set; }
    public List<HeroItem> ShopItems { get; private set; }
    public int RecruitLoyalty { get; set; }
    public int ActionShopLoyalty { get; set; }
    public int ShopLoyalty { get; set; }

    // REPUTATION
    public int Reputation_Mages { get; set; }
    public int Reputation_Mutants { get; set; }
    public int Reputation_Rogues { get; set; }
    public int Reputation_Techs { get; set; }
    public int Reputation_Warriors { get; set; }
    #endregion

    #region METHODS
    /******
     * *****
     * ****** START
     * *****
     *****/
    private void Start()
    {
        //PlayerPrefs.DeleteAll(); // FOR TESTING ONLY
        currentTip = Random.Range(0, loadingTips.Length);
        ActiveNPCHeroes = new List<NPCHero>();
        ActiveLocations = new List<Location>();
        VisitedLocations = new List<string>();
        ShopItems = new List<HeroItem>();

        LoadPlayerPreferences();
        Debug.Log("Application Version: " + Application.version);
        //SceneLoader.LoadScene(SceneLoader.Scene.TitleScene, false, false); // Remove if loading asynchronous asset bundles
    }

    /******
     * *****
     * ****** GET_LOCATION_BACKGROUND
     * *****
     *****/
    public Sprite GetLocationBackground()
    {
        if (CurrentLocation == null)
        {
            Debug.LogWarning("CURRENT LOCATION IS NULL!");
            return null;
        }

        Location.Background background = CurrentLocation.LocationBackground;

        switch (background)
        {
            case Location.Background.City:
                return locationBG_City;
            case Location.Background.Wasteland:
                return locationBG_Wasteland;
            default:
                Debug.LogError("INVALID LOCATION BACKGROUND!");
                return null;
        }
    }

    /******
     * *****
     * ****** GET_RANDOM_ENCOUNTER_LOCATION
     * *****
     *****/
    public void AddRandomEncounter()
    {
        Location[] randomEncounters = Resources.LoadAll<Location>("Random Encounters");
        randomEncounters.Shuffle();

        foreach (Location location in randomEncounters)
        {
            if (VisitedLocations.FindIndex
                (x => x == location.LocationName) == -1)
            {
                GetActiveLocation(location);
                return;
            }
        }
        Debug.LogWarning("NO VALID RANDOM ENCOUNTERS!");
    }

    /******
     * *****
     * ****** PLAY_TUTORIAL
     * *****
     *****/
    public void PlayTutorial()
    {
        IsTutorial = true;
        SceneLoader.LoadAction += Tutorial_Load;
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
    }

    private void Tutorial_Load()
    {
        ManagerHandler.P_MAN.AetherCells = 0;
        string playerHeroName = "Kili, Neon Rider";
        string enemyHeroName = "Tiny Mutant";
        Hero[] heroes = Resources.LoadAll<Hero>("Tutorial");
        EnemyHero enemyHero = ScriptableObject.CreateInstance<EnemyHero>();

        foreach (Hero hero in heroes)
        {
            if (hero.HeroName == playerHeroName) ManagerHandler.P_MAN.HeroScript = hero as PlayerHero;
            else if (hero.HeroName == enemyHeroName)
            {
                enemyHero.LoadHero(hero);
                ManagerHandler.D_MAN.EngagedHero = enemyHero;
            }
        }

        foreach (UnitCard unit in ManagerHandler.CA_MAN.TutorialPlayerUnits)
            for (int i = 0; i < 5; i++)
                ManagerHandler.CA_MAN.AddCard(unit, PLAYER);
    }

    public void Tutorial_Tooltip(int tipNumber)
    {
        string tip;
        bool isCentered = true;
        bool showContinue = false;
        switch (tipNumber)
        {
            case 1:
                tip = "Redraw any number of cards from your starting hand. Click each card you want to redraw, " +
                    "then click the <color=\"yellow\"><b>Confirm Button</b></color> (or press the <color=\"yellow\"><b>Space Bar</b></color>).";
                break;
            case 2:
                tip = "Play a card by dragging it out of your hand. Cards you can play are highlighted in <color=\"green\"><b>green</b></color>.";
                break;
            case 3:
                tip = "End your turn by clicking the <color=\"yellow\"><b>End Turn Button</b></color> " +
                    "(or pressing the <color=\"yellow\"><b>Space Bar</b></color>).";
                break;
            case 4:
                tip = "Click your hero power to use it (below your hero's portrait).";
                break;
            case 5:
                tip = "Attack an enemy unit by dragging an ally to them.";
                isCentered = false;
                break;
            case 6:
                tip = "<b>Attack the enemy hero to win!</b>\nRead more game rules in settings (top right).</b>";
                showContinue = true;
                break;
            default:
                Debug.LogError("INVALID TIP NUMBER!");
                return;
        }
        ManagerHandler.U_MAN.CreateInfoPopup(tip, UIManager.InfoPopupType.Tutorial, isCentered, showContinue);
    }

    /******
     * *****
     * ****** NEW_GAME
     * *****
     *****/
    public void NewGame()
    {
        SceneLoader.LoadAction += NewGame_Load;
        SceneLoader.LoadScene(SceneLoader.Scene.NarrativeScene);
    }

    private void NewGame_Load()
    {
        Narrative[] allNarratives = Resources.LoadAll<Narrative>("Narratives");
        Narrative LoadNarrative(string narrative)
        {
            foreach (Narrative n in allNarratives)
                if (n.NarrativeName == narrative) return n;

            Debug.LogError($"NARRATIVE {narrative} NOT FOUND!");
            return null;
        }

        // Narratives
        settingNarrative = LoadNarrative("Welcome to the Drift");
        newGameNarrative = LoadNarrative("Part 1: Stuck in Sylus");
        CurrentNarrative = settingNarrative;

        // Locations
        Location[] allLocations = Resources.LoadAll<Location>("Locations");
        Location LoadLocation(string location)
        {
            foreach (Location l in allLocations)
            {
                if (l.LocationFullName == location)
                    return l;
            }
            Debug.LogError($"NARRATIVE {location} NOT FOUND!");
            return null;
        }
        GetActiveLocation(LoadLocation("Your Ship"));
        GetActiveLocation(LoadLocation("Sekherd and 7th"));
        GetActiveLocation(LoadLocation("The Rathole Bar and Lounge"));
        GetActiveLocation(LoadLocation("The Emporium"));
        GetActiveLocation(LoadLocation("The Trash Heaps"));
        GetActiveLocation(LoadLocation("The Oasis"));

        CurrentHour = 4;
        IsNewHour = true;
        IsTutorial = false;
        ManagerHandler.CA_MAN.LoadNewRecruits();
        ManagerHandler.CA_MAN.LoadNewActions();
        ShopItems = GetShopItems();
        RecruitLoyalty = 3; // First recruit free
        ActionShopLoyalty = 3; // First action free
        ShopLoyalty = 3; // First item free
        ManagerHandler.P_MAN.AetherCells = PLAYER_START_AETHER;
        ManagerHandler.P_MAN.DeckList.Clear();

        // REPUTATION
        int startRep = PLAYER_START_UNITS;
        Reputation_Mages = startRep;
        Reputation_Mutants = startRep;
        Reputation_Rogues = startRep;
        Reputation_Techs = startRep;
        Reputation_Warriors = startRep;
    }

    /******
     * *****
     * ****** END_GAME
     * *****
     *****/
    public void EndGame()
    {
        // Game Manager
        foreach (NPCHero npc in ActiveNPCHeroes)
        {
            if (npc != null) Destroy(npc);
        }
        ActiveNPCHeroes.Clear();

        foreach (Location loc in ActiveLocations)
        {
            if (loc != null) Destroy(loc);
        }
        ActiveLocations.Clear();
        VisitedLocations.Clear();

        // Player Manager
        // Don't destroy ManagerHandler.P_MAN objects, they are assets not instances
        ManagerHandler.P_MAN.HeroScript = null;
        ManagerHandler.P_MAN.DeckList.Clear();
        ManagerHandler.P_MAN.CurrentDeck.Clear();
        //ManagerHandler.P_MAN.AetherCells = 0;
        ManagerHandler.P_MAN.HeroAugments.Clear();
        ManagerHandler.P_MAN.HeroItems.Clear();
        // Enemy Manager
        Destroy(ManagerHandler.EN_MAN.HeroScript);
        ManagerHandler.EN_MAN.HeroScript = null;
        // Dialogue Manager
        ManagerHandler.D_MAN.EndDialogue();
        // UI Manager
        ManagerHandler.U_MAN.ClearAugmentBar();
        ManagerHandler.U_MAN.ClearItemBar();
        // Scene Loader
        SceneLoader.LoadScene(SceneLoader.Scene.TitleScene);
    }

    /******
     * *****
     * ****** SAVE_PLAYER_PREFERENCES
     * *****
     *****/
    public void SavePlayerPreferences()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, ManagerHandler.AU_MAN.MusicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME, ManagerHandler.AU_MAN.SFXVolume);

        int hideExplicit = 0;
        if (HideExplicitLanguage) hideExplicit = 1;
        PlayerPrefs.SetInt(HIDE_EXPLICIT_LANGUAGE, hideExplicit);
    }
    public void LoadPlayerPreferences()
    {
        ManagerHandler.AU_MAN.MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1);
        ManagerHandler.AU_MAN.SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1);
        HideExplicitLanguage = PlayerPrefs.GetInt(HIDE_EXPLICIT_LANGUAGE, 1) == 1;
    }

    /******
     * *****
     * ****** SAVE_GAME
     * *****
     *****/
    public bool CheckSave() => SaveLoad.LoadGame() != null;
    public void SaveGame() // PUBLIC FOR BETA ONLY
    {
        string[] deckList = new string[ManagerHandler.P_MAN.DeckList.Count];
        for (int i = 0; i < deckList.Length; i++)
            deckList[i] = ManagerHandler.P_MAN.DeckList[i].CardName;

        string[] augments = new string[ManagerHandler.P_MAN.HeroAugments.Count];
        for (int i = 0; i < augments.Length; i++)
            augments[i] = ManagerHandler.P_MAN.HeroAugments[i].AugmentName;

        string[] items = new string[ManagerHandler.P_MAN.HeroItems.Count];
        for (int i = 0; i < items.Length; i++)
            items[i] = ManagerHandler.P_MAN.HeroItems[i].ItemName;

        string[,] npcsAndClips = new string[ActiveNPCHeroes.Count, 2];
        for (int i = 0; i < npcsAndClips.Length / 2; i++)
        {
            npcsAndClips[i, 0] = ActiveNPCHeroes[i].HeroName;

            DialogueClip clip = ActiveNPCHeroes[i].NextDialogueClip;
            string clipName = clip.ToString();
            clipName = clipName.Replace($" ({clip.GetType().Name})", "");
            npcsAndClips[i, 1] = clipName;
        }

        string[,] locationsNPCsObjectives = new string[ActiveLocations.Count, 3];
        for (int i = 0; i < locationsNPCsObjectives.Length / 3; i++)
        {
            locationsNPCsObjectives[i, 0] = ActiveLocations[i].LocationName;
            locationsNPCsObjectives[i, 1] = ActiveLocations[i].CurrentNPC.HeroName;
            locationsNPCsObjectives[i, 2] = ActiveLocations[i].CurrentObjective;
        }

        string[] shopItems = new string[ShopItems.Count];
        for (int i = 0; i < shopItems.Length; i++)
            shopItems[i] = ShopItems[i].ItemName;

        string[] recruitUnits = new string[ManagerHandler.CA_MAN.PlayerRecruitUnits.Count];
        for (int i = 0; i < recruitUnits.Length; i++)
            recruitUnits[i] = ManagerHandler.CA_MAN.PlayerRecruitUnits[i].CardName;

        string[] shopActions = new string[ManagerHandler.CA_MAN.ActionShopCards.Count];
        for (int i = 0; i < shopActions.Length; i++)
            shopActions[i] = ManagerHandler.CA_MAN.ActionShopCards[i].CardName;

        string narrativeName = "";
        if (CurrentNarrative != null)
        {
            narrativeName = CurrentNarrative.ToString();
            narrativeName = narrativeName.Replace(" (Narrative)", "");
        }

        GameData data = new(CurrentHour, narrativeName, ManagerHandler.P_MAN.HeroScript.HeroName,
            deckList, augments, items, ManagerHandler.P_MAN.AetherCells,
            npcsAndClips, locationsNPCsObjectives, VisitedLocations.ToArray(),
            shopItems, recruitUnits, shopActions,
            RecruitLoyalty, ActionShopLoyalty, ShopLoyalty,
            Reputation_Mages, Reputation_Mutants, Reputation_Rogues, Reputation_Techs, Reputation_Warriors);

        SaveLoad.SaveGame(data);
    }

    /******
     * *****
     * ****** LOAD_GAME
     * *****
     *****/
    public bool LoadGame()
    {
        GameData data = SaveLoad.LoadGame();
        if (data == null) return false;

        // CURRENT HOUR
        CurrentHour = data.CurrentHour;

        // CURRENT NARRATIVE
        if (data.CurrentNarrative != "")
        {
            CurrentNarrative = Resources.Load<Narrative>($"Narratives/{data.CurrentNarrative}");
            if (CurrentNarrative == null) Debug.LogError($"NARRATIVE {data.CurrentNarrative} NOT FOUND!");
        }
        else CurrentNarrative = null;

        // PLAYER HERO
        PlayerHero pHero;
        pHero = Resources.Load<PlayerHero>($"Heroes/Player Heroes/{data.PlayerHero}");
        if (pHero == null) Debug.LogError($"HERO {data.PlayerHero} NOT FOUND!");
        else ManagerHandler.P_MAN.HeroScript = pHero;

        // DECK LIST
        ManagerHandler.P_MAN.DeckList.Clear();
        for (int i = 0; i < data.PlayerDeck.Length; i++)
        {
            Card card;
            card = Resources.Load<Card>($"Cards_Starting/{data.PlayerDeck[i]}");
            if (card == null) card = Resources.Load<Card>($"Cards_Units/{data.PlayerDeck[i]}");
            if (card == null) card = Resources.Load<Card>($"Cards_Actions/{data.PlayerDeck[i]}");
            if (card == null) Debug.LogError($"CARD {data.PlayerDeck[i]} NOT FOUND!");
            else ManagerHandler.CA_MAN.AddCard(card, PLAYER);
        }

        // AUGMENTS
        ManagerHandler.P_MAN.HeroAugments.Clear();
        for (int i = 0; i < data.PlayerAugments.Length; i++)
        {
            HeroAugment augment;
            augment = Resources.Load<HeroAugment>($"Hero Augments/{data.PlayerAugments[i]}");
            if (augment == null) Debug.LogError($"AUGMENT {data.PlayerAugments[i]} NOT FOUND!");
            else ManagerHandler.P_MAN.HeroAugments.Add(augment);
        }

        // ITEMS
        ManagerHandler.P_MAN.HeroItems.Clear();
        for (int i = 0; i < data.PlayerItems.Length; i++)
        {
            HeroItem item;
            item = Resources.Load<HeroItem>($"Hero Items/{data.PlayerItems[i]}");
            if (item == null) Debug.LogError($"ITEM {data.PlayerItems[i]} NOT FOUND!");
            else ManagerHandler.P_MAN.HeroItems.Add(item);
        }

        // AETHER CELLS
        ManagerHandler.P_MAN.AetherCells = data.AetherCells;

        // NPCS
        ActiveNPCHeroes.Clear();
        for (int i = 0; i < data.NPCSAndClips.Length / 2; i++)
        {
            NPCHero npc;
            npc = Resources.Load<NPCHero>($"Heroes/NPC Heroes/{data.NPCSAndClips[i, 0]}");
            if (npc == null) Debug.LogError($"NPC {data.NPCSAndClips[i, 0]} NOT FOUND!");
            else
            {
                npc = GetActiveNPC(npc);
                DialogueClip clip;
                clip = Resources.Load<DialogueClip>($"Dialogue/{npc.HeroName}/{data.NPCSAndClips[i, 1]}");
                if (clip == null) Debug.LogError($"CLIP {data.NPCSAndClips[i, 1]} FOR {npc.HeroName} NOT FOUND!");
                else npc.NextDialogueClip = clip;
            }
        }

        // LOCATIONS
        ActiveLocations.Clear();
        for (int i = 0; i < data.LocationsNPCsObjectives.Length / 3; i++)
        {
            Location loc;
            string name = data.LocationsNPCsObjectives[i, 0];
            loc = Resources.Load<Location>($"Random Encounters/{name}");
            if (loc == null) loc = Resources.Load<Location>($"Locations/{name}");
            if (loc == null) Debug.LogError($"LOCATION {name} NOT FOUND!");
            else
            {
                loc = GetActiveLocation(loc);

                // null checks
                loc.CurrentNPC = GetActiveNPC(Resources.Load<NPCHero>($"Heroes/NPC Heroes/{data.LocationsNPCsObjectives[i, 1]}"));
                loc.CurrentObjective = data.LocationsNPCsObjectives[i, 2];
            }
        }
        VisitedLocations.Clear();
        foreach (string location in data.VisitedLocations)
            VisitedLocations.Add(location);

        // SHOP ITEMS
        ShopItems.Clear();
        for (int i = 0; i < data.ShopItems.Length; i++)
            ShopItems.Add(Resources.Load<HeroItem>("Hero Items/" + data.ShopItems[i]));

        // RECRUIT UNITS
        ManagerHandler.CA_MAN.PlayerRecruitUnits.Clear();
        for (int i = 0; i < data.RecruitUnits.Length; i++)
            ManagerHandler.CA_MAN.PlayerRecruitUnits.Add(Resources.Load<UnitCard>("Cards_Units/" + data.RecruitUnits[i]));

        // SHOP ACTIONS
        ManagerHandler.CA_MAN.ActionShopCards.Clear();
        for (int i = 0; i < data.ShopActions.Length; i++)
            ManagerHandler.CA_MAN.ActionShopCards.Add(Resources.Load<ActionCard>("Cards_Actions/" + data.ShopActions[i]));

        // LOYALTY
        RecruitLoyalty = data.RecruitLoyalty;
        ActionShopLoyalty = data.ActionShopLoyalty;
        ShopLoyalty = data.ShopLoyalty;

        // REPUTATION
        Reputation_Mages = data.Reputation_Mages;
        Reputation_Mutants = data.Reputation_Mutants;
        Reputation_Rogues = data.Reputation_Rogues;
        Reputation_Techs = data.Reputation_Techs;
        Reputation_Warriors = data.Reputation_Warriors;

        return true;
    }

    /******
     * *****
     * ****** START_TITLE_SCENE
     * *****
     *****/
    public void StartTitleScene()
    {
        ManagerHandler.AU_MAN.StartStopSound("Soundtrack_TitleScene", null, AudioManager.SoundType.Soundtrack);
        ManagerHandler.AU_MAN.StartStopSound("Soundscape_TitleScene", null, AudioManager.SoundType.Soundscape);
        GameObject.Find("VersionNumber").GetComponent<TextMeshProUGUI>().SetText(Application.version);

        IsCombatTest = false;
    }

    /******
     * *****
     * ****** START_HERO_SELECT_SCENE
     * *****
     *****/
    public void StartHeroSelectScene()
    {
        ManagerHandler.AU_MAN.StopCurrentSoundscape();
        FindObjectOfType<HeroSelectSceneDisplay>().DisplaySelectedHero();
    }

    /******
     * *****
     * ****** ENTER_WORLD_MAP
     * *****
     *****/
    public void EnterWorldMap()
    {
        ManagerHandler.AU_MAN.StartStopSound("Soundtrack_WorldMapScene", null, AudioManager.SoundType.Soundtrack);
        ManagerHandler.AU_MAN.StartStopSound("SFX_EnterWorldMap");

        foreach (Location loc in ActiveLocations)
        {
            GameObject location = Instantiate(locationIconPrefab, ManagerHandler.U_MAN.CurrentCanvas.transform);
            location.GetComponent<LocationIcon>().Location = loc;
        }

        SaveGame();
        if (CurrentNarrative != null)
        {
            if (CurrentNarrative.IsGameEnd) ManagerHandler.U_MAN.CreateGameEndPopup();
            else ManagerHandler.U_MAN.CreateNarrativePopup(CurrentNarrative);
            CurrentNarrative = null;
        }

        FindObjectOfType<TimeClockDisplay>().SetClockValues(CurrentHour, IsNewHour);
        if (IsNewHour) IsNewHour = false;
    }

    public enum DifficultyLevel
    {
        Standard_1 = 1,
        Standard_2 = 2,
        Boss_1 = 3,
        Standard_3 = 4,
        Boss_2 = 5,
    }
    //public int GetSurgeDelay(int difficulty) => 7 - (2 * (difficulty - 1));
    public int GetSurgeDelay(int difficulty) => 6 - difficulty;
    public int GetAetherReward(DifficultyLevel difficultyLevel)
    {
        int reward;
        switch (difficultyLevel)
        {
            case DifficultyLevel.Standard_1:
                reward = AETHER_COMBAT_REWARD_1;
                break;
            case DifficultyLevel.Standard_2:
                reward = AETHER_COMBAT_REWARD_2;
                break;
            case DifficultyLevel.Standard_3:
                reward = AETHER_COMBAT_REWARD_3;
                break;

            case DifficultyLevel.Boss_1:
                reward = AETHER_COMBAT_REWARD_BOSS_1;
                break;
            case DifficultyLevel.Boss_2:
                reward = AETHER_COMBAT_REWARD_BOSS_2;
                break;
            default:
                Debug.LogError("INVALID DIFFICULTY!");
                return 0;
        }

        return reward + ADDITIONAL_AETHER_REWARD * (ManagerHandler.CO_MAN.DifficultyLevel - 1);
    }

    /******
     * *****
     * ****** CHANGE_REPUTATION
     * *****
     *****/
    public enum ReputationType
    {
        Mages,
        Mutants,
        Rogues,
        Techs,
        Warriors
    }

    public void ChangeReputation(ReputationType repType, int repChange)
    {
        if (repChange == 0)
        {
            Debug.LogError("REP CHANGE IS 0!");
            return;
        }

        switch (repType)
        {
            case ReputationType.Mages:
                Reputation_Mages += repChange;
                break;
            case ReputationType.Mutants:
                Reputation_Mutants += repChange;
                break;
            case ReputationType.Rogues:
                Reputation_Rogues += repChange;
                break;
            case ReputationType.Techs:
                Reputation_Techs += repChange;
                break;
            case ReputationType.Warriors:
                Reputation_Warriors += repChange;
                break;
        }
        ManagerHandler.EV_MAN.NewDelayedAction(() => ManagerHandler.U_MAN.SetReputation(repType, repChange), 0.5f);
    }

    public int GetReputation(ReputationType repType)
    {
        switch (repType)
        {
            case ReputationType.Mages:
                return Reputation_Mages;
            case ReputationType.Mutants:
                return Reputation_Mutants;
            case ReputationType.Rogues:
                return Reputation_Rogues;
            case ReputationType.Techs:
                return Reputation_Techs;
            case ReputationType.Warriors:
                return Reputation_Warriors;
            default:
                Debug.LogError("INVALID REPUTATION TYPE!");
                return 0;
        }
    }

    public int GetReputationTier(ReputationType repType)
    {
        int reputation = GetReputation(repType);
        int tier;
        if (reputation < REPUTATION_TIER_1) tier = 0;
        else if (reputation < REPUTATION_TIER_2) tier = 1;
        else if (reputation < REPUTATION_TIER_3) tier = 2;
        else tier = 3;
        return tier;
    }

    public ReputationBonuses GetReputationBonuses(ReputationType repType)
    {
        switch (repType)
        {
            case ReputationType.Mages:
                return ReputationBonus_Mages;
            case ReputationType.Mutants:
                return ReputationBonus_Mutants;
            case ReputationType.Rogues:
                return ReputationBonus_Rogues;
            case ReputationType.Techs:
                return ReputationBonus_Techs;
            case ReputationType.Warriors:
                return ReputationBonus_Warriors;
            default:
                Debug.LogError("INVALID REPUTATION TYPE!");
                return null;
        }
    }

    private void ResolveReputationEffects(int resolveOrder)
    {
        // MAGES
        int mageTier = GetReputationTier(ReputationType.Mages);
        ReputationBonuses mageBonuses = GetReputationBonuses(ReputationType.Mages);
        GetBonusEffects(mageBonuses, mageTier);

        // MUTANTS
        int mutantTier = GetReputationTier(ReputationType.Mutants);
        ReputationBonuses mutantBonuses = GetReputationBonuses(ReputationType.Mutants);
        GetBonusEffects(mutantBonuses, mutantTier);

        // ROGUES
        int rogueTier = GetReputationTier(ReputationType.Rogues);
        ReputationBonuses rogueBonuses = GetReputationBonuses(ReputationType.Rogues);
        GetBonusEffects(rogueBonuses, rogueTier);

        // TECHS
        int techTier = GetReputationTier(ReputationType.Techs);
        ReputationBonuses techBonuses = GetReputationBonuses(ReputationType.Techs);
        GetBonusEffects(techBonuses, techTier);

        // WARRIORS
        int warriorTier = GetReputationTier(ReputationType.Warriors);
        ReputationBonuses warriorBonuses = GetReputationBonuses(ReputationType.Warriors);
        GetBonusEffects(warriorBonuses, warriorTier);

        void GetBonusEffects(ReputationBonuses bonuses, int reputationTier)
        {
            float delay = 0;
            if (resolveOrder == 3) delay = 0.5f;

            if (reputationTier > 0 && resolveOrder == bonuses.Tier1_ResolveOrder)
                ScheduleEffects(bonuses.ReputationType, bonuses.Tier1_Effects);

            if (reputationTier > 1 && resolveOrder == bonuses.Tier2_ResolveOrder)
                ScheduleEffects(bonuses.ReputationType, bonuses.Tier2_Effects);

            if (reputationTier > 2 && resolveOrder == bonuses.Tier3_ResolveOrder)
                ScheduleEffects(bonuses.ReputationType, bonuses.Tier3_Effects);

            void ScheduleEffects(ReputationType repType, List<EffectGroup> effects)
            {
                bool showTrigger;
                if (resolveOrder == 1) showTrigger = false;
                else showTrigger = true;

                ManagerHandler.EV_MAN.NewDelayedAction(() => ResolveEffects(repType, effects, showTrigger), delay);
            }
        }

        void ResolveEffects(ReputationType repType, List<EffectGroup> repEffects, bool showTrigger)
        {
            if (showTrigger) ManagerHandler.U_MAN.SetReputation(repType, 0, true);

            if (repEffects != null && repEffects.Count > 0)
                ManagerHandler.EF_MAN.StartEffectGroupList(repEffects, ManagerHandler.P_MAN.HeroObject);
        }
    }

    /******
     * *****
     * ****** NEXT_HOUR
     * *****
     *****/
    public void NextHour(bool addRandomEncounter)
    {
        if (CurrentHour > 4)
        {
            Debug.LogError("CURRENT HOUR > 4!");
            return;
        }

        // NEW HOUR
        IsNewHour = true;
        // NEW DAY
        if (CurrentHour == 4) CurrentHour = 1;
        // NEXT HOUR
        else CurrentHour++;
        // RANDOM ENCOUNTER
        if (addRandomEncounter && CurrentHour != 4)
            AddRandomEncounter();
    }

    /******
     * *****
     * ****** LOCATION_OPEN
     * *****
     *****/
    public bool LocationOpen(Location location)
    {
        if (location.IsHomeBase) return true; // If the location is homebase, return true ALWAYS
        if (CurrentHour == 4) return false; // If the current hour is 4, return false ALWAYS

        if (location.IsPriorityLocation &&
            VisitedLocations.FindIndex(x => location.LocationName == x) == -1)
            return true; // If a priority location has NOT been visited, it's NEVER closed

        switch (CurrentHour)
        {
            case 1:
                if (location.IsClosed_Hour1) return false;
                return true;
            case 2:
                if (location.IsClosed_Hour2) return false;
                return true;
            case 3:
                if (location.IsClosed_Hour3) return false;
                return true;
            default:
                Debug.LogError($"INVALID HOUR! <{CurrentHour}>");
                return false;
        }
    }

    /******
     * *****
     * ****** ENTER_HOME_BASE
     * *****
     *****/
    public void EnterHomeBase()
    {
        ManagerHandler.AU_MAN.StopCurrentSoundscape();
        ManagerHandler.AU_MAN.StartStopSound("SFX_EnterHomeBase");

        bool hasRested;
        if (CurrentHour == 4) hasRested = true;
        else hasRested = false;

        FindObjectOfType<HomeBaseSceneDisplay>().ClaimRewardButton.SetActive(hasRested);

        if (hasRested)
        {
            ManagerHandler.U_MAN.CreateFleetingInfoPopup("You have rested!\nShops refreshed!");
            NextHour(true);
            ShopItems = GetShopItems();
            ManagerHandler.CA_MAN.LoadNewRecruits();
            ManagerHandler.CA_MAN.LoadNewActions();

            List<Location> refreshedShops = new List<Location>();
            foreach (Location loc in ActiveLocations)
            {
                if (VisitedLocations.FindIndex(x => x == loc.LocationName) == -1) continue;

                if (loc.IsShop || loc.IsRecruitment || loc.IsActionShop)
                    refreshedShops.Add(loc);
            }

            foreach (Location loc in refreshedShops)
                VisitedLocations.Remove(loc.LocationName);
        }
    }

    /******
     * *****
     * ****** START_DIALOGUE
     * *****
     *****/
    public void StartDialogue()
    {
        ManagerHandler.AU_MAN.StartStopSound(null,
            CurrentLocation.LocationSoundscape, AudioManager.SoundType.Soundscape);
        ManagerHandler.D_MAN.StartDialogue();
    }

    /******
     * *****
     * ****** START/END_NARRATIVE
     * *****
     *****/
    public void StartNarrative()
    {
        ManagerHandler.AU_MAN.StartStopSound("Soundtrack_Narrative1", null,
            AudioManager.SoundType.Soundtrack);
        ManagerHandler.AU_MAN.StartStopSound(null, CurrentNarrative.NarrativeStartSound);
        ManagerHandler.AU_MAN.StartStopSound(null, CurrentNarrative.NarrativeSoundscape,
            AudioManager.SoundType.Soundscape);

        FindObjectOfType<NarrativeSceneDisplay>().CurrentNarrative = CurrentNarrative;
    }
    public void EndNarrative()
    {
        if (CurrentNarrative == settingNarrative) // TESTING
        {
            CurrentNarrative = newGameNarrative; // TESTING
            SceneLoader.LoadScene(SceneLoader.Scene.HeroSelectScene);
        }
        else Debug.LogError("NO CONDITIONS MATCHED!");
    }

    /******
     * *****
     * ****** START_COMBAT
     * *****
     *****/
    public void StartCombat()
    {
        ManagerHandler.U_MAN.StartCombatScene();
        ManagerHandler.CO_MAN.StartCombatScene();

        ManagerHandler.P_MAN.ResetForCombat();
        ManagerHandler.EN_MAN.ResetForCombat();

        PlayerHeroDisplay pHD = ManagerHandler.P_MAN.HeroObject.GetComponent<PlayerHeroDisplay>();
        EnemyHeroDisplay eHD = ManagerHandler.EN_MAN.HeroObject.GetComponent<EnemyHeroDisplay>();

        pHD.HeroBase.SetActive(false);
        pHD.HeroStats.SetActive(false);
        pHD.HeroNameObject.SetActive(false);

        eHD.HeroBase.SetActive(false);
        eHD.HeroStats.SetActive(false);
        eHD.HeroNameObject.SetActive(false);

        ManagerHandler.U_MAN.EndTurnButton.SetActive(false);
        ManagerHandler.U_MAN.CombatLog.SetActive(false);

        // ENEMY MANAGER
        EnemyHero enemyHero = ManagerHandler.D_MAN.EngagedHero as EnemyHero;
        if (enemyHero == null)
        {
            Debug.LogError("ENEMY HERO IS NULL!");
            return;
        }

        // ENEMY HERO
        ManagerHandler.EN_MAN.HeroScript = enemyHero;

        int enemyHealth;
        if (IsTutorial) enemyHealth = TUTORIAL_STARTING_HEALTH;
        else enemyHealth = ENEMY_STARTING_HEALTH;
        ManagerHandler.EN_MAN.CurrentHealth = enemyHealth;

        int energyPerTurn = START_ENERGY_PER_TURN;
        if ((ManagerHandler.EN_MAN.HeroScript as EnemyHero).IsBoss)
            energyPerTurn += BOSS_BONUS_ENERGY + ManagerHandler.CO_MAN.DifficultyLevel - 1;
        ManagerHandler.EN_MAN.EnergyPerTurn = energyPerTurn;
        ManagerHandler.EN_MAN.CurrentEnergy = 0;
        ManagerHandler.EN_MAN.DamageTakenTurn = 0;

        ManagerHandler.EN_MAN.TurnNumber = 0;

        // PLAYER MANAGER
        ManagerHandler.P_MAN.CurrentHealth = ManagerHandler.P_MAN.MaxHealth;
        ManagerHandler.P_MAN.EnergyPerTurn = START_ENERGY_PER_TURN;
        ManagerHandler.P_MAN.CurrentEnergy = 0;
        ManagerHandler.P_MAN.HeroUltimateProgress = 0;
        ManagerHandler.P_MAN.DamageTakenTurn = 0;
        foreach (HeroItem item in ManagerHandler.P_MAN.HeroItems) // TESTING
            item.IsUsed = false;

        ManagerHandler.P_MAN.TurnNumber = 0;

        // UPDATE DECKS
        ManagerHandler.CA_MAN.UpdateDeck(PLAYER);
        ManagerHandler.CA_MAN.UpdateDeck(ENEMY);

        // DISPLAY HEROES
        ManagerHandler.P_MAN.HeroObject.GetComponent<HeroDisplay>().HeroScript = ManagerHandler.P_MAN.HeroScript;
        ManagerHandler.EN_MAN.HeroObject.GetComponent<HeroDisplay>().HeroScript = ManagerHandler.EN_MAN.HeroScript;

        // SCHEDULE ACTIONS
        ManagerHandler.EV_MAN.NewDelayedAction(() => ManagerHandler.AN_MAN.CombatIntro(), 1);
        ManagerHandler.EV_MAN.NewDelayedAction(() => CombatStart(), 1);
        ManagerHandler.EV_MAN.NewDelayedAction(() => StartCombatTurn(ManagerHandler.P_MAN, true), 2);

        // AUDIO
        string soundtrack;
        if ((ManagerHandler.EN_MAN.HeroScript as EnemyHero).IsBoss) soundtrack = "Soundtrack_CombatBoss1";
        else soundtrack = "Soundtrack_Combat1";
        ManagerHandler.AU_MAN.StartStopSound(soundtrack, null, AudioManager.SoundType.Soundtrack);
        ManagerHandler.AU_MAN.StopCurrentSoundscape();
        FunctionTimer.Create(() => ManagerHandler.AU_MAN.StartStopSound("SFX_StartCombat1"), 0.15f);

        void CombatStart()
        {
            ManagerHandler.U_MAN.CombatLogEntry($"<b><color=\"green\">{ManagerHandler.P_MAN.HeroScript.HeroShortName}</color> VS <color=\"red\">{ManagerHandler.EN_MAN.HeroScript.HeroName}</b></color>");

            ManagerHandler.CA_MAN.ShuffleDeck(ManagerHandler.P_MAN, false);
            ManagerHandler.CA_MAN.ShuffleDeck(ManagerHandler.EN_MAN, false);

            for (int i = 0; i < START_HAND_SIZE; i++)
                ManagerHandler.EV_MAN.NewDelayedAction(() => AllDraw(), 0.5f);

            if (IsTutorial) // TUTORIAL!
            {
                ManagerHandler.EV_MAN.NewDelayedAction(() => ManagerHandler.U_MAN.CreateTutorialActionPopup(), 0);
                ManagerHandler.EV_MAN.NewDelayedAction(() => ManagerHandler.EV_MAN.PauseDelayedActions(true), 0);
                ManagerHandler.EV_MAN.NewDelayedAction(() => Tutorial_Tooltip(1), 0);
            }
            ResolveReputationEffects(1); // REPUTATION EFFECTS [RESOLVE ORDER = 1]
            PlayStartingUnits();
            ManagerHandler.EV_MAN.NewDelayedAction(() => Mulligan_Player(), 0.5f);
            ManagerHandler.EV_MAN.NewDelayedAction(() => ManagerHandler.EN_MAN.Mulligan(), 0.5f);
            ResolveReputationEffects(2); // REPUTATION EFFECTS [RESOLVE ORDER = 2]
            ResolveReputationEffects(3); // REPUTATION EFFECTS [RESOLVE ORDER = 3]

            string cognitiveMagnifier = "Cognitive Magnifier";
            if (ManagerHandler.P_MAN.GetAugment(cognitiveMagnifier))
            {
                ManagerHandler.EV_MAN.NewDelayedAction(() => CognitiveMagnifierEffect(), 0.25f);

                void CognitiveMagnifierEffect()
                {
                    ManagerHandler.AN_MAN.TriggerAugment(cognitiveMagnifier);

                    ManagerHandler.EF_MAN.StartEffectGroupList(new List<EffectGroup>
                    { cognitiveMagnifierEffect }, ManagerHandler.P_MAN.HeroObject);
                }
            }

            if (IsTutorial) ManagerHandler.EV_MAN.NewDelayedAction(() => Tutorial_Tooltip(2), 0); // TUTORIAL!

            void AllDraw()
            {
                ManagerHandler.CA_MAN.DrawCard(ManagerHandler.P_MAN);
                ManagerHandler.CA_MAN.DrawCard(ManagerHandler.EN_MAN);
            }

            void PlayStartingUnits()
            {
                List<UnitCard> startingUnits =
                    (ManagerHandler.EN_MAN.HeroScript as EnemyHero).Reinforcements[ManagerHandler.EN_MAN.ReinforcementGroup].StartingUnits;
                foreach (UnitCard card in startingUnits)
                {
                    UnitCard newCard = ManagerHandler.CA_MAN.NewCardInstance(card) as UnitCard;
                    ManagerHandler.EV_MAN.NewDelayedAction(() =>
                    ManagerHandler.EF_MAN.PlayCreatedUnit(newCard, false, new List<Effect>(), ManagerHandler.EN_MAN.HeroObject), 0.5f);
                }
            }
        }

        void Mulligan_Player() =>
            ManagerHandler.EF_MAN.StartEffectGroupList(new List<EffectGroup> { mulliganEffect }, ManagerHandler.P_MAN.HeroObject);
    }

    /******
     * *****
     * ****** END_COMBAT
     * *****
     *****/
    public void EndCombat(bool playerWins)
    {
        if (playerWins)
        {
            ManagerHandler.AU_MAN.StartStopSound(null, ManagerHandler.EN_MAN.HeroScript.HeroLose);
            FunctionTimer.Create(() =>
            ManagerHandler.AU_MAN.StartStopSound(null, ManagerHandler.P_MAN.HeroScript.HeroWin), 2f);
        }
        else
        {
            ManagerHandler.AU_MAN.StartStopSound(null, ManagerHandler.P_MAN.HeroScript.HeroLose);
            FunctionTimer.Create(() =>
            ManagerHandler.AU_MAN.StartStopSound(null, ManagerHandler.EN_MAN.HeroScript.HeroWin), 2f);
        }

        ManagerHandler.EV_MAN.ClearDelayedActions();
        ManagerHandler.U_MAN.PlayerIsTargetting = false;
        ManagerHandler.EF_MAN.EffectsResolving = false;
        ManagerHandler.P_MAN.IsMyTurn = false;

        foreach (HeroItem item in ManagerHandler.P_MAN.HeroItems) item.IsUsed = false;

        // Created Cards Played
        ManagerHandler.P_MAN.ResetForCombat();
        ManagerHandler.EN_MAN.ResetForCombat();

        FunctionTimer.Create(() => ManagerHandler.U_MAN.CreateCombatEndPopup(playerWins), 2f);
    }

    /******
     * *****
     * ****** START_COMBAT_TURN
     * *****
     *****/
    private void StartCombatTurn(HeroManager hero, bool isFirstTurn = false)
    {
        bool isPlayerTurn = hero == ManagerHandler.P_MAN;
        string logText = "\n";
        if (isPlayerTurn) logText += "[Your Turn]";
        else logText += "[Enemy Turn]";

        ManagerHandler.EV_MAN.NewDelayedAction(() => TurnPopup(), 0);
        ManagerHandler.EV_MAN.NewDelayedAction(() => ManagerHandler.U_MAN.CombatLogEntry(logText), 0);

        if (isPlayerTurn)
        {
            ManagerHandler.EV_MAN.NewDelayedAction(() => PlayerTurnStart(), 2);
            ManagerHandler.EV_MAN.NewDelayedAction(() => TurnDraw(), 0.5f);

            string synapticStabilizer = "Synaptic Stabilizer";

            if (isFirstTurn && ManagerHandler.P_MAN.GetAugment(synapticStabilizer))
            {
                ManagerHandler.EV_MAN.NewDelayedAction(() => SynapticStabilizerEffect(), 0.5f);
            }

            void SynapticStabilizerEffect()
            {
                ManagerHandler.P_MAN.CurrentEnergy++;
                ManagerHandler.AN_MAN.ModifyHeroEnergyState(1, ManagerHandler.P_MAN.HeroObject);
                ManagerHandler.AN_MAN.TriggerAugment(synapticStabilizer);
                ManagerHandler.CA_MAN.SelectPlayableCards();
            }

            void PlayerTurnStart()
            {
                ManagerHandler.P_MAN.IsMyTurn = true;
                ManagerHandler.EN_MAN.IsMyTurn = false;
                ManagerHandler.P_MAN.HeroPowerUsed = false;

                int startEnergy = ManagerHandler.P_MAN.CurrentEnergy;
                if (ManagerHandler.P_MAN.EnergyPerTurn < ManagerHandler.P_MAN.MaxEnergyPerTurn) ManagerHandler.P_MAN.EnergyPerTurn++;
                ManagerHandler.P_MAN.CurrentEnergy = ManagerHandler.P_MAN.EnergyPerTurn;

                int energyChange = ManagerHandler.P_MAN.CurrentEnergy - startEnergy;
                ManagerHandler.AN_MAN.ModifyHeroEnergyState(energyChange, ManagerHandler.P_MAN.HeroObject);

                ManagerHandler.P_MAN.TurnNumber++;

                // TUTORIAL!
                if (IsTutorial)
                {
                    switch (ManagerHandler.P_MAN.EnergyPerTurn)
                    {
                        case 2:
                            Tutorial_Tooltip(4);
                            break;
                    }
                }
            }

            void TurnDraw()
            {
                ManagerHandler.CA_MAN.DrawCard(ManagerHandler.P_MAN);
                ManagerHandler.CA_MAN.SelectPlayableCards();
            }
        }

        if (isPlayerTurn) ManagerHandler.EV_MAN.NewDelayedAction(() =>
        ManagerHandler.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_TURN_START, hero), 0);
        else
        {
            ManagerHandler.P_MAN.IsMyTurn = false;
            ManagerHandler.EN_MAN.IsMyTurn = true;
            ManagerHandler.EV_MAN.NewDelayedAction(() => ManagerHandler.EN_MAN.StartEnemyTurn(), 1);
        }

        void TurnPopup()
        {
            ManagerHandler.AU_MAN.StartStopSound("SFX_NextTurn");
            ManagerHandler.U_MAN.CreateTurnPopup(isPlayerTurn);
        }
    }

    /******
     * *****
     * ****** END_TURN
     * *****
     *****/
    public void EndCombatTurn(HeroManager hero)
    {
        ManagerHandler.EV_MAN.NewDelayedAction(() =>
        ManagerHandler.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_TURN_END, hero), 0);

        ManagerHandler.EV_MAN.NewDelayedAction(() => ManagerHandler.CO_MAN.RefreshAllUnits(), 0.5f);
        ManagerHandler.EV_MAN.NewDelayedAction(() => RemoveEffects(), 0);
        ManagerHandler.EV_MAN.NewDelayedAction(() => ResetTriggerCounts(), 0);

        if (hero == ManagerHandler.EN_MAN)
        {
            ManagerHandler.EV_MAN.NewDelayedAction(() => StartCombatTurn(ManagerHandler.P_MAN), 0.5f);
        }
        else if (hero == ManagerHandler.P_MAN)
        {
            if (IsTutorial) // TUTORIAL!
            {
                switch (ManagerHandler.P_MAN.EnergyPerTurn)
                {
                    case 1:
                        if (ManagerHandler.P_MAN.CurrentEnergy > 0) return;
                        else ManagerHandler.U_MAN.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
                        break;
                    case 2:
                        if (!ManagerHandler.P_MAN.HeroPowerUsed || ManagerHandler.EN_MAN.PlayZoneCards.Count > 0) return;
                        else ManagerHandler.U_MAN.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
                        break;
                }
            }

            ManagerHandler.P_MAN.IsMyTurn = false;
            ManagerHandler.CA_MAN.SelectPlayableCards(true);
            ManagerHandler.EV_MAN.NewDelayedAction(() => StartCombatTurn(ManagerHandler.EN_MAN), 0.5f);
        }
        else Debug.LogError("INVALID PLAYER!");

        void RemoveEffects()
        {
            ManagerHandler.EF_MAN.RemoveTemporaryEffects();
            ManagerHandler.EF_MAN.RemoveGiveNextEffects(hero);
            ManagerHandler.EF_MAN.RemoveChangeNextCostEffects(hero);
            ManagerHandler.EF_MAN.RemoveModifyNextEffects(hero);

            ManagerHandler.P_MAN.DamageTakenTurn = 0;
            ManagerHandler.EN_MAN.DamageTakenTurn = 0;
        }
        void ResetTriggerCounts()
        {
            foreach (GameObject unit in ManagerHandler.P_MAN.PlayZoneCards) ResetTrigger(unit);
            foreach (GameObject unit in ManagerHandler.EN_MAN.PlayZoneCards) ResetTrigger(unit);

            void ResetTrigger(GameObject unit)
            {
                UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
                foreach (CardAbility ca in ucd.CurrentAbilities)
                {
                    if (ca is TriggeredAbility tra)
                    {
                        tra.TriggerCount = 0;
                        ucd.EnableTriggerIcon(tra.AbilityTrigger, true);
                    }
                    else if (ca is ModifierAbility ma)
                    {
                        ma.TriggerCount = 0;
                        ucd.EnableTriggerIcon(null, true);
                    }
                }
            }
        }
    }

    /******
     * *****
     * ****** START_CREDITS
     * *****
     *****/
    public void StartCredits()
    {
        ManagerHandler.AU_MAN.StartStopSound("Soundtrack_Combat1",
            null, AudioManager.SoundType.Soundtrack);
        ManagerHandler.AU_MAN.StopCurrentSoundscape();
    }

    /******
     * *****
     * ****** GET_ACTIVE_NPC
     * *****
     *****/
    public NPCHero GetActiveNPC(NPCHero npc)
    {
        if (npc == null)
        {
            Debug.LogError("NPC IS NULL!");
            return null;
        }

        int activeNPC;
        activeNPC = ActiveNPCHeroes.FindIndex(x => x.HeroName == npc.HeroName);
        if (activeNPC != -1) return ActiveNPCHeroes[activeNPC];
        else
        {
            NPCHero newNPC;
            if (npc is EnemyHero) newNPC = ScriptableObject.CreateInstance<EnemyHero>();
            else newNPC = ScriptableObject.CreateInstance<NPCHero>();
            newNPC.LoadHero(npc);
            newNPC.NextDialogueClip = npc.FirstDialogueClip;
            if (newNPC.NextDialogueClip == null) Debug.LogError("NEXT CLIP IS NULL!");
            ActiveNPCHeroes.Add(newNPC);
            return newNPC;
        }
    }

    /******
     * *****
     * ****** GET_ACTIVE_LOCATION
     * *****
     *****/
    public Location GetActiveLocation(Location location, NPCHero newNPC = null)
    {
        if (location == null)
        {
            Debug.LogError("LOCATION IS NULL!");
            return null;
        }
        int activeLocation;
        activeLocation = ActiveLocations.FindIndex
            (x => x.LocationName == location.LocationName);

        if (activeLocation != -1)
        {
            Location loc = ActiveLocations[activeLocation];
            if (newNPC != null) loc.CurrentNPC = GetActiveNPC(newNPC);
            return loc;
        }
        else
        {
            Location newLoc = ScriptableObject.CreateInstance<Location>();
            newLoc.LoadLocation(location);
            newLoc.CurrentObjective = newLoc.FirstObjective;
            if (newNPC != null) newLoc.CurrentNPC = GetActiveNPC(newNPC);
            else newLoc.CurrentNPC = GetActiveNPC(location.FirstNPC);
            ActiveLocations.Add(newLoc);
            return newLoc;
        }
    }

    /******
     * *****
     * ****** GET_SHOP_ITEMS
     * *****
     *****/
    public List<HeroItem> GetShopItems()
    {
        HeroItem[] allItems = Resources.LoadAll<HeroItem>("Hero Items");

        // Rare Item Functionality
        // <Common> : <Rare> ::: <3> : <1>
        List<HeroItem> rarefiedItems = new List<HeroItem>();
        foreach (HeroItem item in allItems)
        {
            rarefiedItems.Add(item);
            if (!item.IsRareItem)
            {
                rarefiedItems.Add(item);
                rarefiedItems.Add(item);
            }
        }
        rarefiedItems.Shuffle();

        List<HeroItem> shopItems = new List<HeroItem>();
        foreach (HeroItem item in rarefiedItems)
        {
            if ((ManagerHandler.P_MAN.HeroItems.FindIndex(x => x.ItemName == item.ItemName) == -1) &&
                (shopItems.FindIndex(x => x.ItemName == item.ItemName) == -1)) shopItems.Add(item);

            if (shopItems.Count == 8) return shopItems;
        }
        return shopItems;
    }

    /******
     * *****
     * ****** GET_ITEM_COST
     * *****
     *****/
    public int GetItemCost(HeroItem item, out bool isDiscounted, bool isItemRemoval)
    {
        int itemCost;
        if (isItemRemoval)
        {
            if (item.IsRareItem) itemCost = SELL_RARE_ITEM_VALUE;
            else itemCost = SELL_ITEM_VALUE;
        }
        else
        {
            if (item.IsRareItem) itemCost = BUY_RARE_ITEM_COST;
            else itemCost = BUY_ITEM_COST;
        }

        if (ShopLoyalty == SHOP_LOYALTY_GOAL)
        {
            isDiscounted = true;
            itemCost -= BUY_ITEM_COST;
        }
        else isDiscounted = false;

        return itemCost;
    }

    /******
     * *****
     * ****** GET_RECRUIT_COST
     * *****
     *****/
    public int GetRecruitCost(UnitCard unitCard, out bool isDiscounted)
    {
        if (unitCard == null)
        {
            Debug.LogError("UNIT CARD IS NULL!");
            isDiscounted = false;
            return 0;
        }

        int recruitCost;
        switch (unitCard.CardRarity)
        {
            case Card.Rarity.Common:
                recruitCost = RECRUIT_COMMON_UNIT_COST;
                break;
            case Card.Rarity.Rare:
                recruitCost = RECRUIT_RARE_UNIT_COST;
                break;
            case Card.Rarity.Legend:
                recruitCost = RECRUIT_LEGEND_UNIT_COST;
                break;
            default:
                Debug.LogError("INVALID RARITY!");
                isDiscounted = false;
                return 0;
        }

        if (RecruitLoyalty == RECRUIT_LOYALTY_GOAL)
        {
            isDiscounted = true;
            recruitCost -= RECRUIT_COMMON_UNIT_COST;
        }
        else isDiscounted = false;
        return recruitCost;
    }

    /******
     * *****
     * ****** GET_ACTION_COST
     * *****
     *****/
    public int GetActionCost(ActionCard actionCard, out bool isDiscounted)
    {
        if (actionCard == null)
        {
            Debug.LogError("UNIT CARD IS NULL!");
            isDiscounted = false;
            return 0;
        }

        int recruitCost;
        switch (actionCard.CardRarity)
        {
            case Card.Rarity.Common:
                recruitCost = BUY_COMMON_ACTION_COST;
                break;
            case Card.Rarity.Rare:
                recruitCost = BUY_RARE_ACTION_COST;
                break;
            case Card.Rarity.Legend:
                recruitCost = BUY_LEGEND_ACTION_COST;
                break;
            default:
                Debug.LogError("INVALID RARITY!");
                isDiscounted = false;
                return 0;
        }

        if (ActionShopLoyalty == ACTION_LOYALTY_GOAL)
        {
            isDiscounted = true;
            recruitCost -= RECRUIT_COMMON_UNIT_COST;
        }
        else isDiscounted = false;
        return recruitCost;
    }

    /******
     * *****
     * ****** GET_CLONE_COST
     * *****
     *****/
    public int GetCloneCost(UnitCard unitCard)
    {
        int cloneCost;
        switch (unitCard.CardRarity)
        {
            case Card.Rarity.Common:
                cloneCost = CLONE_COMMON_UNIT_COST;
                break;
            case Card.Rarity.Rare:
                cloneCost = CLONE_RARE_UNIT_COST;
                break;
            case Card.Rarity.Legend:
                cloneCost = CLONE_LEGEND_UNIT_COST;
                break;
            default:
                Debug.LogError("INVALID RARITY!");
                return 0;
        }
        return cloneCost;
    }

    /******
     * *****
     * ****** GET_SELL_COST
     * *****
     *****/
    public int GetSellCost(Card card)
    {
        int sellCost;
        switch (card.CardRarity)
        {
            case Card.Rarity.Common:
                sellCost = SELL_COMMON_CARD_VALUE;
                break;
            case Card.Rarity.Rare:
                sellCost = SELL_RARE_CARD_VALUE;
                break;
            case Card.Rarity.Legend:
                sellCost = SELL_LEGEND_CARD_VALUE;
                break;
            default:
                Debug.LogError("INVALID RARITY!");
                return 0;
        }
        return sellCost;
    }
    #endregion
}
