using UnityEngine;
using TMPro;

public class RemoveCardButton : MonoBehaviour
{
    [SerializeField] private GameObject removalCost;
    private int RemovalCost
    {
        set
        {
            removalCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    public Card Card { get; set; }
    private void Awake()
    {
        RemovalCost = GameManager.REMOVE_CARD_COST;
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().RemoveCardButton_OnClick(Card);
}
