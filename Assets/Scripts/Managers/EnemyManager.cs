using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static EnemyManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        EnemyDeckList = new List<Card>();
        CurrentEnemyDeck = new List<Card>();
        ReinforcementSchedule = new List<int>();

        cardMan = CardManager.Instance;
        eveMan = EventManager.Instance;
    }

    CardManager cardMan;
    EventManager eveMan;

    /* ENEMY_HERO */
    public Hero EnemyHero
    {
        get => enemyHero;
        set
        {
            enemyHero = value;
            foreach (UnitCard follower in enemyHero.HeroUnits)
            {
                for (int i = 0; i < GameManager.ENEMY_START_FOLLOWERS; i++)
                {
                    CardManager.Instance.AddCard(follower, GameManager.ENEMY);
                }
            }
            foreach (SkillCard skill in EnemyHero.HeroSkills)
            {
                for (int i = 0; i < GameManager.ENEMY_START_SKILLS; i++)
                {
                    CardManager.Instance.AddCard(skill, GameManager.ENEMY);
                }
            }
            ReinforcementSchedule = EnemyHero.ReinforcementSchedule;
            CurrentReinforcements = 0;
        }
    }
    private Hero enemyHero;

    /* ENEMY_DECK */
    public List<Card> EnemyDeckList { get; private set; }
    public List<Card> CurrentEnemyDeck { get; private set; }
    public List<int> ReinforcementSchedule { get; private set; }
    public int CurrentReinforcements { get; set; }

    /* IS_MY_TURN */
    public bool IsMyTurn { get; set; }

    /* HEALTH */
    private int enemyHealth;
    public int EnemyHealth
    {
        get => enemyHealth;
        set
        {
            enemyHealth = value;
            UIManager.Instance.UpdateEnemyHealth(EnemyHealth);
        }
    }

    /******
     * *****
     * ****** START_ENEMY_TURN
     * *****
     *****/
    public void StartEnemyTurn()
    {
        int refo = CurrentReinforcements;
        List<int> refoSched = ReinforcementSchedule;

        // DELAYED ACTIONS
        for (int i = 0; i < refoSched[refo]; i++)
        {
            eveMan.NewDelayedAction(() => cardMan.DrawCard(GameManager.ENEMY), 1.5f);
        }
        for (int i = 0; i < refoSched[refo]; i++)
        {
            eveMan.NewDelayedAction(() => cardMan.PlayCard(cardMan.EnemyHandCards[0]), 1.5f);
        }
        if ((refo + 1) < refoSched.Count) CurrentReinforcements++;
        else CurrentReinforcements = 0;

        cardMan.EnemyHero.GetComponent<EnemyHeroDisplay>().NextReinforcements =
            refoSched[CurrentReinforcements];

        eveMan.NewDelayedAction(() => CMBeginAttack(), 1f);

        void CMBeginAttack()
        {
            foreach (GameObject enemyHero in cardMan.EnemyZoneCards)
            {
                if (!enemyHero.GetComponent<UnitCardDisplay>().IsExhausted)
                {
                    eveMan.NewDelayedAction(() => CMAttack(enemyHero), 1f);
                }
            }
            // END TURN
            eveMan.NewDelayedAction(() => GameManager.Instance.EndTurn(GameManager.ENEMY), 1f);
        }

        void CMAttack(GameObject hero)
        {
            if (cardMan.PlayerZoneCards.Count > 0)
            {
                cardMan.Attack(hero, cardMan.PlayerZoneCards[0]);
            }
            else cardMan.Attack(hero, cardMan.PlayerHero);
        }
    }
}
