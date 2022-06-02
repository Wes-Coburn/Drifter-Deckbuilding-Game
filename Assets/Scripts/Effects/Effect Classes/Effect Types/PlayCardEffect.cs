using UnityEngine;

[CreateAssetMenu(fileName = "New Play Card Effect", menuName = "Effects/Effect/PlayCard")]
public class PlayCardEffect : Effect
{
    [Header("PLAY CARD EFFECT")]

    [SerializeField][Tooltip("The card to play")] private Card playedCard;
    [SerializeField][Tooltip("The type of card to play")] private string playedCardType;

    public Card PlayedCard { get => playedCard; }
    public string PlayedCardType { get => playedCardType; }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        PlayCardEffect playCardEffect = effect as PlayCardEffect;
        playedCard = playCardEffect.playedCard;
        playedCardType = playCardEffect.playedCardType;
    }
}
