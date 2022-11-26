using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField] [TextArea] private string[] loadingTips;
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

    private PlayerManager pMan;
    private EnemyManager enMan;
    private CardManager caMan;
    private CombatManager coMan;
    private UIManager uMan;
    private EventManager evMan;
    private EffectManager efMan;
    private AudioManager auMan;
    private DialogueManager dMan;
    private AnimationManager anMan;

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
    public const int MAXIMUM_ITEMS = 3;
    public const int HERO_ULTMATE_GOAL = 3;
    public const int PLAYER_START_AETHER = 15; // TESTING

    // Enemy
    public const string ENEMY = "Enemy";
    public const int ENEMY_STARTING_HEALTH = 30;
    //public const int ENEMY_STARTING_HEALTH = 1; // FOR TESTING ONLY
    // Tutorial Enemy
    public const int TUTORIAL_STARTING_HEALTH = 10;
    // Boss Enemy
    public const int BOSS_BONUS_ENERGY = 2;

    // Aether Rewards
    public const int IGNORE_CARD_AETHER = 10;
    // Sell Cards
    public const int SELL_COMMON_CARD_VALUE = 10;
    public const int SELL_RARE_CARD_VALUE = 15;
    public const int SELL_LEGEND_CARD_VALUE = 20;
    // Recruits
    public const int RECRUIT_COMMON_UNIT_COST = 30;
    public const int RECRUIT_RARE_UNIT_COST = 45;
    public const int RECRUIT_LEGEND_UNIT_COST = 60;
    public const int RECRUIT_LOYALTY_GOAL = 3;
    // Actions
    public const int BUY_COMMON_ACTION_COST = 30;
    public const int BUY_RARE_ACTION_COST = 45;
    public const int BUY_LEGEND_ACTION_COST = 60;
    public const int ACTION_LOYALTY_GOAL = 3;
    // Cloning
    public const int CLONE_COMMON_UNIT_COST = 35;
    public const int CLONE_RARE_UNIT_COST = 50;
    public const int CLONE_LEGEND_UNIT_COST = 65;
    // Items
    public const int BUY_ITEM_COST = 35;
    public const int BUY_RARE_ITEM_COST = 55;
    public const int SHOP_LOYALTY_GOAL = 3;
    // Sell Items
    public const int SELL_ITEM_VALUE = 15;
    public const int SELL_RARE_ITEM_VALUE = 20;
    // Reputation
    public const int REPUTATION_TIER_1 = 10;
    public const int REPUTATION_TIER_2 = 20;
    public const int REPUTATION_TIER_3 = 30;

    // Combat Reward
    public const int AETHER_COMBAT_REWARD_1 = 20;
    public const int AETHER_COMBAT_REWARD_2 = 25;
    public const int AETHER_COMBAT_REWARD_3 = 30;

    public const int AETHER_COMBAT_REWARD_BOSS_1 = 30;
    public const int AETHER_COMBAT_REWARD_BOSS_2 = 45;
    public const int AETHER_COMBAT_REWARD_BOSS_3 = 60;

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
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        caMan = CardManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        evMan = EventManager.Instance;
        efMan = EffectManager.Instance;
        auMan = AudioManager.Instance;
        dMan = DialogueManager.Instance;
        anMan = AnimationManager.Instance;
        currentTip = Random.Range(0, loadingTips.Length);
        ActiveNPCHeroes = new List<NPCHero>();
        ActiveLocations = new List<Location>();
        VisitedLocations = new List<string>();
        ShopItems = new List<HeroItem>();

        LoadPlayerPreferences();
        Debug.Log("Application Version: " + Application.version);
        SceneLoader.LoadScene(SceneLoader.Scene.TitleScene, false, false);
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
        pMan.AetherCells = 0;
        string playerHeroName = "Kili, Neon Rider";
        string enemyHeroName = "Tiny Mutant";
        Hero[] heroes = Resources.LoadAll<Hero>("Tutorial");
        EnemyHero enemyHero = ScriptableObject.CreateInstance<EnemyHero>();

        foreach (Hero hero in heroes)
        {
            if (hero.HeroName == playerHeroName) pMan.PlayerHero = hero as PlayerHero;
            else if (hero.HeroName == enemyHeroName)
            {
                enemyHero.LoadHero(hero);
                dMan.EngagedHero = enemyHero;
            }
        }

        foreach (UnitCard unit in caMan.TutorialPlayerUnits)
            for (int i = 0; i < 5; i++)
                caMan.AddCard(unit, PLAYER);
    }

    public void Tutorial_Tooltip(int tipNumber)
    {
        string tip;
        bool isCentered = true;
        switch (tipNumber)
        {
            case 1:
                tip = "Redraw any number of cards from your starting hand. Click each card you want to redraw, " +
                    "then click the <color=\"yellow\"><b>Confirm Button</b></color>.";
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
                tip = "<b>Attack the enemy hero to win!</b>\nRead more game rules in settings (top right).\n<b>End your turn to continue.</b>";
                break;
            default:
                Debug.LogError("INVALID TIP NUMBER!");
                return;
        }
        uMan.CreateInfoPopup(tip, UIManager.InfoPopupType.Tutorial, isCentered);
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
            {
                if (n.NarrativeName == narrative)
                    return n;
            }
            Debug.LogError("NARRATIVE + " + narrative + " NOT FOUND!");
            return null;
        }

        settingNarrative = LoadNarrative("Welcome to the Drift");
        newGameNarrative = LoadNarrative("Part 1: Stuck in Sylus");

        // Location
        //CurrentLocation = GetActiveLocation(LoadLocation("Sekherd and 7th"));
        CurrentNarrative = settingNarrative;

        Location[] allLocations = Resources.LoadAll<Location>("Locations");
        Location LoadLocation(string location)
        {
            foreach (Location l in allLocations)
            {
                if (l.LocationFullName == location)
                    return l;
            }
            Debug.LogError("NARRATIVE " + location + " NOT FOUND!");
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
        caMan.LoadNewRecruits();
        caMan.LoadNewActions();
        ShopItems = GetShopItems();
        RecruitLoyalty = 3; // First recruit free
        ActionShopLoyalty = 3; // First action free
        ShopLoyalty = 3; // First item free
        pMan.AetherCells = PLAYER_START_AETHER;
        pMan.PlayerDeckList.Clear();

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
        // Don't destroy pMan objects, they are assets not instances
        pMan.PlayerHero = null;
        pMan.PlayerDeckList.Clear();
        pMan.CurrentPlayerDeck.Clear();
        //pMan.AetherCells = 0;
        pMan.HeroAugments.Clear();
        pMan.HeroItems.Clear();
        // Enemy Manager
        Destroy(enMan.EnemyHero);
        enMan.EnemyHero = null;
        // Dialogue Manager
        dMan.EndDialogue();
        // Effect Manager
        foreach (Effect e in efMan.GiveNextEffects_Player) Destroy(e);
        efMan.GiveNextEffects_Player.Clear();
        foreach (Effect e in efMan.ChangeNextCostEffects_Player) Destroy(e);
        efMan.ChangeNextCostEffects_Player.Clear();
        foreach (Effect e in efMan.GiveNextEffects_Enemy) Destroy(e);
        efMan.GiveNextEffects_Enemy.Clear();
        foreach (Effect e in efMan.ChangeNextCostEffects_Enemy) Destroy(e);
        efMan.ChangeNextCostEffects_Enemy.Clear();
        // UI Manager
        uMan.ClearAugmentBar();
        uMan.ClearItemBar();
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
        PlayerPrefs.SetFloat(MUSIC_VOLUME, auMan.MusicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME, auMan.SFXVolume);

        int hideExplicit = 0;
        if (HideExplicitLanguage) hideExplicit = 1;
        PlayerPrefs.SetInt(HIDE_EXPLICIT_LANGUAGE, hideExplicit);
    }
    public void LoadPlayerPreferences()
    {
        auMan.MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1);
        auMan.SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1);

        bool hideExplicit = false;
        if (PlayerPrefs.GetInt(HIDE_EXPLICIT_LANGUAGE, 1) == 1) hideExplicit = true;
        HideExplicitLanguage = hideExplicit;
    }

    /******
     * *****
     * ****** SAVE_GAME
     * *****
     *****/
    public bool CheckSave()
    {
        GameData data = SaveLoad.LoadGame();
        if (data == null) return false;
        else return true;
    }
    public void SaveGame() // PUBLIC FOR BETA ONLY
    {
        string[] deckList = new string[pMan.PlayerDeckList.Count];
        for (int i = 0; i < deckList.Length; i++)
            deckList[i] = pMan.PlayerDeckList[i].CardName;

        string[] augments = new string[pMan.HeroAugments.Count];
        for (int i = 0; i < augments.Length; i++)
            augments[i] = pMan.HeroAugments[i].AugmentName;

        string[] items = new string[pMan.HeroItems.Count];
        for (int i = 0; i < items.Length; i++)
            items[i] = pMan.HeroItems[i].ItemName;

        string[,] npcsAndClips = new string[ActiveNPCHeroes.Count, 2];
        for (int i = 0; i < npcsAndClips.Length/2; i++)
        {
            npcsAndClips[i, 0] = ActiveNPCHeroes[i].HeroName;

            DialogueClip clip = ActiveNPCHeroes[i].NextDialogueClip;
            string clipName = clip.ToString();
            clipName = clipName.Replace(" (" + clip.GetType().Name + ")", "");
            npcsAndClips[i, 1] = clipName;
        }

        string[,] locationsNPCsObjectives = new string[ActiveLocations.Count, 3];
        for (int i = 0; i < locationsNPCsObjectives.Length/3; i++)
        {
            locationsNPCsObjectives[i, 0] = ActiveLocations[i].LocationName;
            locationsNPCsObjectives[i, 1] = ActiveLocations[i].CurrentNPC.HeroName;
            locationsNPCsObjectives[i, 2] = ActiveLocations[i].CurrentObjective;
        }

        string[] shopItems = new string[ShopItems.Count];
        for (int i = 0; i < shopItems.Length; i++)
            shopItems[i] = ShopItems[i].ItemName;

        string[] recruitUnits = new string[caMan.PlayerRecruitUnits.Count];
        for (int i = 0; i < recruitUnits.Length; i++)
            recruitUnits[i] = caMan.PlayerRecruitUnits[i].CardName;

        string[] shopActions = new string[caMan.ActionShopCards.Count];
        for (int i = 0; i < shopActions.Length; i++)
            shopActions[i] = caMan.ActionShopCards[i].CardName;

        string narrativeName = "";
        if (CurrentNarrative != null)
        {
            narrativeName = CurrentNarrative.ToString();
            narrativeName = narrativeName.Replace(" (Narrative)", "");
        }

        GameData data = new GameData(CurrentHour, narrativeName, pMan.PlayerHero.HeroName,
            deckList, augments, items, pMan.AetherCells,
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
            CurrentNarrative = Resources.Load<Narrative>("Narratives/" + data.CurrentNarrative);
            if (CurrentNarrative == null) Debug.LogError("NARRATIVE " + data.CurrentNarrative + " NOT FOUND!");
        }
        else CurrentNarrative = null;

        // PLAYER HERO
        PlayerHero pHero;
        pHero = Resources.Load<PlayerHero>("Heroes/Player Heroes/" + data.PlayerHero);
        if (pHero == null) Debug.LogError("HERO " + data.PlayerHero + " NOT FOUND!");
        else pMan.PlayerHero = pHero;

        // DECK LIST
        pMan.PlayerDeckList.Clear();
        for (int i = 0; i < data.PlayerDeck.Length; i++)
        {
            Card card;
            card = Resources.Load<Card>("Cards_Starting/" + data.PlayerDeck[i]);
            if (card == null) card = Resources.Load<Card>("Cards_Units/" + data.PlayerDeck[i]);
            if (card == null) card = Resources.Load<Card>("Cards_Actions/" + data.PlayerDeck[i]);
            if (card == null) Debug.LogError("CARD " + data.PlayerDeck[i] + " NOT FOUND!");
            else caMan.AddCard(card, PLAYER);
        }

        // AUGMENTS
        pMan.HeroAugments.Clear();
        for (int i = 0; i < data.PlayerAugments.Length; i++)
        {
            HeroAugment augment;
            augment = Resources.Load<HeroAugment>("Hero Augments/" + data.PlayerAugments[i]);
            if (augment == null) Debug.LogError("AUGMENT " + data.PlayerAugments[i] + " NOT FOUND!");
            else pMan.HeroAugments.Add(augment);
        }

        // ITEMS
        pMan.HeroItems.Clear();
        for (int i = 0; i < data.PlayerItems.Length; i++)
        {
            HeroItem item;
            item = Resources.Load<HeroItem>("Hero Items/" + data.PlayerItems[i]);
            if (item == null) Debug.LogError("ITEM " + data.PlayerItems[i] + " NOT FOUND!");
            else pMan.HeroItems.Add(item);
        }

        // AETHER CELLS
        pMan.AetherCells = data.AetherCells;

        // NPCS
        ActiveNPCHeroes.Clear();
        for (int i = 0; i < data.NPCSAndClips.Length/2; i++)
        {
            NPCHero npc;
            npc = Resources.Load<NPCHero>("Heroes/NPC Heroes/" + data.NPCSAndClips[i, 0]);
            if (npc == null) Debug.LogError("NPC " + data.NPCSAndClips[i, 0] + " NOT FOUND!");
            else
            {
                npc = GetActiveNPC(npc);
                DialogueClip clip;
                clip = Resources.Load<DialogueClip>("Dialogue/" + npc.HeroName + "/" + data.NPCSAndClips[i, 1]);
                if (clip == null) Debug.LogError("CLIP " + data.NPCSAndClips[i, 1] + " FOR " + npc.HeroName + " NOT FOUND!");
                else npc.NextDialogueClip = clip;
            }
        }

        // LOCATIONS
        ActiveLocations.Clear();
        for (int i = 0; i < data.LocationsNPCsObjectives.Length/3; i++)
        {
            Location loc;
            string name = data.LocationsNPCsObjectives[i, 0];
            loc = Resources.Load<Location>("Random Encounters/" + name);
            if (loc == null) loc = Resources.Load<Location>("Locations/" + name);
            if (loc == null) Debug.LogError("LOCATION " + name + " NOT FOUND!");
            else
            {
                loc = GetActiveLocation(loc);

                // null checks
                loc.CurrentNPC = GetActiveNPC(Resources.Load<NPCHero>("Heroes/NPC Heroes/" + data.LocationsNPCsObjectives[i, 1]));
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
        caMan.PlayerRecruitUnits.Clear();
        for (int i = 0; i < data.RecruitUnits.Length; i++)
            caMan.PlayerRecruitUnits.Add(Resources.Load<UnitCard>("Cards_Units/" + data.RecruitUnits[i]));

        // SHOP ACTIONS
        caMan.ActionShopCards.Clear();
        for (int i = 0; i < data.ShopActions.Length; i++)
            caMan.ActionShopCards.Add(Resources.Load<ActionCard>("Cards_Actions/" + data.ShopActions[i]));

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
        auMan.StartStopSound("Soundtrack_TitleScene", null, AudioManager.SoundType.Soundtrack);
        auMan.StartStopSound("Soundscape_TitleScene", null, AudioManager.SoundType.Soundscape);
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
        auMan.StopCurrentSoundscape();
        FindObjectOfType<HeroSelectSceneDisplay>().DisplaySelectedHero();
    }

    /******
     * *****
     * ****** ENTER_WORLD_MAP
     * *****
     *****/
    public void EnterWorldMap()
    {
        auMan.StartStopSound("Soundtrack_WorldMapScene", null, AudioManager.SoundType.Soundtrack);
        auMan.StartStopSound("SFX_EnterWorldMap");

        foreach (Location loc in ActiveLocations)
        {
            GameObject location = Instantiate(locationIconPrefab,
                uMan.CurrentCanvas.transform);
            LocationIcon icon = location.GetComponent<LocationIcon>();
            icon.Location = loc;
        }

        SaveGame();
        if (CurrentNarrative != null)
        {
            if (CurrentNarrative.IsGameEnd) uMan.CreateGameEndPopup();
            else uMan.CreateNarrativePopup(CurrentNarrative);
            CurrentNarrative = null;
        }

        FindObjectOfType<TimeClockDisplay>().SetClockValues(CurrentHour, IsNewHour);
        if (IsNewHour) IsNewHour = false;
    }

    public enum DifficultyLevel
    {
        Standard_1,
        Standard_2,
        Standard_3,

        Boss_1,
        Boss_2,
        Boss_3
    }
    public int GetAetherReward(DifficultyLevel difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case DifficultyLevel.Standard_1:
                return AETHER_COMBAT_REWARD_1;
            case DifficultyLevel.Standard_2:
                return AETHER_COMBAT_REWARD_2;
            case DifficultyLevel.Standard_3:
                return AETHER_COMBAT_REWARD_3;
            case DifficultyLevel.Boss_1:
                return AETHER_COMBAT_REWARD_BOSS_1;
            case DifficultyLevel.Boss_2:
                return AETHER_COMBAT_REWARD_BOSS_2;
            case DifficultyLevel.Boss_3:
                return AETHER_COMBAT_REWARD_BOSS_3;
        }

        Debug.LogError("INVALID DIFFICULTY!");
        return 0;
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
        evMan.NewDelayedAction(() => uMan.SetReputation(repType, repChange), 0.5f);
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

                evMan.NewDelayedAction(() => ResolveEffects(repType, effects, showTrigger), delay);
            }
        }

        void ResolveEffects(ReputationType repType, List<EffectGroup> repEffects, bool showTrigger)
        {
            if (showTrigger) uMan.SetReputation(repType, 0, true);

            if (repEffects != null && repEffects.Count > 0)
                efMan.StartEffectGroupList(repEffects, coMan.PlayerHero);
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

        IsNewHour = true; // TESTING
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
                Debug.LogError("INVALID HOUR! <" + CurrentHour + ">");
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
        auMan.StopCurrentSoundscape();
        auMan.StartStopSound("SFX_EnterHomeBase");

        bool hasRested;
        if (CurrentHour == 4) hasRested = true;
        else hasRested = false;

        FindObjectOfType<HomeBaseSceneDisplay>().ClaimRewardButton.SetActive(hasRested);

        if (hasRested)
        {
            uMan.CreateFleetingInfoPopup("You have rested!\nShops refreshed!");
            NextHour(true);
            ShopItems = GetShopItems();
            caMan.LoadNewRecruits();
            caMan.LoadNewActions();

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
        auMan.StartStopSound(null,
            CurrentLocation.LocationSoundscape, AudioManager.SoundType.Soundscape);
        dMan.StartDialogue();
    }

    /******
     * *****
     * ****** START/END_NARRATIVE
     * *****
     *****/
    public void StartNarrative()
    {
        auMan.StartStopSound("Soundtrack_Narrative1",
            null, AudioManager.SoundType.Soundtrack);
        auMan.StartStopSound(null, CurrentNarrative.NarrativeStartSound);
        auMan.StartStopSound(null,
            CurrentNarrative.NarrativeSoundscape, AudioManager.SoundType.Soundscape);
        NarrativeSceneDisplay nsd = FindObjectOfType<NarrativeSceneDisplay>();
        nsd.CurrentNarrative = CurrentNarrative;
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
        uMan.StartCombatScene();
        coMan.StartCombatScene();

        PlayerHeroDisplay pHD = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>();
        EnemyHeroDisplay eHD = coMan.EnemyHero.GetComponent<EnemyHeroDisplay>();
        uMan.EndTurnButton.SetActive(false);
        pHD.HeroBase.SetActive(false);
        pHD.HeroStats.SetActive(false);
        pHD.HeroNameObject.SetActive(false);
        eHD.HeroBase.SetActive(false);
        eHD.HeroStats.SetActive(false);
        eHD.HeroNameObject.SetActive(false);
        uMan.CombatLog.SetActive(false);

        // EFFECT MANAGER
        foreach (Effect e in efMan.GiveNextEffects_Player) Destroy(e);
        efMan.GiveNextEffects_Player.Clear();
        foreach (Effect e in efMan.ChangeNextCostEffects_Player) Destroy(e);
        efMan.ChangeNextCostEffects_Player.Clear();
        foreach (Effect e in efMan.GiveNextEffects_Enemy) Destroy(e);
        efMan.GiveNextEffects_Enemy.Clear();
        foreach (Effect e in efMan.ChangeNextCostEffects_Enemy) Destroy(e);
        efMan.ChangeNextCostEffects_Enemy.Clear();

        // CREATED CARDS
        coMan.ExploitsPlayed_Player = 0;
        coMan.InventionsPlayed_Player = 0;
        coMan.SchemesPlayed_Player = 0;
        coMan.ExtractionsPlayed_Player = 0;

        coMan.ExploitsPlayed_Enemy = 0;
        coMan.InventionsPlayed_Enemy = 0;
        coMan.SchemesPlayed_Enemy = 0;
        coMan.ExtractionsPlayed_Enemy = 0;

        // ENEMY MANAGER
        enMan.StartCombat();
        EnemyHero enemyHero = dMan.EngagedHero as EnemyHero;
        if (enemyHero == null)
        {
            Debug.LogError("ENEMY HERO IS NULL!");
            return;
        }

        // ENEMY HERO
        enMan.EnemyHero = enemyHero;

        int enemyHealth;
        if (IsTutorial) enemyHealth = TUTORIAL_STARTING_HEALTH;
        else enemyHealth = ENEMY_STARTING_HEALTH;
        enMan.EnemyHealth = enemyHealth;

        int energyPerTurn = START_ENERGY_PER_TURN;
        if (enMan.EnemyHero.IsBoss) energyPerTurn += BOSS_BONUS_ENERGY;
        enMan.EnergyPerTurn = energyPerTurn;
        enMan.CurrentEnergy = 0;
        enMan.DamageTaken_Turn = 0;

        // PLAYER MANAGER
        pMan.PlayerHealth = pMan.MaxPlayerHealth;
        pMan.EnergyPerTurn = START_ENERGY_PER_TURN;
        pMan.CurrentEnergy = 0;
        pMan.HeroUltimateProgress = 0;
        pMan.DamageTaken_Turn = 0;
        foreach (HeroItem item in pMan.HeroItems) // TESTING
            item.IsUsed = false;

        // UPDATE DECKS
        caMan.UpdateDeck(PLAYER);
        caMan.UpdateDeck(ENEMY);

        // DISPLAY HEROES
        coMan.PlayerHero.GetComponent<HeroDisplay>().HeroScript = pMan.PlayerHero;
        coMan.EnemyHero.GetComponent<HeroDisplay>().HeroScript = enMan.EnemyHero;

        // SCHEDULE ACTIONS
        evMan.NewDelayedAction(() => anMan.CombatIntro(), 1);
        evMan.NewDelayedAction(() => CombatStart(), 1);
        evMan.NewDelayedAction(() => StartCombatTurn(PLAYER, true), 2);

        // AUDIO
        string soundtrack;
        if (enMan.EnemyHero.IsBoss) soundtrack = "Soundtrack_CombatBoss1";
        else soundtrack = "Soundtrack_Combat1";
        auMan.StartStopSound(soundtrack, null, AudioManager.SoundType.Soundtrack);
        auMan.StopCurrentSoundscape();
        FunctionTimer.Create(() => auMan.StartStopSound("SFX_StartCombat1"), 0.15f); // TESTING

        void CombatStart()
        {
            uMan.CombatLogEntry("<b><color=\"green\">" + pMan.PlayerHero.HeroShortName +
                "</color> VS <color=\"red\">" + enMan.EnemyHero.HeroName + "</b></color>");

            caMan.ShuffleDeck(PLAYER, false);
            caMan.ShuffleDeck(ENEMY, false);

            for (int i = 0; i < START_HAND_SIZE; i++)
                evMan.NewDelayedAction(() => AllDraw(), 0.5f);

            if (IsTutorial) // TUTORIAL!
            {
                evMan.NewDelayedAction(() => uMan.CreateTutorialActionPopup(), 0);
                evMan.NewDelayedAction(() => evMan.PauseDelayedActions(true), 0);
                evMan.NewDelayedAction(() => Tutorial_Tooltip(1), 0);
            }
            ResolveReputationEffects(1); // REPUTATION EFFECTS [RESOLVE ORDER = 1]
            PlayStartingUnits();
            evMan.NewDelayedAction(() => Mulligan_Player(), 0.5f);
            evMan.NewDelayedAction(() => enMan.Mulligan(), 0.5f);
            ResolveReputationEffects(2); // REPUTATION EFFECTS [RESOLVE ORDER = 2]
            ResolveReputationEffects(3); // REPUTATION EFFECTS [RESOLVE ORDER = 3]

            string cognitiveMagnifier = "Cognitive Magnifier";
            if (pMan.GetAugment(cognitiveMagnifier))
            {
                evMan.NewDelayedAction(() => CognitiveMagnifierEffect(), 0.25f);

                void CognitiveMagnifierEffect()
                {
                    anMan.TriggerAugment(cognitiveMagnifier);

                    efMan.StartEffectGroupList(new List<EffectGroup>
                    { cognitiveMagnifierEffect }, coMan.PlayerHero);
                }
            }

            if (IsTutorial) evMan.NewDelayedAction(() => Tutorial_Tooltip(2), 0); // TUTORIAL!

            void AllDraw()
            {
                coMan.DrawCard(PLAYER);
                coMan.DrawCard(ENEMY);
            }

            void PlayStartingUnits()
            {
                List<UnitCard> startingUnits =
                    enMan.EnemyHero.Reinforcements[enMan.ReinforcementGroup].StartingUnits;
                foreach (UnitCard card in startingUnits)
                {
                    UnitCard newCard = caMan.NewCardInstance(card) as UnitCard;
                    evMan.NewDelayedAction(() =>
                    efMan.PlayCreatedUnit(newCard, false, new List<Effect>(), coMan.EnemyHero), 0.5f);
                }
            }
        }

        void Mulligan_Player() =>
            efMan.StartEffectGroupList(new List<EffectGroup> { mulliganEffect }, gameObject);
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
            auMan.StartStopSound(null, enMan.EnemyHero.HeroLose);
            FunctionTimer.Create(() =>
            auMan.StartStopSound(null, pMan.PlayerHero.HeroWin), 2f);
        }
        else
        {
            auMan.StartStopSound(null, pMan.PlayerHero.HeroLose);
            FunctionTimer.Create(() =>
            auMan.StartStopSound(null, enMan.EnemyHero.HeroWin), 2f);
        }

        evMan.ClearDelayedActions();
        uMan.PlayerIsTargetting = false;
        efMan.EffectsResolving = false;
        pMan.IsMyTurn = false;

        foreach (HeroItem item in pMan.HeroItems) // TESTING
            item.IsUsed = false;

        // Created Cards Played
        coMan.ExploitsPlayed_Player = 0;
        coMan.ExploitsPlayed_Enemy = 0;
        coMan.InventionsPlayed_Player = 0;
        coMan.InventionsPlayed_Enemy = 0;
        coMan.SchemesPlayed_Player = 0;
        coMan.SchemesPlayed_Enemy = 0;
        coMan.ExtractionsPlayed_Player = 0;
        coMan.ExtractionsPlayed_Enemy = 0;

        FunctionTimer.Create(() => uMan.CreateCombatEndPopup(playerWins), 2f);

        //efMan.GiveNextEffects.Clear();
        //efMan.ChangeNextCostEffects.Clear();
    }

    /******
     * *****
     * ****** START_COMBAT_TURN
     * *****
     *****/
    private void StartCombatTurn(string player, bool isFirstTurn = false)
    {
        bool isPlayerTurn;
        if (player == PLAYER) isPlayerTurn = true;
        else if (player == ENEMY) isPlayerTurn = false;
        else
        {
            Debug.LogError("PLAYER NOT FOUND!");
            return;
        }

        evMan.NewDelayedAction(() => TurnPopup(), 0);

        string logText = "\n";
        if (isPlayerTurn) logText += "[Your Turn]";
        else logText += "[Enemy Turn]";
        evMan.NewDelayedAction(() => uMan.CombatLogEntry(logText), 0);

        if (isPlayerTurn)
        {
            evMan.NewDelayedAction(() => PlayerTurnStart(), 2);
            evMan.NewDelayedAction(() => TurnDraw(), 0.5f);

            string synapticStabilizer = "Synaptic Stabilizer";

            if (isFirstTurn && pMan.GetAugment(synapticStabilizer))
            {
                evMan.NewDelayedAction(() => SynapticStabilizerEffect(), 0.5f);
            }

            void SynapticStabilizerEffect()
            {
                pMan.CurrentEnergy++;
                anMan.ModifyHeroEnergyState(1, coMan.PlayerHero);
                anMan.TriggerAugment(synapticStabilizer);
                coMan.SelectPlayableCards();
            }

            void PlayerTurnStart()
            {
                pMan.IsMyTurn = true;
                enMan.IsMyTurn = false;
                pMan.HeroPowerUsed = false;

                int startEnergy = pMan.CurrentEnergy;
                if (pMan.EnergyPerTurn < pMan.MaxEnergyPerTurn) pMan.EnergyPerTurn++;
                pMan.CurrentEnergy = pMan.EnergyPerTurn;

                int energyChange = pMan.CurrentEnergy - startEnergy;
                anMan.ModifyHeroEnergyState(energyChange, coMan.PlayerHero);

                // TUTORIAL!
                if (IsTutorial)
                {
                    switch (pMan.EnergyPerTurn)
                    {
                        case 2:
                            Tutorial_Tooltip(4);
                            break;
                    }
                }
            }

            void TurnDraw()
            {
                coMan.DrawCard(PLAYER);
                coMan.SelectPlayableCards();
            }
        }

        if (isPlayerTurn) evMan.NewDelayedAction(() =>
        caMan.TriggerPlayedUnits(CardManager.TRIGGER_TURN_START, player), 0);
        else
        {
            pMan.IsMyTurn = false;
            enMan.IsMyTurn = true;
            evMan.NewDelayedAction(() => enMan.StartEnemyTurn(), 1);
        }

        void TurnPopup()
        {
            auMan.StartStopSound("SFX_NextTurn");
            uMan.CreateTurnPopup(isPlayerTurn);
        }
    }

    /******
     * *****
     * ****** END_TURN
     * *****
     *****/
    public void EndCombatTurn(string player)
    {
        evMan.NewDelayedAction(() =>
        caMan.TriggerPlayedUnits(CardManager.TRIGGER_TURN_END, player), 0);

        evMan.NewDelayedAction(() => RefreshAllUnits(), 0.5f);
        evMan.NewDelayedAction(() => RemoveEffects(), 0);
        evMan.NewDelayedAction(() => ResetTriggerCounts(), 0);

        if (player == ENEMY)
        {
            evMan.NewDelayedAction(() => StartCombatTurn(PLAYER), 0.5f);
        }
        else if (player == PLAYER)
        {
            if (IsTutorial) // TUTORIAL!
            {
                switch (pMan.EnergyPerTurn)
                {
                    case 1:
                        if (pMan.CurrentEnergy > 0) return;
                        else uMan.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
                        break;
                    case 2:
                        if (!pMan.HeroPowerUsed || coMan.EnemyZoneCards.Count > 0) return;
                        else uMan.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
                        break;
                }
            }

            pMan.IsMyTurn = false;
            coMan.SelectPlayableCards(true);
            evMan.NewDelayedAction(() => StartCombatTurn(ENEMY), 0.5f);
        }

        void RefreshAllUnits()
        {
            coMan.RefreshUnits(PLAYER);
            coMan.RefreshUnits(ENEMY);
        }
        void RemoveEffects()
        {
            efMan.RemoveTemporaryEffects(PLAYER);
            efMan.RemoveTemporaryEffects(ENEMY);
            efMan.RemoveGiveNextEffects(player);
            efMan.RemoveChangeNextCostEffects(player);

            pMan.DamageTaken_Turn = 0;
            enMan.DamageTaken_Turn = 0;
        }
        void ResetTriggerCounts()
        {
            foreach (GameObject unit in coMan.PlayerZoneCards) ResetTrigger(unit);
            foreach (GameObject unit in coMan.EnemyZoneCards) ResetTrigger(unit);

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
        auMan.StartStopSound("Soundtrack_Combat1",
            null, AudioManager.SoundType.Soundtrack);
        auMan.StopCurrentSoundscape();
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
            Debug.Log("NEW NPC CREATED! <" + npc.HeroName + ">");
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
            Debug.Log("LOCATION FOUND! <" + loc.LocationFullName + ">");
            if (newNPC != null) loc.CurrentNPC = GetActiveNPC(newNPC);
            Debug.Log("ACTIVE LOCATIONS: <" + ActiveLocations.Count + ">");
            return loc;
        }
        else
        {
            Location newLoc = ScriptableObject.CreateInstance<Location>();
            newLoc.LoadLocation(location);
            newLoc.CurrentObjective = newLoc.FirstObjective;
            Debug.Log("NEW LOCATION CREATED! <" + newLoc.LocationFullName + ">");
            if (newNPC != null) newLoc.CurrentNPC = GetActiveNPC(newNPC);
            else newLoc.CurrentNPC = GetActiveNPC(location.FirstNPC);
            ActiveLocations.Add(newLoc);
            Debug.Log("ACTIVE LOCATIONS: <" + ActiveLocations.Count + ">");
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
            if ((pMan.HeroItems.FindIndex(x => x.ItemName == item.ItemName) == -1) &&
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
