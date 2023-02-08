using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelect : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject cardOutline;

    public GameObject CardOutline { get => cardOutline; }


    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (ManagerHandler.U_MAN.PlayerIsTargetting) ManagerHandler.EF_MAN.SelectEffectTarget(gameObject);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard == gameObject) return;

        if (ManagerHandler.U_MAN.PlayerIsTargetting) ManagerHandler.EF_MAN.HighlightEffectTarget(gameObject, true);
        else if (DragDrop.ArrowIsDragging)
        {
            if (ManagerHandler.EN_MAN.HandZoneCards.Contains(gameObject)) return;

            DragDrop.Enemy = gameObject;
            UIManager.SelectionType type;

            if (ManagerHandler.CO_MAN.CanAttack(DragDrop.DraggingCard, gameObject))
                type = UIManager.SelectionType.Selected;
            else type = UIManager.SelectionType.Rejected;

            ManagerHandler.U_MAN.SelectTarget(gameObject, type);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard == gameObject) return;

        if (ManagerHandler.U_MAN.PlayerIsTargetting) ManagerHandler.EF_MAN.HighlightEffectTarget(gameObject, false);
        else if (DragDrop.ArrowIsDragging)
        {
            if (ManagerHandler.EN_MAN.HandZoneCards.Contains(gameObject)) return;

            DragDrop.Enemy = null;
            UIManager.SelectionType type;

            if (ManagerHandler.CO_MAN.CanAttack(DragDrop.DraggingCard, gameObject))
                type = UIManager.SelectionType.Highlighted;
            else type = UIManager.SelectionType.Disabled;

            ManagerHandler.U_MAN.SelectTarget(gameObject, type);
        }
    }
}
