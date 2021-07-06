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
    private GameObject currentEffectSource;
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
    public void StartEffectGroup(List<Effect> effectGroup, GameObject effectSource = null)
    {
        Debug.LogWarning("StartNewEffectGroup()");
        currentEffectSource = effectSource; // TESTING
        currentEffectGroup = effectGroup;
        currentEffect = 0;
        newDrawnCards = new List<GameObject>();

        if (!CheckLegalTargets(currentEffectGroup))
        {
            AbortEffectGroup();
            return;
        }
        StartNextEffect(true);
    }

    /******
     * *****
     * ****** CHECK_LEGAL_TARGETS
     * *****
     *****/
    public bool CheckLegalTargets(List<Effect> effectGroup, bool preCheck = false)
    {
        legalTargets = new List<List<GameObject>>();
        acceptedTargets = new List<List<GameObject>>();

        for (int i = 0; i < effectGroup.Count; i++)
        {
            Debug.Log("<Effect " + (i + 1) + "> <" + effectGroup[i].ToString() + ">");
            legalTargets.Add(new List<GameObject>());
            acceptedTargets.Add(new List<GameObject>());
        }

        int n = 0;
        foreach (Effect effect in effectGroup)
        {
            if (IsTargetEffect(effect))
            {
                if (!GetLegalTargets(effect, n))
                {
                    legalTargets = null;
                    Debug.LogWarning("CheckLegalTargets() returned FALSE");
                    return false;
                }
            }
            n++;
        }
        if (preCheck)
        {
            legalTargets = null;
            acceptedTargets = null;
        }
        return true;
    }

    /******
     * *****
     * ****** IS_TARGET_EFFECT
     * *****
     *****/
    private bool IsTargetEffect(Effect effect)
    {
        if (effect is DrawEffect de && de.IsDiscardEffect) return true;
        else if (effect is DrawEffect) return false;
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
        if (!isFirstEffect) currentEffect++;
        
        if (currentEffect < currentEffectGroup.Count)
        {
            Debug.Log("StartNextEffect() #" + (currentEffect + 1) + "/" + currentEffectGroup.Count + 
                ": <" + currentEffectGroup[currentEffect].ToString() + ">");
        }

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
        Debug.LogWarning("StartNonTargetEffect()");
        ConfirmNonTargetEffect();
    }

    /******
     * *****
     * ****** START_TARGET_EFFECT
     * *****
     *****/
    private void StartTargetEffect(Effect effect)
    {
        Debug.LogWarning("StartTargetEffect()");
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
        Debug.Log("GetLegalTargets() <" + effect.ToString() + ">");

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
        Debug.LogWarning("SelectTarget()");
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
        Debug.LogWarning("AcceptEffectTarget()");
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
            Debug.LogError("ERROR: acceptedTargets > targetNumber: " + acceptedTargets[currentEffect].Count + " > " + targetNumber);
    }
    private void RejectEffectTarget()
    {
        Debug.LogWarning("RejectEffectTarget()");
    }

    /******
     * *****
     * ****** CONFIRM_EFFECTS
     * *****
     *****/
    private void ConfirmNonTargetEffect() // TESTING
    {
        Debug.LogWarning("ConfirmNonTargetEffect()");
        Effect effect = currentEffectGroup[currentEffect];
        if (effect is DrawEffect)
        {
            string player;
            if (effect.Targets == CardManager.PLAYER_HAND) player = GameManager.PLAYER;
            else player = GameManager.ENEMY;

            for (int i = 0; i < effect.Value; i++)
            {
                GameObject card = CardManager.Instance.DrawCard(player);
                newDrawnCards.Add(card);
            }
        }
        StartNextEffect();
    }
    private void ConfirmTargetEffect()
    {
        Debug.LogWarning("ConfirmTargetEffect()");
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
        Debug.Log("ResolveEffect() <" + effect.ToString() + ">");
        
        // DRAW
        if (effect is DrawEffect de)
        {
            if (de.IsDiscardEffect)
            {
                string player;
                if (de.Targets == CardManager.PLAYER_HAND) player = GameManager.PLAYER;
                else player = GameManager.ENEMY;
                foreach (GameObject target in targets)
                {
                    CardManager.Instance.DiscardCard(target, player);
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
        // STAT_CHANGE
        else if (effect is StatChangeEffect)
        {
            foreach (GameObject target in targets)
            {
                CardManager.Instance.AddEffect(target, effect);
            }
        }
        // GIVE_ABILITY
        else if (effect is GiveAbilityEffect gae)
        {
            foreach (GameObject target in targets)
            {
                HeroCardDisplay hcd = target.GetComponent<HeroCardDisplay>();
                if (gae.Countdown != 0) 
                    hcd.AddTemporaryAbility(gae.CardAbility, gae.Countdown);
                else hcd.AddCurrentAbility(gae.CardAbility);
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
        Debug.LogWarning("ResolveEffectGroup()");

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
        Debug.LogWarning("AbortEffectGroup()");

        if (currentEffectSource != null)
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
        Debug.LogWarning("FinishEffectGroup(wasAborted = " + wasAborted + ")");

        if (currentEffectSource != null && !wasAborted)
        {
            string player;
            if (currentEffectSource.CompareTag(CardManager.PLAYER_CARD)) player = GameManager.PLAYER;
            else player = GameManager.ENEMY;
            CardManager.Instance.DiscardCard(currentEffectSource, player);
        }

        currentEffectSource = null;
        currentEffect = 0;
        currentEffectGroup = null;
        legalTargets = null;
        acceptedTargets = null;
        newDrawnCards = null;
    }
}
