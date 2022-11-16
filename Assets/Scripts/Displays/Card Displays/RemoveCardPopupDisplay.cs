using UnityEngine;
using TMPro;

public class RemoveCardPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private Card card;
    private int cost;

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
            card = value;

            switch (card.CardRarity)
            {
                case Card.Rarity.Common:
                    cost = GameManager.SELL_COMMON_CARD_VALUE;
                    break;
                case Card.Rarity.Rare:
                    cost = GameManager.SELL_RARE_CARD_VALUE;
                    break;
                case Card.Rarity.Legend:
                    cost = GameManager.SELL_LEGEND_CARD_VALUE;
                    break;
                default:
                    Debug.LogError("INVALID RARITY!");
                    return;
            }

            string text = "Sell <b><u>" + card.CardName + "</u></b>" +
                " for <color=\"yellow\"><b>" + cost + "</b></color> aether?";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.RemovePlayerCard(card);
        pMan.AetherCells += cost;
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.RemoveCard, false);

        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress); // TESTING
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyInteractablePopup(gameObject);
}