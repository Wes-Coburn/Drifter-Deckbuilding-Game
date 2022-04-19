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
            int aether = pMan.AetherCells;
            card = value;
            string text = "Remove " + card.CardName +
                " for " + gMan.GetRemoveCardCost(card) +
                " aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.RemovePlayerCard(card);
        pMan.AetherCells -= gMan.GetRemoveCardCost(card);
        CardPageDisplay.CardPageType type;
        if (card is SkillCard) type = CardPageDisplay.CardPageType.RemoveSkillCard;
        else type = CardPageDisplay.CardPageType.RemoveMainCard;
        uMan.CreateCardPagePopup(type, false);
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyRemoveCardPopup();
}