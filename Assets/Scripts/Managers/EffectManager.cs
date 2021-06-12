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

    /* CLASS_VARIABLES */
    private List<Effect> currentEffectGroup;
    private int currentEffect;
    private List<List<GameObject>> legalTargets;
    private List<List<GameObject>> acceptedTargets;


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
        if (effect.TargetsAll || effect.Targets == "Self" || effect.Targets == "Opponent") return false; // TESTING
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
        // UIManager CenterScreenPopup

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

        if (effect.Targets == "Ally")
        {
            Debug.Log("Effect target is " + effect.TargetNumber + " ALLY(S)!");
            Debug.Log("<<< LEGAL TARGETS >>>");
            int i = 1;
            foreach (GameObject card in CardManager.Instance.playerZoneCards)
            {
                legalTargets[currentEffect].Add(card);
                Debug.Log(i++ + ") " + card.GetComponent<HeroCardDisplay>().GetCardName());
            }
        }
        else if (effect.Targets == "Enemy")
        {
            Debug.Log("Effect target is " + effect.TargetNumber + " ENEMY(S)!");
            Debug.Log("<<< LEGAL TARGETS >>>");
            int i = 1;
            foreach (GameObject card in CardManager.Instance.enemyZoneCards)
            {
                legalTargets[currentEffect].Add(card);
                Debug.Log(i++ + ") " + card.GetComponent<HeroCardDisplay>().GetCardName());
            }
        }

        if (legalTargets[currentEffect].Count < 1) return false;
        else if (effect.IsRequired && legalTargets[currentEffect].Count < effect.TargetNumber) return false;

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

        Debug.LogWarning("Legal targets remaining: " + legalTargets[currentEffect].Count);

        int targetNumber = currentEffectGroup[currentEffect].TargetNumber;
        
        if (!currentEffectGroup[currentEffect].IsRequired)
        {
            if ((legalTargets[currentEffect].Count + acceptedTargets[currentEffect].Count) < targetNumber)
            {
                targetNumber = (legalTargets[currentEffect].Count + acceptedTargets[currentEffect].Count);
            }
        }

        if (acceptedTargets[currentEffect].Count == targetNumber)
        {
            ConfirmTargetEffect();
        }
        else if (acceptedTargets[currentEffect].Count > targetNumber) 
            Debug.LogError("ERROR: acceptedTargets[currentEffect].Count > targetNumber: " + acceptedTargets[currentEffect].Count + " > " + targetNumber);
    }
    private void RejectEffectTarget() // TESTING
    {
        Debug.Log(">>>RejectEffectTarget()<<<");
        // UIManager CenterScreenPopup
    }

    /******
     * *****
     * ****** CONFIRM_EFFECTS
     * *****
     *****/
    private void ConfirmNonTargetEffect() // TESTING
    {
        Debug.Log(">>>ConfirmNonTargetEffect()<<");
        StartNextEffect();
    }
    private void ConfirmTargetEffect() // TESTING
    {
        Debug.Log(">>>ConfirmTargetEffect()<<");
        UIManager.Instance.PlayerIsTargetting = false;

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
    private void ResolveNonTargetEffect(Effect effect) // TESTING
    {
        Debug.LogWarning(">>>ResolveNonTargetEffect()<<");
    }
    private void ResolveTargetEffect(List<GameObject> targets, Effect effect) // TESTING
    {
        Debug.LogWarning(">>>ResolveTargetEffect()<<");
        if (effect is DrawEffect)
        {
            for (int i= 0; i < effect.Value; i++)
            {
                CardManager.Instance.DrawCard(effect.Targets);
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
    }

    /******
     * *****
     * ****** RESOLVE_EFFECT_GROUP
     * *****
     *****/
    private void ResolveEffectGroup() // TESTING
    {
        Debug.LogWarning(">>>ResolveEffectGroup()<<<");

        // iterate through currentEffectList and resolve each effect

        currentEffect = 0;
        foreach (Effect effect in currentEffectGroup)
        {
            if (IsTargetEffect(effect))
            {
                ResolveTargetEffect(acceptedTargets[currentEffect], effect);
            }
            else ResolveNonTargetEffect(effect);
            currentEffect++;
        }
        FinishEffectGroup();
    }

    /******
     * *****
     * ****** ABORT_EFFECT_GROUP
     * *****
     *****/
    private void AbortEffectGroup() // TESTING
    {
        Debug.LogWarning(">>>AbortEffectGroup()<<<");
        FinishEffectGroup();
    }

    /******
     * *****
     * ****** FINISH_EFFECT_GROUP
     * *****
     *****/
    private void FinishEffectGroup() // TESTING
    {
        Debug.LogWarning(">>>FinishEffectGroup()<<<");
        currentEffect = 0;
        currentEffectGroup = null;
        legalTargets = null;
        acceptedTargets = null;
    }
}
