using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPageDisplay : MonoBehaviour
{
    [Header("IS_SCROLL_PAGE")]
    [SerializeField] private bool isScrollPage;

    [Header("PREFABS")]
    [SerializeField] private GameObject cardPageCardContainerPrefab;
    [SerializeField] private GameObject cardShopButtonPrefab;

    [Header("REFERENCES")]
    [SerializeField] private GameObject cardGroup;
    [SerializeField] private GameObject costGroup, pageTitle, noCardsTooltip;

    [Header("PROGRESS BAR")]
    [SerializeField] private GameObject progressBar;
    [SerializeField] private GameObject progressFill, progressBarText;

    private Scrollbar scrollbar;

    private List<Card> cardGroupList;
    private CardPageType cardPageType;

    public enum CardPageType
    {
        RemoveCard,
        RecruitUnit,
        AcquireAction,
        CloneUnit,
    }

    public bool IsScrollPage { get => isScrollPage; }

    private void Awake()
    {
        if (isScrollPage)
        {
            scrollbar = GetComponentInChildren<Scrollbar>();
            if (scrollbar == null) Debug.LogError("SCROLLBAR IS NULL!");
        }
    }

    private void Update()
    {
        if (!isScrollPage) return;
        float newValue = scrollbar.value + Input.mouseScrollDelta.y * 0.05f;
        if (newValue > 1) newValue = 1;
        else if (newValue < 0) newValue = 0;
        scrollbar.value = newValue;
    }

    public void SetProgressBar(int currentProgress, int newProgress, bool isReady, bool isFirstDisplay = false)
    {
        int goal;
        string endText;
        switch (cardPageType)
        {
            case CardPageType.RecruitUnit:
                goal = GameManager.RECRUIT_LOYALTY_GOAL;
                endText = "Units Recruited";
                break;
            case CardPageType.AcquireAction:
                goal = GameManager.ACTION_LOYALTY_GOAL;
                endText = "Actions Acquired";
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return;
        }

        string progressText = isReady ? "DISCOUNT APPLIED!" : newProgress + "/" + goal + " " + endText;
        progressBarText.GetComponent<TextMeshProUGUI>().SetText(progressText);

        if (!(isFirstDisplay && newProgress < 1))
            Managers.AN_MAN.SetProgressBar(currentProgress, newProgress, progressBar, progressFill);
    }

    public void DisplayCardPage(CardPageType cardPageType, bool isReload, float scrollValue)
    {
        this.cardPageType = cardPageType;
        cardGroupList = new();
        string titleText;
        bool setProgressBar = false;
        int progress = 0;

        switch (cardPageType)
        {
            case CardPageType.RemoveCard:
                titleText = "Sell a Card";
                foreach (Card c in Managers.P_MAN.DeckList)
                    cardGroupList.Add(c);
                break;
            case CardPageType.RecruitUnit:
                setProgressBar = true;
                progress = Managers.G_MAN.RecruitLoyalty;
                titleText = "Recruit a Unit";
                foreach (Card c in Managers.CA_MAN.PlayerRecruitUnits)
                    cardGroupList.Add(c);
                break;
            case CardPageType.AcquireAction:
                setProgressBar = true;
                progress = Managers.G_MAN.ActionShopLoyalty;
                titleText = "Acquire an Action";
                foreach (Card c in Managers.CA_MAN.ActionShopCards)
                    cardGroupList.Add(c);
                break;
            case CardPageType.CloneUnit:
                titleText = "Clone a Unit";
                foreach (Card c in Managers.P_MAN.DeckList)
                    if (c is UnitCard) cardGroupList.Add(c);
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return;
        }

        if (progressBar != null)
        {
            bool isReady = progress == GameManager.SHOP_LOYALTY_GOAL;
            progressBar.SetActive(setProgressBar);
            if (setProgressBar) SetProgressBar(0, progress, isReady, true);
        }

        pageTitle.GetComponent<TextMeshProUGUI>().SetText(titleText);

        if (cardGroupList.Count > 0)
        {
            noCardsTooltip.SetActive(false);
            cardGroupList = cardGroupList.OrderBy(c => c.StartEnergyCost)
                              .ThenBy(c => c.CardName)
                              .ToList();
        }
        else noCardsTooltip.SetActive(true);
        if (isScrollPage) LoadScrollPage(scrollValue);
        else LoadCardPage();
        if (!isReload) Managers.AU_MAN.StartStopSound("SFX_CreatePopup1");
    }

    private void LoadScrollPage(float scrollValue)
    {
        var rect = cardGroup.GetComponent<RectTransform>().rect;
        int rows = Mathf.CeilToInt(cardGroupList.Count / 4f);
        if (rows < 1) rows = 1;
        float height = 650 * rows + 100;
        cardGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, height);
        GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = scrollValue;

        foreach (Card card in cardGroupList)
        {
            var container = Instantiate(cardPageCardContainerPrefab, cardGroup.transform);
            var cpccd = container.GetComponent<CardPageCardContainerDisplay>();
            var cardPageCard = Managers.CA_MAN.ShowCard(card, new Vector2(), CardManager.DisplayType.Cardpage);
            var cd = cardPageCard.GetComponent<CardDisplay>();
            cd.DisableVisuals();
            cardPageCard.transform.localScale = new Vector2(4, 4);
            cardPageCard.transform.SetParent(cpccd.CardPageCard.transform, false);
            cardPageCard.transform.localPosition = new Vector2(); // TESTING
            CreateCardPageButton(card, cpccd.CardCostButton);
        }
    }

    private void LoadCardPage()
    {
        if (cardGroupList.Count > 4)
        {
            Debug.LogError("MORE THAN 4 CARDS ON CARD PAGE!");
            return;
        }
        foreach (var card in cardGroupList)
        {
            var cardObj = Managers.CA_MAN.ShowCard(card, new Vector2(), CardManager.DisplayType.Cardpage);
            cardObj.transform.SetParent(cardGroup.transform);

            var cd = cardObj.GetComponent<CardDisplay>();
            cd.DisableVisuals();
            cardObj.transform.localScale = new Vector2(4, 4);
            var button = CreateCardPageButton(card, costGroup);
            if (button == null) return;
        }
    }

    private GameObject CreateCardPageButton(Card card, GameObject parent)
    {
        var button = Instantiate(cardShopButtonPrefab, parent.transform);
        button.GetComponent<CardShopButton>().SetCard(card, cardPageType);
        return button;
    }

    public void CloseCardPageButton_OnClick()
    {
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
            Managers.D_MAN.DisplayDialoguePopup();

        Managers.U_MAN.DestroyCardPage(true);
        Managers.U_MAN.DestroyInteractablePopup(gameObject);
    }
}
