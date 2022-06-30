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

    [Header("LOCATION BACKGROUNDS")]
    [SerializeField] private Sprite locationBG_City;
    [SerializeField] private Sprite locationBG_Wasteland;
    [Header("LOCATION ICON")]
    [SerializeField] GameObject locationIconPrefab;
    [Header("MULLIGAN EFFECT")]
    [SerializeField] private EffectGroup mulliganEffect;
    [Header("LOADING TIPS")]
    [SerializeField] [TextArea] private string[] loadingTips;
    [Header("REPUTATION BONUSES")]
    [SerializeField] private ReputationBonuses reputation_Mages;
    [SerializeField] private ReputationBonuses reputation_Mutants;
    [SerializeField] private ReputationBonuses reputation_Rogues;
    [SerializeField] private ReputationBonuses reputation_Techs;
    [SerializeField] private ReputationBonuses reputation_Warriors;

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
    private Location homeBaseLocation;
    private Location firstLocation;

    private int currentTip;
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
    public int ShopLoyalty { get; set; }
    public int RecruitLoyalty { get; set; }

    // REPUTATION
    public int Reputation_Mages { get; set; }
    public int Reputation_Mutants { get; set; }
    public int Reputation_Rogues { get; set; }
    public int Reputation_Techs { get; set; }
    public int Reputation_Warriors { get; set; }

    /* GAME_MANAGER_DATA */
    // Player Preferences
    public const string MUSIC_VOLUME = "MusicVolume";
    public const string SFX_VOLUME = "SFXVolume";
    public const string HIDE_EXPLICIT_LANGUAGE = "HideExplicitLanguage";

    // Universal
    public const int WOUNDED_VALUE = 15; // TESTING
    public const int START_HAND_SIZE = 4;
    public const int MAX_HAND_SIZE = 10; // TESTING
    public const int MAX_UNITS_PLAYED = 6;

    // Player
    public const string PLAYER = "Player";
    public const int MINIMUM_MAIN_DECK_SIZE = 15;
    public const int PLAYER_STARTING_HEALTH = 30;
    public const int PLAYER_START_UNITS = 3;
    public const int START_ENERGY_PER_TURN = 1;
    public const int MAXIMUM_ENERGY_PER_TURN = 5;
    public const int MAXIMUM_ENERGY = 10;
    public const int HERO_ULTMATE_GOAL = 3;
    public const int PLAYER_START_AETHER = 0;
    
    // Enemy
    public const string ENEMY = "Enemy";
    public const int ENEMY_STARTING_HEALTH = 30;
    //public const int ENEMY_STARTING_HEALTH = 15; // FOR TESTING ONLY
    public const int ENEMY_HAND_SIZE = 0;

    public const int BOSS_BONUS_HEALTH = 5;
    public const int BOSS_BONUS_ENERGY = 2;

    // Aether Rewards
    public const int IGNORE_CARD_AETHER = 2;
    // Card Removal
    public const int REMOVE_CARD_COST = 2;
    public const int REMOVE_RARE_CARD_COST = 4;
    // Recruits
    public const int RECRUIT_UNIT_COST = 3;
    public const int RECRUIT_RARE_UNIT_COST = 5;
    public const int RECRUIT_LOYALTY_GOAL = 3;
    // Cloning
    public const int CLONE_UNIT_COST = 3;
    public const int CLONE_RARE_UNIT_COST = 5;
    // Items
    public const int BUY_ITEM_COST = 2;
    public const int BUY_RARE_ITEM_COST = 3;
    public const int SHOP_LOYALTY_GOAL = 3;
    // Reputation
    public const int REPUTATION_TIER_1 = 10;
    public const int REPUTATION_TIER_2 = 20;
    public const int REPUTATION_TIER_3 = 30;

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
        SceneLoader.LoadScene(SceneLoader.Scene.TitleScene, false, false); // TESTING
    }

    /******
     * *****
     * ****** LOAD_NARRATIVE
     * *****
     *****/
    private Narrative LoadNarrative(string narrativeName)
    {
        Narrative[] allNarratives = Resources.LoadAll<Narrative>("Narratives");
        foreach(Narrative narrative in allNarratives)
        {
            if (narrative.NarrativeName == narrativeName)
                return narrative;
        }
        Debug.LogError("NARRATIVE " + narrativeName + " NOT FOUND!");
        return null;
    }

    /******
     * *****
     * ****** LOAD_LOCATION
     * *****
     *****/
    private Location LoadLocation(string locationName)
    {
        Location[] allLocations = Resources.LoadAll<Location>("Locations");
        foreach (Location location in allLocations)
        {
            if (location.LocationFullName == locationName)
                return location;
        }
        Debug.LogError("NARRATIVE " + locationName + " NOT FOUND!");
        return null;
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
            Debug.LogError("CURRENT LOCATION IS NULL!");
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
        string enemyHeroName = "Tutorial Enemy Hero";
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

        foreach (SkillCard skill in pMan.PlayerHero.HeroSkills)
            caMan.AddCard(skill, PLAYER);
        foreach (UnitCard unit in caMan.TutorialPlayerUnits)
            for (int i = 0; i < 5; i++)
                caMan.AddCard(unit, PLAYER);
    }

    public void Tutorial_Tooltip(int tipNumber)
    {
        string tip;
        bool isCentered = false;
        switch (tipNumber)
        {
            case 1:
                tip = "Replace any number of cards (0 or more) from your starting hand. Click each card you want to replace, then click the confirm button.";
                isCentered = true;
                break;
            case 2:
                tip = "Play a card from your hand by dragging it into play. Cards you can play are highlighted in <color=\"green\">green<color=\"yellow\">.";
                break;
            case 3:
                tip = "End your turn by clicking the <b>end turn button</b> or pressing the <b>space bar</b>.";
                isCentered = true;
                break;
            case 4:
                tip = "Use your hero power by clicking on the icon next to your hero.";
                isCentered = true;
                break;
            case 5:
                tip = "Attack an enemy unit by dragging your ally to them.";
                break;
            case 6:
                tip = "<i>Attack the enemy hero to win!</i>\nRead more game rules in settings (top right), and " +
                    "right click on cards for more information.\n<i>End your turn to continue.</i>";
                isCentered = true;
                break;
            default:
                Debug.LogError("INVALID TIP NUMBER!");
                return;
        }
        tip = "<color=\"yellow\"><b>[Tutorial]</b>\n" + tip + "</color>";
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
        settingNarrative = LoadNarrative("Welcome to the Drift");
        newGameNarrative = LoadNarrative("Part 1: Stuck in Sylus");
        homeBaseLocation = LoadLocation("Your Ship");
        firstLocation = LoadLocation("Sekherd and 7th");

        // Hour
        CurrentHour = 1;
        IsNewHour = true;

        // Location
        CurrentLocation = GetActiveLocation(firstLocation);
        CurrentNarrative = settingNarrative;
        GetActiveLocation(homeBaseLocation);

        IsCombatTest = false;
        IsTutorial = false;
        caMan.LoadNewRecruits();
        ShopItems = GetShopItems();
        ShopLoyalty = 0;
        RecruitLoyalty = 0;
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
        foreach (NPCHero npc in ActiveNPCHeroes) Destroy(npc);
        ActiveNPCHeroes.Clear();
        foreach (Location loc in ActiveLocations) Destroy(loc);
        ActiveLocations.Clear();
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
            npcsAndClips[i, 1] = ActiveNPCHeroes[i].NextDialogueClip.ToString();
        }

        string[,] locationsNPCsObjectives = new string[ActiveLocations.Count, 3];
        for (int i = 0; i < locationsNPCsObjectives.Length/3; i++)
        {
            locationsNPCsObjectives[i, 0] = ActiveLocations[i].LocationFullName;
            locationsNPCsObjectives[i, 1] = ActiveLocations[i].CurrentNPC.HeroName;
            locationsNPCsObjectives[i, 2] = ActiveLocations[i].CurrentObjective;
        }

        string[] shopItems = new string[ShopItems.Count];
        for (int i = 0; i < shopItems.Length; i++)
            shopItems[i] = ShopItems[i].ItemName;

        string[] recruitMages = new string[caMan.PlayerRecruitMages.Count];
        for (int i = 0; i < recruitMages.Length; i++)
            recruitMages[i] = caMan.PlayerRecruitMages[i].CardName;

        string[] recruitMutants = new string[caMan.PlayerRecruitMutants.Count];
        for (int i = 0; i < recruitMutants.Length; i++)
            recruitMutants[i] = caMan.PlayerRecruitMutants[i].CardName;

        string[] recruitRogues = new string[caMan.PlayerRecruitRogues.Count];
        for (int i = 0; i < recruitRogues.Length; i++)
            recruitRogues[i] = caMan.PlayerRecruitRogues[i].CardName;

        string[] recruitTechs = new string[caMan.PlayerRecruitTechs.Count];
        for (int i = 0; i < recruitTechs.Length; i++)
            recruitTechs[i] = caMan.PlayerRecruitTechs[i].CardName;

        string[] recruitWarriors = new string[caMan.PlayerRecruitWarriors.Count];
        for (int i = 0; i < recruitWarriors.Length; i++)
            recruitWarriors[i] = caMan.PlayerRecruitWarriors[i].CardName;

        string narrativeName = "";
        if (CurrentNarrative != null) narrativeName = CurrentNarrative.NarrativeName;

        GameData data = new GameData(CurrentHour, narrativeName, pMan.PlayerHero.HeroName,
            deckList, augments, items, pMan.AetherCells,
            npcsAndClips, locationsNPCsObjectives, VisitedLocations.ToArray(),
            shopItems, recruitMages, recruitMutants, recruitRogues, recruitTechs, recruitWarriors,
            RecruitLoyalty, ShopLoyalty,
            Reputation_Mages, Reputation_Mutants, Reputation_Rogues, Reputation_Techs, Reputation_Warriors);
        SaveLoad.SaveGame(data);
    }

    /******
     * *****
     * ****** CHECK_SAVE
     * *****
     *****/
    public bool CheckSave()
    {
        GameData data = SaveLoad.LoadGame();
        if (data == null) return false;
        else return true;
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

        /** LOAD RESOURCES **/
        // NARRATIVES
        Narrative[] narratives = Resources.LoadAll<Narrative>("Narratives");
        List<Narrative> allNarratives = new List<Narrative>();
        for (int i = 0; i < narratives.Length; i++)
            allNarratives.Add(narratives[i]);

        // HEROES
        Hero[] heroes = Resources.LoadAll<Hero>("Heroes");
        List<Hero> allHeroes = new List<Hero>();
        for (int i = 0; i < heroes.Length; i++)
            allHeroes.Add(heroes[i]);

        // CARDS
        List<Card> allcards = new List<Card>();
        Card[] cards = Resources.LoadAll<Card>("Cards");
        Card[] combatRewards = Resources.LoadAll<Card>("Combat Rewards");
        Card[] recruitUnits = Resources.LoadAll<Card>("Recruit Units");
        Card[] executionCards = Resources.LoadAll<Card>("Execution Cards");
        for (int i = 0; i < cards.Length; i++)
            allcards.Add(cards[i]);
        for (int i = 0; i < combatRewards.Length; i++)
            allcards.Add(combatRewards[i]);
        for (int i = 0; i < recruitUnits.Length; i++)
            allcards.Add(recruitUnits[i]);
        for (int i = 0; i < executionCards.Length; i++)
            allcards.Add(executionCards[i]);

        // LOCATIONS
        Location[] locations = Resources.LoadAll<Location>("Locations");
        Location[] randomEncounters = Resources.LoadAll<Location>("Random Encounters");
        List<Location> allLocations = new List<Location>();
        for (int i = 0; i < locations.Length; i++)
            allLocations.Add(locations[i]);
        for (int i = 0; i < randomEncounters.Length; i++)
            allLocations.Add(randomEncounters[i]);

        // DIALOGUE
        DialogueClip[] clips = Resources.LoadAll<DialogueClip>("Dialogue");
        List<DialogueClip> allClips = new List<DialogueClip>();
        for (int i = 0; i < clips.Length; i++)
            allClips.Add(clips[i]);

        // AGUMENTS
        HeroAugment[] augments = Resources.LoadAll<HeroAugment>("Hero Augments");
        List<HeroAugment> allAugments = new List<HeroAugment>();
        for (int i = 0; i < augments.Length; i++)
            allAugments.Add(augments[i]);

        // ITEMS
        HeroItem[] items = Resources.LoadAll<HeroItem>("Items");
        List<HeroItem> allItems = new List<HeroItem>();
        for (int i = 0; i < items.Length; i++)
            allItems.Add(items[i]);

        // CURRENT HOUR
        CurrentHour = data.CurrentHour; // TESTING

        // CURRENT NARRATIVE
        if (data.CurrentNarrative != "")
            CurrentNarrative = GetNarrative(data.CurrentNarrative);
        else CurrentNarrative = null;

        // PLAYER HERO
        pMan.PlayerHero = GetHero(data.PlayerHero) as PlayerHero;

        // DECK LIST
        pMan.PlayerDeckList.Clear();
        for (int i = 0; i < data.PlayerDeck.Length; i++)
            caMan.AddCard(GetCard(data.PlayerDeck[i]), PLAYER);

        // AUGMENTS
        pMan.HeroAugments.Clear();
        for (int i = 0; i < data.PlayerAugments.Length; i++)
            pMan.HeroAugments.Add(GetAugment(data.PlayerAugments[i]));

        // ITEMS
        pMan.HeroItems.Clear();
        for (int i = 0; i < data.PlayerItems.Length; i++)
            pMan.HeroItems.Add(GetItem(data.PlayerItems[i]));

        // AETHER CELLS
        pMan.AetherCells = data.AetherCells;

        // NPCS
        ActiveNPCHeroes.Clear();
        for (int i = 0; i < data.NPCSAndClips.Length/2; i++)
        {
            NPCHero npc = GetActiveNPC(GetHero(data.NPCSAndClips[i, 0]) as NPCHero);
            npc.NextDialogueClip = GetClip(data.NPCSAndClips[i, 1]);
        }

        // LOCATIONS
        ActiveLocations.Clear();
        for (int i = 0; i < data.LocationsNPCsObjectives.Length/3; i++)
        {
            Location loc = GetActiveLocation(GetLocation(data.LocationsNPCsObjectives[i, 0]));
            loc.CurrentNPC = GetActiveNPC(GetHero(data.LocationsNPCsObjectives[i, 1]) as NPCHero);
            loc.CurrentObjective = data.LocationsNPCsObjectives[i, 2];
        }
        VisitedLocations.Clear();
        foreach (string location in data.VisitedLocations)
            VisitedLocations.Add(location);

        // SHOP ITEMS
        ShopItems.Clear();
        for (int i = 0; i < data.ShopItems.Length; i++)
            ShopItems.Add(GetItem(data.ShopItems[i]));

        // RECRUITS
        // MAGES
        caMan.PlayerRecruitMages.Clear();
        for (int i = 0; i < data.RecruitMages.Length; i++)
            caMan.PlayerRecruitMages.Add(GetCard(data.RecruitMages[i]) as UnitCard);
        // MUTANTS
        caMan.PlayerRecruitMutants.Clear();
        for (int i = 0; i < data.RecruitMutants.Length; i++)
            caMan.PlayerRecruitMutants.Add(GetCard(data.RecruitMutants[i]) as UnitCard);
        // ROGUES
        caMan.PlayerRecruitRogues.Clear();
        for (int i = 0; i < data.RecruitRogues.Length; i++)
            caMan.PlayerRecruitRogues.Add(GetCard(data.RecruitRogues[i]) as UnitCard);
        // TECHS
        caMan.PlayerRecruitTechs.Clear();
        for (int i = 0; i < data.RecruitTechs.Length; i++)
            caMan.PlayerRecruitTechs.Add(GetCard(data.RecruitTechs[i]) as UnitCard);
        // WARRIORS
        caMan.PlayerRecruitWarriors.Clear();
        for (int i = 0; i < data.RecruitWarriors.Length; i++)
            caMan.PlayerRecruitWarriors.Add(GetCard(data.RecruitWarriors[i]) as UnitCard);

        // LOYALTY
        RecruitLoyalty = data.RecruitLoyalty;
        ShopLoyalty = data.ShopLoyalty;

        // REPUTATION
        Reputation_Mages = data.Reputation_Mages;
        Reputation_Mutants = data.Reputation_Mutants;
        Reputation_Rogues = data.Reputation_Rogues;
        Reputation_Techs = data.Reputation_Techs;
        Reputation_Warriors = data.Reputation_Warriors;

        Narrative GetNarrative(string narrativeName)
        {
            int index = allNarratives.FindIndex(x => x.NarrativeName == narrativeName);
            if (index != -1) return allNarratives[index];
            else Debug.LogError("NARRATIVE NOT FOUND!");
            throw new System.NullReferenceException();
        }
        Hero GetHero(string heroName)
        {
            int index = allHeroes.FindIndex(x => x.HeroName == heroName);
            if (index != -1) return allHeroes[index];
            else Debug.LogError("HERO NOT FOUND!");
            throw new System.NullReferenceException();
        }
        Card GetCard(string cardName)
        {
            int index = allcards.FindIndex(x => x.CardName == cardName);
            if (index != -1) return allcards[index];
            else Debug.LogError("CARD NOT FOUND!");
            throw new System.NullReferenceException();
        }
        Location GetLocation(string locationName)
        {
            int index = allLocations.FindIndex(x => x.LocationFullName == locationName);
            if (index != -1) return allLocations[index];
            else Debug.LogError("LOCATION NOT FOUND!");
            throw new System.NullReferenceException();
        }
        DialogueClip GetClip(string clipName)
        {
            int index = allClips.FindIndex(x => x.ToString() == clipName);
            if (index != -1) return allClips[index];
            else Debug.LogError("CLIP NOT FOUND!");
            throw new System.NullReferenceException();
        }
        HeroAugment GetAugment(string augmentName)
        {
            int index = allAugments.FindIndex(x => x.AugmentName == augmentName);
            if (index != -1) return allAugments[index];
            else Debug.LogError("AUGMENT NOT FOUND!");
            throw new System.NullReferenceException();
        }
        HeroItem GetItem(string itemName)
        {
            int index = allItems.FindIndex(x => x.ItemName == itemName);
            if (index != -1) return allItems[index];
            else Debug.LogError("ITEM NOT FOUND!");
            throw new System.NullReferenceException();
        }
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
            uMan.CreateNarrativePopup(CurrentNarrative);
            CurrentNarrative = null;
        }

        FindObjectOfType<TimeClockDisplay>().SetClockValues(CurrentHour, IsNewHour);
        if (IsNewHour) IsNewHour = false; // TESTING
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
            if (resolveOrder == 3) delay = 0.25f;

            if (reputationTier > 0 && resolveOrder == bonuses.Tier1_ResolveOrder) ScheduleEffects(bonuses.Tier1_Effects);
            if (reputationTier > 1 && resolveOrder == bonuses.Tier2_ResolveOrder) ScheduleEffects(bonuses.Tier2_Effects);
            if (reputationTier > 2 && resolveOrder == bonuses.Tier3_ResolveOrder) ScheduleEffects(bonuses.Tier3_Effects);

            void ScheduleEffects(List<EffectGroup> effects) =>
                evMan.NewDelayedAction(() => efMan.StartEffectGroupList(effects, coMan.PlayerHero), delay);
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
        if (VisitedLocations.FindIndex(x => location.LocationName == x) == -1)
            return true; // If the location has NOT been visited, it's NEVER closed
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
        
        if (CurrentHour == 4)
        {
            uMan.CreateFleetingInfoPopup("You have rested!", true);
            NextHour(true);
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
        auMan.StartStopSound(null, CurrentNarrative.NarrativeStartSound); // TESTING
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
        pHD.HeroStats.SetActive(false);
        pHD.HeroNameObject.SetActive(false);
        pHD.PowerUsedIcon.SetActive(false);
        pHD.UltimateUsedIcon.SetActive(true);
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

        // ENEMY MANAGER
        enMan.StartCombat();
        EnemyHero enemyHero = dMan.EngagedHero as EnemyHero;
        if (enemyHero == null)
        {
            Debug.LogError("ENEMY HERO IS NULL!");
            return;
        }

        enMan.EnemyHero = enemyHero;
        int enemyHealth;
        if (IsTutorial) enemyHealth = 10;
        else enemyHealth = ENEMY_STARTING_HEALTH;
        enMan.EnemyHealth = enemyHealth;

        // PLAYER MANAGER
        pMan.PlayerHealth = pMan.MaxPlayerHealth;
        pMan.EnergyPerTurn = START_ENERGY_PER_TURN;
        pMan.EnergyLeft = pMan.StartEnergy;
        pMan.HeroUltimateProgress_Direct = 0;

        // UPDATE DECKS
        caMan.UpdateDeck(PLAYER);
        caMan.UpdateDeck(ENEMY);

        // DISPLAY HEROES
        coMan.PlayerHero.GetComponent<HeroDisplay>().HeroScript = pMan.PlayerHero;
        coMan.EnemyHero.GetComponent<HeroDisplay>().HeroScript = enMan.EnemyHero;

        // SCHEDULE ACTIONS
        evMan.NewDelayedAction(() => anMan.CombatIntro(), 1);
        evMan.NewDelayedAction(() => CombatStart(), 1);
        evMan.NewDelayedAction(() => StartCombatTurn(PLAYER), 2);

        // AUDIO
        string soundtrack;
        if (enMan.EnemyHero.IsBoss) soundtrack = "Soundtrack_CombatBoss1";
        else soundtrack = "Soundtrack_Combat1";
        auMan.StartStopSound(soundtrack, null, AudioManager.SoundType.Soundtrack);
        auMan.StopCurrentSoundscape();
        auMan.StartStopSound("SFX_StartCombat1");

        void CombatStart()
        {
            int bonusCards = 0;
            if (pMan.GetAugment("Cognitive Magnifier")) bonusCards = 1;

            caMan.ShuffleDeck(PLAYER, false);
            caMan.ShuffleDeck(ENEMY, false);

            for (int i = 0; i < START_HAND_SIZE; i++)
                evMan.NewDelayedAction(() => AllDraw(), 0.5f);

            for (int i = 0; i < bonusCards; i++)
                evMan.NewDelayedAction(() => coMan.DrawCard(PLAYER), 0.5f);

            if (IsTutorial) evMan.NewDelayedAction(() => Tutorial_Tooltip(1), 0); // TUTORIAL!
            ResolveReputationEffects(1); // REPUTATION EFFECTS [RESOLVE ORDER = 1]
            PlayStartingUnits(); // TESTING
            evMan.NewDelayedAction(() => Mulligan_Player(), 0.5f);
            evMan.NewDelayedAction(() => enMan.Mulligan(), 0.5f);
            ResolveReputationEffects(2); // REPUTATION EFFECTS [RESOLVE ORDER = 2]
            ResolveReputationEffects(3); // REPUTATION EFFECTS [RESOLVE ORDER = 3]
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
                    efMan.PlayCreatedCard(newCard, coMan.EnemyHero), 0.5f);
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
            caMan.ShuffleRecruits();
            ShopItems = GetShopItems();
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
        FunctionTimer.Create(() => uMan.CreateCombatEndPopup(playerWins), 2f);

        //efMan.GiveNextEffects.Clear();
        //efMan.ChangeNextCostEffects.Clear();
    }

    /******
     * *****
     * ****** START_TURN
     * *****
     *****/
    private void StartCombatTurn(string player)
    {
        bool isPlayerTurn;
        if (player == PLAYER) isPlayerTurn = true;
        else if (player == ENEMY) isPlayerTurn = false;
        else
        {
            Debug.LogError("PLAYER NOT FOUND!");
            return;
        }

        coMan.ActionsPlayedThisTurn = 0;
        evMan.NewDelayedAction(() => TurnPopup(), 0);

        string logText = "\n";
        if (isPlayerTurn) logText += "[Your Turn]";
        else logText += "[Enemy Turn]";
        evMan.NewDelayedAction(() => uMan.CombatLogEntry(logText), 0);

        if (isPlayerTurn)
        {
            evMan.NewDelayedAction(() => PlayerTurnStart(), 2);
            evMan.NewDelayedAction(() => TurnDraw(), 0.5f);

            void TurnDraw()
            {
                coMan.DrawCard(PLAYER);
                coMan.SelectPlayableCards();
            }
            void PlayerTurnStart()
            {
                pMan.IsMyTurn = true;
                enMan.IsMyTurn = false;
                pMan.HeroPowerUsed = false;
                int startEnergy = pMan.EnergyLeft;
                pMan.EnergyLeft += pMan.EnergyPerTurn;
                int energyChange = pMan.EnergyLeft - startEnergy;
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

        evMan.NewDelayedAction(() => RefreshAllUnits(), 0.5f); // TESTING
        evMan.NewDelayedAction(() => RemoveEffects(), 0.5f);

        if (player == ENEMY)
        {
            if (enMan.EnergyPerTurn < enMan.MaxEnergyPerTurn)
                evMan.NewDelayedAction(() => IncreaseMaxEnergy(ENEMY), 0.5f); // TESTING

            evMan.NewDelayedAction(() => StartCombatTurn(PLAYER), 0.5f);
        }
        else if (player == PLAYER)
        {
            if (IsTutorial) // TUTORIAL!
            {
                switch (pMan.EnergyPerTurn)
                {
                    case 1:
                        if (pMan.EnergyLeft > 0) return;
                        else uMan.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
                        break;
                    case 2:
                        if (!pMan.HeroPowerUsed || coMan.EnemyZoneCards.Count > 0) return;
                        else uMan.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
                        break;
                }
            }

            pMan.IsMyTurn = false;
            coMan.SelectPlayableCards();
            if (pMan.EnergyPerTurn < pMan.MaxEnergyPerTurn)
                evMan.NewDelayedAction(() => IncreaseMaxEnergy(PLAYER), 0.5f);

            evMan.NewDelayedAction(() => StartCombatTurn(ENEMY), 0.5f);
        }

        void RefreshAllUnits()
        {
            coMan.RefreshUnits(PLAYER);
            coMan.RefreshUnits(ENEMY);
        }
        void IncreaseMaxEnergy(string player)
        {
            GameObject hero;
            int startEnergy;
            int energyChange;

            if (player == PLAYER)
            {
                hero = coMan.PlayerHero;
                startEnergy = pMan.EnergyPerTurn;
                pMan.EnergyPerTurn++;
                energyChange = pMan.EnergyPerTurn - startEnergy;
            }
            else
            {
                hero = coMan.EnemyHero;
                startEnergy = enMan.EnergyPerTurn;
                enMan.EnergyPerTurn++;
                energyChange = enMan.EnergyPerTurn - startEnergy;
            }

            anMan.ModifyHeroEnergyState(energyChange, hero);
        }
        void RemoveEffects()
        {
            efMan.RemoveTemporaryEffects(PLAYER);
            efMan.RemoveTemporaryEffects(ENEMY);
            efMan.RemoveGiveNextEffects(player); // TESTING
            efMan.RemoveChangeNextCostEffects(player); // TESTING
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
        HeroItem[] allItems = Resources.LoadAll<HeroItem>("Items");
        allItems.Shuffle();
        List<HeroItem> shopItems = new List<HeroItem>();
        int itemsFound = 0;
        foreach (HeroItem item in allItems)
        {
            if (pMan.HeroItems.FindIndex(x => x.ItemName == item.ItemName) == -1)
            {
                shopItems.Add(item);
                itemsFound++;
            }
            if (itemsFound == 3) break;
        }
        return shopItems;
    }

    /******
     * *****
     * ****** GET_ITEM_COST
     * *****
     *****/
    public int GetItemCost(HeroItem item, out bool isDiscounted)
    {
        int itemCost;
        if (item.IsRareItem) itemCost = BUY_RARE_ITEM_COST;
        else itemCost = BUY_ITEM_COST;
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
        if (unitCard.IsRare) recruitCost = RECRUIT_RARE_UNIT_COST;
        else recruitCost = RECRUIT_UNIT_COST;
        if (RecruitLoyalty == RECRUIT_LOYALTY_GOAL)
        {
            isDiscounted = true;
            recruitCost -= RECRUIT_UNIT_COST;
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
        if (unitCard.IsRare) cloneCost = CLONE_RARE_UNIT_COST;
        else cloneCost = CLONE_UNIT_COST;
        return cloneCost;
    }
}
