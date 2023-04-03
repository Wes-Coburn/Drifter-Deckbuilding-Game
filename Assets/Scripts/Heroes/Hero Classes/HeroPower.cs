using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero Power", menuName = "Heroes/Player/Hero Power")]
public class HeroPower : ScriptableObject
{
    [Header("POWER NAME")] public string PowerName;
    [Header("POWER COST"), Range(0, GameManager.MAXIMUM_ENERGY)] public int PowerCost;
    [Header("POWER IMAGE")] public Sprite PowerSprite;
    [Header("POWER SOUND")] public Sound[] PowerSounds;
    [Header("POWER DESCRIPTION"), TextArea] public string PowerDescription;
    [Header("EFFECT GROUPS")] public List<EffectGroup> EffectGroupList;
    [Header("LINKED ABILITIES")] public List<CardAbility> LinkedAbilities;

    [Header("RELATED CARDS"), SerializeField] private List<Card> relatedCards;
    [Header("RELATED CARD TYPES"), SerializeField] private List<Card.CreatedCardType> relatedCardTypes;
    
    public List<Card> RelatedCards
    {
        get
        {
            List<Card> allRelatedCards = new();

            if (relatedCards != null) allRelatedCards.AddRange(relatedCards);

            if (relatedCardTypes != null)
            {
                foreach (var cardType in relatedCardTypes)
                {
                    var cardTypeCards = Managers.CA_MAN.GetCreatedCards(cardType, true);
                    if (cardTypeCards != null) allRelatedCards.AddRange(cardTypeCards);
                }
            }

            return allRelatedCards;
        }
    }
}
