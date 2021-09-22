using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardPageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject cardCostPrefab;
    [SerializeField] private GameObject pageCounter;
    [SerializeField] private GameObject cardGroup;
    [SerializeField] private GameObject costGroup;
    [SerializeField] private GameObject learnSkillPopupPrefab;

    private PlayerManager pMan;
    private List<SkillCard> cardGroupList;
    private List<GameObject> activeCards;
    private int currentPage;
    private int totalPages;
    private GameObject learnSkillPopup;

    private string PageCounterText
    {
        set
        {
            pageCounter.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        cardGroupList = pMan.PlayerHero.HeroMoreSkills;
        List<SkillCard> redundancies = new List<SkillCard>();
        foreach (SkillCard skill in cardGroupList)
        {
            if (pMan.PlayerDeckList.FindIndex
                (x => x.CardName == skill.CardName) != -1)
                redundancies.Add(skill);
        }
        foreach (SkillCard rSkill in redundancies)
            cardGroupList.Remove(rSkill);

        activeCards = new List<GameObject>();
        currentPage = 1;
        double result = cardGroupList.Count / 4.0;
        totalPages = (int)Math.Ceiling(result);
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
            SkillCard skill = cardGroupList[firstIndex + i];
            GameObject card = CombatManager.Instance.ShowCard(skill, new Vector2());
            card.transform.localScale = new Vector2(4, 4);
            card.transform.SetParent(cardGroup.transform);
            activeCards.Add(card);
            GameObject cost = Instantiate(cardCostPrefab, costGroup.transform);
            cost.transform.localScale = new Vector2(1.5f, 1.5f);
            cost.GetComponent<CardCostButton>().SkillCard = skill;
            activeCards.Add(cost);
        }
    }

    public void NextPage()
    {
        if (currentPage == totalPages) return;
        currentPage++;
        LoadCardPage();
    }

    public void PreviousPage()
    {
        if (currentPage == 1) return;
        currentPage--;
        LoadCardPage();
    }

    public void LearnSkill(SkillCard skillCard)
    {
        if (pMan.AetherCells > 0) 
            CreateLearnSkillPopup(skillCard);
        else
        {
            // not enough aether popup
            CreateLearnSkillPopup(skillCard); // FOR TESTING ONLY
        }
    }

    private void CreateLearnSkillPopup(SkillCard skillCard)
    {
        DestroyLearnSkillPopup();
        learnSkillPopup = Instantiate(learnSkillPopupPrefab,
                UIManager.Instance.CurrentCanvas.transform);
        learnSkillPopup.GetComponent<LearnSkillPopupDisplay>().SkillCard = skillCard;
    }

    public void DestroyLearnSkillPopup()
    {
        if (learnSkillPopup != null)
        {
            Destroy(learnSkillPopup);
            learnSkillPopup = null;
        }
    }
}
