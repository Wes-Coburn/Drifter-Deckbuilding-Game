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
    public List<CardAbility> CurrentAbilities { get => UnitCard.CurrentAbilities; }
    public List<CardAbility> DisplayAbilities { get => displayedAbilities; }

    public int CurrentPower
    {
        get
        {
            int power = UnitCard.CurrentPower;
            if (power < 0) power = 0;
            return power;
        }
        set
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
        AbilityIcons = new List<GameObject>();
        displayedAbilities = new List<CardAbility>();
    }

    public void ChangeCurrentPower(int value)
    {
        UnitCard.CurrentPower += value;
        DisplayPower(CurrentPower);
    }

    public Color GetAbilityIconColor(CardAbility cardAbility)
    {
        Color iconColor = Color.white;

        foreach (string posAbi in CardManager.PositiveAbilities)
        {
            if (cardAbility.AbilityName == posAbi)
            {
                iconColor = Color.green;
                break;
            }
        }

        if (iconColor == Color.white)
        {
            foreach (string negAbi in CardManager.NegativeAbilities)
            {
                if (cardAbility.AbilityName == negAbi)
                {
                    iconColor = Color.red;
                    break;
                }
            }
        }

        return iconColor;
    }

    protected override void DisplayCard()
    {
        base.DisplayCard();
        DisplayPower(CurrentPower);
        DisplayHealth(CurrentHealth);
        foreach (CardAbility cardAbility in UnitCard.CurrentAbilities)
            AddCurrentAbility(cardAbility, true);
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
                TextMeshProUGUI powerTxt = powerScoreDisplay.GetComponent<TextMeshProUGUI>();
                if (uc.CurrentPower < uc.StartPower) powerTxt.color = Color.red;
                else if (uc.CurrentPower > uc.StartPower) powerTxt.color = Color.green;

                DisplayHealth(uc.CurrentHealth);
                TextMeshProUGUI healthTxt = healthScoreDisplay.GetComponent<TextMeshProUGUI>();
                if (uc.CurrentHealth < uc.MaxHealth) healthTxt.color = Color.red;
                else if (uc.CurrentHealth > uc.StartHealth) healthTxt.color = Color.green;

                abilityList = ucd.CurrentAbilities;
            }

            ShowAbilities(abilityList);
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

            ShowAbilities(uc.StartingAbilities);
        }

        void ShowAbilities(List<CardAbility> abilityList)
        {
            List<CardAbility> shownAbilities = new List<CardAbility>();
            List<GameObject> shownObjects = new List<GameObject>();
            List<int> shownAbilityCounts = new List<int>();

            foreach (CardAbility ca in abilityList)
            {
                if (ca == null)
                {
                    Debug.LogError("EMPTY ABILITY!");
                    continue;
                }

                int abilityIndex = shownAbilities.FindIndex(x => x.AbilityName == ca.AbilityName);
                if (abilityIndex == -1)
                {
                    GameObject abilityIcon =
                        GetComponent<CardZoom>().CreateZoomAbilityIcon(ca,
                        currentAbilitiesDisplay.transform, 1);

                    AbilityIconDisplay aid = abilityIcon.GetComponent<AbilityIconDisplay>();
                    aid.AbilitySprite.GetComponent<Image>().color = GetAbilityIconColor(ca);

                    shownAbilities.Add(ca);
                    shownObjects.Add(abilityIcon);
                    shownAbilityCounts.Add(1);
                }
                else
                {
                    int abilityCount = ++shownAbilityCounts[abilityIndex];
                    shownObjects[abilityIndex].GetComponent<AbilityIconDisplay>().AbilityMultiplier = abilityCount;
                }
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
        
        GridLayoutGroup glg = currentAbilitiesDisplay.GetComponent<GridLayoutGroup>();
        Vector2 cellSize = glg.cellSize;
        cellSize.y = 12;
        glg.cellSize = cellSize;
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = 1;

        foreach (CardAbility ca in UnitCard.StartingAbilities)
        {
            if (ca == null)
            {
                Debug.LogError("EMPTY ABILITY!");
                continue;
            }

            GameObject abilityIcon = GetComponent<CardZoom>().CreateZoomAbilityIcon(ca,
                currentAbilitiesDisplay.transform, 1);

            AbilityIconDisplay aid = abilityIcon.GetComponent<AbilityIconDisplay>();
            aid.AbilitySprite.GetComponent<Image>().color = GetAbilityIconColor(ca);
        }
    }

    public override void DisplayChooseCard(Card card)
    {
        base.DisplayChooseCard(card);
        DisplayCardPageCard(card);        
    }

    /******
     * *****
     * ****** ADD_CURRENT_ABILITY
     * *****
     *****/
    public bool AddCurrentAbility(CardAbility ca, bool iconOnly = false)
    {
        // NEW CARD ABILITY INSTANCE
        /*
        CardAbility newCardAbility = ScriptableObject.CreateInstance(ca.GetType().Name) as CardAbility;
        newCardAbility.LoadCardAbility(ca);
        ca = newCardAbility;
        */

        if (ca is StaticAbility sa)
        {
            if (iconOnly)
            {
                int abilityIndex = displayedAbilities.FindIndex(x => x.AbilityName == ca.AbilityName);
                if (abilityIndex == -1) ShowAbility(ca);
                return true;
            }

            if (CardManager.GetAbility(gameObject, ca.AbilityName))
            {
                Debug.Log("ABILITY ALREADY EXISTS: <" + ca.AbilityName + ">");
                if (!sa.AbilityStacks) return false;
            }
            else ShowAbility(ca);
        }
        else if (ca is TriggeredAbility ta)
        {
            if (ta.AbilityTrigger.AbilityName != CardManager.TRIGGER_PLAY)
            {
                bool iconFound = false;
                foreach (CardAbility ca2 in displayedAbilities)
                {
                    if (ca2 is TriggeredAbility ta2)
                    {
                        if (ta2.AbilityTrigger.AbilityName == ta.AbilityTrigger.AbilityName)
                            iconFound = true;
                    }
                }
                if (!iconFound) ShowAbility(ta);
            }
        }
        else if (ca is ModifierAbility ma)
        {
            bool iconFound = false;
            foreach (CardAbility ca3 in displayedAbilities)
            {
                if (ca3 is ModifierAbility ma2) iconFound = true;
            }
            if (!iconFound) ShowAbility(ma);
        }

        if (!iconOnly)
        {
            CurrentAbilities.Add(ca);
            string abilityName;
            if (ca is StaticAbility) abilityName = ca.AbilityName;
            else if (ca is TriggeredAbility ta2) abilityName = ta2.AbilityTrigger.AbilityName;
            else
            {
                Debug.LogError("INVALID ABILITY TYPE!");
                return false;
            }
            AbilityTriggerState(abilityName);
        }

        if (ca is StaticAbility sa2) AudioManager.Instance.StartStopSound(null, sa2.GainAbilitySound);
        if (ca.AbilityName == CardManager.ABILITY_BLITZ) IsExhausted = false;
        return true;

        void ShowAbility(CardAbility ca)
        {
            if (ca.AbilityName == CardManager.ABILITY_BLITZ) return;

            AbilityIcons.Add(CreateAbilityIcon(ca));
            displayedAbilities.Add(ca);
        }

        GameObject CreateAbilityIcon(CardAbility cardAbility)
        {
            GameObject abilityIcon = Instantiate(abilityIconPrefab, new Vector2(0, 0), Quaternion.identity);
            AbilityIconDisplay aid = abilityIcon.GetComponent<AbilityIconDisplay>();
            aid.AbilityScript = cardAbility;
            abilityIcon.transform.SetParent(currentAbilitiesDisplay.transform, false);
            aid.AbilitySprite.GetComponent<Image>().color = GetAbilityIconColor(cardAbility);
            return abilityIcon;
        }
    }

    /******
     * *****
     * ****** REMOVE_CURRENT_ABILITY
     * *****
     *****/
    public void RemoveCurrentAbility(string abilityName, bool delay = true)
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
        {
            if (sa.AbilityStacks)
            {
                List<CardAbility> stackedAbilities = new List<CardAbility>();
                foreach (CardAbility ability in CurrentAbilities)
                    if (ability.AbilityName == ca.AbilityName)
                        stackedAbilities.Add(ability);

                foreach (CardAbility stackedCa in stackedAbilities)
                    CurrentAbilities.Remove(stackedCa);
            }

            AudioManager.Instance.StartStopSound(null, sa.LoseAbilitySound);
        }
        else if (ca is TriggeredAbility ta)
        {
            if (ta.AbilityTrigger.AbilityName == CardManager.TRIGGER_PLAY) return;
            if (CardManager.GetTrigger(gameObject, ta.AbilityTrigger.AbilityName)) return;
        }
        else if (ca is ModifierAbility ma)
        {
            List<CardAbility> modAbilities = new List<CardAbility>();
            foreach (CardAbility ability in CurrentAbilities)
                if (ability.AbilityName == ca.AbilityName)
                    modAbilities.Add(ability);

            foreach (CardAbility modCa in modAbilities)
                CurrentAbilities.Remove(modCa);

            foreach (CardAbility ability2 in CurrentAbilities)
                if (ability2 is ModifierAbility) return;
        }

        if (displayIndex == -1)
        {
            Debug.LogError("ABILITY ICON NOT FOUND!");
            return;
        }

        GameObject icon = AbilityIcons[displayIndex];
        AbilityIcons.RemoveAt(displayIndex);
        displayedAbilities.RemoveAt(displayIndex);

        if (delay) FunctionTimer.Create(() => Destroy(icon), 1);
        else Destroy(icon);
    }

    /******
     * *****
     * ****** ABILITY_TRIGGER_STATE
     * *****
     *****/
    public void AbilityTriggerState(string abilityName)
    {
        if (abilityName == CardManager.TRIGGER_PLAY ||
            abilityName == CardManager.ABILITY_BLITZ) return;

        foreach (CardAbility ca in displayedAbilities)
        {
            int displayIndex =
                displayedAbilities.FindIndex(x => x.AbilityName == ca.AbilityName);

            if (ca is StaticAbility sa)
            {
                if (ca.AbilityName == abilityName)
                {
                    TriggerState(AbilityIcons[displayIndex], sa.GainAbilitySound);
                    return;
                }
            }
            else if (ca is TriggeredAbility tra)
            {
                if (tra.AbilityTrigger.AbilityName == abilityName)
                {
                    TriggerState(AbilityIcons[displayIndex], tra.AbilityTrigger.TriggerSound);
                    return;
                }
            }
            else if (ca is ModifierAbility ma)
            {
                if (ma.AbilityName == abilityName)
                {
                    TriggerState(AbilityIcons[displayIndex], null);
                    return;
                }
            }
        }

        Debug.LogError("TRIGGER NOT FOUND!");

        void TriggerState(GameObject icon, Sound triggerSFX)
        {
            AnimationManager.Instance.AbilityTriggerState(icon);
            if (triggerSFX != null) auMan.StartStopSound(null, triggerSFX);
            else auMan.StartStopSound("SFX_Trigger");
        }
    }

    public override void ResetCard()
    {
        base.ResetCard();
        CurrentPower = UnitCard.StartPower;
        CurrentHealth = UnitCard.StartHealth;
        MaxHealth = UnitCard.StartHealth;

        foreach (GameObject go in AbilityIcons)
            Destroy(go);
        AbilityIcons.Clear();
        displayedAbilities.Clear();
        CurrentAbilities.Clear();
        foreach (CardAbility ca in UnitCard.StartingAbilities)
            CurrentAbilities.Add(ca);
        foreach (CardAbility ca in CurrentAbilities)
            AddCurrentAbility(ca, true);
        DisplayCard();
    }

    public override void DisableVisuals()
    {
        base.DisableVisuals();
        exhaustedIcon.SetActive(false);
        destroyedIcon.SetActive(false);
    }
}