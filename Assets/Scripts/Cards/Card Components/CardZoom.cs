using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZoom : MonoBehaviour, IPointerClickHandler
{
    /* ZOOMCARD_DATA */
    private const int    ZOOM_Z_VALUE                =  -4;
    private const float  ZOOM_BUFFER                 =  350;
    private const float  ZOOM_SCALE_VALUE            =  4;
    private const float  CENTER_SCALE_VALUE          =  6;
    private const float  POPUP_SCALE_VALUE           =  3;
    private const float  SMALL_POPUP_SCALE_VALUE     =  2;

    private const string ZOOM_CARD_TIMER            =  "ZoomCardTimer";
    private const string ABILITY_POPUP_TIMER        =  "AbilityPopupTimer";

    /* PREFABS */
    [SerializeField] private GameObject unitZoomCardPrefab;
    [SerializeField] private GameObject actionZoomCardPrefab;
    [SerializeField] private GameObject abilityBoxPrefab;
    [SerializeField] private GameObject abilityPopupPrefab;
    [SerializeField] private GameObject abilityPopupBoxPrefab;
    [SerializeField] private GameObject descriptionPopupPrefab;

    public GameObject UnitZoomCardPrefab { get => unitZoomCardPrefab; }
    public GameObject ActionZoomCardPrefab { get => actionZoomCardPrefab; }

    /* ZONES */
    private GameObject worldSpace;
    private GameObject playerHand;
    private GameObject playerZone;
    private GameObject enemyHand;
    private GameObject enemyZone;
    private GameObject heroSkills;

    /* CARD_DISPLAY */
    private CardDisplay cardDisplay;

    public static bool ZoomCardIsCentered = false;
    public static GameObject CurrentZoomCard { get; set; }
    public static GameObject DescriptionPopup { get; set; }
    public static GameObject AbilityPopupBox { get; set; }
    
    /******
     * *****
     * ****** START
     * *****
     *****/
    private void Start()
    {
        worldSpace = UIManager.Instance.CurrentWorldSpace;
        playerHand = CardManager.Instance.PlayerHand;
        playerZone = CardManager.Instance.PlayerZone;
        enemyHand = CardManager.Instance.EnemyHand;
        enemyZone = CardManager.Instance.EnemyZone;
        cardDisplay = gameObject.GetComponent<CardDisplay>();
        heroSkills = GameObject.Find("HeroSkills");
    }

    /******
     * *****
     * ****** ON_CLICK
     * *****
     *****/
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Right) return;
        if (DragDrop.CardIsDragging || ZoomCardIsCentered) return;
        if (transform.parent.gameObject == enemyHand) return; // HIDE THE ENEMY HAND

        FunctionTimer.StopTimer(ZOOM_CARD_TIMER);
        FunctionTimer.StopTimer(ABILITY_POPUP_TIMER);
        ZoomCardIsCentered = true;
        UIManager.Instance.SetScreenDimmer(true);

        CreateZoomCard(new Vector2(0, 50), CENTER_SCALE_VALUE);
        CreateDescriptionPopup(new Vector2(-590, 0), POPUP_SCALE_VALUE);
        CreateAbilityPopups(new Vector2(590, 0), POPUP_SCALE_VALUE);
    }

    /******
     * *****
     * ****** ON_POINTER_ENTER
     * *****
     *****/
    public void OnPointerEnter()
    {
        if (DragDrop.CardIsDragging || ZoomCardIsCentered || 
            UIManager.Instance.PlayerIsTargetting) return;

        float cardYPos;
        float popupXPos;

        if (transform.parent.gameObject == playerHand) ShowZoomCard();
        else FunctionTimer.Create(() => ShowZoomCard(), 0.5f, ZOOM_CARD_TIMER);

        void ShowZoomCard()
        {
            RectTransform rect;
            if (transform.parent.gameObject == playerHand)
            {
                rect = playerHand.GetComponent<RectTransform>();
                cardYPos = rect.position.y + ZOOM_BUFFER;
            }
            else if (transform.parent.gameObject == playerZone)
            {
                rect = playerZone.GetComponent<RectTransform>();
                cardYPos = rect.position.y + ZOOM_BUFFER;
            }
            else if (transform.parent.gameObject == enemyHand) return; // HIDE ENEMY HAND
            else if (transform.parent.gameObject == enemyZone)
            {
                rect = enemyZone.GetComponent<RectTransform>();
                cardYPos = (int)rect.position.y - ZOOM_BUFFER;
            }
            else if (heroSkills != null && transform.parent.parent.gameObject == heroSkills)
            {
                CreateAbilityPopups(new Vector2(0, 150), ZOOM_SCALE_VALUE);
                return;
            }
            else
            {
                CreateAbilityPopups(new Vector2(500, 0), POPUP_SCALE_VALUE);
                return;
            }
            Vector3 vec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (vec3.x > 0) popupXPos = vec3.x - 400;
            else popupXPos = vec3.x + 400;

            CreateZoomCard(new Vector2(vec3.x, cardYPos), ZOOM_SCALE_VALUE);
            FunctionTimer.Create(() => ShowAbilityPopup(), 0.75f, ABILITY_POPUP_TIMER);
        }

        void ShowAbilityPopup() => CreateAbilityPopups(new Vector2(popupXPos, cardYPos), SMALL_POPUP_SCALE_VALUE);
    }

    /******
     * *****
     * ****** ON_POINTER_EXIT
     * *****
     *****/
    public void OnPointerExit()
    {
        if (DragDrop.CardIsDragging || ZoomCardIsCentered) return;
        UIManager.Instance.DestroyZoomObjects();
        FunctionTimer.StopTimer(ZOOM_CARD_TIMER);
        FunctionTimer.StopTimer(ABILITY_POPUP_TIMER);
    }

    /******
     * *****
     * ****** CREATE_ZOOM_OBJECT
     * *****
     *****/
    private GameObject CreateZoomObject(GameObject prefab, Vector3 vec2, Transform parentTransform, float scaleValue)
    {        
        GameObject zoomObject = Instantiate(prefab, vec2, Quaternion.identity);
        Transform tran = zoomObject.transform;
        tran.SetParent(parentTransform, true);
        tran.position = new Vector3(tran.position.x, tran.position.y, ZOOM_Z_VALUE);
        tran.localScale = new Vector2(scaleValue, scaleValue);
        return zoomObject;
    }

    /******
     * *****
     * ****** CREATE_ZOOM_CARD
     * *****
     *****/
    private void CreateZoomCard(Vector2 vec2, float scaleValue)
    {
        if (CurrentZoomCard != null)
        {
            Destroy(CurrentZoomCard);
            CurrentZoomCard = null;
        }
        if (AbilityPopupBox != null)
        {
            Destroy(AbilityPopupBox);
            AbilityPopupBox = null;
        }

        GameObject cardPrefab;
        if (gameObject.GetComponent<CardDisplay>() is UnitCardDisplay) cardPrefab = unitZoomCardPrefab;
        else if (gameObject.GetComponent<CardDisplay>() is ActionCardDisplay) cardPrefab = actionZoomCardPrefab;
        else
        {
            Debug.LogError("CARD DISPLAY TYPE NOT FOUND!");
            return;
        }
        CurrentZoomCard = CreateZoomObject(cardPrefab, new Vector3(vec2.x, vec2.y), worldSpace.transform, scaleValue);
        CurrentZoomCard.GetComponent<CardDisplay>().DisplayZoomCard(gameObject);
    }

    /******
     * *****
     * ****** CREATE_ZOOM_ABILLITY_ICON
     * *****
     *****/
    public void CreateZoomAbilityIcon(CardAbility cardAbility, Transform parentTransform, float scaleValue)
    {
        if (worldSpace == null) // NEW GAME SCENE
        {
            worldSpace = UIManager.Instance.CurrentWorldSpace;
            cardDisplay = gameObject.GetComponent<CardDisplay>();
        }

        GameObject zoomIconPrefab = gameObject.GetComponent<UnitCardDisplay>().ZoomAbilityIconPrefab;
        GameObject abilityIcon = Instantiate(zoomIconPrefab, new Vector3(0, 0), Quaternion.identity);
        Transform popTran = abilityIcon.transform;
        popTran.SetParent(parentTransform, true);
        popTran.localScale = new Vector2(scaleValue, scaleValue);
        abilityIcon.GetComponent<AbilityIconDisplay>().ZoomAbilityScript = cardAbility;
    }

    /******
     * *****
     * ****** CREATE_DESCRIPTION_POPUP
     * *****
     *****/
    public void CreateDescriptionPopup(Vector2 vec2, float scaleValue)
    {
        if (worldSpace == null) // NEW GAME SCENE
        {
            worldSpace = UIManager.Instance.CurrentWorldSpace;
            cardDisplay = gameObject.GetComponent<CardDisplay>();
        }

        DescriptionPopup = CreateZoomObject(descriptionPopupPrefab, new Vector3(vec2.x, vec2.y), worldSpace.transform, scaleValue);
        DescriptionPopup.GetComponent<DescriptionPopupDisplay>().DisplayDescriptionPopup(cardDisplay.CardScript.CardDescription);
    }

    /******
     * *****
     * ****** CREATE_ABILITY_POPUPS
     * *****
     *****/
    public void CreateAbilityPopups(Vector2 vec2, float scaleValue)
    {
        if (worldSpace == null)
        {
            worldSpace = UIManager.Instance.CurrentWorldSpace;
            cardDisplay = gameObject.GetComponent<CardDisplay>();
        }
        
        AbilityPopupBox = CreateZoomObject(abilityPopupBoxPrefab, 
            new Vector3(vec2.x, vec2.y), worldSpace.transform, scaleValue);

        List<CardAbility> abilityList;
        if (cardDisplay is UnitCardDisplay ucd)
        {
            if (CardZoom.ZoomCardIsCentered) abilityList = ucd.UnitCard.StartingAbilities;
            else abilityList = ucd.CurrentAbilities;
        }
        else if (cardDisplay is ActionCardDisplay acd)
            abilityList = acd.ActionCard.LinkedAbilities;
        else
        {
            if (cardDisplay == null) Debug.LogError("DISPLAY IS NULL!");
            else Debug.LogError("DISPLAY TYPE NOT FOUND!");
            return;
        }

        foreach (CardAbility ca in abilityList)
        {
            GameObject abilityPopup = Instantiate(abilityPopupPrefab, AbilityPopupBox.transform);
            abilityPopup.GetComponent<AbilityPopupDisplay>().AbilityScript = ca;
            foreach (CardAbility linkCa in ca.LinkedAbilites)
            {
                abilityPopup = Instantiate(abilityPopupPrefab, AbilityPopupBox.transform);
                abilityPopup.GetComponent<AbilityPopupDisplay>().AbilityScript = linkCa;
            }
        }
    }
}
