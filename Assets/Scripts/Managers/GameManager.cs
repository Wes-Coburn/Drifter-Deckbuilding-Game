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

    /* TEST_HEROES */
    [SerializeField] private NPCHero npcTestHero; // FOR TESTING ONLY
    public NPCHero NPCTestHero { get => npcTestHero; } // FOR TESTING ONLY
    public bool IsCombatTest { get; set; } // FOR TESTING ONLY

    /* AUGMENT_EFFECTS */
    [SerializeField] private GiveNextUnitEffect augmentBiogenEffect;

    /* SETTING_NARRATIVE */
    [SerializeField] private Narrative settingNarrative;
    /* NEW_GAME_NARRATIVE */
    [SerializeField] private Narrative newGameNarrative;

    /* NEXT_NARRATIVE */
    public Narrative NextNarrative { get; set; }

    /* ACTIVE_NPCS */
    public static List<NPCHero> ActiveNPCHeroes { get; private set; }

    /* GAME_MANAGER_DATA */
    public const int START_ACTIONS_PER_TURN = 1;
    public const int MAX_ACTIONS_PER_TURN = 5;
    public const int MAXIMUM_ACTIONS = 5;

    public const string PLAYER = "Player";
    public const int PLAYER_STARTING_HEALTH = 20;
    //public const int PLAYER_STARTING_HEALTH = 5; // FOR TESTING ONLY
    public const int PLAYER_HAND_SIZE = 4;
    public const int PLAYER_START_FOLLOWERS = 2;
    public const int PLAYER_START_SKILLS = 2;

    public const string ENEMY = "Enemy";
    public const int ENEMY_STARTING_HEALTH = 20;
    //public const int ENEMY_STARTING_HEALTH = 5; // FOR TESTING ONLY
    public const int ENEMY_HAND_SIZE = 0;
    public const int ENEMY_START_FOLLOWERS = 5;
    public const int ENEMY_START_SKILLS = 2;

    /* MANAGERS */
    private PlayerManager playerManager;
    private EnemyManager enemyManager;
    private CardManager cardManager;
    private UIManager UIManager;
    private EventManager eventManager;
    private EffectManager effectManager;
    private AudioManager audioManager;
    private DialogueManager dialogueManager;

    /******
     * *****
     * ****** START
     * *****
     *****/
    private void Start()
    {
        playerManager = PlayerManager.Instance;
        enemyManager = EnemyManager.Instance;
        cardManager = CardManager.Instance;
        UIManager = UIManager.Instance;
        eventManager = EventManager.Instance;
        effectManager = EffectManager.Instance;
        audioManager = AudioManager.Instance;
        dialogueManager = DialogueManager.Instance;
        ActiveNPCHeroes = new List<NPCHero>(); // STATIC
    }

    /******
     * *****
     * ****** GET_ACTIVE_NPC
     * *****
     *****/
    public NPCHero GetActiveNPC(NPCHero npc)
    {
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
            ActiveNPCHeroes.Add(newNPC);
            return newNPC;
        }
    }

    /******
     * *****
     * ****** NEW_GAME
     * *****
     *****/
    public void NewGame()
    {
        IsCombatTest = false; // FOR TESTING ONLY
        NextNarrative = settingNarrative;
        SceneLoader.LoadScene(SceneLoader.Scene.NarrativeScene);
    }

    /******
     * *****
     * ****** END_GAME
     * *****
     *****/
    public void EndGame(bool combatStarted = true)
    {
        // Game Manager
        foreach (NPCHero npc in ActiveNPCHeroes) Destroy(npc);
        ActiveNPCHeroes.Clear();
        // Player Manager
        Destroy(playerManager.PlayerHero);
        playerManager.PlayerHero = null;
        // Enemy Manager
        Destroy(enemyManager.EnemyHero);
        enemyManager.EnemyHero = null;
        // Dialogue Manager
        dialogueManager.EndDialogue();
        // Effect Manager
        foreach (Effect e in effectManager.GiveNextEffects) Destroy(e);
        effectManager.GiveNextEffects.Clear();
        // Scene Loader
        SceneLoader.LoadScene(SceneLoader.Scene.TitleScene);
    }

    /******
     * *****
     * ****** START/END_NARRATIVE
     * *****
     *****/
    public void StartNarrative()
    {
        //AudioManager.Instance.StartStopSound("Soundtrack_xxx", null, AudioManager.SoundType.Soundtrack);
        Debug.Log("START NARRATIVE!");
        NarrativeSceneDisplay nsd = FindObjectOfType<NarrativeSceneDisplay>();
        nsd.Narrative = NextNarrative;
        if (NextNarrative == newGameNarrative) NextNarrative = null;
    }
    public void EndNarrative()
    {
        if (NextNarrative == settingNarrative) 
            SceneLoader.LoadScene(SceneLoader.Scene.NewGameScene);
        else if (NextNarrative == playerManager.PlayerHero.HeroBackstory)
        {
            NextNarrative = newGameNarrative;
            SceneLoader.LoadScene(SceneLoader.Scene.NarrativeScene, true);
        }
        else SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
    }


    /******
     * *****
     * ****** START_COMBAT
     * *****
     *****/
    public void StartCombat()
    {
        enemyManager.StartCombat();
        EnemyHero enemyHero = dialogueManager.EngagedHero as EnemyHero;
        if (enemyHero == null)
        {
            Debug.LogError("ENEMY HERO IS NULL!");
            return;
        }

        audioManager.StartStopSound("Soundtrack_Combat1", null, AudioManager.SoundType.Soundtrack);
        FunctionTimer.Create(() => audioManager.StartStopSound("SFX_StartCombat"), 1f);
        
        /* UPDATE_DECKS */
        enemyManager.EnemyHero = enemyHero;
        cardManager.UpdateDeck(PLAYER);
        cardManager.UpdateDeck(ENEMY);
        /* PLAYER_HEALTH */
        int bonusHealth = 0;
        if (playerManager.GetAugment("Kinetic Regulator")) bonusHealth = 5;
        playerManager.PlayerHealth = PLAYER_STARTING_HEALTH + bonusHealth;
        /* PLAYER_ACTIONS */
        int bonusActions = 0;
        if (playerManager.GetAugment("Synaptic Stabilizer")) bonusActions = 1;
        playerManager.ActionsPerTurn = START_ACTIONS_PER_TURN + bonusActions;
        playerManager.PlayerActionsLeft = 0;
        /* ENEMY_HEALTH */
        enemyManager.EnemyHealth = ENEMY_STARTING_HEALTH;
        /* HERO_DISPLAYS */
        cardManager.PlayerHero.GetComponent<HeroDisplay>().HeroScript = playerManager.PlayerHero;
        cardManager.EnemyHero.GetComponent<HeroDisplay>().HeroScript = enemyManager.EnemyHero;
        /* OTHER_AUGMENTS */
        if (playerManager.GetAugment("Biogenic Enhancer"))
        {
            GiveNextUnitEffect gnue = ScriptableObject.CreateInstance<GiveNextUnitEffect>();
            gnue.LoadEffect(augmentBiogenEffect);
            effectManager.GiveNextEffects.Add(gnue);
        }
        /* DELAYED_ACTIONS */
        for (int i = 0; i < PLAYER_HAND_SIZE; i++)
            eventManager.NewDelayedAction(() => cardManager.DrawCard(PLAYER), 1f);
        /* START_TURN */
        eventManager.NewDelayedAction(() => StartTurn(PLAYER), 1f);
    }

    /******
     * *****
     * ****** END_COMBAT
     * *****
     *****/
    public void EndCombat(bool playerWins)
    {
        if (playerWins) audioManager.StartStopSound
                (null, playerManager.PlayerHero.HeroWin);
        else audioManager.StartStopSound
                (null, playerManager.PlayerHero.HeroLose);

        playerManager.IsMyTurn = false;
        effectManager.GiveNextEffects.Clear();
        eventManager.ClearDelayedActions();
        FunctionTimer.Create(() => UIManager.CreateCombatEndPopup(playerWins), 2f);
    }

    /******
     * *****
     * ****** START_TURN
     * *****
     *****/
    private void StartTurn(string player)
    {
        cardManager.PrepareAllies(player);
        // PLAYER
        if (player == PLAYER)
        {
            playerManager.IsMyTurn = true;
            enemyManager.IsMyTurn = false;
            UIManager.UpdateEndTurnButton(true);
            playerManager.HeroPowerUsed = false;
            void RefillPlayerActions ()
            {
                playerManager.PlayerActionsLeft = playerManager.ActionsPerTurn;
                audioManager.StartStopSound("SFX_ActionRefill");
            }
            eventManager.NewDelayedAction(() => RefillPlayerActions(), 0f);
            eventManager.NewDelayedAction(() => cardManager.DrawCard(PLAYER), 1f);
        }
        // ENEMY
        else if (player == ENEMY)
        {
            playerManager.IsMyTurn = false;
            enemyManager.IsMyTurn = true;
            UIManager.UpdateEndTurnButton(false);
            enemyManager.StartEnemyTurn();
        }
        UIManager.CreateTurnPopup(playerManager.IsMyTurn);
    }

    /******
     * *****
     * ****** END_TURN
     * *****
     *****/
    public void EndTurn(string player)
    {
        cardManager.RemoveTemporaryEffects(PLAYER);
        cardManager.RemoveTemporaryEffects(ENEMY);
        cardManager.RemoveGiveNextEffects();
        if (player == ENEMY) StartTurn(PLAYER);
        else if (player == PLAYER)
        {
            if (playerManager.ActionsPerTurn < MAX_ACTIONS_PER_TURN)
                playerManager.ActionsPerTurn++;
            StartTurn(ENEMY);
        }
    }
}
