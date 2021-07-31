using UnityEngine;

public class NewCardPopupDisplay : MonoBehaviour
{
    [SerializeField] private Card TestCard; // FOR TESTING ONLY
    private void Start()
    {
        CurrentCard = TestCard;
    }
    [SerializeField] private GameObject newCardZone;

    private Card currentCard;
    public Card CurrentCard
    {
        get => currentCard;
        set
        {
            currentCard = value;
            DisplayCard();
        }
    }

    private void DisplayCard()
    {
        GameObject newCard = CardManager.Instance.ShowCard(CurrentCard);
        newCard.transform.SetParent(newCardZone.transform, false);
        newCard.transform.localScale = new Vector2(5, 5);
    }
}
