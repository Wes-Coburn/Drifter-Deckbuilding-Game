using UnityEngine;

public class DestroyInfoPopup : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
        UIManager.Instance.DestroyInfoPopup(false);
}
