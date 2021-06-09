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
    private List<Effect> currentEffectList;
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
     *                                     StartNonTargetEffect ------ StartTargetEffect
     *                                              |                           |
     *                                    ConfirmNonTargetEffect        GetLegalTargets
     *                                                                         |
     *                                                                  __SelectTarget__
     *                                                                 |                |
     *                                                            AcceptTarget     RejectTarget
     *                                                                 |
     *                                               [When all targets have been selected]
     *                                                                 |
     *                                                         ConfirmTargetEffect
     *                                                         
     *                                                       ___________
     *                                                            |
     *                                                     StartNextEffect
     *                                                            |
     *                                            [When all effects have been confirmed]
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
    public void StartNewEffectGroup(List<Effect> effectGroup)
    {
        Debug.Log(">>>StartNewEffectGroup()<<<");
        currentEffectList = effectGroup;
        currentEffect = 0;
        legalTargets = new List<List<GameObject>>();
        acceptedTargets = new List<List<GameObject>>();

        for (int i = 0; i < effectGroup.Count; i++)
        {
            legalTargets.Add(new List<GameObject>());
            acceptedTargets.Add(new List<GameObject>());
        }

        Debug.LogWarning("Legal/accepted targets counts = " + legalTargets.Count + " / " + acceptedTargets.Count);
        StartNextEffect(true);
    }

    /******
     * *****
     * ****** IS_TARGET_EFFECT
     * *****
     *****/
    private bool IsTargetEffect(Effect effect)
    {
        if (effect.Targets == "Self" || effect.Targets == "Opponent")
        {
            return false;
        }
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

        if (currentEffect >= currentEffectList.Count)
        {
            ResolveEffectGroup();
            return;
        }

        Effect effect = currentEffectList[currentEffect];
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
        Debug.Log(">>>StartTargetEffect()<<<");
        UIManager.Instance.PlayerIsTargetting = true;
        GetLegalTargets(effect); // get legal targets
        // UIManager CenterScreenPopup
    }

    /******
     * *****
     * ****** GET/CLEAR/SELECT_LEGAL_TARGETS
     * *****
     *****/
    private void GetLegalTargets(Effect effect)
    {
        Debug.Log(">>>GetLegalTargets()<<<");

        if (effect.Targets == "Ally")
        {
            Debug.Log("Legal target is an ALLY!");
            foreach (GameObject card in CardManager.Instance.playerZoneCards)
            {
                legalTargets[currentEffect].Add(card);
            }
        }
        else if (effect.Targets == "Enemy")
        {
            Debug.Log("Legal target is an ENEMY!");
            foreach (GameObject card in CardManager.Instance.enemyZoneCards)
            {
                legalTargets[currentEffect].Add(card);
            }
        }

        Debug.LogWarning("*/*/* Legal Targets */*/*");
        int i = 1;
        foreach (GameObject card in legalTargets[currentEffect]) // FOR TESTING ONLY
        {
            Debug.Log(i++ + ") " + card.GetComponent<HeroCardDisplay>().GetCardName());
        }
        
    }
    private void ClearLegalTargets() // TESTING
    {
        legalTargets.Clear();
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
        Debug.Log(">>>AcceptEffectTarget()<<<");
        acceptedTargets[currentEffect].Add(card);
        if (acceptedTargets[currentEffect].Count == currentEffectList[currentEffect].TargetNumber)
        {
            ConfirmTargetEffect();
        }
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
        ClearLegalTargets();
        StartNextEffect();
    }

    /******
     * *****
     * ****** RESOLVE_EFFECTS
     * *****
     *****/
    private void ResolveNonTargetEffect(Effect effect) // TESTING
    {
        Debug.Log(">>>ResolveNonTargetEffect()<<");
    }
    private void ResolveTargetEffect(List<GameObject> targets, Effect effect) // TESTING
    {
        Debug.Log(">>>ResolveTargetEffect()<<");
        if (effect is DamageEffect)
        {

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
        Debug.Log(">>>ResolveEffectGroup()<<<");

        // iterate through currentEffectList and resolve each effect

        currentEffect = 0;
        foreach (Effect effect in currentEffectList)
        {
            if (IsTargetEffect(effect))
            {
                ResolveTargetEffect(acceptedTargets[currentEffect++], effect);
            }
            else ResolveNonTargetEffect(effect);
        }

        FinishEffectGroup();
    }

    /******
     * *****
     * ****** FINISH_EFFECT_GROUP
     * *****
     *****/
    private void FinishEffectGroup() // TESTING
    {
        Debug.Log(">>>FinishEffectGroup()<<<");
    }
}
