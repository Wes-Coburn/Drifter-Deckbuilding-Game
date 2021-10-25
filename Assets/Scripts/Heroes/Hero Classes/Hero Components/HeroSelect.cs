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
        if (UIManager.Instance.PlayerIsTargetting) efMan.SelectTarget(gameObject);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!DragDrop.ArrowIsDragging) return;
        DragDrop.Enemy = gameObject;
        if (coMan.CanAttack(DragDrop.DraggingCard, gameObject, true))
            uMan.SelectTarget(gameObject, true, true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (!DragDrop.ArrowIsDragging) return;
        DragDrop.Enemy = null;
        if (coMan.CanAttack(DragDrop.DraggingCard, gameObject, true))
            uMan.SelectTarget(gameObject, true);
    }
}
