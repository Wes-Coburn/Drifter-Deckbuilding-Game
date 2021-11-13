using System.Collections.Generic;
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

    [Header("GAME CHAPTERS")]
    [SerializeField] string[] gameChapters;
    [Header("NARRATIVES")]
    [SerializeField] private Narrative settingNarrative;
    [SerializeField] private Narrative newGameNarrative;
    [Header("LOCATIONS")]
    [SerializeField] private Location homeBaseLocation;
    [SerializeField] private Location firstLocation;
    [SerializeField] GameObject locationIconPrefab;
    [Header("AUGMENT EFFECTS")]
    [SerializeField] private GiveNextUnitEffect augmentBiogenEffect;

    private PlayerManager pMan;
    private EnemyManager enMan;
    private CardManager caMan;
    private CombatManager coMan;
    private UIManager uMan;
    private EventManager evMan;
    private EffectManager efMan;
    private AudioManager auMan;
    private DialogueManager dMan;
    private int currentChapter;

    public bool IsCombatTest { get; set; } // FOR TESTING ONLY
    public bool HideExplicitLanguage { get; private set; }
    public string NextChapter
    {
        get
        {
            if (currentChapter > (gameChapters.Length - 1)) return null;
            else return gameChapters[currentChapter++];
        }
    }
    public Narrative NextNarrative { get; set; }
    public List<NPCHero> ActiveNPCHeroes { get; private set; }
    public List<Location> ActiveLocations { get; private set; }
    public Location CurrentLocation { get; set; }

    /* GAME_MANAGER_DATA */
    // Universal
    public const int START_ACTIONS_PER_TURN = 1;
    public const int MAX_ACTIONS_PER_TURN = 5;
    public const int MAXIMUM_ACTIONS = 5;
    public const int MAX_HAND_SIZE = 6;
    public const int MAX_UNITS_PLAYED = 5;

    // Player
    public const string PLAYER = "Player";
    public const int PLAYER_STARTING_HEALTH = 20;
    //public const int PLAYER_STARTING_HEALTH = 1; // FOR TESTING ONLY
    public const int PLAYER_HAND_SIZE = 4;
    public const int PLAYER_START_FOLLOWERS = 2;
    public const int PLAYER_START_SKILLS = 2;

    // Aether Costs
    public const int LEARN_SKILL_COST = 2;
    public const int RECRUIT_UNIT_COST = 2; // TESTING
    public const int ACQUIRE_AUGMENT_COST = 4;
    public const int REMOVE_CARD_COST = 1;

    // Enemy
    public const string ENEMY = "Enemy";
    //public const int ENEMY_STARTING_HEALTH = 20;
    public const int ENEMY_STARTING_HEALTH = 1; // FOR TESTING ONLY
    public const int ENEMY_HAND_SIZE = 0;
    public const int ENEMY_START_FOLLOWERS = 5;
    public const int ENEMY_START_SKILLS = 2;

    /******
     * *****
     * ****** START
     * *****
     *****/
    private void Start()
    {
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        caMan = CardManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        evMan = EventManager.Instance;
        efMan = EffectManager.Instance;
        auMan = AudioManager.Instance;
        dMan = DialogueManager.Instance;
        ActiveNPCHeroes = new List<NPCHero>();
        ActiveLocations = new List<Location>();
    }

    /******
     * *****
     * ****** NEW_GAME
     * *****
     *****/
    public void NewGame(bool hideExplicitLanguage)
    {
        IsCombatTest = false; // FOR TESTING ONLY
        HideExplicitLanguage = hideExplicitLanguage;
        currentChapter = 0;
        NextNarrative = settingNarrative;
        caMan.ShuffleRecruits(); // TESTING
        GetActiveLocation(homeBaseLocation);
        CurrentLocation = GetActiveLocation(firstLocation);
        SceneLoader.LoadScene(SceneLoader.Scene.NarrativeScene);
    }

    /******
     * *****
     * ****** SAVE/LOAD_GAME
     * *****
     *****/
    private void SaveGame()
    {
        string[] deckList = new string[pMan.PlayerDeckList.Count];
        for (int i = 0; i < deckList.Length; i++)
            deckList[i] = pMan.PlayerDeckList[i].CardName;

        string[] augments = new string[pMan.HeroAugments.Count];
        for (int i = 0; i < augments.Length; i++)
            augments[i] = pMan.HeroAugments[i].AugmentName;

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

        GameData data = new GameData(HideExplicitLanguage, pMan.PlayerHero.HeroName, deckList, augments, pMan.AetherCells,
            npcsAndClips, locationsNPCsObjectives, recruitMages, recruitRogues, recruitTechs, recruitWarriors);
        SaveLoad.SaveGame(data);
    }
    public bool LoadGame(bool isPrecheck = false)
    {
        GameData data = SaveLoad.LoadGame();
        if (data == null) return false;
        else if (isPrecheck) return true;

        /** LOAD RESOURCES **/
        // HEROES
        Hero[] heroes = Resources.LoadAll<Hero>("Heroes");
        List<Hero> allHeroes = new List<Hero>();
        for (int i = 0; i < heroes.Length; i++)
            allHeroes.Add(heroes[i]);

        // CARDS
        Card[] cards = Resources.LoadAll<Card>("Cards");
        List<Card> allcards = new List<Card>();
        for (int i = 0; i < cards.Length; i++)
            allcards.Add(cards[i]);

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
        HeroAugment[] augments = Resources.LoadAll<HeroAugment>("Heroes");
        List<HeroAugment> allAugments = new List<HeroAugment>();
        for (int i = 0; i < augments.Length; i++)
            allAugments.Add(augments[i]);

        /** LOAD DATA **/
        // HIDE EXPLICIT LANGUAGE
        HideExplicitLanguage = data.HideExplicitLanguage;

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

        Hero GetHero(string heroName)
        {
            int index = allHeroes.FindIndex(x => x.HeroName == heroName);
            if (index != -1) return allHeroes[index];
            else Debug.LogError("HERO NOT FOUND!");
            return null;
        }
        Card GetCard(string cardName)
        {
            int index = allcards.FindIndex(x => x.CardName == cardName);
            if (index != -1) return allcards[index];
            else Debug.LogError("CARD NOT FOUND!");
            return null;
        }
        Location GetLocation(string locationName)
        {
            int index = allLocations.FindIndex(x => x.LocationFullName == locationName);
            if (index != -1) return allLocations[index];
            else Debug.LogError("LOCATION NOT FOUND!");
            return null;
        }
        DialogueClip GetClip(string clipName)
        {
            int index = allClips.FindIndex(x => x.ToString() == clipName);
            if (index != -1) return allClips[index];
            else Debug.LogError("CLIP NOT FOUND!");
            return null;
        }
        HeroAugment GetAugment(string augmentName)
        {
            int index = allAugments.FindIndex(x => x.AugmentName == augmentName);
            if (index != -1) return allAugments[index];
            else Debug.LogError("AUGMENT NOT FOUND!");
            return null;
        }

        return true;
    }

    /******
     * *****
     * ****** END_GAME
     * *****
     *****/
    public void EndGame()
    {
        // Game Manager
        currentChapter = 0; // Unnecessary
        foreach (NPCHero npc in ActiveNPCHeroes) Destroy(npc);
        ActiveNPCHeroes.Clear();
        foreach (Location loc in ActiveLocations) Destroy(loc);
        ActiveLocations.Clear();
        // Player Manager
        // Don't destroy PlayerHero, it is an asset, not an instance
        pMan.PlayerHero = null;
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
        // Corotoutines
        StopAllCoroutines(); // TESTING
        // Scene Loader
        SceneLoader.LoadScene(SceneLoader.Scene.TitleScene);
    }

    /******
     * *****
     * ****** ENTER/EXIT_WORLD_MAP
     * *****
     *****/
    public void EnterWorldMap()
    {
        Debug.Log("ENTER WORLD MAP!");
        foreach (Location loc in ActiveLocations)
        {
            GameObject location = Instantiate(locationIconPrefab,
                uMan.CurrentCanvas.transform);
            LocationIcon icon = location.GetComponent<LocationIcon>();
            icon.Location = loc;
            icon.LocationName = loc.LocationName;
            icon.WorldMapPosition = loc.WorldMapPosition;
            if (loc.IsHomeBase) icon.SetHomeBaseImage();
        }

        SaveGame(); // TESTING
    }

    public void ExitWorldMap()
    {
        Debug.LogWarning("BLANK!");
    }

    /******
     * *****
     * ****** START/END_NARRATIVE
     * *****
     *****/
    public void StartNarrative()
    {
        auMan.StartStopSound("Soundtrack_Narrative1", null,
            AudioManager.SoundType.Soundtrack);
        NarrativeSceneDisplay nsd = FindObjectOfType<NarrativeSceneDisplay>();
        nsd.Narrative = NextNarrative;
        Debug.Log("START NARRATIVE: " + NextNarrative.ToString());
    }
    public void EndNarrative()
    {
        if (NextNarrative == settingNarrative) 
            SceneLoader.LoadScene(SceneLoader.Scene.HeroSelectScene);
        else if (NextNarrative == pMan.PlayerHero.HeroBackstory)
        {
            NextNarrative = newGameNarrative;
            SceneLoader.LoadScene(SceneLoader.Scene.NarrativeScene, true);
        }
        else if (NextNarrative == newGameNarrative) 
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
        else Debug.LogError("NO CONDITIONS MATCHED!");
    }

    /******
     * *****
     * ****** START_COMBAT
     * *****
     *****/
    public void StartCombat()
    {
        auMan.StartStopSound("Soundtrack_Combat1",
            null, AudioManager.SoundType.Soundtrack);
        auMan.StartStopSound("SFX_StartCombat");

        PlayerHeroDisplay pHD = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>();
        EnemyHeroDisplay eHD = coMan.EnemyHero.GetComponent<EnemyHeroDisplay>();
        uMan.EndTurnButton.SetActive(false);
        pHD.HeroStats.SetActive(false);
        pHD.PowerUsedIcon.SetActive(false);
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
        enMan.EnemyHealth = ENEMY_STARTING_HEALTH;
        // PLAYER MANAGER
        int bonusHealth = 0;
        if (pMan.GetAugment("Kinetic Amplifier")) bonusHealth = 5;
        pMan.PlayerHealth = PLAYER_STARTING_HEALTH + bonusHealth;
        int bonusActions = 0;
        if (pMan.GetAugment("Synaptic Stabilizer")) bonusActions = 1;
        pMan.ActionsPerTurn = START_ACTIONS_PER_TURN + bonusActions;
        pMan.PlayerActionsLeft = 0;
        if (pMan.GetAugment("Biogenic Enhancer"))
        {
            GiveNextUnitEffect gnue = ScriptableObject.CreateInstance<GiveNextUnitEffect>();
            gnue.LoadEffect(augmentBiogenEffect);
            efMan.GiveNextEffects.Add(gnue);
        }
        // UPDATE DECKS
        caMan.UpdateDeck(PLAYER);
        caMan.UpdateDeck(ENEMY);
        // DISPLAY HEROES
        coMan.PlayerHero.GetComponent<HeroDisplay>().HeroScript = pMan.PlayerHero;
        coMan.EnemyHero.GetComponent<HeroDisplay>().HeroScript = enMan.EnemyHero;
        // SCHEDULE ACTIONS
        evMan.NewDelayedAction(() => AnimationManager.Instance.CombatIntro(), 1f);
        evMan.NewDelayedAction(() => CombatStart(), 4f);
        evMan.NewDelayedAction(() => StartCombatTurn(PLAYER), 1f);

        void CombatStart()
        {
            caMan.ShuffleDeck(pMan.CurrentPlayerDeck);
            for (int i = 0; i < PLAYER_HAND_SIZE; i++)
                evMan.NewDelayedAction(() => coMan.DrawCard(PLAYER), 0.5f);
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
            auMan.StartStopSound(null, pMan.PlayerHero.HeroWin);
            caMan.ShuffleRecruits();
        }
        else auMan.StartStopSound
                (null, pMan.PlayerHero.HeroLose);
        pMan.IsMyTurn = false;
        efMan.GiveNextEffects.Clear();
        evMan.ClearDelayedActions();
        FunctionTimer.Create(() => uMan.CreateCombatEndPopup(playerWins), 2f);
    }

    /******
     * *****
     * ****** START_TURN
     * *****
     *****/
    private void StartCombatTurn(string player)
    {
        bool isPlayerTurn;
        coMan.ActionsPlayedThisTurn = 0;

        if (player == PLAYER)
        {
            isPlayerTurn = true;
            evMan.NewDelayedAction(() => PlayerTurnStart(), 0.5f);
            evMan.NewDelayedAction(() => coMan.DrawCard(PLAYER), 1);

            void PlayerTurnStart()
            {
                pMan.IsMyTurn = true;
                enMan.IsMyTurn = false;
                pMan.HeroPowerUsed = false;
                pMan.PlayerActionsLeft = pMan.ActionsPerTurn;
                AnimationManager.Instance.ModifyHeroActionsState();
            }
        }
        else if (player == ENEMY)
        {
            isPlayerTurn = false;
            pMan.IsMyTurn = false;
            enMan.IsMyTurn = true;
            evMan.NewDelayedAction(() => enMan.StartEnemyTurn(), 0);
        }
        else
        {
            Debug.LogError("PLAYER NOT FOUND!");
            return;
        }
        uMan.UpdateEndTurnButton(isPlayerTurn);
        FunctionTimer.Create(() => uMan.CreateTurnPopup(isPlayerTurn), 1);
    }

    /******
     * *****
     * ****** END_TURN
     * *****
     *****/
    public void EndCombatTurn(string player)
    {
        evMan.NewDelayedAction(() => coMan.PrepareAllies(player), 0.5f);
        evMan.NewDelayedAction(() => RemoveEffects(), 0.5f);

        if (player == ENEMY) StartCombatTurn(PLAYER);
        else if (player == PLAYER)
        {
            if (pMan.ActionsPerTurn < MAX_ACTIONS_PER_TURN)
                pMan.ActionsPerTurn++;
            StartCombatTurn(ENEMY);
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
            Debug.Log("Creating new NPC instance!");
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
            Debug.LogWarning("LOCATION FOUND! <" + loc.LocationFullName + ">");
            if (newNPC != null) loc.CurrentNPC = GetActiveNPC(newNPC);
            Debug.LogWarning("ACTIVE LOCATIONS: <" + ActiveLocations.Count + ">");
            return loc;
        }
        else
        {
            Location newLoc = ScriptableObject.CreateInstance<Location>();
            newLoc.LoadLocation(location);
            newLoc.CurrentObjective = newLoc.FirstObjective;
            Debug.LogWarning("NEW LOCATION CREATED! <" + newLoc.LocationFullName + ">");
            if (newNPC != null) newLoc.CurrentNPC = GetActiveNPC(newNPC);
            else newLoc.CurrentNPC = GetActiveNPC(location.FirstNPC);
            ActiveLocations.Add(newLoc);
            Debug.LogWarning("ACTIVE LOCATIONS: <" + ActiveLocations.Count + ">");
            return newLoc;
        }
    }
}
