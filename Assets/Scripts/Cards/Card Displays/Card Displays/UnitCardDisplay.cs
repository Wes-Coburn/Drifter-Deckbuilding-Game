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
    [SerializeField] private GameObject abilityIconPrefab;
    [SerializeField] private GameObject zoomAbilityIconPrefab;

    private AudioManager auMan;
    private List<CardAbility> displayedAbilities;
    
    public UnitCard UnitCard { get => CardScript as UnitCard; }
    public GameObject UnitStats { get => unitStats; }
    public GameObject PowerScore { get => powerScoreDisplay; }
    public GameObject HealthScore { get => healthScoreDisplay; }
    public GameObject ZoomAbilityIconPrefab { get => zoomAbilityIconPrefab; }

    public List<GameObject> AbilityIcons { get; set; }
    public List<CardAbility> CurrentAbilities { get; set; }

    public int CurrentPower
    {
        get
        {
            int power = UnitCard.CurrentPower;
            if (power < 0) power = 0;
            return power;
        }
        private set
        {
            UnitCard.CurrentPower = value;
            DisplayPower(CurrentPower);
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
        get
        {
            int health = UnitCard.CurrentHealth;
            if (health < 0) health = 0;
            return health;
        }
        set
        {
            UnitCard.CurrentHealth = value;
            DisplayHealth(CurrentHealth);
        }
    }
    private void DisplayHealth(int health)
    {
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

    protected override void Awake()
    {
        base.Awake();
        auMan = AudioManager.Instance;
        CurrentAbilities = new List<CardAbility>();
        AbilityIcons = new List<GameObject>();
        displayedAbilities = new List<CardAbility>();
    }

    public void ChangeCurrentPower(int value)
    {
        UnitCard.CurrentPower += value;
        DisplayPower(CurrentPower);
    }

    private GameObject CreateAbilityIcon(CardAbility cardAbility)
    {
        GameObject abilityIcon = Instantiate(abilityIconPrefab, new Vector2(0, 0), Quaternion.identity);
        abilityIcon.GetComponent<AbilityIconDisplay>().AbilityScript = cardAbility;
        abilityIcon.transform.SetParent(currentAbilitiesDisplay.transform, false);
        return abilityIcon;
    }

    protected override void DisplayCard()
    {
        base.DisplayCard();
        CurrentPower = UnitCard.StartPower;
        MaxHealth = UnitCard.StartHealth;
        CurrentHealth = MaxHealth;
        foreach (CardAbility cardAbility in UnitCard.StartingAbilities)
            AddCurrentAbility(cardAbility);
    }

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
    }

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
            if (ta.AbilityTrigger.AbilityName != CardManager.TRIGGER_PLAY)
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
        }
    }

    public override void ResetCard()
    {
        base.ResetCard();
        foreach (GameObject go in AbilityIcons)
            Destroy(go);
        AbilityIcons.Clear();
        displayedAbilities.Clear();
        CurrentAbilities.Clear();
        DisplayCard();
    }

    public override void DisableVisuals()
    {
        base.DisableVisuals();
        exhaustedIcon.SetActive(false);
        destroyedIcon.SetActive(false);
    }
}