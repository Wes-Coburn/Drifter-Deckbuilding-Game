using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZoom : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject unitZoomCardPrefab;
    [SerializeField] private GameObject actionZoomCardPrefab;
    [SerializeField] private GameObject abilityPopupPrefab;
    [SerializeField] private GameObject abilityPopupBoxPrefab;
    [SerializeField] private GameObject descriptionPopupPrefab;
    [SerializeField] private CardAbility exhaustedAbility;
    
    private const float  ZOOM_BUFFER                 =  350;
    public const float   ZOOM_SCALE_VALUE            =  4;
    private const float  CENTER_SCALE_VALUE          =  6;
    private const float  POPUP_SCALE_VALUE           =  3;
    //private const float  MED_POPUP_SCALE_VALUE       =  2.5f;
    private const float  SMALL_POPUP_SCALE_VALUE     =  2;

    private UIManager uMan;
    private EffectManager efMan;
    private PlayerManager pMan;
    private EnemyManager enMan;

    private CardDisplay cardDisplay;
    private List<GameObject> zoomPopups;

    public static bool ZoomCardIsCentered = false;
    public const string ZOOM_CARD_TIMER = "ZoomCardTimer";
    public const string ABILITY_POPUP_TIMER = "AbilityPopupTimer";

    public static GameObject CurrentZoomCard { get; private set; }
    public static GameObject DescriptionPopup { get; private set; }
    public static GameObject AbilityPopupBox { get; private set; }

    public GameObject UnitZoomCardPrefab { get => unitZoomCardPrefab; }
    public GameObject ActionZoomCardPrefab { get => actionZoomCardPrefab; }

    private void Awake()
    {
        uMan = UIManager.Instance;
        efMan = EffectManager.Instance;
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        cardDisplay = GetComponent<CardDisplay>();
        zoomPopups = new List<GameObject>();
    }

    public static void NullifyProperties()
    {
        CurrentZoomCard = null;
        DescriptionPopup = null;
        AbilityPopupBox = null;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard != null || ZoomCardIsCentered) return;

        if (uMan.PlayerIsTargetting && !efMan.CurrentEffectGroup.Targets.PlayerHand) return;

        GameObject parent = null;
        GameObject container = GetComponent<CardDisplay>().CardContainer;
        if (container != null) parent = container.transform.parent.gameObject;
        if (enMan.HandZone != null && parent == enMan.HandZone) return;

        if (pointerEventData.button != PointerEventData.InputButton.Right) return;

        uMan.DestroyZoomObjects();
        ZoomCardIsCentered = true;
        uMan.SetScreenDimmer(true);
        CreateDescriptionPopup(new Vector2(-590, 0), POPUP_SCALE_VALUE);
        CreateAbilityPopups(new Vector2(590, 0), POPUP_SCALE_VALUE, false);
        CreateZoomCard(new Vector2(0, 50), CENTER_SCALE_VALUE);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard != null || ZoomCardIsCentered) return;

        if (uMan.PlayerIsTargetting && !efMan.CurrentEffectGroup.Targets.PlayerHand) return;

        GameObject parent = null;
        GameObject container = GetComponent<CardDisplay>().CardContainer;
        if (container != null) parent = container.transform.parent.gameObject;
        else return;

        if (parent == enMan.HandZone || parent == pMan.ActionZone || parent == enMan.ActionZone) return;

        float cardYPos;
        float popupXPos;

        if (parent == pMan.HandZone || parent == enMan.HandZone) ShowZoomCard(parent);
        else FunctionTimer.Create(() => ShowZoomCard(parent), 0.5f, ZOOM_CARD_TIMER);

        void ShowAbilityPopup() =>
            CreateAbilityPopups(new Vector2(popupXPos, cardYPos), SMALL_POPUP_SCALE_VALUE, true);

        void ShowZoomCard(GameObject parent)
        {
            if (this == null) return;

            RectTransform rect;

            if (parent == pMan.HandZone)
            {
                Vector2 bufferDistance = container.GetComponent<CardContainer>().BufferDistance;
                float bufferY = -bufferDistance.y;

                rect = pMan.HandZone.GetComponent<RectTransform>();
                cardYPos = rect.position.y + ZOOM_BUFFER + 20 + bufferY;
            }
            else if (parent == pMan.PlayZone)
            {
                rect = pMan.PlayZone.GetComponent<RectTransform>();
                cardYPos = rect.position.y + ZOOM_BUFFER - 20;
            }
            else if (parent == enMan.PlayZone)
            {
                rect = enMan.PlayZone.GetComponent<RectTransform>();
                cardYPos = (int)rect.position.y - ZOOM_BUFFER;
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
    }

    public void OnPointerExit(PointerEventData pointerEventData)
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
        GameObject zoomObject = Instantiate(prefab, uMan.CurrentZoomCanvas.transform);
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
        if (this == null) return;

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
    public GameObject CreateZoomAbilityIcon(CardAbility ca, Transform parent, float scaleValue)
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

        if (ZoomCardIsCentered)
        {
            foreach (Transform tran in zoomAbilityIcon.transform)
            {
                if (tran.gameObject.TryGetComponent(out UnityEngine.UI.Image image))
                    image.raycastTarget = true;
            }
        }
        return zoomAbilityIcon;
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

        string filteredText =
            DialogueManager.Instance.FilterText(cardDisplay.CardScript.CardDescription);
        DescriptionPopup.GetComponent<DescriptionPopupDisplay>
            ().DisplayDescriptionPopup(filteredText);
    }

    /******
     * *****
     * ****** CREATE_ABILITY_POPUPS
     * *****
     *****/
    public void CreateAbilityPopups(Vector2 vec2, float scaleValue, bool showCurrent)
    {
        if (this == null) return;

        if (uMan == null)
        {
            Debug.LogWarning("UIMANAGER IS NULL!");
            uMan = UIManager.Instance;
            cardDisplay = GetComponent<CardDisplay>();
        }

        List<CardAbility> abilityList;
        List<CardAbility> singleList = new List<CardAbility>();
        bool isPlayerSource = HeroManager.GetSourceHero(gameObject) == pMan;

        if (cardDisplay is UnitCardDisplay ucd)
        {
            if (showCurrent)
            {
                abilityList = ucd.CurrentAbilities;
                if (ucd.IsExhausted) AddSingle(exhaustedAbility);
            }
            else abilityList = ucd.UnitCard.StartingAbilities;
        }
        else if (cardDisplay is ActionCardDisplay acd)
            abilityList = acd.ActionCard.LinkedAbilities;
        else
        {
            if (cardDisplay == null) Debug.LogError("DISPLAY IS NULL!");
            else Debug.LogError("DISPLAY TYPE NOT FOUND!");
            return;
        }

        if (abilityList == null)
        {
            Debug.LogError("ABILITY LIST IS NULL!");
            return;
        }

        foreach (CardAbility ca in abilityList)
        {
            AddSingle(ca);
            foreach (CardAbility linkCa in ca.LinkedAbilites)
            {
                AddSingle(linkCa);
                foreach (CardAbility linkCa2 in linkCa.LinkedAbilites)
                    AddSingle(linkCa2);
            }
        }

        DestroyAbilityPopups();
        if (singleList.Count > 4) vec2.y = 0;
        if (singleList.Count > 9) uMan.HideSkybar(true);

        AbilityPopupBox = CreateZoomObject(abilityPopupBoxPrefab, vec2, scaleValue);
        foreach (CardAbility single in singleList)
            CreatePopup(single);

        void AddSingle(CardAbility ca)
        {
            if (ca is TriggeredAbility || ca is AbilityTrigger)
            {
                string newTriggerName = "";
                if (ca is TriggeredAbility ta) newTriggerName = ta.AbilityTrigger.AbilityName;
                else if (ca is AbilityTrigger at) newTriggerName = at.AbilityName;

                foreach (CardAbility ca2 in singleList)
                {
                    if (ca2 is TriggeredAbility ta2)
                    {
                        if (ta2.AbilityTrigger.AbilityName == newTriggerName) return;
                    }
                    else if (ca2 is AbilityTrigger at2)
                    {
                        if (at2.AbilityName == newTriggerName) return;
                    }
                }
                singleList.Add(ca);
            }
            else if (ca is ModifierAbility) { }
            else
            {
                if (singleList.FindIndex(x => x.AbilityName ==
                ca.AbilityName) == -1) singleList.Add(ca);
            }
        }
        void CreatePopup(CardAbility ca)
        {
            GameObject abilityPopup = 
                Instantiate(abilityPopupPrefab, AbilityPopupBox.transform);
            abilityPopup.GetComponent<AbilityPopupDisplay>().DisplayAbilityPopup(ca, false, isPlayerSource);
        }
    }

    public void DestroyZoomPopups()
    {
        if (ZoomCardIsCentered) return;
        foreach (GameObject go in zoomPopups)
            Destroy(go);
    }

    private void OnDestroy() => DestroyZoomPopups();
}
