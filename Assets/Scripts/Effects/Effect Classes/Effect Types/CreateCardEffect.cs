using UnityEngine;

[CreateAssetMenu(fileName = "New Create Card Effect", menuName = "Effects/Effect/Create Card")]
public class CreateCardEffect : Effect
{
    [SerializeField] [Tooltip("The card to create")] private Card createdCard;
    [SerializeField] [Tooltip("The type of card to create")] private string createdCardType;

    public Card CreatedCard { get => createdCard; }
    public string CreatedCardType { get => createdCardType; }
}
