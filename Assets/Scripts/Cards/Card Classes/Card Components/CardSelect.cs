using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelect : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject cardOutline;

    private CombatManager coMan;
    private UIManager uMan;
    private EffectManager efMan;

    public GameObject CardOutline { get => cardOutline; }

    private void Start()
    {
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        efMan = EffectManager.Instance;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (uMan.PlayerIsTargetting) efMan.SelectEffectTarget(gameObject);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard == gameObject) return;

        if (uMan.PlayerIsTargetting) efMan.HighlightEffectTarget(gameObject, true);
        else if (DragDrop.ArrowIsDragging)
        {
            if (coMan.EnemyHandCards.Contains(gameObject)) return;

            DragDrop.Enemy = gameObject;
            UIManager.SelectionType type;

            if (coMan.CanAttack(DragDrop.DraggingCard, gameObject))
                type = UIManager.SelectionType.Selected;
            else type = UIManager.SelectionType.Rejected;

            uMan.SelectTarget(gameObject, type);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (DragDrop.DraggingCard == gameObject) return;

        if (uMan.PlayerIsTargetting) efMan.HighlightEffectTarget(gameObject, false);
        else if (DragDrop.ArrowIsDragging)
        {
            if (coMan.EnemyHandCards.Contains(gameObject)) return;

            DragDrop.Enemy = null;
            UIManager.SelectionType type;

            if (coMan.CanAttack(DragDrop.DraggingCard, gameObject))
                type = UIManager.SelectionType.Highlighted;
            else type = UIManager.SelectionType.Disabled;

            uMan.SelectTarget(gameObject, type);
        }
    }
}
