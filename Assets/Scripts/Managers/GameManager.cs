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

    /* MANAGERS */
    private PlayerManager pMan;
    private EnemyManager enMan;
    private CombatManager coMan;
    private UIManager uMan;
    private EventManager evMan;
    private EffectManager efMan;
    private AudioManager auMan;
    private DialogueManager dMan;

    [SerializeField] private GiveNextUnitEffect augmentBiogenEffect;
    [SerializeField] private Narrative settingNarrative;
    [SerializeField] private Narrative newGameNarrative;

    /* !FOR_TESTING_ONLY! */
    public NPCHero NPCTestHero { get => npcTestHero; }
    public bool IsCombatTest { get; set; }
    [SerializeField] private NPCHero npcTestHero;
    /* !FOR_TESTING_ONLY! */

    public Narrative NextNarrative { get; set; }
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

    /******
     * *****
     * ****** START
     * *****
     *****/
    private void Start()
    {
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        evMan = EventManager.Instance;
        efMan = EffectManager.Instance;
        auMan = AudioManager.Instance;
        dMan = DialogueManager.Instance;
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
    public void EndGame()
    {
        // Game Manager
        foreach (NPCHero npc in ActiveNPCHeroes) Destroy(npc);
        ActiveNPCHeroes.Clear();
        // Player Manager
        Destroy(pMan.PlayerHero);
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
        auMan.StartStopSound("Soundtrack_Narrative1", null, AudioManager.SoundType.Soundtrack);
        NarrativeSceneDisplay nsd = FindObjectOfType<NarrativeSceneDisplay>();
        nsd.Narrative = NextNarrative;
        if (NextNarrative == newGameNarrative) NextNarrative = null;
    }
    public void EndNarrative()
    {
        if (NextNarrative == settingNarrative) 
            SceneLoader.LoadScene(SceneLoader.Scene.NewGameScene);
        else if (NextNarrative == pMan.PlayerHero.HeroBackstory)
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
        enMan.StartCombat();
        EnemyHero enemyHero = dMan.EngagedHero as EnemyHero;
        if (enemyHero == null)
        {
            Debug.LogError("ENEMY HERO IS NULL!");
            return;
        }
        auMan.StartStopSound("Soundtrack_Combat1", null, AudioManager.SoundType.Soundtrack);
        FunctionTimer.Create(() => auMan.StartStopSound("SFX_StartCombat"), 1f);
        /* UPDATE_DECKS */
        enMan.EnemyHero = enemyHero;
        coMan.UpdateDeck(PLAYER);
        coMan.UpdateDeck(ENEMY);
        /* PLAYER_HEALTH */
        int bonusHealth = 0;
        if (pMan.GetAugment("Kinetic Regulator")) bonusHealth = 5;
        pMan.PlayerHealth = PLAYER_STARTING_HEALTH + bonusHealth;
        /* PLAYER_ACTIONS */
        int bonusActions = 0;
        if (pMan.GetAugment("Synaptic Stabilizer")) bonusActions = 1;
        pMan.ActionsPerTurn = START_ACTIONS_PER_TURN + bonusActions;
        pMan.PlayerActionsLeft = 0;
        /* ENEMY_HEALTH */
        enMan.EnemyHealth = ENEMY_STARTING_HEALTH;
        /* HERO_DISPLAYS */
        coMan.PlayerHero.GetComponent<HeroDisplay>().HeroScript = pMan.PlayerHero;
        coMan.EnemyHero.GetComponent<HeroDisplay>().HeroScript = enMan.EnemyHero;
        if (pMan.GetAugment("Biogenic Enhancer"))
        {
            GiveNextUnitEffect gnue = ScriptableObject.CreateInstance<GiveNextUnitEffect>();
            gnue.LoadEffect(augmentBiogenEffect);
            efMan.GiveNextEffects.Add(gnue);
        }
        for (int i = 0; i < PLAYER_HAND_SIZE; i++)
            evMan.NewDelayedAction(() => coMan.DrawCard(PLAYER), 1f);
        evMan.NewDelayedAction(() => StartTurn(PLAYER), 1f);
    }

    /******
     * *****
     * ****** END_COMBAT
     * *****
     *****/
    public void EndCombat(bool playerWins)
    {
        if (playerWins) auMan.StartStopSound
                (null, pMan.PlayerHero.HeroWin);
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
    private void StartTurn(string player)
    {
        evMan.NewDelayedAction(() => coMan.PrepareAllies(player), 0.5f);
        if (player == PLAYER)
        {
            pMan.IsMyTurn = true;
            enMan.IsMyTurn = false;
            pMan.HeroPowerUsed = false;
            evMan.NewDelayedAction(() => RefillPlayerActions(), 0.5f);
            evMan.NewDelayedAction(() => coMan.DrawCard(PLAYER), 1);
            void RefillPlayerActions()
            {
                pMan.PlayerActionsLeft = pMan.ActionsPerTurn;
                auMan.StartStopSound("SFX_ActionRefill");
            }
        }
        else if (player == ENEMY)
        {
            pMan.IsMyTurn = false;
            enMan.IsMyTurn = true;
            evMan.NewDelayedAction(() => enMan.StartEnemyTurn(), 0);
        }
        else
        {
            Debug.LogError("PLAYER NOT FOUND!");
            return;
        }
        uMan.UpdateEndTurnButton(pMan.IsMyTurn);
        FunctionTimer.Create(() => uMan.CreateTurnPopup(pMan.IsMyTurn), 1);
    }

    /******
     * *****
     * ****** END_TURN
     * *****
     *****/
    public void EndTurn(string player)
    {
        void RemoveEffects()
        {
            efMan.RemoveTemporaryEffects(PLAYER);
            efMan.RemoveTemporaryEffects(ENEMY);
            efMan.RemoveGiveNextEffects();
        }
        evMan.NewDelayedAction(() => RemoveEffects(), 0.5f);
        if (player == ENEMY) StartTurn(PLAYER);
        else if (player == PLAYER)
        {
            if (pMan.ActionsPerTurn < MAX_ACTIONS_PER_TURN)
                pMan.ActionsPerTurn++;
            StartTurn(ENEMY);
        }
    }
}
