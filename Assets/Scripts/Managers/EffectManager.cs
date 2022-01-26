using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private PlayerManager pMan;
    private EnemyManager enMan;
    private CombatManager coMan;
    private UIManager uMan;
    private AudioManager auMan;
    private AnimationManager anMan;
    private EventManager evMan;
    private GameObject dragArrow;

    // Effects
    private bool effectsResolving;
    private bool isAdditionalEffect;
    private GameObject effectSource;
    private string triggerName;
    private int currentEffectGroup;
    private int currentEffect;

    private List<EffectGroup> effectGroupList;
    private List<List<GameObject>> legalTargets;
    private List<List<GameObject>> acceptedTargets;
    private List<EffectGroup> additionalEffectGroups;

    private List<GameObject> newDrawnCards;
    private List<GameObject> unitsToDestroy;
    private List<GiveNextUnitEffect> giveNextEffects;

    [SerializeField] private GameObject effectRay;

    private const string IS_ADDITIONAL_EFFECT = "IsAdditionalEffect";

    public bool EffectsResolving
    {
        get => effectsResolving;
        private set
        {
            effectsResolving = value;
            evMan.PauseDelayedActions(value);

            uMan.EndTurnButton.GetComponent
                <Button>().interactable = !value;
        }
    }
    public Effect CurrentEffect
    {
        get
        {
            if (effectGroupList == null)
            {
                Debug.LogWarning("GROUP LIST IS NULL!");
                return null;
            }
            return effectGroupList[currentEffectGroup].Effects[currentEffect];
        }
    }
    public List<GameObject> CurrentLegalTargets
    {
        get
        {
            if (effectGroupList == null)
            {
                Debug.LogError("GROUP LIST IS NULL!");
                return null;
            }
            return legalTargets[currentEffectGroup];
        }
    }

    public List<GameObject> NewDrawnCards { get => newDrawnCards; }
    public List<GiveNextUnitEffect> GiveNextEffects { get => giveNextEffects; }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;
        anMan = AnimationManager.Instance;
        evMan = EventManager.Instance;

        giveNextEffects = new List<GiveNextUnitEffect>();
        newDrawnCards = new List<GameObject>();
        unitsToDestroy = new List<GameObject>();
        additionalEffectGroups = new List<EffectGroup>();
        
        effectsResolving = false; // TESTING
    }

    /******
     * *****
     * ****** IS_PLAYER_SOURCE
     * *****
     *****/
    private bool IsPlayerSource(GameObject source)
    {
        if (source == null)
        {
            Debug.LogError("SOURCE IS NULL!");
            return false;
        }
        if (source.CompareTag(CombatManager.PLAYER_CARD) ||
            source.CompareTag(CombatManager.PLAYER_HERO) || 
            source.TryGetComponent(out ItemIcon _))
            return true;
        else return false;
    }

    /******
     * *****
     * ****** START_EFFECT_GROUP_LIST
     * *****
     *****/
    public void StartEffectGroupList(List<EffectGroup> groupList,
        GameObject source, string triggerName = null)
    {
        Debug.LogWarning("START EFFECT GROUP LIST!");

        if (source == null)
        {
            Debug.LogError("SOURCE IS NULL!");
            return;
        }

        // WATCH
        if (EffectsResolving)
        {
            Debug.LogWarning("GROUP LIST DELAYED!");
            EventManager.Instance.NewDelayedAction(() =>
            StartEffectGroupList(groupList, source, triggerName), 0.5f, true);
            return;
        }

        EffectsResolving = true;
        effectSource = source;
        this.triggerName = triggerName;
        currentEffectGroup = 0;
        currentEffect = 0;
        newDrawnCards.Clear();
        effectGroupList = new List<EffectGroup>();

        List<Effect> emptyEffects = new List<Effect>();
        foreach (EffectGroup eg in groupList)
        {
            if (eg == null)
            {
                Debug.LogError("EMPTY EFFECT GROUP!");
                continue;
            }
            foreach (Effect e in eg.Effects)
            {
                if (e == null)
                {
                    emptyEffects.Add(e);
                    Debug.LogError("EMPTY EFFECT!");
                }
            }
            foreach (Effect e in emptyEffects)
                eg.Effects.Remove(e);
            effectGroupList.Add(eg);
        }

        if (additionalEffectGroups.Count > 0) isAdditionalEffect = true;
        else isAdditionalEffect = false;
        additionalEffectGroups.Clear();
        if (effectSource.TryGetComponent(out UnitCardDisplay ucd))
        {
            if (triggerName != IS_ADDITIONAL_EFFECT)
                ucd.AbilityTriggerState(triggerName);
        }

        if (!CheckLegalTargets(effectGroupList, effectSource))
            AbortEffectGroupList(false);
        else StartNextEffectGroup(true);
    }

    /******
     * *****
     * ****** START_NEXT_EFFECT_GROUP
     * *****
     *****/
    private void StartNextEffectGroup(bool isFirstGroup = false)
    {
        if (!isFirstGroup) currentEffectGroup++;
        if (currentEffectGroup < effectGroupList.Count)
        {
            Debug.Log("[GROUP #" + (currentEffectGroup + 1) +
                "] <" + effectGroupList[currentEffectGroup].ToString() + ">");
            StartNextEffect(true);
        }
        else if (currentEffectGroup == effectGroupList.Count)
            ResolveEffectGroupList();
        else Debug.LogError("EffectGroup > GroupList!");
    }

    /******
     * *****
     * ****** IS_TARGET_EFFECT
     * *****
     *****/
    private bool IsTargetEffect(EffectGroup group, Effect effect)
    {
        if (effect is DrawEffect de && de.IsDiscardEffect) return true;
        else if (effect is DrawEffect || effect is GiveNextUnitEffect) return false;
        else if (group.Targets.TargetsAll || group.Targets.TargetsSelf) return false;
        else if (group.Targets.PlayerHero || group.Targets.EnemyHero)
        {
            if (!group.Targets.PlayerUnit && !group.Targets.EnemyUnit) return false;
        }
        return true;
    }

    /******
     * *****
     * ****** START_NEXT_EFFECT
     * *****
     *****/
    private void StartNextEffect(bool isFirstEffect = false)
    {
        EffectGroup eg = effectGroupList[currentEffectGroup];
        if (!isFirstEffect) currentEffect++;
        else currentEffect = 0;

        if (currentEffect < eg.Effects.Count)
        {
            if (eg.Effects[currentEffect] == null)
            {
                Debug.LogError("EFFECT IS NULL!");
                return;
            }

            Debug.Log("[EFFECT #" + (currentEffect + 1) + 
                "] <" + eg.Effects[currentEffect].ToString() + ">");
        }
        else if (currentEffect == eg.Effects.Count)
        {
            StartNextEffectGroup();
            return;
        }
        else Debug.LogError("CurrentEffect > Effects!");

        Effect effect = eg.Effects[currentEffect];
        if (IsTargetEffect(eg, effect)) StartTargetEffect(effect);
        else StartNonTargetEffect();
    }
    
    /******
     * *****
     * ****** START_NON_TARGET_EFFECT
     * *****
     *****/
    private void StartNonTargetEffect()
    {
        EffectTargets et = effectGroupList[currentEffectGroup].Targets;
        if (et.TargetsSelf)
        {
            acceptedTargets[currentEffectGroup].Add(effectSource);
            ConfirmNonTargetEffect();
            return;
        }

        if (IsPlayerSource(effectSource))
        {
            if (et.PlayerHero) acceptedTargets[currentEffectGroup].Add(coMan.PlayerHero);
            if (et.EnemyHero) acceptedTargets[currentEffectGroup].Add(coMan.EnemyHero);
            if (et.TargetsAll)
            {
                if (et.PlayerUnit)
                {
                    foreach (GameObject card in coMan.PlayerZoneCards)
                        acceptedTargets[currentEffectGroup].Add(card);
                }
                if (et.EnemyUnit)
                {
                    foreach (GameObject card in coMan.EnemyZoneCards)
                        acceptedTargets[currentEffectGroup].Add(card);
                }
            }
        }
        else
        {
            if (et.EnemyHero) acceptedTargets[currentEffectGroup].Add(coMan.PlayerHero);
            if (et.PlayerHero) acceptedTargets[currentEffectGroup].Add(coMan.EnemyHero);
            if (et.TargetsAll)
            {
                if (et.PlayerUnit)
                {
                    foreach (GameObject card in coMan.EnemyZoneCards)
                        acceptedTargets[currentEffectGroup].Add(card);
                }
                if (et.EnemyUnit)
                {
                    foreach (GameObject card in coMan.PlayerZoneCards)
                        acceptedTargets[currentEffectGroup].Add(card);
                }
            }
        }
        ConfirmNonTargetEffect();
    }

    /******
     * *****
     * ****** START_TARGET_EFFECT
     * *****
     *****/
    private void StartTargetEffect(Effect effect)
    {
        if (acceptedTargets[currentEffectGroup].Count > 0)
        {
            StartNextEffectGroup();
            return;
        }

        uMan.PlayerIsTargetting = true;
        string description =
            effectGroupList[currentEffectGroup].EffectsDescription;

        if (effect is DrawEffect de && de.IsDiscardEffect)
        {
            EffectTargets et = effectGroupList[currentEffectGroup].Targets;
            if (et.VariableNumber)
            {
                if (de.IsMulliganEffect)
                {
                    uMan.SetConfirmEffectButton(true);
                    description = "Choose cards to replace."; // MULLIGAN EFFECT
                }
                else description = "Discard any number of cards.";
            }
            else
            {
                int value = et.TargetNumber;
                if (value > 1) description = "Discard " + value + " cards.";
                else description = "Discard a card.";
            }
            anMan.ShiftPlayerHand(true);
            foreach (GameObject newTarget in newDrawnCards)
            {
                if (!legalTargets[currentEffectGroup].Contains(newTarget))
                    legalTargets[currentEffectGroup].Add(newTarget);
            }
            newDrawnCards.Clear();
        }
        else
        {
            if (string.IsNullOrEmpty(triggerName))
                uMan.SetCancelEffectButton(true);
            else if (triggerName == CardManager.TRIGGER_PLAY)
                uMan.SetCancelEffectButton(true);

            if (dragArrow != null)
            {
                Destroy(dragArrow);
                Debug.LogError("DRAG ARROW ALREADY EXISTS!");
            }
            dragArrow = Instantiate(coMan.DragArrowPrefab, uMan.CurrentWorldSpace.transform);

            GameObject startPoint;
            if (effectSource.TryGetComponent(out ItemIcon _))
                startPoint = coMan.PlayerHero;
            else startPoint = effectSource;
            dragArrow.GetComponent<DragArrow>().SourceCard = startPoint;
        }

        uMan.CreateInfoPopup(description);
        // Prevent targetting same object twice in a group list
        List<GameObject> redundancies = new List<GameObject>();
        foreach (GameObject target in legalTargets[currentEffectGroup])
        {
            if (target == null)
            {
                Debug.LogError("TARGET IS NULL!");
                continue;
            }
            foreach (List<GameObject> targetList in acceptedTargets)
            {
                if (targetList.Contains(target))
                    redundancies.Add(target);
            }
        }
        foreach (GameObject target in redundancies)
        {
            foreach (List<GameObject> targetList in legalTargets)
                targetList.Remove(target);
        }

        // Non-Required effects that returned TRUE from GetLegalTargets, but have 0 legal targets
        if (legalTargets[currentEffectGroup].Count < 1)
        {
            Debug.LogWarning("EFFECT CONFIRMED WITH NO LEGAL TARGETS!");
            ConfirmTargetEffect();
            return;
        }

        foreach (GameObject target in legalTargets[currentEffectGroup])
            uMan.SelectTarget(target, true);
    }

    /******
     * *****
     * ****** CHECK_LEGAL_TARGETS
     * *****
     *****/
    public bool CheckLegalTargets(List<EffectGroup> groupList,
        GameObject source, bool isPreCheck = false)
    {
        effectGroupList = groupList;
        effectSource = source;
        legalTargets = new List<List<GameObject>>();
        acceptedTargets = new List<List<GameObject>>();

        for (int i = 0; i < effectGroupList.Count; i++)
        {
            legalTargets.Add(new List<GameObject>());
            acceptedTargets.Add(new List<GameObject>());
        }

        List<EffectGroup> targetGroups = new List<EffectGroup>();
        List<int> invalidTargetGroups = new List<int>();

        foreach (EffectGroup eg in effectGroupList)
            foreach (Effect effect in eg.Effects)
                if (IsTargetEffect(eg, effect) || effect is DrawEffect)
                {
                    targetGroups.Add(eg);
                    break;
                }

        int group = 0;
        foreach (EffectGroup eg in effectGroupList)
        {
            if (targetGroups.Contains(eg))
                foreach (Effect effect in eg.Effects)
                {
                    if (!GetLegalTargets(group, effect, eg.Targets,
                        GetAdditionalTargets(eg), out bool requiredEffect))
                    {
                        invalidTargetGroups.Add(group);
                        int groupsRemaining = targetGroups.Count - invalidTargetGroups.Count;
                        Debug.Log("INVALID TARGET GROUP! <" + groupsRemaining + "/" +
                            targetGroups.Count + "> REMAINING!");
                        if (groupsRemaining < 1 || requiredEffect)
                        {
                            Debug.LogWarning("CHECK LEGAL TARGETS = FALSE!");
                            return false;
                        }
                        else break;
                    }
                }
            group++;
        }

        Debug.LogWarning("CHECK LEGAL TARGETS = TRUE");
        if (isPreCheck) ClearTargets();
        else ClearInvalids();
        return true;

        int GetAdditionalTargets(EffectGroup eg)
        {
            int additionalTargets = 0;
            foreach (EffectGroup group in effectGroupList)
                if (group.Targets.CompareTargets(eg.Targets))
                    additionalTargets++;
            if (additionalTargets > 0) additionalTargets--;
            return additionalTargets;
        }
        void ClearInvalids()
        {
            foreach (int i in invalidTargetGroups)
            {
                effectGroupList.RemoveAt(i);
                legalTargets.RemoveAt(i);
                acceptedTargets.RemoveAt(i);
            }
        }
        void ClearTargets()
        {
            effectGroupList = null;
            effectSource = null;
            legalTargets = null;
            acceptedTargets = null;
        }
    }

    /******
     * *****
     * ****** GET/CLEAR/SELECT_LEGAL_TARGETS
     * *****
     *****/
    private bool GetLegalTargets(int currentGroup, Effect effect,
        EffectTargets targets, int additionalTargets, out bool requiredEffect)
    {
        requiredEffect = effect.IsRequired;
        List<List<GameObject>> targetZones = new List<List<GameObject>>();

        if (IsPlayerSource(effectSource))
        {
            if (targets.PlayerHand) targetZones.Add(coMan.PlayerHandCards);
            if (targets.PlayerUnit) targetZones.Add(coMan.PlayerZoneCards);
            if (targets.EnemyUnit) targetZones.Add(coMan.EnemyZoneCards);
            if (targets.PlayerHero) AddTarget(coMan.PlayerHero);
            if (targets.EnemyHero) AddTarget(coMan.EnemyHero);
        }
        else
        {
            if (targets.PlayerHand) targetZones.Add(coMan.EnemyHandCards);
            if (targets.PlayerUnit) targetZones.Add(coMan.EnemyZoneCards);
            if (targets.EnemyUnit) targetZones.Add(coMan.PlayerZoneCards);
            if (targets.EnemyHero) AddTarget(coMan.PlayerHero);
            if (targets.PlayerHero) AddTarget(coMan.EnemyHero);
        }

        foreach (List<GameObject> zone in targetZones)
            foreach (GameObject target in zone)
                if (target != effectSource)
                {
                    if (coMan.IsUnitCard(target))
                    {
                        if (!unitsToDestroy.Contains(target))
                            if (coMan.GetUnitDisplay(target).CurrentHealth > 0)
                                AddTarget(target);
                    }
                    else AddTarget(target);
                }

        if (effect is DrawEffect de)
        {
            // DRAW EFFECTS
            if (!de.IsDiscardEffect)
            {
                // No cards left
                int cardsLeft = pMan.CurrentPlayerDeck.Count + coMan.PlayerDiscardCards.Count;
                if (cardsLeft < effect.Value)
                {
                    Debug.LogWarning("NO CARDS LEFT!");
                    if (requiredEffect) return false; // Unless required, draw as many as possible
                }

                // Hand is full
                int cardsAfterDraw = coMan.PlayerHandCards.Count + effect.Value; // TESTING
                if (cardsAfterDraw > GameManager.MAX_HAND_SIZE)
                {
                    Debug.LogWarning("HAND IS FULL!");
                    if (requiredEffect) return false; // Unless required, draw as many as possible
                }
            }
            // DISCARD EFFECTS
            else
            {
                // Variable target numbers
                if (targets.VariableNumber && coMan.PlayerHandCards.Count > 0) return true;
                if (coMan.PlayerHandCards.Count < effect.Value)
                {
                    Debug.LogWarning("NOT ENOUGH CARDS!");
                    if (requiredEffect) return false;
                }
            }
            return true;
        }
        Debug.Log("LEGAL TARGETS <" + (legalTargets[currentGroup].Count - additionalTargets) + ">");

        if (effect is GiveNextUnitEffect) return true;
        if (legalTargets[currentGroup].Count < 1) return false;
        if (requiredEffect &&
            legalTargets[currentGroup].Count <
            effectGroupList[currentGroup].Targets.TargetNumber + additionalTargets)
            return false;
        return true;

        void AddTarget(GameObject target) =>
            legalTargets[currentGroup].Add(target);
    }

    /******
     * *****
     * ****** SELECT_TARGET
     * *****
     *****/
    public void SelectTarget(GameObject target)
    {
        if (acceptedTargets[currentEffectGroup].Contains(target))
            RemoveEffectTarget(target);
        else if (legalTargets[currentEffectGroup].Contains(target))
            AcceptEffectTarget(target);
        else RejectEffectTarget();
    }

    /******
     * *****
     * ****** ACCEPT/REJECT_TARGET
     * *****
     *****/
    private void AcceptEffectTarget(GameObject target)
    {   
        EffectGroup eg = effectGroupList[currentEffectGroup];
        int targetNumber = eg.Targets.TargetNumber;
        int legalTargetNumber =
            legalTargets[currentEffectGroup].Count +
            acceptedTargets[currentEffectGroup].Count;

        if (!CurrentEffect.IsRequired &&
            legalTargetNumber < targetNumber)
            targetNumber = legalTargetNumber;

        int accepted = acceptedTargets[currentEffectGroup].Count;
        if (accepted == targetNumber)
        {
            if (eg.Targets.VariableNumber)
                Debug.Log("ALL TARGETS SELECTED!");
            else Debug.LogError("ERROR!");
            return;
        }
        else if (accepted > targetNumber)
        {
            
            Debug.LogError("TOO MANY ACCEPTED TARGETS!");
            return;
        }

        auMan.StartStopSound("SFX_AcceptTarget");
        uMan.SelectTarget(target, true, true);
        acceptedTargets[currentEffectGroup].Add(target);
        legalTargets[currentEffectGroup].Remove(target);

        Debug.Log("ACCEPTED TARGETS: <" +
            acceptedTargets[currentEffectGroup].Count +
            "> OF <" + targetNumber + "> REQUIRED TARGETS");

        if (eg.Targets.VariableNumber)
        {
            bool showConfirm = false;
            if (CurrentEffect is DrawEffect de &&
                de.IsMulliganEffect) showConfirm = true;
            else if (acceptedTargets[currentEffectGroup].Count > 0)
                showConfirm = true;
            uMan.SetConfirmEffectButton(showConfirm);
        }
        else if (acceptedTargets[currentEffectGroup].Count == targetNumber)
            ConfirmTargetEffect();
    }
    private void RejectEffectTarget()
    {
        uMan.CreateFleetingInfoPopup("You can't target that!");
        auMan.StartStopSound("SFX_Error");
    }

    /******
     * *****
     * ****** REMOVE_TARGET
     * *****
     *****/
    private void RemoveEffectTarget(GameObject target)
    {
        auMan.StartStopSound("SFX_AcceptTarget");
        uMan.SelectTarget(target, true, false);
        acceptedTargets[currentEffectGroup].Remove(target);
        legalTargets[currentEffectGroup].Add(target);

        EffectGroup eg = effectGroupList[currentEffectGroup];
        if (eg.Targets.VariableNumber)
        {
            if (acceptedTargets[currentEffectGroup].Count < 1)
            {
                // MULLIGAN EFFECT
                bool isMulligan;
                if (CurrentEffect is DrawEffect de &&
                    de.IsMulliganEffect) isMulligan = true;
                else isMulligan = false;
                uMan.SetConfirmEffectButton(isMulligan);
            }
        }
        Debug.Log("TARGET REMOVED!");
    }

    /******
     * *****
     * ****** CONFIRM_EFFECTS
     * *****
     *****/
    private void ConfirmNonTargetEffect()
    {
        EffectGroup eg = effectGroupList[currentEffectGroup];
        Effect effect = eg.Effects[currentEffect];
        float delay = 0;
        if (effect is DrawEffect)
        {
            string hero;
            if (eg.Targets.PlayerHand) hero = GameManager.PLAYER;
            else hero = GameManager.ENEMY;
            for (int i = 0; i < effect.Value; i++)
            {
                delay = i * 0.5f;
                FunctionTimer.Create(() => coMan.DrawCard(hero), delay);
            }
        }
        FunctionTimer.Create(() => StartNextEffect(), delay);
    }
    public void ConfirmTargetEffect()
    {
        uMan.PlayerIsTargetting = false;
        uMan.DismissInfoPopup();
        foreach (GameObject target in legalTargets[currentEffectGroup])
            uMan.SelectTarget(target, false);
        if (CurrentEffect is DrawEffect de)
        {
            if (de.IsDiscardEffect)
                anMan.ShiftPlayerHand(false);
            if (de.IsMulliganEffect)
                CardManager.Instance.ShuffleDeck(GameManager.PLAYER);
        }
        else
        {
            uMan.SetCancelEffectButton(false);
            Destroy(dragArrow);
            dragArrow = null;
        }
        StartNextEffect();
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT_GROUP_LIST
     * *****
     *****/
    private void ResolveEffectGroupList()
    {
        currentEffectGroup = 0;
        currentEffect = 0; // Unnecessary

        foreach (EffectGroup eg in effectGroupList)
            ResolveEffectGroup(eg, acceptedTargets[currentEffectGroup++]);

        // TESTING
        if (EffectRay.ActiveRays < 1) FinishEffectGroupList(false);
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT_GROUP
     * *****
     *****/
    private void ResolveEffectGroup(EffectGroup eg, List<GameObject> targets)
    {
        if (eg == null)
        {
            Debug.LogError("EFFECT GROUP IS NULL!");
            return;
        }

        bool isPowerChange = false;
        bool isHealthChange = false;
        bool isNegativeChange = false;

        foreach (Effect effect in eg.Effects)
        {
            if (effect is StatChangeEffect sce)
            {
                if (sce.IsHealthChange) isHealthChange = true;
                else isPowerChange = true;
                if (sce.IsNegative) isNegativeChange = true;
            }
            else if (effect is HealEffect) isHealthChange = true;
        }

        bool statsChanged = false;
        foreach (Effect effect in eg.Effects)
        {
            ResolveEffect(targets, effect);
            if (!statsChanged && (effect is StatChangeEffect || effect is HealEffect))
            {
                statsChanged = true;
                StatChanges(); // TESTING
            }
        }

        void StatChanges()
        {
            if (!isHealthChange && !isPowerChange) return;
            foreach (GameObject target in targets)
            {
                if (target == null)
                {
                    Debug.LogError("TARGET IS NULL!");
                    continue;
                }
                if (coMan.IsUnitCard(target))
                    anMan.UnitStatChangeState(target, isPowerChange, isHealthChange);
                else if (isHealthChange) anMan.ModifyHeroHealthState(target); // TESTING
                else
                {
                    Debug.LogError("INVALID STAT CHANGE!");
                    return;
                }

                string sound;
                if (isNegativeChange) sound = "SFX_StatMinus";
                else sound = "SFX_StatPlus";
                auMan.StartStopSound(sound);
            }
        }
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT
     * *****
     *****/
    private void ResolveEffect(List<GameObject> allTargets, Effect effect)
    {
        List<GameObject> invalidTargets = new List<GameObject>();
        List<GameObject> validTargets = new List<GameObject>();

        foreach (GameObject t in allTargets)
            if (t == null)
            {
                Debug.LogWarning("EMPTY TARGET!");
                invalidTargets.Add(t);
            }

        foreach (GameObject t in invalidTargets)
            allTargets.Remove(t);

        if (effect.IfHasAbilityCondition != null)
        {
            foreach (GameObject t in allTargets)
                if (!CardManager.GetAbility(t, effect.IfHasAbilityCondition.AbilityName))
                    invalidTargets.Add(t);
        }

        // TESTING
        if (effect.IfHasTriggerCondition != null)
        {
            foreach (GameObject t in allTargets)
                if (!CardManager.GetTrigger(t, effect.IfHasTriggerCondition.AbilityName))
                    invalidTargets.Add(t);
        }

        // TESTING
        if (effect.IfHasAbility != null)
        {
            foreach (GameObject t in allTargets)
                if (CardManager.GetAbility(t, effect.IfHasAbility.AbilityName))
                    foreach (EffectGroup eg in effect.IfHasAbilityEffects)
                        additionalEffectGroups.Add(eg);
        }

        // TESTING
        if (effect.IfHasTrigger != null)
        {
            foreach (GameObject t in allTargets)
                if (CardManager.GetTrigger(t, effect.IfHasTrigger.AbilityName))
                    foreach (EffectGroup eg in effect.IfHasTriggerEffects)
                        additionalEffectGroups.Add(eg);
        }

        // TESTING
        if (effect.ForEachEffects.Count > 0)
        {
            foreach (GameObject t in allTargets)
                foreach (EffectGroup eg in effect.ForEachEffects)
                    additionalEffectGroups.Add(eg);
        }

        // TESTING
        foreach (GameObject t in allTargets)
            if (!invalidTargets.Contains(t) && !validTargets.Contains(t)) // TESTING
                validTargets.Add(t);

        // DRAW
        if (effect is DrawEffect drE)
        {
            if (drE.IsDiscardEffect)
                foreach (GameObject target in validTargets)
                    coMan.DiscardCard(target);
        }
        // DAMAGE
        else if (effect is DamageEffect dmgE)
        {
            float delay = 0;
            foreach (GameObject target in validTargets)
            {
                // If the source is NOT a card, shoot the ray from the PLAYER HERO
                GameObject raySource = effectSource;
                if (!effectSource.TryGetComponent<CardDisplay>(out _))
                    raySource = coMan.PlayerHero;

                // TESTING
                FunctionTimer.Create(() => auMan.StartStopSound("SFX_DealDamage1"), delay);
                CreateEffectRay(raySource.transform.position, target, () => DealDamage(), delay);
                delay += 0.5f;

                // TESTING
                void DealDamage()
                {
                    if (coMan.TakeDamage(target, effect.Value))
                    {
                        foreach (EffectGroup efg in dmgE.IfDestroyedEffects)
                            additionalEffectGroups.Add(efg);
                    }
                }
            }
        }
        else if (effect is DestroyEffect)
        {
            foreach (GameObject target in validTargets)
                coMan.DestroyUnit(target);
        }
        // HEALING
        else if (effect is HealEffect)
        {
            foreach (GameObject target in validTargets)
                coMan.HealDamage(target, effect.Value);
        }
        // EXHAUST/REFRESH
        else if (effect is ExhaustEffect ee)
        {
            auMan.StartStopSound("SFX_Refresh");
            foreach (GameObject target in validTargets)
            {
                UnitCardDisplay ucd = coMan.GetUnitDisplay(target);
                if (ee.IfAlreadyExhaustedEffects != null)
                {
                    if (ucd.IsExhausted == ee.SetExhausted)
                    {
                        // TESTING
                        ResolveEffectGroup(ee.IfAlreadyExhaustedEffects,
                            new List<GameObject> { target });
                        continue;
                    }
                }
                ucd.IsExhausted = ee.SetExhausted;
            }
        }
        // REPLENISH
        else if (effect is ReplenishEffect)
        {
            int newEnergy = pMan.EnergyLeft + effect.Value;
            if (newEnergy > pMan.EnergyPerTurn) 
                newEnergy = pMan.EnergyPerTurn;
            pMan.EnergyLeft = newEnergy;
            anMan.ModifyHeroEnergyState();
        }
        // EVADE
        else if (effect is EvadeEffect)
        {
            int newRefo = enMan.NextReinforcements - effect.Value;
            if (newRefo < 0) newRefo = 0;
            enMan.NextReinforcements = newRefo;
            anMan.NextReinforcementsState();
        }
        // GIVE_NEXT_UNIT
        else if (effect is GiveNextUnitEffect gnfe)
        {
            GiveNextUnitEffect newGnfe =
                ScriptableObject.CreateInstance<GiveNextUnitEffect>();
            newGnfe.LoadEffect(gnfe);
            giveNextEffects.Add(newGnfe);
        }
        // STAT_CHANGE
        else if (effect is StatChangeEffect)
        {
            foreach (GameObject target in validTargets)
                AddEffect(target, effect);
        }
        // GIVE_ABILITY
        else if (effect is GiveAbilityEffect gae)
        {
            foreach (GameObject target in validTargets)
            {
                if (gae.IfAlreadyHasEffects != null &&
                    gae.CardAbility is StaticAbility sa)
                {
                    if (CardManager.GetAbility(target, sa.AbilityName))
                    {
                        ResolveEffectGroup(gae.IfAlreadyHasEffects,
                            new List<GameObject> { target });
                        continue;
                    }
                }
                AddEffect(target, effect);
            }
        }
        else Debug.LogError("EFFECT TYPE NOT FOUND!");
    }

    /******
     * *****
     * ****** ABORT_EFFECT_GROUP_LIST
     * *****
     *****/
    public void AbortEffectGroupList(bool isUserAbort)
    {
        if (effectSource == null)
        {
            Debug.LogError("EFFECT SOURCE IS NULL!");
            return;
        }

        string handZone;
        if (effectSource.CompareTag(CombatManager.PLAYER_CARD))
            handZone = CombatManager.PLAYER_HAND;
        else handZone = CombatManager.ENEMY_HAND;

        if (effectSource.TryGetComponent(out ActionCardDisplay acd))
        {
            if (isUserAbort)
            {
                pMan.EnergyLeft += acd.CurrentEnergyCost;
                coMan.ChangeCardZone(effectSource, handZone, true); // TESTING
                coMan.PlayerActionZoneCards.Remove(effectSource);
                coMan.PlayerHandCards.Add(effectSource);
            }
        }
        else if (effectSource.TryGetComponent(out UnitCardDisplay ucd))
        {
            if (isUserAbort)
            {
                pMan.EnergyLeft += ucd.CurrentEnergyCost;
                coMan.ChangeCardZone(effectSource, handZone, true); // TESTING
                coMan.PlayerZoneCards.Remove(effectSource);
                coMan.PlayerHandCards.Add(effectSource);
            }
            else
            {
                if (triggerName == CardManager.TRIGGER_PLAY)
                    TriggerGiveNextEffect(effectSource);
            }
        }
        else if (effectSource.CompareTag(CombatManager.PLAYER_HERO))
        {
            if (isUserAbort)
            {
                pMan.HeroPowerUsed = false;
                pMan.EnergyLeft += pMan.PlayerHero.HeroPower.PowerCost;
            }
        }
        else if (effectSource.TryGetComponent(out ItemIcon _))
        {
            // blank
        }
        else
        {
            Debug.LogError("SOURCE TYPE NOT FOUND!");
            return;
        }

        uMan.PlayerIsTargetting = false;
        if (isUserAbort) uMan.DismissInfoPopup();
        FinishEffectGroupList(true);
    }
    
    /******
     * *****
     * ****** FINISH_EFFECT_GROUP_LIST
     * *****
     *****/
    public void FinishEffectGroupList(bool wasAborted)
    {
        string debugText = "RESOLVED";
        if (wasAborted) debugText = "ABORTED";
        Debug.LogWarning("GROUP LIST FINISHED! [" + debugText + "]");

        if (!wasAborted || isAdditionalEffect)
        {
            if (!wasAborted && additionalEffectGroups.Count > 0)
            {
                GameObject source = effectSource;
                foreach (EffectGroup eg in additionalEffectGroups)
                {
                    evMan.NewDelayedAction(() =>
                    StartEffectGroupList(new List<EffectGroup> { eg },
                    source, IS_ADDITIONAL_EFFECT), 0.5f, true);
                }
            }
            else
            {
                if (effectSource.TryGetComponent<ActionCardDisplay>(out _))
                {
                    coMan.DiscardCard(effectSource, true);
                    coMan.ActionsPlayedThisTurn++;
                }
                else if (coMan.IsUnitCard(effectSource))
                {
                    if (triggerName == CardManager.TRIGGER_PLAY)
                        TriggerGiveNextEffect(effectSource);
                }
                else if (effectSource.TryGetComponent(out ItemIcon icon))
                {
                    pMan.HeroItems.Remove(icon.LoadedItem);
                    uMan.SetSkybar(true);
                }
                else if (coMan.PlayerHero == effectSource)
                {
                    CardManager.Instance.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH);
                }
            }
        }

        if (legalTargets != null)
        {
            foreach (List<GameObject> list in legalTargets)
            {
                foreach (GameObject target in list)
                {
                    if (target == null) continue;
                    uMan.SelectTarget(target, false);
                }
            }
        }
        
        if (acceptedTargets != null)
        {
            foreach (List<GameObject> list in acceptedTargets)
            {
                foreach (GameObject target in list)
                {
                    if (target == null) continue;
                    uMan.SelectTarget(target, false);
                }
            }
        }

        if (dragArrow != null)
        {
            Destroy(dragArrow);
            dragArrow = null;
        }

        newDrawnCards.Clear();
        currentEffect = 0;
        currentEffectGroup = 0;
        effectGroupList = null;
        legalTargets = null;
        acceptedTargets = null;
        effectSource = null;
        EffectsResolving = false;
    }

    /******
     * *****
     * ****** ADD_EFFECT
     * *****
     *****/
    public void AddEffect(GameObject card, Effect effect)
    {
        UnitCardDisplay ucd = coMan.GetUnitDisplay(card);

        if (effect is GiveAbilityEffect gae)
        {
            GiveAbilityEffect newGae =
                ScriptableObject.CreateInstance<GiveAbilityEffect>();
            newGae.LoadEffect(gae);
            if (!ucd.AddCurrentAbility(newGae.CardAbility, true))
            {
                // If ability is static and already exists
                // Update countdown instead of adding
                if (newGae.CardAbility is StaticAbility) { }
                else
                {
                    Debug.LogError("ABILITY NOT FOUND!");
                    return;
                }

                foreach (Effect cEffect in ucd.CurrentEffects)
                    if (cEffect is GiveAbilityEffect cGae)
                        if (cGae.CardAbility.AbilityName == newGae.CardAbility.AbilityName)
                        {
                            if ((newGae.Countdown == 0) || (cGae.Countdown != 0 && newGae.Countdown > cGae.Countdown))
                                        cGae.Countdown = newGae.Countdown;
                        }               
            }
            else ucd.CurrentEffects.Add(newGae);
        }
        else if (effect is StatChangeEffect sce)
        {
            if (ucd.CurrentHealth < 1) return; // Don't change stats of destroyed units
            StatChangeEffect newSce =
                ScriptableObject.CreateInstance<StatChangeEffect>();
            newSce.LoadEffect(sce);
            ucd.CurrentEffects.Add(newSce);
            int statChange = newSce.Value;
            if (sce.IsNegative) statChange = -statChange;
            if (sce.IsHealthChange)
            {
                ucd.MaxHealth += statChange;
                ucd.CurrentHealth += statChange;
            }
            else ucd.CurrentPower += statChange;
        }
        else
        {
            Debug.LogError("EFFECT TYPE NOT FOUND!");
            return;
        }
    }

    /******
     * *****
     * ****** REMOVE_TEMPORARY_EFFECTS
     * *****
     *****/
    public void RemoveTemporaryEffects(string hero)
    {
        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }
        List<GameObject> cardZone;
        if (hero == GameManager.PLAYER) cardZone = coMan.PlayerZoneCards;
        else cardZone = coMan.EnemyZoneCards;
        foreach (GameObject card in cardZone)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(card);
            List<Effect> expiredEffects = new List<Effect>();

            bool isPowerChange = false;
            bool isHealthChange = false;

            foreach (Effect effect in ucd.CurrentEffects)
            {
                if (effect.Countdown == 1) // Check for EXPIRED effects
                {
                    Debug.LogWarning("EFFECT REMOVED: <" + effect.ToString() + ">");
                    // GIVE_ABILITY_EFFECT
                    if (effect is GiveAbilityEffect gae)
                        ucd.RemoveCurrentAbility(gae.CardAbility.AbilityName);
                    // STAT_CHANGE_EFFECT
                    else if (effect is StatChangeEffect sce)
                    {
                        int statChange = sce.Value;
                        if (sce.IsNegative) statChange = -statChange;
                        if (sce.IsHealthChange)
                        {
                            ucd.CurrentHealth -= statChange;
                            ucd.MaxHealth -= statChange;
                            isHealthChange = true;
                        }
                        else
                        {
                            ucd.CurrentPower -= statChange;
                            isPowerChange = true;
                        }
                    }
                    expiredEffects.Add(effect);
                }
                else if (effect.Countdown != 0)
                {
                    effect.Countdown -= 1;
                    Debug.LogWarning("COUNTOWN FOR " + 
                        effect.ToString() + " is <" + effect.Countdown + ">");
                }
            }
            anMan.UnitStatChangeState(card, isPowerChange, isHealthChange);

            foreach (Effect effect in expiredEffects)
            {
                ucd.CurrentEffects.Remove(effect);
                DestroyEffect(effect);
            }
            ucd = null;
            expiredEffects = null;
        }
    }

    /******
     * *****
     * ****** TRIGGER_GIVE_NEXT_EFFECT
     * *****
     *****/
    public void TriggerGiveNextEffect(GameObject card)
    {
        List<GiveNextUnitEffect> resolvedGnue = new List<GiveNextUnitEffect>();
        if (GiveNextEffects.Count > 0)
        {
            bool isPowerChange = false;
            bool isHealthChange = false;

            List<GameObject> target = new List<GameObject> { card };
            foreach (GiveNextUnitEffect gnue in GiveNextEffects)
            {
                foreach (Effect e in gnue.Effects)
                {
                    if (e is StatChangeEffect sce)
                    {
                        if (sce.IsHealthChange) isHealthChange = true;
                        else isPowerChange = true;
                    }
                    ResolveEffect(target, e);
                }
                if (--gnue.Multiplier < 1) resolvedGnue.Add(gnue);
            }

            anMan.UnitStatChangeState(target[0], isPowerChange, isHealthChange);
            foreach (GiveNextUnitEffect rGnue in resolvedGnue)
            {
                GiveNextEffects.Remove(rGnue);
                DestroyEffect(rGnue);
            }
        }

        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }
    }

    /******
     * *****
     * ****** REMOVE_GIVE_NEXT_EFFECTS
     * *****
     *****/
    public void RemoveGiveNextEffects()
    {
        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }
        List<GiveNextUnitEffect> expiredGne = new List<GiveNextUnitEffect>();
        foreach (GiveNextUnitEffect gnfe in GiveNextEffects)
            if (gnfe.Countdown == 1) expiredGne.Add(gnfe);
            else if (gnfe.Countdown != 0) gnfe.Countdown -= 1;
        foreach (GiveNextUnitEffect xGnfe in expiredGne)
        {
            GiveNextEffects.Remove(xGnfe);
            DestroyEffect(xGnfe);
        }
    }

    /******
     * *****
     * ****** CREATE_EFFECT_RAY
     * *****
     *****/
    public void CreateEffectRay(Vector2 start, GameObject target, System.Action rayEffect, float delay = 0, bool isEffectGroup = true)
    {
        GameObject ray = Instantiate(effectRay, start, Quaternion.identity);
        ray.transform.SetParent(uMan.CurrentWorldSpace.transform);

        if (delay == 0) SetRay();
        else FunctionTimer.Create(() => SetRay(), delay);

        void SetRay()
        {
            ray.GetComponent<EffectRay>().SetEffectRay(target, rayEffect, isEffectGroup);
        }
    }
}
