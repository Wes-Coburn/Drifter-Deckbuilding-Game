using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPageDisplay : MonoBehaviour
{
    [Header("IS_SCROLL_PAGE")]
    [SerializeField] private bool isScrollPage;

    [Header("PREFABS")]
    [SerializeField] private GameObject cardPageCardContainerPrefab;
    [SerializeField] private GameObject cardShopButtonPrefab;

    [Header("REFERENCES")]
    [SerializeField] private GameObject cardGroup;
    [SerializeField] private GameObject costGroup;
    [SerializeField] private GameObject pageTitle;
    [SerializeField] private GameObject noCardsTooltip;

    [Header("PROGRESS BAR")]
    [SerializeField] private GameObject progressBar;
    [SerializeField] private GameObject progressFill;
    [SerializeField] private GameObject progressBarText;

    private PlayerManager pMan;
    private UIManager uMan;
    private AnimationManager anMan;
    private GameManager gMan;

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
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        anMan = AnimationManager.Instance;
        gMan = GameManager.Instance;

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

        string progressText;
        if (isReady) progressText = "DISCOUNT APPLIED!";
        else progressText = newProgress + "/" + goal + " " + endText;
        progressBarText.GetComponent<TextMeshProUGUI>().SetText(progressText);

        if (!(isFirstDisplay && newProgress < 1))
            AnimationManager.Instance.SetProgressBar(currentProgress, newProgress, progressBar, progressFill);
    }

    public void DisplayCardPage(CardPageType cardPageType, bool playSound, float scrollValue)
    {
        this.cardPageType = cardPageType;
        cardGroupList = new List<Card>();
        string titleText;
        bool setProgressBar = false;
        int progress = 0;

        CardManager caMan = CardManager.Instance;

        switch (cardPageType)
        {
            case CardPageType.RemoveCard:
                titleText = "Sell a Card";
                foreach (Card c in pMan.PlayerDeckList)
                    cardGroupList.Add(c);
                break;
            case CardPageType.RecruitUnit:
                setProgressBar = true;
                progress = gMan.RecruitLoyalty;
                titleText = "Recruit a Unit";
                foreach (Card c in caMan.PlayerRecruitUnits)
                    cardGroupList.Add(c);
                break;
            case CardPageType.AcquireAction:
                setProgressBar = true;
                progress = gMan.ActionShopLoyalty;
                titleText = "Acquire an Action";
                foreach (Card c in caMan.ActionShopCards)
                    cardGroupList.Add(c);
                break;
            case CardPageType.CloneUnit:
                titleText = "Clone a Unit";
                foreach (Card c in pMan.PlayerDeckList)
                    if (c is UnitCard)
                        cardGroupList.Add(c);
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return;
        }

        if (progressBar != null)
        {
            bool isReady;
            if (progress == GameManager.SHOP_LOYALTY_GOAL) isReady = true;
            else isReady = false;

            progressBar.SetActive(setProgressBar);
            if (setProgressBar) SetProgressBar(0, progress, isReady, true);
        }

        pageTitle.GetComponent<TextMeshProUGUI>().SetText(titleText);

        if (cardGroupList.Count > 0)
        {
            noCardsTooltip.SetActive(false);
            cardGroupList.Sort((s1, s2) => s1.StartEnergyCost - s2.StartEnergyCost);

            List<List<Card>> sortList = new List<List<Card>>
            {
                new List<Card>()
            };

            int currentList = 0;
            int currentCost = 0;

            foreach (Card c in cardGroupList)
            {
                int cardCost = c.StartEnergyCost;

                if (cardCost != currentCost)
                {
                    sortList.Add(new List<Card>());
                    currentList++;
                    currentCost = cardCost;
                }

                sortList[currentList].Add(c);
            }

            foreach (List<Card> cardList in sortList)
            {
                cardList.Sort((x, y) => string.Compare(x.CardName, y.CardName));
            }

            cardGroupList.Clear();
            foreach (List<Card> cardList in sortList)
                foreach (Card c in cardList) cardGroupList.Add(c);
        }
        else noCardsTooltip.SetActive(true);
        if (isScrollPage) LoadScrollPage(scrollValue);
        else LoadCardPage();
        if (playSound) AudioManager.Instance.StartStopSound("SFX_CreatePopup1");
    }

    private void LoadScrollPage(float scrollValue)
    {
        Rect rect = cardGroup.GetComponent<RectTransform>().rect;
        int rows = Mathf.CeilToInt(cardGroupList.Count / 4f);
        if (rows < 1) rows = 1;
        float height = 650 * rows + 100;
        cardGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, height);
        GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = scrollValue;

        foreach (Card card in cardGroupList)
        {
            GameObject container = Instantiate(cardPageCardContainerPrefab, cardGroup.transform);
            CardPageCardContainerDisplay cpccd = container.GetComponent<CardPageCardContainerDisplay>();
            GameObject cardPageCard =
                CombatManager.Instance.ShowCard(card, new Vector2(), CombatManager.DisplayType.Cardpage);
            CardDisplay cd = cardPageCard.GetComponent<CardDisplay>();
            cd.DisableVisuals();
            cardPageCard.transform.localScale = new Vector2(4, 4);
            cardPageCard.transform.SetParent(cpccd.CardPageCard.transform, false);
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
        foreach (Card card in cardGroupList)
        {
            GameObject cardObj = CombatManager.Instance.ShowCard
                (card, new Vector2(), CombatManager.DisplayType.Cardpage);
            cardObj.transform.SetParent(cardGroup.transform);

            CardDisplay cd = cardObj.GetComponent<CardDisplay>();
            cd.DisableVisuals();
            cardObj.transform.localScale = new Vector2(4, 4);
            GameObject button = CreateCardPageButton(card, costGroup);
            if (button == null) return;
        }
    }

    private GameObject CreateCardPageButton(Card card, GameObject parent)
    {
        GameObject button = Instantiate(cardShopButtonPrefab, parent.transform);
        button.GetComponent<CardShopButton>().SetCard(card, cardPageType);
        return button;
    }
    public void CloseCardPageButton_OnClick()
    {
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
            DialogueManager.Instance.DisplayDialoguePopup();

        uMan.DestroyCardPage(true);
        uMan.DestroyInteractablePopup(gameObject);
    }
}
