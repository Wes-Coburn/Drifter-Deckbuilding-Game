using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour, IPointerClickHandler
{
    Button button;
    public bool IsInteractable { get => button.interactable; }
    private void Awake() => button = GetComponent<Button>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) UserClick();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button != PointerEventData.InputButton.Left) return;
        UserClick();
    }

    private void UserClick()
    {
        if (Managers.EV_MAN.ActionsDelayed) return;
        if (!button.interactable) return;

        Managers.CO_MAN.EndCombatTurn(Managers.P_MAN);
        GetComponentInParent<SoundPlayer>().PlaySound(0);
    }


}
