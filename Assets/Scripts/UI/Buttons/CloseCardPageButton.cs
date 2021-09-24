using UnityEngine;
using UnityEngine.EventSystems;

public class CloseCardPageButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        UIManager uMan = UIManager.Instance;
        uMan.DestroyRemoveCardPopup();
        uMan.DestroyLearnSkillPopup();
        uMan.DestroyCardPagePopup();
    }
}
