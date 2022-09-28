using UnityEngine;
using UnityEngine.EventSystems;

public class ReputationIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameManager.ReputationType reputationType;

    private UIManager uMan;
    private void Start() => uMan = UIManager.Instance;

    public void OnPointerEnter(PointerEventData pointerEventData) =>
        uMan.CreateReputationPopup(reputationType, gameObject);

    public void OnPointerExit(PointerEventData pointerEventData) =>
        uMan.DestroyReputationPopup();

    private void OnDisable() => UIManager.Instance.DestroyReputationPopup(); // TESTING
}
