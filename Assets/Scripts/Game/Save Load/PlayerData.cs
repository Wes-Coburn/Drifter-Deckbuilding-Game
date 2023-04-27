[System.Serializable]
public class PlayerData : SaveData
{
    public static PlayerData SavedPlayerData = null;
    
    /*
     * <<< | UNIVERSAL SAVE | >>>
     */

    // Save Scene
    public string SaveScene; // SceneLoad.Scene.ToString() -> | WorldMapScene | CombatScene |
    
    // Player Hero
    public string PlayerHero;
    public int CurrentHealth_Player, AetherCells;
    // Power and Ultimate
    public string HeroPower, HeroUltimate;
    // Player Hero "Inventory"
    public string[] PlayerDeck, PlayerItems;
    // Hour
    public int CurrentHour;
    public bool IsNewHour;
    // Narrative
    public string CurrentNarrative;
    // Engaged Hero
    public string EngagedHero;
    // Locations
    public string CurrentLocation;
    public string[] VisitedLocations;
    public string[,] ActiveLocations; // string[3] -> 0: LocationName, 1: CurrentNPC.HeroName, 2: CurrentObjective
    // Dialogue
    public string[,] NPCSAndClips;
    // Shops
    public string[,] ShopItems; // string[2] -> 0: ItemName, 1: ItemUsed (combat only)
    public string[] RecruitUnits, ShopActions;
    // Loyalty
    public int RecruitLoyalty, ActionShopLoyalty, ShopLoyalty;

    /*
     * <<< | COMBAT SAVE | >>>
     */

    // Enemy Hero
    public string EnemyHero;
    // Hero Turn
    public string HeroTurnCombat;
    public const string TURN_PLAYER = "PLAYER_TURN";
    public const string TURN_ENEMY = "ENEMY_TURN";
    // Turn Number
    public int TurnNumber_Player, TurnNumber_Enemy;
    // Enemy Health
    public int CurrentHealth_Enemy;
    // Energy Per Turn
    public int EnergyPerTurn_Player, EnergyPerTurn_Enemy;
    // Current Energy
    public int CurrentEnergy_Player, CurrentEnergy_Enemy;
    // Hero Ultimate Progress
    public int HeroUltimateProgress;
    // Allies Destroyed
    public int AlliesDestroyed_ThisTurn_Player, AlliesDestroyed_ThisTurn_Enemy;
    // Damage Taken
    public int DamageTaken_ThisTurn_Player, DamageTaken_ThisTurn_Enemy;
    // Exploits Played
    public int ExploitsPlayed_Player, ExploitsPlayed_Enemy;
    // Inventions Played
    public int InventionsPlayed_Player, InventionsPlayed_Enemy;
    // Schemes Played
    public int SchemesPlayed_Player, SchemesPlayed_Enemy;
    // Extractions Played
    public int ExtractionsPlayed_Player;
    //public int ExtractionsPlayed_Enemy; // Enemies don't play extractions

    /*
     * <<< | EFFECTS | >>>
     * 
     *              (Universal Values)
     *              [0] Name
     * |Effect| --> [1] Countdown
     *              (Multiplier Effect Values)
     *              [2] Multiplier
     *              (StatChange Effect Values)
     *              [3] PowerChange
     *              [4] HealthChange
     *              
     */


    // GiveNextEffects
    public string[][] GiveNextEffects_Player, GiveNextEffects_Enemy;
    // ChangeNextCostEffects
    public string[][] ChangeNextCostEffects_Player, ChangeNextCostEffects_Enemy;
    // ModifyNextEffects
    public string[][] ModifyNextEffects_Player, ModifyNextEffects_Enemy;

    /*
     * <<< | CARDS | >>>
     * 
     *                                     (Universal Values)
     *                                     [0] Name
     *                        |---> [0] -> [1] Cost
     *                        |            [2] BanishAfterPlay
     *            |---> [0]---|            
     *            |           |            (Unit Values)
     *            |           |            [0] Power
     *            |           |---> [1] -> [1] Health
     *            |                        [2] MaxHealth
     *            |                        [3] IsExhausted
     * |Card| --> |
     *            |     (Current Effects)
     *            |---> [1] ---> [0..i] |Effect|
     *            |    
     *            |     (Permanent Effects)
     *            |---> [2] ---> [0..i] |Effect|
     *            |
     *            |     (Current Abilities)
     *            |---> [3] ---> [0..i] |CardAbility|
     * 
     */

