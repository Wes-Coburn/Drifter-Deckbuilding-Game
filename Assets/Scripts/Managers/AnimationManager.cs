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
        Animator anim = go.GetComponent<Animator>();
        anim.Play(animationState);
    }
    public void HiddenState(GameObject card)
    {
        card.GetComponent<CardDisplay>().CardArt = CardManager.Instance.CardBackSprite;
        ChangeAnimationState(card, "Hidden");
    }
    public void RevealedHandState (GameObject card) => ChangeAnimationState(card, "Revealed_Hand");
    public void RevealedPlayState (GameObject card) => ChangeAnimationState(card, "Revealed_Play");
    public void RevealedDragState(GameObject card) => ChangeAnimationState(card, "Revealed_Drag");
    public void PlayedState (GameObject card)
    {
        card.GetComponent<CardDisplay>().CardArt = card.GetComponent<CardDisplay>().CardScript.CardArt;
        ChangeAnimationState(card, "Played");
    }
    //public void ModifyAttackState(GameObject card) => ChangeAnimationState(card, Modify_Attack);
    public void ModifyDefenseState(GameObject card) => ChangeAnimationState(card, "Modify_Defense");
}
