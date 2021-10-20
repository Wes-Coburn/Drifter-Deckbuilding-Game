using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZoom : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitZoomCardPrefab;
    [SerializeField] private GameObject actionZoomCardPrefab;
    [SerializeField] private GameObject abilityBoxPrefab;
    [SerializeField] private GameObject abilityPopupPrefab;
    [SerializeField] private GameObject abilityPopupBoxPrefab;
    [SerializeField] private GameObject descriptionPopupPrefab;
    [SerializeField] private CardAbility exhaustedAbility;
    
    private const float  ZOOM_BUFFER                 =  350;
    private const float  ZOOM_SCALE_VALUE            =  4;
    private const float  CENTER_SCALE_VALUE          =  6;
    private const float  POPUP_SCALE_VALUE           =  3;
    private const float  SMALL_POPUP_SCALE_VALUE     =  2;

    public const string ZOOM_CARD_TIMER      =  "ZoomCardTimer";
    public const string ABILITY_POPUP_TIMER  =  "AbilityPopupTimer";

    public GameObject UnitZoomCardPrefab { get => unitZoomCardPrefab; }
    public GameObject ActionZoomCardPrefab { get => actionZoomCardPrefab; }

    private UIManager uMan;
    private CombatManager coMan;

    /* ZONES */
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
     * ****** AWAKE
     * *****
     *****/
    private void Awake()
    {
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        playerHand = coMan.PlayerHand;
        playerZone = coMan.PlayerZone;
        enemyHand = coMan.EnemyHand;
        enemyZone = coMan.EnemyZone;
        cardDisplay = GetComponent<CardDisplay>();
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
        if (DragDrop.DraggingCard != null || ZoomCardIsCentered || 
            uMan.PlayerIsTargetting || transform.parent.gameObject == enemyHand) return;
        
        uMan.DestroyZoomObjects();
        ZoomCardIsCentered = true;
        uMan.SetScreenDimmer(true);

        CreateDescriptionPopup(new Vector2(-590, 0), POPUP_SCALE_VALUE);
        CreateAbilityPopups(new Vector2(590, 0), POPUP_SCALE_VALUE);
        CreateZoomCard(new Vector2(0, 50), CENTER_SCALE_VALUE);
    }

    /******
     * *****
     * ****** ON_POINTER_ENTER
     * *****
     *****/
    public void OnPointerEnter()
    {
        if (DragDrop.DraggingCard != null || ZoomCardIsCentered) return;
        if (uMan.PlayerIsTargetting && !uMan.PlayerIsDiscarding) return;

        GameObject container = GetComponent<CardDisplay>().CardContainer;
        GameObject parent = container.transform.parent.gameObject;
        if (parent == enemyHand) return; // HIDE THE ENEMY HAND

        float cardYPos;
        float popupXPos;

        if (parent == playerHand) ShowZoomCard(playerHand);
        else FunctionTimer.Create(() => ShowZoomCard(parent), 0.5f, ZOOM_CARD_TIMER);

        void ShowAbilityPopup() =>
            CreateAbilityPopups(new Vector2(popupXPos, cardYPos), SMALL_POPUP_SCALE_VALUE);

        void ShowZoomCard(GameObject parent)
        {
            if (parent == null) return; // If Unit has been destroyed
            RectTransform rect;
            if (parent == playerHand)
            {
                rect = playerHand.GetComponent<RectTransform>();
                cardYPos = rect.position.y + ZOOM_BUFFER;
            }
            else if (parent == playerZone)
            {
                rect = playerZone.GetComponent<RectTransform>();
                cardYPos = rect.position.y + ZOOM_BUFFER;
            }
            else if (parent == enemyZone)
            {
                rect = enemyZone.GetComponent<RectTransform>();
                cardYPos = (int)rect.position.y - ZOOM_BUFFER;
            }
            // New Game Scene
            else if (heroSkills != null && 
                transform.parent.parent.gameObject == heroSkills)
            {
                CreateAbilityPopups(new Vector2(0, 150), ZOOM_SCALE_VALUE);
                return;
            }
            // Card Page Popup
            else if (uMan.CardPagePopup != null)
            {
                CreateAbilityPopups(new Vector2(0, -300), POPUP_SCALE_VALUE);
                return;
            }
            // New Card Popup
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
    }

    /******
     * *****
     * ****** ON_POINTER_EXIT
     * *****
     *****/
    public void OnPointerExit()
    {
        if (DragDrop.DraggingCard != null || ZoomCardIsCentered) return;
        uMan.DestroyZoomObjects();
    }

    /******
     * *****
     * ****** CREATE_ZOOM_OBJECT
     * *****
     *****/
    private GameObject CreateZoomObject(GameObject prefab, Vector2 vec2, Transform parent, float scaleValue)
    {
        GameObject zoomObject = Instantiate(prefab, parent);
        zoomObject.transform.localPosition = vec2;
        zoomObject.transform.localScale = new Vector2(scaleValue, scaleValue);
        return zoomObject;
    }

    /******
     * *****
     * ****** CREATE_ZOOM_CARD
     * *****
     *****/
    private void CreateZoomCard(Vector2 vec2, float scaleValue)
    {
        if (gameObject == null) return; // TESTING
        DestroyZoomCard(); // TESTING

        GameObject cardPrefab;
        if (GetComponent<CardDisplay>() is UnitCardDisplay)
            cardPrefab = unitZoomCardPrefab;
        else if (GetComponent<CardDisplay>() is ActionCardDisplay)
            cardPrefab = actionZoomCardPrefab;
        else
        {
            Debug.LogError("CARD DISPLAY TYPE NOT FOUND!");
            return;
        }
        CurrentZoomCard = CreateZoomObject(cardPrefab, 
            new Vector2(vec2.x, vec2.y), uMan.CurrentCanvas.transform, scaleValue);
        CurrentZoomCard.GetComponent<CardDisplay>().DisplayZoomCard(gameObject);
    }

    public void DestroyZoomCard()
    {
        if (CurrentZoomCard != null)
        {
            Destroy(CurrentZoomCard);
            CurrentZoomCard = null;
        }
    }

    /******
     * *****
     * ****** CREATE_ZOOM_ABILLITY_ICON
     * *****
     *****/
    public void CreateZoomAbilityIcon(CardAbility ca, Transform parent, float scaleValue)
    {
        if (uMan == null)
        {
            Debug.LogWarning("UIMANAGER IS NULL!");
            uMan = UIManager.Instance;
            cardDisplay = GetComponent<CardDisplay>();
        }

        GameObject zoomIconPrefab = GetComponent<UnitCardDisplay>().ZoomAbilityIconPrefab;
        GameObject zoomAbilityIcon = Instantiate(zoomIconPrefab, parent);
        zoomAbilityIcon.transform.localScale = new Vector2(scaleValue, scaleValue);
        zoomAbilityIcon.GetComponent<AbilityIconDisplay>().ZoomAbilityScript = ca;
    }

    /******
     * *****
     * ****** CREATE_DESCRIPTION_POPUP
     * *****
     *****/
    public void CreateDescriptionPopup(Vector2 vec2, float scaleValue)
    {
        if (uMan == null)
        {
            Debug.LogWarning("UIMANAGER IS NULL!");
            uMan = UIManager.Instance;
            cardDisplay = GetComponent<CardDisplay>();
        }
        if (DescriptionPopup != null)
        {
            Destroy(DescriptionPopup);
            DescriptionPopup = null;
        }

        DescriptionPopup = CreateZoomObject(descriptionPopupPrefab,
            new Vector2(vec2.x, vec2.y), uMan.CurrentCanvas.transform, scaleValue);
        DescriptionPopup.GetComponent<DescriptionPopupDisplay>
            ().DisplayDescriptionPopup(cardDisplay.CardScript.CardDescription);
    }

    /******
     * *****
     * ****** CREATE_ABILITY_POPUPS
     * *****
     *****/
    public void CreateAbilityPopups(Vector2 vec2, float scaleValue)
    {
        if (uMan == null)
        {
            Debug.LogWarning("UIMANAGER IS NULL!");
            uMan = UIManager.Instance;
            cardDisplay = GetComponent<CardDisplay>();
        }
        if (AbilityPopupBox != null)
        {
            Destroy(AbilityPopupBox);
            AbilityPopupBox = null;
        }

        AbilityPopupBox = CreateZoomObject(abilityPopupBoxPrefab, 
            vec2, uMan.CurrentCanvas.transform, scaleValue);

        List<CardAbility> abilityList;
        List<CardAbility> singleList = new List<CardAbility>();

        if (cardDisplay is UnitCardDisplay ucd)
        {
            if (ZoomCardIsCentered)
                abilityList = ucd.UnitCard.StartingAbilities;
            else
            {
                abilityList = ucd.CurrentAbilities;
                if (ucd.IsExhausted) AddSingle(exhaustedAbility);
            }
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
            AddSingle(ca);
            foreach (CardAbility linkCa in ca.LinkedAbilites)
                AddSingle(linkCa);
        }
        foreach (CardAbility singleCA in singleList) 
            CreatePopup(singleCA);

        void AddSingle(CardAbility ca)
        {
            if (!singleList.Contains(ca))
                singleList.Add(ca);
        }
        void CreatePopup(CardAbility ca)
        {
            Debug.Log("CreatePopup(): " + ca.AbilityName);
            GameObject abilityPopup = 
                Instantiate(abilityPopupPrefab, AbilityPopupBox.transform);
            abilityPopup.GetComponent<AbilityPopupDisplay>().AbilityScript = ca;
        }
    }
}
