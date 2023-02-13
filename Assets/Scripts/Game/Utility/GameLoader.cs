using System.Collections;
using UnityEngine;

public static class GameLoader
{
    // Player Preferences
    public const string HIDE_EXPLICIT_LANGUAGE = "HideExplicitLanguage";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";

    // Narratives
    public static Narrative SettingNarrative { get; set; }
    public static Narrative NewGameNarrative { get; set; }

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

    public static void Tutorial_Load()
    {
        Managers.P_MAN.AetherCells = 0;
        string playerHeroName = "Kili, Neon Rider";
        string enemyHeroName = "Tiny Mutant";
        Hero[] heroes = Resources.LoadAll<Hero>("Tutorial");
        EnemyHero enemyHero = ScriptableObject.CreateInstance<EnemyHero>();

        foreach (Hero hero in heroes)
        {
            if (hero.HeroName == playerHeroName) Managers.P_MAN.HeroScript = hero as PlayerHero;
            else if (hero.HeroName == enemyHeroName)
            {
                enemyHero.LoadHero(hero);
                Managers.D_MAN.EngagedHero = enemyHero;
            }
        }

        foreach (UnitCard unit in Managers.CA_MAN.TutorialPlayerUnits)
            for (int i = 0; i < 5; i++)
                Managers.CA_MAN.AddCard(unit, Managers.P_MAN);
    }

    /******
     * *****
     * ****** NEW_GAME
     * *****
     *****/
    private static void NewGame_LoadAsync_Progress()
    {
        const int loadItems = 8;
        const float increment = 1f / loadItems;
        SceneLoader.LoadingProgress += increment;
    }
    public static IEnumerator NewGame_LoadAsync()
    {
        // Narratives
        string narPath = "Narratives";
        var request_setting = Resources.LoadAsync<Narrative>($"{narPath}/SettingNarrative");
        yield return request_setting;
        NewGame_LoadAsync_Progress(); // <<< Load Progress

        var request_narrative = Resources.LoadAsync<Narrative>($"{narPath}/NewGameNarrative");
        yield return request_narrative;
        NewGame_LoadAsync_Progress(); // <<< Load Progress

        SettingNarrative = request_setting.asset as Narrative;
        NewGameNarrative = request_narrative.asset as Narrative;
        Managers.G_MAN.CurrentNarrative = SettingNarrative;

        // Locations
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
            var request = Resources.LoadAsync<Location>($"Locations/{loc}");
            yield return request;
            NewGame_LoadAsync_Progress(); // <<< Load Progress (6x)

            Location location = request.asset as Location;
            Managers.G_MAN.GetActiveLocation(location);
        }

        Managers.G_MAN.CurrentHour = 4;
        Managers.G_MAN.IsNewHour = true;
        Managers.G_MAN.IsTutorial = false;
        Managers.CA_MAN.LoadNewRecruits();
        Managers.CA_MAN.LoadNewActions();
        Managers.G_MAN.ShopItems = Managers.G_MAN.GetShopItems();
        Managers.G_MAN.RecruitLoyalty = 3; // First recruit free
        Managers.G_MAN.ActionShopLoyalty = 3; // First action free
        Managers.G_MAN.ShopLoyalty = 3; // First item free
        Managers.P_MAN.AetherCells = GameManager.PLAYER_START_AETHER;
        Managers.P_MAN.DeckList.Clear();

        // REPUTATION
        int startRep = GameManager.PLAYER_START_UNITS;
        Managers.G_MAN.Reputation_Mages = startRep;
        Managers.G_MAN.Reputation_Mutants = startRep;
        Managers.G_MAN.Reputation_Rogues = startRep;
        Managers.G_MAN.Reputation_Techs = startRep;
        Managers.G_MAN.Reputation_Warriors = startRep;

