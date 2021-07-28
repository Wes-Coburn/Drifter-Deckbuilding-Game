using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitCardDisplay : CardDisplay
{
    /* HERO_CARD_SCRIPTABLE_OBJECT */
    public UnitCard UnitCard { get => CardScript as UnitCard; }

    /* ABILITY_ICON_PREFAB */
    public GameObject AbilityIconPrefab;
    public GameObject ZoomAbilityIconPrefab;

    /* POWER */
    [SerializeField] private GameObject attackScoreDisplay;
    public int CurrentPower
    {
        get => UnitCard.CurrentPower;
        set
        {
            UnitCard.CurrentPower = value;
            TextMeshPro txtPro = attackScoreDisplay.GetComponent<TextMeshPro>();
            txtPro.SetText(UnitCard.CurrentPower.ToString());
        }
    }

    /* DEFENSE */
    [SerializeField] private GameObject defenseScoreDisplay;
    public int CurrentDefense
    {
        get => UnitCard.CurrentDefense;
        set
        {
            UnitCard.CurrentDefense = value;
            TextMeshPro txtPro = defenseScoreDisplay.GetComponent<TextMeshPro>();
            txtPro.SetText(UnitCard.CurrentDefense.ToString());
        }
    }
    
    /* MAX_DEFENSE_SCORE */
    [SerializeField] private GameObject maxDefenseDisplay;
    public int MaxDefense
    {
        get => UnitCard.MaxDefense;
        set
        {
            UnitCard.MaxDefense = value;
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
        get => UnitCard.IsExhausted;
        set
        {
            UnitCard.IsExhausted = value;
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
        CurrentPower = UnitCard.StartPower;
        MaxDefense = UnitCard.StartDefense;
        CurrentDefense = MaxDefense;
        
        foreach (CardAbility cardAbility in UnitCard.StartingAbilities)
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
        UnitCardDisplay fcd = parentCard.GetComponent<CardDisplay>() as UnitCardDisplay;
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
     * ****** RESET_UNIT_CARD
     * *****
     *****/
    public void ResetUnitCard(bool played = false)
    {
        if (played)
        {
            if (CardManager.GetAbility(gameObject, "Blitz")) IsExhausted = false;
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
        gameObject.GetComponent<CardSelect>().CardOutline.SetActive(false);
    }

    /******
     * *****
     * ****** ADD_CURRENT_ABILITY
     * *****
     *****/
    public bool AddCurrentAbility(CardAbility ca, bool isPlayed = false)
    {
        if (CardManager.GetAbility(gameObject, ca.AbilityName))
        {
            Debug.LogWarning("ABILITY ALREADY EXISTS: <" + ca.ToString() + ">");
            return false;
        }
        CurrentAbilities.Add(ca); // Add instances instead of objects? (Doesn't matter yet)
        AbilityIcons.Add(CreateAbilityIcon(ca));
        
        if (isPlayed)
        {
            if (ca is StaticAbility sa)
                AudioManager.Instance.StartStopSound(null, sa.GainAbilitySound);
        }

        if (ca.AbilityName == "Blitz") IsExhausted = false;
        return true;
    }

    /******
     * *****
     * ****** REMOVE_CURRENT_ABILITY
     * *****
     *****/
    public void RemoveCurrentAbility(CardAbility ca, string caString = null)
    {
        string abilityName;
        if (caString != null) abilityName = caString;
        else abilityName = ca.AbilityName;

        int abilityIndex = CardManager.GetAbilityIndex(gameObject, abilityName);
        if (abilityIndex == -1)
        {
            Debug.LogError("ABILITY NOT FOUND!");
            return;
        }
        else Debug.Log("ABILITY REMOVED!");
        Destroy(AbilityIcons[abilityIndex]);
        AbilityIcons.RemoveAt(abilityIndex);
        CurrentAbilities.RemoveAt(abilityIndex);

        if (ca is StaticAbility sa) 
            AudioManager.Instance.StartStopSound(null, sa.LoseAbilitySound);
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
        abilityIcon.transform.SetParent(currentAbilitiesDisplay.transform, false);
        return abilityIcon;
    }
}