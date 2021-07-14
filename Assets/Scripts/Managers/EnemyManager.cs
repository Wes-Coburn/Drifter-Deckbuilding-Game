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
    }

    /* ENEMY_HERO */
    public Hero EnemyHero
    {
        get => enemyHero;
        set
        {
            enemyHero = value;
            foreach (FollowerCard follower in enemyHero.HeroFollowers)
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
}
