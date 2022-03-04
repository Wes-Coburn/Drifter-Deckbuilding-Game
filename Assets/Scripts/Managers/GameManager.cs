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

    [Header("NARRATIVES")]
    [SerializeField] private Narrative settingNarrative;
    [SerializeField] private Narrative newGameNarrative;
    [Header("LOCATIONS")]
    [SerializeField] private Location homeBaseLocation;
    [SerializeField] private Location firstLocation;
    [SerializeField] GameObject locationIconPrefab;
    [Header("MULLIGAN EFFECT")]
    [SerializeField] private EffectGroup mulliganEffect;
    [Header("LOADING TIPS")]
    [SerializeField] [TextArea] private string[] loadingTips;

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
    public bool IsCombatTest { get; set; }
    public bool HideExplicitLanguage { get; set; }
    public Narrative CurrentNarrative { get; set; }
    public List<NPCHero> ActiveNPCHeroes { get; private set; }
    public List<Location> ActiveLocations { get; private set; }
    public List<string> VisitedLocations { get; private set; }
    public Location CurrentLocation { get; set; }
    public List<HeroItem> ShopItems { get; private set; }
    public bool Achievement_BETA_Finish { get; set; }

    // Loyalty
    public int ShopLoyalty { get; set; }
    public int RecruitLoyalty { get; set; }

    /* GAME_MANAGER_DATA */
    // Player Preferences
    public const string MUSIC_VOLUME = "MusicVolume";
    public const string SFX_VOLUME = "SFXVolume";
    public const string HIDE_EXPLICIT_LANGUAGE = "HideExplicitLanguage";

    // Universal
    public const int MAX_HAND_SIZE = 8;
    public const int MAX_UNITS_PLAYED = 5;

    // Player
    public const string PLAYER = "Player";
    public const int MINIMUM_DECK_SIZE = 12;
    public const int PLAYER_STARTING_HEALTH = 20;
    public const int PLAYER_HAND_SIZE = 4;
    public const int PLAYER_START_UNITS = 2;
    public const int PLAYER_START_SKILLS = 2;
    public const int START_ENERGY_PER_TURN = 1;
    public const int MAXIMUM_ENERGY_PER_TURN = 5;
    public const int MAXIMUM_ENERGY = 10;
    public const int HERO_ULTMATE_GOAL = 3;
    //public const int HERO_ULTMATE_GOAL = 1; // FOR TESTING ONLY

    // Enemy
    public const string ENEMY = "Enemy";
    public const int ENEMY_STARTING_HEALTH = 20;
    //public const int ENEMY_STARTING_HEALTH = 1; // FOR TESTING ONLY
    public const int BOSS_BONUS_HEALTH = 10;
    public const int ENEMY_HAND_SIZE = 0;
    public const int ENEMY_START_UNITS = 5;

    // Aether Rewards
    public const int IGNORE_CARD_AETHER = 1;
    // Aether Costs
    public const int LEARN_SKILL_COST = 2;
    public const int REMOVE_CARD_COST = 1;
    public const int CLONE_UNIT_COST = 2;
    public const int CLONE_RARE_UNIT_COST = 4;
    // Recruits
    public const int RECRUIT_UNIT_COST = 2;
    public const int RECRUIT_RARE_UNIT_COST = 4;
    public const int RECRUIT_LOYALTY_GOAL = 3;
    // Items
    public const int BUY_ITEM_COST = 1;
    public const int BUY_RARE_ITEM_COST = 2;
    public const int SHOP_LOYALTY_GOAL = 3;

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
        StartTitleScene();
        Debug.Log("Application Version: " + Application.version);
        LoadPlayerPreferences(); // TESTING
    }

    /******
     * *****
     * ****** NEW_GAME
     * *****
     *****/
    public void NewGame()
    {
        IsCombatTest = false;
        CurrentNarrative = settingNarrative;
        caMan.LoadNewRecruits(); // TESTING
        ShopItems = GetShopItems();
        ShopLoyalty = 0;
        RecruitLoyalty = 0;
        GetActiveLocation(homeBaseLocation);
        CurrentLocation = GetActiveLocation(firstLocation);
        pMan.AetherCells = 0; // TESTING
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
        foreach (Effect e in efMan.GiveNextEffects) Destroy(e);
        efMan.GiveNextEffects.Clear();
        // Event Manager
        evMan.ClearDelayedActions();
        // UI Manager
        uMan.ClearAugmentBar();
        uMan.ClearItemBar();
        // Corotoutines
        StopAllCoroutines(); // TESTING
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
        GameData data = new GameData(narrativeName, pMan.PlayerHero.HeroName,
            deckList, augments, items, pMan.AetherCells,
            npcsAndClips, locationsNPCsObjectives, VisitedLocations.ToArray(),
            shopItems, recruitMages, recruitRogues, recruitTechs, recruitWarriors,
            RecruitLoyalty, ShopLoyalty,
            Achievement_BETA_Finish);
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
        else
        {
            Achievement_BETA_Finish = data.Achievement_BETA_Finish;
            return true;
        }
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
        for (int i = 0; i < cards.Length; i++)
            allcards.Add(cards[i]);
        for (int i = 0; i < combatRewards.Length; i++)
            allcards.Add(combatRewards[i]);
        for (int i = 0; i < recruitUnits.Length; i++)
            allcards.Add(recruitUnits[i]);

        // LOCATIONS
        Location[] locations = Resources.LoadAll<Location>("Locations");
        List<Location> allLocations = new List<Location>();
        for (int i = 0; i < locations.Length; i++)
            allLocations.Add(locations[i]);

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
        caMan.PlayerRecruitMages.Clear();
        for (int i = 0; i < data.RecruitMages.Length; i++)
            caMan.PlayerRecruitMages.Add(GetCard(data.RecruitMages[i]) as UnitCard);

        caMan.PlayerRecruitRogues.Clear();
        for (int i = 0; i < data.RecruitRogues.Length; i++)
            caMan.PlayerRecruitRogues.Add(GetCard(data.RecruitRogues[i]) as UnitCard);

        caMan.PlayerRecruitTechs.Clear();
        for (int i = 0; i < data.RecruitTechs.Length; i++)
            caMan.PlayerRecruitTechs.Add(GetCard(data.RecruitTechs[i]) as UnitCard);

        caMan.PlayerRecruitWarriors.Clear();
        for (int i = 0; i < data.RecruitWarriors.Length; i++)
            caMan.PlayerRecruitWarriors.Add(GetCard(data.RecruitWarriors[i]) as UnitCard);

        // LOYALTY
        RecruitLoyalty = data.RecruitLoyalty;
        ShopLoyalty = data.ShopLoyalty;

        // ACHIEVEMENTS
        Achievement_BETA_Finish = data.Achievement_BETA_Finish;

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
        auMan.StopCurrentSoundscape(); // TESTING
        FindObjectOfType<HeroSelectSceneDisplay>().DisplaySelectedHero(); // TESTING
    }

    /******
     * *****
     * ****** ENTER_WORLD_MAP
     * *****
     *****/
    public void EnterWorldMap()
    {
        auMan.StartStopSound("Soundtrack_WorldMapScene", null, AudioManager.SoundType.Soundtrack);
        auMan.StartStopSound("Soundscape_WorldMapScene", null, AudioManager.SoundType.Soundscape);
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
            uMan.CreateNarrativePopup(CurrentNarrative); // TESTING
            CurrentNarrative = null; // TESTING
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
        auMan.StartStopSound(null,
            CurrentNarrative.NarrativeSoundscape, AudioManager.SoundType.Soundscape);
        NarrativeSceneDisplay nsd = FindObjectOfType<NarrativeSceneDisplay>();
        nsd.CurrentNarrative = CurrentNarrative;
        Debug.Log("START NARRATIVE: " + CurrentNarrative.ToString());
    }
    public void EndNarrative()
    {
        if (CurrentNarrative == settingNarrative)
        {
            CurrentNarrative = newGameNarrative;
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
        pHD.PowerUsedIcon.SetActive(false);
        pHD.UltimateUsedIcon.SetActive(true);
        eHD.HeroStats.SetActive(false);

        // ENEMY MANAGER
        enMan.StartCombat();
        EnemyHero enemyHero = dMan.EngagedHero as EnemyHero;
        if (enemyHero == null)
        {
            Debug.LogError("ENEMY HERO IS NULL!");
            return;
        }
        enMan.EnemyHero = enemyHero;
        enMan.EnemyHealth = ENEMY_STARTING_HEALTH; // TESTING

        // PLAYER MANAGER
        pMan.PlayerHealth = pMan.MaxPlayerHealth;
        pMan.EnergyPerTurn = START_ENERGY_PER_TURN;
        pMan.EnergyLeft = pMan.StartEnergy; // TESTING
        pMan.HeroUltimateProgress_Direct = 0;

        // UPDATE DECKS
        caMan.UpdateDeck(PLAYER);
        caMan.UpdateDeck(ENEMY);
        // DISPLAY HEROES
        coMan.PlayerHero.GetComponent<HeroDisplay>().HeroScript = pMan.PlayerHero;
        coMan.EnemyHero.GetComponent<HeroDisplay>().HeroScript = enMan.EnemyHero;
        // SCHEDULE ACTIONS
        evMan.NewDelayedAction(() => anMan.CombatIntro(), 1f);
        float delay = 0; // TESTING
        foreach (HeroItem item in pMan.HeroItems) delay += 0.5f;
        foreach (HeroAugment aug in pMan.HeroAugments) delay += 0.5f;
        if (delay < 3) delay = 3;

        evMan.NewDelayedAction(() => CombatStart(), delay);
        evMan.NewDelayedAction(() => StartCombatTurn(PLAYER), delay + 1);
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
            if (pMan.GetAugment("Cognitive Magnifier")) bonusCards = 2;
            caMan.ShuffleDeck(PLAYER, false);
            for (int i = 0; i < PLAYER_HAND_SIZE + bonusCards; i++)
                evMan.NewDelayedAction(() => coMan.DrawCard(PLAYER), 0.5f);
            evMan.NewDelayedAction(() => Mulligan(), 0.5f);
        }
        void Mulligan()
        {
            efMan.StartEffectGroupList(new List<EffectGroup> { mulliganEffect }, gameObject);
        }
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
        pMan.IsMyTurn = false;
        efMan.GiveNextEffects.Clear();
        evMan.ClearDelayedActions();
        FunctionTimer.Create(() =>
        uMan.CreateCombatEndPopup(playerWins), 2f);
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

        if (isPlayerTurn)
        {
            evMan.NewDelayedAction(() => PlayerTurnStart(), 2);
            evMan.NewDelayedAction(() => TurnDraw(), 1);

            void TurnDraw()
            {
                coMan.DrawCard(PLAYER);
                coMan.SelectPlayableCards(); // TESTING
            }
            void PlayerTurnStart()
            {
                pMan.IsMyTurn = true;
                enMan.IsMyTurn = false;
                pMan.HeroPowerUsed = false;
                int startEnergy = pMan.EnergyLeft;
                pMan.EnergyLeft += pMan.EnergyPerTurn;
                int energyChange = pMan.EnergyLeft - startEnergy;
                anMan.ModifyHeroEnergyState(energyChange);
            }
        }

        evMan.NewDelayedAction(() =>
        caMan.TriggerPlayedUnits(CardManager.TURN_START, player), 0); // TESTING

        if (!isPlayerTurn)
        {
            pMan.IsMyTurn = false;
            enMan.IsMyTurn = true;
            evMan.NewDelayedAction(() => enMan.StartEnemyTurn(), 2);
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
        caMan.TriggerPlayedUnits(CardManager.TURN_END, player), 0.5f);

        evMan.NewDelayedAction(() => coMan.RefreshUnits(player), 0.5f);
        evMan.NewDelayedAction(() => RemoveEffects(), 0.5f);

        if (player == ENEMY) evMan.NewDelayedAction(() => StartCombatTurn(PLAYER), 0.5f);
        else if (player == PLAYER)
        {
            pMan.IsMyTurn = false;
            coMan.SelectPlayableCards(); // TESTING
            if (pMan.EnergyPerTurn < pMan.MaxEnergyPerTurn)
                evMan.NewDelayedAction(() => IncreaseMaxEnergy(), 0.5f);
            evMan.NewDelayedAction(() => StartCombatTurn(ENEMY), 0.5f);
        }

        void IncreaseMaxEnergy()
        {
            int startEnergy = pMan.EnergyPerTurn;
            pMan.EnergyPerTurn++;
            int energyChange = pMan.EnergyPerTurn - startEnergy;
            anMan.ModifyHeroEnergyState(energyChange);
        }
        void RemoveEffects()
        {
            efMan.RemoveTemporaryEffects(PLAYER);
            efMan.RemoveTemporaryEffects(ENEMY);
            efMan.RemoveGiveNextEffects();
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
            Debug.Log("Creating NEW NPC instance! <" + npc.HeroName + ">");
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
        allItems.Shuffle(); // TESTING
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
    public int GetItemCost(HeroItem item)
    {
        int itemCost;
        if (item.IsRareItem) itemCost = BUY_RARE_ITEM_COST;
        else itemCost = BUY_ITEM_COST;
        if (ShopLoyalty == SHOP_LOYALTY_GOAL) itemCost -= BUY_ITEM_COST; // TESTING
        return itemCost;
    }

    /******
     * *****
     * ****** GET_RECRUIT_COST
     * *****
     *****/
    public int GetRecruitCost(UnitCard unitCard)
    {
        int recruitCost;
        if (unitCard.IsRare) recruitCost = RECRUIT_RARE_UNIT_COST;
        else recruitCost = RECRUIT_UNIT_COST;
        if (RecruitLoyalty == RECRUIT_LOYALTY_GOAL) recruitCost -= RECRUIT_UNIT_COST; // TESTING
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
