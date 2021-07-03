using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroCardDisplay : CardDisplay
{
    /* HERO_CARD_SCRIPTABLE_OBJECT */
    public HeroCard HeroCardScript
    {
        get => CardScript as HeroCard;
    }

    /* ABILITY_ICON_PREFAB */
    public GameObject AbilityIconPrefab;

    /* HERO_CARD_DATA */
    /* LEVEL_UP_CONDITION */
    [SerializeField] private GameObject levelUpCondition;

    /* ATTACK_SCORE */
    [SerializeField] private GameObject attackScoreDisplay;
    private int currentAttackScore;
    public int CurrentAttackScore
    {
        get => currentAttackScore;
        set
        {
            currentAttackScore = value;
            SetAttackScore(CurrentAttackScore);
        }
    }
    public int TemporaryAttackModifier { get; set; }

    /* DEFENSE_SCORE */
    [SerializeField] private GameObject defenseScoreDisplay;
    private int currentDefenseScore;
    public int CurrentDefenseScore
    {
        get => currentDefenseScore;
        set
        {
            currentDefenseScore = value;
            SetDefenseScore(CurrentDefenseScore);
        }
    }
    
    /* MAX_DEFENSE_SCORE */
    [SerializeField] private GameObject maxDefenseScoreDisplay;
    private int maxDefenseScore;
    public int MaxDefenseScore
    {
        get => maxDefenseScore;
        set
        {
            maxDefenseScore = value;
            if (maxDefenseScoreDisplay == null) return;
            TextMeshPro txtPro = maxDefenseScoreDisplay.GetComponent<TextMeshPro>();
            txtPro.SetText(MaxDefenseScore.ToString());
        }
    }

    /* ABILITIES */
    public List<CardAbility> CurrentAbilities;
    public List<GameObject> AbilityIcons;
    public List<CardAbility> TemporaryAbilities; // TESTING
    [SerializeField] private GameObject currentAbilitiesDisplay;
    
    /* EXHAUSTED_ICON */
    [SerializeField] private GameObject exhaustedIcon;
    private bool canAttack = false;
    public bool CanAttack
    {
        get => canAttack;
        set
        {
            canAttack = value;
            exhaustedIcon.SetActive(!CanAttack);
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
        SetLevelUpCondition("Level Up: " + HeroCardScript.XPCondition);
        SetAttackScore(HeroCardScript.AttackScore);
        TemporaryAttackModifier = 0; // Necessary?

        MaxDefenseScore = HeroCardScript.DefenseScore;
        CurrentDefenseScore = MaxDefenseScore;
        
        foreach (CardAbility cardAbility in HeroCardScript.Level1Abilities)
        {
            if (cardAbility == null) continue; // Skip empty abilities
            AddCurrentAbility(cardAbility, false);
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
        HeroCardDisplay hcd = parentCard.GetComponent<CardDisplay>() as HeroCardDisplay;
        SetLevelUpCondition(hcd.GetLevelUpCondition());
        SetAttackScore(hcd.GetAttackScore());

        CurrentDefenseScore = hcd.CurrentDefenseScore;
        MaxDefenseScore = hcd.MaxDefenseScore;

        foreach (CardAbility cardAbility in hcd.CurrentAbilities)
        {
            if (cardAbility == null) continue; // Skip empty abilities
            gameObject.GetComponent<CardZoom>().CreateZoomAbilityIcon(cardAbility, currentAbilitiesDisplay.transform, 1);
        }
    }

    /******
     * *****
     * ****** ADD_CURRENT_ABILITY
     * *****
     *****/
    public void AddCurrentAbility(CardAbility cardAbility, bool isTemporary)
    {
        if (CurrentAbilities.Contains(cardAbility)) return; // TESTING
        CurrentAbilities.Add(cardAbility);        
        AbilityIcons.Add(CreateAbilityIcon(cardAbility));
        if (isTemporary) TemporaryAbilities.Add(cardAbility); // TESTING
    }

    /******
     * *****
     * ****** REMOVE_CURRENT_ABILITY
     * *****
     *****/
    public void RemoveCurrentAbility(CardAbility cardAbility)
    {
        if (!CurrentAbilities.Contains(cardAbility)) return; // TESTING
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
        Destroy(abilityIcon.GetComponent<BoxCollider2D>());
        abilityIcon.GetComponent<AbilityIconDisplay>().AbilityScript = cardAbility;
        abilityIcon.transform.SetParent(currentAbilitiesDisplay.transform, false);
        return abilityIcon;
    }

    /******
     * *****
     * ****** GETTERS/SETTERS
     * *****
     *****/
    public int GetDefenseScore() => System.Convert.ToInt32(defenseScoreDisplay.GetComponent<TextMeshPro>().text);
    public void SetDefenseScore(int newScore)
    {
        TextMeshPro txtPro = defenseScoreDisplay.GetComponent<TextMeshPro>();
        txtPro.SetText(newScore.ToString());
    }
    public int GetAttackScore() => System.Convert.ToInt32(attackScoreDisplay.GetComponent<TextMeshPro>().text);
    public void SetAttackScore(int newScore)
    {
        TextMeshPro txtPro = attackScoreDisplay.GetComponent<TextMeshPro>();
        txtPro.SetText(newScore.ToString());
    }
    public string GetLevelUpCondition() => levelUpCondition.GetComponent<TextMeshPro>().text;
    public void SetLevelUpCondition(string newCondition)
    {
        TextMeshPro txtPro = levelUpCondition.GetComponent<TextMeshPro>();
        txtPro.SetText(newCondition);
    }
}