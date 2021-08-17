using UnityEngine;

public class DestroyTurnPopup : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UIManager.Instance.DestroyTurnPopup();
    }
}
