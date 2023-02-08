using UnityEngine;
using UnityEngine.EventSystems;

public class ReputationIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameManager.ReputationType reputationType;

    public void OnPointerEnter(PointerEventData pointerEventData) =>
        Managers.U_MAN.CreateReputationPopup(reputationType, gameObject);

    public void OnPointerExit(PointerEventData pointerEventData) =>
        Managers.U_MAN.DestroyReputationPopup();

    private void OnDisable() => UIManager.Instance.DestroyReputationPopup(); // TESTING
}
