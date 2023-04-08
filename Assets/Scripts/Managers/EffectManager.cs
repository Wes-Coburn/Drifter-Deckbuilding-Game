using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static EffectManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    #region FIELDS

    private GameObject effectSource, savedTarget, dragArrow;
    private int currentEffectGroup, currentEffect, activeEffects, savedValue;
    private bool effectsResolving, isAdditionalEffect;
    private string triggerName;

    private List<EffectGroup> effectGroupList, additionalEffectGroups;
    private List<List<GameObject>> legalTargets, acceptedTargets;
    private List<List<Card>> acceptedTargets_Cards;
    private List<GameObject> unitsToDestroy;

    [Header("Effect Ray"), SerializeField] private GameObject effectRay;
    [Header("Ray Colors"), SerializeField] private Color damageRayColor;
    [SerializeField] private Color healRayColor;
    [Header("Poison Ability"), SerializeField] private CardAbility poisonAbility;
    #endregion

    #region PROPERTIES
    /*
     * Call FinishEffectGroupList() when the last effect resolves.
     * Only used for effects in an EffectGroup.
     */
    public int ActiveEffects
    {
        get => activeEffects;
        set
        {
            activeEffects = value;
            //Debug.Log("ACTIVE EFFECTS: " + activeEffects);
            if (activeEffects == 0) FinishEffectGroupList(false);
            else if (activeEffects < 0) Debug.LogError("ACTIVE EFFECTS < 0!");
        }
    }
    /*
     * While effects are resolving, disable the end turn button and pause all delayed actions.
     */
    public bool EffectsResolving
    {
        get => EffectRay.ActiveRays > 0 || effectsResolving;
        set
        {
            effectsResolving = value;
            Managers.EV_MAN.PauseDelayedActions(value);
            Managers.U_MAN.UpdateEndTurnButton(!value);
        }
    }
    /*
     * Quickly return the Effect currently resolving.
     */
    public Effect CurrentEffect
    {
        get
        {
            var effects = CurrentEffectGroup != null ? CurrentEffectGroup.Effects : null;
            if (effects == null) return null;

            if (currentEffect > effects.Count - 1)
            {
                Debug.LogError("CURRENT EFFECT > EFFECT LIST");
                return null;
            }
            else return effects[currentEffect];
        }
    }
    /*
     * Quickly return the EffectGroup currently resolving.
     */
    public EffectGroup CurrentEffectGroup
    {
        get
        {
            if (effectGroupList == null)
            {
                Debug.LogError("GROUP LIST IS NULL!");
                return null;
            }
            else if (currentEffectGroup > effectGroupList.Count - 1)
            {
                Debug.LogError("CURRENT GROUP > GROUP LIST!");
                return null;
            }

            return effectGroupList[currentEffectGroup];
        }
    }

    public List<GameObject> UnitsToDestroy { get => unitsToDestroy; }
    public Color DamageRayColor { get => damageRayColor; }
    public CardAbility PoisonAbility { get => poisonAbility; }
    #endregion

    #region METHODS

    #region UTILITY
    private void Start()
    {
        effectsResolving = false;
        effectGroupList = new();
        legalTargets = new();
        acceptedTargets = new();
        acceptedTargets_Cards = new();
        additionalEffectGroups = new();
        unitsToDestroy = new();
    }

    public void Reset_EffectManager() => effectsResolving = false;
    private bool IsTargetEffect(EffectGroup group, Effect effect)
    {
        var tgts = group.Targets;

        if (tgts.NoTargets || tgts.SavedTarget || effect is DelayEffect or SaveValueEffect ||
            (effect is DrawEffect de && de.IsDiscardEffect && de.DiscardAll)) return false;

        if (tgts.TargetsAll || tgts.TargetsSelf || tgts.PlayerDeck ||
            tgts.TargetsLowestHealth || tgts.TargetsStrongest || tgts.TargetsWeakest ||
            tgts.TargetsHighestCost || tgts.TargetsLowestCost) return false;

        if ((tgts.PlayerHero || tgts.EnemyHero) && (!tgts.PlayerUnit && !tgts.EnemyUnit)) return false;

        return true;
    }
    #endregion

    #region EFFECT PROCESSING
    /******
     * *****
     * ****** START_EFFECT_GROUP_LIST
     * *****
     *****/
    public void StartEffectGroupList(List<EffectGroup> groupList, GameObject source, string triggerName = null)
    {
        Debug.Log("<<< START EFFECT GROUP LIST! >>>");

        if (source == null)
        {
            Debug.LogError("SOURCE IS NULL!");
            return;
        }

        if (groupList == null || groupList.Count < 1)
        {
            Debug.LogError("GROUP LIST IS EMPTY!");
            return;
        }

        if (EffectsResolving)
        {
            Debug.LogWarning("GROUP LIST DELAYED (EFFECTS RESOLVING)!");
            Managers.EV_MAN.NewDelayedAction(() =>
            StartEffectGroupList(groupList, source, triggerName), 0.1f, true);
            return;
        }

        EffectsResolving = true;
        effectSource = source;
        this.triggerName = triggerName;
        currentEffectGroup = 0;
        currentEffect = 0;
        savedValue = 0;
        effectGroupList.Clear();

        foreach (var eg in groupList)
        {
            if (eg == null)
            {
                Debug.LogError("EMPTY EFFECT GROUP!");
                continue;
            }

            effectGroupList.Add(eg);
        }

        if (effectGroupList.Count < 1)
        {
            Debug.LogError("EMPTY EFFECT GROUP LIST!");
            EffectsResolving = false;
            Managers.U_MAN.PlayerIsTargetting = false;
            return;
        }

        // ADDITIONAL EFFECTS
        if (additionalEffectGroups.Count > 0)
        {
            isAdditionalEffect = true;
            additionalEffectGroups.RemoveAt(0);
        }
        else isAdditionalEffect = false;

        // UNIT TRIGGER VFX
        if (effectSource.TryGetComponent(out UnitCardDisplay ucd))
        {
            if (!isAdditionalEffect && !string.IsNullOrEmpty(triggerName))
                ucd.AbilityTriggerState(triggerName);
        }

        // CHECK LEGAL TARGETS
        if (!CheckLegalTargets(effectGroupList, effectSource)) CancelEffectGroupList(false);
        else if (!isAdditionalEffect)
        {
            // RESOLVE INDEPENDENT
            List<EffectGroup> resolveIndependent = new();
            foreach (var eg in effectGroupList)
            {
                if (eg.ResolveIndependent)
                {
                    resolveIndependent.Add(eg);
                    additionalEffectGroups.Add(eg);
                }
            }
            foreach (var eg in resolveIndependent) effectGroupList.Remove(eg);

            if (effectGroupList.Count < 1) FinishEffectGroupList(false);
            else StartNextEffectGroup(true);
        }
        else StartNextEffectGroup(true);
    }

    /******
     * *****
     * ****** START_NEXT_EFFECT_GROUP
     * *****
     *****/
    private void StartNextEffectGroup(bool isFirstGroup = false)
    {
        //Debug.Log("START NEXT EFFECT GROUP!");
        if (!isFirstGroup) currentEffectGroup++;
        if (currentEffectGroup < effectGroupList.Count)
        {
            Debug.Log($"[EFFECT GROUP #{currentEffectGroup + 1}] <{effectGroupList[currentEffectGroup]}> ");
            StartNextEffect(true);
        }
        else if (currentEffectGroup == effectGroupList.Count) ResolveEffectGroupList();
        else Debug.LogError("EffectGroup > GroupList!");
    }

    /******
     * *****
     * ****** START_NEXT_EFFECT
     * *****
     *****/
    private void StartNextEffect(bool isFirstEffect = false)
    {
        var eg = effectGroupList[currentEffectGroup];
        currentEffect = isFirstEffect ? 0 : currentEffect + 1;

        if (currentEffect == eg.Effects.Count) StartNextEffectGroup();
        else if (currentEffect < eg.Effects.Count)
        {
            Debug.Log($"[EFFECT #{currentEffect + 1}] <{eg.Effects[currentEffect]}>");

            var effect = eg.Effects[currentEffect];
            if (IsTargetEffect(eg, effect)) StartTargetEffect();
            else StartNonTargetEffect();
        }
        else Debug.LogError("CurrentEffect > Effects!");
    }

    /******
     * *****
     * ****** START_NON_TARGET_EFFECT
     * *****
     *****/
    private void StartNonTargetEffect()
    {
        var et = effectGroupList[currentEffectGroup].Targets;
        var targets = acceptedTargets[currentEffectGroup];
        var targets_Cards = acceptedTargets_Cards[currentEffectGroup];
        var hMan_Source = HeroManager.GetSourceHero(effectSource, out HeroManager hMan_Enemy);

        if (et.NoTargets)
        {
            ConfirmNonTargetEffect();
            return;
        }

        if (et.SavedTarget) AddTarget(savedTarget);
        if (et.TargetsSelf) AddTarget(effectSource);

        if (et.PlayerHero) AddTarget(hMan_Source.HeroObject);
        if (et.EnemyHero) AddTarget(hMan_Enemy.HeroObject);
        if (et.TargetsAll)
        {
            if (et.PlayerUnit) AddAllTargets(hMan_Source.PlayZoneCards);
            if (et.EnemyUnit) AddAllTargets(hMan_Enemy.PlayZoneCards);
            if (et.PlayerHand) AddAllTargets(hMan_Source.HandZoneCards);
            if (et.PlayerDeck) AddAllTargets_Cards(hMan_Source.CurrentDeck);
        }
        /* // Effects that target some cards in the player's deck (the top X)
        else if (et.PlayerDeck)
        {
            if (et.UnitCard || et.ActionCard)
            {
                int cardsAdded = 0;
                foreach (var card in hMan_Source.CurrentDeck)
                    if (IsValidTarget(card) &&
                        AddTarget_Card(card) &&
                        ++cardsAdded >= et.TargetNumber) break;
            }
            else Debug.LogError("INVALID EFFECT TARGET!");

            bool IsValidTarget(Card card) => (card is UnitCard && et.UnitCard) || (card is ActionCard && et.ActionCard);
        }
        */
        if (et.TargetsLowestHealth)
        {
            if (et.PlayerUnit) AddLowHealthUnit(hMan_Source.PlayZoneCards, false);
            if (et.EnemyUnit) AddLowHealthUnit(hMan_Enemy.PlayZoneCards, true);
        }
        if (et.TargetsStrongest)
        {
            if (et.PlayerUnit) AddStrongestUnit(hMan_Source.PlayZoneCards, false);
            if (et.EnemyUnit) AddStrongestUnit(hMan_Enemy.PlayZoneCards, true);
        }
        if (et.TargetsWeakest)
        {
            if (et.PlayerUnit) AddWeakestUnit(hMan_Source.PlayZoneCards, false);
            if (et.EnemyUnit) AddWeakestUnit(hMan_Enemy.PlayZoneCards, true);
        }
        if (et.TargetsHighestCost)
        {
            if (et.PlayerHand) AddHighCostTarget(hMan_Source.HandZoneCards);//, false);
        }
        if (et.TargetsLowestCost)
        {
            if (et.PlayerHand) AddLowCostTarget(hMan_Source.HandZoneCards);//, false);
        }

        ConfirmNonTargetEffect();

        void AddAllTargets(List<GameObject> cardZone)
        {
            foreach (var card in cardZone) AddTarget(card);
        }

        void AddAllTargets_Cards(List<Card> cardZone)
        {
            foreach (var card in cardZone) AddTarget_Card(card);
        }

        void AddLowHealthUnit(List<GameObject> cardZone, bool targetsEnemy)
        {
            if (targets.Count > 0) return;
            var target = Managers.CO_MAN.GetLowestHealthUnit(cardZone, targetsEnemy);
            if (target != null) AddTarget(target);
        }

        void AddStrongestUnit(List<GameObject> cardZone, bool targetsEnemy)
        {
            if (targets.Count > 0) return;
            var target = Managers.CO_MAN.GetStrongestUnit(cardZone, targetsEnemy);
            if (target != null) AddTarget(target);
        }

        void AddWeakestUnit(List<GameObject> cardZone, bool targetsEnemy)
        {
            if (targets.Count > 0) return;
            var target = Managers.CO_MAN.GetWeakestUnit(cardZone, targetsEnemy);
            if (target != null) AddTarget(target);
        }

        void AddHighCostTarget(List<GameObject> cardZone)//, bool targetsEnemy)
        {
            if (targets.Count > 0) return;
            var target = Managers.CO_MAN.GetHighestCostTarget(cardZone);
            if (target != null) AddTarget(target);
        }

        void AddLowCostTarget(List<GameObject> cardZone)//, bool targetsEnemy)
        {
            if (targets.Count > 0) return;
            var target = Managers.CO_MAN.GetLowestCostTarget(cardZone);
            if (target != null) AddTarget(target);
        }

        bool AddTarget(GameObject target)
        {
            if (target == null)
            {
                Debug.LogError("TARGET IS NULL!");
                return false;
            }
            if (targets.Contains(target)) return false;
            if (hMan_Source != HeroManager.GetSourceHero(target))
            {
                if (target.TryGetComponent(out DragDrop dd) && dd.IsPlayed)
                {
                    if (CardManager.GetAbility(target, CardManager.ABILITY_WARD)) return false;
                }
            }
            targets.Add(target);
            return true;
        }

        bool AddTarget_Card(Card target)
        {
            if (target == null)
            {
                Debug.LogError("TARGET IS NULL!");
                return false;
            }
            if (targets_Cards.Contains(target)) return false;
            targets_Cards.Add(target);
            return true;
        }
    }

    /******
     * *****
     * ****** START_TARGET_EFFECT
     * *****
     *****/
    private void StartTargetEffect()
    {
        Debug.Log("START TARGET EFFECT!");
        var effect = CurrentEffect;

        if (acceptedTargets[currentEffectGroup].Count > 0)
        {
            StartNextEffectGroup();
            return;
        }

        // Prevent targetting same object twice in an EFFECT GROUP LIST
        List<GameObject> redundancies = new();
        foreach (var target in legalTargets[currentEffectGroup])
        {
            if (target == null)
            {
                Debug.LogError("TARGET IS NULL!");
                continue;
            }

            foreach (var targetList in acceptedTargets)
            {
                if (targetList.Contains(target)) redundancies.Add(target);
            }
        }
        foreach (var target in redundancies)
        {
            foreach (var targetList in legalTargets)
                targetList.Remove(target);
        }

        // Non-Required effects that returned TRUE from GetLegalTargets, but have 0 legal targets
        if (effect is DrawEffect de && de.IsDiscardEffect) { }
        else if (legalTargets[currentEffectGroup].Count < 1)
        {
            Debug.Log("EFFECT CONFIRMED WITH NO LEGAL TARGETS!");
            ConfirmTargetEffect();
            return;
        }

        // ENEMY BEHAVIOR
        if (HeroManager.GetSourceHero(effectSource) == Managers.EN_MAN)
        {
            var targets = effectGroupList[currentEffectGroup].Targets;
            var availableTargets = legalTargets[currentEffectGroup];
            var confirmedTargets = acceptedTargets[currentEffectGroup];
            List<GameObject> priorityTargets = new();

            bool isNegativeEffect = true;
            if (effect is HealEffect || (effect is ExhaustEffect ee && !ee.SetExhausted)) isNegativeEffect = false;

            foreach (var t in availableTargets)
            {
                if (HeroManager.GetSourceHero(t) == Managers.P_MAN)
                {
                    if (isNegativeEffect) priorityTargets.Add(t);
                }
                else if (!isNegativeEffect) priorityTargets.Add(t);
            }

            int totalTargets = availableTargets.Count;
            if (totalTargets > targets.TargetNumber) totalTargets = targets.TargetNumber;

            while (confirmedTargets.Count < totalTargets)
            {
                GameObject target;
                if (priorityTargets.Count > 0)
                {
                    if (targets.EnemyUnit && (!targets.EnemyHero))
                        target = Managers.CO_MAN.GetStrongestUnit(priorityTargets, true);
                    else if (targets.PlayerUnit && (!targets.PlayerHero))
                        target = Managers.CO_MAN.GetStrongestUnit(priorityTargets, false);
                    else target = GetRandomTarget(priorityTargets);
                    priorityTargets.Remove(target);
                }
                else target = availableTargets[UnityEngine.Random.Range(0, availableTargets.Count)];

                availableTargets.Remove(target);
                confirmedTargets.Add(target);
            }

            ConfirmTargetEffect();
            return;

            static GameObject GetRandomTarget(List<GameObject> targets) =>
                targets[UnityEngine.Random.Range(0, targets.Count)];
        }

        // PLAYER TARGETTING
        Managers.U_MAN.PlayerIsTargetting = true;
        string description = effectGroupList[currentEffectGroup].EffectsDescription;
        var et = effectGroupList[currentEffectGroup].Targets;
        
        if (et.PlayerHand) Managers.AN_MAN.ShiftPlayerHand(true); // TESTING

        if (effect is DrawEffect de2 && de2.IsDiscardEffect)
        {
            //Managers.AN_MAN.ShiftPlayerHand(true); // moved^
            if (et.VariableNumber)
            {
                if (et.AllowZero) Managers.U_MAN.SetConfirmEffectButton(true);
                else Managers.U_MAN.SetCancelEffectButton(true);

                if (de2.IsMulliganEffect) description = "Choose cards to redraw.";
                else if (et.TargetNumber < GameManager.MAX_HAND_SIZE)
                {
                    string card = "card";
                    if (et.TargetNumber > 1) card = "cards";
                    description = $"Discard up to {et.TargetNumber} {card}.";
                }
                else description = "Discard any number of cards.";
            }
            else
            {
                int value = et.TargetNumber;
                if (value > 1) description = $"Discard {value} cards.";
                else description = "Discard a card.";
            }
        }
        else
        {
            /*
            if (effect is ChangeCostEffect ||
                (effect is CopyCardEffect cpyCrd && !cpyCrd.PlayCopy))
                Managers.AN_MAN.ShiftPlayerHand(true);
            */

            if (!isAdditionalEffect && 
                (string.IsNullOrEmpty(triggerName) || triggerName == CardManager.TRIGGER_PLAY))
                Managers.U_MAN.SetCancelEffectButton(true);

            //Debug.LogWarning($"IsAdditinoalEffect >>> {isAdditionalEffect} >>> TriggerName >>> {triggerName}");

            if (dragArrow != null)
            {
                Destroy(dragArrow);
                Debug.LogError("DRAG ARROW ALREADY EXISTS!");
            }
            dragArrow = Instantiate(Managers.CA_MAN.DragArrowPrefab, Managers.U_MAN.CurrentWorldSpace.transform);

            GameObject startPoint;
            if (effectSource.TryGetComponent(out ItemIcon _)) startPoint = Managers.P_MAN.HeroObject;
            else startPoint = effectSource;
            dragArrow.GetComponent<DragArrow>().SourceCard = startPoint;
        }

        Managers.U_MAN.CreateInfoPopup(description, UIManager.InfoPopupType.Default);

        foreach (var card in Managers.P_MAN.HandZoneCards.Concat(Managers.P_MAN.PlayZoneCards))
            if (!legalTargets[currentEffectGroup].Contains(card))
            {
                Managers.U_MAN.SelectTarget(card, UIManager.SelectionType.Disabled);
            }

        foreach (var target in legalTargets[currentEffectGroup])
            Managers.U_MAN.SelectTarget(target, UIManager.SelectionType.Highlighted);
    }

    /******
     * *****
     * ****** CONFIRM_EFFECTS
     * *****
     *****/
    private void ConfirmNonTargetEffect()
    {
        // FOR_EACH_EFFECTS
        if (CurrentEffect.ForEachEffects.Count > 0)
        {
            foreach (var t in acceptedTargets[currentEffectGroup])
                foreach (var group in CurrentEffect.ForEachEffects)
                    additionalEffectGroups.Add(group);
        }
        StartNextEffect();
    }
    public void ConfirmTargetEffect()
    {
        Managers.U_MAN.PlayerIsTargetting = false;
        Managers.U_MAN.DismissInfoPopup();
        Managers.U_MAN.SetCancelEffectButton(false);

        foreach (var target in legalTargets[currentEffectGroup])
            Managers.U_MAN.SelectTarget(target, UIManager.SelectionType.Disabled);

        if (CurrentEffectGroup.Targets.PlayerHand) Managers.AN_MAN.ShiftPlayerHand(false); // TESTING

        // FOR_EACH_EFFECTS
        if (CurrentEffect.ForEachEffects.Count > 0)
        {
            foreach (var t in acceptedTargets[currentEffectGroup])
                foreach (var group in CurrentEffect.ForEachEffects)
                    additionalEffectGroups.Add(group);
        }
        if (CurrentEffect is DrawEffect de)
        {
            //if (de.IsDiscardEffect) Managers.AN_MAN.ShiftPlayerHand(false);
            if (de.IsMulliganEffect) Managers.CA_MAN.ShuffleDeck(Managers.P_MAN);
        }
        else
        {
            /*
            if (CurrentEffect is ChangeCostEffect or CopyCardEffect)
                Managers.AN_MAN.ShiftPlayerHand(false);
            */

            if (dragArrow != null)
            {
                Destroy(dragArrow);
                dragArrow = null;
            }
        }
        StartNextEffect();
    }

    /******
     * *****
     * ****** CANCEL_EFFECT_GROUP_LIST
     * *****
     *****/
    public void CancelEffectGroupList(bool isUserCancel)
    {
        if (effectSource == null)
        {
            Debug.LogError("EFFECT SOURCE IS NULL!");
            return;
        }

        var hMan = HeroManager.GetSourceHero(effectSource);

        if (effectSource.TryGetComponent(out ActionCardDisplay acd))
        {
            if (isUserCancel)
            {
                Managers.P_MAN.CurrentEnergy += acd.CurrentEnergyCost;
                Managers.CA_MAN.ChangeCardZone(effectSource, hMan.HandZone, CardManager.ZoneChangeType.ReturnToIndex);
                Managers.P_MAN.ActionZoneCards.Remove(effectSource);
                Managers.P_MAN.HandZoneCards.Add(effectSource);
            }
        }
        else if (effectSource.TryGetComponent(out UnitCardDisplay ucd))
        {
            if (isUserCancel)
            {
                Managers.P_MAN.CurrentEnergy += ucd.CurrentEnergyCost;
                Managers.CA_MAN.ChangeCardZone(effectSource, hMan.HandZone, CardManager.ZoneChangeType.ReturnToIndex);
                Managers.P_MAN.PlayZoneCards.Remove(effectSource);
                Managers.P_MAN.HandZoneCards.Add(effectSource);
            }
            else if (triggerName == CardManager.TRIGGER_PLAY)
            {
                ResolveChangeNextCostEffects(effectSource); // Resolves IMMEDIATELY

                Managers.CA_MAN.TriggerTrapAbilities(effectSource); // Resolves 3rd
                TriggerModifiers_PlayCard(effectSource); // Resolves 2nd
                TriggerGiveNextEffects(effectSource); // Resolves 1st
            }
        }
        else if (effectSource.CompareTag(Managers.P_MAN.HERO_POWER_TAG))
        {
            if (isUserCancel)
            {
                Managers.P_MAN.HeroPowerUsed = false;
                Managers.P_MAN.CurrentEnergy += Managers.P_MAN.HeroScript.CurrentHeroPower.PowerCost;
            }
        }
        else if (effectSource.CompareTag(Managers.P_MAN.HERO_ULTIMATE_TAG))
        {
            if (isUserCancel)
            {
                Managers.P_MAN.CurrentEnergy += Managers.P_MAN.GetUltimateCost(out _);
                Managers.P_MAN.HeroUltimateProgress = GameManager.HERO_ULTMATE_GOAL;
            }
        }
        else if (effectSource.CompareTag(Managers.EN_MAN.HERO_POWER_TAG))
        {
            // blank
        }
        else if (effectSource.CompareTag(Managers.P_MAN.HERO_TAG))
        {
            // blank
        }
        else if (effectSource.CompareTag(Managers.EN_MAN.HERO_TAG))
        {
            // blank
        }
        else if (effectSource.TryGetComponent(out ItemIcon _))
        {
            // blank
        }
        else
        {
            Debug.LogError($"INVALID SOURCE ---> <{effectSource}>\n" +
                $"INVALID TAG ---> <{effectSource.tag}>");
        }

        Managers.U_MAN.PlayerIsTargetting = false;
        if (isUserCancel)
        {
            Managers.U_MAN.DismissInfoPopup();
            if (CurrentEffectGroup.Targets.PlayerHand)
                Managers.AN_MAN.ShiftPlayerHand(false);
        }
        FinishEffectGroupList(true);
    }

    /******
     * *****
     * ****** FINISH_EFFECT_GROUP_LIST
     * *****
     *****/
    public void FinishEffectGroupList(bool wasCancelled)
    {
        if (!effectsResolving) // Combat End Behavior
        {
            Debug.LogWarning("[COMBAT END BEHAVIOR]\nCANNOT FINISH GROUP! >>> EFFECTS NOT RESOLVING!");
            additionalEffectGroups.Clear();
            FinishEffectCleanup();
            return;
        }

        Debug.Log("<<< GROUP LIST FINISHED! [" + (wasCancelled ? "CANCELLED" : "RESOLVED") + "] >>>");

        if (effectSource == null) // GiveNextEffects Behavior
        {
            Debug.Log("EFFECT SOURCE IS NULL!");
            DeselectTargets();
            FinishEffectCleanup();
            return;
        }

        if (!wasCancelled || isAdditionalEffect)
        {
            if (additionalEffectGroups.Count > 0)
            {
                var source = effectSource;
                var group = additionalEffectGroups[0];

                Managers.EV_MAN.NewDelayedAction(() =>
                StartEffectGroupList(new List<EffectGroup> { group },
                source), 0, true, true); // PRIORITY ACTION
            }
            else
            {
                if (CombatManager.IsActionCard(effectSource))
                {
                    Managers.U_MAN.CombatLog_PlayCard(effectSource);
                    ResolveChangeNextCostEffects(effectSource); // Resolves IMMEDIATELY
                    TriggerModifiers_PlayCard(effectSource); // Resolves 1st

                    var source = effectSource;
                    Managers.EV_MAN.NewDelayedAction(() => DiscardAction(source), 0.5f, false, true); // PRIORITY ACTION

                    void DiscardAction(GameObject source)
                    {
                        if (source == null)
                        {
                            Debug.LogError("ACTION ALREADY DISCARDED!");
                            return;
                        }
                        var cd = source.GetComponent<CardDisplay>();
                        var hMan = HeroManager.GetSourceHero(source);
                        if (cd.CardScript.CardRarity is Card.Rarity.Rare) { } // Don't count Created Card Ultimates
                        else
                        {
                            switch (cd.CardScript.CardSubType)
                            {
                                case CardManager.EXPLOIT:
                                    hMan.ExploitsPlayed++;
                                    break;
                                case CardManager.INVENTION:
                                    hMan.InventionsPlayed++;
                                    break;
                                case CardManager.SCHEME:
                                    hMan.SchemesPlayed++;
                                    break;
                                case CardManager.EXTRACTION:
                                    hMan.ExtractionsPlayed++;
                                    break;
                            }
                        }
                        Managers.CA_MAN.DiscardCard(source, true);
                        Managers.EV_MAN.NewDelayedAction(() =>
                        Managers.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_SPARK, hMan), 0, true);
                    }
                }
                // Enemies resolve these triggers in ManagerHandler.CA_MAN.PlayCard()
                else if (CombatManager.IsUnitCard(effectSource) && HeroManager.GetSourceHero(effectSource) == Managers.P_MAN)
                {
                    if (triggerName == CardManager.TRIGGER_PLAY)
                    {
                        Managers.U_MAN.CombatLog_PlayCard(effectSource);
                        ResolveChangeNextCostEffects(effectSource); // Resolves IMMEDIATELY
                        Managers.CA_MAN.TriggerTrapAbilities(effectSource); // Resolves 3rd
                        TriggerModifiers_PlayCard(effectSource); // Resolves 2nd
                        TriggerGiveNextEffects(effectSource); // Resolves 1st
                    }
                }
                else if (effectSource.TryGetComponent(out ItemIcon icon))
                {
                    Managers.U_MAN.CombatLogEntry($"You used {TextFilter.Clrz_ylw(icon.LoadedItem.ItemName)} (Item).");
                    //icon.IsUsed = true; // Deprecated w/ roguelike update, items are disposable
                    Managers.P_MAN.HeroItems.Remove(icon.LoadedItem); // TESTING
                    Managers.U_MAN.SetSkybar(true);
                }
                else if (effectSource.CompareTag(Managers.P_MAN.HERO_POWER_TAG))
                {
                    string powerName = Managers.P_MAN.HeroScript.CurrentHeroPower.PowerName;
                    Managers.U_MAN.CombatLogEntry($"You used {TextFilter.Clrz_ylw(powerName)} (Hero Power).");
                    Managers.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH, Managers.P_MAN);
                    Managers.P_MAN.HeroUltimateProgress++;

                    // TUTORIAL!
                    if (Managers.G_MAN.IsTutorial && Managers.P_MAN.EnergyPerTurn == 2)
                        Managers.G_MAN.Tutorial_Tooltip(5);
                }
                else if (effectSource.CompareTag(Managers.EN_MAN.HERO_POWER_TAG))
                {
                    string powerName = Managers.EN_MAN.HeroScript.CurrentHeroPower.PowerName;
                    Managers.U_MAN.CombatLogEntry($"Enemy used {TextFilter.Clrz_ylw(powerName)} (Hero Power).");
                    Managers.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH, Managers.EN_MAN);
                }
                else if (effectSource.CompareTag(Managers.P_MAN.HERO_ULTIMATE_TAG))
                {
                    string powerName = (Managers.P_MAN.HeroScript as PlayerHero).CurrentHeroUltimate.PowerName;
                    Managers.U_MAN.CombatLogEntry($"You used {TextFilter.Clrz_ylw(powerName)} (Hero Ultimate).");
                    Managers.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH, Managers.P_MAN);
                    Managers.P_MAN.HeroUltimateProgress = 0;
                }
            }
        }
        else additionalEffectGroups.Clear();

        // EFFECT CLEANUP
        StartEffectCleanup();

        void StartEffectCleanup()
        {
            DeselectTargets();
            FinishEffectCleanup();
            if (Managers.P_MAN.IsMyTurn) Managers.CA_MAN.SelectPlayableCards();
        }

        void DeselectTargets()
        {
            if (legalTargets != null)
            {
                foreach (var list in legalTargets)
                    foreach (var target in list)
                    {
                        if (target == null) continue;
                        Managers.U_MAN.SelectTarget(target, UIManager.SelectionType.Disabled);
                    }
            }

            if (acceptedTargets != null)
            {
                foreach (var list in acceptedTargets)
                    foreach (var target in list)
                    {
                        if (target == null) continue;
                        Managers.U_MAN.SelectTarget(target, UIManager.SelectionType.Disabled);
                    }
            }
        }

        void FinishEffectCleanup()
        {
            if (dragArrow != null)
            {
                Destroy(dragArrow);
                dragArrow = null;
            }

            currentEffect = 0;
            currentEffectGroup = 0;
            savedValue = 0;
            effectSource = null;
            effectGroupList.Clear();
            legalTargets.Clear();
            acceptedTargets.Clear();
            EffectsResolving = false;
        }
    }
    #endregion

    #region TARGET VALIDATION
    /******
     * *****
     * ****** CHECK_LEGAL_TARGETS
     * *****
     *****/
    public bool CheckLegalTargets(List<EffectGroup> groupList, GameObject source, bool isPreCheck = false)
    {
        effectGroupList = groupList.ToList(); // MUST create a new list, otherwise major bug that clears the actual scriptable object
        effectSource = source;

        legalTargets.Clear();
        acceptedTargets.Clear();
        acceptedTargets_Cards.Clear();

        for (int i = 0; i < effectGroupList.Count; i++)
        {
            legalTargets.Add(new List<GameObject>());
            acceptedTargets.Add(new List<GameObject>());
            acceptedTargets_Cards.Add(new List<Card>());
        }

        List<int> invalidGroups = new();
        int group = 0;

        foreach (var eg in effectGroupList)
        {
            if (eg == null)
            {
                Debug.LogError("GROUP IS NULL!");
                continue;
            }

            foreach (var effect in eg.Effects)
            {
                if (effect == null)
                {
                    Debug.LogError("EFFECT IS NULL!");
                    continue;
                }

                if (effect.IgnoreLegality) continue;

                if (!GetLegalTargets(group, effect, eg.Targets,
                    GetAdditionalTargets(eg), out bool requiredEffect, isPreCheck))
                {
                    invalidGroups.Add(group);
                    int groupsRemaining = effectGroupList.Count - invalidGroups.Count;

                    Debug.Log($"EFFECT GROUP: <{effectGroupList[group]}>");
                    Debug.Log($"INVALID GROUP: <{eg}>\n<{groupsRemaining}/{effectGroupList.Count}> REMAINING!");

                    if (groupsRemaining < 1 || requiredEffect) return false;
                    else break;
                }
            }
            group++;
        }

        if (isPreCheck) ClearTargets();
        else ClearInvalids();
        return true;

        int GetAdditionalTargets(EffectGroup eg)
        {
            int additionalTargets = 0;
            foreach (var group in effectGroupList)
            {
                if (group == null)
                {
                    Debug.LogError("GROUP IS NULL!");
                    continue;
                }
                if (group.Targets.CompareTargets(eg.Targets)) additionalTargets++;
            }
            if (additionalTargets > 0) additionalTargets--;
            return additionalTargets;
        }
        void ClearInvalids()
        {
            foreach (int i in invalidGroups.AsEnumerable().Reverse())
            {
                if (effectGroupList.Count > i) effectGroupList.RemoveAt(i);
                if (legalTargets.Count > i) legalTargets.RemoveAt(i);
                if (acceptedTargets.Count > i) acceptedTargets.RemoveAt(i);
                if (acceptedTargets_Cards.Count > i) acceptedTargets_Cards.RemoveAt(i);
            }
        }
        void ClearTargets()
        {
            effectSource = null;
            effectGroupList.Clear();
            legalTargets.Clear();
            acceptedTargets.Clear();
            acceptedTargets_Cards.Clear();
        }
    }

    /******
     * *****
     * ****** GET_LEGAL_TARGETS
     * *****
     *****/
    private bool GetLegalTargets(int currentGroup, Effect effect,
        EffectTargets targets, int additionalTargets, out bool requiredEffect, bool isPreCheck)
    {
        requiredEffect = effect.IsRequired;

        if (targets.PlayerDeck || targets.SavedTarget ||
            effect is DelayEffect or SaveValueEffect or ReplenishEffect or GiveNextUnitEffect or ModifyNextEffect ||
            (effect is ChangeCostEffect cce && cce.ChangeNextCost)) return true;

        var hMan_Source = HeroManager.GetSourceHero(effectSource, out HeroManager hMan_Enemy);

        if (hMan_Source == null)
        {
            Debug.LogError("SOURCE HERO IS NULL!");
        }

        /* Not always true
        if (targets.NoTargets && effect.PreCheckConditions)
        {
            Debug.LogError("EFFECTS WITH NO TARGETS CANNOT PRECHECK CONDITIONS!");
            return false;
        }
        */

        // DERIVED VALUES
        if (effect.IsDerivedValue)
        {
            if (effect.DerivedValue == Effect.DerivedValueType.Allies_Count &&
                hMan_Source.PlayZoneCards.Count < 1) return false;
        }

        if (!targets.NoTargets)
        {
            List<List<GameObject>> targetZones = new();

            if (targets.TargetsSelf) AddTarget(effectSource);
            if (targets.PlayerHand) targetZones.Add(hMan_Source.HandZoneCards);
            if (targets.PlayerUnit) targetZones.Add(hMan_Source.PlayZoneCards);
            if (targets.EnemyUnit) targetZones.Add(hMan_Enemy.PlayZoneCards);
            if (targets.PlayerHero) AddTarget(hMan_Source.HeroObject);
            if (targets.EnemyHero) AddTarget(hMan_Enemy.HeroObject);

            foreach (var zone in targetZones)
                foreach (var target in zone)
                    AddTarget(target);

            /*
            // PRECHECK CONDITIONS
            if (effect.PreCheckConditions)
            {
                // CHECK CONDITIONS INDEPENDENT
                if (!(effect.CheckConditionsIndependent && !isAdditionalEffect))
                    legalTargets[currentGroup] = GetValidTargets(effect, legalTargets[currentGroup]);
            }
            */
        }

        // PRECHECK CONDITIONS
        if (effect.PreCheckConditions)
        {
            // CHECK CONDITIONS INDEPENDENT
            if (!(effect.CheckConditionsIndependent && !isAdditionalEffect))
            {
                if (targets.NoTargets) // TESTING
                {
                    var validTargets = GetValidTargets(effect, new List<GameObject> { effectSource });
                    if (validTargets.Count < 1) return false;
                }

                legalTargets[currentGroup] = GetValidTargets(effect, legalTargets[currentGroup]);
            }
        }

        int handCount = hMan_Source.HandZoneCards.Count;
        int discardCount = hMan_Source.DiscardZoneCards.Count;
        int deckCount = hMan_Source.CurrentDeck.Count;

        if (effect is DrawEffect de)
        {
            // DRAW EFFECTS
            if (!de.IsDiscardEffect)
            {
                int cardsLeft = deckCount + discardCount;

                if (cardsLeft < effect.Value)
                {
                    Debug.LogWarning("NO CARDS LEFT!");
                    return false;
                }

                int cardsAfterDraw = handCount + effect.Value;

                if (isPreCheck)
                {
                    // If this is a pre-check for actions, account for the card in HAND
                    if (CombatManager.IsActionCard(effectSource)) cardsAfterDraw--;
                }
                if (cardsAfterDraw > GameManager.MAX_HAND_SIZE) return false;

                if (targets.NoTargets) return true;
            }

            // DISCARD EFFECTS
            else
            {
                if (targets.VariableNumber && targets.AllowZero) return true;

                if (isPreCheck)
                {
                    // If this is a pre-check for actions, account for the card in HAND
                    if (CombatManager.IsActionCard(effectSource)) handCount--;
                }

                if (handCount < 1) return false;
                if (targets.VariableNumber || targets.TargetsAll) return true;

                if (handCount < effect.Value)
                {
                    Debug.Log("NOT ENOUGH CARDS!");
                    if (requiredEffect) return false;
                }
                return true;
            }
        }
        else if (effect is CreateCardEffect or ReturnCardEffect ||
            (effect is CopyCardEffect cpyCrd && !cpyCrd.PlayCopy))
        {
            int cardsAfterDraw = handCount + 1;

            if (isPreCheck)
            {
                // If this is a pre-check for actions, account for the card in HAND
                if (CombatManager.IsActionCard(effectSource)) cardsAfterDraw--;
            }
            if (cardsAfterDraw > GameManager.MAX_HAND_SIZE)
            {
                Debug.Log("HAND IS FULL!");
                if (effect is ReturnCardEffect) return false;
                if (requiredEffect) return false; // Unless required, create as many as possible
            }
            return true;
        }
        else if (effect is PlayCardEffect || (effect is CopyCardEffect cpyCrd2 && cpyCrd2.PlayCopy))
        {
            var pce = effect is PlayCardEffect ? effect as PlayCardEffect : null;
            int unitCount = pce != null && pce.EnemyCard ? hMan_Enemy.PlayZoneCards.Count : hMan_Source.PlayZoneCards.Count;

            if (unitCount >= GameManager.MAX_UNITS_PLAYED) return false;
            return true;
        }
        else if (effect is ChangeControlEffect)
        {
            if (triggerName == CardManager.TRIGGER_TURN_END) return true; // Always resolve control-returning effects (excess units are destroyed)
            if (hMan_Source.PlayZoneCards.Count >= GameManager.MAX_UNITS_PLAYED) return false;
        }

        if (legalTargets[currentGroup].Count < 1) return false;
        if (requiredEffect && legalTargets[currentGroup].Count <
            effectGroupList[currentGroup].Targets.TargetNumber + additionalTargets)
            return false;
        return true;

        void AddTarget(GameObject target)
        {
            if (target == null)
            {
                Debug.LogError("TARGET IS NULL!");
                return;
            }

            bool isUnit = CombatManager.IsUnitCard(target);

            // Ignore actions for effects targetting units in hand
            if (!isUnit && effect is StatChangeEffect or GiveAbilityEffect) return;

            // Ignore enemies with Ward
            if (hMan_Source != HeroManager.GetSourceHero(target) &&
                hMan_Enemy.PlayZoneCards.Contains(target) &&
                CardManager.GetAbility(target, CardManager.ABILITY_WARD)) return;

            // Check to see if the source should be able to target itself
            if (target == effectSource &&
                !(targets.TargetsSelf || targets.PlayerHero || targets.EnemyHero ||
                targets.TargetsWeakest || targets.TargetsStrongest || targets.TargetsLowestHealth)) return;

            // ENEMY BEHAVIOR
            if (hMan_Source == Managers.EN_MAN && isUnit)
            {
                foreach (var ability in CombatManager.GetUnitDisplay(target).CurrentAbilities)
                    if (ability is TrapAbility) return;
            }
            if (effect is ChangeCostEffect cce)
            {
                if (!isUnit && !cce.ChangeActionCost) return;
                if (isUnit && !cce.ChangeUnitCost) return;
            }
            else if (effect is CopyCardEffect cpyCrd)
            {
                if (!isUnit && !cpyCrd.CopyAction) return;
                if (isUnit && !cpyCrd.CopyUnit) return;
            }
            else if (effect is GiveAbilityEffect gae && gae.Type ==
                GiveAbilityEffect.GiveAbilityType.RandomPositiveKeyword)
            {
                bool isValid = false;
                foreach (var posKey in Managers.CA_MAN.GeneratableKeywords)
                {
                    if (!CardManager.GetAbility(target, posKey.AbilityName))
                    {
                        isValid = true;
                        break;
                    }
                }
                if (!isValid) return;
            }

            if (isUnit)
            {
                if (CombatManager.GetUnitDisplay(target).CurrentHealth < 1) return;
                if (unitsToDestroy.Contains(target)) return;
            }

            var targetList = legalTargets[currentGroup];
            if (!targetList.Contains(target)) targetList.Add(target);
        }
    }

    /******
     * *****
     * ****** GET_VALID_TARGETS
     * *****
     *****/
    private List<GameObject> GetValidTargets(Effect effect, List<GameObject> allTargets)
    {
        HeroManager hMan_Enemy = null;
        List<GameObject> validTargets = new();
        List<GameObject> invalidTargets = new();

        var hMan_Source = effectSource != null ? HeroManager.GetSourceHero(effectSource, out hMan_Enemy) : null;

        foreach (var target in allTargets)
        {
            if (target == null)
            {
                Debug.LogWarning("EMPTY TARGET!");
                invalidTargets.Add(target);
            }
        }

        switch (effect.EffectConditionType)
        {
            case Effect.ConditionType.NONE:
                break;
            case Effect.ConditionType.PlayerWounded:
                if (!hMan_Source.IsWounded()) InvalidateAllTargets();
                break;
            case Effect.ConditionType.PlayerWounded_Not:
                if (hMan_Source.IsWounded()) InvalidateAllTargets();
                break;
            case Effect.ConditionType.EnemyWounded:
                if (!hMan_Enemy.IsWounded()) InvalidateAllTargets();
                break;
            case Effect.ConditionType.EnemyWounded_Not:
                if (hMan_Enemy.IsWounded()) InvalidateAllTargets();
                break;
            case Effect.ConditionType.Exhausted:
                ValidateCondition((GameObject unit) => CombatManager.GetUnitDisplay(unit).IsExhausted);
                break;
            case Effect.ConditionType.Exhausted_Not:
                ValidateCondition((GameObject unit) => CombatManager.GetUnitDisplay(unit).IsExhausted, true);
                break;
            case Effect.ConditionType.Damaged:
                ValidateCondition(CombatManager.IsDamaged);
                break;
            case Effect.ConditionType.Damaged_Not:
                ValidateCondition(CombatManager.IsDamaged, true);
                break;
            case Effect.ConditionType.HasGreaterPower:
                ValidateCondition((GameObject unit) => CombatManager.GetUnitDisplay(unit).CurrentPower > effect.EffectCondition_Value);
                break;
            case Effect.ConditionType.HasLessPower:
                ValidateCondition((GameObject unit) => CombatManager.GetUnitDisplay(unit).CurrentPower < effect.EffectCondition_Value);
                break;
            case Effect.ConditionType.HasAbility:
                ValidateCondition((GameObject unit) => CardManager.GetAbility(unit, effect.IfHasAbility_Value.AbilityName));
                break;
            case Effect.ConditionType.HasAbility_Not:
                ValidateCondition((GameObject unit) => CardManager.GetAbility(unit, effect.IfHasAbility_Value.AbilityName), true);
                break;
            case Effect.ConditionType.HasTrigger:
                ValidateCondition((GameObject unit) => CardManager.GetTrigger(unit, effect.IfHasTrigger_Value.AbilityName));
                break;
            case Effect.ConditionType.HasTrigger_Not:
                ValidateCondition((GameObject unit) => CardManager.GetTrigger(unit, effect.IfHasTrigger_Value.AbilityName), true);
                break;
            case Effect.ConditionType.HasMoreKeywords:
                ValidateCondition((GameObject unit) => GetUnitKeywords(unit) > effect.EffectCondition_Value);
                break;
            case Effect.ConditionType.HasLessKeywords:
                ValidateCondition((GameObject unit) => GetUnitKeywords(unit) < effect.EffectCondition_Value);
                break;
            case Effect.ConditionType.AlliesDestroyed_ThisTurn:
                if (effect.EffectCondition_Value < 1) Debug.LogWarning("EFFECT CONDITION VALUE < 1!");
                if (hMan_Source.AlliesDestroyed_ThisTurn < effect.EffectCondition_Value) InvalidateAllTargets();
                break;
            case Effect.ConditionType.EnemiesDestroyed_ThisTurn:
                if (effect.EffectCondition_Value < 1) Debug.LogWarning("EFFECT CONDITION VALUE < 1!");
                if (hMan_Enemy.AlliesDestroyed_ThisTurn < effect.EffectCondition_Value) InvalidateAllTargets();
                break;
            case Effect.ConditionType.CostsLess:
                ValidateCondition((GameObject card) => card.GetComponent<CardDisplay>().CurrentEnergyCost < effect.EffectCondition_Value);
                break;
            case Effect.ConditionType.CostsMore:
                ValidateCondition((GameObject card) => card.GetComponent<CardDisplay>().CurrentEnergyCost > effect.EffectCondition_Value);
                break;
            case Effect.ConditionType.HasMoreCards_Player:
                if (hMan_Source.HandZoneCards.Count <= effect.EffectCondition_Value) InvalidateAllTargets();
                break;
            case Effect.ConditionType.HasLessCards_Player:
                if (hMan_Source.HandZoneCards.Count >= effect.EffectCondition_Value) InvalidateAllTargets();
                break;
            default:
                Debug.LogError("INVALID CONDITION TYPE!");
                break;
        }

        void InvalidateAllTargets()
        {
            foreach (var t in allTargets)
                invalidTargets.Add(t);
        }

        void ValidateCondition(Func<GameObject, bool> validator, bool reverse = false)
        {
            foreach (var t in allTargets)
            {
                bool isValid = reverse ? !validator(t) : validator(t);
                if (!isValid) invalidTargets.Add(t);
            }
        }

        int GetUnitKeywords(GameObject unit)
        {
            int keywords = 0;
            foreach (var keyword in CardManager.PositiveAbilities)
                if (CardManager.GetAbility(unit, keyword)) keywords++;
            return keywords;
        }

        foreach (var t in allTargets)
            if (!invalidTargets.Contains(t) && !validTargets.Contains(t))
                validTargets.Add(t);

        return validTargets;
    }
    #endregion

    #region EFFECT RESOLUTION
    /******
     * *****
     * ****** RESOLVE_EFFECT_GROUP_LIST
     * *****
     *****/
    private void ResolveEffectGroupList()
    {
        //Debug.Log("RESOLVE EFFECT GROUP LIST!");

        currentEffectGroup = 0;
        currentEffect = 0; // Unnecessary

        float delay = 0;
        if (!string.IsNullOrEmpty(triggerName) && triggerName != CardManager.TRIGGER_PLAY) delay = 0.25f;

        foreach (var eg in effectGroupList)
        {
            if (eg.Targets.PlayerDeck)
            {
                ResolveEffectGroup(eg, null, acceptedTargets_Cards[currentEffectGroup], delay, out float newDelay);
                delay = newDelay;
            }
            else
            {
                ResolveEffectGroup(eg, acceptedTargets[currentEffectGroup], null, delay, out float newDelay);
                delay = newDelay;
            }

            currentEffectGroup++;
        }
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT_GROUP
     * *****
     *****/
    private void ResolveEffectGroup(EffectGroup eg, List<GameObject> targets, List<Card> targets_Cards, float delay, out float newDelay)
    {
        Debug.Log($"RESOLVE EFFECT GROUP <{eg}> ");

        newDelay = delay;
        if (eg == null)
        {
            Debug.LogError("EFFECT GROUP IS NULL!");
            return;
        }

        List<GameObject> targetList = null;
        List<Card> targetList_Cards = null;

        if (targets != null)
        {
            targetList = new();
            foreach (var t in targets) targetList.Add(t);
        }
        if (targets_Cards != null)
        {
            targetList_Cards = new();
            foreach (var t in targets_Cards) targetList_Cards.Add(t);
        }

        foreach (var effect in eg.Effects)
        {
            if (effect is DelayEffect de)
            {
                newDelay += de.DelayValue;
                continue;
            }

            bool shootRay = false;
            if (eg.Targets.TargetsHighestCost || eg.Targets.TargetsLowestCost) shootRay = true;
            else if (!(eg.Targets.TargetsSelf && !eg.Targets.TargetsAll))
            {
                if (effect is DamageEffect or DestroyEffect or HealEffect
                    || effect.ShootRay) shootRay = true;
            }

            if (targetList_Cards != null)
            {
                ActiveEffects++;
                FunctionTimer.Create(() =>
                ResolveEffect_Cards(targetList_Cards, effect, 0, out _), newDelay);
            }
            else
            {
                if (targetList != null && targetList.Count > 0 &&
                    Managers.EN_MAN.HandZoneCards.Contains(targetList[0])) shootRay = false;

                if (shootRay) ResolveEffect(targetList, effect, shootRay, newDelay, out newDelay);
                else
                {
                    ActiveEffects++;
                    FunctionTimer.Create(() =>
                    ResolveEffect(targetList, effect, shootRay, 0, out _), newDelay);
                }
            }

            if (effect is DrawEffect or GiveNextUnitEffect or ModifyNextEffect ||
                (effect is ChangeCostEffect cce && cce.ChangeNextCost)) { }
            else newDelay += 0.5f;
        }
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT
     * *****
     *****/
    private void SaveEffectValue(SaveValueEffect.SavedValueType valueType, GameObject target)
    {
        if (!target.TryGetComponent(out UnitCardDisplay ucd))
        {
            Debug.LogError("TARGET IS NOT UNIT CARD!");
            return;
        }

        switch (valueType)
        {
            case SaveValueEffect.SavedValueType.Target_Power:
                savedValue = ucd.CurrentPower;
                break;
            case SaveValueEffect.SavedValueType.Target_Health:
                savedValue = ucd.CurrentHealth;
                break;
        }
    }

    private void ResolveEffect_Cards(List<Card> allTargets, Effect effect, float delay, out float newDelay)
    {
        Debug.Log($"RESOLVE EFFECT <{effect}>");
        newDelay = delay;

        if (effect is StatChangeEffect sce)
        {
            foreach (var target in allTargets)
            {
                if (target is not UnitCard uc) continue;

                var newSce = ScriptableObject.CreateInstance<StatChangeEffect>();
                newSce.LoadEffect(sce);

                if (newSce.DoublePower) newSce.PowerChange = uc.CurrentPower;
                if (newSce.DoubleHealth) newSce.HealthChange = uc.CurrentHealth;

                uc.CurrentPower += newSce.PowerChange;
                uc.MaxHealth += newSce.HealthChange;
                uc.CurrentHealth += newSce.HealthChange;

                uc.CurrentEffects.Add(newSce);
            }
        }
        else
        {
            Debug.LogError("INVALID EFFECT!");
            return;
        }

        FunctionTimer.Create(() => ActiveEffects--, newDelay);
    }

    public void ResolveEffect(List<GameObject> allTargets, Effect effect, bool shootRay,
        float delay, out float newDelay, bool isEffectGroup = true, GameObject newEffectSource = null)
    {
        Debug.Log($"RESOLVE EFFECT <{effect}>");
        newDelay = delay;

        // NEW EFFECT SOURCE
        if (newEffectSource == null)
        {
            if (effectSource == null) Debug.LogWarning("SOURCE IS NULL!");
            newEffectSource = effectSource;
        }

        /******
        * *****
        * ****** VALID TARGETS (EFFECT CONDITIONS)
        * *****
        *****/

        List<GameObject> validTargets;
        bool isValidEffect = true;

        /*
        if ((effect is DrawEffect dre && !dre.IsDiscardEffect) || effect is CreateCardEffect or PlayCardEffect)
        {
            validTargets = GetValidTargets(effect, allTargets.Count < 1 ? new List<GameObject> { effectSource } : allTargets);
            if (validTargets.Count < 1) isValidEffect = false;
        }
        */

        if (allTargets.Count < 1) // TESTING
        {
            validTargets = GetValidTargets(effect, new List<GameObject> { effectSource });
            if (validTargets.Count < 1) isValidEffect = false;
        }

        validTargets = GetValidTargets(effect, allTargets);

        /******
        * *****
        * ****** CONDITIONAL EFFECTS
        * *****
        *****/
        // If Has Ability
        if (effect.IfHasAbility != null) ApplyConditionalEffects((GameObject unit) =>
        CardManager.GetAbility(unit, effect.IfHasAbility.AbilityName), effect.IfHasAbilityEffects);

        // If Has Trigger
        if (effect.IfHasTrigger != null) ApplyConditionalEffects((GameObject unit) =>
        CardManager.GetTrigger(unit, effect.IfHasTrigger.AbilityName), effect.IfHasTriggerEffects);

        // If Has Greater Power
        if (effect.IfHasGreaterPowerEffects != null && effect.IfHasGreaterPowerEffects.Count > 0)
            ApplyConditionalEffects((GameObject unit) =>
            CombatManager.GetUnitDisplay(unit).CurrentPower > effect.IfHasGreaterPowerValue, effect.IfHasGreaterPowerEffects);

        // If Has Lower Power
        if (effect.IfHasLowerPowerEffects != null && effect.IfHasLowerPowerEffects.Count > 0)
            ApplyConditionalEffects((GameObject unit) =>
            CombatManager.GetUnitDisplay(unit).CurrentPower < effect.IfHasLowerPowerValue, effect.IfHasLowerPowerEffects);

        void ApplyConditionalEffects(Func<GameObject, bool> condition, List<EffectGroup> effects)
        {
            foreach (var t in validTargets)
                if (condition(t))
                    foreach (var eg in effects)
                        additionalEffectGroups.Add(eg);
        }

        /******
        * *****
        * ****** RESOLUTION
        * *****
        *****/
        // If the source is an item, shoot the ray from the PLAYER HERO
        GameObject raySource = null;
        if (newEffectSource != null)
        {
            raySource = newEffectSource;
            if (newEffectSource.TryGetComponent<ItemIcon>(out _))
                raySource = Managers.P_MAN.HeroObject;
        }
        else raySource = Managers.P_MAN.HeroObject;

        // SAVE VALUE
        if (effect is SaveValueEffect sve)
        {
            if (EffectRayError()) return;
            if (validTargets.Count > 0) SaveEffectValue(sve.ValueType, validTargets[0]);
            else Debug.LogError("NO VALUE SAVED!");
        }
        // SAVE TARGET
        else if (effect is SaveTargetEffect ste)
        {
            if (EffectRayError()) return;
            if (validTargets.Count > 0) savedTarget = validTargets[0];
            else Debug.LogError("NO TARGET SAVED!");
        }
        // DRAW
        else if (effect is DrawEffect de)
        {
            if (EffectRayError()) return;

            if (de.IsDiscardEffect)
                foreach (var target in validTargets)
                    Managers.CA_MAN.DiscardCard(target);

            else if (isValidEffect)
            {
                var hMan = HeroManager.GetSourceHero(newEffectSource);
                var target = validTargets.Count > 0 ? validTargets[0] : null;

                DeriveEffectValues(target, effect);
                var addEffects = de.AdditionalEffects;
                for (int i = 0; i < effect.Value; i++)
                {
                    newDelay += 0.5f;
                    delay = newDelay; // Unnecessary?

                    FunctionTimer.Create(() => NewActiveEffect(target,
                        () => Managers.CA_MAN.DrawCard(hMan, null, addEffects), shootRay), newDelay);
                }
            }
        }
        // DAMAGE
        else if (effect is DamageEffect dmgE)
        {
            if (validTargets.Count > 0)
            {
                string sfx = shootRay ? "SFX_DamageRay_Start" : "SFX_DamageRay_End";
                FunctionTimer.Create(() =>
                Managers.AU_MAN.StartStopSound(sfx), delay);
            }

            foreach (var target in validTargets)
                NewActiveEffect(target, () =>
                DealDamage(target), shootRay);

            void DealDamage(GameObject target)
            {
                bool sourceIsUnit = false;
                bool targetIsUnit = false;

                UnitCardDisplay sourceUcd = null;
                UnitCardDisplay targetUcd = null;

                if (newEffectSource != null)
                {
                    sourceIsUnit = CombatManager.IsUnitCard(newEffectSource);
                    if (sourceIsUnit) sourceUcd = newEffectSource.GetComponent<UnitCardDisplay>();
                    targetIsUnit = target.TryGetComponent(out targetUcd);
                }

                Managers.CO_MAN.TakeDamage(target, effect.Value,
                    out bool targetDamaged, out bool targetDestroyed, false);

                if (targetDestroyed)
                {
                    if (dmgE.IfDestroyedEffects != null)
                        foreach (EffectGroup efg in dmgE.IfDestroyedEffects)
                            additionalEffectGroups.Add(efg);

                    if (sourceIsUnit && targetIsUnit)
                    {
                        if (sourceUcd.CurrentHealth > 0 && !UnitsToDestroy.Contains(newEffectSource) &&
                            CardManager.GetTrigger(newEffectSource, CardManager.TRIGGER_DEATHBLOW))
                        {
                            var source = newEffectSource;
                            Managers.EV_MAN.NewDelayedAction(() =>
                            Managers.CA_MAN.TriggerUnitAbility(source,
                            CardManager.TRIGGER_DEATHBLOW), 0.5f, true);
                        }
                    }
                }
                else if (targetDamaged)
                {
                    if (sourceIsUnit && targetIsUnit)
                    {
                        if (CardManager.GetAbility(newEffectSource, CardManager.ABILITY_POISONOUS))
                            targetUcd.AddCurrentAbility(poisonAbility);
                    }
                }
            }
        }
        // DESTROY
        else if (effect is DestroyEffect)
        {
            if (validTargets.Count > 0)
            {
                string sfx = shootRay ? "SFX_DamageRay_Start" : "SFX_DamageRay_End";
                FunctionTimer.Create(() =>
                Managers.AU_MAN.StartStopSound(sfx), delay);
            }

            foreach (var target in validTargets)
                NewActiveEffect(target, () =>
                DestroyUnit(target), shootRay);

            void DestroyUnit(GameObject target)
            {
                if (target == null)
                {
                    Debug.LogError("TARGET IS NULL!");
                    return;
                }

                if (!target.TryGetComponent(out UnitCardDisplay ucd))
                {
                    Debug.LogError("TARGET DISPLAY IS NULL!");
                    return;
                }

                Managers.AN_MAN.ShakeCamera(AnimationManager.Bump_Light);
                int previousHealth = ucd.CurrentHealth;
                ucd.CurrentHealth = 0;
                Managers.AN_MAN.UnitTakeDamageState(target, previousHealth, false);
                Managers.CO_MAN.DestroyUnit(target);
            }
        }
        // HEALING
        else if (effect is HealEffect)
        {
            foreach (var target in validTargets)
            {
                if (CombatManager.IsUnitCard(target) && CombatManager.IsDamaged(target))
                {
                    TriggerModifiers_SpecialTrigger(ModifierAbility.TriggerType.DamagedAllyHealed,
                        HeroManager.GetSourceHero(target).PlayZoneCards, target);
                }

                NewActiveEffect(target, () =>
                Managers.CO_MAN.HealDamage(target, effect as HealEffect), shootRay);
            }
        }
        // EXHAUST/REFRESH
        else if (effect is ExhaustEffect ee)
        {
            Managers.AU_MAN.StartStopSound("SFX_Refresh");
            foreach (var target in validTargets)
            {
                if (!ee.SetExhausted && CombatManager.GetUnitDisplay(target).IsExhausted)
                {
                    TriggerModifiers_SpecialTrigger(ModifierAbility.TriggerType.AllyRefreshed,
                        HeroManager.GetSourceHero(target).PlayZoneCards, target);
                }

                NewActiveEffect(target, () => SetExhaustion(target), shootRay);
            }

            void SetExhaustion(GameObject target)
            {
                var ucd = CombatManager.GetUnitDisplay(target);
                if (ucd.IsExhausted != ee.SetExhausted)
                    ucd.IsExhausted = ee.SetExhausted;
            }
        }
        // REPLENISH
        else if (effect is ReplenishEffect)
        {
            if (EffectRayError()) return;

            if (isValidEffect)
            {
                var hMan = HeroManager.GetSourceHero(newEffectSource);
                int startEnergy = hMan.CurrentEnergy;
                int energyPerTurn = hMan.EnergyPerTurn;

                int newEnergy = startEnergy + effect.Value;
                if (newEnergy > energyPerTurn) newEnergy = energyPerTurn;
                if (newEnergy < startEnergy) newEnergy = startEnergy;

                if (newEnergy > startEnergy)
                {
                    if (hMan == Managers.P_MAN) Managers.P_MAN.CurrentEnergy = newEnergy;
                    else Managers.EN_MAN.CurrentEnergy = newEnergy;
                }

                int energyChange = newEnergy - startEnergy;
                Managers.AN_MAN.ModifyHeroEnergyState(energyChange, hMan.HeroObject);
            }
        }
        // GIVE_NEXT_UNIT
        else if (effect is GiveNextUnitEffect gnfe)
        {
            if (EffectRayError()) return;

            // NEW EFFECT INSTANCE
            var newGnfe = ScriptableObject.CreateInstance<GiveNextUnitEffect>();
            newGnfe.LoadEffect(gnfe);
            HeroManager.GetSourceHero(newEffectSource).GiveNextEffects.Add(newGnfe);
        }
        // MODIFY_NEXT
        else if (effect is ModifyNextEffect mne)
        {
            if (EffectRayError()) return;

            // NEW EFFECT INSTANCE
            var newMne = ScriptableObject.CreateInstance<ModifyNextEffect>();
            newMne.LoadEffect(mne);
            HeroManager.GetSourceHero(newEffectSource).ModifyNextEffects.Add(newMne);
        }
        // STAT_CHANGE/GIVE_ABILITY
        else if (effect is StatChangeEffect)
        {
            foreach (var target in validTargets)
            {
                if (!CombatManager.IsUnitCard(target)) continue;
                NewActiveEffect(target, () =>
                AddEffect(target, effect), shootRay);
            }
        }
        // GIVE_ABILITY
        else if (effect is GiveAbilityEffect)
        {
            foreach (var target in validTargets)
            {
                if (!CombatManager.IsUnitCard(target)) continue;
                NewActiveEffect(target, () =>
                AddEffect(target, effect), shootRay);
            }
        }
        // REMOVE_ABILITY
        else if (effect is RemoveAbilityEffect rae)
        {
            foreach (var target in validTargets)
            {
                if (!CombatManager.IsUnitCard(target)) continue;
                NewActiveEffect(target, () =>
                RemoveAbilities(target), shootRay);
            }

            void RemoveAbilities(GameObject target)
            {
                if (!target.TryGetComponent(out UnitCardDisplay ucd))
                {
                    Debug.LogError("TARGET IS NOT UNIT CARD!");
                    return;
                }

                if (rae.RemoveAbility != null) ucd.RemoveCurrentAbility(rae.RemoveAbility.AbilityName, false);
                else if (rae.RemoveAllAbilities || rae.RemoveAllButNegativeAbilities)
                {
                    List<string> abilitiesToRemove = new();
                    foreach (var ca in ucd.CurrentAbilities)
                    {
                        if (rae.RemoveAllButNegativeAbilities)
                        {
                            foreach (string negativeAbility in CardManager.NegativeAbilities)
                                if (ca.AbilityName == negativeAbility) goto NextAbility;
                        }

                        // Don't remove ChangeControl abilities
                        if (ca is TriggeredAbility tra)
                        {
                            foreach (var eg in tra.EffectGroupList)
                                foreach (var e in eg.Effects)
                                    if (e is ChangeControlEffect)
                                        goto NextAbility;
                        }

                        abilitiesToRemove.Add(ca.AbilityName);
                    NextAbility:;
                    }

                    foreach (string ability in abilitiesToRemove)
                        ucd.RemoveCurrentAbility(ability, false);

                    ucd.CardScript.CurrentEffects.Clear();
                }
                else
                {
                    if (rae.RemovePositiveAbilities)
                    {
                        foreach (string positiveAbility in CardManager.PositiveAbilities)
                            ucd.RemoveCurrentAbility(positiveAbility, false);
                    }

                    if (rae.RemoveNegativeAbilities)
                    {
                        foreach (string negativeAbility in CardManager.NegativeAbilities)
                        {
                            if (negativeAbility == CardManager.ABILITY_WOUNDED) continue;
                            ucd.RemoveCurrentAbility(negativeAbility, false);
                        }
                    }
                }
            }
        }
        // CREATE_CARD
        else if (effect is CreateCardEffect cce)
        {
            if (EffectRayError()) return;

            if (isValidEffect)
            {
                Card cardScript = null;
                if (cce.CreatedCard != null) cardScript = cce.CreatedCard;
                else if (cce.RandomCard || !string.IsNullOrEmpty(cce.CreatedCardType))
                {
                    List<Card> cardPool = new();
                    string cardType = cce.CreatedCardType;

                    if (cce.RandomCard)
                    {
                        if (!cce.RestrictType || cce.IncludeUnits)
                        {
                            Card[] unitCards;
                            unitCards = Resources.LoadAll<Card>("Cards_Units");
                            foreach (var c in unitCards) cardPool.Add(c);
                        }

                        if (!cce.RestrictType || cce.IncludeActions)
                        {
                            Card[] actionCards;
                            actionCards = Resources.LoadAll<Card>("Cards_Actions");
                            foreach (var c in actionCards) cardPool.Add(c);
                        }
                    }
                    else
                    {
                        cardPool.AddRange(Managers.CA_MAN.GetCreatedCards(cardType, false)); // TESTING

                        if (cardPool.Count < 1)
                        {
                            Debug.LogError("CARDS NOT FOUND!");
                            return;
                        }
                    }

                    // Created Card Parameters
                    List<Card> invalidCards = new();

                    if (cce.RestrictCost)
                    {
                        foreach (var c in cardPool)
                            if (c.StartEnergyCost < cce.MinCost ||
                                c.StartEnergyCost > cce.MaxCost) invalidCards.Add(c);
                    }

                    if (cce.RestrictType)
                    {
                        foreach (var c in cardPool)
                            if ((!cce.IncludeUnits && c is UnitCard) ||
                                (!cce.IncludeActions && c is ActionCard)) invalidCards.Add(c);
                    }

                    if (cce.RestrictSubtype)
                    {
                        foreach (var c in cardPool)
                            if (c.CardType != cce.CardSubtype) invalidCards.Add(c);
                    }

                    if (cce.ExcludeSelf)
                    {
                        if (!newEffectSource.TryGetComponent(out CardDisplay cardDisplay))
                        {
                            Debug.LogWarning("SOURCE IS NOT A CARD!");
                        }
                        else
                        {
                            int selfIndex = cardPool.FindIndex(x => x.CardName == cardDisplay.CardName);
                            if (selfIndex != -1) cardPool.RemoveAt(selfIndex);
                        }
                    }

                    foreach (var c in invalidCards) cardPool.Remove(c);
                    cardPool.Shuffle(); // Extra randomization
                    cardScript = cardPool[UnityEngine.Random.Range(0, cardPool.Count)];
                }
                else
                {
                    Debug.LogError("INVALID TYPE!");
                    return;
                }
                cardScript = Managers.CA_MAN.NewCardInstance(cardScript);
                var newCardObj = DrawCreatedCard(cardScript, cce.AdditionalEffects);
            }
        }
        // PLAY_CARD
        else if (effect is PlayCardEffect pce)
        {
            if (EffectRayError()) return;

            if (isValidEffect)
            {
                Card cardScript = null;
                if (pce.PlayedCard != null) cardScript = pce.PlayedCard;
                else if (!string.IsNullOrEmpty(pce.PlayedCardType))
                {
                    List<Card> cardPool = new();
                    var createdCards = Resources.LoadAll<Card>("Cards_Created");
                    foreach (var c in createdCards)
                        if (c.CardSubType == pce.PlayedCardType) cardPool.Add(c);
                    if (cardPool.Count < 1)
                    {
                        Debug.LogError("CARDS NOT FOUND!");
                        return;
                    }

                    cardPool.Shuffle(); // Extra randomization
                    cardScript = cardPool[UnityEngine.Random.Range(0, cardPool.Count)];
                }
                else
                {
                    Debug.LogError("INVALID TYPE!");
                    return;
                }

                if (cardScript is not UnitCard) Debug.LogError("SCRIPT IS NOT UNIT CARD!");

                cardScript = Managers.CA_MAN.NewCardInstance(cardScript);
                var newCardObj = PlayCreatedUnit(cardScript as UnitCard, pce.EnemyCard, pce.AdditionalEffects);
            }
        }
        // COPY_CARD
        else if (effect is CopyCardEffect cpyCrd)
        {
            if (EffectRayError()) return;

            foreach (var target in validTargets)
            {
                var cardDisplay = target.GetComponent<CardDisplay>();
                var card = cardDisplay.CardScript;
                var newCard = Managers.CA_MAN.NewCardInstance(card, cpyCrd.IsExactCopy);
                GameObject newCardObj = null;

                if (!cpyCrd.CopyUnit && newCard is UnitCard) Debug.LogError("SCRIPT IS NOT UNIT CARD!");
                else if (!cpyCrd.CopyAction && newCard is ActionCard) Debug.LogError("SCRIPT IS NOT ACTION CARD!");

                if (cpyCrd.PlayCopy)
                {
                    if (newCard is ActionCard) Debug.LogError("CANNOT PLAY COPIED ACTIONS!");
                    else newCardObj = PlayCreatedUnit(newCard as UnitCard, false, cpyCrd.AdditionalEffects);
                }
                else newCardObj = DrawCreatedCard(newCard, cpyCrd.AdditionalEffects);

                if (newCardObj == null)
                {
                    Debug.LogWarning("NEW CARD IS NULL!");
                    continue;
                }

                if (cpyCrd.IsExactCopy)
                {
                    List<Effect> newEffects = newCard.CurrentEffects.ToList();
                    foreach (var e in newEffects) AddEffect(newCardObj, e, true, false);
                }
            }
        }
        // CHANGE_COST_EFFECT
        else if (effect is ChangeCostEffect chgCst)
        {            
            if (chgCst.ChangeNextCost)
            {
                var newChgCst = ScriptableObject.CreateInstance<ChangeCostEffect>();
                newChgCst.LoadEffect(chgCst);

                var hMan = HeroManager.GetSourceHero(newEffectSource);
                hMan.ChangeNextCostEffects.Add(newChgCst);

                foreach (var card in hMan.HandZoneCards)
                {
                    if (!IsValidTarget_ChangeCost(card, newChgCst)) continue;

                    card.GetComponent<CardDisplay>().ChangeNextCostValue += newChgCst.ChangeValue;
                }
            }
            else
            {
                foreach (var target in validTargets)
                    NewActiveEffect(target, () =>
                    AddEffect(target, chgCst), shootRay);
            }
        }
        // CHANGE_CONTROL_EFFECT
        else if (effect is ChangeControlEffect chgCtrl)
        {
            foreach (var target in validTargets)
                NewActiveEffect(target, () =>
                Managers.CA_MAN.ChangeUnitControl(target), shootRay);
        }
        // RETURN_CARD_EFFECT
        else if (effect is ReturnCardEffect)
        {
            if (EffectRayError()) return;

            var hMan = HeroManager.GetSourceHero(newEffectSource);

            foreach (var target in validTargets)
                NewActiveEffect(target, () =>
                ReturnCard(target), shootRay);

            void ReturnCard(GameObject target)
            {
                Managers.CA_MAN.ChangeCardZone(target, hMan.HandZone);
                hMan.PlayZoneCards.Remove(target);
                hMan.HandZoneCards.Add(target);
            }
        }
        else Debug.LogError("EFFECT TYPE NOT FOUND!");

        // IF RESOLVES EFFECTS
        foreach (var target in validTargets)
        {
            foreach (var e in effect.IfResolvesEffects)
            {
                if (!e.ShootRay) ActiveEffects++;
                if (!effect.ResolveSimultaneous) newDelay += 0.5f;

                ResolveEffect(new List<GameObject> { target }, e, e.ShootRay, newDelay, out newDelay);
            }
        }

        // IF RESOLVES GROUPS
        foreach (EffectGroup eg in effect.IfResolvesGroups) additionalEffectGroups.Insert(0, eg);

        // ACTIVE EFFECTS
        if (isEffectGroup && !shootRay)
        {
            if ((effect is not DrawEffect or DelayEffect or SaveValueEffect or
                SaveTargetEffect or GiveNextUnitEffect or ModifyNextEffect) &&
                !(effect is ChangeCostEffect cce && cce.ChangeNextCost))
            {
                newDelay += 0.25f;
            }

            FunctionTimer.Create(() => ActiveEffects--, newDelay);
        }

        // NEW ACTIVE EFFECT
        void NewActiveEffect(GameObject target, Action action, bool shootRay)
        {
            // NEW EFFECT INSTANCE
            var newEffect = ScriptableObject.CreateInstance(effect.GetType()) as Effect;
            newEffect.LoadEffect(effect);
            effect = newEffect;

            var effectRayType = isEffectGroup ?
                EffectRay.EffectRayType.EffectGroup : EffectRay.EffectRayType.Default;

            if (shootRay)
            {
                if (effect is HealEffect && !CombatManager.IsDamaged(target)) 
                {
                    ActiveEffects++;
                    FunctionTimer.Create(() => ActiveEffects--, delay);
                    return;
                }

                Color rayColor = effect.RayColor;
                if (effect is DamageEffect or DestroyEffect) rayColor = damageRayColor;
                else if (effect is HealEffect) rayColor = healRayColor;

                if (rayColor.a == 0) rayColor = Color.red;

                ShootEffectRay(target, effect, () => ResolveActiveEffect(target, effect, action));

                void ShootEffectRay(GameObject target, Effect effect, Action action) =>
                    CreateEffectRay(raySource.transform.position, target,
                    () => action(), rayColor, effectRayType, delay);
            }
            else ResolveActiveEffect(target, effect, action);

            void ResolveActiveEffect(GameObject target, Effect effect, Action action)
            {
                if (effect is not DrawEffect) DeriveEffectValues(target, effect);
                action();
            }
        }

        void DeriveEffectValues(GameObject target, Effect effect)
        {
            if (target == null || newEffectSource == null) return;

            target.TryGetComponent(out UnitCardDisplay tarUcd);
            newEffectSource.TryGetComponent(out UnitCardDisplay ucd);

            UnitCardDisplay savTarUcd = null;
            if (savedTarget != null) savedTarget.TryGetComponent(out savTarUcd);

            if (effect.IsDerivedValue) effect.Value = GetDerivedValue(effect.DerivedValue);

            if (effect is StatChangeEffect sce)
            {
                if (tarUcd == null)
                {
                    Debug.LogError("TARGET IS NOT UNIT CARD!");
                    return;
                }

                if (sce.PowerIsDerived) sce.PowerChange = GetDerivedValue(sce.DerivedPowerType);
                if (sce.HealthIsDerived) sce.HealthChange = GetDerivedValue(sce.DerivedHealthType);
            }

            int GetDerivedValue(Effect.DerivedValueType derivedValueType)
            {
                switch (derivedValueType)
                {
                    case Effect.DerivedValueType.Saved_Value:
                        return savedValue;
                    case Effect.DerivedValueType.Source_Power:
                        return ucd.CurrentPower;
                    case Effect.DerivedValueType.Source_Health:
                        return ucd.CurrentHealth;
                    case Effect.DerivedValueType.Target_Power:
                        return tarUcd.CurrentPower;
                    case Effect.DerivedValueType.Target_Health:
                        return tarUcd.CurrentHealth;
                    case Effect.DerivedValueType.Target_Keywords:
                        return Managers.CA_MAN.GetPositiveKeywords(target);
                    case Effect.DerivedValueType.Allies_Count:
                        return HeroManager.GetSourceHero(newEffectSource).PlayZoneCards.Count;
                    case Effect.DerivedValueType.SavedTarget_Power:
                        return savTarUcd.CurrentPower;
                    case Effect.DerivedValueType.SavedTarget_Health:
                        return savTarUcd.CurrentHealth;
                    default:
                        Debug.LogError("INVALID TYPE!");
                        return 0;
                }
            }
        }

        bool EffectRayError()
        {
            if (effect.ShootRay)
            {
                Debug.LogError($"CANNOT SHOOT RAY FOR <{effect}> !");
                return true;
            }
            return false;
        }
    }
    /******
     * *****
     * ****** DRAW_CREATED_CARD
     * *****
     *****/
    private GameObject DrawCreatedCard(Card cardScript, List<Effect> additionalEffects)
    {
        var card = Managers.CA_MAN.DrawCard(HeroManager.GetSourceHero(effectSource), cardScript);

        foreach (var addEffect in additionalEffects)
            ResolveEffect(new List<GameObject> { card }, addEffect, false, 0, out _, false);

        return card;
    }

    /******
     * *****
     * ****** PLAY_CREATED_UNIT
     * *****
     *****/
    public GameObject PlayCreatedUnit(UnitCard unitCardScript, bool enemyCard,
        List<Effect> additionalEffects, GameObject newEffectSource = null)
    {
        if (newEffectSource == null)
        {
            if (effectSource == null)
            {
                Debug.LogError("SOURCE IS NULL!");
                return null;
            }
            newEffectSource = effectSource;
        }

        var hMan_Source = HeroManager.GetSourceHero(newEffectSource, out HeroManager hMan_Enemy);
        if (enemyCard) (hMan_Enemy, hMan_Source) = (hMan_Source, hMan_Enemy);

        string cardTag = hMan_Source.CARD_TAG;
        string errorMessage;

        if (hMan_Source == Managers.P_MAN) errorMessage = "You can't play more units!";
        else errorMessage = "Enemy can't play more units!";

        if (hMan_Source.PlayZoneCards.Count >= GameManager.MAX_UNITS_PLAYED)
        {
            Managers.U_MAN.CreateFleetingInfoPopup(errorMessage);
            return null;
        }

        Vector2 newPosition = newEffectSource.transform.position;
        var card = Managers.CA_MAN.ShowCard(unitCardScript, newPosition,
            CardManager.DisplayType.Default, true);

        card.tag = cardTag;
        hMan_Source.PlayZoneCards.Add(card);
        Managers.CA_MAN.ChangeCardZone(card, hMan_Source.PlayZone);

        Managers.AN_MAN.CreateParticleSystem(card, ParticleSystemHandler.ParticlesType.Drag, 1);
        Managers.AU_MAN.StartStopSound("SFX_CreateCard");
        Managers.U_MAN.CombatLog_PlayCard(card);

        foreach (var addEffect in additionalEffects)
            ResolveEffect(new List<GameObject> { card }, addEffect, false, 0, out _, false);

        var cardDisplay = card.GetComponent<CardDisplay>();
        var container = cardDisplay.CardContainer.GetComponent<CardContainer>();
        container.OnAttachAction += () => PlayUnit(card);
        return card;

        void PlayUnit(GameObject unitCard)
        {
            var ucd = unitCard.GetComponent<UnitCardDisplay>();
            Managers.AU_MAN.StartStopSound(null, ucd.UnitCard.CardPlaySound);
            unitCard.GetComponent<DragDrop>().IsPlayed = true;
            Managers.AN_MAN.UnitStatChangeState(unitCard, 0, 0, false, true);
            unitCard.transform.SetAsFirstSibling();

            Managers.CA_MAN.TriggerTrapAbilities(card); // Resolves 4th
            TriggerModifiers_PlayCard(card); // Resolves 3rd
            TriggerGiveNextEffects(card); // Resolves 2nd
            Managers.CA_MAN.TriggerUnitAbility(card, CardManager.TRIGGER_PLAY); // Resolves 1st
        }
    }
    #endregion

    #region TARGET SELECTION
    /******
     * *****
     * ****** SELECT/ACCEPT/REJECT/REMOVE_EFFECT_TARGET
     * *****
     *****/
    public void HighlightEffectTarget(GameObject target, bool isSelected)
    {
        if (currentEffectGroup > acceptedTargets.Count - 1) return; // Unnecessary?
        if (currentEffectGroup > legalTargets.Count - 1) return; // Unnecessary?

        UIManager.SelectionType type;
        if (legalTargets[currentEffectGroup].Contains(target))
        {
            if (isSelected) type = UIManager.SelectionType.Playable;
            else type = UIManager.SelectionType.Highlighted;
        }
        else if (acceptedTargets[currentEffectGroup].Contains(target))
            type = UIManager.SelectionType.Selected;
        else
        {
            if (isSelected) type = UIManager.SelectionType.Rejected;
            else type = UIManager.SelectionType.Disabled;
        }
        Managers.U_MAN.SelectTarget(target, type);
    }
    public void SelectEffectTarget(GameObject target)
    {
        if (currentEffectGroup > acceptedTargets.Count - 1) return; // Unnecessary?
        if (currentEffectGroup > legalTargets.Count - 1) return; // Unnecessary?

        if (effectSource != null && CombatManager.IsUnitCard(target))
        {
            if (HeroManager.GetSourceHero(effectSource) != HeroManager.GetSourceHero(target))
            {
                if (CardManager.GetAbility(target, CardManager.ABILITY_WARD))
                {
                    RejectEffectTarget("Enemies with Ward can't be targetted!");
                    return;
                }
            }
        }

        if (acceptedTargets[currentEffectGroup].Contains(target)) RemoveEffectTarget(target);
        else if (legalTargets[currentEffectGroup].Contains(target)) AcceptEffectTarget(target);
        else RejectEffectTarget("You can't target that!");
    }
    private void AcceptEffectTarget(GameObject target)
    {        
        int targetNumber = CurrentEffectGroup.Targets.TargetNumber;
        int legalTargetNumber = legalTargets[currentEffectGroup].Count +
            acceptedTargets[currentEffectGroup].Count;

        if (!CurrentEffect.IsRequired && legalTargetNumber < targetNumber)
            targetNumber = legalTargetNumber;

        int accepted = acceptedTargets[currentEffectGroup].Count;
        if (accepted == targetNumber)
        {
            if (CurrentEffectGroup.Targets.VariableNumber) Debug.Log("ALL TARGETS SELECTED!");
            else Debug.LogError("TARGETTING ERROR!");
            return;
        }
        else if (accepted > targetNumber)
        {

            Debug.LogError("TOO MANY ACCEPTED TARGETS!");
            return;
        }

        Managers.AU_MAN.StartStopSound("SFX_AcceptTarget");
        Managers.U_MAN.SelectTarget(target, UIManager.SelectionType.Selected);
        acceptedTargets[currentEffectGroup].Add(target);
        legalTargets[currentEffectGroup].Remove(target);

        //Debug.Log($"ACCEPTED TARGETS: <{acceptedTargets[currentEffectGroup].Count}> OF <{targetNumber}> REQUIRED TARGETS");

        if (CurrentEffectGroup.Targets.VariableNumber) Managers.U_MAN.SetConfirmEffectButton(true);
        else if (acceptedTargets[currentEffectGroup].Count == targetNumber) ConfirmTargetEffect();
    }
    private void RejectEffectTarget(string message)
    {
        Managers.U_MAN.CreateFleetingInfoPopup(message);
        Managers.AU_MAN.StartStopSound("SFX_Error");
    }
    private void RemoveEffectTarget(GameObject target)
    {
        EffectTargets et = effectGroupList[currentEffectGroup].Targets;

        Managers.AU_MAN.StartStopSound("SFX_AcceptTarget");
        Managers.U_MAN.SelectTarget(target, UIManager.SelectionType.Highlighted);
        acceptedTargets[currentEffectGroup].Remove(target);
        legalTargets[currentEffectGroup].Add(target);

        if (acceptedTargets[currentEffectGroup].Count < 1)
        {
            if (!(CurrentEffect is DrawEffect de && de.IsMulliganEffect || et.AllowZero))
                Managers.U_MAN.SetConfirmEffectButton(false);
        }
    }
    #endregion

    #region EFFECT APPLICATION
    /******
     * *****
     * ****** ADD_EFFECT
     * *****
     *****/
    public void AddEffect(GameObject card, Effect effect, bool newInstance = true, bool applyEffect = true, bool showEffect = true)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return;
        }

        if (Managers.EN_MAN.HandZoneCards.Contains(card)) showEffect = false; // Hide enemy effects

        var cd = card.GetComponent<CardDisplay>();
        UnitCardDisplay ucd = null;

        if (CombatManager.IsUnitCard(card))
        {
            ucd = CombatManager.GetUnitDisplay(card);
            if (ucd.CurrentHealth < 1) return;
        }

        if (effect is ChangeCostEffect chgCst)
        {
            bool isUnit = CombatManager.IsUnitCard(card);
            if (!isUnit && !chgCst.ChangeActionCost) return;
            if (isUnit && !chgCst.ChangeUnitCost) return;

            ChangeCostEffect newChgCst;
            if (newInstance)
            {
                newChgCst = ScriptableObject.CreateInstance<ChangeCostEffect>();
                newChgCst.LoadEffect(chgCst);
            }
            else newChgCst = chgCst;

            // Make sure the change cost effect hasn't already been applied
            if (cd.CardScript.CurrentEffects.Contains(chgCst))
            {
                Debug.LogWarning("CHANGE COST EFFECT ALREADY EXISTS!");
                return;
            }

            AddCurrentEffect(newChgCst);
            if (applyEffect) cd.ChangeCurrentEnergyCost(newChgCst.ChangeValue);
        }
        else if (effect is GiveAbilityEffect gae)
        {
            if (ucd == null)
            {
                Debug.LogError("UNIT CARD DISPLAY IS NULL!");
                return;
            }

            GiveAbilityEffect newGae;
            if (newInstance)
            {
                newGae = ScriptableObject.CreateInstance<GiveAbilityEffect>();
                newGae.LoadEffect(gae);
            }
            else newGae = gae;

            if (!applyEffect)
            {
                AddCurrentEffect(newGae);
                return;
            }

            switch (newGae.Type)
            {
                case GiveAbilityEffect.GiveAbilityType.Default:
                    GiveAbility_Default();
                    break;
                case GiveAbilityEffect.GiveAbilityType.RandomPositiveKeyword:
                    GiveAbility_RandomPositiveKeyword();
                    break;
                case GiveAbilityEffect.GiveAbilityType.SavedTarget_PositiveKeywords:
                    GiveAbilities_SavedTarget_PositiveKeywords();
                    break;
                default:
                    Debug.LogError("INVALID TYPE!");
                    return;
            }

            void GiveAbility_Default(GiveAbilityEffect defGae = null)
            {
                if (defGae == null) defGae = newGae;

                if (defGae.Type != GiveAbilityEffect.GiveAbilityType.Default)
                {
                    Debug.LogError("GIVE ABILITY EFFECT IS NOT DEFAULT TYPE!");
                    return;
                }

                if (!ucd.AddCurrentAbility(defGae.CardAbility, false, showEffect))
                {
                    // If ability is static and already exists, update countdown instead of adding ability
                    if (defGae.CardAbility is StaticAbility) { }
                    else
                    {
                        Debug.LogError("ABILITY NOT FOUND!");
                        return;
                    }

                    foreach (var cEffect in ucd.CardScript.CurrentEffects)
                        if (cEffect is GiveAbilityEffect cGae)
                            if (cGae.CardAbility.AbilityName == defGae.CardAbility.AbilityName)
                            {
                                if ((defGae.Countdown == 0) ||
                                    (cGae.Countdown != 0 && defGae.Countdown > cGae.Countdown))
                                    cGae.Countdown = defGae.Countdown;
                            }
                }
                else
                {
                    AddCurrentEffect(defGae);
                    SpecialTrigger_AllyGainsAbility(defGae.CardAbility);
                }
            }

            void GiveAbility_RandomPositiveKeyword()
            {
                CardAbility randomKeyword;
                List<CardAbility> generatableKeywords = new();
                foreach (var keyword in Managers.CA_MAN.GeneratableKeywords)
                {
                    if (!CardManager.GetAbility(card, keyword.AbilityName))
                        generatableKeywords.Add(keyword);
                }

                if (generatableKeywords.Count < 1) Debug.Log("NO GENERATABLE KEYWORDS!");
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, generatableKeywords.Count);
                    randomKeyword = generatableKeywords[randomIndex];
                    ucd.AddCurrentAbility(randomKeyword, false, showEffect);

                    // Convert effect into non-random GiveAbilityEffect
                    newGae.name = $"Give_{randomKeyword.AbilityName}"; // For Resources.Load functionality
                    //Debug.LogWarning($"CONVERTED EFFECT NAME ---> {newGae.name}");
                    newGae.Type = GiveAbilityEffect.GiveAbilityType.Default;
                    newGae.CardAbility = randomKeyword;

                    SpecialTrigger_AllyGainsAbility(randomKeyword);
                }

                AddCurrentEffect(newGae);
            }

            void GiveAbilities_SavedTarget_PositiveKeywords()
            {
                if (savedTarget == null)
                {
                    Debug.LogError("SAVED TARGET IS NULL!");
                    return;
                }
                if (!savedTarget.TryGetComponent(out UnitCardDisplay savedUcd))
                {
                    Debug.LogError("SAVED TARGET IS NOT UNIT CARD!");
                    return;
                }

                foreach (var ca in savedUcd.CurrentAbilities)
                {
                    if (CardManager.PositiveAbilities.Contains(ca.AbilityName))
                    {
                        // Convert effect into NEW INSTANCE of DEFAULT GiveAbilityEffect
                        var fixedGae = ScriptableObject.CreateInstance<GiveAbilityEffect>();
                        fixedGae.LoadEffect(newGae);

                        fixedGae.name = $"Give_{ca.AbilityName}"; // Rename effect for Resources.Load functionality
                        Debug.LogWarning($"CONVERTED EFFECT NAME ::: {fixedGae.name}");
                        fixedGae.Type = GiveAbilityEffect.GiveAbilityType.Default;
                        fixedGae.CardAbility = ca;

                        GiveAbility_Default(fixedGae);
                    }
                }
            }

            void SpecialTrigger_AllyGainsAbility(CardAbility cardAbility)
            {
                var cardZone = HeroManager.GetSourceHero(card).PlayZoneCards;
                TriggerModifiers_SpecialTrigger(ModifierAbility.TriggerType.AllyGainsAbility, cardZone, null, cardAbility);
            }
        }
        else if (effect is StatChangeEffect sce)
        {
            if (ucd == null)
            {
                Debug.LogError("UNIT CARD DISPLAY IS NULL!");
                return;
            }

            StatChangeEffect newSce;
            if (newInstance)
            {
                newSce = ScriptableObject.CreateInstance<StatChangeEffect>();
                newSce.LoadEffect(sce);
            }
            else newSce = sce;

            AddCurrentEffect(newSce);

            if (!applyEffect) return;

            if (newSce.ResetStats)
            {
                newSce.PowerChange = ucd.UnitCard.StartPower - ucd.CurrentPower;
                if (newSce.PowerChange > 0) newSce.PowerChange = 0;

                newSce.HealthChange = ucd.UnitCard.StartHealth - ucd.CurrentHealth;
                if (newSce.HealthChange > 0) newSce.HealthChange = 0;

                List<Effect> statChangeEffects = new();
                foreach (var e in ucd.CardScript.CurrentEffects)
                    if (e is StatChangeEffect) statChangeEffects.Add(e);
                foreach (var e in statChangeEffects)
                    ucd.CardScript.CurrentEffects.Remove(e);

                if (ucd.CurrentPower > ucd.UnitCard.StartPower)
                    ucd.CurrentPower = ucd.UnitCard.StartPower;

                ucd.MaxHealth = ucd.UnitCard.StartHealth;
                if (ucd.CurrentHealth > ucd.MaxHealth)
                    ucd.CurrentHealth = ucd.MaxHealth;
            }
            else
            {
                if (newSce.DoublePower) newSce.PowerChange = ucd.CurrentPower;
                if (newSce.SetPowerZero) newSce.PowerChange = -ucd.CurrentPower;
                if (newSce.DoubleHealth) newSce.HealthChange = ucd.CurrentHealth;

                ucd.ChangeCurrentPower(newSce.PowerChange);
                ucd.MaxHealth += newSce.HealthChange;
                ucd.CurrentHealth += newSce.HealthChange;
            }

            if (showEffect) Managers.AN_MAN.ShowStatChange(card, newSce, false);
        }
        else
        {
            Debug.LogError("EFFECT TYPE NOT FOUND!");
            return;
        }

        void AddCurrentEffect(Effect effect)
        {
            if (newInstance && effect.IsPermanent)
                cd.CardScript.PermanentEffects.Add(effect);

            cd.CardScript.CurrentEffects.Add(effect);
        }
    }

    /******
     * *****
     * ****** REMOVE_TEMPORARY_EFFECTS
     * *****
     *****/
    public void RemoveTemporaryEffects()
    {
        var playZones = Managers.P_MAN.PlayZoneCards.Concat(Managers.EN_MAN.PlayZoneCards).ToList();
        foreach (var card in playZones)
        {
            var ucd = CombatManager.GetUnitDisplay(card);
            List<Effect> expiredEffects = new();

            foreach (var effect in ucd.CardScript.CurrentEffects)
            {
                if (effect.Countdown == 1) // Check for EXPIRED effects
                {
                    Debug.Log($"EFFECT REMOVED: <{effect}>");
                    expiredEffects.Add(effect);

                    Managers.EV_MAN.NewDelayedAction(() => RemoveEffect(card, effect), 0.25f);

                    static void RemoveEffect(GameObject card, Effect effect)
                    {
                        var cd = card.GetComponent<CardDisplay>();
                        var ucd = cd as UnitCardDisplay;

                        if (effect is ChangeCostEffect chgCst)
                            cd.ChangeCurrentEnergyCost(-chgCst.ChangeValue);

                        else if (effect is GiveAbilityEffect gae)
                            ucd.RemoveCurrentAbility(gae.CardAbility.AbilityName);

                        else if (effect is StatChangeEffect sce)
                        {
                            ucd.ChangeCurrentPower(-sce.PowerChange);
                            ucd.MaxHealth -= sce.HealthChange;
                            int oldHealth = ucd.CurrentHealth;
                            if (ucd.CurrentHealth > ucd.MaxHealth) ucd.CurrentHealth = ucd.MaxHealth;
                            sce.HealthChange = oldHealth - ucd.CurrentHealth;
                            Managers.AN_MAN.ShowStatChange(card, sce, true);
                        }
                    }
                }
                else if (effect.Countdown != 0) effect.Countdown--;
            }

            foreach (Effect effect in expiredEffects)
            {
                ucd.CardScript.CurrentEffects.Remove(effect);
                Destroy(effect);
            }

            ucd = null;
            expiredEffects = null;
        }
    }

    /******
     * *****
     * ****** TRIGGER_GIVE_NEXT_EFFECTS
     * *****
     *****/
    public void TriggerGiveNextEffects(GameObject card)
    {
        var hMan = HeroManager.GetSourceHero(card);
        var giveNextEffects = hMan.GiveNextEffects;
        List<GiveNextUnitEffect> resolvedGnue = new();

        if (giveNextEffects.Count < 1) return;

        List<GameObject> target = new() { card };
        foreach (var gnue in giveNextEffects)
        {
            foreach (var e in gnue.Effects)
                Managers.EV_MAN.NewDelayedAction(() =>
                ResolveEffect(target, e, true, 0, out _, false, hMan.HeroObject), 0.25f, true);

            if (!gnue.Unlimited && --gnue.Multiplier < 1) resolvedGnue.Add(gnue);
        }

        foreach (var rGnue in resolvedGnue)
        {
            giveNextEffects.Remove(rGnue);
            Destroy(rGnue);
        }
    }

    /******
     * *****
     * ****** REMOVE_GIVE_NEXT_EFFECTS
     * *****
     *****/
    public void RemoveGiveNextEffects(HeroManager hero)
    {
        List<GiveNextUnitEffect> expiredGne = new();

        foreach (var gnfe in hero.GiveNextEffects)
            if (gnfe.Countdown == 1) expiredGne.Add(gnfe);
            else if (gnfe.Countdown != 0) gnfe.Countdown--;

        foreach (var xGnfe in expiredGne)
        {
            hero.GiveNextEffects.Remove(xGnfe);
            Destroy(xGnfe);
        }
    }

    /******
     * *****
     * ****** TRIGGER_MODIFIERS_TRIGGER_ABILITY
     * *****
     *****/
    public int TriggerModifiers_TriggerAbility(string abilityTrigger, GameObject card)
    {
        int modsFound = 0;
        var cardZone = HeroManager.GetSourceHero(card).PlayZoneCards;

        foreach (var unit in cardZone)
        {
            bool modFound = false;
            var ucd = unit.GetComponent<UnitCardDisplay>();
            foreach (var ca in ucd.CurrentAbilities)
            {
                if (ca is ModifierAbility ma)
                {
                    if (ma.AllAbilityTriggers || (ma.AbilityTrigger != null && 
                        ma.AbilityTrigger.AbilityName == abilityTrigger))
                    {
                        modsFound++;
                        modFound = true;
                    }
                }
            }

            if (modFound)
            {
                foreach (var ca in ucd.DisplayAbilities)
                    if (ca is ModifierAbility)
                    {
                        ucd.AbilityTriggerState(ca.AbilityName);
                        break;
                    }
            }
        }

        return modsFound += TriggerModifyNextEffect_AbilityTrigger(abilityTrigger, card);
    }

    /******
     * *****
     * ****** TRIGGER_MODIFIERS_PLAY_CARD
     * *****
     *****/
    public void TriggerModifiers_PlayCard(GameObject card)
    {
        var cardZone = HeroManager.GetSourceHero(card).PlayZoneCards;
        var cd = card.GetComponent<CardDisplay>();

        foreach (GameObject unit in cardZone)
        {
            if (unit == card) continue;

            bool modFound = false;
            var ucd = unit.GetComponent<UnitCardDisplay>();
            foreach (var ca in ucd.CurrentAbilities)
            {
                if (ca is ModifierAbility ma)
                {
                    if (cd is UnitCardDisplay)
                    {
                        if (ma.ModifyPlayUnit)
                        {
                            modFound = true;

                            if (ma.EffectGroupList.Count > 0)
                                Managers.EV_MAN.NewDelayedAction(() =>
                                StartEffectGroupList(ma.EffectGroupList, unit), 0, true);

                            foreach (var e in ma.PlayUnitEffects)
                            {
                                Managers.EV_MAN.NewDelayedAction(() =>
                                ResolveEffect(new List<GameObject> { card },
                                e, true, 0, out _, false, unit), 0.5f, true);
                            }
                        }
                    }
                    else if (cd is ActionCardDisplay)
                    {
                        if (ma.ModifyPlayAction)
                        {
                            if (!string.IsNullOrEmpty(ma.PlayActionType) &&
                                ma.PlayActionType != cd.CardScript.CardSubType) continue;

                            modFound = true;
                            Managers.EV_MAN.NewDelayedAction(() =>
                            StartEffectGroupList(ma.EffectGroupList, unit), 0, true);
                        }
                    }
                }
            }

            if (modFound)
            {
                foreach (var ca in ucd.DisplayAbilities)
                    if (ca is ModifierAbility)
                    {
                        ucd.AbilityTriggerState(ca.AbilityName);
                        break;
                    }
            }
        }
    }

    /******
     * *****
     * ****** TRIGGER_MODIFIERS_SPECIAL_TRIGGER
     * *****
     *****/
    public void TriggerModifiers_SpecialTrigger(ModifierAbility.TriggerType triggerType,
        List<GameObject> cardZone, GameObject target = null, CardAbility allyAbility = null)
    {
        foreach (var unit in cardZone)
        {
            var ucd = CombatManager.GetUnitDisplay(unit);
            if (ucd.CurrentHealth < 1 || UnitsToDestroy.Contains(unit)) continue;

            bool modFound = false;

            foreach (var ca in ucd.CurrentAbilities)
            {
                if (ca is ModifierAbility ma)
                {
                    if (ma.TriggerLimit != 0 && ma.TriggerCount >= ma.TriggerLimit) continue;

                    if (ma.ModifySpecialTrigger && ma.SpecialTriggerType == triggerType)
                    {
                        if (ma.SpecialTriggerType == ModifierAbility.TriggerType.AllyGainsAbility)
                        {
                            if (allyAbility == null)
                            {
                                Debug.LogError("ALLY ABILITY IS NULL!");
                                continue;
                            }

                            if (ma.AllyAbility.AbilityName != allyAbility.AbilityName) continue;
                        }

                        ma.TriggerCount++;
                        modFound = true;

                        int totalTriggers = 1 +
                            TriggerModifiers_TriggerAbility(ma.SpecialTriggerType.ToString(), unit);

                        if (ma.RemoveAfterTrigger)
                        {
                            Managers.EV_MAN.NewDelayedAction(() =>
                            ucd.RemoveCurrentAbility(ca.AbilityName, false), 0, true);
                        }

                        for (int i = 0; i < totalTriggers; i++)
                        {
                            bool effectsFound = false;

                            if (ma.EffectGroupList.Count > 0)
                            {
                                effectsFound = true;

                                Managers.EV_MAN.NewDelayedAction(() =>
                                StartEffectGroupList(ma.EffectGroupList, unit), 0.5f, true);
                            }

                            if (ma.SpecialTriggerEffects.Count > 0)
                            {
                                effectsFound = true;

                                if (target == null)
                                {
                                    Debug.LogError("TARGET IS NULL!");
                                    return;
                                }

                                foreach (var e in ma.SpecialTriggerEffects)
                                {
                                    Managers.EV_MAN.NewDelayedAction(() =>
                                    ResolveEffect(new List<GameObject> { target }, e, e.ShootRay, 0, out _, false, unit), 0.5f, true);
                                }
                            }

                            if (!effectsFound) Debug.LogError($"EMPTY EFFECT/EFFECT GROUP! <{ca}>");
                        }
                    }
                }
            }

            if (modFound)
            {
                foreach (var ca in ucd.CurrentAbilities)
                    if (ca is ModifierAbility)
                    {
                        ucd.AbilityTriggerState(ca.AbilityName);
                        break;
                    }
            }

            bool hasEnabledModifiers = false;
            foreach (var ca in ucd.CurrentAbilities)
                if (ca is ModifierAbility ma)
                {
                    if (ma.TriggerLimit != 0 && ma.TriggerCount >= ma.TriggerLimit) { }
                    else hasEnabledModifiers = true;
                }

            if (!hasEnabledModifiers) ucd.EnableTriggerIcon(null, false);
        }
    }

    public bool IsValidTarget_ChangeCost(GameObject card, ChangeCostEffect chgCst)
    {
        if (CombatManager.IsUnitCard(card))
        {
            if (!chgCst.ChangeUnitCost) return false;
        }
        else if (!chgCst.ChangeActionCost) return false;

        return true;
    }

    /******
     * *****
     * ****** APPLY_CHANGE_NEXT_COST_EFFECTS
     * *****
     *****/
    public void ApplyChangeNextCostEffects(GameObject card)
    {
        foreach (var chgCst in HeroManager.GetSourceHero(card).ChangeNextCostEffects)
        {
            if (!IsValidTarget_ChangeCost(card, chgCst)) continue;

            card.GetComponent<CardDisplay>().ChangeNextCostValue += chgCst.ChangeValue;
        }
    }

    /******
     * *****
     * ****** RESOLVE_CHANGE_NEXT_COST_EFFECTS
     * *****
     *****/
    public void ResolveChangeNextCostEffects(GameObject card)
    {
        var hMan = HeroManager.GetSourceHero(card);
        if (hMan.ChangeNextCostEffects.Count < 1) return;

        List<ChangeCostEffect> expiredChgCst = new();
        foreach (var chgCst in hMan.ChangeNextCostEffects)
        {
            if (!IsValidTarget_ChangeCost(card, chgCst)) continue;

            if (!chgCst.Unlimited && --chgCst.Multiplier < 1) expiredChgCst.Add(chgCst);
        }
        foreach (var xChgCst in expiredChgCst)
            FinishChangeNextCostEffect(hMan, xChgCst);
    }

    /******
     * *****
     * ****** FINISH_CHANGE_NEXT_COST_EFFECTS
     * *****
     *****/
    private void FinishChangeNextCostEffect(HeroManager hero, ChangeCostEffect rChgCst)
    {
        foreach (var card in hero.HandZoneCards)
        {
            if (!IsValidTarget_ChangeCost(card, rChgCst)) continue;

            card.GetComponent<CardDisplay>().ChangeNextCostValue -= rChgCst.ChangeValue;
        }

        if (!hero.ChangeNextCostEffects.Remove(rChgCst))
            Debug.LogError("CHANGE COST EFFECT NOT FOUND!");

        Destroy(rChgCst);
    }

    /******
     * *****
     * ****** REMOVE_CHANGE_NEXT_COST_EFFECTS
     * *****
     *****/
    public void RemoveChangeNextCostEffects(HeroManager hero)
    {
        if (hero.ChangeNextCostEffects.Count < 1) return;

        List<ChangeCostEffect> expiredChgCst = new();
        foreach (var chgCst in hero.ChangeNextCostEffects)
            if (chgCst.Countdown == 1) expiredChgCst.Add(chgCst);
            else if (chgCst.Countdown != 0) chgCst.Countdown--;

        foreach (var xChgCst in expiredChgCst)
            FinishChangeNextCostEffect(hero, xChgCst);
    }

    /******
     * *****
     * ****** TRIGGER_MODIFY_NEXT_EFFECTS
     * *****
     *****/
    public int TriggerModifyNextEffect_AbilityTrigger(string abilityTrigger, GameObject effectSource)
    {
        var hMan = HeroManager.GetSourceHero(effectSource);
        if (hMan.ModifyNextEffects.Count < 1) return 0;

        int triggerCount = 0;
        List<ModifyNextEffect> resolveMne = new();

        foreach (var mne in hMan.ModifyNextEffects)
        {
            var mna = mne.ModifyNextAbility;

            if ((mna.ModifySpecialTrigger && mna.SpecialTriggerType.ToString() == abilityTrigger) || // Matches the specified special trigger
                (mna.AbilityTrigger != null && mna.AbilityTrigger.AbilityName == abilityTrigger) || // Matches the specified ability trigger
                mna.AllAbilityTriggers)
            {
                triggerCount++;
                if (!mne.Unlimited && --mne.Multiplier < 1) resolveMne.Add(mne);
            }
        }

        foreach (var rMne in resolveMne)
        {
            hMan.ModifyNextEffects.Remove(rMne);
            Destroy(rMne);
        }

        return triggerCount;
    }

    /******
     * *****
     * ****** REMOVE_MODIFY_NEXT_EFFECTS
     * *****
     *****/
    public void RemoveModifyNextEffects(HeroManager hero)
    {
        List<ModifyNextEffect> expiredMne = new();

        foreach (var gnfe in hero.ModifyNextEffects)
            if (gnfe.Countdown == 1) expiredMne.Add(gnfe);
            else if (gnfe.Countdown != 0) gnfe.Countdown--;

        foreach (var xGnfe in expiredMne)
        {
            hero.ModifyNextEffects.Remove(xGnfe);
            Destroy(xGnfe);
        }
    }

    /******
     * *****
     * ****** CREATE_EFFECT_RAY
     * *****
     *****/
    public void CreateEffectRay(Vector2 start, GameObject target, System.Action rayEffect,
        Color rayColor, EffectRay.EffectRayType effectRayType, float delay = 0)
    {
        var ray = Instantiate(effectRay, start, Quaternion.identity);
        ray.transform.SetParent(Managers.U_MAN.CurrentWorldSpace.transform);

        if (delay < 0)
        {
            Debug.LogError("DELAY IS NEGATIVE!");
            return;
        }

        if (delay == 0) SetRay();
        else FunctionTimer.Create(() => SetRay(), delay);
        if (effectRayType is EffectRay.EffectRayType.EffectGroup) ActiveEffects++;

        void SetRay() => ray.GetComponent<EffectRay>().SetEffectRay(target, rayEffect, rayColor, effectRayType);
    }
    #endregion
    #endregion
}
