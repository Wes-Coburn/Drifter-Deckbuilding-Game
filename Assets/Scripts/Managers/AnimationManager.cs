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

    private void ChangeAnimationState (GameObject card, string animationState)
    {
        Animator anim = card.GetComponent<Animator>();
        anim.Play(animationState);
    }
    //public void HiddenState (GameObject card) => ChangeAnimationState(card, HIDDEN_STATE);
    public void ExhaustedState(GameObject card) => ChangeAnimationState(card, "Hidden");
    public void RevealedState (GameObject card) => ChangeAnimationState(card, "Revealed");
    public void PlayedState (GameObject card) => ChangeAnimationState(card, "Played");
    //public void ModifyAttackState(GameObject card) => ChangeAnimationState(card, Modify_Attack);
    public void ModifyDefenseState(GameObject card) => ChangeAnimationState(card, "Modify_Defense");
}
