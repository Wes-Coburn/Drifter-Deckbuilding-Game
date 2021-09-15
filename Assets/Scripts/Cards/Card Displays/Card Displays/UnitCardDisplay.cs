using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitCardDisplay : CardDisplay
{
    [SerializeField] private GameObject currentAbilitiesDisplay;
    [SerializeField] private GameObject exhaustedIcon;
    [SerializeField] private GameObject attackScoreDisplay;
    [SerializeField] private GameObject healthScoreDisplay;

    public GameObject AbilityIconPrefab;
    public GameObject ZoomAbilityIconPrefab;
    public List<Effect> CurrentEffects;
    public List<GameObject> AbilityIcons;

    public UnitCard UnitCard { get => CardScript as UnitCard; }
    public List<CardAbility> CurrentAbilities;
    public int CurrentPower
    {
        get => UnitCard.CurrentPower;
        set
        {
            UnitCard.CurrentPower = value;
            TextMeshProUGUI txtPro = attackScoreDisplay.GetComponent<TextMeshProUGUI>();
            txtPro.SetText(UnitCard.CurrentPower.ToString());
        }
    }
    public int CurrentHealth
    {
        get => UnitCard.CurrentHealth;
        set
        {
            UnitCard.CurrentHealth = value;
            TextMeshProUGUI txtPro = healthScoreDisplay.GetComponent<TextMeshProUGUI>();
            txtPro.SetText(UnitCard.CurrentHealth.ToString());
        }
    }
    public int MaxHealth
    {
        get => UnitCard.MaxHealth;
        set => UnitCard.MaxHealth = value;
    }
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
        MaxHealth = UnitCard.StartHealth;
        CurrentHealth = MaxHealth;
        
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
    public override void DisplayZoomCard(GameObject parentCard, Card card = null)
    {
        base.DisplayZoomCard(parentCard, card);

        if (card == null)
        {
            UnitCardDisplay ucd = parentCard.GetComponent<UnitCardDisplay>();
            UnitCard uc = ucd.UnitCard;
            List<CardAbility> abilityList;

            if (CardZoom.ZoomCardIsCentered)
            {
                CurrentPower = uc.StartPower;
                MaxHealth = uc.StartHealth;
                CurrentHealth = MaxHealth;
                abilityList = uc.StartingAbilities;
            }
            else
            {
                CurrentPower = uc.CurrentPower;
                MaxHealth = uc.MaxHealth;
                CurrentHealth = uc.CurrentHealth;
                abilityList = ucd.CurrentAbilities;
            }

            foreach (CardAbility cardAbility in abilityList)
            {
                if (cardAbility == null) continue; // Skip empty abilities
                GetComponent<CardZoom>().CreateZoomAbilityIcon(cardAbility, 
                    currentAbilitiesDisplay.transform, 1);
            }
        }
        else
        {
            UnitCard uc = card as UnitCard;
            CurrentPower = uc.StartPower;
            MaxHealth = uc.StartHealth;
            CurrentHealth = uc.StartHealth;
            foreach (CardAbility ca in uc.StartingAbilities) 
                CurrentAbilities.Add(ca);
            foreach (CardAbility cardAbility in uc.StartingAbilities)
            {
                if (cardAbility == null) continue; // Skip empty abilities
                GetComponent<CardZoom>().CreateZoomAbilityIcon(cardAbility, 
                    currentAbilitiesDisplay.transform, 1);
            }
        }
    }

    /******
     * *****
     * ****** RESET_UNIT_CARD
     * *****
     *****/
    public void ResetUnitCard(bool isPlayed = false)
    {
        if (isPlayed)
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
        GetComponent<DragDrop>().IsPlayed = isPlayed;
        GetComponent<CardSelect>().CardOutline.SetActive(false);
    }

    /******
     * *****
     * ****** ADD_CURRENT_ABILITY
     * *****
     *****/
    public bool AddCurrentAbility(CardAbility ca, bool isPlayed = false)
    {
        if (ca is StaticAbility &&
            CardManager.GetAbility(gameObject, ca.AbilityName))
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
    public void RemoveCurrentAbility(string abilityName)
    {
        int abilityIndex = CardManager.GetAbilityIndex(gameObject, abilityName);
        if (abilityIndex == -1)
        {
            Debug.LogError("ABILITY NOT FOUND!");
            return;
        }
        else Debug.Log("ABILITY <" + abilityName + "> REMOVED!");

        CardAbility ca = CurrentAbilities[abilityIndex];
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
    private GameObject CreateAbilityIcon(CardAbility cardAbility)
    {
        GameObject abilityIcon = Instantiate(AbilityIconPrefab, new Vector2(0, 0), Quaternion.identity);
        abilityIcon.GetComponent<AbilityIconDisplay>().AbilityScript = cardAbility;
        abilityIcon.transform.SetParent(currentAbilitiesDisplay.transform, false);
        return abilityIcon;
    }
}