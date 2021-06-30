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

    /* EFFECT_MANAGER_DATA */
    public const string ALLY = "Ally";

    /* CLASS_VARIABLES */
    private List<Effect> currentEffectGroup;
    private int currentEffect;
    private List<List<GameObject>> legalTargets;
    private List<List<GameObject>> acceptedTargets;
    private List<GameObject> newDrawnCards;


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
    public void StartNewEffectGroup(List<Effect> effectGroup, GameObject effectSource = null) // effectSource is unused
    {
        Debug.LogWarning(">>>StartNewEffectGroup()<<<");
        currentEffectGroup = effectGroup;
        currentEffect = 0;
        legalTargets = new List<List<GameObject>>();
        acceptedTargets = new List<List<GameObject>>();
        newDrawnCards = new List<GameObject>();

        for (int i = 0; i < effectGroup.Count; i++)
        {
            legalTargets.Add(new List<GameObject>());
            acceptedTargets.Add(new List<GameObject>());
        }

        int n = 0;
        foreach (Effect effect in currentEffectGroup)
        {
            if (IsTargetEffect(effect))
            {
                if (!GetLegalTargets(effect, n))
                {
                    AbortEffectGroup();
                    return;
                }
            }
            n++;
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
        if (effect is DrawEffect && ((DrawEffect)effect).IsDiscardEffect) return true;
        else if (effect.TargetsAll || effect.Targets == "Player" || effect.Targets == "Opponent") return false; // TESTING
        else return true;
    }

    /******
     * *****
     * ****** START_NEXT_EFFECT
     * *****
     *****/
    private void StartNextEffect(bool isFirstEffect = false)
    {
        Debug.Log(">>>StartNextEffect()<<<");
        if (!isFirstEffect) currentEffect++;

        if (currentEffect == currentEffectGroup.Count)
        {
            ResolveEffectGroup();
            return;
        }
        else if (currentEffect > currentEffectGroup.Count) Debug.LogError("ERROR: CurrentEffect > currentEffectGroup.Count");

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
        Debug.Log(">>>StartNonTargetEffect()<<<");
        ConfirmNonTargetEffect();
    }

    /******
     * *****
     * ****** START_TARGET_EFFECT
     * *****
     *****/
    private void StartTargetEffect(Effect effect)
    {
        Debug.Log(">>>StartTargetEffect()<<<");
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
            target.GetComponent<CardSelect>().CardOutline.SetActive(true);
        }
    }

    /******
     * *****
     * ****** GET/CLEAR/SELECT_LEGAL_TARGETS
     * *****
     *****/
    private bool GetLegalTargets(Effect effect, int currentEffect)
    {
        Debug.Log(">>>GetLegalTargets()<<<");

        List<GameObject> targetZoneCards = null;

        switch (effect.Targets)
        {
            case "PlayerHand":
                targetZoneCards = CardManager.Instance.PlayerHandCards;
                break;
            case "Ally":
                targetZoneCards = CardManager.Instance.PlayerZoneCards;
                break;
            case "Enemy":
                targetZoneCards = CardManager.Instance.EnemyZoneCards;
                break;
        }

        foreach (GameObject target in targetZoneCards) legalTargets[currentEffect].Add(target); 

        if (effect is DrawEffect) return true;
        if (legalTargets[currentEffect].Count < 1) return false;
        if (effect.IsRequired && legalTargets[currentEffect].Count < effect.TargetNumber) return false;
        return true;
    }

    public void SelectTarget(GameObject selectedCard)
    {
        Debug.Log(">>>SelectTarget()<<<");
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
        Debug.Log(">>>AcceptEffectTarget()<<<");
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
            Debug.LogError("ERROR: acceptedTargets[currentEffect].Count > targetNumber: " + acceptedTargets[currentEffect].Count + " > " + targetNumber);
    }
    private void RejectEffectTarget()
    {
        Debug.Log(">>>RejectEffectTarget()<<<");
        // UIManager InfoPopup
    }

    /******
     * *****
     * ****** CONFIRM_EFFECTS
     * *****
     *****/
    private void ConfirmNonTargetEffect() // TESTING
    {
        Debug.Log(">>>ConfirmNonTargetEffect()<<");
        Effect effect = currentEffectGroup[currentEffect];
        if (effect is DrawEffect)
        {
            for (int i = 0; i < effect.Value; i++)
            {
                GameObject card = CardManager.Instance.DrawCard(effect.Targets);
                newDrawnCards.Add(card);
            }
        }
        StartNextEffect();
    }
    private void ConfirmTargetEffect()
    {
        Debug.Log(">>>ConfirmTargetEffect()<<");
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
    private void ResolveEffect(List<GameObject> targets, Effect effect)
    {
        Debug.LogWarning(">>>ResolveTargetEffect()<<");
        if (effect is DrawEffect)
        {
            if (((DrawEffect)effect).IsDiscardEffect)
            {
                foreach (GameObject target in targets)
                {
                    CardManager.Instance.DiscardCard(target, effect.Targets);
                }
            }
        }
        else if (effect is DamageEffect)
        {
            foreach (GameObject target in targets)
            {
                CardManager.Instance.TakeDamage(target, effect.Value);
            }
        }
        else if (effect is HealingEffect)
        {

        }
        else if (effect is MarkEffect)
        {

        }
        else if (effect is StatChangeEffect)
        {
            StatChangeEffect ste = effect as StatChangeEffect;
            if (ste.IsDefenseChange)
            {
                if (effect.Value < 0)
                {
                    Debug.LogError("StatChangeEffect shouldn't be used to decrease defense! Use TakeDamage() instead");
                }
            }

            foreach (GameObject target in targets)
            {
                CardManager.Instance.ChangeStats(target, effect.Value, ste.IsDefenseChange, ste.IsTemporary);
            }
        }
        else if (effect is GiveAbilityEffect)
        {
            GiveAbilityEffect gae = effect as GiveAbilityEffect;
            foreach (GameObject target in targets)
            {
                target.GetComponent<HeroCardDisplay>().AddCurrentAbility(gae.CardAbility, gae.IsTemporary);
            }
        }
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT_GROUP
     * *****
     *****/
    private void ResolveEffectGroup()
    {
        Debug.LogWarning(">>>ResolveEffectGroup()<<<");

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
        Debug.LogWarning(">>>AbortEffectGroup()<<<");
        FinishEffectGroup(true);
    }

    /******
     * *****
     * ****** FINISH_EFFECT_GROUP
     * *****
     *****/
    private void FinishEffectGroup(bool wasAborted = false)
    {
        if (wasAborted) Debug.LogWarning(">>>FinishEffectGroup(wasAborted = true)<<<");
        else Debug.LogWarning(">>>FinishEffectGroup(wasAborted = false)<<<");
        currentEffect = 0;
        currentEffectGroup = null;
        legalTargets = null;
        acceptedTargets = null;
        newDrawnCards = null;
    }
}
