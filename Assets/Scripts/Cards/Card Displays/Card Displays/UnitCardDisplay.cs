using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardDisplay : CardDisplay
{
    [Header("Stats"), SerializeField] private GameObject unitStats;
    [SerializeField] private GameObject powerScoreDisplay;
    [SerializeField] private GameObject healthScoreDisplay;
    [SerializeField] private GameObject currentAbilitiesDisplay;
    [Header("Visual Effects"), SerializeField] private GameObject exhaustedIcon;
    [SerializeField] private GameObject cardDimmer;
    [SerializeField] private GameObject destroyedIcon;
    [Header("Ability VFX"), SerializeField] private GameObject vfx_Defender;
    [SerializeField] private GameObject vfx_Forcefield;
    [SerializeField] private GameObject vfx_Stealth;
    [SerializeField] private GameObject vfx_Ward;
    [Header("Prefabs"), SerializeField] private GameObject abilityIconPrefab;
    [SerializeField] private GameObject zoomAbilityIconPrefab;

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
        var txtPro = healthScoreDisplay.GetComponent<TextMeshProUGUI>();
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
            cardDimmer.SetActive(value);
        }
    }

    protected void Awake()
    {
        AbilityIcons = new List<GameObject>();
        displayedAbilities = new List<CardAbility>();
    }

    public void ChangeCurrentPower(int value)
    {
        UnitCard.CurrentPower += value;
        DisplayPower(CurrentPower);
    }

    protected override void DisplayCard()
    {
        base.DisplayCard();
        DisplayPower(CurrentPower);
        DisplayHealth(CurrentHealth);

        vfx_Defender.SetActive(false);
        vfx_Forcefield.SetActive(false);
        vfx_Stealth.SetActive(false);
        vfx_Ward.SetActive(false);

        foreach (CardAbility cardAbility in UnitCard.CurrentAbilities) // Reverse without modifying the list .AsEnumerable().Reverse()
            AddCurrentAbility(cardAbility, true);
    }

    public override void DisplayZoomCard(GameObject parentCard, Card card = null)
    {
        base.DisplayZoomCard(parentCard, card);

        UnitCard uc;
        if (card == null) // For zoom cards based on a child card
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
                DisplayPower(ucd.CurrentPower);
                TextMeshProUGUI powerTxt = powerScoreDisplay.GetComponent<TextMeshProUGUI>();
                if (ucd.CurrentPower < uc.StartPower) powerTxt.color = Color.red;
                else if (ucd.CurrentPower > uc.StartPower) powerTxt.color = Color.green;

                DisplayHealth(ucd.CurrentHealth);
                TextMeshProUGUI healthTxt = healthScoreDisplay.GetComponent<TextMeshProUGUI>();
                if (ucd.CurrentHealth < ucd.MaxHealth) healthTxt.color = Color.red;
                else if (ucd.CurrentHealth > uc.StartHealth) healthTxt.color = Color.green;

                abilityList = ucd.CurrentAbilities;
            }

            ShowAbilities(abilityList);
        }
        else // For zoom cards NOT based on a child card, for formatting only
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
            List<CardAbility> shownAbilities = new();
            List<GameObject> shownObjects = new();
            List<int> shownAbilityCounts = new();

            foreach (CardAbility ca in abilityList) // Reverse without modifying the list .AsEnumerable().Reverse()
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
                    aid.AbilitySprite.GetComponent<Image>().color = Managers.CA_MAN.GetAbilityColor(ca);

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

        currentAbilitiesDisplay.transform.localPosition = new Vector2(0, -15);
        var glg = currentAbilitiesDisplay.GetComponent<GridLayoutGroup>();
        glg.startAxis = GridLayoutGroup.Axis.Vertical;
        glg.constraintCount = 1;

        foreach (var ca in UnitCard.StartingAbilities.AsEnumerable().Reverse()) // Reverse without modifying the list
        {
            if (ca == null)
            {
                Debug.LogError($"EMPTY ABILITY! <{card.CardName}>");
                continue;
            }

            var abilityIcon = GetComponent<CardZoom>().CreateZoomAbilityIcon(ca,
                currentAbilitiesDisplay.transform, 1);

            var aid = abilityIcon.GetComponent<AbilityIconDisplay>();
            aid.AbilitySprite.GetComponent<Image>().color = Managers.CA_MAN.GetAbilityColor(ca);
        }
    }

    public void EnableTriggerIcon(AbilityTrigger abilityTrigger, bool isEnabled)
    {
        int index = 0;
        foreach (var ca in displayedAbilities)
        {
            if (abilityTrigger != null)
            {
                if (ca is TriggeredAbility tra &&
                    tra.AbilityTrigger.AbilityName ==
                    abilityTrigger.AbilityName) ToggleIcon();
            }
            else if (ca is ModifierAbility) ToggleIcon();
            
            index++;

            void ToggleIcon()
            {
                Color color;
                if (isEnabled) color = Managers.CA_MAN.GetAbilityColor(ca);
                else color = Color.gray;

                var icon = AbilityIcons[index];
                var aid = icon.GetComponent<AbilityIconDisplay>();
                aid.AbilitySprite.GetComponent<Image>().color = color;
            }
        }
    }

    private GameObject CreateAbilityIcon(CardAbility cardAbility)
    {
        var abilityIcon = Instantiate(abilityIconPrefab, new Vector2(0, 0), Quaternion.identity);
        var aid = abilityIcon.GetComponent<AbilityIconDisplay>();
        aid.AbilityScript = cardAbility;
        abilityIcon.transform.SetParent(currentAbilitiesDisplay.transform, false);
        aid.AbilitySprite.GetComponent<Image>().color = Managers.CA_MAN.GetAbilityColor(cardAbility);
        return abilityIcon;
    }

    /******
     * *****
     * ****** ADD_CURRENT_ABILITY
     * *****
     *****/
    public bool AddCurrentAbility(CardAbility ca, bool iconOnly = false, bool showEffect = true)
    {
        var newCardAbility = ScriptableObject.CreateInstance(ca.GetType().Name) as CardAbility;
        newCardAbility.LoadCardAbility(ca);
        ca = newCardAbility;

        if (ca is StaticAbility sa)
        {
            switch (sa.AbilityName)
            {
                case CardManager.ABILITY_DEFENDER:
                    if (!CardManager.GetAbility(gameObject, CardManager.ABILITY_STEALTH))
                        vfx_Defender.SetActive(true);
                    break;
                case CardManager.ABILITY_FORCEFIELD:
                    vfx_Forcefield.SetActive(true);
                    break;
                case CardManager.ABILITY_STEALTH:
                    vfx_Stealth.SetActive(true);
                    break;
                case CardManager.ABILITY_WARD:
                    vfx_Ward.SetActive(true);
                    break;
            }

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
                int index = 0;
                foreach (var ca2 in displayedAbilities)
                {
                    if (ca2 is TriggeredAbility ta2)
                    {
                        if (ta2.AbilityTrigger.AbilityName == ta.AbilityTrigger.AbilityName)
                        {
                            iconFound = true;

                            // When adding another ability with the same trigger, reset the ability icon in case it's disabled (gray)
                            var aid = AbilityIcons[index].GetComponent<AbilityIconDisplay>();
                            aid.AbilitySprite.GetComponent<Image>().color = Managers.CA_MAN.GetAbilityColor(ca2);
                        }
                    }
                    index++;
                }
                if (!iconFound) ShowAbility(ta);
            }
        }
        else if (ca is ModifierAbility ma)
        {
            bool iconFound = false;
            foreach (var ca3 in displayedAbilities)
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
            if (showEffect) AbilityTriggerState(abilityName);
        }

        if (showEffect && ca is StaticAbility sa2) AudioManager.Instance.StartStopSound(null, sa2.GainAbilitySound);
        if (ca.AbilityName == CardManager.ABILITY_BLITZ) IsExhausted = false;
        return true;

        void ShowAbility(CardAbility ca)
        {
            if (ca.AbilityName == CardManager.ABILITY_BLITZ) return;
            AbilityIcons.Add(CreateAbilityIcon(ca));
            displayedAbilities.Add(ca);
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
            Debug.LogWarning("ABILITY <" + abilityName + "> NOT FOUND!");
            return;
        }
        else Debug.Log("ABILITY <" + abilityName + "> REMOVED!");

        switch (abilityName)
        {
            case CardManager.ABILITY_DEFENDER:
                vfx_Defender.SetActive(false);
                break;
            case CardManager.ABILITY_FORCEFIELD:
                vfx_Forcefield.SetActive(false);
                break;
            case CardManager.ABILITY_STEALTH:
                vfx_Stealth.SetActive(false);
                if (CardManager.GetAbility(gameObject,
                    CardManager.ABILITY_DEFENDER)) vfx_Defender.SetActive(true);
                break;
            case CardManager.ABILITY_WARD:
                vfx_Ward.SetActive(false);
                break;
        }

        var ca = CurrentAbilities[abilityIndex];
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

            Managers.AU_MAN.StartStopSound(null, sa.LoseAbilitySound);
        }
        else if (ca is TriggeredAbility ta)
        {
            string triggerName = ta.AbilityTrigger.AbilityName;
            if (triggerName == CardManager.TRIGGER_PLAY) return;
            if (CardManager.GetTrigger(gameObject, triggerName)) return;
        }
        else if (ca is ModifierAbility ma)
        {
            List<CardAbility> modAbilities = new();
            foreach (var ability in CurrentAbilities)
                if (ability.AbilityName == ca.AbilityName)
                    modAbilities.Add(ability);

            foreach (var modCa in modAbilities)
                CurrentAbilities.Remove(modCa);

            foreach (var ability2 in CurrentAbilities)
                if (ability2 is ModifierAbility) return;
        }

        if (displayIndex == -1)
        {
            if (abilityName != CardManager.ABILITY_BLITZ)
                Debug.LogError("ABILITY ICON FOR <" + abilityName + "> NOT FOUND!");

            return;
        }

        var icon = AbilityIcons[displayIndex];
        AbilityIcons.RemoveAt(displayIndex);
        displayedAbilities.RemoveAt(displayIndex);

        if (delay) FunctionTimer.Create(() => DestroyIcon(icon), 1);
        else DestroyIcon(icon);

        static void DestroyIcon(GameObject icon)
        {
            if (icon != null) Destroy(icon);
        }
    }

    /******
     * *****
     * ****** ABILITY_TRIGGER_STATE
     * *****
     *****/
    public void AbilityTriggerState(string abilityName)
    {
        if (abilityName == CardManager.TRIGGER_PLAY || abilityName == CardManager.ABILITY_BLITZ) return;

        foreach (var ca in displayedAbilities)
        {
            int displayIndex = displayedAbilities.FindIndex(x => x.AbilityName == ca.AbilityName);

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

        static void TriggerState(GameObject icon, Sound triggerSFX)
        {
            Managers.AN_MAN.AbilityTriggerState(icon);
            if (triggerSFX != null && triggerSFX.clip != null)
                Managers.AU_MAN.StartStopSound(null, triggerSFX);
            else Managers.AU_MAN.StartStopSound("SFX_Trigger");
        }
    }

    public override void ResetCard()
    {
        base.ResetCard();
        CurrentPower = UnitCard.StartPower;
        CurrentHealth = UnitCard.StartHealth;
        MaxHealth = UnitCard.StartHealth;

        foreach (var go in AbilityIcons) Destroy(go);
        AbilityIcons.Clear();
        displayedAbilities.Clear();
        CurrentAbilities.Clear();
        foreach (var ca in UnitCard.StartingAbilities)
        {
            var newCa = ScriptableObject.CreateInstance(ca.GetType().Name) as CardAbility;
            newCa.LoadCardAbility(ca);
            CurrentAbilities.Add(newCa);
        }
        foreach (var ca in CurrentAbilities)
            AddCurrentAbility(ca, true);

        ResetEffects();
        DisplayCard();
    }

    public override void DisableVisuals()
    {
        base.DisableVisuals();
        exhaustedIcon.SetActive(false);
        cardDimmer.SetActive(false);
        destroyedIcon.SetActive(false);
        DisableVFX();
    }

    public void DisableVFX()
    {
        vfx_Defender.SetActive(false);
        vfx_Forcefield.SetActive(false);
        vfx_Stealth.SetActive(false);
        vfx_Ward.SetActive(false);
    }

    public void EnableVFX()
    {
        if (CardManager.GetAbility(gameObject, CardManager.ABILITY_FORCEFIELD))
            vfx_Forcefield.SetActive(true);
        if (CardManager.GetAbility(gameObject, CardManager.ABILITY_WARD))
            vfx_Ward.SetActive(true);

        if (CardManager.GetAbility(gameObject, CardManager.ABILITY_STEALTH))
            vfx_Stealth.SetActive(true);
        else if (CardManager.GetAbility(gameObject, CardManager.ABILITY_DEFENDER))
            vfx_Defender.SetActive(true);
    }
}