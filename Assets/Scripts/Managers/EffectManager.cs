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

    private void Start()
    {
        giveNextEffects = new List<GiveNextUnitEffect>();
    }

    /* CLASS_VARIABLES */
    private List<EffectGroup> effectGroupList;
    private GameObject effectSource;

    private int currentEffectGroup;
    private int currentEffect;

    private List<List<GameObject>> legalTargets;
    private List<List<GameObject>> acceptedTargets;
    private List<GameObject> newDrawnCards;

    public List<GiveNextUnitEffect> GiveNextEffects
    {
        get => giveNextEffects;
        private set => giveNextEffects = value;
    }
    private List<GiveNextUnitEffect> giveNextEffects;

    /******
     * *****
     * ****** START_EFFECT_GROUP_LIST
     * *****
     *****/
    public void StartEffectGroupList(List<EffectGroup> groupList, GameObject source)
    {
        Debug.LogWarning("START GROUP LIST");
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
        Debug.LogWarning("START NEXT GROUP");

        if (!isFirstGroup) currentEffectGroup++;

        if (currentEffectGroup < effectGroupList.Count)
        {
            Debug.Log("[GROUP #" + (currentEffectGroup + 1) +
                "] <" + effectGroupList[currentEffectGroup].ToString() + ">");

            StartNextEffect(true);
        }
        else if (currentEffectGroup == effectGroupList.Count)
        {
            ResolveEffectGroupList();
        }
        else Debug.LogError("EffectGroup > GroupList!");
    }

    /******
     * *****
     * ****** IS_TARGET_EFFECT
     * *****
     *****/
    private bool IsTargetEffect(Effect effect)
    {
        EffectGroup eg = effectGroupList[currentEffectGroup];

        if (effect is DrawEffect de && de.IsDiscardEffect) return true;
        else if (effect is DrawEffect || effect is GiveNextUnitEffect) return false;
        else if (eg.Targets.TargetsAll || 
            eg.Targets.PlayerHero || 
            eg.Targets.EnemyHero) return false;
        else return true;
    }

    /******
     * *****
     * ****** START_NEXT_EFFECT
     * *****
     *****/
    private void StartNextEffect(bool isFirstEffect = false)
    {
        Debug.LogWarning("START NEXT EFFECT");

        EffectGroup eg = effectGroupList[currentEffectGroup];

        if (!isFirstEffect) currentEffect++;
        
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
        if (IsTargetEffect(effect)) StartTargetEffect(effect);
        else StartNonTargetEffect(effect);
    }

    /******
     * *****
     * ****** START_NON_TARGET_EFFECT
     * *****
     *****/
    private void StartNonTargetEffect(Effect effect)
    {
        Debug.LogWarning("START NON_TARGET EFFECT");
        ConfirmNonTargetEffect();
    }

    /******
     * *****
     * ****** START_TARGET_EFFECT
     * *****
     *****/
    private void StartTargetEffect(Effect effect)
    {
        Debug.LogWarning("START TARGET EFFECT");

        if (acceptedTargets[currentEffectGroup].Count > 0)
        {
            StartNextEffectGroup(); // TESTING TESTING TESTING
            return;
        }

        UIManager.Instance.PlayerIsTargetting = true;
        string targetDescription = effectGroupList[currentEffectGroup].EffectsDescription;
        if (string.IsNullOrEmpty(targetDescription)) targetDescription = effectGroupList[currentEffectGroup].Effects[currentEffect].TargetDescription;
        UIManager.Instance.CreateInfoPopup(targetDescription);

        if (effect is DrawEffect)
        {
            foreach (GameObject newTarget in newDrawnCards)
            {
                legalTargets[currentEffectGroup].Add(newTarget);
            }
            newDrawnCards.Clear();
        }

        foreach (GameObject target in legalTargets[currentEffectGroup])
        {
            if (target != null) target.GetComponent<CardSelect>().CardOutline.SetActive(true);
            else Debug.LogWarning("TARGET WAS NULL!");
        }
    }

    /******
     * *****
     * ****** CHECK_LEGAL_TARGETS
     * *****
     *****/
    public bool CheckLegalTargets(List<EffectGroup> groupList, GameObject source, bool isPreCheck = false)
    {
        Debug.LogWarning("CHECK LEGAL TARGETS");

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
        
        foreach (EffectGroup eg in effectGroupList)
        {
            foreach (Effect effect in eg.Effects)
            {
                if (IsTargetEffect(effect))
                {
                    if (!GetLegalTargets(effect, eg.Targets))
                    {
                        ClearTargets();
                        Debug.LogWarning("NO VALID TARGETS!");
                        return false;
                    }
                }
            }
        }

        if (isPreCheck) ClearTargets();
        return true;
    }

    /******
     * *****
     * ****** GET/CLEAR/SELECT_LEGAL_TARGETS
     * *****
     *****/
    private bool GetLegalTargets(Effect effect, EffectTargets targets)
    {
        Debug.LogWarning("GET LEGAL TARGETS");

        List<List<GameObject>> targetZones = new List<List<GameObject>>();

        if (effectSource.CompareTag(CardManager.PLAYER_CARD) || effectSource.CompareTag(CardManager.PLAYER_HERO))
        {
            Debug.LogWarning("PLAYER TARGETTING!");
            if (targets.PlayerHand) targetZones.Add(CardManager.Instance.PlayerHandCards);
            if (targets.PlayerUnit) targetZones.Add(CardManager.Instance.PlayerZoneCards);
            if (targets.EnemyUnit) targetZones.Add(CardManager.Instance.EnemyZoneCards);
        }
        else
        {
            Debug.LogWarning("ENEMY TARGETTING!");
            if (targets.PlayerHand) targetZones.Add(CardManager.Instance.EnemyHandCards);
            if (targets.PlayerUnit) targetZones.Add(CardManager.Instance.EnemyZoneCards);
            if (targets.EnemyUnit) targetZones.Add(CardManager.Instance.PlayerZoneCards);
        }

        foreach (List<GameObject> zone in targetZones)
        {
            foreach (GameObject target in zone) legalTargets[currentEffectGroup].Add(target);
        }

        if (effect is DrawEffect || effect is GiveNextUnitEffect) return true;
        if (legalTargets[currentEffect].Count < 1) return false;
        if (effect.IsRequired && legalTargets[currentEffectGroup].Count < 
            effectGroupList[currentEffectGroup].Targets.TargetNumber) return false;
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
    private void AcceptEffectTarget(GameObject card)
    {
        Debug.LogWarning("ACCEPT EFFECT TARGET");

        acceptedTargets[currentEffectGroup].Add(card);
        legalTargets[currentEffectGroup].Remove(card);
        card.GetComponent<CardSelect>().CardOutline.SetActive(false);
        
        EffectGroup eg = effectGroupList[currentEffectGroup];
        int targetNumber = eg.Targets.TargetNumber;

        if (!eg.Effects[currentEffect].IsRequired)
        {
            int possibleTargets = (legalTargets[currentEffectGroup].Count + acceptedTargets[currentEffectGroup].Count);

            if (possibleTargets < targetNumber && possibleTargets > 0) targetNumber = possibleTargets;
        }

        Debug.LogWarning("ACCEPTED TARGETS: <" + acceptedTargets[currentEffectGroup].Count + "> // TARGET NUMBER: <" + targetNumber + ">");

        if (acceptedTargets[currentEffectGroup].Count == targetNumber) ConfirmTargetEffect();
        
        else if (acceptedTargets[currentEffectGroup].Count > targetNumber)
            Debug.LogError("Accepted Targets > Target Number!");
    }
    private void RejectEffectTarget()
    {
        Debug.LogWarning("RejectEffectTarget()");
        // DISPLAY INFO POPUP
    }

    /******
     * *****
     * ****** CONFIRM_EFFECTS
     * *****
     *****/
    private void ConfirmNonTargetEffect()
    {
        Debug.LogWarning("CONFIRM NON_TARGET EFFECT");

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
                GameObject card = CardManager.Instance.DrawCard(hero);
                if (card != null) newDrawnCards.Add(card);
                else Debug.LogError("CARD IS NULL!");
            }
        }
        StartNextEffect();
    }
    private void ConfirmTargetEffect()
    {
        Debug.LogWarning("CONFIRM TARGET EFFECT");

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
        Debug.LogWarning("RESOLVE EFFECT");

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
        else if (effect is HealingEffect)
        {

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
        else Debug.LogError("EFFECT TYPE NOT FOUND!");
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT_GROUP_LIST
     * *****
     *****/
    private void ResolveEffectGroupList()
    {
        Debug.LogWarning("RESOLVE GROUP LIST");

        currentEffectGroup = 0;
        currentEffect = 0; // Unnecessary

        foreach (EffectGroup eg in effectGroupList)
        {
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
        Debug.LogWarning("ABORT EFFECT GROUP");

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
        Debug.LogWarning("FINISH EFFECT GROUP");

        Debug.LogWarning("FinishEffectGroup() WAS_ABORTED = <" + wasAborted + ">");

        if (!wasAborted && effectSource.TryGetComponent<ActionCardDisplay>(out _))
        {
            string hero;
            if (effectSource.CompareTag(CardManager.PLAYER_CARD)) hero = GameManager.PLAYER;
            else hero = GameManager.ENEMY;
            CardManager.Instance.DiscardCard(effectSource, hero);
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