    // Current Deck
    public string[][][][] CurrentDeck_Player, CurrentDeck_Enemy;
    // Hand Zone Cards
    public string[][][][] HandZoneCards_Player, HandZoneCards_Enemy;
    // Play Zone Cards
    public string[][][][] PlayZoneCards_Player, PlayZoneCards_Enemy;
    // Discard Zone Cards
    public string[][][][] DiscardZoneCards_Player, DiscardZoneCards_Enemy;

    public PlayerData(
        /*
         * <<< | UNIVERSAL SAVE | >>>
         */

        // Save Scene
        string saveScene,
        // Player Hero
        string playerHero, int currentHealth_Player, int aetherCells,
        // Power and Ultimate
        string heroPower, string heroUltimate,
        // Player Hero "Inventory"
        string[] deckList, string[] items,
        // Hour and Narrative
        int currentHour, bool isNewHour, string currentNarrative,
        // Engaged Hero and Current Location
        string engagedHero, string currentLocation,
        // Locations
        string[] visitedLocations, string[,] locationsNPCsObjectives,
        // Dialogue
        string[,] npcsAndClips,
        // Shops
        string[,] shopItems, string[] recruitUnits, string[] shopActions,
        // Loyalty
        int recruitLoyalty, int actionShopLoyalty, int shopLoyalty,

        /*
         * <<< | COMBAT SAVE | >>>
         */

        // Enemy Hero
        string enemyHero,
        // Hero Turn
        string heroTurnCombat,
        // Turn Number
        int turnNumber_Player, int turnNumber_Enemy,
        // Enemy Health
        int currentHealth_Enemy,
        // Energy Per Turn
        int energyPerTurn_Player, int energyPerTurn_Enemy,
        // Current Energy
        int currentEnergy_Player, int currentEnergy_Enemy,
        // Hero Ultimate Progress
        int heroUltimateProgress,
        // AlliesDestroyed_ThisTurn
        int alliesDestroyed_ThisTurn_Player, int alliesDestroyed_ThisTurn_Enemy,
        // DamageTaken_ThisTurn
        int damageTaken_ThisTurn_Player, int damageTaken_ThisTurn_Enemy,
        // Exploits Played
        int exploitsPlayed_Player, int exploitsPlayed_Enemy,
        // Inventions Played
        int inventionsPlayed_Player, int inventionsPlayed_Enemy,
        // Schemes Played
        int schemesPlayed_Player, int schemesPlayed_Enemy,
        // Extractions Played
        int extractionsPlayed_Player, // int extractionsPlayed_Enemy

        
        // GiveNextEffects
        string[][] giveNextEffects_Player, string[][] giveNextEffects_Enemy,
        // ChangeNextCostEffects
        string[][] changeNextCostEffects_Player, string[][] changeNextCostEffects_Enemy,
        // ModifyNextEffects
        string[][] modifyNextEffects_Player, string[][] modifyNextEffects_Enemy,

        /** CARDS **/
        // Current Deck
        string[][][][] currentDeck_Player, string[][][][] currentDeck_Enemy,
        // Hand Zone Cards
        string[][][][] handZoneCards_Player, string[][][][] handZoneCards_Enemy,
        // Play Zone Cards
        string[][][][] playZoneCards_Player, string[][][][] playZoneCards_Enemy,
        // Discard Zone Cards
        string[][][][] discardZoneCards_Player, string[][][][] discardZoneCards_Enemy
        )
    {
        /*
         * <<< | UNIVERSAL SAVE | >>>
         */

        // Save Scene
        SaveScene = saveScene;
        // Player Hero
        PlayerHero = playerHero;
        CurrentHealth_Player = currentHealth_Player;
        AetherCells = aetherCells;
        // Power and Ultimate
        HeroPower = heroPower;
        HeroUltimate = heroUltimate;
        // Hour and Narrative
        CurrentHour = currentHour;
        IsNewHour = isNewHour;
        CurrentNarrative = currentNarrative;
        // Engaged Hero
        EngagedHero = engagedHero;
        // Current Location
        CurrentLocation = currentLocation;
        // Player Hero "Inventory"
        PlayerDeck = deckList;
        PlayerItems = items;
        // Locations
        ActiveLocations = locationsNPCsObjectives;
        VisitedLocations = visitedLocations;
        // Dialogue
        NPCSAndClips = npcsAndClips;
        // Shops
        ShopItems = shopItems;
        RecruitUnits = recruitUnits;
        ShopActions = shopActions;
        // Loyalty
        RecruitLoyalty = recruitLoyalty;
        ActionShopLoyalty = actionShopLoyalty;
        ShopLoyalty = shopLoyalty;

        /*
         * <<< | COMBAT SAVE | >>>
         */

        if (saveScene != SceneLoader.Scene.CombatScene.ToString()) return;

        // Enemy Hero
        EnemyHero = enemyHero;
        // Hero Turn
        HeroTurnCombat = heroTurnCombat;
        // Turn Number
        TurnNumber_Player = turnNumber_Player;
        TurnNumber_Enemy = turnNumber_Enemy;
        // Enemy Health
        CurrentHealth_Enemy = currentHealth_Enemy;
        // Energy Per Turn
        EnergyPerTurn_Player = energyPerTurn_Player;
        EnergyPerTurn_Enemy = energyPerTurn_Enemy;
        // Current Energy
        CurrentEnergy_Player = currentEnergy_Player;
        CurrentEnergy_Enemy = currentEnergy_Enemy;
        // Hero Ultimate Progress
        HeroUltimateProgress = heroUltimateProgress;
        // AlliesDestroyed_ThisTurn
        AlliesDestroyed_ThisTurn_Player = alliesDestroyed_ThisTurn_Player;
        AlliesDestroyed_ThisTurn_Enemy = alliesDestroyed_ThisTurn_Enemy;
        // DamageTaken_ThisTurn
        DamageTaken_ThisTurn_Player = damageTaken_ThisTurn_Player;
        DamageTaken_ThisTurn_Enemy = damageTaken_ThisTurn_Enemy;
        // ExploitsPlayed
        ExploitsPlayed_Player = exploitsPlayed_Player;
        ExploitsPlayed_Enemy = exploitsPlayed_Enemy;
        // InventionsPlayed
        InventionsPlayed_Player = inventionsPlayed_Player;
        InventionsPlayed_Enemy = inventionsPlayed_Enemy;
        // SchemesPlayed
        SchemesPlayed_Player = schemesPlayed_Player;
        SchemesPlayed_Enemy = schemesPlayed_Enemy;
        // ExtractionsPlayed
        ExtractionsPlayed_Player = extractionsPlayed_Player;
        // ExtractionsPlayed_Enemy = extractionsPlayed_Enemy;
        
        // GiveNextEffects
        GiveNextEffects_Player = giveNextEffects_Player;
        GiveNextEffects_Enemy = giveNextEffects_Enemy;
        // ChangeNextCostEffects
        ChangeNextCostEffects_Player = changeNextCostEffects_Player;
        ChangeNextCostEffects_Enemy = changeNextCostEffects_Enemy;
        // ModifyNextEffects
        ModifyNextEffects_Player = modifyNextEffects_Player;
        ModifyNextEffects_Enemy = modifyNextEffects_Enemy;
        
        // Current Deck
        CurrentDeck_Player = currentDeck_Player;
        CurrentDeck_Enemy = currentDeck_Enemy;
        // Hand Zone Cards
        HandZoneCards_Player = handZoneCards_Player;
        HandZoneCards_Enemy = handZoneCards_Enemy;
        // Play Zone Cards
        PlayZoneCards_Player = playZoneCards_Player;
        PlayZoneCards_Enemy = playZoneCards_Enemy;
        // Discard Zone Cards
        DiscardZoneCards_Player = discardZoneCards_Player;
        DiscardZoneCards_Enemy = discardZoneCards_Enemy;
    }
}