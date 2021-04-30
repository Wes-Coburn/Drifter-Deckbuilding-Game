using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroCardDisplay : CardDisplay
{
    /* HERO_CARD_SCRIPTABLE_OBJECT */
    private HeroCard heroCard;

    /* ABILITY_ICON_PREFAB */
    public GameObject AbilityIconPrefab;

    /* HERO_CARD_DATA */
    /* XP */
    public int CurrentXP { get; private set; }
    [SerializeField] private GameObject XPCondition;
    [SerializeField] private GameObject XP1;
    [SerializeField] private GameObject XP2;
    [SerializeField] private GameObject XP3;
    [SerializeField] private GameObject XP4;
    /* ATTACK_SCORE */
    [SerializeField] private GameObject AttackScore;
    [SerializeField] private GameObject AttackScoreModifier;
    /* DEFENSE_SCORE */
    [SerializeField] private GameObject DefenseScore;
    [SerializeField] private GameObject DefenseScoreModifier;
    /* ABILITIES */
    public List<CardAbility> CurrentAbilities;
    public List<GameObject> AbilityIcons;
    [SerializeField] private GameObject CurrentAbilitiesDisplay;
    /* EXHAUSTED_ICON */
    [SerializeField] private GameObject ExhaustedIcon;

    private bool canAttack = false;
    public bool CanAttack
    {
        get => canAttack;
        set
        {
            canAttack = value;
            gameObject.GetComponent<Animator>().SetBool("Exhausted", !CanAttack);
        }
    }

    /******
     * *****
     * ****** DISPLAY_CARD
     * *****
     *****/
    protected override void DisplayCard()
    {
        base.DisplayCard();
        heroCard = (HeroCard)CardScript;
        SetCurrentXP(0);
        SetXPCondition("Gain XP: " + heroCard.XPCondition);
        SetAttackScore(heroCard.AttackScore);
        SetDefenseScore(heroCard.DefenseScore);
        
        foreach (CardAbility cardAbility in heroCard.Level1Abilities)
        {
            if (cardAbility == null) continue; // Skip empty abilities
            AddCurrentAbility(cardAbility);
        }
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_CARD
     * *****
     *****/
    public override void DisplayZoomCard(GameObject parentCard)
    {
        base.DisplayZoomCard(parentCard);
        CardDisplay parentCardDisplay = parentCard.GetComponent<CardDisplay>();
        HeroCardDisplay heroParentCardDisplay = (HeroCardDisplay)parentCardDisplay;
        SetXPCondition(heroParentCardDisplay.GetXPCondition());
        SetCurrentXP(heroParentCardDisplay.GetCurrentXP());
        SetAttackScore(heroParentCardDisplay.GetAttackScore());
        SetDefenseScore(heroParentCardDisplay.GetDefenseScore());

        foreach (CardAbility cardAbility in heroParentCardDisplay.CurrentAbilities)
        {
            if (cardAbility == null) continue; // Skip empty abilities

            gameObject.GetComponent<CardZoom>().CreateZoomAbilityIcon(cardAbility, CurrentAbilitiesDisplay.transform, 1);
        }
    }

    /******
     * *****
     * ****** ADD_CURRENT_ABILITY
     * *****
     *****/
    public void AddCurrentAbility(CardAbility cardAbility)
    {
        CurrentAbilities.Add(cardAbility);        
        AbilityIcons.Add(CreateAbilityIcon(cardAbility));
    }

    /******
     * *****
     * ****** REMOVE_CURRENT_ABILITY
     * *****
     *****/
    public void RemoveCurrentAbility(CardAbility cardAbility) // IMPLEMENT!
    {
        int abilityIndex = CurrentAbilities.FindIndex(x => x.AbilityName == cardAbility.AbilityName);
        Destroy(AbilityIcons[abilityIndex]);
        AbilityIcons.RemoveAt(abilityIndex);
        CurrentAbilities.RemoveAt(abilityIndex);
    }

    /******
     * *****
     * ****** CREATE_ABILITY_ICON
     * *****
     *****/
    public GameObject CreateAbilityIcon(CardAbility cardAbility)
    {
        GameObject abilityIcon = Instantiate(AbilityIconPrefab, new Vector2(0, 0), Quaternion.identity);
        abilityIcon.GetComponent<AbilityIconDisplay>().AbilityScript = cardAbility;
        abilityIcon.transform.SetParent(CurrentAbilitiesDisplay.transform, false);
        return abilityIcon;
    }

    /******
     * *****
     * ****** GETTERS/SETTERS
     * *****
     *****/
    public int GetAttackScore() => System.Convert.ToInt32(AttackScore.GetComponent<TextMeshPro>().text);
    public void SetAttackScore(int attackScore)
    {
        TextMeshPro txtPro = AttackScore.GetComponent<TextMeshPro>();
        txtPro.SetText(attackScore.ToString());
    }
    public void SetAttackScoreModifier(int attackScoreModifier) // IMPLEMENT!
    {
        TextMeshPro txtPro = AttackScoreModifier.GetComponent<TextMeshPro>();
        txtPro.SetText(attackScoreModifier.ToString());
    }
    public int GetDefenseScore() => System.Convert.ToInt32(DefenseScore.GetComponent<TextMeshPro>().text);
    public void SetDefenseScore(int defenseScore)
    {
        TextMeshPro txtPro = DefenseScore.GetComponent<TextMeshPro>();
        txtPro.SetText(defenseScore.ToString());
    }
    public void SetDefenseScoreModifier(int defenseScoreModifier)
    {
        TextMeshPro txtPro = DefenseScoreModifier.GetComponent<TextMeshPro>();
        txtPro.SetText(defenseScoreModifier.ToString());
    }
    public string GetXPCondition() => XPCondition.GetComponent<TextMeshPro>().text;
    public void SetXPCondition(string xpCondition)
    {
        TextMeshPro txtPro = XPCondition.GetComponent<TextMeshPro>();
        txtPro.SetText(xpCondition);
    }
    public int GetCurrentXP() => CurrentXP;
    public void SetCurrentXP(int currentXP) // THIS METHOD MIGHT CAUSE A VISUAL STUTTER WHEN currentXP > 0 ??
    {
        CurrentXP = currentXP;

        XP1.SetActive(false);
        XP2.SetActive(false);
        XP3.SetActive(false);
        XP4.SetActive(false);

        switch (currentXP)
        {
            case 0:
                break;
            case 1:
                XP1.SetActive(true);
                break;
            case 2:
                XP2.SetActive(true);
                goto case 1;
            case 3:
                XP3.SetActive(true);
                goto case 2;
            case 4:
                XP4.SetActive(true);
                goto case 3;
        }
    }

    
}