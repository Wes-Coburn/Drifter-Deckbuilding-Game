using System;
using System.Collections;
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
    public GameObject CurrentBackground { get; set; }
    public GameObject CurrentCanvas { get; set; }

    /* HEALTH */
    private GameObject playerHealth;
    private GameObject enemyHealth;

    /* ACTIONS_LEFT */
    private GameObject playerActionsLeft;
    private GameObject enemyActionsLeft;

    /* END_TURN_BUTTON */
    private GameObject endTurnButton;

    /* SCREEN_DIMMER */
    private GameObject screenDimmer;

    /* WAIT_FOR_SECONDS */
    public Action OnWaitForSecondsCallback { get; set; }

    /* PLAYER_IS_TARGETING */
    public bool PlayerIsTargetting { get; set; } // TESTING

    public void Start()
    {
        CurrentBackground = GameObject.Find("Background");
        CurrentCanvas = GameObject.Find("Canvas");
        PlayerIsTargetting = false; // TESTING
    }

    public void LoadGameScene()
    {
        playerHealth = GameObject.Find("PlayerHealth");
        enemyHealth = GameObject.Find("EnemyHealth");
        playerActionsLeft = GameObject.Find("PlayerActionsLeft");
        enemyActionsLeft = GameObject.Find("EnemyActionsLeft");
        endTurnButton = GameObject.Find("EndTurnButton");
    }
    
    public void WaitForSeconds(float delay) => StartCoroutine(WaitForSecondsNumerator(delay));
    private IEnumerator WaitForSecondsNumerator(float delay)
    {
        yield return new WaitForSeconds(delay);
        WaitForSecondsCallback();
    }
    private void WaitForSecondsCallback()
    {
        if (OnWaitForSecondsCallback != null)
        {
            OnWaitForSecondsCallback();
            OnWaitForSecondsCallback = null;
        }
    }

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
        endTurnButton.GetComponent<EndTurnButtonDisplay>().EndTurnSide.SetActive(isMyTurn);
        endTurnButton.GetComponent<EndTurnButtonDisplay>().OpponentTurnSide.SetActive(!isMyTurn);
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
            screenDimmer.transform.SetParent(CurrentBackground.transform);
        }
        else Destroy(screenDimmer);
    }
}
