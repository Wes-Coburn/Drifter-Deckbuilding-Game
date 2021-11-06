using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCardDisplay : CardDisplay
{
    [SerializeField] private GameObject unitStats;
    [SerializeField] private GameObject currentAbilitiesDisplay;
    [SerializeField] private GameObject attackScoreDisplay;
    [SerializeField] private GameObject healthScoreDisplay;
    [SerializeField] private GameObject exhaustedIcon;
    [SerializeField] private GameObject destroyedIcon;

    [SerializeField] private GameObject abilityIconPrefab;
    [SerializeField] private GameObject zoomAbilityIconPrefab;

    private List<CardAbility> displayedAbilities;

    public UnitCard UnitCard { get => CardScript as UnitCard; }
    public GameObject UnitStats { get => unitStats; }
    public GameObject ZoomAbilityIconPrefab { get => zoomAbilityIconPrefab; }

    public List<Effect> CurrentEffects { get; set; }
    public List<GameObject> AbilityIcons { get; set; }
    public List<CardAbility> CurrentAbilities { get; set; }

    public int CurrentPower
    {
        get => UnitCard.CurrentPower;
        set
        {
            UnitCard.CurrentPower = value;
            int displayValue = 0;
            if (value >= 0) displayValue = value; // TESTING
            TextMeshProUGUI txtPro = attackScoreDisplay.GetComponent<TextMeshProUGUI>();
            txtPro.SetText(displayValue.ToString());
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

    private void Awake()
    {
        CurrentEffects = new List<Effect>();
        CurrentAbilities = new List<CardAbility>();
        AbilityIcons = new List<GameObject>();
        displayedAbilities = new List<CardAbility>();
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
    public void DisplayCardPageCard(UnitCard unitCard)
    {
        cardScript = unitCard;
        base.DisplayCard();
        CurrentPower = UnitCard.StartPower;
        MaxHealth = UnitCard.StartHealth;
        CurrentHealth = MaxHealth;

        GridLayoutGroup glg =
            currentAbilitiesDisplay.GetComponent<GridLayoutGroup>();
        Vector2 cellSize = glg.cellSize;
        cellSize.y = 16;
        glg.cellSize = cellSize;

        foreach (CardAbility cardAbility in UnitCard.StartingAbilities)
        {
            if (cardAbility == null) continue; // Skip empty abilities
            GetComponent<CardZoom>().CreateZoomAbilityIcon(cardAbility,
                currentAbilitiesDisplay.transform, 1);
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
            if (CardManager.GetAbility(gameObject, CardManager.ABILITY_BLITZ))
                IsExhausted = false;
            else IsExhausted = true;
        }
        else
        {
            IsExhausted = false;
            foreach (GameObject go in AbilityIcons)
                Destroy(go);
            AbilityIcons.Clear();
            displayedAbilities.Clear(); // TESTING
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
        if (ca is StaticAbility)
        {
            if (CardManager.GetAbility(gameObject, ca.AbilityName))
            {
                Debug.Log("ABILITY ALREADY EXISTS: <" + ca.ToString() + ">");
                return false;
            }
            else ShowAbility(ca);
        }
        else if (ca is TriggeredAbility ta)
        {
            if (ta.AbilityTrigger.AbilityName != CardManager.TRIGGER_PLAY)
            {
                if (CardManager.EvergreenTriggers.Contains(ta.AbilityTrigger.AbilityName))
                {
                    // TESTING TESTING TESTING
                    foreach (CardAbility ca2 in displayedAbilities)
                    {
                        if (ca2 is TriggeredAbility ta2)
                        {
                            if (ta2.AbilityTrigger.AbilityName == ta.AbilityTrigger.AbilityName)
                                return false;
                        }
                    }
                    ShowAbility(ta);
                }
                else
                {
                    Debug.LogError("ABILITY TRIGGER NOT FOUND!");
                    return false;
                }
            }
        }

        CurrentAbilities.Add(ca); // Add instances instead? (Doesn't matter yet)

        if (isPlayed)
        {
            if (ca is StaticAbility sa)
                AudioManager.Instance.StartStopSound(null, sa.GainAbilitySound);
        }

        if (ca.AbilityName == "Blitz") IsExhausted = false;
        return true;

        void ShowAbility(CardAbility ca)
        {
            AbilityIcons.Add(CreateAbilityIcon(ca));
            displayedAbilities.Add(ca);
        }
    }

    /******
     * *****
     * ****** REMOVE_CURRENT_ABILITY
     * *****
     *****/
    public void RemoveCurrentAbility(string abilityName)
    {
        int abilityIndex = CurrentAbilities.FindIndex(x => x.AbilityName == abilityName);
        int displayIndex = displayedAbilities.FindIndex(x => x.AbilityName == abilityName);

        if (abilityIndex == -1)
        {
            Debug.LogWarning("ABILITY NOT FOUND!");
            return;
        }
        else Debug.Log("ABILITY <" + abilityName + "> REMOVED!");

        CardAbility ca = CurrentAbilities[abilityIndex];
        CurrentAbilities.RemoveAt(abilityIndex);

        if (ca is StaticAbility sa)
            AudioManager.Instance.StartStopSound(null, sa.LoseAbilitySound);
        else if (ca is TriggeredAbility ta) // TESTING
        {
            if (ta.AbilityTrigger.AbilityName == CardManager.TRIGGER_PLAY) return;
            if (CardManager.GetTrigger(gameObject, ta.AbilityTrigger.AbilityName)) return;
        }

        if (displayIndex == -1)
        {
            Debug.LogError("ABILITY ICON NOT FOUND!");
            return;
        }
        Destroy(AbilityIcons[displayIndex]);
        AbilityIcons.RemoveAt(displayIndex);
        displayedAbilities.RemoveAt(displayIndex);
    }

    /******
     * *****
     * ****** CREATE_ABILITY_ICON
     * *****
     *****/
    private GameObject CreateAbilityIcon(CardAbility cardAbility)
    {
        GameObject abilityIcon = Instantiate(abilityIconPrefab, new Vector2(0, 0), Quaternion.identity);
        abilityIcon.GetComponent<AbilityIconDisplay>().AbilityScript = cardAbility;
        abilityIcon.transform.SetParent(currentAbilitiesDisplay.transform, false);
        return abilityIcon;
    }

    // TESTING
    public void AbilityTriggerState(string triggerName)
    {
        if (triggerName == CardManager.TRIGGER_PLAY) return;

        foreach (CardAbility ca in displayedAbilities)
        {
            int displayIndex =
                displayedAbilities.FindIndex(x => x.AbilityName == ca.AbilityName);

            if (ca is StaticAbility)
            {
                if (ca.AbilityName == triggerName)
                {
                    TriggerVisuals(AbilityIcons[displayIndex]);
                    return;
                }
            }
            else if (ca is TriggeredAbility tra)
            {
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    TriggerVisuals(AbilityIcons[displayIndex]);
                    return;
                }
            }
        }

        Debug.LogError("TRIGGER NOT FOUND!");

        static void TriggerVisuals(GameObject icon)
        {
            AnimationManager.Instance.AbilityTriggerState(icon);
            AudioManager.Instance.StartStopSound("SFX_Trigger");
        }
    }

    public override void DisableVisuals()
    {
        base.DisableVisuals();
        exhaustedIcon.SetActive(false);
        destroyedIcon.SetActive(false);
    }
}