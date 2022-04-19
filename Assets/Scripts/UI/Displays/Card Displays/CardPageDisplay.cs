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
    [SerializeField] private GameObject learnSkillButtonPrefab;
    [SerializeField] private GameObject recruitUnitButtonPrefab;
    [SerializeField] private GameObject removeCardButtonPrefab;
    [SerializeField] private GameObject cloneUnitButtonPrefab;

    [Header("REFERENCES")]
    [SerializeField] private GameObject cardGroup;
    [SerializeField] private GameObject costGroup;
    [SerializeField] private GameObject pageTitle;
    [SerializeField] private GameObject pageTabs;
    [SerializeField] private GameObject noCardsTooltip;

    [Header("PROGRESS BAR")]
    [SerializeField] private GameObject progressBar;
    [SerializeField] private GameObject progressFill;
    [SerializeField] private GameObject progressBarText;

    private PlayerManager pMan;
    private UIManager uMan;
    private AnimationManager anMan;

    private Scrollbar scrollbar;

    private List<Card> cardGroupList;
    private CardPageType cardPageType;

    public enum CardPageType
    {
        LearnSkill,
        RemoveMainCard,
        RemoveSkillCard,
        RecruitUnit,
        CloneUnit,
    }

    public bool IsScrollPage { get => isScrollPage; }
    private void Awake()
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        anMan = AnimationManager.Instance;
        if (isScrollPage)
        {
            scrollbar = GetComponentInChildren<Scrollbar>();
            if (scrollbar == null) Debug.LogError("SCROLLBAR IS NULL!");
        }
    }
    private void Update()
    {
        if (!isScrollPage) return;
        float newValue = scrollbar.value + Input.mouseScrollDelta.y * 0.1f;
        if (newValue > 1) newValue = 1;
        else if (newValue < 0) newValue = 0;
        scrollbar.value = newValue;
    }
    public void SetProgressBar(int currentProgress, int newProgress, bool isReady, bool isFirstDisplay = false)
    {
        string progressText;
        if (isReady) progressText = "DISCOUNT APPLIED!";
        else progressText = newProgress + "/" + GameManager.RECRUIT_LOYALTY_GOAL + " Units Recruited";
        progressBarText.GetComponent<TextMeshProUGUI>().SetText(progressText);

        if (isFirstDisplay && newProgress < 1) return;
        AnimationManager.Instance.SetProgressBar(AnimationManager.ProgressBarType.Recruit,
            currentProgress, newProgress, isReady, progressBar, progressFill);
    }
    public void DisplayCardPage(CardPageType cardPageType, bool playSound, float scrollValue)
    {
        this.cardPageType = cardPageType;
        cardGroupList = new List<Card>();
        string titleText;
        bool setProgressBar = false;
        bool showTabs = true;
        int progress = 0;

        switch (cardPageType)
        {
            case CardPageType.LearnSkill:
                titleText = "Learn a Skill";
                foreach (Card c in pMan.PlayerHero.HeroSkills)
                    cardGroupList.Add(c);
                break;
            case CardPageType.RemoveMainCard:
                titleText = "Remove a Card - Main Deck";
                foreach (Card c in pMan.PlayerDeckList)
                {
                    if (c is SkillCard) continue;
                    else cardGroupList.Add(c);
                }
                break;
            case CardPageType.RemoveSkillCard:
                titleText = "Remove a Card - Skill Deck";
                foreach (Card c in pMan.PlayerDeckList)
                    if (c is SkillCard) cardGroupList.Add(c);
                break;
            case CardPageType.RecruitUnit:
                setProgressBar = true;
                showTabs = false;
                progress = GameManager.Instance.RecruitLoyalty;
                titleText = "Recruit a Unit";
                foreach (Card c in CardManager.Instance.PlayerRecruitUnits)
                    cardGroupList.Add(c);
                break;
            case CardPageType.CloneUnit:
                showTabs = false;
                titleText = "Clone a Unit";
                foreach (Card c in pMan.PlayerDeckList)
                    if (c is UnitCard)
                        cardGroupList.Add(c);
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return;
        }

        if (progressBar != null) progressBar.SetActive(setProgressBar);
        if (setProgressBar) SetProgressBar(0, progress, false, true);
        pageTitle.GetComponent<TextMeshProUGUI>().SetText(titleText);

        if (cardGroupList.Count > 0)
        {
            cardGroupList.Sort((x, y) => string.Compare(x.CardName, y.CardName));
            cardGroupList.Sort((s1, s2) => s1.StartEnergyCost - s2.StartEnergyCost);
            noCardsTooltip.SetActive(false);
        }
        else noCardsTooltip.SetActive(true);
        if (isScrollPage) LoadScrollPage(scrollValue, showTabs);
        else LoadCardPage();
        if (playSound) AudioManager.Instance.StartStopSound("SFX_CreatePopup1");
    }

    private void LoadScrollPage(float scrollValue, bool showTabs)
    {
        pageTabs.SetActive(showTabs);
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
        GameObject buttonPrefab;
        switch (cardPageType)
        {
            case CardPageType.LearnSkill:
                buttonPrefab = learnSkillButtonPrefab;
                break;
            case CardPageType.RemoveMainCard:
                buttonPrefab = removeCardButtonPrefab;
                break;
            case CardPageType.RemoveSkillCard:
                buttonPrefab = removeCardButtonPrefab;
                break;
            case CardPageType.RecruitUnit:
                buttonPrefab = recruitUnitButtonPrefab;
                break;
            case CardPageType.CloneUnit:
                buttonPrefab = cloneUnitButtonPrefab;
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return null;
        }

        GameObject button = Instantiate(buttonPrefab, parent.transform);
        switch (cardPageType)
        {
            case CardPageType.LearnSkill:
                button.GetComponent<LearnSkillButton>().SkillCard = card as SkillCard;
                break;
            case CardPageType.RemoveMainCard:
                button.GetComponent<RemoveCardButton>().Card = card;
                break;
            case CardPageType.RemoveSkillCard:
                button.GetComponent<RemoveCardButton>().Card = card as SkillCard;
                break;
            case CardPageType.RecruitUnit:
                button.GetComponent<RecruitUnitButton>().UnitCard = card as UnitCard;
                break;
            case CardPageType.CloneUnit:
                button.GetComponent<CloneUnitButton>().UnitCard = card as UnitCard;
                break;
            default:
                Debug.LogError("INVALID TYPE!");
                return null;
        }
        return button;
    }

    // TABS
    public void MainDeckTab_OnClick()
    {
        FindObjectOfType<HomeBaseSceneDisplay>().RemoveMainCardButton_OnClick(false);
    }

    public void SkillDeckTab_OnClick()
    {
        FindObjectOfType<HomeBaseSceneDisplay>().RemoveSkillCardButton_OnClick(false);
    }
    public void LearnSkillTab_OnClick()
    {
        FindObjectOfType<HomeBaseSceneDisplay>().LearnSkillButton_OnClick(false);
    }

    // BUTTONS
    public void LearnSkillButton_OnClick(SkillCard skillCard)
    {
        if (anMan.ProgressBarRoutine != null) return;

        if (pMan.AetherCells < GameManager.LEARN_SKILL_COST)
            uMan.InsufficientAetherPopup();
        else uMan.CreateLearnSkillPopup(skillCard);
    }

    public void RecruitUnitButton_OnClick(UnitCard unitCard)
    {
        if (anMan.ProgressBarRoutine != null) return;
        
        if (pMan.AetherCells < GameManager.Instance.GetRecruitCost(unitCard, out _))
            uMan.InsufficientAetherPopup();
        else uMan.CreateRecruitUnitPopup(unitCard);
    }

    public void RemoveCardButton_OnClick(Card card)
    {
        if (anMan.ProgressBarRoutine != null) return;

        if (card is SkillCard)
        {
            if (pMan.SkillDeckCount <= GameManager.MINIMUM_SKILL_DECK_SIZE)
            {
                uMan.CreateFleetingInfoPopup(MinWarning(), true);
                return;
            }
        }
        else
        {
            if (pMan.MainDeckCount <= GameManager.MINIMUM_MAIN_DECK_SIZE)
            {
                uMan.CreateFleetingInfoPopup(MinWarning(), true);
                return;
            }
        }

        if (pMan.AetherCells < GameManager.Instance.GetRemoveCardCost(card))
            uMan.InsufficientAetherPopup();
        else uMan.CreateRemoveCardPopup(card);

        string MinWarning()
        {
            string message = "Your ";
            string deck;
            string minimum;
            if (card is SkillCard)
            {
                deck = "skill deck";
                minimum = GameManager.MINIMUM_SKILL_DECK_SIZE.ToString();
            }
            else
            {
                deck = "main deck";
                minimum = GameManager.MINIMUM_MAIN_DECK_SIZE.ToString();
            }
            message += deck + " can't have less than " + minimum + " cards!";
            return message;
        }
    }

    public void CloneUnitButton_OnClick(UnitCard unitCard)
    {
        if (anMan.ProgressBarRoutine != null) return;
        if (pMan.AetherCells < GameManager.CLONE_UNIT_COST)
            uMan.InsufficientAetherPopup();
        else uMan.CreateCloneUnitPopup(unitCard);
    }

    public void CloseCardPageButton_OnClick()
    {
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
            DialogueManager.Instance.DisplayDialoguePopup();
        uMan.DestroyCardPagePopup();
    }
}
