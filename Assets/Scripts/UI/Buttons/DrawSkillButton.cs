using UnityEngine;
using UnityEngine.EventSystems;

public class DrawSkillButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (!PlayerManager.Instance.IsMyTurn || EffectManager.Instance.EffectsResolving ||
            EventManager.Instance.ActionsDelayed) return;
        PlayerManager.Instance.DrawSkill();
    }
}
