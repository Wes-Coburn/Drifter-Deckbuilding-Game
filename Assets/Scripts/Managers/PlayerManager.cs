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
        PlayerDeck = new List<int>();
        while (PlayerDeck.Count < 30) // FOR TESTING ONLY
        {
            PlayerDeck.Add(1);
            PlayerDeck.Add(3);
            PlayerDeck.Add(4);
        }
    }

    /* PLAYER_DECK */
    public List<int> PlayerDeck { get; private set; }

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
