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

    private UIManager uMan;
    private CombatManager coMan;
    private CardDisplay cardDisplay;
    private List<GameObject> zoomPopups;

    /* ZONES */
    private GameObject playerHand;
    private GameObject playerZone;
    private GameObject enemyHand;
    private GameObject enemyZone;

    public static bool ZoomCardIsCentered = false;
    public const string ZOOM_CARD_TIMER = "ZoomCardTimer";
    public const string ABILITY_POPUP_TIMER = "AbilityPopupTimer";
    public GameObject UnitZoomCardPrefab { get => unitZoomCardPrefab; }
    public GameObject ActionZoomCardPrefab { get => actionZoomCardPrefab; }
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
        zoomPopups = new List<GameObject>();
    }

    /******
     * *****
     * ****** ON_CLICK
     * *****
     *****/
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard != null || ZoomCardIsCentered) return;
        if (transform.parent.gameObject == enemyHand) return;
        if (pointerEventData.button != PointerEventData.InputButton.Right)
        {
            if (!uMan.PlayerIsTargetting)
                uMan.CreateFleetingInfoPopup("Right click on a card for more information!", true);
            return;
        }
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
        if (uMan.PlayerIsTargetting) return;

        GameObject parent = null;
        GameObject container = GetComponent<CardDisplay>().CardContainer;
        if (container != null) parent = container.transform.parent.gameObject;

        float cardYPos;
        float popupXPos;

        if (parent == playerHand) ShowZoomCard(playerHand);
        else FunctionTimer.Create(() => ShowZoomCard(parent), 0.5f, ZOOM_CARD_TIMER);

        void ShowAbilityPopup() =>
            CreateAbilityPopups(new Vector2(popupXPos, cardYPos), SMALL_POPUP_SCALE_VALUE);

        void ShowZoomCard(GameObject parent)
        {
            RectTransform rect;
            if (parent != null)
            {
                if (parent == playerHand)
                {
                    rect = playerHand.GetComponent<RectTransform>();
                    cardYPos = rect.position.y + ZOOM_BUFFER;
                }
                else if (parent == playerZone)
                {
                    rect = playerZone.GetComponent<RectTransform>();
                    cardYPos = rect.position.y + ZOOM_BUFFER - 20;
                }
                else if (parent == enemyHand)
                {
                    rect = enemyHand.GetComponent<RectTransform>();
                    cardYPos = rect.position.y - ZOOM_BUFFER - 50;
                }
                else if (parent == enemyZone)
                {
                    rect = enemyZone.GetComponent<RectTransform>();
                    cardYPos = (int)rect.position.y - ZOOM_BUFFER;
                }
                // New Game Scene
                else if (SceneLoader.IsActiveScene(SceneLoader.Scene.HeroSelectScene)) // TESTING
                {
                    CreateAbilityPopups(new Vector2(0, 150), ZOOM_SCALE_VALUE);
                    return;
                }
                else
                {
                    Debug.LogError("INVALID PARENT!");
                    return;
                }

                Vector3 vec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (vec3.x > 0) popupXPos = vec3.x - 400;
                else popupXPos = vec3.x + 400;
                CreateZoomCard(new Vector2(vec3.x, cardYPos), ZOOM_SCALE_VALUE);
                FunctionTimer.Create(() => ShowAbilityPopup(), 0.75f, ABILITY_POPUP_TIMER);
            }
            else
            {
                // Card Page Popup
                if (uMan.CardPagePopup != null) CreateAbilityPopups(new Vector2(0, -300), POPUP_SCALE_VALUE);
                // New Card Popup
                else if (uMan.NewCardPopup != null) CreateAbilityPopups(new Vector2(500, 0), POPUP_SCALE_VALUE);
                // Choose Card Popup
                else if (uMan.ChooseCardPopup != null) CreateAbilityPopups(new Vector2(0, -300), POPUP_SCALE_VALUE);
            }
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
    private GameObject CreateZoomObject(GameObject prefab, Vector2 vec2, float scaleValue)
    {
        GameObject currentCanvas;
        if (ZoomCardIsCentered) currentCanvas = uMan.CurrentZoomCanvas;
        else currentCanvas = uMan.CurrentCanvas;
        GameObject zoomObject = Instantiate(prefab, currentCanvas.transform);
        zoomObject.transform.localPosition = vec2;
        zoomObject.transform.localScale = new Vector2(scaleValue, scaleValue);
        zoomPopups.Add(zoomObject);
        return zoomObject;
    }

    /******
     * *****
     * ****** CREATE_ZOOM_CARD
     * *****
     *****/
    private void CreateZoomCard(Vector2 vec2, float scaleValue)
    {
        if (gameObject == null) return; // Unnecessary?
        DestroyZoomCard();

        GameObject cardPrefab;
        if (GetComponent<CardDisplay>() is UnitCardDisplay) cardPrefab = unitZoomCardPrefab;
        else cardPrefab = actionZoomCardPrefab;

        CurrentZoomCard = CreateZoomObject(cardPrefab, new Vector2(vec2.x, vec2.y), scaleValue);
        CurrentZoomCard.GetComponent<CardDisplay>().DisplayZoomCard(gameObject);
    }

    private void DestroyZoomCard()
    {
        if (CurrentZoomCard != null)
        {
            zoomPopups.Remove(CurrentZoomCard);
            Destroy(CurrentZoomCard);
            CurrentZoomCard = null;
        }
    }

    private void DestroyDescriptionPopup()
    {
        if (DescriptionPopup != null)
        {
            zoomPopups.Remove(DescriptionPopup);
            Destroy(DescriptionPopup);
            DescriptionPopup = null;
        }
    }

    private void DestroyAbilityPopups()
    {
        if (AbilityPopupBox != null)
        {
            zoomPopups.Remove(AbilityPopupBox);
            Destroy(AbilityPopupBox);
            AbilityPopupBox = null;
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
        DestroyDescriptionPopup();

        DescriptionPopup = CreateZoomObject(descriptionPopupPrefab,
            new Vector2(vec2.x, vec2.y), scaleValue);
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

        DestroyAbilityPopups();
        AbilityPopupBox = CreateZoomObject(abilityPopupBoxPrefab, vec2, scaleValue);
        List<CardAbility> abilityList;
        List<CardAbility> singleList = new List<CardAbility>();

        if (cardDisplay is UnitCardDisplay ucd)
        {
            if (ZoomCardIsCentered) abilityList = ucd.UnitCard.StartingAbilities;
            else if (SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene)) // TESTING
            {
                abilityList = ucd.CurrentAbilities;
                if (ucd.IsExhausted) AddSingle(exhaustedAbility);
            }
            else abilityList = ucd.UnitCard.StartingAbilities; // TESTING
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
        foreach (CardAbility single in singleList) 
            CreatePopup(single);

        void AddSingle(CardAbility ca)
        {
            if (ca is TriggeredAbility ta)
            {
                foreach (CardAbility ca2 in singleList)
                {
                    if (ca2 is TriggeredAbility ta2)
                    {
                        if (ta.AbilityTrigger.AbilityName ==
                            ta2.AbilityTrigger.AbilityName) return;
                    }
                }
                singleList.Add(ca);
            }
            else
            {
                if (singleList.FindIndex(x => x.AbilityName ==
                ca.AbilityName) == -1)
                    singleList.Add(ca);
            }
        }
        void CreatePopup(CardAbility ca)
        {
            GameObject abilityPopup = 
                Instantiate(abilityPopupPrefab, AbilityPopupBox.transform);
            abilityPopup.GetComponent<AbilityPopupDisplay>().AbilityScript = ca;
        }
    }

    public void DestroyZoomPopups()
    {
        if (ZoomCardIsCentered) return;
        FunctionTimer.StopTimer(ZOOM_CARD_TIMER);
        foreach (GameObject go in zoomPopups)
            DestroyObject(go);

        static void DestroyObject(GameObject go)
        {
            if (go != null)
            {
                Destroy(go);
                go = null;
            }
        }
    }
}
