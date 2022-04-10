using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPopupButton : MonoBehaviour, IPointerClickHandler
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) UserClick();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        UserClick();
    }

    private void UserClick()
    {
        UIManager.Instance.CreateMenuPopup();
    }
}
