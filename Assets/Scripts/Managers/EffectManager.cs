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

    private void Start() => giveNextEffects = new List<GiveNextUnitEffect>(); // STATIC

    /* CLASS_VARIABLES */
    private List<EffectGroup> effectGroupList;
    private GameObject effectSource;

    private int currentEffectGroup;
    private int currentEffect;

    private List<List<GameObject>> legalTargets;
    private List<List<GameObject>> acceptedTargets;
    public List<GameObject> NewDrawnCards
    {
        get => newDrawnCards;
        private set => newDrawnCards = value;
    }
    private List<GameObject> newDrawnCards;

    public List<GiveNextUnitEffect> GiveNextEffects
    {
        get => giveNextEffects;
        private set => giveNextEffects = value;
    }
    private static List<GiveNextUnitEffect> giveNextEffects;
    
    /******
     * *****
     * ****** START_EFFECT_GROUP_LIST
     * *****
     *****/
    public void StartEffectGroupList(List<EffectGroup> groupList, GameObject source)
    {
        // TESTING TESTING TESTING
        if (effectSource != null)
        {
            EventManager.Instance.NewDelayedAction(() => StartEffectGroupList(groupList, source), 1f);
            return;
        }

        effectGroupList = groupList;
        effectSource = source;
        currentEffectGroup = 0;
        currentEffect = 0;
        newDrawnCards = new List<GameObject>();

        if (!CheckLegalTargets(effectGroupList, effectSource)) AbortEffectGroup();
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
        else if (group.Targets.TargetsAll ||
                 group.Targets.PlayerHero ||
                 group.Targets.EnemyHero  ||
                 group.Targets.TargetsSelf) return false;
        else return true;
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
        else StartNonTargetEffect(effect);
    }

    /******
     * *****
     * ****** START_NON_TARGET_EFFECT
     * *****
     *****/
    private void StartNonTargetEffect(Effect effect)
    {
        EffectTargets et = effectGroupList[currentEffectGroup].Targets;

        if (et.TargetsSelf)
        {
            acceptedTargets[currentEffectGroup].Add(effectSource);
            ConfirmNonTargetEffect();
            return;
        }

        if (effectSource.CompareTag(CardManager.PLAYER_CARD) || effectSource.CompareTag(CardManager.PLAYER_HERO))
        {
            if (et.PlayerHero) acceptedTargets[currentEffectGroup].Add(CardManager.Instance.PlayerHero);
            if (et.EnemyHero) acceptedTargets[currentEffectGroup].Add(CardManager.Instance.EnemyHero);

            if (et.TargetsAll)
            {
                if (et.PlayerUnit)
                {
                    foreach (GameObject card in CardManager.Instance.PlayerZoneCards)
                        acceptedTargets[currentEffectGroup].Add(card);
                }
                if (et.EnemyUnit)
                {
                    foreach (GameObject card in CardManager.Instance.EnemyZoneCards)
                        acceptedTargets[currentEffectGroup].Add(card);
                }
            }
        }
        else if (effectSource.CompareTag(CardManager.ENEMY_CARD) || effectSource.CompareTag(CardManager.ENEMY_HERO))
        {
            if (et.EnemyHero) acceptedTargets[currentEffectGroup].Add(CardManager.Instance.PlayerHero);
            if (et.PlayerHero) acceptedTargets[currentEffectGroup].Add(CardManager.Instance.EnemyHero);

            if (et.TargetsAll)
            {
                if (et.PlayerUnit)
                {
                    foreach (GameObject card in CardManager.Instance.EnemyZoneCards)
                        acceptedTargets[currentEffectGroup].Add(card);
                }
                if (et.EnemyUnit)
                {
                    foreach (GameObject card in CardManager.Instance.PlayerZoneCards)
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

        UIManager.Instance.PlayerIsTargetting = true;
        string targetDescription = effectGroupList[currentEffectGroup].EffectsDescription;
        if (string.IsNullOrEmpty(targetDescription)) targetDescription = 
                effectGroupList[currentEffectGroup].Effects[currentEffect].TargetDescription;
        UIManager.Instance.CreateInfoPopup(targetDescription);

        if (effect is DrawEffect)
        {
            foreach (GameObject newTarget in newDrawnCards)
                legalTargets[currentEffectGroup].Add(newTarget);

            newDrawnCards.Clear();
        }

        Debug.LogWarning("LEGAL TARGETS FOR GROUP " + (currentEffectGroup+1) + 
            " EFFECT " + (currentEffect+1) + ": " + legalTargets[currentEffectGroup].Count);

        foreach (GameObject target in legalTargets[currentEffectGroup])
        {
            if (target != null) target.GetComponent<CardSelect>().CardOutline.SetActive(true);
            else Debug.LogError("TARGET WAS NULL!");
        }
    }

    /******
     * *****
     * ****** CHECK_LEGAL_TARGETS
     * *****
     *****/
    public bool CheckLegalTargets(List<EffectGroup> groupList, GameObject source, bool isPreCheck = false)
    {
        void ClearTargets()
        {
            effectGroupList = null;
            effectSource = null;
            legalTargets = null;
            acceptedTargets = null;
        }

        effectGroupList = groupList;
        effectSource = source;
        legalTargets = new List<List<GameObject>>();
        acceptedTargets = new List<List<GameObject>>();

        for (int i = 0; i < effectGroupList.Count; i++)
        {
            legalTargets.Add(new List<GameObject>());
            acceptedTargets.Add(new List<GameObject>());
        }

        int group = 0;
        foreach (EffectGroup eg in effectGroupList)
        {
            Debug.Log(eg.ToString());
            foreach (Effect effect in eg.Effects)
            {
                Debug.Log(effect.ToString());
                if (IsTargetEffect(eg, effect))
                {
                    if (!GetLegalTargets(group, effect, eg.Targets))
                    {
                        Debug.LogWarning("NO LEGAL TARGETS!");
                        GameObject tempSource = effectSource;
                        ClearTargets();
                        effectSource = tempSource;
                        return false;
                    }
                }
            }
            group++;
        }
        if (isPreCheck) ClearTargets();
        return true;
    }

    /******
     * *****
     * ****** GET/CLEAR/SELECT_LEGAL_TARGETS
     * *****
     *****/
    private bool GetLegalTargets(int currentGroup, Effect effect, EffectTargets targets)
    {
        Debug.Log(effect.ToString() + " // " + targets.ToString());
        List<List<GameObject>> targetZones = new List<List<GameObject>>();

        if (effectSource.CompareTag(CardManager.PLAYER_CARD) || effectSource.CompareTag(CardManager.PLAYER_HERO))
        {
            if (targets.PlayerHand) targetZones.Add(CardManager.Instance.PlayerHandCards);
            if (targets.PlayerUnit) targetZones.Add(CardManager.Instance.PlayerZoneCards);
            if (targets.EnemyUnit) targetZones.Add(CardManager.Instance.EnemyZoneCards);

            // TESTING
            if (targets.PlayerHero) legalTargets[currentGroup].Add(CardManager.Instance.PlayerHero);
            if (targets.EnemyHero) legalTargets[currentGroup].Add(CardManager.Instance.EnemyHero);
        }
        else
        {
            if (targets.PlayerHand) targetZones.Add(CardManager.Instance.EnemyHandCards);
            if (targets.PlayerUnit) targetZones.Add(CardManager.Instance.EnemyZoneCards);
            if (targets.EnemyUnit) targetZones.Add(CardManager.Instance.PlayerZoneCards);

            // TESTING
            if (targets.EnemyHero) legalTargets[currentGroup].Add(CardManager.Instance.PlayerHero);
            if (targets.PlayerHero) legalTargets[currentGroup].Add(CardManager.Instance.EnemyHero);
        }

        foreach (List<GameObject> zone in targetZones)
        {
            foreach (GameObject target in zone) legalTargets[currentGroup].Add(target);
        }

        if (effect is DrawEffect || effect is GiveNextUnitEffect) return true;
        if (legalTargets[currentGroup].Count < 1) return false;
        if (effect.IsRequired && legalTargets[currentGroup].Count < 
            effectGroupList[currentGroup].Targets.TargetNumber) return false;
        return true;
    }

    public void SelectTarget(GameObject selectedCard)
    {
        foreach (GameObject card in legalTargets[currentEffectGroup])
        {
            if (card == selectedCard)
            {
                AcceptEffectTarget(card);
                return;
            }
        }
        RejectEffectTarget();
    }

    /******
     * *****
     * ****** ACCEPT/REJECT_TARGET
     * *****
     *****/
    private void AcceptEffectTarget(GameObject target)
    {
        AudioManager.Instance.StartStopSound("SFX_AcceptTarget");
        acceptedTargets[currentEffectGroup].Add(target);
        legalTargets[currentEffectGroup].Remove(target);
        target.GetComponent<CardSelect>().CardOutline.SetActive(false);
        
        EffectGroup eg = effectGroupList[currentEffectGroup];
        int targetNumber = eg.Targets.TargetNumber;

        if (!eg.Effects[currentEffect].IsRequired)
        {
            int possibleTargets = (legalTargets[currentEffectGroup].Count + 
                acceptedTargets[currentEffectGroup].Count);
            if (possibleTargets < targetNumber && possibleTargets > 0) 
                targetNumber = possibleTargets;
        }
        Debug.LogWarning("ACCEPTED TARGETS: <" + acceptedTargets[currentEffectGroup].Count +
            "> // TARGET NUMBER: <" + targetNumber + ">");

        if (acceptedTargets[currentEffectGroup].Count == targetNumber) ConfirmTargetEffect();
        else if (acceptedTargets[currentEffectGroup].Count > targetNumber)
            Debug.LogError("Accepted Targets > Target Number!");
    }
    private void RejectEffectTarget()
    {
        Debug.LogWarning("RejectEffectTarget()");
        AudioManager.Instance.StartStopSound("SFX_Error");
        // DISPLAY INFO POPUP
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
        if (effect is DrawEffect)
        {
            Debug.LogWarning("DRAW EFFECT!");
            string hero;
            if (eg.Targets.PlayerHand) hero = GameManager.PLAYER;
            else hero = GameManager.ENEMY;

            for (int i = 0; i < effect.Value; i++)
            {
                CardManager.Instance.DrawCard(hero);
            }
        }
        StartNextEffect();
    }
    private void ConfirmTargetEffect()
    {
        UIManager.Instance.PlayerIsTargetting = false;
        UIManager.Instance.DismissInfoPopup();
        foreach (GameObject target in legalTargets[currentEffectGroup])
        {
            target.GetComponent<CardSelect>().CardOutline.SetActive(false);
        }
        StartNextEffect();
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT
     * *****
     *****/
    public void ResolveEffect(List<GameObject> targets, Effect effect)
    {
        // DRAW
        if (effect is DrawEffect de)
        {
            EffectGroup eg = effectGroupList[currentEffectGroup];
            if (de.IsDiscardEffect)
            {
                string hero;
                if (eg.Targets.PlayerHand) hero = GameManager.PLAYER;
                else hero = GameManager.ENEMY;
                foreach (GameObject target in targets)
                {
                    CardManager.Instance.DiscardCard(target, hero);
                }
            }
        }
        // DAMAGE
        else if (effect is DamageEffect)
        {
            foreach (GameObject target in targets)
            {
                CardManager.Instance.TakeDamage(target, effect.Value);
            }
        }
        // HEALING
        else if (effect is HealEffect)
        {
            foreach (GameObject target in targets)
            {
                CardManager.Instance.HealDamage(target, effect.Value);
            }
        }
        // MARK
        else if (effect is MarkEffect)
        {

        }
        else if (effect is ExhaustEffect ee)
        {
            foreach (GameObject target in targets)
            {
                target.GetComponent<UnitCardDisplay>().IsExhausted = ee.SetExhausted;
            }
        }
        else if (effect is GiveNextUnitEffect gnfe)
        {
            GiveNextUnitEffect newGnfe = ScriptableObject.CreateInstance<GiveNextUnitEffect>();
            newGnfe.LoadEffect(gnfe);
            giveNextEffects.Add(newGnfe);
        }
        // STAT_CHANGE/GIVE_ABILITY
        else if (effect is StatChangeEffect || effect is GiveAbilityEffect)
        {
            foreach (GameObject target in targets)
            {
                CardManager.Instance.AddEffect(target, effect);
            }
        }
        else
        {
            Debug.LogError("EFFECT TYPE NOT FOUND!");
            return;
        }
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
        {
            AudioManager.Instance.StartStopSound(eg.EffectGroupSound);
            foreach (Effect effect in eg.Effects)
            {
                ResolveEffect(acceptedTargets[currentEffectGroup], effect);
            }
            currentEffectGroup++;
        }
        FinishEffectGroup();
    }

    /******
     * *****
     * ****** ABORT_EFFECT_GROUP
     * *****
     *****/
    private void AbortEffectGroup()
    {
        if (effectSource == null)
        {
            Debug.LogError("EFFECT SOURCE IS NULL!");
            return;
        }
        if (effectSource.TryGetComponent<ActionCardDisplay>(out _))
        {
            string zone;
            if (effectSource.CompareTag(CardManager.PLAYER_CARD)) zone = CardManager.PLAYER_HAND;
            else zone = CardManager.ENEMY_HAND;
            CardManager.Instance.ChangeCardZone(effectSource, zone);
            AnimationManager.Instance.RevealedHandState(effectSource);
        }
        FinishEffectGroup(true);
    }
    
    /******
     * *****
     * ****** FINISH_EFFECT_GROUP
     * *****
     *****/
    private void FinishEffectGroup(bool wasAborted = false)
    {
        Debug.LogWarning("FinishEffectGroup() WAS_ABORTED = <" + wasAborted + ">");

        if (!wasAborted && effectSource.TryGetComponent<ActionCardDisplay>(out _))
        {
            string hero;
            if (effectSource.CompareTag(CardManager.PLAYER_CARD)) hero = GameManager.PLAYER;
            else hero = GameManager.ENEMY;
            CardManager.Instance.DiscardCard(effectSource, hero, true);
        }

        effectGroupList = null;
        effectSource = null;
        currentEffect = 0;
        currentEffectGroup = 0;
        legalTargets = null;
        acceptedTargets = null;
        newDrawnCards = null;
    }
}
