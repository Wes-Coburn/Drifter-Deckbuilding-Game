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

    [SerializeField] private GameObject screenDimmerPrefab;
    [SerializeField] private GameObject infoPopupPrefab;
    [SerializeField] private GameObject combatEndPopupPrefab;

    private GameObject screenDimmer;
    private GameObject infoPopup;
    private GameObject combatEndPopup;
    private GameObject selectedEnemy;
    private GameObject endTurnButton;

    public Action OnWaitForSecondsCallback { get; set; }
    public bool PlayerIsTargetting { get; set; }
    public GameObject CurrentWorldSpace { get; set; }
    public GameObject CurrentCanvas { get; set; }

    public void Start()
    {
        CurrentWorldSpace = GameObject.Find("WorldSpace");
        CurrentCanvas = GameObject.Find("Canvas");
        PlayerIsTargetting = false;
    }

    public void StartCombatScene()
    {
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
     * ****** SELECT_ENEMY
     * *****
     *****/
    public void SelectEnemy(GameObject enemy, bool enabled)
    {
        if (selectedEnemy != null)
        {
            GameObject oldEnemy;
            oldEnemy = selectedEnemy;
            selectedEnemy = null;
            SelectEnemy(oldEnemy, false);
        }

        if (enabled) selectedEnemy = enemy;
        if (enemy.TryGetComponent<CardSelect>(out CardSelect cs))
            cs.CardOutline.SetActive(enabled);
        else if (enemy.TryGetComponent<HeroSelect>(out HeroSelect hs))
            hs.TargetIcon.SetActive(enabled);
        else
        {
            Debug.LogError("TARGET SELECT SCRIPT NOT FOUND!");
            return;
        }
    }

    /******
     * *****
     * ****** SETTERS
     * *****
     *****/
    public void UpdateEndTurnButton(bool isMyTurn)
    {
        BoxCollider2D bc = endTurnButton.GetComponent<BoxCollider2D>();
        bc.enabled = isMyTurn;
        EndTurnButtonDisplay etbd = endTurnButton.GetComponent<EndTurnButtonDisplay>();
        etbd.EndTurnSide.SetActive(isMyTurn);
        etbd.OpponentTurnSide.SetActive(!isMyTurn);
    }

    /******
     * *****
     * ****** DESTROY_ZOOM_OBJECT(S)
     * *****
     *****/
    public void DestroyZoomObjects()
    {
        static void DestroyObject(GameObject go)
        {
            Destroy(go);
            go = null;
        }

        SetScreenDimmer(false);
        CardZoom.ZoomCardIsCentered = false;

        List<GameObject> objectsToDestroy = new List<GameObject>
        {
            CardZoom.CurrentZoomCard,
            CardZoom.DescriptionPopup,
            CardZoom.AbilityPopupBox,
            AbilityZoom.AbilityPopup
        };

        foreach (GameObject go in objectsToDestroy)
        {
            if (go != null) DestroyObject(go);
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
            GameObject prefab = screenDimmerPrefab;
            GameObject parent = CurrentWorldSpace;
            screenDimmer = Instantiate(prefab, new Vector3(0, 0, -3), Quaternion.identity, parent.transform);
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
        infoPopup = Instantiate(infoPopupPrefab, new Vector2(680, 0), Quaternion.identity, CurrentWorldSpace.transform);
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

    /******
     * *****
     * ****** CREATE_COMBAT_END_POPUP
     * *****
     *****/
    public void CreateCombatEndPopup(bool playerWins)
    {
        combatEndPopup = Instantiate(combatEndPopupPrefab, CurrentCanvas.transform);
        CombatEndPopupDisplay cepd = combatEndPopup.GetComponent<CombatEndPopupDisplay>();
        cepd.VictoryText.SetActive(playerWins);
        cepd.DefeatText.SetActive(!playerWins);
        cepd.GetComponent<SoundPlayer>().PlaySound(0);
    }
}
