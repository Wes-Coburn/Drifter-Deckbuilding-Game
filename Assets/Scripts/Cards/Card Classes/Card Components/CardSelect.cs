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
        if (!DragDrop.ArrowIsDragging || DragDrop.DraggingCard == gameObject) return;
        if (coMan.EnemyHandCards.Contains(gameObject)) return; // TESTING
        DragDrop.Enemy = gameObject;
        if (coMan.CanAttack(DragDrop.DraggingCard, gameObject, true))
            uMan.SelectTarget(gameObject, UIManager.SelectionType.Selected);
        else uMan.SelectTarget(gameObject, UIManager.SelectionType.Rejected);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (!DragDrop.ArrowIsDragging || DragDrop.DraggingCard == gameObject) return;
        if (coMan.EnemyHandCards.Contains(gameObject)) return; // TESTING
        DragDrop.Enemy = null;
        if (coMan.CanAttack(DragDrop.DraggingCard, gameObject, true))
            uMan.SelectTarget(gameObject, UIManager.SelectionType.Highlighted);
        else uMan.SelectTarget(gameObject, UIManager.SelectionType.Disabled);
    }
}
