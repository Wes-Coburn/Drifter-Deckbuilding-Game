using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static AnimationManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    
    public void ChangeAnimationState(GameObject go, string animationState)
    {
        if (go.TryGetComponent<ActionCardDisplay>(out _)) return;
        if (go.TryGetComponent<Animator>(out Animator anim))
            if (anim.enabled)
                anim.Play(animationState);
            else Debug.LogWarning("ANIMATOR NOT FOUND!");
    }

    /* HERO_ANIMATIONS */
    public void ModifyHealthState(GameObject hero) => ChangeAnimationState(hero, "Modify_Health");
    /* UNIT_ANIMATIONS */
    public void RevealedHandState(GameObject card) => ChangeAnimationState(card, "Revealed_Hand");
    public void RevealedPlayState(GameObject card) => ChangeAnimationState(card, "Revealed_Play");
    public void RevealedDragState(GameObject card) => ChangeAnimationState(card, "Revealed_Drag");
    public void DragPlayedState(GameObject card) => ChangeAnimationState(card, "Drag_Played");
    public void PlayedState(GameObject card)
    {
        card.GetComponent<CardDisplay>().CardArt = card.GetComponent<CardDisplay>().CardScript.CardArt;
        ChangeAnimationState(card, "Played");
    }
    //public void ModifyAttackState(GameObject card) => ChangeAnimationState(card, Modify_Attack);
    public void ModifyDefenseState(GameObject card) => ChangeAnimationState(card, "Modify_Defense");
    public void ZoomedState(GameObject card) => ChangeAnimationState(card, "Zoomed");
    /* UNIT_ATTACK */
    public void UnitAttack(GameObject attacker, GameObject defender, bool defenderIsUnit) => 
        StartCoroutine(AttackNumerator(attacker, defender, defenderIsUnit));

    private IEnumerator AttackNumerator(GameObject attacker, GameObject defender, bool defenderIsUnit = true)
    {
        float bufferDistance;
        float attackDelay;
        float retreatDelay;

        // UNIT
        if (defenderIsUnit)
        {
            bufferDistance = 250f;
            attackDelay = 0.04f;
            retreatDelay = 0.08f;
        }
        // HERO
        else
        {
            bufferDistance = 500f;
            attackDelay = 0.02f;
            retreatDelay = 0.04f;
        }

        float distance;
        int atkIndex = attacker.transform.GetSiblingIndex();
        Transform atkStartParent = attacker.transform.parent;
        Vector3 atkStartPos = attacker.transform.position;
        Vector2 defPos = defender.transform.position;
        attacker.transform.SetParent(UIManager.Instance.CurrentWorldSpace.transform);
        DragPlayedState(attacker);
        attacker.GetComponent<ChangeLayer>().ZoomLayer();
        // ATTACK
        do
        {
            distance = Vector2.Distance(attacker.transform.position, defPos);
            attacker.transform.position = Vector2.MoveTowards(attacker.transform.position, defPos, 100f);
            yield return new WaitForSeconds(attackDelay);
        }
        while (distance > bufferDistance);
        // RETREAT
        do
        {
            distance = Vector2.Distance(attacker.transform.position, atkStartPos);
            attacker.transform.position = Vector2.MoveTowards(attacker.transform.position, atkStartPos, 100f);
            yield return new WaitForSeconds(retreatDelay);
        }
        while (distance > 0);

        RevealedPlayState(attacker);
        attacker.GetComponent<ChangeLayer>().CardsLayer();
        attacker.transform.SetParent(atkStartParent);
        attacker.transform.position = new Vector3(atkStartPos.x, atkStartPos.y, CardManager.CARD_Z_POSITION);
        attacker.transform.SetSiblingIndex(atkIndex);
    }
}