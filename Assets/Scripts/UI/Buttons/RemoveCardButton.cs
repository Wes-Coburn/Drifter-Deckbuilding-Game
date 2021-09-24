using UnityEngine;

public class RemoveCardButton : MonoBehaviour
{
    public Card Card { get; set; }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().RemoveCardButton_OnClick(Card);
}
