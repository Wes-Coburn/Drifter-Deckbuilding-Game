using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZoom : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject abilityPopupPrefab, abilityPopupBoxPrefab, descriptionPopupPrefab;
    [SerializeField] private GameObject relatedCards_PreviousButton, relatedCards_NextButton, relatedCardsBG;
    [SerializeField] private CardAbility exhaustedAbility;

    private const float ZOOM_BUFFER = 350;
    public const float ZOOM_SCALE_VALUE = 4;
    private const float CENTER_SCALE_VALUE = 6;
    private const float POPUP_SCALE_VALUE = 3;
    //private const float MED_POPUP_SCALE_VALUE =  2.5f;
    private const float SMALL_POPUP_SCALE_VALUE = 2;

    private CardDisplay cardDisplay;
    private List<GameObject> zoomPopups;

    private Card RelatedCard => BaseZoomCard == null ? null :
        (ActiveZoomCard < 1 ? null : BaseZoomCard.RelatedCards[ActiveZoomCard - 1]);
    private Card GetActiveZoomCard() => BaseZoomCard == null ? cardDisplay.CardScript :
        (RelatedCard == null ? BaseZoomCard : RelatedCard);
    
    public static bool ZoomCardIsCentered = false;
    public const string ZOOM_CARD_TIMER = "ZoomCardTimer";
    public const string ABILITY_POPUP_TIMER = "AbilityPopupTimer";

    public static Card BaseZoomCard { get; set; } // Used to distinguish the original card from related cards
    public static int ActiveZoomCard { get; set; } // 0 for baseZoomCard, 1..n for related cards
    public static GameObject CurrentZoomCard { get; private set; }
    public static GameObject DescriptionPopup { get; private set; }
    public static GameObject AbilityPopupBox { get; private set; }

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        zoomPopups = new();
        if (relatedCards_PreviousButton != null) relatedCards_PreviousButton.SetActive(false);
        if (relatedCards_NextButton != null) relatedCards_NextButton.SetActive(false);
        if (relatedCardsBG != null) relatedCardsBG.SetActive(false);
    }

    public static void NullifyProperties()
    {
        CurrentZoomCard = null;
        DescriptionPopup = null;
        AbilityPopupBox = null;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Right) return;

        if (DragDrop.DraggingCard != null || ZoomCardIsCentered ||
            (Managers.U_MAN.PlayerIsTargetting &&
            !Managers.EF_MAN.CurrentEffectGroup.Targets.PlayerHand)) return;

        GameObject parent = null;
        var container = GetComponent<CardDisplay>().CardContainer;
        if (container != null) parent = container.transform.parent.gameObject;

        if (Managers.EN_MAN.HandZone != null && parent == Managers.EN_MAN.HandZone) return;

        Managers.U_MAN.DestroyZoomObjects();
        ZoomCardIsCentered = true;
        Managers.U_MAN.SetScreenDimmer(true);

        DisplayCenteredZoomCard(true);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard != null || ZoomCardIsCentered ||
            (Managers.U_MAN.PlayerIsTargetting &&
            !Managers.EF_MAN.CurrentEffectGroup.Targets.PlayerHand)) return;

        float popYPos = 0;
        float popXPos = 0;
        GameObject parent = null;

        var container = GetComponent<CardDisplay>().CardContainer;
        if (container != null) parent = container.transform.parent.gameObject;
        else
        {
            // Buffer for card page cards, avoiding related cards in hero select scene
            bool isCardPageCard = !FindObjectOfType<HeroSelectSceneDisplay>();
            int buffer = isCardPageCard ? 300 : 0;

            popYPos = transform.position.y + buffer;
            popXPos = transform.position.x;

            if (isCardPageCard)
            {
                if (popYPos > buffer) popYPos = buffer;
                else if (popYPos < -buffer) popYPos = -buffer;
            }

            ShowAbilityPopup(SMALL_POPUP_SCALE_VALUE);
            return;
        }

        if (parent == Managers.EN_MAN.HandZone || parent == Managers.EN_MAN.ActionZone ||
            parent == Managers.P_MAN.ActionZone) return;

        if (parent == Managers.P_MAN.HandZone || parent == Managers.EN_MAN.HandZone) ShowZoomCard(parent);
        else FunctionTimer.Create(() => ShowZoomCard(parent), 0.5f, ZOOM_CARD_TIMER);

        void ShowAbilityPopup(float scale) => CreateAbilityPopups(new Vector2(popXPos, popYPos), scale, true);

        void ShowZoomCard(GameObject parent)
        {
            if (parent == null) return;

            RectTransform rect;

            if (parent == Managers.P_MAN.HandZone)
            {
                Vector2 bufferDistance = container.GetComponent<CardContainer>().BufferDistance;
                float bufferY = -bufferDistance.y;

                rect = Managers.P_MAN.HandZone.GetComponent<RectTransform>();
                popYPos = rect.position.y + ZOOM_BUFFER + 20 + bufferY;
            }
            else if (parent == Managers.P_MAN.PlayZone)
            {
                rect = Managers.P_MAN.PlayZone.GetComponent<RectTransform>();
                popYPos = rect.position.y + ZOOM_BUFFER - 20;
            }
            else if (parent == Managers.EN_MAN.PlayZone)
            {
                rect = Managers.EN_MAN.PlayZone.GetComponent<RectTransform>();
                popYPos = rect.position.y - ZOOM_BUFFER;
            }
            else
            {
                Debug.LogError("INVALID PARENT!");
                return;
            }

            Vector3 vec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            popXPos = vec3.x + (vec3.x > 0 ? -400 : 400);
            CreateZoomCard(new Vector2(vec3.x, popYPos), ZOOM_SCALE_VALUE);
            FunctionTimer.Create(() => ShowAbilityPopup(SMALL_POPUP_SCALE_VALUE), 0.75f, ABILITY_POPUP_TIMER);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard != null || ZoomCardIsCentered) return;
        Managers.U_MAN.DestroyZoomObjects();
    }

    /******
     * *****
     * ****** RELATED_CARDS
     * *****
     *****/
    public void SetRelatedCards(Card baseZoomCard)
    {
        if (baseZoomCard != null) BaseZoomCard = baseZoomCard;
        bool hasRelatedCards = BaseZoomCard.RelatedCards.Length > 0;
        relatedCardsBG.SetActive(hasRelatedCards);

        relatedCards_NextButton.SetActive(ActiveZoomCard < BaseZoomCard.RelatedCards.Length);
        relatedCards_PreviousButton.SetActive(ActiveZoomCard > 0);
    }
    public void RelatedCards_PreviousButton_OnClick()
    {
        Managers.AU_MAN.StartStopSound("SFX_Click1");
        ActiveZoomCard--;
        DisplayCenteredZoomCard();
    }
    public void RelatedCards_NextButton_OnClick()
    {
        Managers.AU_MAN.StartStopSound("SFX_Click1");
        ActiveZoomCard++;
        DisplayCenteredZoomCard();
    }

    /******
     * *****
     * ****** CREATE_ZOOM_OBJECT
     * *****
     *****/
    private GameObject CreateZoomObject(GameObject prefab, Vector2 vec2, float scaleValue)
    {
        var zoomObject = Instantiate(prefab, Managers.U_MAN.CurrentZoomCanvas.transform);
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
    private void CreateZoomCard(Vector2 vec2, float scaleValue, bool isBaseZoomCard = false)
    {
        var cm = Managers.CA_MAN;
        DestroyZoomCard();

        // Determine the Zoom Card Prefab
        GameObject cardPrefab;
        // For centered zoom cards, use the BaseZoomCard
        if (ZoomCardIsCentered) cardPrefab = GetActiveZoomCard() is UnitCard ?
                cm.UnitZoomCardPrefab : cm.ActionZoomCardPrefab;
        // For non-centered zoom card popups, use the current gameObject
        else cardPrefab = GetComponent<CardDisplay>() is UnitCardDisplay ?
                cm.UnitZoomCardPrefab : cm.ActionZoomCardPrefab;

        // Create the Zoom Card
        CurrentZoomCard = CreateZoomObject(cardPrefab, vec2, scaleValue);

        // Display the Zoom Card
        var cd = CurrentZoomCard.GetComponent<CardDisplay>();
        // For centered zoom cards, use the BaseZoomCard, set the BaseZoomCard if needed, and set the related cards
        if (ZoomCardIsCentered) cd.DisplayZoomCard(GetActiveZoomCard(), isBaseZoomCard); // TESTING
        // For non-centered zoom card popups, use the current gameObject
        else cd.DisplayZoomCard(gameObject);
    }

    /******
     * *****
     * ****** CREATE_ZOOM_ABILLITY_ICON
     * *****
     *****/
    public GameObject CreateZoomAbilityIcon(CardAbility ca, Transform parent, float scaleValue)
    {
        if (Managers.U_MAN == null)
        {
            Debug.LogWarning("UIMANAGER IS NULL!");
            cardDisplay = GetComponent<CardDisplay>();
        }

        var zoomIconPrefab = GetComponent<UnitCardDisplay>().ZoomAbilityIconPrefab;
        var zoomAbilityIcon = Instantiate(zoomIconPrefab, parent);
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
        if (Managers.U_MAN == null)
        {
            Debug.LogWarning("UIMANAGER IS NULL!");
            cardDisplay = GetComponent<CardDisplay>();
        }
        DestroyDescriptionPopup();
        DescriptionPopup = CreateZoomObject(descriptionPopupPrefab, new Vector2(vec2.x, vec2.y), scaleValue);

        //string filteredText = Managers.D_MAN.FilterText(cardDisplay.CardScript.CardDescription);
        string filteredText = Managers.D_MAN.FilterText(GetActiveZoomCard().CardDescription); // TESTING
        DescriptionPopup.GetComponent<DescriptionPopupDisplay>().DisplayDescriptionPopup(filteredText);
    }

    /******
     * *****
     * ****** CREATE_ABILITY_POPUPS
     * *****
     *****/
    public void CreateAbilityPopups(Vector2 popPosition, float scaleValue, bool showCurrent)
    {
        if (Managers.U_MAN == null)
        {
            Debug.LogError("UIMANAGER IS NULL!");
            cardDisplay = GetComponent<CardDisplay>();
        }

        List<CardAbility> abilityList;
        List<CardAbility> singleList = new();
        bool isPlayerSource = HeroManager.GetSourceHero(gameObject) == Managers.P_MAN;

        if (RelatedCard == null) // Original Zoom Card
        {
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
                Debug.LogError("INVALID DISPLAY TYPE!");
                return;
            }
        }
        else // Related Card Zoom
        {
            if (RelatedCard is UnitCard uc) abilityList = uc.StartingAbilities;
            else if (RelatedCard is ActionCard ac) abilityList = ac.LinkedAbilities;
            else
            {
                Debug.LogError("INVALID CARD TYPE!");
                return;
            }
        }

        foreach (var ca in abilityList)
        {
            AddSingle(ca);
            foreach (var linkCa in ca.LinkedAbilites)
            {
                AddSingle(linkCa);
                foreach (var linkCa2 in linkCa.LinkedAbilites)
                    AddSingle(linkCa2);
            }
        }

        DestroyAbilityPopups();
        if (singleList.Count > 4) popPosition.y = 0; // Long list
        if (singleList.Count > 9) Managers.U_MAN.HideSkybar(true); // Xtra-Long List

        AbilityPopupBox = CreateZoomObject(abilityPopupBoxPrefab, popPosition, scaleValue);
        foreach (var single in singleList) CreatePopup(single);

        void AddSingle(CardAbility ca)
        {
            if (ca is TriggeredAbility || ca is AbilityTrigger)
            {
                string newTriggerName = "";
                if (ca is TriggeredAbility ta) newTriggerName = ta.AbilityTrigger.AbilityName;
                else if (ca is AbilityTrigger at) newTriggerName = at.AbilityName;

                foreach (var ca2 in singleList)
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
            else if (ca is not ModifierAbility)
            {
                if (singleList.FindIndex(x => x.AbilityName ==
                ca.AbilityName) == -1) singleList.Add(ca);
            }
        }
        void CreatePopup(CardAbility ca)
        {
            var abilityPopup = Instantiate(abilityPopupPrefab, AbilityPopupBox.transform);
            abilityPopup.GetComponent<AbilityPopupDisplay>().DisplayAbilityPopup(ca, false, isPlayerSource);
        }
    }

    /******
     * *****
     * ****** DISPLAY_CENTERED_ZOOM_CARD
     * *****
     *****/
    private void DisplayCenteredZoomCard(bool isBaseZoomCard = false)
    {
        if (isBaseZoomCard) ActiveZoomCard = 0;
        CreateDescriptionPopup(new Vector2(-590, 0), POPUP_SCALE_VALUE);
        CreateAbilityPopups(new Vector2(590, 0), POPUP_SCALE_VALUE, false);
        CreateZoomCard(new Vector2(0, 50), CENTER_SCALE_VALUE, isBaseZoomCard);
    }

    public void DestroyZoomPopups()
    {
        if (ZoomCardIsCentered) return;

        foreach (var go in zoomPopups) Destroy(go);
        FunctionTimer.StopTimer(ZOOM_CARD_TIMER); // TESTING
        FunctionTimer.StopTimer(ABILITY_POPUP_TIMER); // TESTING
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

    private void OnDestroy() => DestroyZoomPopups();
}
