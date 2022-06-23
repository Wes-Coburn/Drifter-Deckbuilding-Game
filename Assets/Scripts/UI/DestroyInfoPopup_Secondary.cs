using UnityEngine;

public class DestroyInfoPopup_Secondary : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
        UIManager.Instance.DestroyInfoPopup(UIManager.InfoPopupType.Secondary);
}
