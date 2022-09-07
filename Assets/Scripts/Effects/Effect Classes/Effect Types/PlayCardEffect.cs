using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Play Card Effect", menuName = "Effects/Effect/PlayCard")]
public class PlayCardEffect : Effect
{
    [Header("PLAY CARD EFFECT")]

    [SerializeField, Tooltip("If enabled, the enemy hero plays the card")] private bool enemyCard;
    [SerializeField, Tooltip("The card to play")] private Card playedCard;
    [SerializeField, Tooltip("The type of card to play")] private string playedCardType;
    [SerializeField] private List<Effect> additionalEffects;

    public bool EnemyCard { get => enemyCard; }
    public Card PlayedCard { get => playedCard; }
    public string PlayedCardType { get => playedCardType; }
    public List<Effect> AdditionalEffects { get => additionalEffects; }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        PlayCardEffect playCardEffect = effect as PlayCardEffect;
        enemyCard = playCardEffect.enemyCard;
        playedCard = playCardEffect.PlayedCard;
        playedCardType = playCardEffect.PlayedCardType;
        additionalEffects = playCardEffect.AdditionalEffects;
    }
}
