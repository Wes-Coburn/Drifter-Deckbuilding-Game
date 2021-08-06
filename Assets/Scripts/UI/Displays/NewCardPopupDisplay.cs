using UnityEngine;

public class NewCardPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject newCardZone;
    [SerializeField] private GameObject newCardChest;
    [SerializeField] private GameObject addCardButton;
    [SerializeField] private GameObject ignoreCardButton;
    [SerializeField] private GameObject toolip;

    private Card currentCard;
    public Card CurrentCard
    {
        get => currentCard;
        set
        {
            currentCard = value;
            DisplayNewCardChest();
        }
    }

    private void DisplayNewCardChest()
    {
        gameObject.GetComponent<SoundPlayer>().PlaySound(0);
        newCardChest.SetActive(true);
        addCardButton.SetActive(false);
        ignoreCardButton.SetActive(false);
        toolip.SetActive(false);
    }

    public void DisplayNewCard()
    {
        newCardChest.SetActive(false);
        addCardButton.SetActive(true);
        ignoreCardButton.SetActive(true);
        toolip.SetActive(true);

        GameObject newCard = CardManager.Instance.ShowCard(CurrentCard);
        newCard.transform.SetParent(newCardZone.transform, false);
        AnimationManager.Instance.RevealedHandState(newCard);
         if (newCard.TryGetComponent<UnitCardDisplay>(out UnitCardDisplay ucd)) ucd.IsExhausted = false;
    }

    public void AddCard()
    {
        CardManager.Instance.DestroyNewCardPopup();
    }

    public void IgnoreCard()
    {
        PlayerManager.Instance.PlayerDeckList.Remove(CurrentCard);
        CardManager.Instance.DestroyNewCardPopup();
    }
}
