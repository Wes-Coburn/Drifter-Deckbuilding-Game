using UnityEngine;
using TMPro;

public class RemoveCardButton : MonoBehaviour
{
    [SerializeField] private GameObject removalCost;

    private Card card;
    private int RemovalCost
    {
        set
        {
            removalCost.GetComponent<TextMeshProUGUI>().SetText("+" + value.ToString());
        }
    }
    public Card Card
    {
        get => card;
        set
        {
            card = value;
            int cost;
            if (card.IsRare) cost = GameManager.SELL_RARE_CARD_VALUE;
            else cost = GameManager.SELL_CARD_VALUE;
            RemovalCost = cost;
        }
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().RemoveCardButton_OnClick(Card);
}
