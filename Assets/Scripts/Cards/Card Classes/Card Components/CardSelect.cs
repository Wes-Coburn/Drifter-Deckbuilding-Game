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
        if (uMan.PlayerIsTargetting) efMan.SelectTarget(gameObject);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!DragDrop.ArrowIsDragging || DragDrop.DraggingCard == gameObject) return;
        DragDrop.Enemy = gameObject;
        if (coMan.CanAttack(DragDrop.DraggingCard, gameObject, true))
            uMan.SelectTarget(gameObject, true, true);
        else uMan.SelectTarget(gameObject, true, false, true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (!DragDrop.ArrowIsDragging || DragDrop.DraggingCard == gameObject) return;
        DragDrop.Enemy = null;
        if (coMan.CanAttack(DragDrop.DraggingCard, gameObject, true))
            uMan.SelectTarget(gameObject, true);
        else uMan.SelectTarget(gameObject, false);
    }
}
