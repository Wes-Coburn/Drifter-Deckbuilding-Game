using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCardDisplay : CardDisplay
{
    [SerializeField] private GameObject unitStats;
    [SerializeField] private GameObject currentAbilitiesDisplay;
    [SerializeField] private GameObject powerScoreDisplay;
    [SerializeField] private GameObject healthScoreDisplay;
    [SerializeField] private GameObject exhaustedIcon;
    [SerializeField] private GameObject destroyedIcon;
    [SerializeField] private GameObject eliteIcon;
    [SerializeField] private GameObject abilityIconPrefab;
    [SerializeField] private GameObject zoomAbilityIconPrefab;

    private AudioManager auMan;
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
            if (value >= 0) displayValue = value;
            DisplayPower(displayValue);
        }
    }
    private void DisplayPower(int power)
    {
        TextMeshProUGUI txtPro =
            powerScoreDisplay.GetComponent<TextMeshProUGUI>();
        txtPro.SetText(power.ToString());
    }

    public int CurrentHealth
    {
        get => UnitCard.CurrentHealth;
        set
        {
            UnitCard.CurrentHealth = value;
            DisplayHealth(value);
        }
    }
    private void DisplayHealth(int health)
    {
        if (health < 0) health = 0;
        TextMeshProUGUI txtPro =
            healthScoreDisplay.GetComponent<TextMeshProUGUI>();
        txtPro.SetText(health.ToString());
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
        auMan = AudioManager.Instance;
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
        eliteIcon.SetActive(UnitCard.IsElite); // TESTING
        foreach (CardAbility cardAbility in UnitCard.StartingAbilities)
            AddCurrentAbility(cardAbility);
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_CARD
     * *****
     *****/
    public override void DisplayZoomCard(GameObject parentCard, Card card = null)
    {
        base.DisplayZoomCard(parentCard, card);

        UnitCard uc;
        if (card == null)
        {
            UnitCardDisplay ucd = parentCard.GetComponent<UnitCardDisplay>();
            uc = ucd.UnitCard;
            List<CardAbility> abilityList;

            if (CardZoom.ZoomCardIsCentered)
            {
                DisplayPower(uc.StartPower);
                DisplayHealth(uc.StartHealth);
                abilityList = uc.StartingAbilities;
            }
            else
            {
                DisplayPower(uc.CurrentPower);
                DisplayHealth(uc.CurrentHealth);
                abilityList = ucd.CurrentAbilities;
            }

            foreach (CardAbility ca in abilityList)
            {
                if (ca == null)
                {
                    Debug.LogError("EMPTY ABILITY!");
                    continue;
                }
                GetComponent<CardZoom>().CreateZoomAbilityIcon(ca,
                    currentAbilitiesDisplay.transform, 1);
            }
        }
        else
        {
            uc = card as UnitCard;
            DisplayPower(uc.StartPower);
            DisplayHealth(uc.StartHealth);

            foreach (CardAbility ca in uc.StartingAbilities)
            {
                if (ca == null)
                {
                    Debug.LogError("EMPTY ABILITY!");
                    continue;
                }
                CurrentAbilities.Add(ca);
            }
            foreach (CardAbility ca in uc.StartingAbilities)
            {
                if (ca == null)
                {
                    Debug.LogError("EMPTY ABILITY!");
                    continue;
                }
                GetComponent<CardZoom>().CreateZoomAbilityIcon(ca,
                    currentAbilitiesDisplay.transform, 1);
            }
        }

        eliteIcon.SetActive(uc.IsElite); // TESTING
    }

    /******
     * *****
     * ****** DISPLAY_CARD_PAGE_CARD
     * *****
     *****/
    public override void DisplayCardPageCard(Card card)
    {
        base.DisplayCardPageCard(card);
        cardScript = card;
        base.DisplayCard();
        CurrentPower = UnitCard.StartPower;
        MaxHealth = UnitCard.StartHealth;
        CurrentHealth = MaxHealth;
        GridLayoutGroup glg =
            currentAbilitiesDisplay.GetComponent<GridLayoutGroup>();
        Vector2 cellSize = glg.cellSize;
        cellSize.y = 16;
        glg.cellSize = cellSize;

        foreach (CardAbility ca in UnitCard.StartingAbilities)
        {
            if (ca == null)
            {
                Debug.LogError("EMPTY ABILITY!");
                continue;
            }
            GetComponent<CardZoom>().CreateZoomAbilityIcon(ca,
                currentAbilitiesDisplay.transform, 1);
        }

        eliteIcon.SetActive(UnitCard.IsElite); // TESTING
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
            displayedAbilities.Clear();
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
                Debug.Log("ABILITY ALREADY EXISTS: <" + ca.AbilityName + ">");
                return false;
            }
            else ShowAbility(ca);
        }
        else if (ca is TriggeredAbility ta)
        {
            if (CardManager.EvergreenTriggers.Contains(ta.AbilityTrigger.AbilityName))
            {
                bool iconsFound = false;
                foreach (CardAbility ca2 in displayedAbilities)
                {
                    if (ca2 is TriggeredAbility ta2)
                    {
                        if (ta2.AbilityTrigger.AbilityName ==
                            ta.AbilityTrigger.AbilityName)
                            iconsFound = true;
                    }
                }
                if (!iconsFound) ShowAbility(ta);
            }
            else if (ta.AbilityTrigger.AbilityName != CardManager.TRIGGER_PLAY)
            {
                Debug.LogError("ABILITY TRIGGER NOT FOUND!");
                return false;
            }
        }

        CurrentAbilities.Add(ca); // Add instances instead? (Doesn't matter yet)

        if (isPlayed)
        {
            if (ca is StaticAbility sa)
                AudioManager.Instance.StartStopSound(null, sa.GainAbilitySound);
        }

        if (ca.AbilityName == CardManager.ABILITY_BLITZ) IsExhausted = false;
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
        else if (ca is TriggeredAbility ta)
        {
            if (ta.AbilityTrigger.AbilityName == CardManager.TRIGGER_PLAY) return;
            if (CardManager.GetTrigger(gameObject, ta.AbilityTrigger.AbilityName)) return;
        }

        if (displayIndex == -1)
        {
            Debug.LogError("ABILITY ICON NOT FOUND!");
            return;
        }

        GameObject icon = AbilityIcons[displayIndex];
        FunctionTimer.Create(() => Destroy(icon), 1);
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

    /******
     * *****
     * ****** ABILITY_TRIGGER_STATE
     * *****
     *****/
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
                    TriggerState(AbilityIcons[displayIndex]);
                    return;
                }
            }
            else if (ca is TriggeredAbility tra)
            {
                if (tra.AbilityTrigger.AbilityName == triggerName)
                {
                    TriggerState(AbilityIcons[displayIndex]);
                    return;
                }
            }
        }

        Debug.LogError("TRIGGER NOT FOUND!");

        void TriggerState(GameObject icon)
        {
            AnimationManager.Instance.AbilityTriggerState(icon);
            auMan.StartStopSound("SFX_Trigger");
            auMan.StartStopSound(null, CardScript.CardPlaySound); // TESTING
        }
    }

    public override void DisableVisuals()
    {
        base.DisableVisuals();
        exhaustedIcon.SetActive(false);
        destroyedIcon.SetActive(false);
    }
}