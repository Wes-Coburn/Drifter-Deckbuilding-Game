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
                for (int i = 0; i < CardManager.ENEMY_START_FOLLOWERS; i++)
                {
                    CardManager.Instance.AddCard(follower, GameManager.ENEMY);
                }
            }
            foreach (SkillCard skill in EnemyHero.HeroSkills)
            {
                for (int i = 0; i < CardManager.ENEMY_START_SKILLS; i++)
                {
                    CardManager.Instance.AddCard(skill, GameManager.ENEMY);
                }
            }
        }
    }
    private Hero enemyHero;

    /* ENEMY_DECK */
    public List<Card> EnemyDeckList { get; private set; }
    public List<Card> CurrentEnemyDeck { get; private set; }

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
    
    /* ACTIONS_LEFT */
    private int enemyActionsLeft;
    public int EnemyActionsLeft
    {
        get => enemyActionsLeft;
        set
        {
            enemyActionsLeft = value;
            if (enemyActionsLeft > GameManager.MAXIMUM_ACTIONS) enemyActionsLeft = GameManager.MAXIMUM_ACTIONS;
            UIManager.Instance.UpdateEnemyActionsLeft(EnemyActionsLeft);
        }
    }
}
