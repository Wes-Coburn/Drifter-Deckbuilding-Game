using System.Collections.Generic;
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

    private PlayerManager pMan;
    private EnemyManager enMan;
    private CombatManager coMan;
    private UIManager uMan;
    private AudioManager auMan;
    private AnimationManager anMan;
    private EventManager evMan;
    private CardManager caMan;

    private GameObject effectSource;
    private bool effectsResolving;
    private bool isAdditionalEffect;
    private string triggerName;
    private int currentEffectGroup;
    private int currentEffect;
    private int activeEffects;
    private GameObject dragArrow;

    private List<EffectGroup> effectGroupList;
    private List<List<GameObject>> legalTargets;
    private List<List<GameObject>> acceptedTargets;
    private List<List<Card>> acceptedTargets_Cards;
    private List<EffectGroup> additionalEffectGroups;

    private List<GameObject> unitsToDestroy;

    private List<GiveNextUnitEffect> giveNextEffects_Player;
    private List<ChangeCostEffect> changeNextCostEffects_Player;

    private List<GiveNextUnitEffect> giveNextEffects_Enemy;
    private List<ChangeCostEffect> changeNextCostEffects_Enemy;

    [Header("Effect Ray"), SerializeField] private GameObject effectRay;
    [Header("Ray Colors"), SerializeField] private Color damageRayColor;
    [SerializeField] private Color healRayColor;
    [Header("Regeneration Effect"), SerializeField] private Effect regenerationEffect;
    [Header("Poison Effect"), SerializeField] private Effect poisonEffect;
    [SerializeField] private CardAbility poisonAbility;

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
    public bool EffectsResolving
    {
        get
        {
            if (EffectRay.ActiveRays > 0) return true; // TESTING
            return effectsResolving;
        }
        set
        {
            effectsResolving = value;
            evMan.PauseDelayedActions(value);
            uMan.UpdateEndTurnButton(!value);
        }
    }
    public Effect CurrentEffect
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
                Debug.LogError("CURRENT GROUP > GROUP LIST");
                return null;
            }
            else
            {
                List<Effect> effects = effectGroupList[currentEffectGroup].Effects;
                if (currentEffect > effects.Count - 1)
                {
                    Debug.LogError("CURRENT EFFECT > EFFECT LIST");
                    return null;
                }
                else return effects[currentEffect];
            }
        }
    }

    public List<GameObject> UnitsToDestroy { get => unitsToDestroy; }
    public List<GiveNextUnitEffect> GiveNextEffects_Player { get => giveNextEffects_Player; }
    public List<ChangeCostEffect> ChangeNextCostEffects_Player { get => changeNextCostEffects_Player; }
    public List<GiveNextUnitEffect> GiveNextEffects_Enemy { get => giveNextEffects_Enemy; }
    public List<ChangeCostEffect> ChangeNextCostEffects_Enemy { get => changeNextCostEffects_Enemy; }
    public Color DamageRayColor { get => damageRayColor; }
    public Effect PoisonEffect { get => poisonEffect; }
    public Effect RegenerationEffect { get => regenerationEffect; }
    public CardAbility PoisonAbility { get => poisonAbility; }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;
        anMan = AnimationManager.Instance;
        evMan = EventManager.Instance;
        caMan = CardManager.Instance;

        giveNextEffects_Player = new List<GiveNextUnitEffect>();
        changeNextCostEffects_Player = new List<ChangeCostEffect>();
        giveNextEffects_Enemy = new List<GiveNextUnitEffect>();
        changeNextCostEffects_Enemy = new List<ChangeCostEffect>();
        unitsToDestroy = new List<GameObject>();
        additionalEffectGroups = new List<EffectGroup>();
        effectsResolving = false;
    }

    public void Reset_EffectManager() => effectsResolving = false;
    public void ClearDestroyedUnits()
    {
        List<GameObject> newList = new List<GameObject>();
        foreach (GameObject unit in UnitsToDestroy)
            if (unit != null) newList.Add(unit);

        UnitsToDestroy.Clear();
        foreach (GameObject unit in newList)
            UnitsToDestroy.Add(unit);
    }
    public bool IsPlayerSource(GameObject source)
    {
        if (source == null)
        {
            Debug.LogError("SOURCE IS NULL!");
            return false;
        }

        if (string.IsNullOrEmpty(source.tag))
            Debug.LogWarning("SOURCE HAS NO TAG!");

        if (source.CompareTag(CombatManager.ENEMY_CARD) ||
            source.CompareTag(CombatManager.ENEMY_HERO) ||
            source.CompareTag(CombatManager.ENEMY_HERO_POWER))
            return false;
        else return true;
    }
    private bool IsTargetEffect(EffectGroup group, Effect effect)
    {
        EffectTargets targets = group.Targets;

        if (targets.NoTargets) return false; // TESTING

        if (effect is DrawEffect de && de.IsDiscardEffect && de.DiscardAll) return false; // TESTING

        //else if (effect is AcquireItemEffect) return false; // CHANGE TO <NO TARGETS>

        //else if (effect is GiveNextUnitEffect) return false; // CHANGE TO <NO TARGETS>

        //else if (effect is ChangeCostEffect cce && cce.ChangeNextCost) return false; // CHANGE TO <NO TARGETS>

        //else if (effect is CreateCardEffect || effect is PlayCardEffect) return false; // CHANGE TO <NO TARGETS>

        else if (targets.TargetsAll || targets.TargetsSelf) return false;

        else if (targets.PlayerDeck) return false; // TESTING

        else if (targets.PlayerHero || targets.EnemyHero)
        {
            if (!targets.PlayerUnit && !targets.EnemyUnit) return false;
        }

        else if (targets.TargetsLowestHealth || targets.TargetsStrongest || targets.TargetsWeakest) return false;

        return true;
    }

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
            evMan.NewDelayedAction(() =>
            StartEffectGroupList(groupList, source, triggerName), 0.1f, true); // TESTING
            return;
        }

        EffectsResolving = true;
        effectSource = source;
        this.triggerName = triggerName;
        currentEffectGroup = 0;
        currentEffect = 0;
        effectGroupList = new List<EffectGroup>();

        // REMOVE EMPTY EFFECTS
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

            foreach (Effect e in emptyEffects) eg.Effects.Remove(e);
            effectGroupList.Add(eg);
        }

        if (effectGroupList.Count < 1)
        {
            Debug.LogError("EMPTY EFFECT GROUP LIST!");
            EffectsResolving = false;
            return;
        }

        // ADDITIONAL EFFECTS
        if (additionalEffectGroups.Count > 0)
        {
            isAdditionalEffect = true;
            additionalEffectGroups.RemoveAt(0);
        }
        else isAdditionalEffect = false;

        // UNIT ABILITY TRIGGER
        if (effectSource.TryGetComponent(out UnitCardDisplay ucd))
        {
            if (!isAdditionalEffect && !string.IsNullOrEmpty(triggerName))
                ucd.AbilityTriggerState(triggerName);
        }

        // CHECK LEGAL TARGETS
        if (!CheckLegalTargets(effectGroupList, effectSource)) AbortEffectGroupList(false);
        else if (!isAdditionalEffect)
        {
            // RESOLVE INDEPENDENT
            List<EffectGroup> resolveIndependent = new List<EffectGroup>();
            foreach (EffectGroup eg in effectGroupList)
            {
                if (eg.ResolveIndependent)
                {
                    resolveIndependent.Add(eg);
                    additionalEffectGroups.Add(eg);
                }
            }
            foreach (EffectGroup eg in resolveIndependent) effectGroupList.Remove(eg);

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
            Debug.Log("[EFFECT GROUP #" + (currentEffectGroup + 1) +
                "] <" + effectGroupList[currentEffectGroup].ToString() + ">");
            StartNextEffect(true);
        }

        else if (currentEffectGroup == effectGroupList.Count)
            ResolveEffectGroupList();

        else Debug.LogError("EffectGroup > GroupList!");
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

        if (currentEffect == eg.Effects.Count) StartNextEffectGroup();
        else if (currentEffect < eg.Effects.Count)
        {
            Debug.Log("[EFFECT #" + (currentEffect + 1) +
                "] <" + eg.Effects[currentEffect].ToString() + ">");

            Effect effect = eg.Effects[currentEffect];
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
        EffectTargets et = effectGroupList[currentEffectGroup].Targets;
        List<GameObject> targets = acceptedTargets[currentEffectGroup];
        List<Card> targets_Cards = acceptedTargets_Cards[currentEffectGroup]; // TESTING
        bool isPlayerSource = IsPlayerSource(effectSource);

        if (et.NoTargets)
        {
            ConfirmNonTargetEffect(); // TESTING
            return;
        }

        if (et.TargetsSelf) AddTarget(effectSource);

        if (isPlayerSource)
        {
            if (et.PlayerHero) AddTarget(coMan.PlayerHero);
            if (et.EnemyHero) AddTarget(coMan.EnemyHero);
            if (et.TargetsAll)
            {
                if (et.PlayerUnit) AddAllTargets(coMan.PlayerZoneCards);
                if (et.EnemyUnit) AddAllTargets(coMan.EnemyZoneCards);
                if (et.PlayerHand) AddAllTargets(coMan.PlayerHandCards);
                if (et.PlayerDeck) AddAllTargets_Cards(pMan.CurrentPlayerDeck); // TESTING
            }
            if (et.TargetsLowestHealth)
            {
                if (et.PlayerUnit) AddLowHealthUnit(coMan.PlayerZoneCards, false);
                if (et.EnemyUnit) AddLowHealthUnit(coMan.EnemyZoneCards, true);
            }
            if (et.TargetsStrongest)
            {
                if (et.PlayerUnit) AddStrongestUnit(coMan.PlayerZoneCards, false);
                if (et.EnemyUnit) AddStrongestUnit(coMan.EnemyZoneCards, true);
            }
            if (et.TargetsWeakest)
            {
                if (et.PlayerUnit) AddWeakestUnit(coMan.PlayerZoneCards, false);
                if (et.EnemyUnit) AddWeakestUnit(coMan.EnemyZoneCards, true);
            }
        }
        else
        {
            if (et.EnemyHero) AddTarget(coMan.PlayerHero);
            if (et.PlayerHero) AddTarget(coMan.EnemyHero);
            if (et.TargetsAll)
            {
                if (et.PlayerUnit) AddAllTargets(coMan.EnemyZoneCards);
                if (et.EnemyUnit) AddAllTargets(coMan.PlayerZoneCards);
                if (et.PlayerHand) AddAllTargets(coMan.EnemyHandCards);
                if (et.PlayerDeck) AddAllTargets_Cards(enMan.CurrentEnemyDeck); // TESTING
            }
            if (et.TargetsLowestHealth)
            {
                if (et.PlayerUnit) AddLowHealthUnit(coMan.EnemyZoneCards, false);
                if (et.EnemyUnit) AddLowHealthUnit(coMan.PlayerZoneCards, true);
            }
            if (et.TargetsStrongest)
            {
                if (et.PlayerUnit) AddStrongestUnit(coMan.EnemyZoneCards, false);
                if (et.EnemyUnit) AddStrongestUnit(coMan.PlayerZoneCards, true);
            }
            if (et.TargetsWeakest)
            {
                if (et.PlayerUnit) AddWeakestUnit(coMan.EnemyZoneCards, false);
                if (et.EnemyUnit) AddWeakestUnit(coMan.PlayerZoneCards, true);
            }
        }

        // CONFIRM NON TARGET EFFECT
        ConfirmNonTargetEffect();

        void AddAllTargets(List<GameObject> cardZone)
        {
            foreach (GameObject card in cardZone)
                AddTarget(card);
        }

        void AddAllTargets_Cards(List<Card> cardZone)
        {
            foreach (Card card in cardZone)
                AddTarget_Card(card);
        }

        void AddLowHealthUnit(List<GameObject> cardZone, bool targetsEnemy)
        {
            if (targets.Count > 0) return; // TESTING

            GameObject target = coMan.GetLowestHealthUnit(cardZone, targetsEnemy);
            if (target != null) AddTarget(target);
        }

        void AddWeakestUnit(List<GameObject> cardZone, bool targetsEnemy)
        {
            if (targets.Count > 0) return; // TESTING

            GameObject target = coMan.GetWeakestUnit(cardZone, targetsEnemy);
            if (target != null) AddTarget(target);
        }

        void AddStrongestUnit(List<GameObject> cardZone, bool targetsEnemy)
        {
            if (targets.Count > 0) return; // TESTING

            GameObject target = coMan.GetStrongestUnit(cardZone, targetsEnemy);
            if (target != null) AddTarget(target);
        }

        void AddTarget(GameObject target)
        {
            if (targets.Contains(target)) return;

            if (isPlayerSource != IsPlayerSource(target))
            {
                if (target.TryGetComponent(out DragDrop dd) && dd.IsPlayed)
                {
                    if (CardManager.GetAbility(target,
                        CardManager.ABILITY_WARD)) return;
                }
            }
            targets.Add(target);
        }

        void AddTarget_Card(Card target)
        {
            if (targets_Cards.Contains(target)) return;
            targets_Cards.Add(target);
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
        Effect effect = CurrentEffect;

        if (acceptedTargets[currentEffectGroup].Count > 0)
        {
            StartNextEffectGroup();
            return;
        }

        // Non-Required effects that returned TRUE from GetLegalTargets, but have 0 legal targets
        if (effect is DrawEffect de && de.IsDiscardEffect) { }
        else if (legalTargets[currentEffectGroup].Count < 1)
        {
            Debug.LogWarning("EFFECT CONFIRMED WITH NO LEGAL TARGETS!");
            ConfirmTargetEffect();
            return;
        }

        // Prevent targetting same object twice in an EFFECT GROUP LIST
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


        // ENEMY BEHAVIOR
        if (!IsPlayerSource(effectSource))
        {
            EffectTargets targets = effectGroupList[currentEffectGroup].Targets;
            List<GameObject> availableTargets = legalTargets[currentEffectGroup];
            List<GameObject> confirmedTargets = acceptedTargets[currentEffectGroup];
            List<GameObject> priorityTargets = new List<GameObject>();

            bool isNegativeEffect = true;
            if (effect is HealEffect) isNegativeEffect = false;

            foreach (GameObject t in availableTargets)
            {
                if (IsPlayerSource(t))
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
                        target = coMan.GetStrongestUnit(priorityTargets, true);
                    else if (targets.PlayerUnit && (!targets.PlayerHero))
                        target = coMan.GetStrongestUnit(priorityTargets, false);
                    else target = GetRandomTarget(priorityTargets);
                    priorityTargets.Remove(target);
                }
                else target = availableTargets[Random.Range(0, availableTargets.Count)];

                availableTargets.Remove(target);
                confirmedTargets.Add(target);
            }

            ConfirmTargetEffect();
            return;

            static GameObject GetRandomTarget(List<GameObject> targets) =>
                targets[Random.Range(0, targets.Count)];
        }

        // PLAYER TARGETTING
        uMan.PlayerIsTargetting = true;
        string description = effectGroupList[currentEffectGroup].EffectsDescription;
        EffectTargets et = effectGroupList[currentEffectGroup].Targets;

        if (effect is DrawEffect de3 && de3.IsDiscardEffect)
        {
            anMan.ShiftPlayerHand(true);
            if (et.VariableNumber)
            {
                if (et.AllowZero) uMan.SetConfirmEffectButton(true);
                else uMan.SetCancelEffectButton(true); // TESTING

                if (de3.IsMulliganEffect) description = "Choose cards to redraw."; // "Redraw" or "Replace"
                else if (et.TargetNumber < 8)
                {
                    string card = "card";
                    if (et.TargetNumber > 1) card = "cards";
                    description = "Discard up to " + et.TargetNumber + " " + card + ".";
                }
                else description = "Discard any number of cards.";
            }
            else
            {
                int value = et.TargetNumber;
                if (value > 1) description = "Discard " + value + " cards.";
                else description = "Discard a card.";
            }
        }
        else
        {
            if (effect is ChangeCostEffect ||
                (effect is CopyCardEffect cpyCrd && !cpyCrd.PlayCopy))
                anMan.ShiftPlayerHand(true);

            if (string.IsNullOrEmpty(triggerName))
                uMan.SetCancelEffectButton(true);
            else if (triggerName == CardManager.TRIGGER_PLAY)
                uMan.SetCancelEffectButton(true);

            if (dragArrow != null)
            {
                Destroy(dragArrow);
                Debug.LogError("DRAG ARROW ALREADY EXISTS!");
            }
            dragArrow = Instantiate(caMan.DragArrowPrefab, uMan.CurrentWorldSpace.transform);

            GameObject startPoint;
            if (effectSource.TryGetComponent(out ItemIcon _)) startPoint = coMan.PlayerHero;
            else startPoint = effectSource;
            dragArrow.GetComponent<DragArrow>().SourceCard = startPoint;
        }

        uMan.CreateInfoPopup(description, UIManager.InfoPopupType.Default);

        foreach (GameObject card in coMan.PlayerHandCards)
            if (!legalTargets[currentEffectGroup].Contains(card))
            {
                uMan.SelectTarget(card, UIManager.SelectionType.Disabled);
            }

        foreach (GameObject target in legalTargets[currentEffectGroup])
            uMan.SelectTarget(target, UIManager.SelectionType.Highlighted);
    }

    /******
     * *****
     * ****** CHECK_LEGAL_TARGETS
     * *****
     *****/
    public bool CheckLegalTargets(List<EffectGroup> groupList, GameObject source, bool isPreCheck = false)
    {
        effectGroupList = groupList;
        effectSource = source;
        legalTargets = new List<List<GameObject>>();
        acceptedTargets = new List<List<GameObject>>();
        acceptedTargets_Cards = new List<List<Card>>();

        for (int i = 0; i < effectGroupList.Count; i++)
        {
            legalTargets.Add(new List<GameObject>());
            acceptedTargets.Add(new List<GameObject>());
            acceptedTargets_Cards.Add(new List<Card>());
        }

        List<int> invalidGroups = new List<int>();
        int group = 0;

        foreach (EffectGroup eg in effectGroupList)
        {
            if (eg == null)
            {
                Debug.LogError("GROUP IS NULL!");
                continue;
            }

            foreach (Effect effect in eg.Effects)
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
                    //Debug.Log("INVALID TARGET GROUP! <" + groupsRemaining + "/" + effectGroupList.Count + "> REMAINING!");

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
            foreach (EffectGroup group in effectGroupList)
                if (group.Targets.CompareTargets(eg.Targets)) additionalTargets++;
            if (additionalTargets > 0) additionalTargets--;
            return additionalTargets;
        }
        void ClearInvalids()
        {
            foreach (int i in invalidGroups)
            {
                if (effectGroupList.Count > i) effectGroupList.RemoveAt(i);
                if (legalTargets.Count > i) legalTargets.RemoveAt(i);
                if (acceptedTargets.Count > i) acceptedTargets.RemoveAt(i);
                if (acceptedTargets_Cards.Count > i) acceptedTargets_Cards.RemoveAt(i);
            }
        }
        void ClearTargets()
        {
            effectGroupList = null;
            effectSource = null;
            legalTargets = null;
            acceptedTargets = null;
            acceptedTargets_Cards = null;
        }
    }

    /******
     * *****
     * ****** GET/CLEAR/SELECT_LEGAL_TARGETS
     * *****
     *****/
    private bool GetLegalTargets(int currentGroup, Effect effect,
        EffectTargets targets, int additionalTargets, out bool requiredEffect, bool isPreCheck)
    {
        requiredEffect = effect.IsRequired;
        if (targets.PlayerDeck) return true; // TESTING
        if (effect is ReplenishEffect || effect is GiveNextUnitEffect) return true;
        if (effect is ChangeCostEffect cce && cce.ChangeNextCost) return true;
        bool isPlayerSource = IsPlayerSource(effectSource);

        if (targets.NoTargets && effect.PreCheckConditions)
        {
            Debug.LogError("EFFECTS WITH NO TARGETS CANNOT PRECHECK CONDITIONS!");
            return false;
        }

        if (!targets.NoTargets) // TESTING
        {
            List<List<GameObject>> targetZones = new List<List<GameObject>>();

            if (targets.TargetsSelf) AddTarget(effectSource);
            if (isPlayerSource)
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
                    AddTarget(target);

            // PRECHECK CONDITIONS
            if (effect.PreCheckConditions)
            {
                // CHECK CONDITIONS INDEPENDENT
                if (effect.CheckConditionsIndependent && !isAdditionalEffect) { }
                else legalTargets[currentGroup] = GetValidTargets(effect, legalTargets[currentGroup]);
            }
        }

        if (effect is DrawEffect de)
        {
            int handCount;
            int discardCount;
            int deckCount;

            if (isPlayerSource)
            {
                handCount = coMan.PlayerHandCards.Count;
                discardCount = coMan.PlayerDiscardCards.Count;
                deckCount = pMan.CurrentPlayerDeck.Count;
            }
            else
            {
                handCount = coMan.EnemyHandCards.Count;
                discardCount = coMan.EnemyDiscardCards.Count;
                deckCount = enMan.CurrentEnemyDeck.Count;
            }

            // DRAW EFFECTS
            if (!de.IsDiscardEffect)
            {
                int cardsLeft = deckCount + discardCount;
                if (cardsLeft < effect.Value) return false;

                int cardsAfterDraw = handCount + effect.Value;

                if (isPreCheck)
                {
                    // If this is a pre-check for actions, account for the card in HAND
                    if (coMan.IsActionCard(effectSource)) cardsAfterDraw--;
                }
                if (cardsAfterDraw > GameManager.MAX_HAND_SIZE) return false;

                if (targets.NoTargets) return true; // TESTING
            }

            // DISCARD EFFECTS
            else
            {
                if (targets.VariableNumber && targets.AllowZero) return true;

                if (isPreCheck)
                {
                    // If this is a pre-check for actions, account for the card in HAND
                    if (coMan.IsActionCard(effectSource)) handCount--;
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
        else if (effect is CreateCardEffect || (effect is CopyCardEffect cpyCrd && !cpyCrd.PlayCopy))
        {
            List<GameObject> handCards;
            if (isPlayerSource) handCards = coMan.PlayerHandCards;
            else handCards = coMan.EnemyHandCards;

            int cardsAfterDraw = handCards.Count + 1;
            if (isPreCheck)
            {
                // If this is a pre-check for actions, account for the card in HAND
                if (coMan.IsActionCard(effectSource)) cardsAfterDraw--;
            }
            if (cardsAfterDraw > GameManager.MAX_HAND_SIZE)
            {
                Debug.Log("HAND IS FULL!");
                if (requiredEffect) return false; // Unless required, create as many as possible
            }
            return true;
        }
        else if (effect is PlayCardEffect || (effect is CopyCardEffect cpyCrd2 && cpyCrd2.PlayCopy))
        {
            PlayCardEffect pce = null;
            if (effect is PlayCardEffect) pce = effect as PlayCardEffect;

            int unitCount;
            if (isPlayerSource)
            {
                if (pce != null && pce.EnemyCard) unitCount = coMan.EnemyZoneCards.Count; // TESTING
                else unitCount = coMan.PlayerZoneCards.Count;
            }
            else
            {
                if (pce != null && pce.EnemyCard) unitCount = coMan.PlayerZoneCards.Count; // TESTING
                else unitCount = coMan.EnemyZoneCards.Count;
            }

            if (unitCount >= GameManager.MAX_UNITS_PLAYED) return false;
            return true;
        }
        else if (effect is ChangeControlEffect)
        {
            if (triggerName == CardManager.TRIGGER_TURN_END) return true; // Always resolve control-returning effects (excess units are destroyed)

            if (isPlayerSource)
            {
                if (coMan.PlayerZoneCards.Count >= GameManager.MAX_UNITS_PLAYED) return false;
            }
            else if (coMan.EnemyZoneCards.Count >= GameManager.MAX_UNITS_PLAYED) return false;
        }
        else if (effect is AcquireItemEffect)
        {
            if (pMan.HeroItems.Count >= GameManager.MAXIMUM_ITEMS) return false;
            else return true;
        }

        if (legalTargets[currentGroup].Count < 1) return false;
        if (requiredEffect && legalTargets[currentGroup].Count <
            effectGroupList[currentGroup].Targets.TargetNumber + additionalTargets)
            return false;
        return true;

        void AddTarget(GameObject target)
        {
            if (isPlayerSource != IsPlayerSource(target))
            {
                if (target.TryGetComponent(out DragDrop dd) && dd.IsPlayed)
                {
                    if (CardManager.GetAbility(target, CardManager.ABILITY_WARD)) return;
                }
            }

            bool includeSelf = false;
            if (targets.TargetsSelf ||
                targets.TargetsWeakest ||
                targets.TargetsStrongest ||
                targets.TargetsLowestHealth) includeSelf = true;
            if (target == effectSource && !includeSelf) return;

            bool isUnit = coMan.IsUnitCard(target);
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
            else if (effect is GiveAbilityEffect gae && gae.RandomPositiveKeyword)
            {
                bool isValid = false;
                foreach (CardAbility posKey in caMan.GeneratableKeywords)
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
                if (coMan.GetUnitDisplay(target).CurrentHealth < 1) return;
                if (unitsToDestroy.Contains(target)) return;
            }

            List<GameObject> targetList = legalTargets[currentGroup];
            if (!targetList.Contains(target)) targetList.Add(target);
        }
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
            foreach (GameObject t in acceptedTargets[currentEffectGroup])
                foreach (EffectGroup group in CurrentEffect.ForEachEffects)
                    additionalEffectGroups.Add(group);
        }

        StartNextEffect();
    }
    public void ConfirmTargetEffect()
    {
        uMan.PlayerIsTargetting = false;
        uMan.DismissInfoPopup();
        uMan.SetCancelEffectButton(false); // TESTING

        foreach (GameObject target in legalTargets[currentEffectGroup])
            uMan.SelectTarget(target, UIManager.SelectionType.Disabled);
        
        // FOR_EACH_EFFECTS
        if (CurrentEffect.ForEachEffects.Count > 0)
        {
            foreach (GameObject t in acceptedTargets[currentEffectGroup])
                foreach (EffectGroup group in CurrentEffect.ForEachEffects)
                    additionalEffectGroups.Add(group);
        }
        
        if (CurrentEffect is DrawEffect de)
        {
            if (de.IsDiscardEffect) anMan.ShiftPlayerHand(false);
            if (de.IsMulliganEffect) caMan.ShuffleDeck(GameManager.PLAYER);
        }
        else
        {
            if (CurrentEffect is ChangeCostEffect || CurrentEffect is CopyCardEffect)
                anMan.ShiftPlayerHand(false);

            //uMan.SetCancelEffectButton(false);
            
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
     * ****** RESOLVE_EFFECT_GROUP_LIST
     * *****
     *****/
    private void ResolveEffectGroupList()
    {
        //Debug.Log("RESOLVE EFFECT GROUP LIST!");

        currentEffectGroup = 0;
        currentEffect = 0; // Unnecessary

        float delay = 0;
        if (triggerName != null && triggerName != CardManager.TRIGGER_PLAY) delay = 0.25f;

        foreach (EffectGroup eg in effectGroupList)
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
        Debug.Log("RESOLVE EFFECT GROUP <" + eg.ToString() + ">");

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
            targetList = new List<GameObject>();
            foreach (GameObject t in targets) targetList.Add(t);
        }
        if (targets_Cards != null)
        {
            targetList_Cards = new List<Card>();
            foreach (Card t in targets_Cards) targetList_Cards.Add(t);
        }

        foreach (Effect effect in eg.Effects)
        {
            if (effect is DelayEffect de)
            {
                newDelay += de.DelayValue;
                continue;
            }

            bool shootRay = false;
            if (!(eg.Targets.TargetsSelf && !eg.Targets.TargetsAll))
            {
                if (effect is DamageEffect ||
                    effect is DestroyEffect ||
                    effect is HealEffect ||
                    effect.ShootRay) shootRay = true;
            }

            if (targetList_Cards != null) // TESTING
            {
                ActiveEffects++;
                FunctionTimer.Create(() =>
                ResolveEffect_Cards(targetList_Cards, effect, 0, out _), newDelay);
            }
            else if (shootRay) ResolveEffect(targetList, effect, shootRay, newDelay, out newDelay);
            else
            {
                ActiveEffects++;
                FunctionTimer.Create(() =>
                ResolveEffect(targetList, effect, shootRay, 0, out _), newDelay);
            }

            if (effect is DrawEffect || effect is GiveNextUnitEffect || (effect is ChangeCostEffect cce && cce.ChangeNextCost)) { }
            else newDelay += 0.5f;
        }
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT
     * *****
     *****/
    private void ResolveEffect_Cards(List<Card> allTargets, Effect effect, float delay, out float newDelay)
    {
        Debug.Log("RESOLVE EFFECT <" + effect.ToString() + ">");
        newDelay = delay;

        if (effect is StatChangeEffect sce)
        {
            foreach (Card target in allTargets)
            {
                if (target is UnitCard uc) { }
                else continue;

                StatChangeEffect newSce = ScriptableObject.CreateInstance<StatChangeEffect>();
                newSce.LoadEffect(sce);

                if (newSce.DoublePower) newSce.PowerChange = uc.CurrentPower;
                if (newSce.DoubleHealth) newSce.HealthChange = uc.CurrentHealth;

                uc.CurrentPower += newSce.PowerChange;
                uc.MaxHealth += newSce.HealthChange;
                uc.CurrentHealth += newSce.HealthChange;

                // Can't add to current effects without CardDisplay, move CurrentEffects to Card?
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
        Debug.Log("RESOLVE EFFECT <" + effect.ToString() + ">");
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
        
        if (effect is DrawEffect dre && !dre.IsDiscardEffect || effect is CreateCardEffect || effect is PlayCardEffect)
        {
            validTargets = GetValidTargets(effect, new List<GameObject> { effectSource });
            if (validTargets.Count < 1) isValidEffect = false;
        }
        
        validTargets = GetValidTargets(effect, allTargets); // TESTING

        /******
        * *****
        * ****** CONDITIONAL EFFECTS
        * *****
        *****/
        // IF_HAS_ABILITY_EFFECTS
        if (effect.IfHasAbility != null)
        {
            foreach (GameObject t in validTargets)
                if (CardManager.GetAbility(t, effect.IfHasAbility.AbilityName))
                    foreach (EffectGroup eg in effect.IfHasAbilityEffects)
                        additionalEffectGroups.Add(eg);
        }
        // IF_HAS_TRIGGER_EFFECTS
        if (effect.IfHasTrigger != null)
        {
            foreach (GameObject t in validTargets)
                if (CardManager.GetTrigger(t, effect.IfHasTrigger.AbilityName))
                    foreach (EffectGroup eg in effect.IfHasTriggerEffects)
                        additionalEffectGroups.Add(eg);
        }
        // IF_HAS_GREATER_POWER_EFFECTS
        if (effect.IfHasGreaterPowerEffects.Count > 0)
        {
            foreach (GameObject t in validTargets)
                if (coMan.GetUnitDisplay(t).CurrentPower > effect.IfHasGreaterPowerValue)
                    foreach (EffectGroup eg in effect.IfHasGreaterPowerEffects)
                        additionalEffectGroups.Add(eg);
        }
        // IF_HAS_LOWER_POWER_EFFECTS
        if (effect.IfHasLowerPowerEffects.Count > 0)
        {
            foreach (GameObject t in validTargets)
                if (coMan.GetUnitDisplay(t).CurrentPower < effect.IfHasLowerPowerValue)
                    foreach (EffectGroup eg in effect.IfHasLowerPowerEffects)
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
                raySource = coMan.PlayerHero;
        }
        else raySource = coMan.PlayerHero;

        // DRAW
        if (effect is DrawEffect de)
        {
            if (EffectRayError()) return;

            if (de.IsDiscardEffect)
                foreach (GameObject target in validTargets)
                    coMan.DiscardCard(target);

            else if (isValidEffect)
            {
                string hero;
                if (IsPlayerSource(newEffectSource)) hero = GameManager.PLAYER;
                else hero = GameManager.ENEMY;

                GameObject target;
                if (validTargets.Count > 0) target = validTargets[0];
                else target = null; // TESTING

                DeriveEffectValues(target); // TESTING

                List<Effect> addEffects = de.AdditionalEffects;
                for (int i = 0; i < effect.Value; i++)
                {
                    newDelay += 0.5f;
                    delay = newDelay; // Unnecessary?

                    FunctionTimer.Create(() => NewActiveEffect(target,
                        () => coMan.DrawCard(hero, null, addEffects), shootRay), newDelay);
                }
            }
        }
        // DAMAGE
        else if (effect is DamageEffect dmgE)
        {
            if (validTargets.Count > 0) FunctionTimer.Create(() =>
            auMan.StartStopSound("SFX_DamageRay_Start"), delay);

            foreach (GameObject target in validTargets)
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
                    sourceIsUnit = coMan.IsUnitCard(newEffectSource);
                    if (sourceIsUnit) sourceUcd = newEffectSource.GetComponent<UnitCardDisplay>();
                    targetIsUnit = target.TryGetComponent(out targetUcd);
                }

                coMan.TakeDamage(target, effect.Value,
                    out bool targetDamaged, out bool targetDestroyed);

                if (targetDestroyed)
                {
                    foreach (EffectGroup efg in dmgE.IfDestroyedEffects)
                        additionalEffectGroups.Add(efg);

                    if (sourceIsUnit && targetIsUnit)
                    {
                        if (sourceUcd.CurrentHealth > 0 && !UnitsToDestroy.Contains(newEffectSource) &&
                            CardManager.GetTrigger(newEffectSource, CardManager.TRIGGER_DEATHBLOW))
                        {
                            GameObject source = newEffectSource;
                            evMan.NewDelayedAction(() =>
                            caMan.TriggerUnitAbility(source,
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
            foreach (GameObject target in validTargets)
                NewActiveEffect(target, () =>
                DestroyUnit(target), shootRay);

            void DestroyUnit(GameObject target)
            {
                uMan.ShakeCamera(UIManager.Bump_Light);
                UnitCardDisplay ucd = target.GetComponent<UnitCardDisplay>();
                int previousHealth = ucd.CurrentHealth;
                ucd.CurrentHealth = 0;
                anMan.UnitTakeDamageState(target, previousHealth);
                coMan.DestroyUnit(target);
            }
        }
        // HEALING
        else if (effect is HealEffect)
        {
            foreach (GameObject target in validTargets)
                NewActiveEffect(target, () =>
                coMan.HealDamage(target, effect as HealEffect), shootRay); // TESTING passing 'effect' not 'healEffect'
        }
        // EXHAUST/REFRESH
        else if (effect is ExhaustEffect ee)
        {
            auMan.StartStopSound("SFX_Refresh");
            foreach (GameObject target in validTargets)
                NewActiveEffect(target, () =>
                SetExhaustion(target), shootRay);

            void SetExhaustion(GameObject target)
            {
                UnitCardDisplay ucd = coMan.GetUnitDisplay(target);
                if (ucd.IsExhausted != ee.SetExhausted)
                    ucd.IsExhausted = ee.SetExhausted;
            }
        }
        // REPLENISH
        else if (effect is ReplenishEffect)
        {
            if (EffectRayError()) return;

            GameObject hero;
            int startEnergy;
            int energyPerTurn;
            int newEnergy;

            if (IsPlayerSource(newEffectSource))
            {
                hero = coMan.PlayerHero;
                startEnergy = pMan.CurrentEnergy;
                energyPerTurn = pMan.EnergyPerTurn;
            }
            else
            {
                hero = coMan.EnemyHero;
                startEnergy = enMan.CurrentEnergy;
                energyPerTurn = enMan.EnergyPerTurn;
            }

            newEnergy = startEnergy + effect.Value;
            if (newEnergy > energyPerTurn) newEnergy = energyPerTurn;
            if (newEnergy < startEnergy) newEnergy = startEnergy;

            if (newEnergy > startEnergy)
            {
                if (IsPlayerSource(newEffectSource)) pMan.CurrentEnergy = newEnergy;
                else enMan.CurrentEnergy = newEnergy;
            }

            int energyChange = newEnergy - startEnergy;
            anMan.ModifyHeroEnergyState(energyChange, hero);
        }
        // GIVE_NEXT_UNIT
        else if (effect is GiveNextUnitEffect gnfe)
        {
            if (EffectRayError()) return;

            // NEW EFFECT INSTANCE
            GiveNextUnitEffect newGnfe = ScriptableObject.CreateInstance<GiveNextUnitEffect>();
            newGnfe.LoadEffect(gnfe);

            List<GiveNextUnitEffect> giveNextUnitEffects;
            if (IsPlayerSource(newEffectSource)) giveNextUnitEffects = giveNextEffects_Player;
            else giveNextUnitEffects = giveNextEffects_Enemy;

            giveNextUnitEffects.Add(newGnfe);
        }
        // STAT_CHANGE/GIVE_ABILITY
        else if (effect is StatChangeEffect sce)
        {
            foreach (GameObject target in validTargets)
            {
                NewActiveEffect(target, () =>
                AddEffect(target, effect), shootRay);
            }
        }
        // GIVE_ABILITY
        else if (effect is GiveAbilityEffect)
        {
            foreach (GameObject target in validTargets)
            {
                NewActiveEffect(target, () =>
                AddEffect(target, effect), shootRay);
            }
        }
        // REMOVE_ABILITY
        else if (effect is RemoveAbilityEffect rae)
        {
            foreach (GameObject target in validTargets)
                NewActiveEffect(target, () =>
                RemoveAbilities(target), shootRay);

            void RemoveAbilities(GameObject target)
            {
                if (!target.TryGetComponent(out UnitCardDisplay ucd))
                {
                    Debug.LogError("TARGET IS NOT UNIT CARD!");
                    return;
                }

                if (rae.RemoveAbility != null) ucd.RemoveCurrentAbility(rae.RemoveAbility.AbilityName, false); // TESTING
                else if (rae.RemoveAllAbilities || rae.RemoveAllButNegativeAbilities)
                {
                    List<string> abilitiesToRemove = new List<string>();
                    foreach (CardAbility ca in ucd.CurrentAbilities)
                    {
                        if (rae.RemoveAllButNegativeAbilities)
                        {
                            foreach (string negativeAbility in CardManager.NegativeAbilities)
                                if (ca.name == negativeAbility) goto NextAbility;
                        }

                        // Don't remove ChangeControl abilities
                        if (ca is TriggeredAbility tra)
                        {
                            foreach (EffectGroup eg in tra.EffectGroupList)
                                foreach (Effect e in eg.Effects)
                                    if (e is ChangeControlEffect)
                                        goto NextAbility;
                        }

                        abilitiesToRemove.Add(ca.AbilityName);
                        NextAbility:;
                    }

                    foreach (string ability in abilitiesToRemove)
                        ucd.RemoveCurrentAbility(ability, false);

                    ucd.CurrentEffects.Clear();
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
                            ucd.RemoveCurrentAbility(negativeAbility, false);
                    }
                }
            }
        }
        // CREATE_CARD
        else if (effect is CreateCardEffect cce)
        {
            if (EffectRayError()) return;

            if (isValidEffect) // TESTING
            {
                Card cardScript = null;
                if (cce.CreatedCard != null) cardScript = cce.CreatedCard;
                else if (cce.RandomCard || !string.IsNullOrEmpty(cce.CreatedCardType))
                {
                    List<Card> cardPool = new List<Card>();
                    string cardType = cce.CreatedCardType;

                    if (cce.RandomCard)
                    {
                        if (!cce.RestrictType || cce.IncludeUnits)
                        {
                            Card[] unitCards;
                            unitCards = Resources.LoadAll<Card>("Cards_Units");
                            foreach (Card c in unitCards)
                                cardPool.Add(c);
                        }

                        if (!cce.RestrictType || cce.IncludeActions)
                        {
                            Card[] actionCards;
                            actionCards = Resources.LoadAll<Card>("Cards_Actions");
                            foreach (Card c in actionCards)
                                cardPool.Add(c);
                        }
                    }
                    else
                    {
                        Card[] createdCards = Resources.LoadAll<Card>("Cards_Created");
                        foreach (Card c in createdCards)
                            if (c.CardSubType == cardType) cardPool.Add(c);

                        if (cardPool.Count < 1)
                        {
                            Debug.LogError("CARDS NOT FOUND!");
                            return;
                        }
                    }

                    // Created Card Parameters
                    List<Card> invalidCards = new List<Card>();

                    if (cce.RestrictCost)
                    {
                        foreach (Card c in cardPool)
                            if (c.StartEnergyCost < cce.MinCost ||
                                c.StartEnergyCost > cce.MaxCost) invalidCards.Add(c);
                    }

                    if (cce.RestrictType)
                    {
                        foreach (Card c in cardPool)
                            if ((!cce.IncludeUnits && c is UnitCard) ||
                                (!cce.IncludeActions && c is ActionCard)) invalidCards.Add(c);
                    }

                    if (cce.RestrictSubtype)
                    {
                        foreach (Card c in cardPool)
                            if (c.CardType != cce.CardSubtype) invalidCards.Add(c);
                    }

                    if (cce.ExcludeSelf)
                    {
                        if (!newEffectSource.TryGetComponent(out CardDisplay cardDisplay))
                        {
                            Debug.LogError("SOURCE IS NOT A CARD!");
                        }
                        else
                        {
                            int selfIndex = cardPool.FindIndex(x => x.CardName == cardDisplay.CardName);
                            if (selfIndex != -1) cardPool.RemoveAt(selfIndex);
                        }
                    }

                    foreach (Card c in invalidCards) cardPool.Remove(c);

                    cardScript = cardPool[Random.Range(0, cardPool.Count)];
                }
                else
                {
                    Debug.LogError("INVALID TYPE!");
                    return;
                }
                cardScript = caMan.NewCardInstance(cardScript);
                GameObject newCardObj = DrawCreatedCard(cardScript);

                foreach (Effect addEffect in cce.AdditionalEffects)
                    ResolveEffect(new List<GameObject> { newCardObj }, addEffect, false, 0, out _, false);
            }
        }
        // PLAY_CARD
        else if (effect is PlayCardEffect pce)
        {
            if (EffectRayError()) return;

            if (isValidEffect) // TESTING
            {
                Card cardScript = null;
                if (pce.PlayedCard != null) cardScript = pce.PlayedCard;
                else if (!string.IsNullOrEmpty(pce.PlayedCardType))
                {
                    List<Card> cardPool = new List<Card>();
                    Card[] createdCards = Resources.LoadAll<Card>("Cards_Created");
                    foreach (Card c in createdCards)
                        if (c.CardSubType == pce.PlayedCardType) cardPool.Add(c);
                    if (cardPool.Count < 1)
                    {
                        Debug.LogError("CARDS NOT FOUND!");
                        return;
                    }
                    cardScript = cardPool[Random.Range(0, cardPool.Count)];
                }
                else
                {
                    Debug.LogError("INVALID TYPE!");
                    return;
                }

                if (cardScript is UnitCard) { }
                else Debug.LogError("SCRIPT IS NOT UNIT CARD!");

                cardScript = caMan.NewCardInstance(cardScript);
                GameObject newCardObj = PlayCreatedUnit(cardScript as UnitCard, pce.EnemyCard); // TESTING

                foreach (Effect addEffect in pce.AdditionalEffects)
                    ResolveEffect(new List<GameObject> { newCardObj }, addEffect, false, 0, out _, false);
            }
        }
        // COPY_CARD
        else if (effect is CopyCardEffect cpyCrd)
        {
            if (EffectRayError()) return;

            foreach (GameObject target in validTargets)
            {
                CardDisplay cardDisplay = target.GetComponent<CardDisplay>();
                Card card = cardDisplay.CardScript;
                Card newCard = caMan.NewCardInstance(card, cpyCrd.IsExactCopy);
                GameObject newCardObj = null;

                if (!cpyCrd.CopyUnit && newCard is UnitCard) Debug.LogError("SCRIPT IS NOT UNIT CARD!");
                else if (!cpyCrd.CopyAction && newCard is ActionCard) Debug.LogError("SCRIPT IS NOT ACTION CARD!");

                if (cpyCrd.PlayCopy)
                {
                    if (newCard is ActionCard) Debug.LogError("CANNOT PLAY COPIED ACTIONS!");
                    else newCardObj = PlayCreatedUnit(newCard as UnitCard, false);
                }
                else newCardObj = DrawCreatedCard(newCard);

                if (newCardObj == null)
                {
                    Debug.LogWarning("NEW CARD IS NULL!");
                    continue;
                }

                if (cpyCrd.IsExactCopy)
                {
                    foreach (Effect e in cardDisplay.CurrentEffects)
                        AddEffect(newCardObj, e, true, false);
                }

                foreach (Effect addEffect in cpyCrd.AdditionalEffects)
                    ResolveEffect(new List<GameObject> { newCardObj }, addEffect, false, 0, out _, false);
            }
        }
        // CHANGE_COST_EFFECT
        else if (effect is ChangeCostEffect chgCst)
        {
            if (EffectRayError()) return;

            if (chgCst.ChangeNextCost)
            {
                ChangeCostEffect newChgCst = ScriptableObject.CreateInstance<ChangeCostEffect>();
                newChgCst.LoadEffect(chgCst);

                List<ChangeCostEffect> changeCostEffects;
                List<GameObject> affectedCards;

                if (IsPlayerSource(newEffectSource))
                {
                    changeCostEffects = ChangeNextCostEffects_Player;
                    affectedCards = coMan.PlayerHandCards;
                }
                else
                {
                    changeCostEffects = ChangeNextCostEffects_Enemy;
                    affectedCards = coMan.EnemyHandCards;
                }

                changeCostEffects.Add(newChgCst);
                foreach (GameObject card in affectedCards)
                    AddEffect(card, newChgCst, false);
            }
            else
            {
                foreach (GameObject target in validTargets)
                    AddEffect(target, chgCst);
            }
        }
        // CHANGE_CONTROL_EFFECT
        else if (effect is ChangeControlEffect chgCtrl)
        {
            foreach (GameObject target in validTargets)
                NewActiveEffect(target, () =>
                coMan.ChangeUnitControl(target), shootRay);
        }
        // ACQUIRE_ITEM_EFFECT
        else if (effect is AcquireItemEffect aie)
        {
            if (pMan.HeroItems.Count < GameManager.MAXIMUM_ITEMS)
            {
                HeroItem[] allItems = Resources.LoadAll<HeroItem>("Items");
                List<HeroItem> itemPool = new List<HeroItem>();
                foreach (HeroItem item in allItems)
                {
                    if (!aie.IncludeRareItems && item.IsRareItem) continue;
                    else itemPool.Add(item);
                }

                List<HeroItem> newItemPool = GetItemPool(itemPool, false);
                if (itemPool.Count < 1) newItemPool = GetItemPool(itemPool, true);

                int itemIndex = Random.Range(0, newItemPool.Count);
                pMan.AddItem(newItemPool[itemIndex], true);
            }
            else uMan.CreateFleetingInfoPopup("You can't have more than " + GameManager.MAXIMUM_ITEMS + " items!");

            List<HeroItem> GetItemPool(List<HeroItem> itemPool, bool includeOwned)
            {
                List<HeroItem> newItemPool = new List<HeroItem>();
                foreach (HeroItem item in itemPool)
                {
                    if (!includeOwned && pMan.HeroItems.FindIndex(x => x.ItemName == item.ItemName) != -1) continue;
                    else newItemPool.Add(item);
                }
                return newItemPool;
            }
        }
        else Debug.LogError("EFFECT TYPE NOT FOUND!");

        // IF RESOLVES EFFECTS
        foreach (GameObject target in validTargets)
        {
            foreach (Effect e in effect.IfResolvesEffects)
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
            if (effect is DrawEffect || effect is GiveNextUnitEffect ||
                    (effect is ChangeCostEffect cce && cce.ChangeNextCost)) { }
            else newDelay += 0.25f;

            FunctionTimer.Create(() => ActiveEffects--, newDelay);
        }

        // NEW ACTIVE EFFECT
        void NewActiveEffect(GameObject target, System.Action action, bool shootRay)
        {
            // NEW EFFECT INSTANCE
            Effect newEffect = ScriptableObject.CreateInstance(effect.GetType().Name) as Effect;
            newEffect.LoadEffect(effect);
            effect = newEffect;

            if (effect is DrawEffect) { }
            else DeriveEffectValues(target); // TESTING

            EffectRay.EffectRayType effectRayType;
            if (isEffectGroup) effectRayType = EffectRay.EffectRayType.EffectGroup;
            else effectRayType = EffectRay.EffectRayType.Default;

            if (shootRay)
            {
                Color rayColor = effect.RayColor;
                if (effect is DamageEffect || effect is DestroyEffect) rayColor = damageRayColor;
                else if (effect is HealEffect) rayColor = healRayColor;

                if (rayColor.a == 0) rayColor = Color.red;

                ShootEffectRay(target, effect, () => action());

                void ShootEffectRay(GameObject target, Effect effect, System.Action action) =>
                    CreateEffectRay(raySource.transform.position, target,
                    () => action(), rayColor, effectRayType, delay);
            }
            else action();
        }

        void DeriveEffectValues(GameObject target)
        {
            if (target == null) return;

            if (effect.IsDerivedValue)
            {
                newEffectSource.TryGetComponent(out UnitCardDisplay ucd); // TESTING

                int newValue;
                switch (effect.DerivedValue)
                {
                    case Effect.DerivedValueType.Source_Power:
                        newValue = ucd.CurrentPower;
                        break;
                    case Effect.DerivedValueType.Source_Health:
                        newValue = ucd.CurrentHealth;
                        break;
                    case Effect.DerivedValueType.Target_Keywords: // TESTING
                        newValue = caMan.GetPositiveKeywords(target);
                        break;
                    default:
                        Debug.LogError("INVALID TYPE!");
                        return;
                }
                effect.Value = newValue;
            }

            if (effect is StatChangeEffect sce)
            {
                if (!target.TryGetComponent(out UnitCardDisplay ucd))
                {
                    Debug.LogError("TARGET IS NOT UNIT CARD!");
                    return;
                }

                if (sce.PowerIsDerived)
                {
                    switch (sce.DerivedPowerType)
                    {
                        case Effect.DerivedValueType.Target_Keywords:
                            sce.PowerChange = caMan.GetPositiveKeywords(target);
                            Debug.LogWarning("DERIVED POWER <" + sce.PowerChange + ">");
                            break;
                        default:
                            Debug.LogError("INVALID TYPE!");
                            return;
                    }
                }

                if (sce.HealthIsDerived)
                {
                    switch (sce.DerivedHealthType)
                    {
                        case Effect.DerivedValueType.Target_Keywords:
                            sce.HealthChange = caMan.GetPositiveKeywords(target);
                            Debug.LogWarning("DERIVED HEALTH <" + sce.HealthChange + ">");
                            break;
                        default:
                            Debug.LogError("INVALID TYPE!");
                            return;
                    }
                }
            }
        }

        bool EffectRayError()
        {
            if (effect.ShootRay)
            {
                Debug.LogError("CANNOT SHOOT RAY FOR <" + effect.ToString() + "> !");
                return true;
            }
            return false;
        }
    }

    /******
     * *****
     * ****** GET_VALID_TARGETS
     * *****
     *****/
    private List<GameObject> GetValidTargets(Effect effect, List<GameObject> allTargets)
    {
        List<GameObject> validTargets = new List<GameObject>();
        List<GameObject> invalidTargets = new List<GameObject>();

        foreach (GameObject t in allTargets)
        {
            if (t == null)
            {
                Debug.LogWarning("EMPTY TARGET!");
                invalidTargets.Add(t);
            }
        }

        // IF_HAS_ITEMS_CONDITION
        if (effect.IfHasItemsCondition)
        {
            bool isValid;
            int playerItems = pMan.HeroItems.Count;
            int hasItemsValue = effect.IfHasItemsValue;

            if (effect.IsLessItemsCondition)
            {
                if (playerItems < hasItemsValue) isValid = true;
                else isValid = false;
            }
            else
            {
                if (playerItems > hasItemsValue) isValid = true;
                else isValid = false;
            }

            if (!isValid)
            {
                foreach (GameObject t in allTargets)
                    invalidTargets.Add(t);
            }
        }

        // IF_WOUNDED_CONDITIONS
        if (effect.IfAnyWoundedCondition)
        {
            bool isValid;
            if (HeroIsWounded(effectSource, true) ||
                HeroIsWounded(effectSource, false)) isValid = true;
            else isValid = false;

            ValidateWoundedTargets(isValid);
        }
        else
        {
            if (effect.IfPlayerWoundedCondition)
                ValidateWoundedTargets(HeroIsWounded(effectSource, true));

            if (effect.IfEnemyWoundedCondition)
                ValidateWoundedTargets(HeroIsWounded(effectSource, false));
        }

        void ValidateWoundedTargets(bool isValid)
        {
            if (effect.IfNotWoundedCondition) isValid = !isValid;

            if (!isValid)
            {
                foreach (GameObject t in allTargets)
                    invalidTargets.Add(t);
            }
        }

        bool HeroIsWounded(GameObject effectSource, bool getPlayerHealth)
        {
            if (effectSource == null)
            {
                Debug.LogError("SOURCE IS NULL!");
                return false;
            }

            int damageTaken_Turn;
            if (IsPlayerSource(effectSource))
            {
                if (getPlayerHealth) damageTaken_Turn = pMan.DamageTaken_Turn;
                else damageTaken_Turn = enMan.DamageTaken_Turn;
            }
            else
            {
                if (getPlayerHealth) damageTaken_Turn = enMan.DamageTaken_Turn;
                else damageTaken_Turn = pMan.DamageTaken_Turn;
            }

            if (damageTaken_Turn >= GameManager.WOUNDED_VALUE) return true;
            else return false;
        }


        // IF_EXHAUSTED_CONDITION
        if (effect.IfExhaustedCondition)
        {
            foreach (GameObject t in allTargets)
                if (!coMan.GetUnitDisplay(t).IsExhausted)
                    invalidTargets.Add(t);
        }
        // IF_REFRESHED_CONDITION
        if (effect.IfRefreshedCondition)
        {
            foreach (GameObject t in allTargets)
                if (coMan.GetUnitDisplay(t).IsExhausted)
                    invalidTargets.Add(t);
        }
        // IF_DAMAGED_CONDITION
        if (effect.IfDamagedCondition)
        {
            foreach (GameObject t in allTargets)
                if (!coMan.IsDamaged(t))
                    invalidTargets.Add(t);
        }
        // IF_NOT_DAMAGED_CONDITION
        if (effect.IfNotDamagedCondition)
        {
            foreach (GameObject t in allTargets)
                if (coMan.IsDamaged(t))
                    invalidTargets.Add(t);
        }
        // IF_HAS_POWER_CONDITION
        if (effect.IfHasPowerCondition)
        {
            int powerValue = effect.IfHasPowerValue;
            bool isLessCondition = effect.IsLessPowerCondition;
            foreach (GameObject t in allTargets)
            {
                bool isValidTarget = false;
                if (!isLessCondition)
                {
                    if (coMan.GetUnitDisplay(t).CurrentPower > powerValue)
                        isValidTarget = true;
                }
                else
                {
                    if (coMan.GetUnitDisplay(t).CurrentPower < powerValue)
                        isValidTarget = true;
                }
                if (!isValidTarget) invalidTargets.Add(t);
            }
        }
        // IF_HAS_ABILITY_CONDITION
        if (effect.IfHasAbilityCondition != null)
        {
            foreach (GameObject t in allTargets)
            {
                if (CardManager.GetAbility(t,
                    effect.IfHasAbilityCondition.AbilityName))
                {
                    if (effect.IfNotHasAbilityCondition)
                        invalidTargets.Add(t);
                }
                else
                {
                    if (!effect.IfNotHasAbilityCondition)
                        invalidTargets.Add(t);
                }
            }
        }
        // IF_HAS_TRIGGER_CONDITION
        if (effect.IfHasTriggerCondition != null)
        {
            foreach (GameObject t in allTargets)
            {
                if (CardManager.GetTrigger(t,
                    effect.IfHasTriggerCondition.AbilityName))
                {
                    if (effect.IfNotHasTriggerCondition)
                        invalidTargets.Add(t);
                }
                else
                {
                    if (!effect.IfNotHasTriggerCondition)
                        invalidTargets.Add(t);
                }
            }
        }

        foreach (GameObject t in allTargets)
            if (!invalidTargets.Contains(t) && !validTargets.Contains(t))
                validTargets.Add(t);

        return validTargets;
    }

    /******
     * *****
     * ****** DRAW_CREATED_CARD
     * *****
     *****/
    private GameObject DrawCreatedCard(Card cardScript)
    {
        Debug.LogWarning("DRAW CREATED CARD!");

        string hero;
        if (IsPlayerSource(effectSource)) hero = GameManager.PLAYER;
        else hero = GameManager.ENEMY;
        return coMan.DrawCard(hero, cardScript);
    }

    /******
     * *****
     * ****** PLAY_CREATED_UNIT
     * *****
     *****/
    public GameObject PlayCreatedUnit(UnitCard unitCardScript, bool enemyCard, GameObject newEffectSource = null)
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

        string cardTag;
        string cardZone;
        List<GameObject> zoneList;
        string errorMessage;

        bool isPlayerCard = IsPlayerSource(newEffectSource); // TESTING
        if (enemyCard) isPlayerCard = !isPlayerCard;

        if (isPlayerCard)
        {
            cardTag = CombatManager.PLAYER_CARD;
            cardZone = CombatManager.PLAYER_ZONE;
            zoneList = coMan.PlayerZoneCards;
            errorMessage = "You can't play more units!";
        }
        else
        {
            cardTag = CombatManager.ENEMY_CARD;
            cardZone = CombatManager.ENEMY_ZONE;
            zoneList = coMan.EnemyZoneCards;
            errorMessage = "Enemy can't play more units!";
        }

        if (zoneList.Count >= GameManager.MAX_UNITS_PLAYED)
        {
            uMan.CreateFleetingInfoPopup(errorMessage);
            return null;
        }

        Vector2 newPosition = newEffectSource.transform.position;
        GameObject card = coMan.ShowCard(unitCardScript, newPosition,
            CombatManager.DisplayType.Default, true);

        card.tag = cardTag;
        zoneList.Add(card);
        coMan.ChangeCardZone(card, cardZone);
        anMan.CreateParticleSystem(card, ParticleSystemHandler.ParticlesType.Drag, 1);
        auMan.StartStopSound("SFX_CreateCard");
        uMan.CombatLog_PlayCard(card);

        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
        CardContainer container = cardDisplay.CardContainer.GetComponent<CardContainer>();
        container.OnAttachAction += () => PlayUnit(card);
        return card;

        void PlayUnit(GameObject unitCard)
        {
            UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
            auMan.StartStopSound(null, ucd.UnitCard.CardPlaySound);

            unitCard.GetComponent<DragDrop>().IsPlayed = true;
            anMan.UnitStatChangeState(unitCard, 0, 0, false, true);

            FunctionTimer.Create(() => SetFirstSibling(unitCard), 0.5f);
        }

        void SetFirstSibling(GameObject card)
        {
            if (card == null) return;
            card.transform.SetAsFirstSibling();
        }
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
        if (IsPlayerSource(effectSource)) handZone = CombatManager.PLAYER_HAND;
        else handZone = CombatManager.ENEMY_HAND;

        if (effectSource.TryGetComponent(out ActionCardDisplay acd))
        {
            if (isUserAbort)
            {
                pMan.CurrentEnergy += acd.CurrentEnergyCost;
                coMan.ChangeCardZone(effectSource, handZone, true);
                coMan.PlayerActionZoneCards.Remove(effectSource);
                coMan.PlayerHandCards.Add(effectSource);
            }
        }
        else if (effectSource.TryGetComponent(out UnitCardDisplay ucd))
        {
            if (isUserAbort)
            {
                pMan.CurrentEnergy += ucd.CurrentEnergyCost;
                coMan.ChangeCardZone(effectSource, handZone, true);
                coMan.PlayerZoneCards.Remove(effectSource);
                coMan.PlayerHandCards.Add(effectSource);
            }
            else
            {
                if (triggerName == CardManager.TRIGGER_PLAY)
                {
                    ResolveChangeNextCostEffects(effectSource); // Resolves IMMEDIATELY
                    caMan.TriggerTrapAbilities(effectSource); // Resolves 3rd
                    TriggerModifiers_PlayUnit(effectSource); // Resolves 2nd
                    TriggerGiveNextEffects(effectSource); // Resolves 1st
                }
            }
        }
        else if (effectSource.CompareTag(CombatManager.HERO_POWER))
        {
            if (isUserAbort)
            {
                pMan.HeroPowerUsed = false;
                pMan.CurrentEnergy += pMan.PlayerHero.HeroPower.PowerCost;
            }
        }
        else if (effectSource.CompareTag(CombatManager.HERO_ULTIMATE))
        {
            if (isUserAbort)
            {
                pMan.CurrentEnergy += pMan.GetUltimateCost(out _);
                pMan.HeroUltimateProgress = GameManager.HERO_ULTMATE_GOAL;
            }
        }
        else if (effectSource.CompareTag(CombatManager.ENEMY_HERO_POWER))
        {
            // blank
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
        if (isUserAbort)
        {
            uMan.DismissInfoPopup();

            if (effectGroupList[currentEffectGroup].Targets.PlayerHand)
                anMan.ShiftPlayerHand(false);
        }
        FinishEffectGroupList(true);
    }
    
    /******
     * *****
     * ****** FINISH_EFFECT_GROUP_LIST
     * *****
     *****/
    public void FinishEffectGroupList(bool wasAborted)
    {
        if (!effectsResolving) // For Combat End
        {
            Debug.LogWarning("CANNOT FINISH GROUP! EFFECTS NOT RESOLVING!");
            additionalEffectGroups.Clear();
            FinishEffectCleanup();
            return;
        }

        string debugText = "RESOLVED";
        if (wasAborted) debugText = "ABORTED";
        Debug.Log("<<< GROUP LIST FINISHED! [" + debugText + "] >>>");

        if (effectSource == null) // For GiveNextEffects
        {
            Debug.LogWarning("EFFECT SOURCE IS NULL!");
            DeselectTargets();
            FinishEffectCleanup();
            return;
        }

        if (!wasAborted || isAdditionalEffect)
        {
            if (additionalEffectGroups.Count > 0)
            {
                GameObject source = effectSource;
                EffectGroup group = additionalEffectGroups[0];

                evMan.NewDelayedAction(() =>
                StartEffectGroupList(new List<EffectGroup> { group },
                source), 0, true, true); // PRIORITY ACTION
            }
            else
            {
                additionalEffectGroups.Clear();

                if (coMan.IsActionCard(effectSource))
                {
                    uMan.CombatLog_PlayCard(effectSource);
                    ResolveChangeNextCostEffects(effectSource);

                    GameObject source = effectSource;
                    evMan.NewDelayedAction(() => DiscardAction(source), 0.5f, false, true); // PRIORITY ACTION

                    void DiscardAction(GameObject source)
                    {
                        if (source == null)
                        {
                            Debug.LogError("ACTION ALREADY DISCARDED!");
                            return;
                        }

                        CardDisplay cd = source.GetComponent<CardDisplay>();
                        if (!cd.CardScript.IsRare) // Don't count Created Card Ultimates
                        {
                            switch (cd.CardScript.CardSubType, IsPlayerSource(source))
                            {
                                case (CardManager.EXPLOIT, true):
                                    coMan.ExploitsPlayed_Player++;
                                    break;
                                case (CardManager.EXPLOIT, false):
                                    coMan.ExploitsPlayed_Enemy++;
                                    break;
                                case (CardManager.INVENTION, true):
                                    coMan.InventionsPlayed_Player++;
                                    break;
                                case (CardManager.INVENTION, false):
                                    coMan.InventionsPlayed_Enemy++;
                                    break;
                                case (CardManager.SCHEME, true):
                                    coMan.SchemesPlayed_Player++;
                                    break;
                                case (CardManager.SCHEME, false):
                                    coMan.SchemesPlayed_Enemy++;
                                    break;
                                case (CardManager.EXTRACTION, true):
                                    coMan.ExtractionsPlayed_Player++;
                                    break;
                                case (CardManager.EXTRACTION, false):
                                    coMan.ExtractionsPlayed_Enemy++;
                                    break;
                            }
                        }

                        coMan.DiscardCard(source, true);
                        coMan.ActionsPlayed_ThisTurn++;
                    }
                }
                else if (coMan.IsUnitCard(effectSource) && IsPlayerSource(effectSource))
                {
                    if (triggerName == CardManager.TRIGGER_PLAY)
                    {
                        uMan.CombatLog_PlayCard(effectSource);
                        ResolveChangeNextCostEffects(effectSource); // Resolves IMMEDIATELY
                        caMan.TriggerTrapAbilities(effectSource); // Resolves 3rd
                        TriggerModifiers_PlayUnit(effectSource); // Resolves 2nd
                        TriggerGiveNextEffects(effectSource); // Resolves 1st
                    }
                }
                else if (effectSource.TryGetComponent(out ItemIcon icon))
                {
                    uMan.CombatLogEntry("You used <b><color=\"yellow\">" +
                        icon.LoadedItem.ItemName + "</b></color> (Item).");
                    pMan.HeroItems.Remove(icon.LoadedItem);
                    uMan.SetSkybar(true);
                }
                else if (effectSource.CompareTag(CombatManager.HERO_POWER))
                {
                    uMan.CombatLogEntry("You used <b><color=\"yellow\">" +
                        pMan.PlayerHero.HeroPower.PowerName + "</b></color> (Hero Power).");
                    caMan.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH, GameManager.PLAYER);
                    pMan.HeroUltimateProgress++;

                    // TUTORIAL!
                    if (GameManager.Instance.IsTutorial && pMan.EnergyPerTurn == 2)
                        GameManager.Instance.Tutorial_Tooltip(5);
                }
                else if (effectSource.CompareTag(CombatManager.ENEMY_HERO_POWER))
                {
                    uMan.CombatLogEntry("Enemy used <b><color=\"yellow\">" +
                        enMan.EnemyHero.EnemyHeroPower.PowerName + "</b></color> (Hero Power).");
                    caMan.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH, GameManager.ENEMY);
                }
                else if (effectSource.CompareTag(CombatManager.HERO_ULTIMATE))
                {
                    uMan.CombatLogEntry("You used <b><color=\"yellow\">" +
                        pMan.PlayerHero.HeroUltimate.PowerName + "</b></color> (Hero Ultimate).");
                    caMan.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH, GameManager.PLAYER);
                    pMan.HeroUltimateProgress = 0;
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
            if (pMan.IsMyTurn) coMan.SelectPlayableCards();
        }

        void DeselectTargets()
        {
            if (legalTargets != null)
            {
                foreach (List<GameObject> list in legalTargets)
                {
                    foreach (GameObject target in list)
                    {
                        if (target == null) continue;
                        uMan.SelectTarget(target, UIManager.SelectionType.Disabled);
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
                        uMan.SelectTarget(target, UIManager.SelectionType.Disabled);
                    }
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
            effectGroupList = null;
            legalTargets = null;
            acceptedTargets = null;
            effectSource = null;
            EffectsResolving = false;
        }
    }

    /******
     * *****
     * ****** SELECT/ACCEPT/REJECT/REMOVE_EFFECT_TARGET
     * *****
     *****/
    public void HighlightEffectTarget(GameObject target, bool isSelected)
    {
        if (acceptedTargets == null || currentEffectGroup > acceptedTargets.Count - 1) return; // Unnecessary?
        else if (legalTargets == null || currentEffectGroup > legalTargets.Count - 1) return; // Unnecessary?

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
        uMan.SelectTarget(target, type);
    }
    public void SelectEffectTarget(GameObject target)
    {
        if (acceptedTargets == null || currentEffectGroup > acceptedTargets.Count - 1) return; // Unnecessary?
        else if (legalTargets == null || currentEffectGroup > legalTargets.Count - 1) return; // Unnecessary?

        if (acceptedTargets[currentEffectGroup].Contains(target)) RemoveEffectTarget(target);
        else if (legalTargets[currentEffectGroup].Contains(target)) AcceptEffectTarget(target);
        else
        {
            string message = "You can't target that!";

            if (effectSource != null)
            {
                if (IsPlayerSource(effectSource) != IsPlayerSource(target))
                {
                    if (CardManager.GetAbility(target, CardManager.ABILITY_WARD))
                        message = "Enemies with Ward can't be targetted!";
                }
            }

            RejectEffectTarget(message);
        }
    }
    private void AcceptEffectTarget(GameObject target)
    {
        EffectGroup eg = effectGroupList[currentEffectGroup];
        int targetNumber = eg.Targets.TargetNumber;
        int legalTargetNumber = legalTargets[currentEffectGroup].Count +
            acceptedTargets[currentEffectGroup].Count;

        if (!CurrentEffect.IsRequired && legalTargetNumber < targetNumber)
            targetNumber = legalTargetNumber;

        int accepted = acceptedTargets[currentEffectGroup].Count;
        if (accepted == targetNumber)
        {
            if (eg.Targets.VariableNumber) Debug.Log("ALL TARGETS SELECTED!");
            else Debug.LogError("TARGETTING ERROR!");
            return;
        }
        else if (accepted > targetNumber)
        {

            Debug.LogError("TOO MANY ACCEPTED TARGETS!");
            return;
        }

        auMan.StartStopSound("SFX_AcceptTarget");
        uMan.SelectTarget(target, UIManager.SelectionType.Selected);
        acceptedTargets[currentEffectGroup].Add(target);
        legalTargets[currentEffectGroup].Remove(target);

        Debug.Log("ACCEPTED TARGETS: <" + acceptedTargets[currentEffectGroup].Count +
            "> OF <" + targetNumber + "> REQUIRED TARGETS");

        if (eg.Targets.VariableNumber)
        {
            bool showConfirm = false;
            if (CurrentEffect is DrawEffect de && de.IsMulliganEffect)
                showConfirm = true;
            else if (acceptedTargets[currentEffectGroup].Count > 0)
                showConfirm = true;
            uMan.SetConfirmEffectButton(showConfirm);
        }
        else if (acceptedTargets[currentEffectGroup].Count == targetNumber)
            ConfirmTargetEffect();
    }
    private void RejectEffectTarget(string message)
    {
        uMan.CreateFleetingInfoPopup(message);
        auMan.StartStopSound("SFX_Error");
    }
    private void RemoveEffectTarget(GameObject target)
    {
        EffectTargets et = effectGroupList[currentEffectGroup].Targets;

        auMan.StartStopSound("SFX_AcceptTarget");
        uMan.SelectTarget(target, UIManager.SelectionType.Highlighted);
        acceptedTargets[currentEffectGroup].Remove(target);
        legalTargets[currentEffectGroup].Add(target);

        if (acceptedTargets[currentEffectGroup].Count < 1)
        {
            if (CurrentEffect is DrawEffect de && de.IsMulliganEffect || et.AllowZero) { } // TESTING
            else uMan.SetConfirmEffectButton(false);
        }
    }

    /******
     * *****
     * ****** ADD_EFFECT
     * *****
     *****/
    public void AddEffect(GameObject card, Effect effect, bool newInstance = true, bool applyEffect = true)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return;
        }

        UnitCardDisplay ucd = null;
        if (coMan.IsUnitCard(card))
        {
            ucd = coMan.GetUnitDisplay(card);
            if (ucd.CurrentHealth < 1) return; // Don't add effects to units with 0 health
        }

        if (effect is ChangeCostEffect chgCst)
        {
            bool isUnit = coMan.IsUnitCard(card);
            if (!isUnit && !chgCst.ChangeActionCost) return;
            if (isUnit && !chgCst.ChangeUnitCost) return;

            ChangeCostEffect newChgCst;
            if (newInstance)
            {
                newChgCst = ScriptableObject.CreateInstance<ChangeCostEffect>();
                newChgCst.LoadEffect(chgCst);
            }
            else newChgCst = chgCst;

            CardDisplay cd = card.GetComponent<CardDisplay>();
            cd.CurrentEffects.Add(newChgCst);

            if (applyEffect) cd.ChangeCurrentEnergyCost(newChgCst.ChangeValue);
        }
        else if (effect is GiveAbilityEffect gae)
        {
            if (ucd == null)
            {
                Debug.LogError("UNIT CARD DISPLAY IS NULL!");
                return;
            }

            GiveAbilityEffect newGae = ScriptableObject.CreateInstance<GiveAbilityEffect>();
            newGae.LoadEffect(gae);

            if (!applyEffect)
            {
                ucd.CurrentEffects.Add(newGae);
                return;
            }

            if (newGae.RandomPositiveKeyword) // TESTING
            {
                List<CardAbility> generatableKeywords = new List<CardAbility>();
                foreach (CardAbility keyword in caMan.GeneratableKeywords)
                {
                    if (!CardManager.GetAbility(card, keyword.AbilityName))
                        generatableKeywords.Add(keyword);
                }
                
                if (generatableKeywords.Count < 1)
                {
                    Debug.LogWarning("NO GENERATABLE KEYWORDS!");
                }
                else
                {
                    int randomIndex = Random.Range(0, generatableKeywords.Count);
                    ucd.AddCurrentAbility(generatableKeywords[randomIndex]);
                }
            }

            else if (!ucd.AddCurrentAbility(newGae.CardAbility))
            {
                // If ability is static and already exists, update countdown instead of adding ability
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
                            if ((newGae.Countdown == 0) ||
                                (cGae.Countdown != 0 && newGae.Countdown > cGae.Countdown))
                                        cGae.Countdown = newGae.Countdown;
                        }
            }
            else ucd.CurrentEffects.Add(newGae);
        }
        else if (effect is StatChangeEffect sce)
        {
            if (ucd == null)
            {
                Debug.LogError("UNIT CARD DISPLAY IS NULL!");
                return;
            }

            StatChangeEffect newSce = ScriptableObject.CreateInstance<StatChangeEffect>();
            newSce.LoadEffect(sce);
            ucd.CurrentEffects.Add(newSce);

            if (!applyEffect) return;

            if (newSce.ResetStats)
            {
                newSce.PowerChange = ucd.UnitCard.StartPower - ucd.CurrentPower;
                if (newSce.PowerChange > 0) newSce.PowerChange = 0;

                newSce.HealthChange = ucd.UnitCard.StartHealth - ucd.CurrentHealth;
                if (newSce.HealthChange > 0) newSce.HealthChange = 0;

                List<Effect> statChangeEffects = new List<Effect>();
                foreach (Effect e in ucd.CurrentEffects)
                    if (e is StatChangeEffect)
                        statChangeEffects.Add(e);

                foreach (Effect e in statChangeEffects)
                    ucd.CurrentEffects.Remove(e);

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

            anMan.ShowStatChange(card, newSce, false);
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
        List<GameObject> cardZone;
        if (hero == GameManager.PLAYER) cardZone = coMan.PlayerZoneCards;
        else cardZone = coMan.EnemyZoneCards;
        foreach (GameObject card in cardZone)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(card);
            List<Effect> expiredEffects = new List<Effect>();

            foreach (Effect effect in ucd.CurrentEffects)
            {
                if (effect.Countdown == 1) // Check for EXPIRED effects
                {
                    Debug.Log("EFFECT REMOVED: <" + effect.ToString() + ">");
                    expiredEffects.Add(effect);

                    evMan.NewDelayedAction(() => RemoveEffect(card, effect), 0.5f);

                    void RemoveEffect(GameObject card, Effect effect)
                    {
                        CardDisplay cd = card.GetComponent<CardDisplay>();
                        UnitCardDisplay ucd = cd as UnitCardDisplay;

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
                            anMan.ShowStatChange(card, sce, true);
                        }
                    }
                }
                else if (effect.Countdown != 0)
                {
                    effect.Countdown--;
                    Debug.Log("COUNTOWN FOR " + effect.ToString() +
                        " is <" + effect.Countdown + ">");
                }
            }

            foreach (Effect effect in expiredEffects)
            {
                ucd.CurrentEffects.Remove(effect);
                DestroyEffect(effect);
            }

            ucd = null;
            expiredEffects = null;
        }

        static void DestroyEffect(Effect effect)
        {
            Destroy(effect);
            effect = null;
        }
    }

    /******
     * *****
     * ****** TRIGGER_GIVE_NEXT_EFFECTS
     * *****
     *****/
    public void TriggerGiveNextEffects(GameObject card)
    {
        GameObject playerSource;
        List<GiveNextUnitEffect> giveNextEffects;
        List<GiveNextUnitEffect> resolvedGnue = new List<GiveNextUnitEffect>();

        if (IsPlayerSource(card))
        {
            giveNextEffects = GiveNextEffects_Player;
            playerSource = coMan.PlayerHero;
        }
        else
        {
            giveNextEffects = GiveNextEffects_Enemy;
            playerSource = coMan.EnemyHero;
        }

        if (giveNextEffects.Count > 0)
        {
            List<GameObject> target = new List<GameObject> { card };
            foreach (GiveNextUnitEffect gnue in giveNextEffects)
            {
                foreach (Effect e in gnue.Effects)
                    evMan.NewDelayedAction(() =>
                    ResolveEffect(target, e, true, 0, out _, false, playerSource), 0.25f, true);

                if (!gnue.Unlimited && --gnue.Multiplier < 1) resolvedGnue.Add(gnue); // TESTING
            }

            foreach (GiveNextUnitEffect rGnue in resolvedGnue)
            {
                giveNextEffects.Remove(rGnue);
                Destroy(rGnue);
            }
        }
    }

    /******
     * *****
     * ****** REMOVE_GIVE_NEXT_EFFECTS
     * *****
     *****/
    public void RemoveGiveNextEffects(string player)
    {
        List<GiveNextUnitEffect> giveNextEffects;
        List<GiveNextUnitEffect> expiredGne = new List<GiveNextUnitEffect>();

        if (player == GameManager.PLAYER) giveNextEffects = GiveNextEffects_Player;
        else giveNextEffects= GiveNextEffects_Enemy;

        foreach (GiveNextUnitEffect gnfe in giveNextEffects)
            if (gnfe.Countdown == 1) expiredGne.Add(gnfe);
            else if (gnfe.Countdown != 0) gnfe.Countdown--;

        foreach (GiveNextUnitEffect xGnfe in expiredGne)
        {
            giveNextEffects.Remove(xGnfe);
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
        List<GameObject> cardZone;
        if (card.CompareTag(CombatManager.PLAYER_CARD)) cardZone = coMan.PlayerZoneCards;
        else if (card.CompareTag(CombatManager.ENEMY_CARD)) cardZone = coMan.EnemyZoneCards;
        else
        {
            Debug.LogError("INVALID CARD TAG!");
            return 0;
        }

        foreach (GameObject unit in cardZone)
        {
            bool modFound = false;
            UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
            foreach (CardAbility ca in ucd.CurrentAbilities)
            {
                if (ca is ModifierAbility ma)
                {
                    if (ma.AbilityTrigger != null &&
                        ma.AbilityTrigger.AbilityName == abilityTrigger)
                    {
                        modsFound++;
                        modFound = true;
                    }
                }
            }

            if (modFound)
            {
                foreach (CardAbility ca in ucd.DisplayAbilities)
                {
                    if (ca is ModifierAbility)
                    {
                        ucd.AbilityTriggerState(ca.AbilityName);
                        break;
                    }
                }
            }
        }

        return modsFound;
    }

    /******
     * *****
     * ****** TRIGGER_MODIFIERS_PLAY_UNIT
     * *****
     *****/
    public void TriggerModifiers_PlayUnit(GameObject unitCard)
    {
        List<GameObject> cardZone;
        if (unitCard.CompareTag(CombatManager.PLAYER_CARD)) cardZone = coMan.PlayerZoneCards;
        else if (unitCard.CompareTag(CombatManager.ENEMY_CARD)) cardZone = coMan.EnemyZoneCards;
        else
        {
            Debug.LogError("INVALID CARD TAG!");
            return;
        }

        foreach (GameObject unit in cardZone)
        {
            if (unit == unitCard) continue;

            bool modFound = false;
            UnitCardDisplay ucd = unit.GetComponent<UnitCardDisplay>();
            foreach (CardAbility ca in ucd.CurrentAbilities)
            {
                if (ca is ModifierAbility ma)
                {
                    if (ma.PlayUnitEffects != null)
                    {
                        foreach (Effect effect in ma.PlayUnitEffects)
                        {
                            modFound = true;
                            evMan.NewDelayedAction(() =>
                            ResolveEffect(new List<GameObject> { unitCard },
                            effect, true, 0, out _, false, unit), 0.5f, true);
                        }
                    }
                }
            }

            if (modFound)
            {
                foreach (CardAbility ca in ucd.DisplayAbilities)
                {
                    if (ca is ModifierAbility)
                    {
                        ucd.AbilityTriggerState(ca.AbilityName);
                        break;
                    }
                }
            }
        }
    }

    /******
     * *****
     * ****** APPLY_CHANGE_NEXT_COST_EFFECTS
     * *****
     *****/
    public void ApplyChangeNextCostEffects(GameObject card)
    {
        List<ChangeCostEffect> changeCostEffects;
        if (card.CompareTag(CombatManager.PLAYER_CARD)) changeCostEffects = ChangeNextCostEffects_Player;
        else changeCostEffects = ChangeNextCostEffects_Enemy;

        foreach (ChangeCostEffect chgCst in changeCostEffects)
            AddEffect(card, chgCst, false);
    }

    /******
     * *****
     * ****** RESOLVE_CHANGE_NEXT_COST_EFFECTS
     * *****
     *****/
    public void ResolveChangeNextCostEffects(GameObject card)
    {
        string player;
        if (card.CompareTag(CombatManager.PLAYER_CARD)) player = GameManager.PLAYER;
        else player = GameManager.ENEMY;

        List<ChangeCostEffect> changeCostEffects;
        if (player == GameManager.PLAYER) changeCostEffects = ChangeNextCostEffects_Player;
        else changeCostEffects = ChangeNextCostEffects_Enemy;

        if (changeCostEffects.Count < 1) return;

        List<Effect> currentEffects = card.GetComponent<CardDisplay>().CurrentEffects;
        List<ChangeCostEffect> resolvedChgCst = new List<ChangeCostEffect>();

        foreach (Effect effect in currentEffects)
        {
            if (effect is ChangeCostEffect chgCst && chgCst.ChangeNextCost)
            {
                if (!chgCst.Unlimited && --chgCst.Multiplier < 1) resolvedChgCst.Add(chgCst); // TESTING
            }
        }

        foreach (ChangeCostEffect rChgCst in resolvedChgCst)
            FinishChangeNextCostEffect(rChgCst, player);
    }

    /******
     * *****
     * ****** FINISH_CHANGE_NEXT_COST_EFFECTS
     * *****
     *****/
    private void FinishChangeNextCostEffect(ChangeCostEffect rEffect, string player)
    {
        List<ChangeCostEffect> changeCostEffects;
        if (player == GameManager.PLAYER) changeCostEffects = ChangeNextCostEffects_Player;
        else changeCostEffects = ChangeNextCostEffects_Enemy;

        List<GameObject> affectedCards = new List<GameObject>();
        List<List<GameObject>> affectedZones = new List<List<GameObject>>();
        if (player == GameManager.PLAYER)
        {
            affectedZones.Add(coMan.PlayerHandCards);
            affectedZones.Add(coMan.PlayerZoneCards);
        }
        else
        {
            affectedZones.Add(coMan.EnemyHandCards);
            affectedZones.Add(coMan.EnemyZoneCards);
        }

        foreach (List<GameObject> cards in affectedZones)
            foreach (GameObject card in cards)
                affectedCards.Add(card);

        foreach (GameObject card in affectedCards)
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                continue;
            }

            CardDisplay cd = card.GetComponent<CardDisplay>();
            if (cd.CurrentEffects.Remove(rEffect))
                cd.ChangeCurrentEnergyCost(-rEffect.ChangeValue);
        }

        bool removed = changeCostEffects.Remove(rEffect);
        if (!removed) Debug.LogError("CHANGE COST EFFECT NOT FOUND!");
        Destroy(rEffect);
    }

    /******
     * *****
     * ****** REMOVE_CHANGE_NEXT_COST_EFFECTS
     * *****
     *****/
    public void RemoveChangeNextCostEffects(string player)
    {
        List<ChangeCostEffect> changeCostEffects;
        if (player == GameManager.PLAYER) changeCostEffects = ChangeNextCostEffects_Player;
        else changeCostEffects = ChangeNextCostEffects_Enemy;

        if (changeCostEffects.Count < 1) return;

        List<ChangeCostEffect> expiredChgCst = new List<ChangeCostEffect>();
        foreach (ChangeCostEffect chgCst in changeCostEffects)
            if (chgCst.Countdown == 1) expiredChgCst.Add(chgCst);
            else if (chgCst.Countdown != 0) chgCst.Countdown--;

        foreach (ChangeCostEffect xChgCst in expiredChgCst)
            FinishChangeNextCostEffect(xChgCst, player);
    }

    /******
     * *****
     * ****** CREATE_EFFECT_RAY
     * *****
     *****/
    public void CreateEffectRay(Vector2 start, GameObject target, System.Action rayEffect,
        Color rayColor, EffectRay.EffectRayType effectRayType, float delay = 0)
    {
        GameObject ray = Instantiate(effectRay, start, Quaternion.identity);
        ray.transform.SetParent(uMan.CurrentWorldSpace.transform);

        if (delay < 0)
        {
            Debug.LogError("DELAY IS NEGATIVE!");
            return;
        }

        if (delay == 0) SetRay();
        else FunctionTimer.Create(() => SetRay(), delay);

        if (effectRayType is EffectRay.EffectRayType.EffectGroup) ActiveEffects++;

        void SetRay()
        {
            ray.GetComponent<EffectRay>().SetEffectRay(target, rayEffect, rayColor, effectRayType);
        }
    }
}
