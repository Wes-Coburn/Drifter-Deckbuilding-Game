using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameLoader
{
    // Narratives
    public static Narrative SettingNarrative { get; set; }
    public static Narrative NewGameNarrative { get; set; }

    // Player Preferences
    public const string HIDE_EXPLICIT_LANGUAGE = "HideExplicitLanguage";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";

    // Remove type name suffix, i.e. " (DamageEffect)"
    private static string GetObjectName(ScriptableObject scrObj)
    {
        string name = scrObj.ToString();
        return name.Replace($" ({scrObj.GetType().Name})", "");
    }

    public static void SavePlayerPreferences()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, Managers.AU_MAN.MusicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME, Managers.AU_MAN.SFXVolume);
        PlayerPrefs.SetInt(HIDE_EXPLICIT_LANGUAGE, Managers.G_MAN.HideExplicitLanguage ? 1 : 0);
    }

    public static void LoadPlayerPreferences()
    {
        Managers.AU_MAN.MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1);
        Managers.AU_MAN.SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1);
        Managers.G_MAN.HideExplicitLanguage = PlayerPrefs.GetInt(HIDE_EXPLICIT_LANGUAGE, 1) == 1;
    }

    /******
     * *****
     * ****** TUTORIAL
     * *****
     *****/
    public static IEnumerator LoadTutorial_Async()
    {
        Managers.G_MAN.IsTutorial = true;

        // Player Manager
        Managers.P_MAN.CurrentHealth = GameManager.PLAYER_STARTING_HEALTH;
        Managers.P_MAN.CurrentAether = 0;

        // Heroes
        string phName = "Kili, Neon Rider";
        string ehName = "Tiny Mutant";
        var heroes = Resources.LoadAll<Hero>("Tutorial");

        foreach (var hero in heroes)
        {
            if (hero.HeroName == phName)
            {
                var playerHero = ScriptableObject.CreateInstance<PlayerHero>();
                playerHero.LoadHero(hero);
                Managers.P_MAN.HeroScript = playerHero;
            }
            else if (hero.HeroName == ehName)
            {
                var enemyHero = ScriptableObject.CreateInstance<EnemyHero>();
                enemyHero.LoadHero(hero);
                Managers.D_MAN.EngagedHero = enemyHero;
            }
        }

        Managers.P_MAN.DeckList.Clear(); // TESTING
        foreach (var unit in Managers.CA_MAN.TutorialPlayerUnits)
            for (int i = 0; i < 5; i++)
                Managers.CA_MAN.AddCard(unit, Managers.P_MAN);

        SceneLoader.CurrentLoadRoutine = null; // TESTING
        SceneLoader.LoadingProgress = 1;
        SceneLoader.LoadScene_Finish(SceneLoader.Scene.CombatScene);

        yield break; // Required for IEnumerator
    }

    /******
     * *****
     * ****** NEW_GAME
     * *****
     *****/
    private static IEnumerator LoadNewGame_Async_Progress()
    {
        yield return new WaitUntil(() => Managers.U_MAN.SceneFadeRoutine == null); // TESTING
        if (SceneLoader.CurrentLoadRoutine != null) yield break; // TESTING

        const int loadItems = 8;
        const float increment = 1f / loadItems;
        if (SceneLoader.LoadingProgress < 0) SceneLoader.LoadingProgress = 0;
        SceneLoader.LoadingProgress += increment;
    }
    public static IEnumerator LoadNewGame_Async()
    {
        var gm = Managers.G_MAN;
        gm.IsTutorial = false;

        // Loyalty
        gm.RecruitLoyalty = 3; // First recruit free
        gm.ActionShopLoyalty = 3; // First action free
        gm.ShopLoyalty = 3; // First item free

        // Player Manager
        Managers.P_MAN.CurrentHealth = GameManager.PLAYER_STARTING_HEALTH - GameManager.HEAL_ON_REST;
        Managers.P_MAN.CurrentAether = GameManager.PLAYER_START_AETHER;
        Managers.P_MAN.DeckList.Clear();

        // Starting Units
        foreach (var uc in Managers.CA_MAN.PlayerStartUnits)
            for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                Managers.CA_MAN.AddCard(uc, Managers.P_MAN);

        // Load New Content
        Managers.CA_MAN.LoadNewRecruits();
        Managers.CA_MAN.LoadNewActions();
        gm.LoadNewItems();

        // Starting Narratives
        string narPath = "Narratives";
        var request_setting = Resources.LoadAsync<Narrative>($"{narPath}/SettingNarrative");
        yield return request_setting;

        if (request_setting.asset == null) Debug.LogError("Failed to load NARRATIVE asset!");
        else
        {
            SettingNarrative = request_setting.asset as Narrative;
            gm.CurrentNarrative = SettingNarrative;
        }

        yield return LoadNewGame_Async_Progress(); // <<< Load Progress #1

        var request_narrative = Resources.LoadAsync<Narrative>($"{narPath}/NewGameNarrative");
        yield return request_narrative;

        if (request_narrative.asset == null) Debug.LogError("Failed to load NARRATIVE asset!");
        else NewGameNarrative = request_narrative.asset as Narrative;

        yield return LoadNewGame_Async_Progress(); // <<< Load Progress #2

        // Starting Locations
        foreach (string loc in new string[]
        {
            "Your Ship",
            "The Alley",
            "The Rathole",
            "The Emporium",
            "The Heaps",
            "The Oasis",
        })
        {
            var request_location = Resources.LoadAsync<Location>($"Locations/{loc}");
            yield return request_location;
            yield return LoadNewGame_Async_Progress(); // <<< Load Progress #3-8

            if (request_location.asset == null)
            {
                Debug.LogError("Failed to load LOCATION asset!");
                continue;
            }

            var location = request_location.asset as Location;
            gm.GetActiveLocation(location);
        }

        // Current Hour
        gm.CurrentHour = 3;
        gm.NextHour(false); // Set current hour to 4 and change homebase objective

        SceneLoader.BackgroundLoadRoutine = null; // TESTING
        if (SceneLoader.SceneIsLoading) SceneLoader.LoadScene_Finish(SceneLoader.Scene.NarrativeScene);
    }

    /******
     * *****
     * ****** SAVED_GAME
     * *****
     *****/
    public static void LoadSavedGame_GameData()
    {
        if (SaveLoad.LoadGame(SaveLoad.SaveType.Game) is not GameData data)
        {
            GameData newData = new(Managers.G_MAN.StartingHeroes,
                // Powers and Augments
                new string[]{}, new string[]{},
                // Reputation
                0, 0, 0, 0, 0,
                // Tutorials
                true);
            SaveLoad.SaveGame(newData, SaveLoad.SaveType.Game);
            data = SaveLoad.LoadGame(SaveLoad.SaveType.Game) as GameData;
        }

        // AUGMENTS
        for (int i = 0; i < data.HeroAugments.Length; i++)
        {
            var request_Aug = Resources.LoadAsync<HeroAugment>($"Hero Augments/{data.HeroAugments[i]}");
            //yield return request_Aug;

            if (request_Aug.asset == null)
            {
                Debug.LogError("Failed to load AUGMENT asset!");
                continue;
            }

            Managers.P_MAN.AddAugment(request_Aug.asset as HeroAugment);
        }

        // HEROES AND POWERS
        var heroes = Managers.G_MAN.UnlockedHeroes;
        var powers = Managers.G_MAN.UnlockedPowers;
        foreach (string hero in data.UnlockedHeroes) heroes.Add(hero);
        foreach (string power in data.UnlockedPowers) powers.Add(power);

        // REPUTATION
        var gm = Managers.G_MAN;
        gm.Reputation_Mages = data.Reputation_Mages;
        gm.Reputation_Mutants = data.Reputation_Mutants;
        gm.Reputation_Rogues = data.Reputation_Rogues;
        gm.Reputation_Techs = data.Reputation_Techs;
        gm.Reputation_Warriors = data.Reputation_Warriors;

        // Tutorials
        gm.TutorialActive_WorldMap = data.TutorialActive_WorldMap;
    }
    private static IEnumerator LoadSavedGame_PlayerData_Async_Progress(bool isCombatLoad)
    {
        yield return new WaitUntil(() => Managers.U_MAN.SceneFadeRoutine == null); // TESTING
        if (SceneLoader.CurrentLoadRoutine != null) yield break; // TESTING

        int loadItems = isCombatLoad ? 18 : 11;
        float increment = 1f / loadItems;
        if (SceneLoader.LoadingProgress < 0) SceneLoader.LoadingProgress = 0;
        SceneLoader.LoadingProgress += increment;
    }
    public static IEnumerator LoadSavedGame_PlayerData_Async()
    {
        if (SaveLoad.LoadGame(SaveLoad.SaveType.Player) is not PlayerData data)
        {
            Debug.LogError("SAVE DATA IS NULL!");
            yield break;
        }

        var gm = Managers.G_MAN;
        var pm = Managers.P_MAN;
        var em = Managers.EN_MAN;

        bool isCombatLoad = data.SaveScene == SceneLoader.Scene.CombatScene.ToString();

        /*
         * <<< | UNIVERSAL LOAD | >>>
         */

        // PLAYER HERO
        var request_PHero = Resources.LoadAsync<PlayerHero>($"Heroes/Player Heroes/{data.PlayerHero}");
        yield return request_PHero;
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #1

        if (request_PHero.asset == null) Debug.LogError("Failed to load PLAYERHERO asset!");
        else
        {
            var playerHero = ScriptableObject.CreateInstance<PlayerHero>();
            playerHero.LoadHero(request_PHero.asset as PlayerHero);
            pm.HeroScript = playerHero;
        }

        // CURRENT HEALTH
        pm.CurrentHealth = data.CurrentHealth_Player;

        // AETHER CELLS
        pm.CurrentAether = data.AetherCells;

        // HERO POWER
        var pHero = pm.HeroScript as PlayerHero;
        HeroPower phPower = null;
        string powerName = data.HeroPower;
        if (powerName == pHero.HeroPower.PowerName) phPower = pHero.HeroPower;
        else
        {
            foreach (var pwr in pHero.AltHeroPowers)
                if (powerName == pwr.PowerName) phPower = pwr;
        }

        if (phPower == null) Debug.LogError($"POWER <{powerName}> NOT FOUND!");
        else pHero.CurrentHeroPower = phPower;

        // HERO ULTIMATE
        HeroPower phUlt = null;
        string ultName = data.HeroUltimate;
        if (ultName == pHero.HeroUltimate.PowerName) phUlt = pHero.HeroUltimate;
        else
        {
            foreach (var pwr in pHero.AltHeroUltimates)
                if (ultName == pwr.PowerName) phUlt = pwr;
        }

        if (phUlt == null) Debug.LogError($"ULTIMATE <{ultName}> NOT FOUND!");
        else pHero.CurrentHeroUltimate = phUlt;

        // DECK LIST
        pm.DeckList.Clear();
        for (int i = 0; i < data.PlayerDeck.Length; i++)
        {
            var request_Card = Resources.LoadAsync<Card>($"Cards_Starting/{data.PlayerDeck[i]}");
            yield return request_Card;

            if (request_Card.asset == null)
            {
                request_Card = Resources.LoadAsync<Card>($"Cards_Units/{data.PlayerDeck[i]}");
                yield return request_Card;
            }
            if (request_Card.asset == null)
            {
                request_Card = Resources.LoadAsync<Card>($"Cards_Actions/{data.PlayerDeck[i]}");
                yield return request_Card;
            }

            if (request_Card.asset == null)
            {
                Debug.LogError("Failed to load CARD asset!");
                continue;
            }

            if (request_Card.asset is UnitCard)
                Managers.CA_MAN.AddCard(request_Card.asset as UnitCard, pm);
            else if (request_Card.asset is ActionCard)
                Managers.CA_MAN.AddCard(request_Card.asset as ActionCard, pm);
        }
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #2

        // ITEMS
        pm.HeroItems.Clear();
        for (int i = 0; i < data.PlayerItems.Length; i++)
        {
            var request_Item = Resources.LoadAsync<HeroItem>($"Hero Items/{data.PlayerItems[i]}");
            yield return request_Item;

            if (request_Item.asset == null)
            {
                Debug.LogError("Failed to load ITEM asset!");
                continue;
            }

            pm.HeroItems.Add(request_Item.asset as HeroItem);
        }
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #3

        // CURRENT HOUR
        gm.CurrentHour = data.CurrentHour;

        // IS NEW HOUR
        gm.IsNewHour = data.IsNewHour;

        // CURRENT NARRATIVE
        if (data.CurrentNarrative != "")
        {
            var request_CurNar = Resources.LoadAsync<Narrative>($"Narratives/{data.CurrentNarrative}");
            yield return request_CurNar;

            if (request_CurNar.asset == null) Debug.LogError("Failed to load NARRATIVE asset!");
            else gm.CurrentNarrative = request_CurNar.asset as Narrative;
        }
        else gm.CurrentNarrative = null;
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #4

        // ENGAGED HERO
        if (data.EngagedHero != "")
        {
            var request_EngHero = Resources.LoadAsync<NPCHero>($"Heroes/NPC Heroes/{data.EngagedHero}");
            yield return request_EngHero;

            if (request_EngHero.asset == null) Debug.LogError("Failed to load HERO asset!");
            else Managers.D_MAN.EngagedHero =
                    Managers.G_MAN.GetActiveNPC(request_EngHero.asset as NPCHero);
        }
        else Managers.D_MAN.EngagedHero = null;
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #5

        // CURRENT LOCATION
        if (data.CurrentLocation != "")
        {
            var request_CurLoc = Resources.LoadAsync<Location>($"Locations/{data.CurrentLocation}");
            yield return request_CurLoc;

            if (request_CurLoc.asset == null) Debug.LogError("Failed to load LOCATION asset!");
            else Managers.G_MAN.CurrentLocation =
                    Managers.G_MAN.GetActiveLocation(request_CurLoc.asset as Location);
        }
        else Managers.G_MAN.CurrentLocation = null;
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #6

        // NPCS
        gm.ActiveNPCHeroes.Clear();
        for (int i = 0; i < data.NPCSAndClips.Length / 2; i++)
        {
            var request_NPC = Resources.LoadAsync<NPCHero>($"Heroes/NPC Heroes/{data.NPCSAndClips[i, 0]}");
            yield return request_NPC;

            if (request_NPC.asset == null)
            {
                Debug.LogError("Failed to load NPCHERO asset!");
                continue;
            }

            var npc = gm.GetActiveNPC(request_NPC.asset as NPCHero);
            var request_Clip = Resources.LoadAsync<DialogueClip>($"Dialogue/{npc.HeroName}/{data.NPCSAndClips[i, 1]}");
            yield return request_Clip;
            npc.NextDialogueClip = request_Clip.asset as DialogueClip;
        }
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #7

        // LOCATIONS
        gm.ActiveLocations.Clear();
        for (int i = 0; i < data.ActiveLocations.Length / 3; i++)
        {
            string name = data.ActiveLocations[i, 0];
            var request_Loc = Resources.LoadAsync<Location>($"Random Encounters/{name}");
            yield return request_Loc;
            if (request_Loc.asset == null) request_Loc = Resources.LoadAsync<Location>($"Locations/{name}");
            yield return request_Loc;

            if (request_Loc.asset == null)
            {
                Debug.LogError("Failed to load LOCATION asset!");
                continue;
            }

            var location = gm.GetActiveLocation(request_Loc.asset as Location);
            var request_NPC = Resources.LoadAsync<NPCHero>($"Heroes/NPC Heroes/{data.ActiveLocations[i, 1]}");
            yield return request_NPC;
            location.CurrentNPC = gm.GetActiveNPC(request_NPC.asset as NPCHero);
            location.CurrentObjective = data.ActiveLocations[i, 2];
        }
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #8

        gm.VisitedLocations.Clear();
        foreach (string location in data.VisitedLocations)
            gm.VisitedLocations.Add(location);

        // SHOP ITEMS
        gm.ShopItems.Clear();
        for (int i = 0; i < data.ShopItems.GetLength(0); i++)
        {
            var request_Item = Resources.LoadAsync<HeroItem>("Hero Items/" + data.ShopItems[i, 0]);
            yield return request_Item;



            string isUsed_Str = data.ShopItems[i, 1];
            bool isUsed = !string.IsNullOrEmpty(isUsed_Str) && bool.Parse(isUsed_Str);
            var itemAsset = request_Item.asset as HeroItem;
            itemAsset.IsUsed = isUsed;
            gm.ShopItems.Add(itemAsset);
        }
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #9

        // RECRUIT UNITS
        Managers.CA_MAN.PlayerRecruitUnits.Clear();
        for (int i = 0; i < data.RecruitUnits.Length; i++)
        {
            var request_Recruit = Resources.LoadAsync<UnitCard>("Cards_Units/" + data.RecruitUnits[i]);
            yield return request_Recruit;

            Managers.CA_MAN.PlayerRecruitUnits.Add(request_Recruit.asset as UnitCard);
        }
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #10

        // SHOP ACTIONS
        Managers.CA_MAN.ActionShopCards.Clear();
        for (int i = 0; i < data.ShopActions.Length; i++)
        {
            var request_Action = Resources.LoadAsync<ActionCard>("Cards_Actions/" + data.ShopActions[i]);
            yield return request_Action;
            Managers.CA_MAN.ActionShopCards.Add(request_Action.asset as ActionCard);
        }
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #11 [END non-combat loading]

        // LOYALTY
        gm.RecruitLoyalty = data.RecruitLoyalty;
        gm.ActionShopLoyalty = data.ActionShopLoyalty;
        gm.ShopLoyalty = data.ShopLoyalty;

        /*
         * <<< | COMBAT LOAD | >>>
         */

        if (!isCombatLoad)
        {
            SceneLoader.BackgroundLoadRoutine = null; // TESTING
            if (SceneLoader.SceneIsLoading) SceneLoader.LoadScene_Finish(SceneLoader.Scene.WorldMapScene); // TESTING
            yield break;
        }

        // >>> Some combat values are loaded in CombatManager.StartCombat() <<<

        // Reset combat data
        pm.ResetForCombat();
        em.ResetForCombat();

        // Turn Number
        pm.TurnNumber_Direct = data.TurnNumber_Player;
        em.TurnNumber_Direct = data.TurnNumber_Enemy;

        // Allies Destroyed
        pm.AlliesDestroyed_ThisTurn = data.AlliesDestroyed_ThisTurn_Player;
        em.AlliesDestroyed_ThisTurn = data.AlliesDestroyed_ThisTurn_Enemy;

        // Exploits Played
        pm.ExploitsPlayed = data.ExploitsPlayed_Player;
        em.ExploitsPlayed = data.ExploitsPlayed_Enemy;

        // Inventions Played
        pm.InventionsPlayed = data.InventionsPlayed_Player;
        em.InventionsPlayed = data.InventionsPlayed_Enemy;

        // Schemes Played
        pm.SchemesPlayed = data.SchemesPlayed_Player;
        em.SchemesPlayed = data.SchemesPlayed_Enemy;

        // Extractions Played
        pm.ExtractionsPlayed = data.ExtractionsPlayed_Player;

        // Give Next Effects
        yield return gm.StartCoroutine(LoadEffects(data.GiveNextEffects_Player, pm.GiveNextEffects));
        yield return gm.StartCoroutine(LoadEffects(data.GiveNextEffects_Enemy, em.GiveNextEffects));
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #12

        // Change Next Cost Effects
        yield return gm.StartCoroutine(LoadEffects(data.ChangeNextCostEffects_Player, pm.ChangeNextCostEffects));
        yield return gm.StartCoroutine(LoadEffects(data.ChangeNextCostEffects_Enemy, em.ChangeNextCostEffects));
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #13

        // Modify Next Effects
        yield return gm.StartCoroutine(LoadEffects(data.ModifyNextEffects_Player, pm.ModifyNextEffects));
        yield return gm.StartCoroutine(LoadEffects(data.ModifyNextEffects_Enemy, em.ModifyNextEffects));
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #14

        // Current Decks
        yield return gm.StartCoroutine(LoadCards(data.CurrentDeck_Player, pm.CurrentDeck));
        yield return gm.StartCoroutine(LoadCards(data.CurrentDeck_Enemy, em.CurrentDeck));
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #15

        // Hand Zone Cards
        yield return gm.StartCoroutine(LoadCards_Obj(data.HandZoneCards_Player, pm.HandZoneCards, Managers.P_MAN));
        yield return gm.StartCoroutine(LoadCards_Obj(data.HandZoneCards_Enemy, em.HandZoneCards, Managers.EN_MAN));
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #16

        // Play Zone Cards
        yield return gm.StartCoroutine(LoadCards_Obj(data.PlayZoneCards_Player, pm.PlayZoneCards, Managers.P_MAN));
        yield return gm.StartCoroutine(LoadCards_Obj(data.PlayZoneCards_Enemy, em.PlayZoneCards, Managers.EN_MAN));
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #17

        // Discard Zone Cards
        yield return gm.StartCoroutine(LoadCards(data.DiscardZoneCards_Player, pm.DiscardZoneCards));
        yield return gm.StartCoroutine(LoadCards(data.DiscardZoneCards_Enemy, em.DiscardZoneCards));
        yield return LoadSavedGame_PlayerData_Async_Progress(isCombatLoad); // <<< Load Progress #18 [END combat loading]

        /*
         * <<< | LOAD SCENE FINISH | >>>
         */

        PlayerData.SavedPlayerData = data;
        SceneLoader.BackgroundLoadRoutine = null; // TESTING
        if (SceneLoader.SceneIsLoading) SceneLoader.LoadScene_Finish(SceneLoader.Scene.CombatScene);

        /*
         * <<< | LOAD METHODS | >>>
         */

        IEnumerator LoadCards_Obj(string[][][][] cards, List<GameObject> newCards_Obj, HeroManager cardOwner)
        {
            newCards_Obj.Clear();
            List<Card> newCards = new();

            yield return LoadCards(cards, newCards);

            int i = 0;
            foreach (var card in newCards)
            {
                var card_Obj = Managers.CA_MAN.ShowCard(card, new Vector2(9999, 9999),
                    CardManager.DisplayType.Default, card.BanishAfterPlay);

                // Card Tag
                card_Obj.tag = cardOwner.CARD_TAG;

                if (card_Obj.TryGetComponent(out UnitCardDisplay ucd))
                {
                    // IsExhausted
                    ucd.IsExhausted = bool.Parse(cards[i][0][1][3]);

                    // Trigger Counts
                    List<string> enabledTriggers = new();
                    bool hasEnabledModifiers = false;

                    foreach (var ca in ucd.CurrentAbilities)
                    {
                        if (ca is TriggeredAbility tra)
                        {
                            if (tra.TriggerLimit == 0 || tra.TriggerCount < tra.TriggerLimit)
                                enabledTriggers.Add(tra.AbilityTrigger.AbilityName);
                        }
                        else if (ca is ModifierAbility ma)
                        {
                            if (ma.TriggerLimit == 0 || ma.TriggerCount < ma.TriggerLimit)
                                hasEnabledModifiers = true;
                        }
                    }

                    foreach (var ca in ucd.CurrentAbilities)
                    {
                        if (ca is TriggeredAbility tra &&
                            (enabledTriggers.FindIndex(x => x == tra.AbilityTrigger.AbilityName) == -1))
                            ucd.EnableTriggerIcon(tra.AbilityTrigger, false);
                        else if (ca is ModifierAbility ma && !hasEnabledModifiers)
                            ucd.EnableTriggerIcon(null, false);
                    }
                }

                newCards_Obj.Add(card_Obj);
                i++;
            }
        }

        IEnumerator LoadCards(string[][][][] cards, List<Card> newCards)
        {
            newCards.Clear();

            for (int i = 0; i < cards.GetLength(0); i++)
            {
                var savedCard = cards[i];
                string cardName = savedCard[0][0][0];
                ResourceRequest LoadResource(string path) => Resources.LoadAsync<Card>($"{path}/{cardName}");

                ResourceRequest request_Card = null;
                var assetPaths = new string[]
                {
                    "Cards_Starting",
                    "Cards_Units",
                    "Cards_Actions",
                    "Cards_Created",
                    "Cards_Created_Unique",
                };

                int tryPathIdx = 0;
                while ((tryPathIdx < assetPaths.Length) && (request_Card == null || request_Card.asset == null))
                {
                    string tryPath = assetPaths[tryPathIdx++];
                    request_Card = LoadResource(tryPath);
                    yield return request_Card;
                }

                if (request_Card.asset == null)
                {
                    Debug.LogError($"Failed to load CARD asset <{cardName}>!");
                    continue;
                }

                var cardAsset = request_Card.asset as Card;
                var newCard = ScriptableObject.CreateInstance(cardAsset.GetType()) as Card;
                newCard.LoadCard(cardAsset);
                newCards.Add(newCard);

                /** |Universal| **/
                // Cost
                newCard.CurrentEnergyCost = int.Parse(savedCard[0][0][1]);
                // BanishAfterPlay
                newCard.BanishAfterPlay = bool.Parse(savedCard[0][0][2]);
                // CurrentEffects
                yield return LoadEffects(savedCard[1], newCard.CurrentEffects);
                // PermanentEffects
                yield return LoadEffects(savedCard[2], newCard.PermanentEffects);

                /** |Units| **/
                if (newCard is not UnitCard newUC) continue;

                if (string.IsNullOrEmpty(savedCard[0][1][0]))
                {
                    Debug.LogError("UNIT VALUES NOT FOUND!");
                    continue;
                }

                // Power
                newUC.CurrentPower = int.Parse(savedCard[0][1][0]);
                // Health
                newUC.CurrentHealth = int.Parse(savedCard[0][1][1]);
                // MaxHealth
                newUC.MaxHealth = int.Parse(savedCard[0][1][2]);

                // CurrentAbilities
                yield return LoadAbilities(savedCard[3], newUC.CurrentAbilities);
            }
        }

        IEnumerator LoadAbilities(string[][] abilities, List<CardAbility> newAbilities)
        {
            newAbilities.Clear();

            for (int i = 0; i < abilities.GetLength(0); i++)
            {
                var savedAbility = abilities[i];
                string abilityName = savedAbility[0];

                var request_Ability = Resources.LoadAsync<CardAbility>($"Card Abilities/{abilityName}");
                yield return request_Ability;

                if (request_Ability.asset == null)
                {
                    Debug.LogError($"Failed to load ABILITY asset <{abilityName}>!");
                    continue;
                }

                var abilityAsset = request_Ability.asset as CardAbility;
                var newAbility = ScriptableObject.CreateInstance(abilityAsset.GetType()) as CardAbility;
                newAbility.LoadCardAbility(abilityAsset);
                newAbilities.Add(newAbility);
                // Trigger Count
                if (newAbility is TriggeredAbility tra) tra.TriggerCount = int.Parse(savedAbility[1]);
            }
        }

        IEnumerator LoadEffects<TEffect>(string[][] effects, List<TEffect> newEffects) where TEffect : Effect
        {
            newEffects.Clear();

            for (int i = 0; i < effects.GetLength(0); i++)
            {
                var savedEffect = effects[i];
                string effectName = savedEffect[0];

                var request_Effect = Resources.LoadAsync<Effect>($"Effects/{effectName}");
                yield return request_Effect;

                if (request_Effect.asset == null)
                {
                    Debug.LogError($"Failed to load EFFECT asset <{effectName}>!");
                    continue;
                }

                var effectAsset = request_Effect.asset as TEffect;
                var newEffect = ScriptableObject.CreateInstance(effectAsset.GetType()) as TEffect;
                newEffect.LoadEffect(effectAsset);
                newEffects.Add(newEffect);
                // Countdown
                newEffect.Countdown = int.Parse(savedEffect[1]);
                // Multiplier
                if (newEffect is MultiplierEffect mle) mle.Multiplier = int.Parse(savedEffect[2]);
                // StatChange
                if (newEffect is StatChangeEffect sce)
                {
                    sce.PowerChange = int.Parse(savedEffect[3]);
                    sce.HealthChange = int.Parse(savedEffect[4]);
                }
            }
        }
    }

    /******
     * *****
     * ****** SAVE_GAME
     * *****
     *****/
    public static bool CheckSave() => SaveLoad.LoadGame(SaveLoad.SaveType.Player) != null;
    public static void SaveGame_GameData()
    {
        var unlockedHeroes = Managers.G_MAN.UnlockedHeroes.ToArray();
        var unlockedPowers = Managers.G_MAN.UnlockedPowers.ToArray();

        // Augments
        var augments = new string[Managers.P_MAN.HeroAugments.Count];
        for (int i = 0; i < augments.Length; i++)
            augments[i] = Managers.P_MAN.HeroAugments[i].AugmentName;

        // Reputation
        var gm = Managers.G_MAN;
        int repMag = gm.Reputation_Mages;
        int repMut = gm.Reputation_Mutants;
        int repRog = gm.Reputation_Rogues;
        int repTec = gm.Reputation_Techs;
        int repWar = gm.Reputation_Warriors;

        GameData data = new(unlockedHeroes, unlockedPowers, augments,
            repMag, repMut, repRog, repTec, repWar,
            gm.TutorialActive_WorldMap);
        SaveLoad.SaveGame(data, SaveLoad.SaveType.Game);
    }
    public static void SaveGame() // PUBLIC FOR BETA ONLY
    {
        var gm = Managers.G_MAN;
        var pm = Managers.P_MAN;
        var em = Managers.EN_MAN;

        SaveGame_GameData();

        /*
         * <<< | UNIVERSAL SAVE | >>>
         */

        // Player Hero
        var pHero = pm.HeroScript as PlayerHero;
        string name_ph = pHero.HeroName;
        int health_ph = pm.CurrentHealth;
        int aether = pm.CurrentAether;
        string power_ph = GetObjectName(pHero.CurrentHeroPower);
        string ultimate = GetObjectName(pHero.CurrentHeroUltimate);

        // Decklist
        var deckList = new string[pm.DeckList.Count];
        for (int i = 0; i < deckList.Length; i++)
            deckList[i] = pm.DeckList[i].CardName;

        // Items
        var items = new string[pm.HeroItems.Count];
        for (int i = 0; i < items.Length; i++)
            items[i] = pm.HeroItems[i].ItemName;

        // Hour, Visited Locations
        int currentHour = gm.CurrentHour;
        bool isNewHour = gm.IsNewHour;
        var visitedLocations = gm.VisitedLocations.ToArray();

        // Current Narrative
        string narrativeName = "";
        if (gm.CurrentNarrative != null) narrativeName = GetObjectName(gm.CurrentNarrative);

        // Engaged Hero and Current Location
        string engagedHero = "";
        if (Managers.D_MAN.EngagedHero != null) engagedHero = Managers.D_MAN.EngagedHero.HeroName;
        string currentLocation = "";
        if (Managers.G_MAN.CurrentLocation != null) currentLocation = Managers.G_MAN.CurrentLocation.LocationName;

        // NPCSAndClips
        var npcsAndClips = new string[gm.ActiveNPCHeroes.Count, 2];
        for (int i = 0; i < npcsAndClips.Length / 2; i++)
        {
            npcsAndClips[i, 0] = gm.ActiveNPCHeroes[i].HeroName;
            var clip = gm.ActiveNPCHeroes[i].NextDialogueClip;
            npcsAndClips[i, 1] = GetObjectName(clip);
        }

        // Active Locations
        var activeLocations = new string[gm.ActiveLocations.Count, 3];
        for (int i = 0; i < activeLocations.GetLength(0); i++) // OLD CODE: i < activeLocations.Length / 3
        {
            activeLocations[i, 0] = gm.ActiveLocations[i].LocationName;
            activeLocations[i, 1] = gm.ActiveLocations[i].CurrentNPC.HeroName;
            activeLocations[i, 2] = gm.ActiveLocations[i].CurrentObjective;
        }

        // Shop Items
        var shopItems = new string[gm.ShopItems.Count, 2];
        for (int i = 0; i < shopItems.GetLength(0); i++)
        {
            shopItems[i, 0] = gm.ShopItems[i].ItemName;
            shopItems[i, 1] = gm.ShopItems[i].IsUsed.ToString();
        }

        // Recruit Units
        var recruitUnits = new string[Managers.CA_MAN.PlayerRecruitUnits.Count];
        for (int i = 0; i < recruitUnits.Length; i++)
            recruitUnits[i] = Managers.CA_MAN.PlayerRecruitUnits[i].CardName;

        // Shop Actions
        var shopActions = new string[Managers.CA_MAN.ActionShopCards.Count];
        for (int i = 0; i < shopActions.Length; i++)
            shopActions[i] = Managers.CA_MAN.ActionShopCards[i].CardName;

        // Loyalty
        int recLoyal = gm.RecruitLoyalty;
        int actLoyal = gm.ActionShopLoyalty;
        int shoLoyal = gm.ShopLoyalty;

        /*
         * <<< | COMBAT SAVE | >>>
         */

        string saveScene = SceneManager.GetActiveScene().name;
        bool isCombatSave = saveScene == SceneLoader.Scene.CombatScene.ToString();

        // Enemy Hero
        var enHero = isCombatSave ? em.HeroScript as EnemyHero : null;
        string name_eh = isCombatSave ? enHero.HeroName : "";
        string heroTurn = isCombatSave ? (pm.IsMyTurn ? PlayerData.TURN_PLAYER : PlayerData.TURN_ENEMY) : "";
        
        // Turn Numbers
        int turn_ph = isCombatSave ? pm.TurnNumber : 0;
        int turn_eh = isCombatSave ? em.TurnNumber : 0;

        // Enemy Health
        int health_eh = isCombatSave ? em.CurrentHealth : 0;

        // Energy Per Turn
        int energyPer_ph = isCombatSave ? pm.EnergyPerTurn : 0;
        int energyPer_eh = isCombatSave ? em.EnergyPerTurn : 0;

        // Current Energy
        int energyCur_ph = isCombatSave ? pm.CurrentEnergy : 0;
        int energyCur_eh = isCombatSave ? em.CurrentEnergy : 0;

        // Ultimate Progress
        int ultProg = isCombatSave ? pm.HeroUltimateProgress : 0;

        // Allies Destroyed
        int allDest_ph = isCombatSave ? pm.AlliesDestroyed_ThisTurn : 0;
        int allDest_eh = isCombatSave ? em.AlliesDestroyed_ThisTurn : 0;

        // Damage Taken
        int dmgTak_ph = isCombatSave ? pm.DamageTaken_ThisTurn : 0;
        int dmgTak_eh = isCombatSave ? em.DamageTaken_ThisTurn : 0;

        // Exploits
        int exploits_ph = isCombatSave ? pm.ExploitsPlayed : 0;
        int exploits_eh = isCombatSave ? em.ExploitsPlayed : 0;

        // Inventions
        int inventions_ph = isCombatSave ? pm.ExploitsPlayed : 0;
        int inventions_eh = isCombatSave ? em.ExploitsPlayed : 0;

        // Schemes
        int schemes_ph = isCombatSave ? pm.ExploitsPlayed : 0;
        int schemes_eh = isCombatSave ? em.ExploitsPlayed : 0;

        // Extractions
        int extractions_ph = isCombatSave ? pm.ExtractionsPlayed : 0;

        // Give Next Effects
        var giveNextEffects_Player = isCombatSave ? SaveEffects(pm.GiveNextEffects.ToArray()) : null;
        var giveNextEffects_Enemy = isCombatSave ? SaveEffects(em.GiveNextEffects.ToArray()) : null;

        // Change Next Cost Effects
        var changeNextCostEffects_Player = isCombatSave ? SaveEffects(pm.ChangeNextCostEffects.ToArray()) : null;
        var changeNextCostEffects_Enemy = isCombatSave ? SaveEffects(em.ChangeNextCostEffects.ToArray()) : null;

        // Modify Next Effects
        var modifyNextEffects_Player = isCombatSave ? SaveEffects(pm.ModifyNextEffects.ToArray()) : null;
        var modifyNextEffects_Enemy = isCombatSave ? SaveEffects(em.ModifyNextEffects.ToArray()) : null;

        // Current Deck
        var currentDeck_Player = isCombatSave ? SaveCards(pm.CurrentDeck) : null;
        var currentDeck_Enemy = isCombatSave ? SaveCards(em.CurrentDeck) : null;

        // Hand Zone Cards
        var handZoneCards_Player = isCombatSave ? SaveCards_Obj(pm.HandZoneCards) : null;
        var handZoneCards_Enemy = isCombatSave ? SaveCards_Obj(em.HandZoneCards) : null;

        // Play Zone Cards
        var playZoneCards_Player = isCombatSave ? SaveCards_Obj(pm.PlayZoneCards) : null;
        var playZoneCards_Enemy = isCombatSave ? SaveCards_Obj(em.PlayZoneCards) : null;

        // Discard Zone Cards
        var discardZoneCards_Player = isCombatSave ? SaveCards(pm.DiscardZoneCards) : null;
        var discardZoneCards_Enemy = isCombatSave ? SaveCards(em.DiscardZoneCards) : null;

        PlayerData data = new(
            
            /*
             * <<< | UNIVERSAL SAVE | >>>
             */

            // Save Scene
            saveScene,
            // Player Hero
            name_ph, health_ph, aether,
            // Power and Ultimate
            power_ph, ultimate,
            // Player Hero "Inventory"
            deckList, items,
            // Hour and Narrative
            currentHour, isNewHour, narrativeName,
            // Engaged Hero and Current Location
            engagedHero, currentLocation,
            // Locations
            visitedLocations, activeLocations,
            // Dialogue
            npcsAndClips,
            // Shops
            shopItems, recruitUnits, shopActions,
            // Loyalty
            recLoyal, actLoyal, shoLoyal,

            /*
             * <<< | COMBAT SAVE | >>>
             */

            // Enemy Hero
            name_eh,
            // Hero Turn
            heroTurn,
            // Turn Number
            turn_ph, turn_eh,
            // Enemy Health
            health_eh,
            // Energy Per Turn
            energyPer_ph, energyPer_eh,
            // Current Energy
            energyCur_ph, energyCur_eh,
            // Hero Ultimate Progress
            ultProg,
            // AlliesDestroyed_ThisTurn
            allDest_ph, allDest_eh,
            // DamageTaken_ThisTurn
            dmgTak_ph, dmgTak_eh,
            // Exploits Played
            exploits_ph, exploits_eh,
            // Inventions Played
            inventions_ph, inventions_eh,
            // Schemes Played
            schemes_ph, schemes_eh,
            // Extractions Played
            extractions_ph,
            // GiveNextEffects
            giveNextEffects_Player, giveNextEffects_Enemy,
            // ChangeNextCostEffects
            changeNextCostEffects_Player, changeNextCostEffects_Enemy,
            // ModifyNextEffects
            modifyNextEffects_Player, modifyNextEffects_Enemy,
            // Current Deck
            currentDeck_Player, currentDeck_Enemy,
            // Hand Zone Cards,
            handZoneCards_Player, handZoneCards_Enemy,
            // Play Zone Cards
            playZoneCards_Player, playZoneCards_Enemy,
            // Discard Zone cards
            discardZoneCards_Player, discardZoneCards_Enemy
            );

        SaveLoad.SaveGame(data, SaveLoad.SaveType.Player);

        /*
         * <<< | SAVE METHODS | >>>
         */

        // Card Saving
        static string[][][][] SaveCards_Obj(List<GameObject> cards)
        {
            var newCards = new List<Card>();
            foreach (var card in cards)
                newCards.Add(card.GetComponent<CardDisplay>().CardScript);
            var savedCards = SaveCards(newCards);

            for (int i = 0; i < savedCards.GetLength(0); i++)
            {
                if (!cards[i].TryGetComponent(out UnitCardDisplay ucd)) continue;
                savedCards[i][0][1][3] = ucd.IsExhausted.ToString();
            }
            return savedCards;
        }

        static string[][][][] SaveCards(List<Card> cards)
        {
            var newCards = new string[cards.Count][][][];
            for (int i = 0; i < cards.Count; i++)
            {
                newCards[i] = new string[4][][];
                newCards[i][0] = new string[2][];
                newCards[i][0][0] = new string[3];
                newCards[i][0][1] = new string[4];

                var newCard = newCards[i];
                var loadCard = cards[i];

                /** |Universal| **/
                // Name
                newCard[0][0][0] = loadCard.CardName;
                // Cost
                newCard[0][0][1] = loadCard.CurrentEnergyCost.ToString();
                // BanishAfterPlay
                newCard[0][0][2] = loadCard.BanishAfterPlay.ToString();
                // CurrentEffects
                newCard[1] = SaveEffects(loadCard.CurrentEffects.ToArray());
                // PermanentEffects
                newCard[2] = SaveEffects(loadCard.PermanentEffects.ToArray());

                /** |Units| **/
                if (loadCard is not UnitCard loadUC) continue;

                // Power
                newCard[0][1][0] = loadUC.CurrentPower.ToString();
                // Health
                newCard[0][1][1] = loadUC.CurrentHealth.ToString();
                // MaxHealth
                newCard[0][1][2] = loadUC.MaxHealth.ToString();

                // Current Abilities
                newCard[3] = SaveAbilities(loadUC.CurrentAbilities.ToArray());
            }
            return newCards;
        }

        // Ability Saving
        static string[][] SaveAbilities(CardAbility[] abilities)
        {
            var newAbilities = new string[abilities.Length][];
            for (int i = 0; i < abilities.Length; i++)
            {
                newAbilities[i] = new string[2];

                var newAbility = newAbilities[i];
                var loadAbility = abilities[i];

                // Name
                newAbility[0] = GetObjectName(loadAbility);
                // TriggerCount
                if (loadAbility is TriggeredAbility tra) newAbility[1] = tra.TriggerCount.ToString();
            }
            return newAbilities;
        }

        // Effect Saving
        static string[][] SaveEffects(Effect[] effects)
        {
            var newEffects = new string[effects.Length][];
            for (int i = 0; i < effects.Length; i++)
            {
                newEffects[i] = new string[5];

                var newEffect = newEffects[i];
                var loadEffect = effects[i];

                // Name
                newEffect[0] = GetObjectName(loadEffect);
                // Countdown
                newEffect[1] = loadEffect.Countdown.ToString();
                // Multiplier
                if (loadEffect is MultiplierEffect me) newEffect[2] = me.Multiplier.ToString();
                // StatChange
                if (loadEffect is StatChangeEffect sce)
                {
                    newEffect[3] = sce.PowerChange.ToString();
                    newEffect[4] = sce.HealthChange.ToString();
                }
            }
            return newEffects;
        }
    }
}
