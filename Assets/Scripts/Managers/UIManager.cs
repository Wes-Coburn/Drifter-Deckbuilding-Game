using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject screenDimmerPrefab;
    private GameObject screenDimmer;

    /* INFO_POPUP */
    [SerializeField] private GameObject infoPopupPrefab;
    private GameObject infoPopup;

    /* WAIT_FOR_SECONDS */
    public Action OnWaitForSecondsCallback { get; set; }

    /* PLAYER_IS_TARGETING */
    public bool PlayerIsTargetting { get; set; }

    public void Start()
    {
        CurrentBackground = GameObject.Find("Background");
        CurrentCanvas = GameObject.Find("Canvas");
        PlayerIsTargetting = false;
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
    public void UpdatePlayerActionsLeft(int newPlayerActionsLeft)
    {
        playerActionsLeft.GetComponent<TextMeshProUGUI>().SetText("Actions: " + newPlayerActionsLeft.ToString());
    }
    public void UpdateEnemyActionsLeft(int newEnemyActionsLeft)
    {
        enemyActionsLeft.GetComponent<TextMeshProUGUI>().SetText("Actions: " + newEnemyActionsLeft.ToString());
    }
    public void UpdateEndTurnButton(bool isMyTurn)
    {
        BoxCollider2D boxCollider = endTurnButton.GetComponent<BoxCollider2D>();
        boxCollider.enabled = isMyTurn;
        endTurnButton.GetComponent<EndTurnButtonDisplay>().EndTurnSide.SetActive(isMyTurn);
        endTurnButton.GetComponent<EndTurnButtonDisplay>().OpponentTurnSide.SetActive(!isMyTurn);
    }

    /******
     * *****
     * ****** DESTROY_ALL_ZOOM_OBJECT(S)
     * *****
     *****/
    public void DestroyAllZoomObjects()
    {
        SetScreenDimmer(false);
        CardZoom.ZoomCardIsCentered = false;

        List<GameObject> objectsToDestroy = new List<GameObject>
        {
            CardZoom.NextLevelPopup,
            CardZoom.LorePopup,
            CardZoom.CurrentZoomCard,
            AbilityZoom.AbilityPopup
        };

        foreach (GameObject go in objectsToDestroy)
        {
            if (go != null) DestroyObject(go);
        }
        void DestroyObject(GameObject go)
        {
            Destroy(go);
            go = null;
        }
    }

    /******
     * *****
     * ****** SET_SCREEN_DIMMER
     * *****
     *****/
    public void SetScreenDimmer(bool screenIsDimmed)
    {
        if (screenDimmer != null)
        {
            Destroy(screenDimmer);
            screenDimmer = null;
        }
        if (screenIsDimmed)
        {
            screenDimmer = Instantiate(screenDimmerPrefab, new Vector3(0, 0, -3), Quaternion.identity, CurrentBackground.transform);
        }
    }

    /******
     * *****
     * ****** CREATE_INFO_POPUP
     * *****
     *****/
    public void CreateInfoPopup(string message)
    {
        DestroyInfoPopup();
        infoPopup = Instantiate(infoPopupPrefab, new Vector2(680, 0), Quaternion.identity, CurrentBackground.transform);
        infoPopup.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
    }

    /******
     * *****
     * ****** DISMISS/DESTROY_INFO_POPUP
     * *****
     *****/
    public void DismissInfoPopup()
    {
        if (infoPopup != null)
            AnimationManager.Instance.ChangeAnimationState(infoPopup, "Exit");
    }
    public void DestroyInfoPopup()
    {
        if (infoPopup != null)
        {
            Destroy(infoPopup);
            infoPopup = null;
        }
    }
}
