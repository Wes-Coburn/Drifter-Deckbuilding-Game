using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowerCardDisplay : CardDisplay
{
    /* HERO_CARD_SCRIPTABLE_OBJECT */
    public FollowerCard FollowerCard { get => CardScript as FollowerCard; }

    /* ABILITY_ICON_PREFAB */
    public GameObject AbilityIconPrefab;

    /* LEVEL_UP_CONDITION */
    [SerializeField] private GameObject levelUpCondition;
    public string LevelUpCondition
    {
        get => FollowerCard.LevelUpCondition;
        set
        {
            TextMeshPro txtPro = levelUpCondition.GetComponent<TextMeshPro>();
            txtPro.SetText(value);
        }
    }

    /* POWER */
    [SerializeField] private GameObject attackScoreDisplay;
    public int CurrentPower
    {
        get => FollowerCard.CurrentPower;
        set
        {
            FollowerCard.CurrentPower = value;
            TextMeshPro txtPro = attackScoreDisplay.GetComponent<TextMeshPro>();
            txtPro.SetText(FollowerCard.CurrentPower.ToString());
        }
    }

    /* DEFENSE */
    [SerializeField] private GameObject defenseScoreDisplay;
    public int CurrentDefense
    {
        get => FollowerCard.CurrentDefense;
        set
        {
            FollowerCard.CurrentDefense = value;
            TextMeshPro txtPro = defenseScoreDisplay.GetComponent<TextMeshPro>();
            txtPro.SetText(FollowerCard.CurrentDefense.ToString());
        }
    }
    
    /* MAX_DEFENSE_SCORE */
    [SerializeField] private GameObject maxDefenseDisplay;
    public int MaxDefense
    {
        get => FollowerCard.MaxDefense;
        set
        {
            FollowerCard.MaxDefense = value;
            if (maxDefenseDisplay != null)
            {
                TextMeshPro txtPro = maxDefenseDisplay.GetComponent<TextMeshPro>();
                txtPro.SetText(MaxDefense.ToString());
            }
        }
    }

    /* EFFECTS */
    public List<Effect> CurrentEffects;
    public List<Effect> TemporaryEffects;
    public List<int> EffectCountdowns;

    /* ABILITIES */
    [SerializeField] private GameObject currentAbilitiesDisplay;
    public List<CardAbility> CurrentAbilities;
    public List<GameObject> AbilityIcons;
    public List<CardAbility> TemporaryAbilities;
    public List<int> AbilityCountdowns;
    
    /* EXHAUSTED_ICON */
    [SerializeField] private GameObject exhaustedIcon;
    public bool IsExhausted
    {
        get => FollowerCard.IsExhausted;
        set
        {
            FollowerCard.IsExhausted = value;
            exhaustedIcon.SetActive(FollowerCard.IsExhausted);
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
        LevelUpCondition = "Level Up: " + FollowerCard.LevelUpCondition;
        CurrentPower = FollowerCard.StartPower;
        MaxDefense = FollowerCard.StartDefense;
        CurrentDefense = MaxDefense;
        
        foreach (CardAbility cardAbility in FollowerCard.Level1Abilities)
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
        FollowerCardDisplay fcd = parentCard.GetComponent<CardDisplay>() as FollowerCardDisplay;
        LevelUpCondition = fcd.LevelUpCondition;        
        CurrentPower = fcd.CurrentPower;
        MaxDefense = fcd.MaxDefense;
        CurrentDefense = fcd.CurrentDefense;

        foreach (CardAbility cardAbility in fcd.CurrentAbilities)
        {
            if (cardAbility == null) continue; // Skip empty abilities
            gameObject.GetComponent<CardZoom>().CreateZoomAbilityIcon(cardAbility, currentAbilitiesDisplay.transform, 1);
        }
    }

    /******
     * *****
     * ****** RESET_HERO_CARD
     * *****
     *****/
    public void ResetHeroCard() // TESTING
    {
        IsExhausted = true;
        gameObject.GetComponent<DragDrop>().IsPlayed = false;
        foreach (GameObject go in AbilityIcons) Destroy(go);
        CurrentEffects.Clear();
        TemporaryEffects.Clear();
        EffectCountdowns.Clear();
        CurrentAbilities.Clear();
        AbilityIcons.Clear();
        TemporaryAbilities.Clear();
        AbilityCountdowns.Clear();
        DisplayCard();
    }

    /******
     * *****
     * ****** ADD_CURRENT_ABILITY
     * *****
     *****/
    public bool AddCurrentAbility(CardAbility ca)
    {
        if (CurrentAbilities.Contains(ca)) return false;
        CurrentAbilities.Add(ca);        
        AbilityIcons.Add(CreateAbilityIcon(ca));
        return true;
    }

    /******
     * *****
     * ****** ADD_TEMORARY_ABILITY
     * *****
     *****/
    public void AddTemporaryAbility(CardAbility ca, int countdown) // TESTING
    {
        if (!AddCurrentAbility(ca)) return;
        TemporaryAbilities.Add(ca);
        AbilityCountdowns.Add(countdown);
    }

    /******
     * *****
     * ****** REMOVE_CURRENT_ABILITY
     * *****
     *****/
    public void RemoveCurrentAbility(CardAbility cardAbility)
    {
        if (!CurrentAbilities.Contains(cardAbility)) return;
        int abilityIndex = CurrentAbilities.FindIndex(x => x.AbilityName == cardAbility.AbilityName);
        Destroy(AbilityIcons[abilityIndex]);
        AbilityIcons.RemoveAt(abilityIndex);
        CurrentAbilities.RemoveAt(abilityIndex);
        if (TemporaryAbilities.Contains(cardAbility)) TemporaryAbilities.Remove(cardAbility);
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
}