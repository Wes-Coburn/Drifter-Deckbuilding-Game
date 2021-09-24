using UnityEngine;
using TMPro;

public class RemoveCardPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private Card card;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private void Awake()
    {
        uMan = UIManager.Instance;
        pMan = PlayerManager.Instance;
    }

    public Card Card
    {
        set
        {
            int aether = pMan.AetherCells;
            card = value;
            string text = "Remove " + card.CardName +
                " for 1 aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.RemovePlayerCard(card);
        pMan.AetherCells -= 1;
        CancelButton_OnClick();
        uMan.DestroyCardPagePopup(); // Temporary fix, eventually reload the page
        // Card removed popup
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyRemoveCardPopup();
}