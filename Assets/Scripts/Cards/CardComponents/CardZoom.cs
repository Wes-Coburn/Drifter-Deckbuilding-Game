using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZoom : MonoBehaviour, IPointerClickHandler
{
    /* CARD_MANAGER_DATA */
    private const string PLAYER_HAND = CardManager.PLAYER_HAND;
    private const string PLAYER_ZONE = CardManager.PLAYER_ZONE;
    private const string ENEMY_HAND = CardManager.ENEMY_HAND;
    private const string ENEMY_ZONE = CardManager.ENEMY_ZONE;

    /* CHANGE_LAYER_DATA */
    private const string ZOOM_LAYER = "Zoom";

    /* ZOOMCARD_DATA */
    private const float ZOOM_BUFFER        =  350;
    private const float ZOOM_SCALE_VALUE   =  4;
    private const float CENTER_SCALE_VALUE =  6;
    private const float POPUP_SCALE_VALUE  =  3;
    private const float POPUP_X_VALUE      =  590;

    /* MANAGERS */
    private UIManager UIManager;

    /* PREFABS */
    [SerializeField] private GameObject heroZoomCard;
    [SerializeField] private GameObject actionZoomCard;

        // ABILITY_PREFABS
    [SerializeField] private GameObject abilityBoxPrefab;
    [SerializeField] private GameObject abilityPopupPrefab;
    
        // NEXT_LEVEL_PREFABS
    [SerializeField] private GameObject nextLevelBox;
    [SerializeField] private GameObject level2Popup;

    // LORE_PREFAB
    [SerializeField] private GameObject descriptionPopupPrefab;

    /* ZONES */
    private GameObject background;
    private GameObject playerHand;
    private GameObject playerZone;
    private GameObject enemyHand;
    private GameObject enemyZone;

    /* STATIC_CLASS_VARIABLES */
    public static bool ZoomCardIsCentered = false;
    public static GameObject CurrentZoomCard { get; set; }
    public static GameObject NextLevelPopup { get; set; }
    public static GameObject DescriptionPopup { get; set; }
    
    /* CLASS_VARIABLES */
    private CardDisplay cardDisplay;
    
    /******
     * *****
     * ****** AWAKE
     * *****
     *****/
    public void Awake()
    {
        UIManager = UIManager.Instance;
        background = GameObject.Find("Background");
        playerHand = GameObject.Find(PLAYER_HAND);
        playerZone = GameObject.Find(PLAYER_ZONE);
        enemyHand = GameObject.Find(ENEMY_HAND);
        enemyZone = GameObject.Find(ENEMY_ZONE);
        cardDisplay = gameObject.GetComponent<CardDisplay>();
    }

    /******
     * *****
     * ****** ON_CLICK
     * *****
     *****/
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Right) return;
        if (transform.parent.gameObject == enemyHand) return; // HIDE THE ENEMY HAND
        if (DragDrop.CardIsDragging || ZoomCardIsCentered || UIManager.Instance.PlayerIsTargetting) return;
        
        
        UIManager.SetScreenDimmer(true);
        ZoomCardIsCentered = true;

        CreateZoomCard(new Vector3(0, 50), CENTER_SCALE_VALUE);
        
        if (cardDisplay is HeroCardDisplay)
        {
            HeroCard hc = cardDisplay.CardScript as HeroCard;
            CreateNextLevelPopup(new Vector2(POPUP_X_VALUE, 0), POPUP_SCALE_VALUE, hc.Level2Abiliites);
            CreateDescriptionPopup(new Vector2(-600, 0), POPUP_SCALE_VALUE);
        }
    }

    /******
     * *****
     * ****** ON_POINTER_ENTER
     * *****
     *****/
    public void OnPointerEnter()
    {
        if (DragDrop.CardIsDragging || ZoomCardIsCentered || UIManager.Instance.PlayerIsTargetting) return;

        float yPos;
        RectTransform rect;
        if (transform.parent.gameObject == playerHand)
        {
            rect = playerHand.GetComponent<RectTransform>();
            yPos = rect.position.y + ZOOM_BUFFER;
        }
        else if (transform.parent.gameObject == playerZone)
        {
            rect = playerZone.GetComponent<RectTransform>();
            yPos = rect.position.y + ZOOM_BUFFER;
        }
        else if (transform.parent.gameObject == enemyHand) return; // HIDE THE ENEMY HAND
        else if (transform.parent.gameObject == enemyZone)
        {
            rect = enemyZone.GetComponent<RectTransform>();
            yPos = (int)rect.position.y - ZOOM_BUFFER;
        }
        else return;
        Vector3 vec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CreateZoomCard(new Vector3(vec3.x, yPos), ZOOM_SCALE_VALUE);
    }

    /******
     * *****
     * ****** ON_POINTER_EXIT
     * *****
     *****/
    public void OnPointerExit()
    {
        if (DragDrop.CardIsDragging || ZoomCardIsCentered || UIManager.Instance.PlayerIsTargetting) return;
        UIManager.DestroyZoomObjects();
    }

    /******
     * *****
     * ****** CREATE_ZOOM_OBJECT
     * *****
     *****/
    private GameObject CreateZoomObject(GameObject prefab, Vector3 vec3, Transform parentTransform, float scaleValue)
    {
        GameObject zoomObject = Instantiate(prefab, vec3, Quaternion.identity);
        Transform popTran = zoomObject.transform;
        popTran.SetParent(parentTransform, true);
        popTran.position = new Vector3(popTran.position.x, popTran.position.y, vec3.z);
        popTran.localScale = new Vector2(scaleValue, scaleValue);
        return zoomObject;
    }

    /******
     * *****
     * ****** CREATE_ZOOM_CARD
     * *****
     *****/
    private void CreateZoomCard(Vector2 vec2, float scaleValue)
    {
        if (CurrentZoomCard != null) Destroy(CurrentZoomCard);
        GameObject cardPrefab = null;
        if (gameObject.GetComponent<CardDisplay>() is HeroCardDisplay) cardPrefab = heroZoomCard;
        else if (gameObject.GetComponent<CardDisplay>() is ActionCardDisplay) cardPrefab = actionZoomCard;
        else Debug.Log("[CreateZoomCard() in CardZoom] CardDisplay TYPE NOT FOUND!");

        CurrentZoomCard = CreateZoomObject(cardPrefab, new Vector3(vec2.x, vec2.y, -4), background.transform, scaleValue);
        CurrentZoomCard.GetComponent<CardDisplay>().DisplayZoomCard(gameObject);
    }

    /******
     * *****
     * ****** CREATE_ZOOM_ABILLITY_ICON
     * *****
     *****/
    public void CreateZoomAbilityIcon(CardAbility cardAbility, Transform parentTransform, float scaleValue)
    {
        GameObject abilityIconPrefab = gameObject.GetComponent<HeroCardDisplay>().AbilityIconPrefab;
        GameObject abilityIcon = Instantiate(abilityIconPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Transform popTran = abilityIcon.transform;
        popTran.SetParent(parentTransform, true);
        popTran.localScale = new Vector2(scaleValue, scaleValue);

        abilityIcon.GetComponent<ChangeLayer>().ZoomLayer();
        abilityIcon.layer = LayerMask.NameToLayer(ZOOM_LAYER);
        foreach (Transform child in abilityIcon.transform) child.gameObject.layer = LayerMask.NameToLayer(ZOOM_LAYER);
        abilityIcon.GetComponent<AbilityIconDisplay>().AbilityScript = cardAbility;
    }

    /******
     * *****
     * ****** CREATE_NEXT_LEVEL_POPUP
     * *****
     *****/
    private void CreateNextLevelPopup(Vector2 vec2, float scaleValue, List<CardAbility> level2Abilities)
    {
        NextLevelPopup = CreateZoomObject(nextLevelBox, new Vector3(vec2.x, vec2.y, -4), background.transform, scaleValue);
        CreateZoomObject(level2Popup, new Vector2(0, 0), NextLevelPopup.transform, scaleValue / 3);
        foreach (CardAbility cardAbility in level2Abilities)
        {
            if (cardAbility == null) continue;
            CreateZoomAbilityIcon(cardAbility, NextLevelPopup.transform, scaleValue);
        }
    }

    /******
     * *****
     * ****** CREATE_LORE_POPUP
     * *****
     *****/
    private void CreateDescriptionPopup(Vector2 vec2, float scaleValue)
    {
        DescriptionPopup = CreateZoomObject(descriptionPopupPrefab, new Vector3(vec2.x, vec2.y, 0), background.transform, scaleValue);
        DescriptionPopup.GetComponent<LorePopupDisplay>().DisplayLorePopup(cardDisplay.CardScript.CardDescription);
    }
}
