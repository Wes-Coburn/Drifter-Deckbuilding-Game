using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Create Card Effect", menuName = "Effects/Effect/CreateCard")]
public class CreateCardEffect : Effect
{
    [Header("CREATE CARD EFFECT")]

    [SerializeField] [Tooltip("The card to create")] private Card createdCard;
    [SerializeField] [Tooltip("The type of card to create")] private string createdCardType;
    [SerializeField] private List<Effect> additionalEffects;

    public Card CreatedCard { get => createdCard; }
    public string CreatedCardType { get => createdCardType; }
    public List<Effect> AdditionalEffects { get => additionalEffects; }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        CreateCardEffect createCardEffect = effect as CreateCardEffect;
        createdCard = createCardEffect.CreatedCard;
        createdCardType = createCardEffect.CreatedCardType;
        additionalEffects = createCardEffect.AdditionalEffects;
    }
}
