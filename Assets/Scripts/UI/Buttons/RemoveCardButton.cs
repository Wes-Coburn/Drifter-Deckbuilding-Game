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
            removalCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    public Card Card
    {
        get => card;
        set
        {
            card = value;
            RemovalCost = GameManager.Instance.GetRemoveCardCost(card);
        }
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().RemoveCardButton_OnClick(Card);
}
