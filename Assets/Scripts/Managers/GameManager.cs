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
    [SerializeField] private NPCHero NPCTestHero; // FOR TESTING ONLY
    /* AUGMENT_EFFECTS */
    [SerializeField] private GiveNextUnitEffect augmentBiogenEffect;
    /* ACTIVE_NPCS */
    private static List<NPCHero> ActiveNPCHeroes;

    /* GAME_MANAGER_DATA */
    public const int START_ACTIONS_PER_TURN = 1;
    public const int MAX_ACTIONS_PER_TURN = 5;
    public const int MAXIMUM_ACTIONS = 5;

    public const string PLAYER = "Player";
    //public const int PLAYER_STARTING_HEALTH = 20;
    public const int PLAYER_STARTING_HEALTH = 1; // FOR TESTING ONLY
    public const int PLAYER_HAND_SIZE = 4;
    public const int PLAYER_START_FOLLOWERS = 2;
    public const int PLAYER_START_SKILLS = 2;

    public const string ENEMY = "Enemy";
    //public const int ENEMY_STARTING_HEALTH = 20;
    public const int ENEMY_STARTING_HEALTH = 1; // FOR TESTING ONLY
    public const int ENEMY_HAND_SIZE = 0;
    public const int ENEMY_START_FOLLOWERS = 5;
    public const int ENEMY_START_SKILLS = 2;

    /* MANAGERS */
    private PlayerManager playerManager;
    private EnemyManager enemyManager;
    private CardManager cardManager;
    private UIManager UIManager;
    EventManager eventManager;

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
        
        ActiveNPCHeroes = new List<NPCHero>(); // STATIC
    }

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
        DialogueManager.Instance.StartDialogue(GetActiveNPC(NPCTestHero)); // FOR TESTING ONLY
    }

    /******
     * *****
     * ****** END_GAME
     * *****
     *****/
    public void EndGame()
    {
        foreach (NPCHero npc in ActiveNPCHeroes) Destroy(npc);
        ActiveNPCHeroes.Clear();
        SceneLoader.LoadScene(SceneLoader.Scene.TitleScene);
    }

    /******
     * *****
     * ****** START_COMBAT
     * *****
     *****/
    public void StartCombat()
    {
        enemyManager.StartCombat();
        EnemyHero enemyHero = DialogueManager.Instance.EngagedHero as EnemyHero;
        if (enemyHero == null)
        {
            Debug.LogError("ENEMY HERO IS NULL!");
            return;
        }

        AudioManager.Instance.StartStopSound("Soundtrack_Combat1", null, AudioManager.SoundType.Soundtrack);
        FunctionTimer.Create(() => AudioManager.Instance.StartStopSound("SFX_StartCombat"), 1f);
        
        /* UPDATE_DECKS */
        enemyManager.EnemyHero = enemyHero;
        cardManager.UpdateDeck(PLAYER);
        cardManager.UpdateDeck(ENEMY);
        /* PLAYER_HEALTH */
        int bonusHealth = 0;
        if (playerManager.GetAugment("Kinetic Regulator")) bonusHealth = 5;
        playerManager.PlayerHealth = PLAYER_STARTING_HEALTH + bonusHealth;
        /* PLAYER_ACTIONS */
        playerManager.PlayerActionsLeft = 0;
        int bonusActions = 0;
        if (playerManager.GetAugment("Synaptic Stabilizer")) bonusActions = 1;
        playerManager.ActionsPerTurn = START_ACTIONS_PER_TURN + bonusActions;
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
            EffectManager.Instance.GiveNextEffects.Add(gnue);
        }
        /* DELAYED_ACTIONS */
        for (int i = 0; i < PLAYER_HAND_SIZE; i++)
        {
            eventManager.NewDelayedAction(() => cardManager.DrawCard(PLAYER), 1f);
        }
        for (int i = 0; i < ENEMY_HAND_SIZE; i++)
        {
            eventManager.NewDelayedAction(() => cardManager.DrawCard(ENEMY), 1f);
        }
        eventManager.NewDelayedAction(() => StartTurn(PLAYER), 1f);
    }

    /******
     * *****
     * ****** END_COMBAT
     * *****
     *****/
    public void EndCombat(bool playerWins)
    {
        if (playerWins)
            AudioManager.Instance.StartStopSound(null, PlayerManager.Instance.PlayerHero.HeroWin);
        else
            AudioManager.Instance.StartStopSound(null, PlayerManager.Instance.PlayerHero.HeroLose);
        UIManager.CreateCombatEndPopup(playerWins);
        // VICTORY or DEFEAT animation
    }

    /******
     * *****
     * ****** START_TURN
     * *****
     *****/
    private void StartTurn(string player)
    {
        cardManager.RefreshFollowers(player);
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
                AudioManager.Instance.StartStopSound("SFX_ActionRefill");
            }
            EventManager.Instance.NewDelayedAction(() => RefillPlayerActions(), 0f);
            EventManager.Instance.NewDelayedAction(() => cardManager.DrawCard(PLAYER), 1f);
        }
        // ENEMY
        else if (player == ENEMY)
        {
            playerManager.IsMyTurn = false;
            enemyManager.IsMyTurn = true;
            UIManager.UpdateEndTurnButton(false);
            EnemyManager.Instance.StartEnemyTurn();
        }
    }

    /******
     * *****
     * ****** END_TURN
     * *****
     *****/
    public void EndTurn(string player)
    {
        CardManager.Instance.RemoveTemporaryEffects(PLAYER);
        CardManager.Instance.RemoveTemporaryEffects(ENEMY);
        CardManager.Instance.RemoveGiveNextEffects();
        if (player == ENEMY) StartTurn(PLAYER);
        else if (player == PLAYER)
        {
            if (playerManager.ActionsPerTurn < MAX_ACTIONS_PER_TURN)
                playerManager.ActionsPerTurn++;
            StartTurn(ENEMY);
        }
    }
}
