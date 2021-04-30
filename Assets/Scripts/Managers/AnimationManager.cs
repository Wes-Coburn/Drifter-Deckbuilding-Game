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

    //private const string HIDDEN_STATE = AnimationManagerData.HIDDEN_STATE;
    private const string EXHAUSTED_STATE = AnimationManagerData.EXHAUSTED_STATE;
    private const string REVEALED_STATE = AnimationManagerData.REVEALED_STATE;
    private const string PLAYED_STATE = AnimationManagerData.PLAYED_STATE;
    //private const string MODIFY_ATTACK_STATE = AnimationManagerData.MODIFY_ATTACK_STATE;
    private const string MODIFY_DEFENSE_STATE = AnimationManagerData.MODIFY_DEFENSE_STATE;

    private void ChangeAnimationState (GameObject card, string animationState)
    {
        Animator anim = card.GetComponent<Animator>();
        anim.Play(animationState);
    }
    //public void HiddenState (GameObject card) => ChangeAnimationState(card, HIDDEN_STATE);
    public void ExhaustedState(GameObject card) => ChangeAnimationState(card, EXHAUSTED_STATE);
    public void RevealedState (GameObject card) => ChangeAnimationState(card, REVEALED_STATE);
    public void PlayedState (GameObject card) => ChangeAnimationState(card, PLAYED_STATE);
    //public void ModifyAttackState(GameObject card) => ChangeAnimationState(card, MODIFY_ATTACK_STATE);
    public void ModifyDefenseState(GameObject card) => ChangeAnimationState(card, MODIFY_DEFENSE_STATE);
}
