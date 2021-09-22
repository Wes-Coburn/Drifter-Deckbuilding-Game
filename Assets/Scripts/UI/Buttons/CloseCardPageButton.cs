using UnityEngine;
using UnityEngine.EventSystems;

public class CloseCardPageButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        FindObjectOfType<CardPageDisplay>().DestroyLearnSkillPopup();
        UIManager.Instance.DestroyCardPagePopup();
    }
}
