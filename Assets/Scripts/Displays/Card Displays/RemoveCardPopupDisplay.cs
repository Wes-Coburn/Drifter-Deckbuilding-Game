using UnityEngine;
using TMPro;

public class RemoveCardPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private GameManager gMan;
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
        gMan = GameManager.Instance;
    }

    public Card Card
    {
        set
        {
            card = value;
            int cost;
            if (card.IsRare) cost = GameManager.REMOVE_RARE_CARD_COST;
            else cost = GameManager.REMOVE_CARD_COST;

            string text = "Sell " + card.CardName +
                " for " + cost + " aether?";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.RemovePlayerCard(card);
        int cost;
        if (card.IsRare) cost = GameManager.REMOVE_RARE_CARD_COST;
        else cost = GameManager.REMOVE_CARD_COST;
        pMan.AetherCells += cost;
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.RemoveCard, false);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyRemoveCardPopup();
}