        SceneLoader.LoadScene_Finish(SceneLoader.Scene.NarrativeScene);
    }

    /******
     * *****
     * ****** SAVE_GAME
     * *****
     *****/
    public static bool CheckSave() => SaveLoad.LoadGame() != null;
    public static void SaveGame() // PUBLIC FOR BETA ONLY
    {
        string[] deckList = new string[Managers.P_MAN.DeckList.Count];
        for (int i = 0; i < deckList.Length; i++)
            deckList[i] = Managers.P_MAN.DeckList[i].CardName;

        string[] augments = new string[Managers.P_MAN.HeroAugments.Count];
        for (int i = 0; i < augments.Length; i++)
            augments[i] = Managers.P_MAN.HeroAugments[i].AugmentName;

        string[] items = new string[Managers.P_MAN.HeroItems.Count];
        for (int i = 0; i < items.Length; i++)
            items[i] = Managers.P_MAN.HeroItems[i].ItemName;

        string[,] npcsAndClips = new string[Managers.G_MAN.ActiveNPCHeroes.Count, 2];
        for (int i = 0; i < npcsAndClips.Length / 2; i++)
        {
            npcsAndClips[i, 0] = Managers.G_MAN.ActiveNPCHeroes[i].HeroName;

            DialogueClip clip = Managers.G_MAN.ActiveNPCHeroes[i].NextDialogueClip;
            string clipName = clip.ToString();
            clipName = clipName.Replace($" ({clip.GetType().Name})", "");
            npcsAndClips[i, 1] = clipName;
        }

        string[,] locationsNPCsObjectives = new string[Managers.G_MAN.ActiveLocations.Count, 3];
        for (int i = 0; i < locationsNPCsObjectives.Length / 3; i++)
        {
            locationsNPCsObjectives[i, 0] = Managers.G_MAN.ActiveLocations[i].LocationName;
            locationsNPCsObjectives[i, 1] = Managers.G_MAN.ActiveLocations[i].CurrentNPC.HeroName;
            locationsNPCsObjectives[i, 2] = Managers.G_MAN.ActiveLocations[i].CurrentObjective;
        }

        string[] shopItems = new string[Managers.G_MAN.ShopItems.Count];
        for (int i = 0; i < shopItems.Length; i++)
            shopItems[i] = Managers.G_MAN.ShopItems[i].ItemName;

        string[] recruitUnits = new string[Managers.CA_MAN.PlayerRecruitUnits.Count];
        for (int i = 0; i < recruitUnits.Length; i++)
            recruitUnits[i] = Managers.CA_MAN.PlayerRecruitUnits[i].CardName;

        string[] shopActions = new string[Managers.CA_MAN.ActionShopCards.Count];
        for (int i = 0; i < shopActions.Length; i++)
            shopActions[i] = Managers.CA_MAN.ActionShopCards[i].CardName;

        string narrativeName = "";
        if (Managers.G_MAN.CurrentNarrative != null)
        {
            narrativeName = Managers.G_MAN.CurrentNarrative.ToString();
            narrativeName = narrativeName.Replace(" (Narrative)", "");
        }

        GameData data = new(Managers.G_MAN.CurrentHour, narrativeName, Managers.P_MAN.HeroScript.HeroName,
            deckList, augments, items, Managers.P_MAN.AetherCells,
            npcsAndClips, locationsNPCsObjectives, Managers.G_MAN.VisitedLocations.ToArray(),
            shopItems, recruitUnits, shopActions,
            // Loyalty
            Managers.G_MAN.RecruitLoyalty, Managers.G_MAN.ActionShopLoyalty, Managers.G_MAN.ShopLoyalty,
            // Reputation
            Managers.G_MAN.Reputation_Mages, Managers.G_MAN.Reputation_Mutants, Managers.G_MAN.Reputation_Rogues, 
            Managers.G_MAN.Reputation_Techs, Managers.G_MAN.Reputation_Warriors);

        SaveLoad.SaveGame(data);
    }

    /******
     * *****
     * ****** LOAD_GAME
     * *****
     *****/
    private static void LoadGame_Async_Progress()
    {
        const int loadItems = 10;
        const float increment = 1f / loadItems;
        SceneLoader.LoadingProgress += increment;
    }
    public static IEnumerator LoadGame_Async()
    {
        GameData data = SaveLoad.LoadGame();

        if (data == null)
        {
            Debug.LogError("SAVE DATA IS NULL!");
            yield return null;
        }

        // CURRENT HOUR
        Managers.G_MAN.CurrentHour = data.CurrentHour;

        // CURRENT NARRATIVE
        if (data.CurrentNarrative != "")
        {
            var request_CurNar = Resources.LoadAsync<Narrative>($"Narratives/{data.CurrentNarrative}");
            yield return request_CurNar;
            LoadGame_Async_Progress(); // <<< Load Progress

            if (request_CurNar.asset != null) Managers.G_MAN.CurrentNarrative = request_CurNar.asset as Narrative;
            else Debug.LogError("NARRATIVE NOT FOUND!");
        }
        else Managers.G_MAN.CurrentNarrative = null;

        var request_PHero = Resources.LoadAsync<PlayerHero>($"Heroes/Player Heroes/{data.PlayerHero}");
        yield return request_PHero;
        LoadGame_Async_Progress(); // <<< Load Progress

        if (request_PHero.asset != null) Managers.P_MAN.HeroScript = request_PHero.asset as PlayerHero;
        else Debug.LogError("HERO NOT FOUND!");

        // DECK LIST
        Managers.P_MAN.DeckList.Clear();
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
                Debug.LogError("CARD NOT FOUND!");
                continue;
            }

            if (request_Card.asset is UnitCard)
                Managers.CA_MAN.AddCard(request_Card.asset as UnitCard, Managers.P_MAN);
            else if (request_Card.asset is ActionCard)
                Managers.CA_MAN.AddCard(request_Card.asset as ActionCard, Managers.P_MAN);
            else Debug.LogError("INVALID CARD TYPE!");
        }
        LoadGame_Async_Progress(); // <<< Load Progress

        // AUGMENTS
        Managers.P_MAN.HeroAugments.Clear();
        for (int i = 0; i < data.PlayerAugments.Length; i++)
        {
            var request_Aug = Resources.LoadAsync<HeroAugment>($"Hero Augments/{data.PlayerAugments[i]}");
            yield return request_Aug;
            if (request_Aug.asset != null) Managers.P_MAN.HeroAugments.Add(request_Aug.asset as HeroAugment);
            else Debug.LogError("AUGMENT NOT FOUND!");
        }
        LoadGame_Async_Progress(); // <<< Load Progress

        // ITEMS
        Managers.P_MAN.HeroItems.Clear();
        for (int i = 0; i < data.PlayerItems.Length; i++)
        {
            var request_Item = Resources.LoadAsync<HeroItem>($"Hero Items/{data.PlayerItems[i]}");
            yield return request_Item;
            if (request_Item.asset != null) Managers.P_MAN.HeroItems.Add(request_Item.asset as HeroItem);
            else Debug.LogError("ITEM NOT FOUND!");
        }
        LoadGame_Async_Progress(); // <<< Load Progress

        // AETHER CELLS
        Managers.P_MAN.AetherCells = data.AetherCells;

        // NPCS
        Managers.G_MAN.ActiveNPCHeroes.Clear();
        for (int i = 0; i < data.NPCSAndClips.Length / 2; i++)
        {
            var request_NPC = Resources.LoadAsync<NPCHero>($"Heroes/NPC Heroes/{data.NPCSAndClips[i, 0]}");
            yield return request_NPC;
            if (request_NPC.asset == null)
            {
                Debug.LogError("NPC NOT FOUND!");
                continue;
            }
            NPCHero npc = Managers.G_MAN.GetActiveNPC(request_NPC.asset as NPCHero);
            var request_Clip = Resources.LoadAsync<DialogueClip>($"Dialogue/{npc.HeroName}/{data.NPCSAndClips[i, 1]}");
            yield return request_Clip;
            npc.NextDialogueClip = request_Clip.asset as DialogueClip;
        }
        LoadGame_Async_Progress(); // <<< Load Progress

        // LOCATIONS
        Managers.G_MAN.ActiveLocations.Clear();
        for (int i = 0; i < data.LocationsNPCsObjectives.Length / 3; i++)
        {
            string name = data.LocationsNPCsObjectives[i, 0];
            var request_Loc = Resources.LoadAsync<Location>($"Random Encounters/{name}");
            yield return request_Loc;
            if (request_Loc.asset == null) request_Loc = Resources.LoadAsync<Location>($"Locations/{name}");
            yield return request_Loc;
            if (request_Loc.asset == null)
            {
                Debug.LogError("LOCATION NOT FOUND!");
                continue;
            }
            Location location = Managers.G_MAN.GetActiveLocation(request_Loc.asset as Location);
            var request_NPC = Resources.LoadAsync<NPCHero>($"Heroes/NPC Heroes/{data.LocationsNPCsObjectives[i, 1]}");
            yield return request_NPC;
            location.CurrentNPC = Managers.G_MAN.GetActiveNPC(request_NPC.asset as NPCHero);
            location.CurrentObjective = data.LocationsNPCsObjectives[i, 2];
        }
        LoadGame_Async_Progress(); // <<< Load Progress

        Managers.G_MAN.VisitedLocations.Clear();
        foreach (string location in data.VisitedLocations)
            Managers.G_MAN.VisitedLocations.Add(location);

        // SHOP ITEMS
        Managers.G_MAN.ShopItems.Clear();
        for (int i = 0; i < data.ShopItems.Length; i++)
        {
            var request_Item = Resources.LoadAsync<HeroItem>("Hero Items/" + data.ShopItems[i]);
            yield return request_Item;
            Managers.G_MAN.ShopItems.Add(request_Item.asset as HeroItem);
        }
        LoadGame_Async_Progress(); // <<< Load Progress

        // RECRUIT UNITS
        Managers.CA_MAN.PlayerRecruitUnits.Clear();
        for (int i = 0; i < data.RecruitUnits.Length; i++)
        {
            var request_Recruit = Resources.LoadAsync<UnitCard>("Cards_Units/" + data.RecruitUnits[i]);
            yield return request_Recruit;
            Managers.CA_MAN.PlayerRecruitUnits.Add(request_Recruit.asset as UnitCard);
        }
        LoadGame_Async_Progress(); // <<< Load Progress

        // SHOP ACTIONS
        Managers.CA_MAN.ActionShopCards.Clear();
        for (int i = 0; i < data.ShopActions.Length; i++)
        {
            var request_Action = Resources.LoadAsync<ActionCard>("Cards_Actions/" + data.ShopActions[i]);
            yield return request_Action;
            Managers.CA_MAN.ActionShopCards.Add(request_Action.asset as ActionCard);
        }
        LoadGame_Async_Progress(); // <<< Load Progress

        // LOYALTY
        Managers.G_MAN.RecruitLoyalty = data.RecruitLoyalty;
        Managers.G_MAN.ActionShopLoyalty = data.ActionShopLoyalty;
        Managers.G_MAN.ShopLoyalty = data.ShopLoyalty;

        // REPUTATION
        Managers.G_MAN.Reputation_Mages = data.Reputation_Mages;
        Managers.G_MAN.Reputation_Mutants = data.Reputation_Mutants;
        Managers.G_MAN.Reputation_Rogues = data.Reputation_Rogues;
        Managers.G_MAN.Reputation_Techs = data.Reputation_Techs;
        Managers.G_MAN.Reputation_Warriors = data.Reputation_Warriors;

        SceneLoader.LoadScene_Finish(SceneLoader.Scene.WorldMapScene);
    }
}
