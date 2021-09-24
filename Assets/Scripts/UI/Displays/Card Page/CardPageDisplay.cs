using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardPageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject learnSkillButtonPrefab;
    [SerializeField] private GameObject removeCardButtonPrefab;
    [SerializeField] private GameObject pageCounter;
    [SerializeField] private GameObject cardGroup;
    [SerializeField] private GameObject costGroup;
    [SerializeField] private GameObject pageTitle;
    [SerializeField] private GameObject noCardsTooltip;

    private PlayerManager pMan;
    private UIManager uMan;
    private List<Card> cardGroupList;
    private List<GameObject> activeCards;
    private bool isCardRemoval;
    private int currentPage;
    private int totalPages;

    private string PageCounterText
    {
        set
        {
            pageCounter.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public void DisplayCardPage(bool isCardRemoval)
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        this.isCardRemoval = isCardRemoval;
        cardGroupList = new List<Card>();
        string titleText;

        if (isCardRemoval)
        {
            titleText = "Remove a Card";
            foreach (Card c in pMan.PlayerDeckList)
                cardGroupList.Add(c);
        }
        else
        {
            titleText = "Learn a Skill";
            foreach (Card c in pMan.PlayerHero.HeroMoreSkills) 
                cardGroupList.Add(c);

            List<SkillCard> redundancies = new List<SkillCard>();
            foreach (SkillCard skill in cardGroupList)
            {
                if (pMan.PlayerDeckList.FindIndex
                    (x => x.CardName == skill.CardName) != -1)
                    redundancies.Add(skill);
            }
            foreach (SkillCard rSkill in redundancies)
                cardGroupList.Remove(rSkill);
        }
        pageTitle.GetComponent<TextMeshProUGUI>().SetText(titleText);
        activeCards = new List<GameObject>();
        currentPage = 1;
        if (cardGroupList.Count > 0)
        {
            noCardsTooltip.SetActive(false);
            double result = cardGroupList.Count / 4.0;
            totalPages = (int)Math.Ceiling(result);
        }
        else
        {
            noCardsTooltip.SetActive(true);
            totalPages = 1;
        }
        LoadCardPage();
    }

    private void LoadCardPage()
    {
        PageCounterText = currentPage + " / " + totalPages;
        int firstIndex = (currentPage - 1) * 4;
        int index;
        foreach (GameObject go in activeCards) Destroy(go);
        activeCards.Clear();
        for (int i = 0; i < 4; i++)
        {
            index = firstIndex + i;
            if (index > cardGroupList.Count - 1) break;
            Card card = cardGroupList[firstIndex + i];
            GameObject cardObj = CombatManager.Instance.ShowCard(card, new Vector2(), false, true);
            if (cardObj.TryGetComponent(out UnitCardDisplay ucd)) ucd.DisableVisuals(); // For non-zoom card only
            cardObj.transform.localScale = new Vector2(4, 4);
            cardObj.transform.SetParent(cardGroup.transform);
            activeCards.Add(cardObj);

            GameObject costPrefab;
            if (isCardRemoval) costPrefab = removeCardButtonPrefab;
            else costPrefab = learnSkillButtonPrefab;
            GameObject cost = Instantiate(costPrefab, costGroup.transform);
            cost.transform.localScale = new Vector2(1.5f, 1.5f);

            if (isCardRemoval) cost.GetComponent<RemoveCardButton>().Card = card;
            else cost.GetComponent<LearnSkillButton>().SkillCard = card as SkillCard;
            activeCards.Add(cost);
        }
    }

    public void NextPageButton_OnClick()
    {
        if (currentPage == totalPages) return;
        currentPage++;
        LoadCardPage();
    }

    public void PreviousPageButton_OnClick()
    {
        if (currentPage == 1) return;
        currentPage--;
        LoadCardPage();
    }
    
    public void LearnSkillButton_OnClick(SkillCard skillCard)
    {
        if (pMan.AetherCells < 2)
            uMan.InsufficientAetherPopup();
        else uMan.CreateLearnSkillPopup(skillCard);
    }

    public void RemoveCardButton_OnClick(Card card)
    {
        if (pMan.PlayerDeckList.Count <= 10)
            uMan.CreateCenteredInfoPopup("You must have at least 10 cards in your deck!");
        else if (pMan.AetherCells < 1)
            uMan.InsufficientAetherPopup();
        else uMan.CreateRemoveCardPopup(card);
    }
}
