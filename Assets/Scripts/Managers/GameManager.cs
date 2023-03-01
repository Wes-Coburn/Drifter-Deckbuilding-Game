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
    [SerializeField, TextArea] private string[] loadingTips;
    [Header("REPUTATION BONUSES")]
    [SerializeField] private ReputationBonuses reputation_Mages;
    [SerializeField] private ReputationBonuses reputation_Mutants;
    [SerializeField] private ReputationBonuses reputation_Rogues;
    [SerializeField] private ReputationBonuses reputation_Techs;
    [SerializeField] private ReputationBonuses reputation_Warriors;

    private int currentTip;

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
    public const int PLAYER_START_UNITS = 2; // Total = value * 5 (unique units)
    public const int MAXIMUM_ENERGY_PER_TURN = 10;
    public const int MAXIMUM_ENERGY = 10;
    public const int MAXIMUM_ITEMS = 2;
    public const int HERO_ULTMATE_GOAL = 3;

        // Starting bonuses
    public const int PLAYER_START_AETHER = 30;
    public const int BONUS_START_REWARDS = 4;

    // Enemy
    public const string ENEMY = "Enemy";
    public const int ENEMY_STARTING_HEALTH = 30;
    //public const int ENEMY_STARTING_HEALTH = 1; // FOR TESTING ONLY
    // Tutorial Enemy
    public const int TUTORIAL_STARTING_HEALTH = 10;
    // Boss Enemy
    private const int ADDITIONAL_ENERGY = 0;

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
            if (currentTip > loadingTips.Length - 1) currentTip = 0;
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

    public List<string> UnlockedHeroes { get; set; }
    public List<string> UnlockedPowers { get; set; }

    public bool IsNewHour { get; set; }
    public int CurrentHour { get; set; }
    public Narrative CurrentNarrative { get; set; }
    public Location CurrentLocation { get; set; }
    public List<NPCHero> ActiveNPCHeroes { get; private set; }
    public List<Location> ActiveLocations { get; private set; }
    public List<string> VisitedLocations { get; private set; }
    public List<HeroItem> ShopItems { get; set; }
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
    #region UTILITY
    private void Start()
    {
        //PlayerPrefs.DeleteAll(); // FOR TESTING ONLY
        currentTip = Random.Range(0, loadingTips.Length);
        ActiveNPCHeroes = new List<NPCHero>();
        ActiveLocations = new List<Location>();
        VisitedLocations = new List<string>();
        ShopItems = new List<HeroItem>();
        UnlockedHeroes = new List<string>();
        UnlockedPowers = new List<string>();

        GameLoader.LoadPlayerPreferences();
        GameLoader.LoadGame_GameData(); // TESTING
        Debug.Log("Application Version: " + Application.version);
    }
    /******
     * *****
     * ****** DIFFICULTY_LEVEL
     * *****
     *****/
    public enum DifficultyLevel
    {
        Standard_1 = 1,
        Standard_2 = 2,
        Boss_1 = 3,
        Standard_3 = 4,
        Boss_2 = 5,
    }
    public int GetSurgeDelay(int difficulty) => 9 - (difficulty * 2);
    public int GetAdditionalEnergy(int difficulty) => ADDITIONAL_ENERGY + difficulty - 1;
    public int GetAdditionalRewardAether(int difficulty) => ADDITIONAL_AETHER_REWARD * (difficulty - 1);
    public int GetAetherReward(DifficultyLevel difficultyLevel)
    {
        int reward;
        switch (difficultyLevel)
        {
            // Standard Difficulties
            case DifficultyLevel.Standard_1:
                reward = AETHER_COMBAT_REWARD_1;
                break;
            case DifficultyLevel.Standard_2:
                reward = AETHER_COMBAT_REWARD_2;
                break;
            case DifficultyLevel.Standard_3:
                reward = AETHER_COMBAT_REWARD_3;
                break;
            // Boss Difficulties
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
        return reward + GetAdditionalRewardAether(Managers.CO_MAN.DifficultyLevel);
    }
    public Sprite GetLocationBackground()
    {
        if (CurrentLocation == null)
        {
            Debug.LogWarning("CURRENT LOCATION IS NULL!");
            return null;
        }

        var background = CurrentLocation.LocationBackground;

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
    public void AddRandomEncounter()
    {
        Location[] randomEncounters = Resources.LoadAll<Location>("Random Encounters");
        randomEncounters.Shuffle();

        foreach (Location location in randomEncounters)
        {
            if (VisitedLocations.FindIndex(x => x == location.LocationName) == -1)
            {
                GetActiveLocation(location);
                return;
            }
        }
        Debug.LogWarning("NO VALID RANDOM ENCOUNTERS!");
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

        // New Hour
        IsNewHour = true;
        // Current Hour
        CurrentHour = CurrentHour == 4 ? 1 : CurrentHour + 1;
        // Random Encounter
        if (addRandomEncounter && CurrentHour != 4) AddRandomEncounter();
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

        int activeNPC = ActiveNPCHeroes.FindIndex(x => x.HeroName == npc.HeroName);
        if (activeNPC != -1) return ActiveNPCHeroes[activeNPC];
        else
        {
            NPCHero newNPC = ScriptableObject.CreateInstance(npc.GetType()) as NPCHero;
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

        int activeLocation = ActiveLocations.FindIndex(x => x.LocationName == location.LocationName);

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
        List<HeroItem> rarefiedItems = new();
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

        List<HeroItem> shopItems = new();
        foreach (HeroItem item in rarefiedItems)
        {
            if ((Managers.P_MAN.HeroItems.FindIndex(x => x.ItemName == item.ItemName) == -1) &&
                (shopItems.FindIndex(x => x.ItemName == item.ItemName) == -1)) shopItems.Add(item);

            if (shopItems.Count == 8) return shopItems;
        }
        return shopItems;
    }
    /******
     * *****
     * ****** TUTORIAL_TOOLTIPS
     * *****
     *****/
    public void Tutorial_Tooltip(int tipNumber)
    {
        string tip;
        bool isCentered = true;
        bool showContinue = false;

        switch (tipNumber)
        {
            case 1:
                tip = $"Click each card you want to redraw, then click the " +
                    $"{TextFilter.Clrz_ylw("Confirm Button")} (or press the {TextFilter.Clrz_ylw("Space Bar")}).";
                break;
            case 2:
                tip = $"Play a card by dragging it out of your hand. Cards you can play are highlighted in {TextFilter.Clrz_grn("green")}.";
                break;
            case 3:
                tip = $"End your turn by clicking the {TextFilter.Clrz_ylw("End Turn Button")} " +
                    $"(or pressing the {TextFilter.Clrz_ylw("Space Bar")}).";
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

        Managers.U_MAN.CreateInfoPopup(tip, UIManager.InfoPopupType.Tutorial, isCentered, showContinue);
    }
    #endregion

    #region SCENE STARTERS
    public void StartTitleScene()
    {
        Managers.AU_MAN.StartStopSound("Soundtrack_TitleScene", null, AudioManager.SoundType.Soundtrack);
        Managers.AU_MAN.StartStopSound("Soundscape_TitleScene", null, AudioManager.SoundType.Soundscape);
        GameObject.Find("VersionNumber").GetComponent<TextMeshProUGUI>().SetText(Application.version);
        IsCombatTest = false;
    }
    public void StartHeroSelectScene()
    {
        //Managers.AU_MAN.StopCurrentSoundscape();
        FindObjectOfType<HeroSelectSceneDisplay>().DisplaySelectedHero();
    }
    public void StartTutorialScene()
    {
        IsTutorial = true;
        SceneLoader.LoadAction += GameLoader.Tutorial_Load;
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
    }
    public void StartWorldMapScene()
    {
        Managers.AU_MAN.StartStopSound("Soundtrack_WorldMapScene", null, AudioManager.SoundType.Soundtrack);
        Managers.AU_MAN.StartStopSound("SFX_EnterWorldMap");

        foreach (Location loc in ActiveLocations)
        {
            GameObject location = Instantiate(locationIconPrefab, Managers.U_MAN.CurrentCanvas.transform);
            location.GetComponent<LocationIcon>().Location = loc;
        }

        GameLoader.SaveGame(); // Save before (1) CurrentNarrative == null and (2) ChooseRewardPopup.BonusRewards > 0

        if (CurrentNarrative != null)
        {
            if (CurrentNarrative.IsGameEnd) Managers.U_MAN.CreateGameEndPopup();
            else Managers.U_MAN.CreateNarrativePopup(CurrentNarrative);
            CurrentNarrative = null;
        }

        FindObjectOfType<TimeClockDisplay>().SetClockValues(CurrentHour, IsNewHour);
        if (IsNewHour) IsNewHour = false;
    }
    public void StartHomeBaseScene()
    {
        //Managers.AU_MAN.StopCurrentSoundscape();
        Managers.AU_MAN.StartStopSound("SFX_EnterHomeBase");

        bool hasRested = CurrentHour == 4;
        FindObjectOfType<HomeBaseSceneDisplay>().ClaimRewardButton.SetActive(hasRested);

        if (hasRested)
        {
            NextHour(true);
            ShopItems = GetShopItems();
            Managers.CA_MAN.LoadNewRecruits();
            Managers.CA_MAN.LoadNewActions();
            Managers.U_MAN.CreateFleetingInfoPopup("You have rested!\nShops refreshed!");

            List<Location> refreshedShops = new();
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
    public void StartCreditsScene()
    {
        //Managers.AU_MAN.StopCurrentSoundscape();
        Managers.AU_MAN.StartStopSound("Soundtrack_Combat1",
            null, AudioManager.SoundType.Soundtrack);
    }
    public void StartNarrativeScene()
    {
        Managers.AU_MAN.StartStopSound("Soundtrack_Narrative1", null,
            AudioManager.SoundType.Soundtrack);
        Managers.AU_MAN.StartStopSound(null, CurrentNarrative.NarrativeStartSound);
        Managers.AU_MAN.StartStopSound(null, CurrentNarrative.NarrativeSoundscape,
            AudioManager.SoundType.Soundscape);

        FindObjectOfType<NarrativeSceneDisplay>().CurrentNarrative = CurrentNarrative;
    }
    public void EndNarrativeScene()
    {
        if (CurrentNarrative == GameLoader.SettingNarrative)
        {
            CurrentNarrative = GameLoader.NewGameNarrative;
            SceneLoader.LoadScene(SceneLoader.Scene.HeroSelectScene);
        }
        else Debug.LogError("NO CONDITIONS MATCHED!");
    }
    #endregion

    #region REPUTATION
    /******
     * *****
     * ****** REPUTATION
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
    public int GetBonusReputation(int difficulty) => difficulty > 2 ? 1 : 0;
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
        Managers.EV_MAN.NewDelayedAction(() => Managers.U_MAN.SetReputation(repType, repChange), 0.5f);
    }

    public void GiveReputationRewards(CombatRewardClip crc)
    {
        int bonusReputation = GetBonusReputation(Managers.CO_MAN.DifficultyLevel);

        if (crc.Reputation_Mages != 0)
            ChangeReputation(ReputationType.Mages, crc.Reputation_Mages + bonusReputation);
        if (crc.Reputation_Mutants != 0)
            ChangeReputation(ReputationType.Mutants, crc.Reputation_Mutants + bonusReputation);
        if (crc.Reputation_Rogues != 0)
            ChangeReputation(ReputationType.Rogues, crc.Reputation_Rogues + bonusReputation);
        if (crc.Reputation_Techs != 0)
            ChangeReputation(ReputationType.Techs, crc.Reputation_Techs + bonusReputation);
        if (crc.Reputation_Warriors != 0)
            ChangeReputation(ReputationType.Warriors, crc.Reputation_Warriors + bonusReputation);
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

    public void ResolveReputationEffects(int resolveOrder)
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

                Managers.EV_MAN.NewDelayedAction(() => ResolveEffects(repType, effects, showTrigger), delay);
            }
        }

        void ResolveEffects(ReputationType repType, List<EffectGroup> repEffects, bool showTrigger)
        {
            if (showTrigger) Managers.U_MAN.SetReputation(repType, 0, true);

            if (repEffects != null && repEffects.Count > 0)
                Managers.EF_MAN.StartEffectGroupList(repEffects, Managers.P_MAN.HeroObject);
        }
    }
    #endregion

    #region COST GETTERS
    /******
     * *****
     * ****** COST_GETTERS
     * *****
     *****/
    public int GetItemCost(HeroItem item, out bool isDiscounted, bool isItemRemoval)
    {
        int itemCost = isItemRemoval ? (item.IsRareItem ? SELL_RARE_ITEM_VALUE : SELL_ITEM_VALUE) :
            (item.IsRareItem ? BUY_RARE_ITEM_COST : BUY_ITEM_COST);

        if (ShopLoyalty == SHOP_LOYALTY_GOAL)
        {
            isDiscounted = true;
            itemCost -= BUY_ITEM_COST;
        }
        else isDiscounted = false;

        return itemCost;
    }
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

    /******
     * *****
     * ****** NEW_GAME
     * *****
     *****/
    public void NewGame()
    {
        SceneLoader.LoadAction_Async += GameLoader.NewGame_LoadAsync;

        //SceneLoader.LoadAction += GameLoader.NewGame_Load;
        SceneLoader.LoadScene(SceneLoader.Scene.NarrativeScene);
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
        Managers.P_MAN.HeroScript = null;
        Managers.P_MAN.DeckList.Clear();
        Managers.P_MAN.CurrentDeck.Clear();
        //ManagerHandler.P_MAN.AetherCells = 0;
        Managers.P_MAN.HeroAugments.Clear();
        Managers.P_MAN.HeroItems.Clear();
        // Enemy Manager
        Destroy(Managers.EN_MAN.HeroScript);
        Managers.EN_MAN.HeroScript = null;
        // Dialogue Manager
        Managers.D_MAN.EndDialogue();
        // UI Manager
        Managers.U_MAN.ClearAugmentBar();
        Managers.U_MAN.ClearItemBar();
        // Scene Loader
        SceneLoader.LoadScene(SceneLoader.Scene.TitleScene);
    }
    #endregion
}
