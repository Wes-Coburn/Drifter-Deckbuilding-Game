using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSelect : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject heroOutline;

    public GameObject HeroOutline { get => heroOutline; }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (UIManager.Instance.PlayerIsTargetting) Managers.EF_MAN.SelectEffectTarget(gameObject);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (Managers.U_MAN.PlayerIsTargetting) Managers.EF_MAN.HighlightEffectTarget(gameObject, true);
        else if (DragDrop.ArrowIsDragging)
        {
            DragDrop.Enemy = gameObject;
            UIManager.SelectionType type;

            if (Managers.CO_MAN.CanAttack(DragDrop.DraggingCard, gameObject))
                type = UIManager.SelectionType.Selected;
            else type = UIManager.SelectionType.Rejected;

            Managers.U_MAN.SelectTarget(gameObject, type);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (Managers.U_MAN.PlayerIsTargetting) Managers.EF_MAN.HighlightEffectTarget(gameObject, false);
        else if (DragDrop.ArrowIsDragging)
        {
            DragDrop.Enemy = null;
            UIManager.SelectionType type;

            if (Managers.CO_MAN.CanAttack(DragDrop.DraggingCard, gameObject))
                type = UIManager.SelectionType.Highlighted;
            else type = UIManager.SelectionType.Disabled;

            Managers.U_MAN.SelectTarget(gameObject, type);
        }
    }
}
