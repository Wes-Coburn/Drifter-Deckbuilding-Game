using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static PlayerManager Instance { get; private set; }
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
        PlayerDeck2 = new List<Card>(); // TESTING

        // REMOVE ALL BELOW
        PlayerDeck = new List<int>();
        while (PlayerDeck.Count < 30)
        {
            PlayerDeck.Add(1);
            PlayerDeck.Add(3);
            PlayerDeck.Add(4);
        }
    }

    /* PLAYER_HERO */
    public Hero PlayerHero
    {
        get => playerHero;
        set
        {
            playerHero = value;
            for (int i = 0; i < CardManager.PLAYER_START_FOLLOWERS; i++)
            {
                CardManager.Instance.AddCard(CardManager.Instance.StartPlayerFollower, GameManager.PLAYER);
            }
            foreach (SkillCard skill in PlayerHero.HeroSkills)
            {
                for (int i = 0; i < CardManager.PLAYER_START_SKILLS; i++)
                {
                    CardManager.Instance.AddCard(skill, GameManager.PLAYER);
                }
            }
        }
    }
    private Hero playerHero;

    /* PLAYER_DECK */
    public List<int> PlayerDeck { get; private set; }
    public List<Card> PlayerDeck2 { get; private set; }

    /* IS_MY_TURN */
    public bool IsMyTurn { get; set; }

    /* HEALTH */
    private int playerHealth;
    public int PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = value;
            UIManager.Instance.UpdatePlayerHealth(PlayerHealth);
        }
    }

    /* ACTIONS_LEFT */
    private int playerActionsLeft;
    public int PlayerActionsLeft
    {
        get => playerActionsLeft;
        set
        {
            playerActionsLeft = value;
            if (playerActionsLeft > GameManager.MAXIMUM_ACTIONS) playerActionsLeft = GameManager.MAXIMUM_ACTIONS;
            UIManager.Instance.UpdatePlayerActionsLeft(PlayerActionsLeft);
        }
    }
}
