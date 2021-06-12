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

    public bool IsMyTurn { get; set; }
    private void Start() => IsMyTurn = false;

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
