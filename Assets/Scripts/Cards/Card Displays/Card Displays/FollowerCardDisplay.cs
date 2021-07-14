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
            txtPro.SetText("Level Up: " + value);
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

    /* ABILITIES */
    [SerializeField] private GameObject currentAbilitiesDisplay;
    public List<CardAbility> CurrentAbilities;
    public List<GameObject> AbilityIcons;
        
    /* EXHAUSTED_ICON */
    [SerializeField] private GameObject exhaustedIcon;
    public bool IsExhausted
    {
        get => FollowerCard.IsExhausted;
        set
        {
            FollowerCard.IsExhausted = value;
            exhaustedIcon.SetActive(value);
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
        LevelUpCondition = FollowerCard.LevelUpCondition;
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
    public void ResetFollowerCard(bool played = false)
    {
        if (played)
        {
            if (CardManager.GetAbility(gameObject, "Blitz") != -1) IsExhausted = false;
            else IsExhausted = true;
        }
        else
        {
            IsExhausted = false;
            foreach (GameObject go in AbilityIcons) Destroy(go);
            AbilityIcons.Clear();
            CurrentEffects.Clear();
            CurrentAbilities.Clear();
            DisplayCard();
        }
        gameObject.GetComponent<DragDrop>().IsPlayed = played;
    }

    /******
     * *****
     * ****** ADD_CURRENT_ABILITY
     * *****
     *****/
    public bool AddCurrentAbility(CardAbility ca)
    {
        if (CardManager.GetAbility(gameObject, ca.AbilityName) != -1)
        {
            Debug.LogWarning("ABILITY ALREADY EXISTS: <" + ca.ToString() + ">");
            return false;
        }
        CurrentAbilities.Add(ca); // Add instances instead of objects? Doesn't currently matter.
        AbilityIcons.Add(CreateAbilityIcon(ca));

        if (ca.AbilityName == "Blitz") IsExhausted = false;
        return true;
    }

    /******
     * *****
     * ****** REMOVE_CURRENT_ABILITY
     * *****
     *****/
    public void RemoveCurrentAbility(CardAbility ca)
    {
        int abilityIndex = CardManager.GetAbility(gameObject, ca.AbilityName);
        if (abilityIndex == -1)
        {
            Debug.LogError("ABILITY NOT FOUND: <" + ca.ToString() + ">");
            return;
        }
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
}