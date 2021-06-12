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

    public bool IsMyTurn;
    private void Start() => IsMyTurn = true;

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
}
