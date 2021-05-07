using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /* PREFABS */
    [SerializeField] private GameObject screenDimmerPrefab;
    [SerializeField] private GameObject centerScreenPopup;

    /* GAME ZONES */
    [SerializeField] private GameObject background;

    /* REPUTATION */
    [SerializeField] private GameObject playerHealth;
    [SerializeField] private GameObject enemyHealth;

    /* ACTIONS_LEFT */
    [SerializeField] private GameObject playerActionsLeft;
    [SerializeField] private GameObject enemyActionsLeft;

    /* END_TURN_BUTTON */
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private GameObject endTurnSide;
    [SerializeField] private GameObject opponentTurnSide;

    /* CLASS_VARAIBLES */
    private GameObject screenDimmer;

    /******
     * *****
     * ****** SETTERS
     * *****
     *****/
    public void UpdatePlayerHealth(int health) => playerHealth.GetComponent<TextMeshProUGUI>().SetText("Health: " + health);
    public void UpdateEnemyHealth(int health) => enemyHealth.GetComponent<TextMeshProUGUI>().SetText("Health: " + health);
    public void UpdatePlayerActionsLeft(int playerActionsLeft) => this.playerActionsLeft.GetComponent<TextMeshProUGUI>().SetText("Actions: " + playerActionsLeft.ToString());
    public void UpdateEnemyActionsLeft(int enemyActionsLeft) => this.enemyActionsLeft.GetComponent<TextMeshProUGUI>().SetText("Actions: " + enemyActionsLeft.ToString());
    public void UpdateEndTurnButton(bool isMyTurn)
    {
        BoxCollider2D boxCollider = endTurnButton.GetComponent<BoxCollider2D>();
        boxCollider.enabled = isMyTurn;
        endTurnSide.SetActive(isMyTurn);
        opponentTurnSide.SetActive(!isMyTurn);
    }

    /******
     * *****
     * ****** DESTROY_ZOOM_OBJECT(S)
     * *****
     *****/
    public void DestroyAllZoomObjects()
    {
        SetScreenDimmer(false);
        CardZoom.ZoomCardIsCentered = false;
        Destroy(CardZoom.NextLevelPopup);
        Destroy(CardZoom.LorePopup);
        Destroy(CardZoom.CurrentZoomCard);
        Destroy(AbilityZoom.AbilityPopup);
    }

    /******
     * *****
     * ****** SET_SCREEN_DIMMER
     * *****
     *****/
    public void SetScreenDimmer(bool screenIsDimmed)
    {
        if (screenIsDimmed)
        {
            screenDimmer = Instantiate(screenDimmerPrefab, new Vector3(0, 0, -3), Quaternion.identity);
            screenDimmer.transform.SetParent(background.transform);
        }
        else Destroy(screenDimmer);
    }
}
