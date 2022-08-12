using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSelect : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject heroOutline;

    private CombatManager coMan;
    private UIManager uMan;
    private EffectManager efMan;

    public GameObject HeroOutline { get => heroOutline; }

    private void Start()
    {
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        efMan = EffectManager.Instance;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (UIManager.Instance.PlayerIsTargetting) efMan.SelectEffectTarget(gameObject);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (uMan.PlayerIsTargetting) efMan.HighlightEffectTarget(gameObject, true);
        else if (DragDrop.ArrowIsDragging)
        {
            DragDrop.Enemy = gameObject;
            UIManager.SelectionType type;

            if (coMan.CanAttack(DragDrop.DraggingCard, gameObject, true))
                type = UIManager.SelectionType.Selected;
            else type = UIManager.SelectionType.Rejected;

            uMan.SelectTarget(gameObject, type);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (uMan.PlayerIsTargetting) efMan.HighlightEffectTarget(gameObject, false);
        else if (DragDrop.ArrowIsDragging)
        {
            DragDrop.Enemy = null;
            UIManager.SelectionType type;

            if (coMan.CanAttack(DragDrop.DraggingCard, gameObject, true))
                type = UIManager.SelectionType.Highlighted;
            else type = UIManager.SelectionType.Disabled;

            uMan.SelectTarget(gameObject, type);
        }
    }
}
