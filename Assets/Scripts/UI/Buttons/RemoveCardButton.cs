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
            if (card.IsRare) cost = GameManager.REMOVE_RARE_CARD_COST;
            else cost = GameManager.REMOVE_CARD_COST;
            RemovalCost = cost;
        }
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().RemoveCardButton_OnClick(Card);
}
