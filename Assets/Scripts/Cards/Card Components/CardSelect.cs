using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelect : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject cardOutline;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color highlightedColor;
    public GameObject CardOutline { get => cardOutline; }
    public Color SelectedColor { get => selectedColor; }
    public Color HighlightedColor { get => highlightedColor; }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        if (UIManager.Instance.PlayerIsTargetting)
            EffectManager.Instance.SelectTarget(gameObject);
    }
}
