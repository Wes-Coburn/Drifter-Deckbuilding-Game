using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Create Card Effect", menuName = "Effects/Effect/CreateCard")]
public class CreateCardEffect : Effect
{
    [Header("CREATE CARD EFFECT")]

    [SerializeField, Tooltip("The card to create")] private Card createdCard;
    [SerializeField, Tooltip("The type of card to create")] private string createdCardType;
    [SerializeField] private List<Effect> additionalEffects;

    [Header("CREATED CARD PARAMETERS"), SerializeField] private bool restrictCost;
    [SerializeField, Range(0,10)] private int minCost;
    [SerializeField, Range(0, 10)] private int maxCost;

    [SerializeField, Space] private bool restrictType;
    [SerializeField] private bool includeUnits;
    [SerializeField] private bool includeActions;

    [SerializeField, Space] private bool restrictSubtype;
    [SerializeField] private string cardSubtype;

    [SerializeField] private bool excludeSelf;

    public Card CreatedCard { get => createdCard; }
    public string CreatedCardType { get => createdCardType; }
    public List<Effect> AdditionalEffects { get => additionalEffects; }
    public bool RestrictCost { get => restrictCost; }
    public int MinCost { get => minCost; }
    public int MaxCost { get => maxCost; }
    public bool RestrictType { get => restrictType; }
    public bool IncludeUnits { get => includeUnits; }
    public bool IncludeActions { get => includeActions; }
    public bool RestrictSubtype { get => restrictSubtype; }
    public string CardSubtype { get => cardSubtype; }
    public bool ExcludeSelf { get => excludeSelf; }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        CreateCardEffect createCardEffect = effect as CreateCardEffect;
        createdCard = createCardEffect.CreatedCard;
        createdCardType = createCardEffect.CreatedCardType;
        additionalEffects = createCardEffect.AdditionalEffects;
        restrictCost = createCardEffect.RestrictCost;
        minCost = createCardEffect.MinCost;
        maxCost = createCardEffect.MaxCost;
        restrictType = createCardEffect.RestrictType;
        includeUnits = createCardEffect.IncludeUnits;
        includeActions = createCardEffect.IncludeActions;
        restrictSubtype = createCardEffect.RestrictSubtype;
        cardSubtype = createCardEffect.CardSubtype;
        excludeSelf = createCardEffect.ExcludeSelf;
    }
}
