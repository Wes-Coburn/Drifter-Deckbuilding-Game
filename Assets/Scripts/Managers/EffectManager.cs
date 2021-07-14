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
        giveNextEffects = new List<GiveNextFollowerEffect>();
    }

    /* CLASS_VARIABLES */
    private List<Effect> currentEffectGroup;
    private int currentEffect;
    private GameObject currentEffectSource;
    private List<List<GameObject>> legalTargets;
    private List<List<GameObject>> acceptedTargets;
    private List<GameObject> newDrawnCards;

    public List<GiveNextFollowerEffect> GiveNextEffects // TESTING
    {
        get => giveNextEffects;
        private set => giveNextEffects = value;
    }
    private List<GiveNextFollowerEffect> giveNextEffects;

    /*
     *                                                      >>> START <<<
     *                                                      
     *                                                    StartNewEffectGroup
     *                                                            |
     *                                                    StartNextEffect
     *                                                            |
     *                                                      GetLegalTargets
     *                                                            |
     *                                     StartNonTargetEffect ------ StartTargetEffect
     *                                              |                           |
     *                                    ConfirmNonTargetEffect        __SelectTarget__
     *                                                                 |                |
     *                                                            AcceptTarget     RejectTarget
     *                                                                 |
     *                                               [When all targets have been accepted]
     *                                                                 |
     *                                                         ConfirmTargetEffect
     *                                                         
     *                                                       ___________
     *                                                            |
     *                                                     StartNextEffect
     *                                                            |
     *                                           [When all effects have been confirmed]
     *                                                            |
     *                                                ____ResolveEffectGroup____
     *                                               |                          |
     *                                    ResolveNonTargetEffect       ResolveTargetEffect
     *                                   
     *                                                       ___________
     *                                                            |
     *                                         [When all effects have been resolved]
     *                                                            |
     *                                                    FinishEffectGroup
     *                                                    
     *                                                       >>> END <<<
     */

    /******
     * *****
     * ****** START_NEW_EFFECT_GROUP
     * *****
     *****/
    public void StartEffectGroup(List<Effect> effectGroup, GameObject effectSource)
    {
        Debug.LogWarning("StartNewEffectGroup()");
        currentEffectSource = effectSource; // TESTING
        currentEffectGroup = effectGroup;
        currentEffect = 0;
        newDrawnCards = new List<GameObject>();
        
        if (!CheckLegalTargets(currentEffectGroup, effectSource))
        {
            AbortEffectGroup();
            return;
        }
        StartNextEffect(true);
    }

    /******
     * *****
     * ****** IS_TARGET_EFFECT
     * *****
     *****/
    private bool IsTargetEffect(Effect effect)
    {
        if (effect is DrawEffect de && de.IsDiscardEffect) return true;
        else if (effect is DrawEffect || effect is GiveNextFollowerEffect) return false;
        else if (effect.TargetsAll || effect.Targets == CardManager.PLAYER_HERO || effect.Targets == CardManager.ENEMY_HERO) return false;
        else return true;
    }

    /******
     * *****
     * ****** START_NEXT_EFFECT
     * *****
     *****/
    private void StartNextEffect(bool isFirstEffect = false)
    {
        if (!isFirstEffect) currentEffect++;
        
        if (currentEffect < currentEffectGroup.Count)
        {
            Debug.Log("StartNextEffect() [EFFECT #" + (currentEffect + 1) + 
                "] <" + currentEffectGroup[currentEffect].ToString() + ">");
        }
        if (currentEffect == currentEffectGroup.Count)
        {
            ResolveEffectGroup();
            return;
        }
        else if (currentEffect > currentEffectGroup.Count) 
            Debug.LogError("ERROR: CURRENT_EFFECT > CURRENT_EFFECT_GROUP");

        Effect effect = currentEffectGroup[currentEffect];
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
        ConfirmNonTargetEffect();
    }

    /******
     * *****
     * ****** START_TARGET_EFFECT
     * *****
     *****/
    private void StartTargetEffect(Effect effect)
    {
        UIManager.Instance.PlayerIsTargetting = true;

        string infoText;
        if (effect is DrawEffect) infoText = "Choose a card in your hand";
        else infoText = "Choose an " + effect.Targets.ToLower();
        UIManager.Instance.CreateInfoPopup(infoText);

        if (effect is DrawEffect)
        {
            foreach (GameObject newTarget in newDrawnCards)
            {
                legalTargets[currentEffect].Add(newTarget);
            }
            newDrawnCards.Clear();
        }

        foreach (GameObject target in legalTargets[currentEffect])
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
    public bool CheckLegalTargets(List<Effect> effectGroup, GameObject effectSource, bool isPreCheck = false)
    {
        if (effectGroup.Count < 1)
        {
            Debug.LogError("EFFECT GROUP IS EMPTY!");
            return false;
        }

        legalTargets = new List<List<GameObject>>();
        acceptedTargets = new List<List<GameObject>>();

        for (int i = 0; i < effectGroup.Count; i++)
        {
            legalTargets.Add(new List<GameObject>());
            acceptedTargets.Add(new List<GameObject>());
        }

        int n = 0;
        foreach (Effect effect in effectGroup)
        {
            if (IsTargetEffect(effect))
            {
                if (!GetLegalTargets(effect, n, effectSource))
                {
                    legalTargets = null;
                    Debug.LogWarning("CheckLegalTargets() RETURNED FALSE!");
                    return false;
                }
            }
            n++;
        }
        if (isPreCheck)
        {
            legalTargets = null;
            acceptedTargets = null;
        }
        return true;
    }

    /******
     * *****
     * ****** GET/CLEAR/SELECT_LEGAL_TARGETS
     * *****
     *****/
    private bool GetLegalTargets(Effect effect, int currentEffect, GameObject effectSource)
    {
        List<GameObject> targetZoneCards = null;

        if (effectSource.CompareTag(CardManager.PLAYER_CARD) || effectSource.CompareTag(CardManager.PLAYER_HERO))
        {
            switch (effect.Targets)
            {
                case CardManager.PLAYER_HAND:
                    targetZoneCards = CardManager.Instance.PlayerHandCards;
                    break;
                case CardManager.PLAYER_FOLLOWER:
                    targetZoneCards = CardManager.Instance.PlayerZoneCards;
                    break;
                case CardManager.ENEMY_FOLLOWER:
                    targetZoneCards = CardManager.Instance.EnemyZoneCards;
                    break;
            }
        }
        else
        {
            switch (effect.Targets) // TESTING
            {
                case CardManager.PLAYER_HAND:
                    targetZoneCards = CardManager.Instance.EnemyHandCards;
                    break;
                case CardManager.PLAYER_FOLLOWER:
                    targetZoneCards = CardManager.Instance.EnemyZoneCards;
                    break;
                case CardManager.ENEMY_FOLLOWER:
                    targetZoneCards = CardManager.Instance.PlayerZoneCards;
                    break;
            }
        }

        foreach (GameObject target in targetZoneCards) legalTargets[currentEffect].Add(target); 

        if (effect is DrawEffect || effect is GiveNextFollowerEffect) return true;
        if (legalTargets[currentEffect].Count < 1) return false;
        if (effect.IsRequired && legalTargets[currentEffect].Count < effect.TargetNumber) return false;
        return true;
    }

    public void SelectTarget(GameObject selectedCard)
    {
        foreach (GameObject card in legalTargets[currentEffect])
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
        acceptedTargets[currentEffect].Add(card);
        legalTargets[currentEffect].Remove(card);
        card.GetComponent<CardSelect>().CardOutline.SetActive(false);

        int targetNumber = currentEffectGroup[currentEffect].TargetNumber;
        if (!currentEffectGroup[currentEffect].IsRequired)
        {
            if ((legalTargets[currentEffect].Count + acceptedTargets[currentEffect].Count) < targetNumber)
            {
                targetNumber = (legalTargets[currentEffect].Count + acceptedTargets[currentEffect].Count);
            }
        }

        if (acceptedTargets[currentEffect].Count == targetNumber) ConfirmTargetEffect();
        else if (acceptedTargets[currentEffect].Count > targetNumber) 
            Debug.LogError("ERROR: ACCEPTED_TARGETS > TARGET_NUMBER ::: " + 
                acceptedTargets[currentEffect].Count + " > " + targetNumber);
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
        Effect effect = currentEffectGroup[currentEffect];
        if (effect is DrawEffect)
        {
            string hero;
            if (effect.Targets == CardManager.PLAYER_HAND) hero = GameManager.PLAYER;
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
        UIManager.Instance.PlayerIsTargetting = false;
        UIManager.Instance.DismissInfoPopup();

        foreach (GameObject target in legalTargets[currentEffect])
        {
            target.GetComponent<CardSelect>().CardOutline.SetActive(false);
        }
        StartNextEffect();
    }

    /******
     * *****
     * ****** RESOLVE_EFFECTS
     * *****
     *****/
    public void ResolveEffect(List<GameObject> targets, Effect effect)
    {
        // DRAW
        if (effect is DrawEffect de)
        {
            if (de.IsDiscardEffect)
            {
                string hero;
                if (de.Targets == CardManager.PLAYER_HAND) hero = GameManager.PLAYER;
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
        else if (effect is ExhaustEffect ee) // TESTING
        {
            foreach (GameObject target in targets)
            {
                target.GetComponent<FollowerCardDisplay>().IsExhausted = ee.SetExhausted;
            }
        }
        else if (effect is GiveNextFollowerEffect gnfe)
        {
            GiveNextFollowerEffect newGnfe = ScriptableObject.CreateInstance<GiveNextFollowerEffect>();
            newGnfe.LoadEffect(gnfe);
            giveNextEffects.Add(newGnfe); // TESTING
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
     * ****** RESOLVE_EFFECT_GROUP
     * *****
     *****/
    private void ResolveEffectGroup()
    {
        currentEffect = 0;
        foreach (Effect effect in currentEffectGroup)
        {
            ResolveEffect(acceptedTargets[currentEffect++], effect);
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
        if (currentEffectSource.TryGetComponent<ActionCardDisplay>(out _))
        {
            string zone;
            if (currentEffectSource.CompareTag(CardManager.PLAYER_CARD)) zone = CardManager.PLAYER_HAND;
            else zone = CardManager.ENEMY_HAND;
            CardManager.Instance.ChangeCardZone(currentEffectSource, zone);
            AnimationManager.Instance.RevealedHandState(currentEffectSource); // TESTING
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

        if (!wasAborted && currentEffectSource.TryGetComponent<ActionCardDisplay>(out _))
        {
            string hero;
            if (currentEffectSource.CompareTag(CardManager.PLAYER_CARD)) hero = GameManager.PLAYER;
            else hero = GameManager.ENEMY;
            CardManager.Instance.DiscardCard(currentEffectSource, hero);
        }

        currentEffectSource = null;
        currentEffect = 0;
        currentEffectGroup = null;
        legalTargets = null;
        acceptedTargets = null;
        newDrawnCards = null;
    }
}
