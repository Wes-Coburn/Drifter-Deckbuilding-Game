using UnityEngine;
using UnityEngine.EventSystems;

public class DrawSkillButton : MonoBehaviour, IPointerClickHandler
{
    private PlayerManager pMan;
    private EndTurnButton endTurnButton;
    private void Start()
    {
        pMan = PlayerManager.Instance;
        endTurnButton = FindObjectOfType<EndTurnButton>();
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;

        if (!pMan.IsMyTurn || !endTurnButton.IsInteractable) return; // TESTING

        pMan.DrawSkill();
    }
}
